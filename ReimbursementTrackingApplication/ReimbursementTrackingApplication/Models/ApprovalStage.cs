namespace ReimbursementTrackingApplication.Models
{
    public enum Stage
    {
        Manager,
        Hr,
        Financial
    }
    public enum Status 
    {
        Approved,
        Rejected,
        Pending,
    }


    public class ApprovalStage
    {
        public int Id { get; set; }
        public int RequestId {  get; set; }
        public ReimbursementRequest Request { get; set; }
        public int ReviewId { get; set; }
        public User Review { get; set; }
        public Stage Stage { get; set; }
        public Status Status { get; set; } = Status.Approved;
        public DateTime ReviewDate { get; set; }
        public string Comments { get; set; } = string.Empty;
        public bool IsDeleted { get; set; } = false;

        public ApprovalStage()
        {
            ReviewDate = DateTime.Now;
            //Request= new ReimbursementRequest();
            //Review = new User();
        }
    }
}
