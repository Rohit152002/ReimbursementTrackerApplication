namespace ReimbursementTrackingApplication.Models.DTOs
{
    public class DashboardResponseDTO
    {
        public int TotalRequests { get; set; }
        public int PendingRequests { get; set; }
        public int ApprovedRequests { get; set; }
        public int RejectedRequests { get; set; }
        public int EmployeesCount { get; set; }
        public List<RecentActivityDTO> RecentActivities { get; set; }
    }
}
