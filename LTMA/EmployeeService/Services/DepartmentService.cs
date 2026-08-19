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
        public async Task<Department> CreateDepartmentAsync(
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

            return department;
        }

        // Returns all departments.
        public async Task<List<Department>>
            GetDepartmentsAsync()
        {
            return await _context.Departments
                .Include(d => d.Employees)
                .ToListAsync();
        }

        // Returns one department using its ID.
        public async Task<Department?>
            GetDepartmentByIdAsync(int id)
        {
            return await _context.Departments
                .Include(d => d.Employees)
                .FirstOrDefaultAsync(d =>
                    d.Id == id);
        }
    }
}