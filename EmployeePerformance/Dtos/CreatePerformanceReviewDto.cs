using System.ComponentModel.DataAnnotations;

namespace EmployeePerformance.Dtos
{
    public class CreatePerformanceReviewDto
    {
        public int EmployeeId { get; set; }

        [Range(1, 10, ErrorMessage = "Performance Score must be between 1 and 10.")]
        public int PerfomanceScore { get; set; }

        public string Comments { get; set; }
    }
}
