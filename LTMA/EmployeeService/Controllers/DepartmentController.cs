using EmployeeService.DTOs;
using EmployeeService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "SuperAdmin")]
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentService _service;

        public DepartmentController(
            IDepartmentService service)
        {
            _service = service;
        }

        // POST: /api/Department
        // Only SuperAdmin can create departments.
        [HttpPost]
        public async Task<IActionResult> Create(
            CreateDepartmentDto request)
        {
            var department =
                await _service.CreateDepartmentAsync(
                    request);

            return Ok(department);
        }

        // GET: /api/Department
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var departments =
                await _service.GetDepartmentsAsync();

            return Ok(departments);
        }

        // GET: /api/Department/1
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(
            int id)
        {
            var department =
                await _service.GetDepartmentByIdAsync(id);

            if (department == null)
            {
                return NotFound(
                    "Department not found.");
            }

            return Ok(department);
        }
    }
}