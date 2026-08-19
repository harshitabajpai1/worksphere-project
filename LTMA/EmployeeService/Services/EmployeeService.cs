using EmployeeService.Data;
using EmployeeService.DTOs;
using EmployeeService.Models;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace EmployeeService.Services
{
    // Contains the business logic related to employees and managers.
    public class EmployeeService : IEmployeeService
    {
        private readonly EmployeeDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EmployeeService(
            EmployeeDbContext context,
            IHttpClientFactory httpClientFactory,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        // Creates either an Employee or a Manager.
        public async Task<Employee> RegisterEmployeeAsync(CreateEmployeeDto request)
        {
            // First check whether the department exists.
            var department = await _context.Departments.FirstOrDefaultAsync(d => d.Id == request.DepartmentId);

            if (department == null)
            {
                throw new Exception("Department not found.");
            }

            // Check whether this email is already registered.
            var existingEmployee = await _context.Employees.FirstOrDefaultAsync(e => e.Email == request.Email);

            if (existingEmployee != null)
            {
                throw new Exception("Employee with this email already exists.");
            }

            // Make sure the designation is valid.
            if (request.Designation != "Employee" && request.Designation != "Manager")
            {
                throw new Exception("Designation must be Employee or Manager.");
            }

            if (string.IsNullOrWhiteSpace(request.InitialPassword))
            {
                throw new Exception("Initial password is required.");
            }

            // If we are creating a Manager,
            // check whether the department already has one.
            if (request.Designation == "Manager")
            {
                var existingManager = await _context.Employees.AnyAsync(e =>
                    e.DepartmentId == request.DepartmentId &&
                    e.Designation == "Manager" &&
                    e.IsActive);

                if (existingManager)
                {
                    throw new Exception("This department already has a manager.");
                }
            }

            var employee = new Employee
            {
                Name = request.Name,
                Email = request.Email,
                DepartmentId = request.DepartmentId,
                Designation = request.Designation,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            // If this is a normal employee,
            // automatically find the manager of the department.
            if (request.Designation == "Employee")
            {
                var manager = await _context.Employees.FirstOrDefaultAsync(e =>
                    e.DepartmentId == request.DepartmentId &&
                    e.Designation == "Manager" &&
                    e.IsActive);

                if (manager == null)
                {
                    throw new Exception("No manager exists for this department.");
                }

                // Automatically assign the employee
                // to that department's manager.
                employee.ManagerId = manager.Id;
            }

            // Add the employee to the database.
            _context.Employees.Add(employee);

            // Save the employee.
            await _context.SaveChangesAsync();

            // Ask AuthService to create the corresponding authentication
            // user. Password hashing remains owned by AuthService.
            var authorizationHeader = _httpContextAccessor.HttpContext?
                .Request.Headers.Authorization.ToString();

            if (string.IsNullOrWhiteSpace(authorizationHeader))
            {
                throw new Exception("Authorization token is required.");
            }

            var authClient = _httpClientFactory.CreateClient("AuthService");
            authClient.DefaultRequestHeaders.Authorization =
                AuthenticationHeaderValue.Parse(authorizationHeader);

            var authResponse = await authClient.PostAsJsonAsync(
                "api/Auth/users",
                new
                {
                    employee.Name,
                    employee.Email,
                    Role = employee.Designation,
                    Password = request.InitialPassword
                });

            if (!authResponse.IsSuccessStatusCode)
            {
                var error = await authResponse.Content.ReadAsStringAsync();
                throw new Exception(
                    $"AuthService could not create the user: {error}");
            }

            // Load the department navigation property.
            await _context.Entry(employee).Reference(e => e.Department).LoadAsync();

            // Load the manager navigation property if the employee has a manager.
            if (employee.ManagerId.HasValue)
            {
                await _context.Entry(employee).Reference(e => e.Manager).LoadAsync();
            }

            return employee;
        }

        // Returns all employees.
        public async Task<List<Employee>> GetEmployeesAsync()
        {
            return await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Manager)
                .ToListAsync();
        }

        // Returns one employee.
        public async Task<Employee?> GetEmployeeByIdAsync(int id)
        {
            return await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Manager)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        // Returns one employee using their email address.
        public async Task<Employee?> GetEmployeeByEmailAsync(string email)
        {
            return await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Manager)
                .FirstOrDefaultAsync(e => e.Email == email);
        }

        // Returns all employees belonging to a particular department.
        public async Task<List<Employee>> GetEmployeesByDepartmentAsync(int departmentId)
        {
            return await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Manager)
                .Where(e => e.DepartmentId == departmentId)
                .ToListAsync();
        }

        // Returns employees reporting to a manager.
        public async Task<List<Employee>> GetEmployeesByManagerIdAsync(int managerId)
        {
            return await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Manager)
                .Where(e => e.ManagerId == managerId)
                .ToListAsync();
        }
    }
}
