namespace ReimbursementTrackingApplication.Models.DTOs
{
    public class ResponsePolicyDTO
    {
        public int Id { get; set; }
        public string PolicyName { get; set; }
        public int MaxAmount { get; set; }
        public string PolicyDescription { get; set; } = string.Empty;
    }
}
