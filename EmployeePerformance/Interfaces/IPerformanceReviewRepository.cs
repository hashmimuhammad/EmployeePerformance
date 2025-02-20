using EmployeePerformance.Dtos;
using EmployeePerformance.Models;

public interface IPerformanceReviewRepository
{
    Task<PerformanceReviewDto> AddPerformanceReviewAsync(CreatePerformanceReviewDto reviewDto);
    Task<bool> DeleteReviewAsync(int reviewId);
    Task<IEnumerable<PerformanceReviewDto>> GetReviewsByEmployeeIdAsync(int employeeId);
    Task<List<PerformanceReviewDto>> GetAllReviewsAsync();
    Task<PerformanceReviewDto> UpdateReviewAsync(int reviewId, UpdatePerformanceReviewDto reviewDto);
}
