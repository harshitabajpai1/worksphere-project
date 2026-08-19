using EmployeeService.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace EmployeeService.Data
{
    // This class is responsible for communicating
    // with the WorkSphere Employee database.
    public class EmployeeDbContext : DbContext
    {
        public EmployeeDbContext(
            DbContextOptions<EmployeeDbContext> options)
            : base(options)
        {
        }

        // Departments table.
        public DbSet<Department> Departments { get; set; }

        // Employees table.
        public DbSet<Employee> Employees { get; set; }

        protected override void OnModelCreating(
            ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // One department can have many employees.
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Department)
                .WithMany(d => d.Employees)
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}