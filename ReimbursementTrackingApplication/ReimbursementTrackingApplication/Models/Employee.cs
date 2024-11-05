namespace ReimbursementTrackingApplication.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public User User { get; set; }
        public int ManagerId { get; set; }
        public User Manager { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public IEnumerable<ReimbursementRequest> Requests { get;set; }
        public bool IsDeleted { get; set; } = false;

        public Employee()
        {
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow; 
            //User = new User();
            //Manager= new User();
            Requests= new List<ReimbursementRequest>();
        }

 
        public void Update()
        {
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
