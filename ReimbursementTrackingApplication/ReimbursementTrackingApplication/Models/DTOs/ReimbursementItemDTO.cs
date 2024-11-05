namespace ReimbursementTrackingApplication.Models.DTOs
{
    public class ReimbursementItemDTO
    {
        public int RequestId { get; set; }
        public double Amount { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public IFormFile receiptFile { get; set; }
       
     
    }
}
