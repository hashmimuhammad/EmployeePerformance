namespace EmployeePerformance.Dtos
{
    public class CreateEmployeeDto
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Department { get; set; }
        public DateTime? JoiningDate { get; set; } = DateTime.UtcNow;
        public decimal CurrentSalary { get; set; }
    }
}
