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

    //public async Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync()
    //{
    //    return await _context.Employees
    //        .Where(e => e.IsActive)
    //        .Select(e => new EmployeeDto
    //        {
    //            EmployeeId = e.EmployeeId,
    //            FullName = e.FullName,
    //            Email = e.Email,
    //            DateOfBirth = e.DateOfBirth,
    //            Department = e.Department,
    //            JoiningDate = e.JoiningDate,
    //            CurrentSalary = e.CurrentSalary,
    //            IsActive = e.IsActive
    //        })
    //        .ToListAsync();
    //}

    public async Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync()
    {
        return await _context.Employees
            .Select(e => new EmployeeDto
            {
                EmployeeId = e.EmployeeId,
                FullName = e.FullName,
                Email = e.Email,
                DateOfBirth = e.DateOfBirth,
                Department = e.Department,
                JoiningDate = e.JoiningDate,
                CurrentSalary = e.CurrentSalary,
                IsActive = e.IsActive  
            })
            .ToListAsync();
    }


    public async Task<EmployeeDto> GetEmployeeByIdAsync(int id)
    {
        var employee = await _context.Employees
            .Where(e => e.EmployeeId == id && e.IsActive)
            .Select(e => new EmployeeDto
            {
                EmployeeId = e.EmployeeId,
                FullName = e.FullName,
                Email = e.Email,
                DateOfBirth = e.DateOfBirth,
                Department = e.Department,
                JoiningDate = e.JoiningDate,
                CurrentSalary = e.CurrentSalary,
                IsActive = e.IsActive
            })
            .FirstOrDefaultAsync();

        return employee;
    }

    public async Task<bool> ApplySalaryIncrementAsync(int employeeId)
    {
        var employee = await _context.Employees.FindAsync(employeeId);
        if (employee == null || !employee.IsActive) return false;

        var latestReview = await _context.PerformanceReviews
            .Where(r => r.EmployeeId == employeeId)
            .OrderByDescending(r => r.ReviewDate)
            .FirstOrDefaultAsync();

        if (latestReview == null || latestReview.ReviewDate < DateTime.UtcNow.AddMonths(-6))
            return false;

        decimal increment = 0;
        if (latestReview.PerfomanceScore >= 8)
            increment = employee.CurrentSalary * 0.10m;
        else if (latestReview.PerfomanceScore >= 5)
            increment = employee.CurrentSalary * 0.05m;

        employee.CurrentSalary += increment;
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteEmployeeAsync(int id)
    {
        var employee = await _context.Employees.FindAsync(id);
        if (employee == null) return false;

        employee.IsActive = false;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<EmployeeDto> AddEmployeeAsync(CreateEmployeeDto employeeDto)
    {
        var employee = new Employee
        {
            FullName = employeeDto.FullName,
            Email = employeeDto.Email,
            DateOfBirth = employeeDto.DateOfBirth,
            Department = employeeDto.Department,
            JoiningDate = employeeDto.JoiningDate ?? DateTime.Now,
            CurrentSalary = employeeDto.CurrentSalary,
            IsActive = true
        };

        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();

        return new EmployeeDto
        {
            EmployeeId = employee.EmployeeId,
            FullName = employee.FullName,
            Email = employee.Email,
            DateOfBirth = employee.DateOfBirth,
            Department = employee.Department,
            JoiningDate = employee.JoiningDate,
            CurrentSalary = employee.CurrentSalary,
            IsActive = employee.IsActive
        };
    }

    public async Task<EmployeeDto> UpdateEmployeeAsync(int id, UpdateEmployeeDto employeeDto)
    {
        var employee = await _context.Employees.FindAsync(id);
        if (employee == null || !employee.IsActive)
        {
            return null;
        }

        
        employee.FullName = employeeDto.FullName ?? employee.FullName;
        employee.Email = employeeDto.Email ?? employee.Email;
        employee.DateOfBirth = employeeDto.DateOfBirth ?? employee.DateOfBirth;
        employee.Department = employeeDto.Department ?? employee.Department;
        employee.CurrentSalary = employeeDto.CurrentSalary ?? employee.CurrentSalary;

        await _context.SaveChangesAsync();

        return new EmployeeDto
        {
            EmployeeId = employee.EmployeeId,
            FullName = employee.FullName,
            Email = employee.Email,
            DateOfBirth = employee.DateOfBirth,
            Department = employee.Department,
            JoiningDate = employee.JoiningDate,
            CurrentSalary = employee.CurrentSalary,
            IsActive = employee.IsActive
        };
    }

}

