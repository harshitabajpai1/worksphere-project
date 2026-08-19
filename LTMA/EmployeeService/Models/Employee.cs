namespace EmployeeService.Models
{
    // Represents an employee or manager in WorkSphere.
    public class Employee
    {
        // Primary key.
        public int Id { get; set; }

        // Employee's full name.
        public string Name { get; set; } = string.Empty;

        // Employee's email address.
        public string Email { get; set; } = string.Empty;

        // Employee's designation.
        // It will be either "Employee" or "Manager".
        public string Designation { get; set; } = string.Empty;

        // Department to which the employee belongs.
        public int DepartmentId { get; set; }

        // Navigation property to the department.
        public Department? Department { get; set; }

        // ID of the manager responsible for this employee.
        //
        // For a normal employee, this contains the manager's Id.
        // For a manager, this will be null.
        public int? ManagerId { get; set; }

        // Navigation property to the manager.
        public Employee? Manager { get; set; }
        // Used to activate/deactivate an employee.
        public bool IsActive { get; set; } = true;

        // Stores when the employee was created.
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}