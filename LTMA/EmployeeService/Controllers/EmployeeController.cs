using EmployeeService.DTOs;
using EmployeeService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "SuperAdmin")]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _service;

        public EmployeeController(IEmployeeService service)
        {
            _service = service;
        }

        // POST: /api/Employee/register
        // SuperAdmin uses this API to create either an Employee or Manager.
        [HttpPost("register")]
        public async Task<IActionResult> Register(CreateEmployeeDto request)
        {
            var employee = await _service.RegisterEmployeeAsync(request);

            var response = new EmployeeResponseDto
            {
                Id = employee.Id,
                Name = employee.Name,
                Email = employee.Email,
                Designation = employee.Designation,
                DepartmentId = employee.DepartmentId,
                DepartmentName = employee.Department?.Name ?? string.Empty,
                ManagerId = employee.ManagerId,
                ManagerName = employee.Manager?.Name,
                IsActive = employee.IsActive,
                CreatedAt = employee.CreatedAt
            };

            return Ok(response);
        }

        // GET: /api/Employee
        // Returns all employees using DTOs to avoid circular JSON references.
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var employees = await _service.GetEmployeesAsync();

            var response = employees.Select(employee => new EmployeeResponseDto
            {
                Id = employee.Id,
                Name = employee.Name,
                Email = employee.Email,
                Designation = employee.Designation,
                DepartmentId = employee.DepartmentId,
                DepartmentName = employee.Department?.Name ?? string.Empty,
                ManagerId = employee.ManagerId,
                ManagerName = employee.Manager?.Name,
                IsActive = employee.IsActive,
                CreatedAt = employee.CreatedAt
            }).ToList();

            return Ok(response);
        }

        // GET: /api/Employee/{id}
        // Returns one employee using a DTO.
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var employee = await _service.GetEmployeeByIdAsync(id);

            if (employee == null)
            {
                return NotFound("Employee not found.");
            }

            var response = new EmployeeResponseDto
            {
                Id = employee.Id,
                Name = employee.Name,
                Email = employee.Email,
                Designation = employee.Designation,
                DepartmentId = employee.DepartmentId,
                DepartmentName = employee.Department?.Name ?? string.Empty,
                ManagerId = employee.ManagerId,
                ManagerName = employee.Manager?.Name,
                IsActive = employee.IsActive,
                CreatedAt = employee.CreatedAt
            };

            return Ok(response);
        }

        // GET: /api/Employee/department/{departmentId}
        // Returns all employees belonging to a department using DTOs.
        [HttpGet("department/{departmentId}")]
        public async Task<IActionResult> GetByDepartment(int departmentId)
        {
            var employees = await _service.GetEmployeesByDepartmentAsync(departmentId);

            var response = employees.Select(employee => new EmployeeResponseDto
            {
                Id = employee.Id,
                Name = employee.Name,
                Email = employee.Email,
                Designation = employee.Designation,
                DepartmentId = employee.DepartmentId,
                DepartmentName = employee.Department?.Name ?? string.Empty,
                ManagerId = employee.ManagerId,
                ManagerName = employee.Manager?.Name,
                IsActive = employee.IsActive,
                CreatedAt = employee.CreatedAt
            }).ToList();

            return Ok(response);
        }
    }
}