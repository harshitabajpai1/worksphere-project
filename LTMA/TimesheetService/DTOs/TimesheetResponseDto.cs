namespace TimesheetService.DTOs
{
    // DTO used when returning timesheet information from the API.
    public class TimesheetResponseDto
    {
        public int Id { get; set; }

        public int EmployeeId { get; set; }

        public DateTime Date { get; set; }

        public decimal HoursWorked { get; set; }

        public string WorkDescription { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public DateTime? ApprovedAt { get; set; }

        public int? ApprovedBy { get; set; }
    }
}
