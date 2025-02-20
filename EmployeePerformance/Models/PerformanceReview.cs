using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeePerformance.Models
{
    public class PerformanceReview
    {
        [Key]
        public int ReviewId { get; set; }
        [Required]
        [ForeignKey("EmployeeId")]
        public int EmployeeId { get; set; }
        public DateTime ReviewDate { get; set; } = DateTime.Now;
        [Range(1, 10)]
        public int PerfomanceScore { get; set; }
        public string Comments { get; set; }
        public Employee Employee { get; set; }
    }
}
