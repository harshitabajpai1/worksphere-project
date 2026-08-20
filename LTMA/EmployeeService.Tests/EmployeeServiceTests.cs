using EmployeeService.Data;
using EmployeeService.DTOs;
using EmployeeService.Services;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace EmployeeService.Tests
{
    // Tests the basic department features of EmployeeService.
    public class EmployeeServiceTests
    {
        private EmployeeDbContext _context = null!;
        private DepartmentService _service = null!;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<EmployeeDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new EmployeeDbContext(options);
            _service = new DepartmentService(_context);
        }

        // Checks that a department is created with valid data.
        [Test]
        public async Task CreateDepartmentAsync_ValidData_CreatesDepartment()
        {
            var request = new CreateDepartmentDto
            {
                Name = "IT"
            };

            var result = await _service.CreateDepartmentAsync(request);

            Assert.That(result.Name, Is.EqualTo("IT"));
            Assert.That(await _context.Departments.CountAsync(), Is.EqualTo(1));
        }

        // Checks that duplicate department names are not allowed.
        [Test]
        public async Task CreateDepartmentAsync_DuplicateName_ThrowsException()
        {
            await _service.CreateDepartmentAsync(new CreateDepartmentDto
            {
                Name = "IT"
            });

            var exception = Assert.ThrowsAsync<Exception>(async () =>
                await _service.CreateDepartmentAsync(new CreateDepartmentDto
                {
                    Name = "IT"
                }));

            Assert.That(exception!.Message, Is.EqualTo("Department already exists."));
        }

        // Checks that a department can be found using its ID.
        [Test]
        public async Task GetDepartmentByIdAsync_ExistingId_ReturnsDepartment()
        {
            var createdDepartment = await _service.CreateDepartmentAsync(new CreateDepartmentDto
            {
                Name = "IT"
            });

            var result = await _service.GetDepartmentByIdAsync(createdDepartment.Id);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Name, Is.EqualTo("IT"));
        }
    }
}
