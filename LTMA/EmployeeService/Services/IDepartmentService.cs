using EmployeeService.DTOs;

namespace EmployeeService.Services
{
    // Defines the operations related to departments.
    public interface IDepartmentService
    {
        Task<DepartmentResponseDto> CreateDepartmentAsync(
            CreateDepartmentDto request);

        Task<List<DepartmentResponseDto>> GetDepartmentsAsync();

        Task<DepartmentResponseDto?> GetDepartmentByIdAsync(int id);
    }
}
