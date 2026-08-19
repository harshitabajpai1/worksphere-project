using System.Security.Claims;
using EmployeeService.DTOs;
using EmployeeService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeService.Controllers
{
    // Returns employee information for the logged-in user.
    [ApiController]
    [Route("api/Employee")]
    [Authorize]
    public class EmployeeProfileController : ControllerBase
    {
        private readonly IEmployeeService _service;

        public EmployeeProfileController(IEmployeeService service)
        {
            _service = service;
        }

        // Returns the logged-in employee or manager.
        // GET: /api/Employee/me
        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentEmployee()
        {
            var emailClaim = User.FindFirst(ClaimTypes.Email);

            if (emailClaim == null)
            {
                return Unauthorized();
            }

            var employee = await _service.GetEmployeeByEmailAsync(emailClaim.Value);

            if (employee == null)
            {
                return NotFound("Employee not found.");
            }

            return Ok(MapEmployee(employee));
        }

        // Returns employees reporting to the logged-in manager.
        // GET: /api/Employee/team
        [HttpGet("team")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> GetTeam()
        {
            var emailClaim = User.FindFirst(ClaimTypes.Email);

            if (emailClaim == null)
            {
                return Unauthorized();
            }

            var manager = await _service.GetEmployeeByEmailAsync(emailClaim.Value);

            if (manager == null)
            {
                return NotFound("Employee not found.");
            }

            var employees = await _service.GetEmployeesByManagerIdAsync(manager.Id);
            var response = employees.Select(employee => MapEmployee(employee)).ToList();

            return Ok(response);
        }

        // Converts an Employee entity into a response DTO.
        private static EmployeeResponseDto MapEmployee(Models.Employee employee)
        {
            return new EmployeeResponseDto
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
        }
    }
}
