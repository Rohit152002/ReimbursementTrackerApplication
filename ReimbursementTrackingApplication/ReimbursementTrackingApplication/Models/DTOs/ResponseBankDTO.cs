namespace ReimbursementTrackingApplication.Models.DTOs
{
    public class ResponseBankDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public UserDTO User { get; set; }   
        public string AccNo { get; set; }
        public string BranchName { get; set; } = string.Empty;
        public string IFSCCode { get; set; } = string.Empty;
        public string BranchAddress { get; set; } = string.Empty;
    }
}
