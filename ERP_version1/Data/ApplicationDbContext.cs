using ERP_version1.Models.AttendanceModule;
using ERP_version1.Models.Common;
using ERP_version1.Models.HR;
using ERP_version1.Models.LeaveModule;
using ERP_version1.Models.PayrollModule;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ERP_version1.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Department> Departments { get; set; }
        public DbSet<Designation> Designations { get; set; }
        public DbSet<Employee> Employees { get; set; }

        public DbSet<Attendance> Attendances { get; set; }

        public DbSet<LeaveType> LeaveTypes { get; set; }
        public DbSet<LeaveApplication> LeaveApplications { get; set; }

        public DbSet<Payroll> Payrolls { get; set; }

        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Employee>()
                .HasOne(e => e.Department)
                .WithMany()
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Employee>()
                .HasOne(e => e.Designation)
                .WithMany()
                .HasForeignKey(e => e.DesignationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Designation>()
                .HasOne(d => d.Department)
                .WithMany()
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Attendance>()
                .HasOne(a => a.Employee)
                .WithMany()
                .HasForeignKey(a => a.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<LeaveApplication>()
                .HasOne(l => l.Employee)
                .WithMany()
                .HasForeignKey(l => l.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<LeaveApplication>()
                .HasOne(l => l.LeaveType)
                .WithMany()
                .HasForeignKey(l => l.LeaveTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Payroll>()
                .HasOne(p => p.Employee)
                .WithMany()
                .HasForeignKey(p => p.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}