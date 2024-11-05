namespace ReimbursementTrackingApplication.Models
{
    public class ExpenseCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public IEnumerable<ReimbursementItem> Items { get; set; }
        public bool IsDeleted { get; set; } = false;

        public ExpenseCategory()
        {
            Items = new List<ReimbursementItem>();
        }

    }
}
