namespace EmployeePerformance.Dtos
{
    public class UpdateEmployeeDto
    {
        
        public string FullName { get; set; }
        public string Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Department { get; set; }
        public decimal? CurrentSalary { get; set; }
    }
}
