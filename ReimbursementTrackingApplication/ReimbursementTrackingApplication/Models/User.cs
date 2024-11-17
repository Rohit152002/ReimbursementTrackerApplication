namespace ReimbursementTrackingApplication.Models
{
    public enum Departments
    {
        HR,
        Finance,
        IT,
        Sales,
        Marketing,
        Operations,
        Legal,
        Admin,
        CustomerSupport
    }

    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public byte[] Password { get; set; }
        public byte[] HashKey { get; set; }
        public Departments? Department { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public IEnumerable<ApprovalStage> Approvals { get; set; }
        public IEnumerable<ReimbursementRequest> Request { get; set; }
        public BankAccount Bank { get; set; }
        public IEnumerable<Employee> ManageEmployee { get; set; }
        public bool IsDeleted { get; set; }= false;
        public Employee Employee { get; set; }
        public User()
        {
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
            Approvals = new List<ApprovalStage>();
            Request= new List<ReimbursementRequest>();
            ManageEmployee = new List<Employee>();
            //Bank= new BankAccount();
            //Employee= new Employee();
        }

        public void Update()
        {
            UpdatedAt = DateTime.Now;
        }
    }

}
