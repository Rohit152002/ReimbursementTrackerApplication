namespace ReimbursementTrackingApplication.Models.DTOs
{
    public class ResponsePayment
    {
        public int Id { get; set; }
        public int RequestId { get; set; }
        public ResponseReimbursementRequestDTO Request { get; set; }
        public double AmountPaid { get; set; }
        public PaymentMethod? PaymentMethod { get; set; }
        public DateTime PaymentDate { get; set; }
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;
    }
}
