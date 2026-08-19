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

        // Sent securely by the SuperAdmin when the account is created.
        // EmployeeService forwards it to AuthService and never stores it.
        public string InitialPassword { get; set; } = string.Empty;
    }
}
