using EmployeeService.Data;
using EmployeeService.DTOs;
using EmployeeService.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeService.Services
{
    // Contains the business logic for departments.
    public class DepartmentService : IDepartmentService
    {
        private readonly EmployeeDbContext _context;

        public DepartmentService(EmployeeDbContext context)
        {
            _context = context;
        }

        // Creates a new department.
        public async Task<DepartmentResponseDto> CreateDepartmentAsync(
            CreateDepartmentDto request)
        {
            // Check whether a department with the
            // same name already exists.
            var existingDepartment =
                await _context.Departments
                    .FirstOrDefaultAsync(d =>
                        d.Name == request.Name);

            if (existingDepartment != null)
            {
                throw new Exception(
                    "Department already exists.");
            }

            var department = new Department
            {
                Name = request.Name
            };

            _context.Departments.Add(department);

            await _context.SaveChangesAsync();

            return new DepartmentResponseDto
            {
                Id = department.Id,
                Name = department.Name
            };
        }

        // Returns all departments.
        public async Task<List<DepartmentResponseDto>>
            GetDepartmentsAsync()
        {
            var departments = await _context.Departments
                .ToListAsync();

            return departments.Select(department =>
                new DepartmentResponseDto
                {
                    Id = department.Id,
                    Name = department.Name
                }).ToList();
        }

        // Returns one department using its ID.
        public async Task<DepartmentResponseDto?>
            GetDepartmentByIdAsync(int id)
        {
            var department = await _context.Departments
                .FirstOrDefaultAsync(d =>
                    d.Id == id);

            if (department == null)
            {
                return null;
            }

            return new DepartmentResponseDto
            {
                Id = department.Id,
                Name = department.Name
            };
        }
    }
}
