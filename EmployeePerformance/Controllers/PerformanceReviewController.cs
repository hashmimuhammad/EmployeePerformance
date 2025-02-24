using EmployeePerformance.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class PerformanceReviewsController : ControllerBase
{
    private readonly IPerformanceReviewRepository _pReviewRepo;

    public PerformanceReviewsController(IPerformanceReviewRepository performanceReviewRepository)
    {
        _pReviewRepo = performanceReviewRepository;
    }

    [HttpGet]
    [Authorize(policy: "Admin")]
    public async Task<IActionResult> GetAllReviews()
    {
        var reviews = await _pReviewRepo.GetAllReviewsAsync();
        return Ok(reviews);
    }

    [HttpGet("employee/{employeeId}")]
    [Authorize(policy: "AdminOrEmployee")]
    public async Task<IActionResult> GetReviewsByEmployeeId(int employeeId)
    {
        var reviews = await _pReviewRepo.GetReviewsByEmployeeIdAsync(employeeId);
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
            var newReview = await _pReviewRepo.AddPerformanceReviewAsync(reviewDto);
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

        var updatedReview = await _pReviewRepo.UpdateReviewAsync(id, reviewDto);
        if (updatedReview == null)
            return NotFound(new { message = "Performance review not found" });

        return Ok(updatedReview);
    }

    [HttpDelete("{id}")]
    [Authorize(policy: "Admin")]
    public async Task<IActionResult> DeletePerformanceReview(int id)
    {
        var result = await _pReviewRepo.DeleteReviewAsync(id);
        if (!result)
            return NotFound(new { message = "Performance review not found" });

        return Ok(new { message = "Performance review deleted successfully" });
    }
}
