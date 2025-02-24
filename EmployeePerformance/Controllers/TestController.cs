using EmployeePerformance.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EmployeePerformance.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly IEmployeeRepository _empRepo;

        // Constructor for Dependency Injection
        public TestController(IEmployeeRepository employeeRepository)
        {
            _empRepo = employeeRepository;
        }

        // POST API for Employee Registration (Requires Admin Role)
        [HttpPost("register")]
        [Authorize(policy: "Admin")]  // Only accessible by Admin role
        public async Task<IActionResult> AddEmployee([FromBody] CreateEmployeeDto employeeDto)
        {
            // If the employeeDto is null, return a bad request response
            if (employeeDto == null)
                return BadRequest(new { message = "Invalid data" });

            // Add the new employee via repository method
            var newEmployee = await _empRepo.AddEmployeeAsync(employeeDto);
            if (newEmployee == null)
                return BadRequest(new { message = "Failed to create employee" });

            // Return CreatedAtAction with the new employee's ID and details
            return CreatedAtAction(nameof(GetEmployeeById), new { id = newEmployee.EmployeeId }, newEmployee);
        }

        // GET API to Retrieve Employee by ID (Public Access with JWT Token)
        [HttpGet("{id}")]
        [Authorize(policy:"Admin")] // Any authenticated user (Admin or Employee)
        public async Task<IActionResult> GetEmployeeById(int id)
        {
            var employee = await _empRepo.GetEmployeeByIdAsync(id);
            if (employee == null)
                return NotFound(new { message = "Employee not found" });

            return Ok(employee);
        }
    }
}
