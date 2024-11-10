namespace ReimbursementTrackingApplication.Models.DTOs
{
    public class PaymentDTO
    {
        public int Id { get; set; } 
        //public int RequestId { get; set; }
        public PaymentMethod PaymentMethod { get; set; }   
    }
}
