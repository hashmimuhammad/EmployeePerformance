
using EmployeePerformance.Data;
using EmployeePerformance.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly ITokenService _tokenService;
    private readonly UserManager<ApplicationUser> _userManager;
   

    public AuthController(ITokenService tokenService, UserManager<ApplicationUser> userManager )
    {
        _tokenService = tokenService;
        _userManager = userManager;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            var password = await _userManager.CheckPasswordAsync(user,loginDto.Password);

            if (user == null || password == null)
            {
                return Unauthorized(new { message = "Invalid email or password" });
            }
            else
            {

                var roles = await _userManager.GetRolesAsync(user);
                var token = _tokenService.GenerateJwtToken(user, roles);
                return Ok(new { token });
            }
        }
        catch (Exception ex)
        {
           
            return StatusCode(500, new { message = "unexpected error occurred.", error = ex.Message });
        }
    }



}

