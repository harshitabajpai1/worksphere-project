using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using TimesheetService.Data;
using TimesheetService.DTOs;
using TimesheetService.Models;

namespace TimesheetService.Services
{
    // Contains the business logic for timesheets.
    public class TimesheetService : ITimesheetService
    {
        private readonly TimesheetDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TimesheetService(TimesheetDbContext context, IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        // Creates a timesheet for the logged-in employee.
        public async Task<TimesheetResponseDto> CreateTimesheetAsync(CreateTimesheetDto request)
        {
            if (request.HoursWorked <= 0)
            {
                throw new Exception("Hours worked must be greater than zero.");
            }

            if (string.IsNullOrWhiteSpace(request.WorkDescription))
            {
                throw new Exception("Work description is required.");
            }

            var employee = await GetCurrentEmployeeAsync();

            if (!employee.IsActive)
            {
                throw new Exception("Employee is not active.");
            }

            var timesheet = new Timesheet
            {
                EmployeeId = employee.Id,
                Date = request.Date,
                HoursWorked = request.HoursWorked,
                WorkDescription = request.WorkDescription,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };

            _context.Timesheets.Add(timesheet);
            await _context.SaveChangesAsync();

            return MapTimesheet(timesheet);
        }

        // Returns timesheets for the logged-in employee.
        public async Task<List<TimesheetResponseDto>> GetMyTimesheetsAsync()
        {
            var employee = await GetCurrentEmployeeAsync();

            var timesheets = await _context.Timesheets
                .Where(timesheet => timesheet.EmployeeId == employee.Id)
                .ToListAsync();

            return timesheets.Select(timesheet => MapTimesheet(timesheet)).ToList();
        }

        // Returns timesheets for the logged-in manager's team.
        public async Task<List<TimesheetResponseDto>> GetTeamTimesheetsAsync()
        {
            var teamEmployees = await GetTeamEmployeesAsync();
            var employeeIds = teamEmployees.Select(employee => employee.Id).ToList();

            var timesheets = await _context.Timesheets
                .Where(timesheet => employeeIds.Contains(timesheet.EmployeeId))
                .ToListAsync();

            return timesheets.Select(timesheet => MapTimesheet(timesheet)).ToList();
        }

        // Approves a pending timesheet for the logged-in manager's team.
        public async Task<TimesheetResponseDto> ApproveTimesheetAsync(int id)
        {
            return await ChangeTimesheetStatusAsync(id, "Approved");
        }

        // Rejects a pending timesheet for the logged-in manager's team.
        public async Task<TimesheetResponseDto> RejectTimesheetAsync(int id)
        {
            return await ChangeTimesheetStatusAsync(id, "Rejected");
        }

        // Returns all timesheets for SuperAdmin.
        public async Task<List<TimesheetResponseDto>> GetAllTimesheetsAsync()
        {
            var timesheets = await _context.Timesheets.ToListAsync();

            return timesheets.Select(timesheet => MapTimesheet(timesheet)).ToList();
        }

        // Changes the status after checking the manager's team.
        private async Task<TimesheetResponseDto> ChangeTimesheetStatusAsync(int id, string status)
        {
            var timesheet = await _context.Timesheets.FirstOrDefaultAsync(item => item.Id == id);

            if (timesheet == null)
            {
                throw new Exception("Timesheet not found.");
            }

            if (timesheet.Status != "Pending")
            {
                throw new Exception($"Only pending timesheets can be {status.ToLower()}.");
            }

            var teamEmployees = await GetTeamEmployeesAsync();
            var isTeamEmployee = teamEmployees.Any(employee => employee.Id == timesheet.EmployeeId);

            if (!isTeamEmployee)
            {
                throw new Exception("You are not allowed to approve this timesheet.");
            }

            var manager = await GetCurrentEmployeeAsync();

            timesheet.Status = status;

            if (status == "Approved")
            {
                timesheet.ApprovedAt = DateTime.UtcNow;
                timesheet.ApprovedBy = manager.Id;
            }

            await _context.SaveChangesAsync();

            return MapTimesheet(timesheet);
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

        // Converts a Timesheet entity into a response DTO.
        private static TimesheetResponseDto MapTimesheet(Timesheet timesheet)
        {
            return new TimesheetResponseDto
            {
                Id = timesheet.Id,
                EmployeeId = timesheet.EmployeeId,
                Date = timesheet.Date,
                HoursWorked = timesheet.HoursWorked,
                WorkDescription = timesheet.WorkDescription,
                Status = timesheet.Status,
                CreatedAt = timesheet.CreatedAt,
                ApprovedAt = timesheet.ApprovedAt,
                ApprovedBy = timesheet.ApprovedBy
            };
        }
    }
}
