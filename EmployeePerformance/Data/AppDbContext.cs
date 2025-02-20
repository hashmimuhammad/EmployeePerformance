using Microsoft.EntityFrameworkCore;
using EmployeePerformance.Models;

namespace EmployeePerformance.Data
{
    public class AppDbContext :DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<PerformanceReview> PerformanceReviews { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>()
                .HasIndex(e => e.Email)
                .IsUnique();

            modelBuilder.Entity<Employee>()
                .HasMany(e => e.PerformanceReviews)
                .WithOne(p => p.Employee)
                .HasForeignKey(p => p.EmployeeId);
        }



    }
}
