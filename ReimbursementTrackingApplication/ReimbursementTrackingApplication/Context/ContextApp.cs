﻿using Microsoft.EntityFrameworkCore;
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


        }
    }
}