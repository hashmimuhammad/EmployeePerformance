namespace EmployeePerformance.Dtos
{
    public class UpdateEmployeeDto
    {
        
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        // public DateTime? DateOfBirth { get; set; }
        public string Department { get; set; } = string.Empty;
        public decimal? CurrentSalary { get; set; }
    }
}
