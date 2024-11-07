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
        public string Comments { get; set; } = string.Empty;

        public List<ResponseReimbursementItemDTO> Items { get; set; }
    }
}
