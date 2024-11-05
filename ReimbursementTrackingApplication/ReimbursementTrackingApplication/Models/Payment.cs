namespace ReimbursementTrackingApplication.Models
{
    public enum PaymentMethod
    {
        DirectDeposit,
        Cash,
        MobilePayment,
        Others
    }
    public enum PaymentStatus
    {
        Pending,
        Processing,
        Paid,
        Failed
    }


    public class Payment
    {
        public int Id { get; set; }
        public int RequestId { get; set; }
        public ReimbursementRequest Request { get; set; }
        public double AmountPaid { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public DateTime PaymentDate { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public bool IsDeleted { get; set; } = false;

        public Payment()
        {
            PaymentDate = DateTime.Now;
            //Request = new ReimbursementRequest();

        }

    }
}
