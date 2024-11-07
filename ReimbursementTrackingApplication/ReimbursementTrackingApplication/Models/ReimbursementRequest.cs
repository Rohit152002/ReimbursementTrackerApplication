namespace ReimbursementTrackingApplication.Models
{
    public enum RequestStatus
    {
        Passed,
        Pending,
        Rejected
    }

    public class ReimbursementRequest
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }  
        public int PolicyId {  get; set; }
        public Policy Policy { get; set; }
        public double TotalAmount { get; set; }
        public string Comments { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public RequestStatus Status { get; set; }=RequestStatus.Pending;
        public IEnumerable<ReimbursementItem> Items { get; set; }
        public IEnumerable<ApprovalStage> Approvals { get; set; }
        public Payment Payment { get; set; }
        public bool IsDeleted { get; set; } = false;
        public ReimbursementRequest()
        {
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
            Items = new List<ReimbursementItem>();
            //Payment = new Payment();
            //Policy = new Policy();
            //User= new User();

            Approvals = new List<ApprovalStage>();
        }

        // Method to manually update the UpdatedAt timestamp
        public void Update()
        {
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
