using EmployeePerformance.Data;
using EmployeePerformance.Dtos;
using EmployeePerformance.Models;
using Microsoft.EntityFrameworkCore;

public class PerformanceReviewRepository : IPerformanceReviewRepository
{
    private readonly AppDbContext _context;

    public PerformanceReviewRepository(AppDbContext context)
    {
        _context = context;
    }

    
    public async Task<PerformanceReviewDto> AddPerformanceReviewAsync(CreatePerformanceReviewDto reviewDto)
    {
        if (reviewDto.PerfomanceScore < 1 || reviewDto.PerfomanceScore > 10)
            throw new ArgumentException("Performance Score must be between 1 and 10.");

        var review = new PerformanceReview
        {
            EmployeeId = reviewDto.EmployeeId,
            PerfomanceScore = reviewDto.PerfomanceScore,
            Comments = reviewDto.Comments,
            ReviewDate = DateTime.UtcNow
        };

        await _context.PerformanceReviews.AddAsync(review);
        await _context.SaveChangesAsync();

        return new PerformanceReviewDto
        {
            ReviewId = review.ReviewId,
            EmployeeId = review.EmployeeId,
            PerfomanceScore = review.PerfomanceScore,
            Comments = review.Comments,
            ReviewDate = review.ReviewDate
        };
    }
    public async Task<List<PerformanceReviewDto>> GetAllReviewsAsync( )
    {
        return await _context.PerformanceReviews
            .Select(r => new PerformanceReviewDto
            {
                ReviewId = r.ReviewId,
                EmployeeId = r.EmployeeId,
                PerfomanceScore = r.PerfomanceScore,
                Comments = r.Comments,
                ReviewDate = r.ReviewDate
            })
            .ToListAsync();
    }


    public async Task<IEnumerable<PerformanceReviewDto>> GetReviewsByEmployeeIdAsync(int employeeId)
    {
        return await _context.PerformanceReviews
            .Where(r => r.EmployeeId == employeeId)
            .Select(r => new PerformanceReviewDto
            {
                ReviewId = r.ReviewId,
                EmployeeId = r.EmployeeId,
                PerfomanceScore = r.PerfomanceScore,
                Comments = r.Comments,
                ReviewDate = r.ReviewDate
            })
            .ToListAsync();
    }

    
    public async Task<PerformanceReviewDto> UpdateReviewAsync(int reviewId, UpdatePerformanceReviewDto reviewDto)
    {
        var review = await _context.PerformanceReviews.FindAsync(reviewId);
        if (review == null) return null;

        review.PerfomanceScore = reviewDto.PerfomanceScore ?? review.PerfomanceScore;
        review.Comments = reviewDto.Comments ?? review.Comments;

        await _context.SaveChangesAsync();

        return new PerformanceReviewDto
        {
            ReviewId = review.ReviewId,
            EmployeeId = review.EmployeeId,
            PerfomanceScore = review.PerfomanceScore,
            Comments = review.Comments,
            ReviewDate = review.ReviewDate
        };
    }

    
    public async Task<bool> DeleteReviewAsync(int reviewId)
    {
        var review = await _context.PerformanceReviews.FindAsync(reviewId);
        if (review == null) return false;

        _context.PerformanceReviews.Remove(review);
        await _context.SaveChangesAsync();
        return true;
    }
}
