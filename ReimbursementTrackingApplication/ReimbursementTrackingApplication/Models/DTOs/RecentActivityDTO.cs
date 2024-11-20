namespace ReimbursementTrackingApplication.Models.DTOs
{
    public class RecentActivityDTO
    {
        public int Id { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty; // E.g., "Approved", "Rejected", "Pending"
        public double Amount { get; set; }
        public DateTime Date { get; set; }
    }
}
