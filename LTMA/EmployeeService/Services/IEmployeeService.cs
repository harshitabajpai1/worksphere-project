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

        Task<List<Employee>>
            GetEmployeesByDepartmentAsync(
                int departmentId);
    }
}