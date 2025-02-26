using Dapper;
using EmployeePerformance.Data;
using EmployeePerformance.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;


[Route("api/[controller]")]
[ApiController]
public class PerformanceReviewsController : ControllerBase
{
    private readonly IPerformanceReviewRepository _pReviewRepo;
    private readonly AppDbContext _dbContext;
    private readonly IDbConnection _dbConnection;

    public PerformanceReviewsController(IPerformanceReviewRepository performanceReviewRepository, AppDbContext dbContext, IDbConnection dbConnection)
    {
        _pReviewRepo = performanceReviewRepository;
        _dbContext = dbContext;
        _dbConnection = dbConnection;
    }

    [HttpGet]
    //[Authorize(policy: "Admin")]
    public async Task<IActionResult> GetAllReviews()
    {
        //var reviews = await _pReviewRepo.GetAllReviewsAsync();
        var reviews = await _dbContext.PerformanceReviews
            .FromSqlRaw("Select * from PerformanceReviews")
            .ToListAsync();
        return Ok(reviews);


    }

    [HttpGet("employee/{employeeId}")]
    [Authorize(policy: "AdminOrEmployee")]
    public async Task<IActionResult> GetReviewsByEmployeeId(int employeeId)
    {
        // var reviews = await _pReviewRepo.GetReviewsByEmployeeIdAsync(employeeId);
        var reviews = await _dbContext.PerformanceReviews
             .FromSqlRaw("Select * from PerformanceReviews Where EmployeeId = @id", employeeId)
             .FirstOrDefaultAsync();
        return Ok(reviews);
    }

    [HttpPost]
    [Authorize(policy: "Admin")]
    public async Task<IActionResult> AddPerformanceReview([FromBody] CreatePerformanceReviewDto reviewDto)
    {
        if (reviewDto == null)
            return BadRequest(new { message = "Invalid data" });

        try
        {
            //var newReview = await _pReviewRepo.AddPerformanceReviewAsync(reviewDto);

            string query = @"
                Insert Into PerformanceReviews (EmployeeId, ReviewDate, PerfomanceScore, Comments) 
                VALUES (@EmployeeId, @ReviewDate, @PerfomanceScore, @Comments);
                ";
            var newReview = new
            {
                EmployeeId = reviewDto.EmployeeId,
                ReviewDate = DateTime.UtcNow,  
                PerfomanceScore = reviewDto.PerfomanceScore,
                Comments = reviewDto.Comments
            };
            await _dbConnection.ExecuteAsync(query, newReview);

            return CreatedAtAction(nameof(GetReviewsByEmployeeId), new { employeeId = newReview.EmployeeId }, newReview);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
 
    [HttpPut("{id}")]
    [Authorize(policy: "Admin")]
    public async Task<IActionResult> UpdatePerformanceReview(int id, [FromBody] UpdatePerformanceReviewDto reviewDto)
    {
        if (reviewDto == null)
            return BadRequest(new { message = "Invalid data" });

        //var updatedReview = await _pReviewRepo.UpdateReviewAsync(id, reviewDto);

        var updateReview = await _dbContext.Database.ExecuteSqlRawAsync(@"
            UPDATE PerformanceReviews 
            SET PerfomanceScore = @p0, Comments = @p1 
            WHERE ReviewId = @p2",

        reviewDto.PerfomanceScore, reviewDto.Comments, id);

        if (updateReview == 0)
            return NotFound(new { message = "Performance review not found" });

        return Ok(new { message = "Performance review updated successfully" });
    }

    [HttpDelete("{id}")]
    //[Authorize(policy: "Admin")]
    public async Task<IActionResult> DeletePerformanceReview(int id)
    {
        //var result = await _pReviewRepo.DeleteReviewAsync(id);
        var result = await _dbContext.Database
            .ExecuteSqlRawAsync("DELETE FROM PerformanceReviews WHERE ReviewId = {0}", id);

        if (result == 0)
            return NotFound(new { message = "Performance review not found" });

        return Ok(new { message = "Performance review deleted successfully" });



    }


}


