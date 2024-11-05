using System.Security.Cryptography.X509Certificates;

namespace ReimbursementTrackingApplication.Models
{
    public class BankAccount
    {
        public int Id { get; set; }
        public int UserId {  get; set; }
        public User User { get; set; }
        public double AccNo { get; set; }
        public string BranchName { get; set; }
        public string IFSCCode { get; set; }
        public string BranchAddress { get; set; }
        public DateTime CreatedAt {  get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsDeleted { get; set; } = false;

        public BankAccount()
        {
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow; 
            //User = new User();
        }

        public void Update()
        {
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
