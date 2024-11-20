using Microsoft.EntityFrameworkCore;
using ReimbursementTrackingApplication.Models;

namespace ReimbursementTrackingApplication.Context
{
    public class ContextApp : DbContext
    {
        public ContextApp(DbContextOptions options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<ApprovalStage> Approvals { get; set; }
        public DbSet<BankAccount> BankAccounts { get; set; }
        public DbSet<ReimbursementItem> ReimbursementItems { get; set; }
        public DbSet<ReimbursementRequest> ReimbursementRequests { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<ExpenseCategory> Expenses { get; set; }
        public DbSet<Policy> Policy { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {



            modelBuilder.Entity<Employee>()
                .HasOne(u => u.Manager)
                .WithMany(o => o.ManageEmployee)
                .HasForeignKey(u => u.ManagerId)
                .HasConstraintName("FK_Employee_Manager")
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Employee>()
                .HasOne(u => u.User)
                .WithOne(u => u.Employee)
                .HasForeignKey<Employee>(u => u.EmployeeId)
                .HasConstraintName("FK_Employee_User")
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<BankAccount>()
                .HasOne(u => u.User)
                .WithOne(u => u.Bank)
                .HasForeignKey<BankAccount>(b => b.UserId)
                .HasConstraintName("FK_Bank_User");

            //modelBuilder.Entity<Employee>()
            //   .HasKey(e => new { e.EmployeeId, e.ManagerId });

            modelBuilder.Entity<ReimbursementRequest>()
                .HasOne(r => r.Policy)
                .WithMany(p => p.Reimbursements)
                .HasForeignKey(r => r.PolicyId)
                .HasConstraintName("FK_Request_Policy");

            modelBuilder.Entity<ReimbursementRequest>()
                .HasOne(r => r.User)
                .WithMany(u => u.Request)
                .HasForeignKey(r => r.UserId)
                .HasConstraintName("FK_Request_User");

            modelBuilder.Entity<ReimbursementItem>()
                .HasOne(i => i.Request)
                .WithMany(r => r.Items)
                .HasForeignKey(i => i.RequestId)
                .HasConstraintName("FK_Item_Request");

            modelBuilder.Entity<ReimbursementItem>()
                .HasOne(i => i.Category)
                .WithMany(r => r.Items)
                .HasForeignKey(i => i.CategoryId)
                .HasConstraintName("FK_Item_Category");

            modelBuilder.Entity<ApprovalStage>()
                .HasOne(a => a.Request)
                .WithMany(r => r.Approvals)
                .HasForeignKey(a => a.RequestId)
                .HasConstraintName("FK_Approval_Request")
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ApprovalStage>()
                .HasOne(a => a.Review)
                .WithMany(r => r.Approvals)
                .HasForeignKey(a => a.ReviewId)
                .HasConstraintName("Fk_Approval_User");

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Request)
                .WithOne(r => r.Payment)
                .HasForeignKey<Payment>(p => p.RequestId)
                .HasConstraintName("FK_Payment_Request");

            modelBuilder.Entity<ApprovalStage>()
                .Property(a => a.Stage)
                .HasConversion(a => a.ToString(),
                a => (Stage)Enum.Parse(typeof(Stage), a)
             );

            modelBuilder.Entity<ApprovalStage>()
                .Property(a => a.Status)
                .HasConversion(a => a.ToString(),
                a => (Status)Enum.Parse(typeof(Status), a)
             );

            modelBuilder.Entity<Payment>()
                .Property(a => a.PaymentStatus)
                .HasConversion(a => a.ToString(),
                a => (PaymentStatus)Enum.Parse(typeof(PaymentStatus), a)
             );

            modelBuilder.Entity<ReimbursementRequest>()
                .Property(a => a.Stage)
                .HasConversion(a => a.ToString(),
                a => (Stage)Enum.Parse(typeof(Stage), a)
             );

            modelBuilder.Entity<ReimbursementRequest>()
                .Property(a => a.Status)
                .HasConversion(a => a.ToString(),
                a => (RequestStatus)Enum.Parse(typeof(RequestStatus), a)
             );


            modelBuilder.Entity<User>()
                           .Property(a => a.Department)
                           .HasConversion(a => a.ToString(),
                           a => (Departments)Enum.Parse(typeof(Departments), a)
                        );

            modelBuilder.Entity<User>()
            .Property(u => u.Gender)
            .HasConversion(u => u.ToString(),
            u => (Gender)Enum.Parse(typeof(Gender), u));

            modelBuilder.Entity<User>()
                            .HasIndex(a => a.Email)
                            .IsUnique();



            //approvals
            //Approvals = new List<ApprovalStage>();
            //Request = new List<ReimbursementRequest>();
            //ManageEmployee = new List<Employee>();
            //Bank = new BankAccount();
            //Employee = new Employee();
        }
    }
}
