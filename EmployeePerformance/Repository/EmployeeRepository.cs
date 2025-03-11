

using EmployeePerformance.Data;
using EmployeePerformance.Dtos;
using EmployeePerformance.Models;
using Microsoft.EntityFrameworkCore;


public class EmployeeRepository : IEmployeeRepository
{
    private readonly AppDbContext _context;

    public EmployeeRepository(AppDbContext context)
    {
        _context = context;
    }

    
    public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
    {
        return await _context.Employees.Where(e => e.IsActive).ToListAsync();
    }

    public async Task<Employee> GetEmployeeByIdAsync(int id)
    {
        return await _context.Employees.FirstOrDefaultAsync(e => e.EmployeeId == id && e.IsActive);
    }

    public async Task<Employee> GetEmployeeByEmailAsync(string email)
    {
        return await _context.Employees.FirstOrDefaultAsync(e => e.Email == email && e.IsActive);
    }

   
    public async Task<Employee> AddEmployeeAsync(CreateEmployeeDto employeeDto)
    {
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(employeeDto.Password);

        var newEmployee = new Employee
        {
            FullName = employeeDto.FullName,
            Email = employeeDto.Email,
            PasswordHash = hashedPassword,
            Department = employeeDto.Department,
            Role = employeeDto.Role,
            JoiningDate = employeeDto.JoiningDate,
            CurrentSalary = employeeDto.CurrentSalary,
            IsActive = true
        };

        _context.Employees.Add(newEmployee);
        await _context.SaveChangesAsync();
        return newEmployee;
    }

    
    public async Task<Employee> UpdateEmployeeAsync(int id, UpdateEmployeeDto employeeDto)
    {
        var employee = await _context.Employees.FirstOrDefaultAsync(e => e.EmployeeId == id && e.IsActive);
        if (employee == null) return null;

        employee.FullName = employeeDto.FullName ?? employee.FullName;
        employee.CurrentSalary = employeeDto.CurrentSalary ?? employee.CurrentSalary;

        await _context.SaveChangesAsync();
        return employee;
    }
    public async Task<bool> DeleteEmployeeAsync(int id)
    {
        var employee = await _context.Employees.FirstOrDefaultAsync(e => e.EmployeeId == id && e.IsActive);
        if (employee == null) return false;

       
        employee.IsActive = false;
        await _context.SaveChangesAsync();
        return true;
    }
    public async Task<bool> ApplySalaryIncrementAsync(int id)
    {
        var employee = await _context.Employees.FirstOrDefaultAsync(e => e.EmployeeId == id && e.IsActive);
        if (employee == null) return false;

        var lastReview = await _context.PerformanceReviews
            .Where(r => r.EmployeeId == id)
            .OrderByDescending(r => r.ReviewDate)
            .FirstOrDefaultAsync();

        if (lastReview == null || (DateTime.UtcNow - lastReview.ReviewDate).TotalDays > 180)
        {
            return false; 
        }

     
        if (lastReview.PerfomanceScore >= 8)
        {
            employee.CurrentSalary *= 1.10M; 
        }
        else if (lastReview.PerfomanceScore >= 5)
        {
            employee.CurrentSalary *= 1.05M; 
        }
        else
        {
            return false; 
        }

        await _context.SaveChangesAsync();
        return true;
    }
    public async Task<IEnumerable<PerformanceReview>> GetPerformanceReviewsByEmployeeIdAsync(int employeeId)
    {
        return await _context.PerformanceReviews
            .Where(r => r.EmployeeId == employeeId)
            .ToListAsync();
    }
    public async Task<bool> AnyAdminExistsAsync()
    {
        return await _context.Employees.AnyAsync(e => e.Role == "Admin" && e.IsActive);
    }
}


