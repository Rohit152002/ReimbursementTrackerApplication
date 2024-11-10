namespace ReimbursementTrackingApplication.Models.DTOs
{
    public class CreatePolicyDTO
    {
        public string PolicyName { get; set; }
        public int MaxAmount { get; set; }
        public string PolicyDescription { get; set; } = string.Empty;
    }
}
