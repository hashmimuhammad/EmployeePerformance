//namespace EmployeePerformance.Dtos
//{
//    public class CreateEmployeeDto
//    {
//        public string FullName { get; set; }
//        public string Email { get; set; }
//        public DateTime DateOfBirth { get; set; }
//        public string Department { get; set; }
//        public DateTime? JoiningDate { get; set; } = DateTime.UtcNow;
//        public decimal CurrentSalary { get; set; }
//    }
//}


using System;
using System.ComponentModel.DataAnnotations;

namespace EmployeePerformance.Dtos
{
    public class CreateEmployeeDto
    {
        [Required]
        public string FullName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string Password { get; set; }  

        [Required]
        [RegularExpression("^(Admin|Employee)$", ErrorMessage = "Role must be either 'Admin' or 'Employee'.")]
        public string Role { get; set; }  

        [Required]
        public string Department { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public DateTime JoiningDate { get; set; } = DateTime.UtcNow;

        [Required]
        public decimal CurrentSalary { get; set; }
    }
}
