using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using EmployeePerformance.Models;

namespace EmployeePerformance.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<PerformanceReview> PerformanceReviews { get; set; }
        public DbSet<LeaveRequest> LeaveRequests { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); 

          
            modelBuilder.Entity<Employee>()
                .HasIndex(e => e.Email)
                .IsUnique();

            modelBuilder.Entity<Employee>()
                .HasMany(e => e.PerformanceReviews)
                .WithOne(p => p.Employee)
                .HasForeignKey(p => p.EmployeeId);

            
            modelBuilder.Entity<PerformanceReview>()
                .Property(p => p.PerfomanceScore)
                .HasDefaultValue(1);
        }
    }
}
