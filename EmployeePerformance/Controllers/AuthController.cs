
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
    private readonly AppDbContext _dbContext;

    public AuthController(ITokenService tokenService, UserManager<ApplicationUser> userManager, AppDbContext dbContext )
    {
        _tokenService = tokenService;
        _userManager = userManager;
        _dbContext = dbContext;

    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        //var user = await _userManager.FindByEmailAsync(loginDto.Email);


         var user = await _dbContext.Set<ApplicationUser>()
            .FromSqlRaw("SELECT * FROM AspNetUsers WHERE Email = {0}", loginDto.Email)
            
            .FirstOrDefaultAsync();
        if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
        {
            var roles = await _userManager.GetRolesAsync(user);
            var token = _tokenService.GenerateJwtToken(user, roles);
            return Ok(new { token });
        }
        else
        {
            return Unauthorized(new { message = "Invalid email or password" });
        }

    }


}

