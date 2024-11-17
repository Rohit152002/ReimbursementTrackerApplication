using Microsoft.AspNetCore.Mvc;

namespace ReimbursementTrackingApplication.Models.DTOs
{
    public class ReimbursementItemDTO
    {
        public int RequestId { get; set; }
        public double Amount { get; set; }
        public int CategoryId { get; set; }

        public string Description { get; set; } = string.Empty;
        [FromForm(Name = "ReceiptFile")]
        public IFormFile receiptFile { get; set; }


    }
}
