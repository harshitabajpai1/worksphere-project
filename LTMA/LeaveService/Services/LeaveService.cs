using LeaveService.Data;
using LeaveService.DTOs;
using LeaveService.Models;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace LeaveService.Services
{
    // Contains the business logic related to leave requests.
    public class LeaveService : ILeaveService
    {
        private readonly LeaveDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LeaveService(LeaveDbContext context, IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        // Creates a new leave request for the logged-in employee.
        public async Task<LeaveResponseDto> ApplyLeaveAsync(ApplyLeaveDto request)
        {
            if (request.StartDate > request.EndDate)
            {
                throw new Exception("Start date cannot be after end date.");
            }

            if (string.IsNullOrWhiteSpace(request.Reason))
            {
                throw new Exception("Reason is required.");
            }

            var employee = await GetCurrentEmployeeAsync();

            if (!employee.IsActive)
            {
                throw new Exception("Employee is not active.");
            }

            // Check whether the employee already has
            // an overlapping pending or approved leave.
            var hasOverlappingLeave = await _context.Leaves.AnyAsync(leave =>
                leave.EmployeeId == employee.Id &&
                (leave.Status == "Pending" || leave.Status == "Approved") &&
                request.StartDate <= leave.EndDate &&
                request.EndDate >= leave.StartDate);

            if (hasOverlappingLeave)
            {
                throw new Exception("Leave dates overlap with an existing leave request.");
            }

            var leave = new Leave
            {
                EmployeeId = employee.Id,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Reason = request.Reason,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };

            _context.Leaves.Add(leave);
            await _context.SaveChangesAsync();

            return MapLeave(leave);
        }

        // Returns leave requests for the logged-in employee.
        public async Task<List<LeaveResponseDto>> GetMyLeavesAsync()
        {
            var employee = await GetCurrentEmployeeAsync();

            var leaves = await _context.Leaves
                .Where(leave => leave.EmployeeId == employee.Id)
                .ToListAsync();

            return leaves.Select(leave => MapLeave(leave)).ToList();
        }

        // Returns leave requests for employees reporting to the logged-in manager.
        public async Task<List<LeaveResponseDto>> GetTeamLeavesAsync()
        {
            var teamEmployees = await GetTeamEmployeesAsync();
            var employeeIds = teamEmployees.Select(employee => employee.Id).ToList();

            var leaves = await _context.Leaves
                .Where(leave => employeeIds.Contains(leave.EmployeeId))
                .ToListAsync();

            return leaves.Select(leave => MapLeave(leave)).ToList();
        }

        // Approves a pending leave request for the logged-in manager's team.
        public async Task<LeaveResponseDto> ApproveLeaveAsync(int id)
        {
            return await ChangeLeaveStatusAsync(id, "Approved");
        }

        // Rejects a pending leave request for the logged-in manager's team.
        public async Task<LeaveResponseDto> RejectLeaveAsync(int id)
        {
            return await ChangeLeaveStatusAsync(id, "Rejected");
        }

        // Returns all leave requests for SuperAdmin.
        public async Task<List<LeaveResponseDto>> GetAllLeavesAsync()
        {
            var leaves = await _context.Leaves.ToListAsync();

            return leaves.Select(leave => MapLeave(leave)).ToList();
        }

        // Changes the status of a leave request after checking the manager's team.
        private async Task<LeaveResponseDto> ChangeLeaveStatusAsync(int id, string status)
        {
            var leave = await _context.Leaves.FirstOrDefaultAsync(item => item.Id == id);

            if (leave == null)
            {
                throw new Exception("Leave request not found.");
            }

            if (leave.Status != "Pending")
            {
                throw new Exception($"Only pending leave requests can be {status.ToLower()}.");
            }

            var teamEmployees = await GetTeamEmployeesAsync();
            var isTeamEmployee = teamEmployees.Any(employee => employee.Id == leave.EmployeeId);

            if (!isTeamEmployee)
            {
                throw new Exception("You are not allowed to approve this leave request.");
            }

            leave.Status = status;
            await _context.SaveChangesAsync();

            return MapLeave(leave);
        }

        // Gets the logged-in employee from EmployeeService.
        private async Task<EmployeeInfoDto> GetCurrentEmployeeAsync()
        {
            var client = CreateEmployeeServiceClient();
            var response = await client.GetAsync("api/Employee/me");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Employee not found.");
            }

            var employee = await response.Content.ReadFromJsonAsync<EmployeeInfoDto>();

            if (employee == null)
            {
                throw new Exception("Employee not found.");
            }

            return employee;
        }

        // Gets the logged-in manager's team from EmployeeService.
        private async Task<List<EmployeeInfoDto>> GetTeamEmployeesAsync()
        {
            var client = CreateEmployeeServiceClient();
            var response = await client.GetAsync("api/Employee/team");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Employee not found.");
            }

            var employees = await response.Content.ReadFromJsonAsync<List<EmployeeInfoDto>>();

            return employees ?? new List<EmployeeInfoDto>();
        }

        // Creates a client that forwards the logged-in user's JWT.
        private HttpClient CreateEmployeeServiceClient()
        {
            var authorizationHeader = _httpContextAccessor.HttpContext?
                .Request.Headers.Authorization.ToString();

            if (string.IsNullOrWhiteSpace(authorizationHeader))
            {
                throw new Exception("Authorization token is required.");
            }

            var client = _httpClientFactory.CreateClient("EmployeeService");
            client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse(authorizationHeader);

            return client;
        }

        // Converts a Leave entity into a response DTO.
        private static LeaveResponseDto MapLeave(Leave leave)
        {
            return new LeaveResponseDto
            {
                Id = leave.Id,
                EmployeeId = leave.EmployeeId,
                StartDate = leave.StartDate,
                EndDate = leave.EndDate,
                Reason = leave.Reason,
                Status = leave.Status,
                CreatedAt = leave.CreatedAt
            };
        }
    }
}
