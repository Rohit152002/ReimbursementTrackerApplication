namespace ReimbursementTrackingApplication.Models.DTOs
{
    public class ApprovalStageDTO
    {
        public int RequestId { get; set; }
        public int ReviewId { get; set; }

        public string Comments { get; set; } = string.Empty;
    }
}
