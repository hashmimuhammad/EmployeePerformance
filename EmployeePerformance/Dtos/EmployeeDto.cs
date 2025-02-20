namespace EmployeePerformance.Dtos
{
    public class EmployeeDto
    {
        public int EmployeeId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Department { get; set; }
        public DateTime JoiningDate { get; set; }
        public decimal CurrentSalary { get; set; }
        public bool IsActive { get; set; }
    }
}
