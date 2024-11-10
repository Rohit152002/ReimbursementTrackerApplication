using Microsoft.AspNetCore.Mvc;

namespace ReimbursementTrackingApplication.Models.DTOs
{
    public class RequestITemDTO
    {
        public double Amount { get; set; }
        public int CategoryId { get; set; }

        public string Description { get; set; } = string.Empty;
       
        public IFormFile receiptFile { get; set; }
    }
}
