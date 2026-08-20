using LeaveService.Data;
using LeaveService.DTOs;
using LeaveService.Models;
using LeaveService.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.Net;
using System.Net.Http.Json;

namespace LeaveService.Tests
{
    // Tests the basic leave request features of LeaveService.
    public class LeaveServiceTests
    {
        private LeaveDbContext _context = null!;
        private LeaveService.Services.LeaveService _service = null!;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<LeaveDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new LeaveDbContext(options);
            _service = CreateService();
        }

        // Checks that a valid leave request is created as Pending.
        [Test]
        public async Task ApplyLeaveAsync_ValidData_CreatesPendingLeave()
        {
            var request = new ApplyLeaveDto
            {
                StartDate = new DateTime(2026, 9, 1),
                EndDate = new DateTime(2026, 9, 3),
                Reason = "Family function"
            };

            var result = await _service.ApplyLeaveAsync(request);

            Assert.That(result.EmployeeId, Is.EqualTo(2));
            Assert.That(result.Status, Is.EqualTo("Pending"));
        }

        // Checks that the start date cannot be after the end date.
        [Test]
        public void ApplyLeaveAsync_InvalidDates_ThrowsException()
        {
            var request = new ApplyLeaveDto
            {
                StartDate = new DateTime(2026, 9, 3),
                EndDate = new DateTime(2026, 9, 1),
                Reason = "Family function"
            };

            var exception = Assert.ThrowsAsync<Exception>(async () =>
                await _service.ApplyLeaveAsync(request));

            Assert.That(exception!.Message, Is.EqualTo("Start date cannot be after end date."));
        }

        // Checks that all saved leave requests are returned.
        [Test]
        public async Task GetAllLeavesAsync_LeavesExist_ReturnsLeaves()
        {
            _context.Leaves.Add(new Leave
            {
                EmployeeId = 2,
                StartDate = new DateTime(2026, 9, 1),
                EndDate = new DateTime(2026, 9, 3),
                Reason = "Family function",
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();

            var result = await _service.GetAllLeavesAsync();

            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].Status, Is.EqualTo("Pending"));
        }

        private LeaveService.Services.LeaveService CreateService()
        {
            var handler = new TestEmployeeHandler();
            var client = new HttpClient(handler)
            {
                BaseAddress = new Uri("https://localhost/")
            };
            var factory = new TestHttpClientFactory(client);
            var accessor = new HttpContextAccessor
            {
                HttpContext = new DefaultHttpContext()
            };
            accessor.HttpContext.Request.Headers.Authorization = "Bearer test-token";

            return new LeaveService.Services.LeaveService(_context, factory, accessor);
        }

        // Returns simple EmployeeService data for the tests.
        private class TestEmployeeHandler : HttpMessageHandler
        {
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                var employee = new EmployeeInfoDto
                {
                    Id = 2,
                    Email = "amit@worksphere.com",
                    Designation = "Employee",
                    IsActive = true
                };

                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = JsonContent.Create(employee)
                });
            }
        }

        private class TestHttpClientFactory : IHttpClientFactory
        {
            private readonly HttpClient _client;

            public TestHttpClientFactory(HttpClient client)
            {
                _client = client;
            }

            public HttpClient CreateClient(string name)
            {
                return _client;
            }
        }
    }
}
