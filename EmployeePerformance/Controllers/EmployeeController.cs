using EmployeePerformance.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;



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


    [HttpGet]
    [Authorize(policy: "Admin")]
    public async Task<IActionResult> GetAllEmployees()
    {
        var employees = await _empRepo.GetAllEmployeesAsync();
        return Ok(employees);
    }

    [HttpGet("{id}")]
    [Authorize(policy: "AdminOrEmployee")]
    public async Task<IActionResult> GetEmployeeById(int id)
    {
        var employee = await _empRepo.GetEmployeeByIdAsync(id);
        if (employee == null)
            return NotFound(new { message = "Employee not found" });

        return Ok(employee);
    }

    [HttpPost]
    [Route("register")]
    [Authorize(policy: "Admin")] 
    public async Task<IActionResult> RegisterUser([FromBody] CreateEmployeeDto employeeDto)
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

        return CreatedAtAction(nameof(GetEmployeeById), new { id = newEmployee.EmployeeId }, newEmployee);
    }

    [HttpPut("{id}")]
    [Authorize(policy: "Admin")]
    public async Task<IActionResult> UpdateEmployee(int id, [FromBody] UpdateEmployeeDto employeeDto)
    {
        if (employeeDto == null)
            return BadRequest(new { message = "Invalid data" });

        var updatedEmployee = await _empRepo.UpdateEmployeeAsync(id, employeeDto);
        if (updatedEmployee == null)
            return NotFound(new { message = "Employee not found or inactive" });

        return Ok(updatedEmployee);
    }

    [HttpDelete("{id}")]
     [Authorize(policy: "Admin")]
    public async Task<IActionResult> DeleteEmployee(int id)
    {
        var result = await _empRepo.DeleteEmployeeAsync(id);
        if (!result)
            return NotFound(new { message = "Employee not found" });

        return Ok(new { message = "Employee deleted (soft delete applied)" });
    }
    


    
    [HttpPut("{id}/increment")]
    [Authorize(policy: "Admin")]
    public async Task<IActionResult> ApplySalaryIncrement(int id)
    {
        var result = await _empRepo.ApplySalaryIncrementAsync(id);
        if (!result)
            return BadRequest(new { message = "Salary increment failed (No recent performance review)" });

        return Ok(new { message = "Salary increment applied successfully" });
    }
}







