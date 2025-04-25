using EmployeePerformance.Data;
using EmployeePerformance.Dtos;
using EmployeePerformance.Interfaces;
using EmployeePerformance.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;



namespace EmployeePerformance.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class LeaveRequestController : ControllerBase
    {
        private readonly ILeaveRepository _leaveRepo;
        private readonly LeaveRequestService _leaveRequestService;

        public LeaveRequestController(ILeaveRepository leaveRepo, LeaveRequestService leaveRequestService)
        {
            _leaveRepo = leaveRepo;
            _leaveRequestService = leaveRequestService;

        }
        [HttpPost]
        [Authorize(policy: "Employee")]
        public async Task<IActionResult> RequestLeave([FromBody] CreateLeaveRequestDto leaveRequestDto)
        {
            try
            {
                if (leaveRequestDto == null)
                {
                    return BadRequest(new { message = "Invalid request data" });
                }

                var leaveRequest = new LeaveRequest
                {
                    EmployeeId = leaveRequestDto.EmployeeId,
                    LeaveType = leaveRequestDto.LeaveType,
                    StartgDate = leaveRequestDto.StartDate, 
                    EndDate = leaveRequestDto.EndDate,
                    Reason = leaveRequestDto.Reason
                };

                await _leaveRepo.AddAsync(leaveRequest);

                return Ok(new { message = "Leave request submitted successfully" });
            }
            catch (Exception ex)
            {
                
                return StatusCode(500, new { message = "unexpected error occurred while submitting the leave request.", error = ex.Message });
            }
        }
        [HttpGet("all")]
        [Authorize(policy: "Admin")]
        public async Task<IActionResult> GetAllRequest()
        {
            var request = await _leaveRepo.GetAllAsync();
            return Ok(request);
        }

        [HttpGet("{leaveId}")]
        [Authorize(policy: "Admin")]
        public async Task<IActionResult> GetByIdAsync(int leaveId)
        {
            try
            {
                var result = await _leaveRepo.GetByIdAsync(leaveId);

                if (result == null)
                {
                    return NotFound(new { message = "Leave request not found" });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                
                return StatusCode(500, new { message = "unexpected error occurred while retrieving the leave request.", error = ex.Message });
            }
        }

        [HttpDelete]
        [Authorize(policy: "Admin")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            try
            {
                var result = await _leaveRepo.DeleteAsync(id);

                if (!result)
                {
                    return NotFound(new { message = "Leave request not found" });
                }

                return Ok(new { message = "Leave request deleted successfully" });
            }
            catch (Exception ex)
            {
                
                return StatusCode(500, new { message = "unexpected error occurred while deleting the leave request.", error = ex.Message });
            }
        }

    }
}
