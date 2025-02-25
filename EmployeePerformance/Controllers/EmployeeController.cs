using EmployeePerformance.Data;
using EmployeePerformance.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;


[Route("api/[controller]")]
[ApiController]
public class EmployeesController : ControllerBase
{
   
    private readonly IEmployeeRepository _empRepo;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly AppDbContext _dbContext;

    public EmployeesController(
        IEmployeeRepository employeeRepository,
        UserManager<ApplicationUser> userManager,
        AppDbContext dbContext,
        RoleManager<IdentityRole> roleManager)
    {
        _empRepo = employeeRepository;
        _userManager = userManager;
        _roleManager = roleManager;
        _dbContext = dbContext;

    }

    [HttpGet("all")]
    [Authorize(policy: "Admin")]
    public async Task<IActionResult> GetAllEmployees()
    {
        //var employees = await _empRepo.GetAllEmployeesAsync();
        var employees = await _dbContext.Employees
            .FromSqlRaw("SELECT * FROM Employees")
            .ToListAsync();



        return Ok(employees);
    }

    //[HttpGet("{email}")]
    //[Authorize(policy: "AdminOrEmployee")]
    //public async Task<IActionResult> GetEmployeeByEmail(string email)
    //{
    //    var employee = await _empRepo.GetEmployeeByEmailAsync(email);
    //    if (employee == null)
    //        return NotFound(new { message = "Employee not found" });

    //    return Ok(employee);
    //}


    [HttpGet("{email}")]
    [Authorize(Roles = "Employee,Admin")]
    public async Task<IActionResult> GetEmployeeByEmail(string email)
    {

        var loggedInUserEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;


        if (User.IsInRole("Employee") && loggedInUserEmail != email)
        {
            return Forbid();
        }

        // var employee = await _empRepo.GetEmployeeByEmailAsync(email);
        var employee = await _dbContext.Employees
             .FromSqlRaw("SELECT * FROM Employees WHERE Email = {0}", email)
             .FirstOrDefaultAsync();
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

        //var existingUser = await _userManager.FindByEmailAsync(employeeDto.Email);
        var existingUser = await _dbContext.Employees
            .FromSqlRaw("Select * From Employees Where Email={0}, employeeDto.Email")
            .FirstOrDefaultAsync();

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

        return CreatedAtAction(nameof(GetEmployeeByEmail), new { id = newEmployee.EmployeeId }, newEmployee);
    }

    [HttpPut("{id}")]
    [Authorize(policy: "Admin")]
    public async Task<IActionResult> UpdateEmployee(int id, [FromBody] UpdateEmployeeDto employeeDto)
    {
        //if (employeeDto == null)
        //    return BadRequest(new { message = "Invalid data" });

        //var updatedEmployee = await _empRepo.UpdateEmployeeAsync(id, employeeDto);
        //if (updatedEmployee == null)
        //    return NotFound(new { message = "Employee not found or inactive" });

        //return Ok(updatedEmployee);
        if (employeeDto == null)
            return BadRequest(new { message = "Invalid data" });

        var result = await _dbContext.Database.ExecuteSqlRawAsync(
            @"UPDATE Employees 
          SET FullName = @FullName, 
              Email = @Email, 
              Department = @Department, 
              CurrentSalary = @CurrentSalary 
              
          WHERE EmployeeId = @id AND IsActive = 1;",
            new SqlParameter("@FullName", employeeDto.FullName),
            new SqlParameter("@Email", employeeDto.Email),
            new SqlParameter("@Department", employeeDto.Department),
            new SqlParameter("@CurrentSalary", employeeDto.CurrentSalary),
            
            new SqlParameter("@id", id)
        );

        if (result == 0) 
            return NotFound(new { message = "Employee not found or inactive" });

        var updatedEmployee = await _dbContext.Employees
            .FromSqlRaw("SELECT * FROM Employees WHERE EmployeeId = @id", new SqlParameter("@id", id))
            .FirstOrDefaultAsync();

        return Ok(updatedEmployee);
    }

    [HttpDelete("{id}")]

     [Authorize(policy: "Admin")]
    public async Task<IActionResult> DeleteEmployee(int id)
    {
        //var result = await _empRepo.DeleteEmployeeAsync(id);
        var result = await _dbContext.Database.ExecuteSqlRawAsync(
            "DELETE FROM Employees WHERE EmployeeId = @id",
            new SqlParameter("@id", id)
);

        if (result == 0)
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







