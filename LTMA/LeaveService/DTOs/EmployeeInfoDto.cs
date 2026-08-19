namespace LeaveService.DTOs
{
    // Contains employee information received from EmployeeService.
    public class EmployeeInfoDto
    {
        public int Id { get; set; }

        public string Email { get; set; } = string.Empty;

        public string Designation { get; set; } = string.Empty;

        public int? ManagerId { get; set; }

        public bool IsActive { get; set; }
    }
}
