namespace ReimbursementTrackingApplication.Models.DTOs
{
    public class ResponseReimbursementRequestDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public UserDTO User { get; set; }
        public int PolicyId { get; set; }
        public string PolicyName { get; set; } = string.Empty;
        public double TotalAmount { get; set; }
        public RequestStatus Status { get; set; }
        public string StatusName => Status.ToString();
        public Stage Stage { get; set; }
        public string StageName => Stage.ToString();
        public string Comments { get; set; } = string.Empty;
        public bool IsApprovedByManager { get; set; } = false;
        public bool IsApprovedByHr { get; set; } = false;
        public bool IsApprovedByFinance { get; set; } = false;

        public List<ResponseReimbursementItemDTO> Items { get; set; }
    }
}
