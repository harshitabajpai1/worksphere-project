namespace EmployeeService.DTOs
{
    // Data required when SuperAdmin creates
    // a manager or employee.
    public class CreateEmployeeDto
    {
        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public int DepartmentId { get; set; }

        // Expected values:
        // "Manager"
        // "Employee"
        public string Designation { get; set; } = string.Empty;
    }
}