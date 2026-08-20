using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.Net;
using System.Net.Http.Json;
using TimesheetService.Data;
using TimesheetService.DTOs;
using TimesheetService.Models;
using TimesheetService.Services;

namespace TimesheetService.Tests
{
    // Tests the basic timesheet features of TimesheetService.
    public class TimesheetServiceTests
    {
        private TimesheetDbContext _context = null!;
        private TimesheetService.Services.TimesheetService _service = null!;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<TimesheetDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new TimesheetDbContext(options);
            _service = CreateService(false);
        }

        // Checks that a valid timesheet is created as Pending.
        [Test]
        public async Task CreateTimesheetAsync_ValidData_CreatesPendingTimesheet()
        {
            var request = new CreateTimesheetDto
            {
                Date = new DateTime(2026, 8, 19),
                HoursWorked = 8,
                WorkDescription = "Worked on employee module"
            };

            var result = await _service.CreateTimesheetAsync(request);

            Assert.That(result.EmployeeId, Is.EqualTo(2));
            Assert.That(result.Status, Is.EqualTo("Pending"));
        }

        // Checks that zero hours are not allowed.
        [Test]
        public void CreateTimesheetAsync_ZeroHours_ThrowsException()
        {
            var request = new CreateTimesheetDto
            {
                Date = new DateTime(2026, 8, 19),
                HoursWorked = 0,
                WorkDescription = "Worked on employee module"
            };

            var exception = Assert.ThrowsAsync<Exception>(async () =>
                await _service.CreateTimesheetAsync(request));

            Assert.That(exception!.Message, Is.EqualTo("Hours worked must be greater than zero."));
        }

        // Checks that a manager can approve a pending team timesheet.
        [Test]
        public async Task ApproveTimesheetAsync_PendingTeamTimesheet_ApprovesTimesheet()
        {
            _service = CreateService(true);

            _context.Timesheets.Add(new Timesheet
            {
                EmployeeId = 2,
                Date = new DateTime(2026, 8, 19),
                HoursWorked = 8,
                WorkDescription = "Worked on employee module",
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();

            var result = await _service.ApproveTimesheetAsync(1);

            Assert.That(result.Status, Is.EqualTo("Approved"));
            Assert.That(result.ApprovedBy, Is.EqualTo(1));
            Assert.That(result.ApprovedAt, Is.Not.Null);
        }

        private TimesheetService.Services.TimesheetService CreateService(bool isManager)
        {
            var handler = new TestEmployeeHandler(isManager);
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

            return new TimesheetService.Services.TimesheetService(_context, factory, accessor);
        }

        // Returns simple EmployeeService data for the tests.
        private class TestEmployeeHandler : HttpMessageHandler
        {
            private readonly bool _isManager;

            public TestEmployeeHandler(bool isManager)
            {
                _isManager = isManager;
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                if (request.RequestUri!.AbsolutePath.EndsWith("/team"))
                {
                    var teamEmployees = new List<EmployeeInfoDto>
                    {
                        new EmployeeInfoDto
                        {
                            Id = 2,
                            Email = "amit@worksphere.com",
                            Designation = "Employee",
                            ManagerId = 1,
                            IsActive = true
                        }
                    };

                    return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = JsonContent.Create(teamEmployees)
                    });
                }

                var employee = new EmployeeInfoDto
                {
                    Id = _isManager ? 1 : 2,
                    Email = _isManager ? "rahul@worksphere.com" : "amit@worksphere.com",
                    Designation = _isManager ? "Manager" : "Employee",
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
