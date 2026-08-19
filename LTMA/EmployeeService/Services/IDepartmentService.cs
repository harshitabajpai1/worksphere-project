using EmployeeService.DTOs;
using EmployeeService.Models;

namespace EmployeeService.Services
{
    // Defines the operations related to departments.
    public interface IDepartmentService
    {
        Task<Department> CreateDepartmentAsync(
            CreateDepartmentDto request);

        Task<List<Department>> GetDepartmentsAsync();

        Task<Department?> GetDepartmentByIdAsync(int id);
    }
}