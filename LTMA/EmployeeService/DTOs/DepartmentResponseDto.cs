namespace EmployeeService.DTOs
{
    // DTO used when returning department information from the API.
    public class DepartmentResponseDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;
    }
}
