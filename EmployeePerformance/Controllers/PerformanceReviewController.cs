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
 

    public PerformanceReviewsController(IPerformanceReviewRepository performanceReviewRepository)
    {
        _pReviewRepo = performanceReviewRepository;
      
    }

    [HttpGet]
    //[Authorize(policy: "Admin")]
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
        {
            return BadRequest(new { message = "Invalid data" });
        }

        try
        {
            var newReview = await _pReviewRepo.AddPerformanceReviewAsync(reviewDto);

            return CreatedAtAction(nameof(GetReviewsByEmployeeId), new { employeeId = newReview.EmployeeId }, newReview);
        }
        
        catch (Exception ex)
        {
            
            return StatusCode(500, new { message = "unexpected error occurred while adding the performance review.", error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    [Authorize(policy: "Admin")]
    public async Task<IActionResult> UpdatePerformanceReview(int id, [FromBody] UpdatePerformanceReviewDto reviewDto)
    {
        if (reviewDto == null)
        {
            return BadRequest(new { message = "Invalid data" });
        }

        try
        {
            var updatedReview = await _pReviewRepo.UpdateReviewAsync(id, reviewDto);

            if (updatedReview == null)
            {
                return NotFound(new { message = "Performance review not found" });
            }

            return Ok(new { message = "Performance review updated successfully" });
        }
        catch (Exception ex)
        {
            
            return StatusCode(500, new { message = "unexpected error occurred while updating the performance review.", error = ex.Message });
        }
    }

    [HttpDelete("{id}")]


    //[Authorize(policy: "Admin")]
    public async Task<IActionResult> DeletePerformanceReview(int id)
    {
        try
        {
            var result = await _pReviewRepo.DeleteReviewAsync(id);

            if (result == null)
            {
                return NotFound(new { message = "Performance review not found" });
            }

            return Ok(new { message = "Performance review deleted successfully" });
        }
        catch (Exception ex)
        {
            
            return StatusCode(500, new { message = "unexpected error occurred while deleting the performance review.", error = ex.Message });
        }
    }


}


