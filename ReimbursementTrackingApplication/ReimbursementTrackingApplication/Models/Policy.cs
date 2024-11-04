namespace ReimbursementTrackingApplication.Models
{
    public class Policy
    {
        public int Id { get; set; }
        public string PolicyName { get; set; }
        public int MaxAmount { get; set; }
        public string PolicyDescription { get; set; } = string.Empty;
        public IEnumerable<ReimbursementRequest> Reimbursements { get; set; }

        public Policy()
        {
            Reimbursements = new List<ReimbursementRequest>();
        }
    }
}
