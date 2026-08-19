namespace TimesheetService.DTOs
{
    // Data required when an employee creates a timesheet.
    public class CreateTimesheetDto
    {
        public DateTime Date { get; set; }

        public decimal HoursWorked { get; set; }

        public string WorkDescription { get; set; } = string.Empty;
    }
}
