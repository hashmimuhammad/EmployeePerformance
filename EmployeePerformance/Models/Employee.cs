using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeePerformance.Models
{
    public class Employee
    {
        [Key]
        public int EmployeeId { get; set; }

        [Required]
        public string FullName { get; set; }
        [Required, EmailAddress]
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Department { get; set; }
        [Required]
        public string PasswordHash { get; set; } 

        [Required]
        public string Role { get; set; }
        public DateTime JoiningDate { get; set; } = DateTime.Now;
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal CurrentSalary { get; set; }
        public bool IsActive { get; set; } = true;
        public ICollection<PerformanceReview> PerformanceReviews { get; set; }


    }
}
