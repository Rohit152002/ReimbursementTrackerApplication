namespace ReimbursementTrackingApplication.Models.DTOs
{
    public class ResponseApprovalStageDTO
    {
        public int RequestId { get; set; }
        public ResponseReimbursementRequestDTO Request { get; set; }
        public int ReviewId { get; set; }
        public User Review { get; set; }

        public string Comments { get; set; } = string.Empty;
    }
}
