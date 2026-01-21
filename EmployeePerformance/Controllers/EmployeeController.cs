using EmployeePerformance.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class EmployeesController : ControllerBase
{

    private readonly IEmployeeRepository _empRepo;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;


    public EmployeesController(
        IEmployeeRepository employeeRepository,
        UserManager<ApplicationUser> userManager,

        RoleManager<IdentityRole> roleManager)
    {
        _empRepo = employeeRepository;
        _userManager = userManager;
        _roleManager = roleManager;


    }

    [HttpGet("all")]
    [Authorize(policy: "Admin")]
    public async Task<IActionResult> GetAllEmployees()
    {
        try
        {
            var employees = await _empRepo.GetAllEmployeesAsync();


            return Ok(employees);
        }catch(Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
    [HttpGet("EmployeeWithReview")]
    //[Authorize(policy: "Admin")]
    public async Task<IActionResult> GetEmployeeWithReview()
    {
        try
        {
            var employees = await _empRepo.GetEmployeeWithReview();


            return Ok(employees);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
    [HttpGet("EmployeeWithLeaveRequest")]
    //[Authorize(policy:"Admin")]

    public async Task<IActionResult> GetEmployeeWithLeaveRequest()
    {
        try
        {
            var employee = await _empRepo.GetEmployeeWithLeaveRequest();
            return Ok(employee);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }

    [HttpGet("FullDetails")]

    public async Task<IActionResult> GetEmployeeFullDetails()
    {
        try
        {
            var employees = await _empRepo.GetEmployeeWithReviewAndRequest();
            return Ok(employees);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }

    [HttpGet("{email}")]
    [Authorize(policy: "AdminOrEmployee")]
    public async Task<IActionResult> GetEmployeeByEmail(string email)
    {
        try
        {
            var employee = await _empRepo.GetEmployeeByEmailAsync(email);
            if (employee == null)
                return NotFound(new { message = "Employee not found" });

            return Ok(employee);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Unexpected error occured.", error = ex.Message });
        }
    }


    [HttpPost]
    [Route("register")]
    [Authorize(policy: "Admin")]
    public async Task<IActionResult> RegisterUser([FromBody] CreateEmployeeDto employeeDto)
    {
        try
        {
            if (employeeDto == null)
                return BadRequest(new { message = "Invalid data" });

            var existingUser = await _userManager.FindByEmailAsync(employeeDto.Email);


            if (existingUser != null)
            {
                return BadRequest(new { message = "Email already exists" });
            }

            if (employeeDto.Role != "Admin" && employeeDto.Role != "Employee")
            {
                return BadRequest(new { message = "Invalid role. Role must be either 'Admin' or 'Employee'." });
            }

            var newUser = new ApplicationUser
            {
                UserName = employeeDto.Email,
                Email = employeeDto.Email
            };

            var userResult = await _userManager.CreateAsync(newUser);
            if (!userResult.Succeeded)
            {
                return BadRequest(userResult.Errors);
            }

            await _userManager.AddToRoleAsync(newUser, employeeDto.Role);

            var newEmployee = await _empRepo.AddEmployeeAsync(employeeDto);

            return Ok(newEmployee);
        }
        catch (Exception ex)
        {

            return StatusCode(500, new { message = "unexpected error occurred while register new user.", error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    [Authorize(policy: "Admin")]
    public async Task<IActionResult> UpdateEmployee(int id, [FromBody] UpdateEmployeeDto employeeDto)
    {
        try
        {
            if (employeeDto == null)
                return BadRequest(new { message = "Invalid data" });

            var updatedEmployee = await _empRepo.UpdateEmployeeAsync(id, employeeDto);
            if (updatedEmployee == null)
                return NotFound(new { message = "Employee not found or inactive" });

            return Ok(updatedEmployee);
        } catch (Exception ex)
        {
            return StatusCode(500, new { message = "Unexpected error occured while update.", error = ex.Message });
        }

    }

    [HttpDelete("{id}")]

    [Authorize(policy: "Admin")]
    public async Task<IActionResult> DeleteEmployee(int id)
    {
        try {
            var result = await _empRepo.DeleteEmployeeAsync(id);


            if (result == null)
                return NotFound(new { message = "Employee not found" });

            return Ok(new { message = "Employee deleted " });
        }catch(Exception ex)
        {
            return StatusCode(500, new {message = "Unexpected error occured.", error=ex.Message});

        }

    }

       
    [HttpPut("{id}/increment")]
    [Authorize(policy: "Admin")]
    public async Task<IActionResult> ApplySalaryIncrement(int id)
    {
        try
        {
            var result = await _empRepo.ApplySalaryIncrementAsync(id);
            if (!result)
                return BadRequest(new { message = "Salary increment failed (No recent performance review)" });

            return Ok(new { message = "Salary increment applied successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new {message = "Unexpected error occured.", error = ex.Message});    
        }
    }
}
