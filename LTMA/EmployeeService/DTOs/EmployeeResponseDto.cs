namespace EmployeeService.DTOs
{
    // DTO used when returning employee information from the API.
    //
    //
    // This prevents circular JSON responses such as:
    // Employee --> Department --> Employees --> Department...
    public class EmployeeResponseDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Designation { get; set; } = string.Empty;

        public int DepartmentId { get; set; }

        public string DepartmentName { get; set; } = string.Empty;

        public int? ManagerId { get; set; }

        public string? ManagerName { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}