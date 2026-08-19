namespace AuthService.DTOs
{
    // Data received from EmployeeService when a SuperAdmin
    // creates an Employee or Manager account.
    public class CreateUserDto
    {
        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        // EmployeeService sends either "Employee" or "Manager".
        public string Role { get; set; } = string.Empty;

        // Received only to create the account. AuthService hashes it
        // before saving and never returns it.
        public string Password { get; set; } = string.Empty;
    }
}
