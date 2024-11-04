﻿namespace ReimbursementTrackingApplication.Models
{
    public class ReimbursementItem
    {
        public int Id { get; set; }
        public int RequestId { get; set; }
        public ReimbursementRequest Request { get; set; }
        public double Amount { get; set; }
        public int CategoryId { get; set; }
        public ExpenseCategory Category { get; set; }
        public string Description { get; set; }
        public string receiptFile { get; set; }
        public DateTime DateIncurred { get; set; }
        public ReimbursementItem()
        {
            DateIncurred = DateTime.Now;
            Request= new ReimbursementRequest();
            Category = new ExpenseCategory();
        }

    }
}
