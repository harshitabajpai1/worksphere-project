namespace LeaveService.DTOs
{
    // Data required when an employee applies for leave.
    public class ApplyLeaveDto
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string Reason { get; set; } = string.Empty;
    }
}
