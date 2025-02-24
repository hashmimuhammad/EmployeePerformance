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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // 👈 Call base method for Identity tables

            // Unique Email Constraint for Employees
            modelBuilder.Entity<Employee>()
                .HasIndex(e => e.Email)
                .IsUnique();

            // Employee - PerformanceReview (One-to-Many)
            modelBuilder.Entity<Employee>()
                .HasMany(e => e.PerformanceReviews)
                .WithOne(p => p.Employee)
                .HasForeignKey(p => p.EmployeeId);

            // Ensure PerformanceScore default value
            modelBuilder.Entity<PerformanceReview>()
                .Property(p => p.PerfomanceScore)
                .HasDefaultValue(1);
        }
    }
}
