namespace EmployeePerformance.Dtos
{
    public class PerformanceReviewDto
    {
        public int ReviewId { get; set; }
        public int EmployeeId { get; set; }
        public DateTime ReviewDate { get; set; }
        public int PerfomanceScore { get; set; }
        public string Comments { get; set; }
    }
}
