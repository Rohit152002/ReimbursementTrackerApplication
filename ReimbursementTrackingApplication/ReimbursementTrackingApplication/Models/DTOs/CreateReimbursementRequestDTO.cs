namespace ReimbursementTrackingApplication.Models.DTOs
{
    public class CreateReimbursementRequestDTO
    {
        public int UserId { get; set; }
        public int PolicyId { get; set; }
        public double TotalAmount { get; set; }
        public string Comments { get; set; } = string.Empty;

        public List<ReimbursementItemDTO> Items { get; set; }
    }
}
