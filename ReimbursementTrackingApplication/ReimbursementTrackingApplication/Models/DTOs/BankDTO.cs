namespace ReimbursementTrackingApplication.Models.DTOs
{
    public class BankDTO
    {
        public int UserId { get; set; }
        public string AccNo { get; set; }
        public string BranchName { get; set; }= string.Empty;
        public string IFSCCode { get; set; } = string.Empty;
        public string BranchAddress { get; set; } = string.Empty;
    }
}
