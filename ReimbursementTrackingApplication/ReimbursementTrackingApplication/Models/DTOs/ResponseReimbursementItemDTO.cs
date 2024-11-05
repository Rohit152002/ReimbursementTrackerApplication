namespace ReimbursementTrackingApplication.Models.DTOs
{
    public class ResponseReimbursementItemDTO
    {

        public int RequestId { get; set; }
        public double Amount { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string receiptFile { get; set; }= string.Empty;

    }
}
