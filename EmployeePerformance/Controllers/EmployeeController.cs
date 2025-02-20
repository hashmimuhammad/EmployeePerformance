using EmployeePerformance.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeRepository _empRepo;

    public EmployeesController(IEmployeeRepository employeeRepository)
    {
        _empRepo = employeeRepository;
    }


    [HttpGet]
    public async Task<IActionResult> GetAllEmployees()
    {
        var employees = await _empRepo.GetAllEmployeesAsync();
        return Ok(employees);
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> GetEmployeeById(int id)
    {
        var employee = await _empRepo.GetEmployeeByIdAsync(id);
        if (employee == null)
            return NotFound(new { message = "Employee not found" });

        return Ok(employee);
    }


    [HttpPost]
    public async Task<IActionResult> AddEmployee([FromBody] CreateEmployeeDto employeeDto)
    {
        if (employeeDto == null)
            return BadRequest(new { message = "Invalid data" });

        var newEmployee = await _empRepo.AddEmployeeAsync(employeeDto);
        return CreatedAtAction(nameof(GetEmployeeById), new { id = newEmployee.EmployeeId }, newEmployee);
    }


    [HttpPut("{id}")]
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
    public async Task<IActionResult> DeleteEmployee(int id)
    {
        var result = await _empRepo.DeleteEmployeeAsync(id);
        if (!result)
            return NotFound(new { message = "Employee not found" });

        return Ok(new { message = "Employee deleted (soft delete applied)" });
    }


    //[HttpPut("{id}/increment")]
    //public async Task<IActionResult> ApplySalaryIncrement(int id)
    //{
    //    var result = await _empRepo.ApplySalaryIncrementAsync(id);
    //    if (!result)
    //        return BadRequest(new { message = "Salary increment failed (No recent performance review)" });

    //    return Ok(new { message = "Salary increment applied successfully" });
    //}

    [HttpPut("{id}/increment")]
    public async Task<IActionResult> ApplySalaryIncrement(int id)
    {
        var result = await _empRepo.ApplySalaryIncrementAsync(id);
        if (!result)
            return BadRequest(new { message = "Salary increment failed (No recent performance review)" });

        return Ok(new { message = "Salary increment applied successfully" });
    }

}


