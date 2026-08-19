using EmployeeService.DTOs;
using EmployeeService.Models;

namespace EmployeeService.Services
{
    // Defines employee-related operations.
    public interface IEmployeeService
    {
        Task<Employee> RegisterEmployeeAsync(
            CreateEmployeeDto request);

        Task<List<Employee>> GetEmployeesAsync();

        Task<Employee?> GetEmployeeByIdAsync(int id);

        Task<Employee?> GetEmployeeByEmailAsync(string email);

        Task<List<Employee>>
            GetEmployeesByDepartmentAsync(
                int departmentId);

        Task<List<Employee>> GetEmployeesByManagerIdAsync(int managerId);
    }
}
