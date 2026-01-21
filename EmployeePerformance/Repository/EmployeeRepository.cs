using Dapper;
using EmployeePerformance.Data;
using EmployeePerformance.Dtos;
using EmployeePerformance.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;


public class EmployeeRepository : IEmployeeRepository
{
    private readonly AppDbContext _context;
    private readonly IDbConnection _connection;

    public EmployeeRepository(AppDbContext context, IDbConnection connection)
    {
        _context = context;
        _connection = connection;
    }


    public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
    {
        //var employee = _context.Employees.Find(1);  // Lazy loading

        string sql = "SELECT * FROM Employees WHERE IsActive = 1";

        var employees = await _connection.QueryAsync<Employee>(sql);
        return employees;
        
    }   

    public async Task<Employee> GetEmployeeByIdAsync(int id)
    {
        string sql = @"SELECT * FROM Employees 
                   WHERE EmployeeId = @Id AND IsActive = 1";

        var employee = await _connection.QueryFirstOrDefaultAsync<Employee>(sql, new { Id = id });
        return employee;
        
    }

    public async Task<Employee> GetEmployeeByEmailAsync(string email)
    {
        string sql = @"
        SELECT * FROM Employees
        WHERE Email = @Email AND IsActive = 1";

        var employee = await _connection.QueryFirstOrDefaultAsync<Employee>(sql, new { Email = email });
        return employee;
        
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


        string sql = @"
        INSERT INTO Employees (FullName, Email, PasswordHash, Department, Role, JoiningDate, CurrentSalary, IsActive)
        VALUES (@FullName, @Email, @PasswordHash, @Department, @Role, @JoiningDate, @CurrentSalary, @IsActive);
        SELECT CAST(SCOPE_IDENTITY() AS int);";

        var id = await _connection.ExecuteScalarAsync<int>(sql, newEmployee);
        newEmployee.EmployeeId = id;
        //_context.Employees.Add(newEmployee);
        //await _context.SaveChangesAsync();

        return newEmployee;
    }

    
    public async Task<Employee> UpdateEmployeeAsync(int id, UpdateEmployeeDto employeeDto)
    {
     

        string selectSql = "SELECT * FROM Employees WHERE EmployeeId = @Id AND IsActive = 1";
        var employee = await _connection.QueryFirstOrDefaultAsync<Employee>(selectSql, new { Id = id });
        if (employee == null)
        {
            return null;
        }

         var fullName = employee.FullName = employeeDto.FullName ?? employee.FullName;
        var currentSalary = employee.CurrentSalary = employeeDto.CurrentSalary ?? employee.CurrentSalary;

        //await _context.SaveChangesAsync();

        string updateSql = @"
        UPDATE Employees
        SET FullName = @FullName,
            CurrentSalary = @CurrentSalary
        WHERE EmployeeId = @Id";

        await _connection.ExecuteAsync(updateSql, new { FullName = fullName, CurrentSalary = currentSalary, Id = id });

        
        employee.FullName = fullName;
        employee.CurrentSalary = currentSalary;
        return employee;
    }
    public async Task<bool> DeleteEmployeeAsync(int id)
    {
        
        string selectSql = "SELECT * FROM Employees WHERE EmployeeId = @Id AND IsActive = 1";
        var employee = await _connection.QueryFirstOrDefaultAsync<Employee>(selectSql, new { Id = id });

        if (employee == null)
            return false;

        
        string updateSql = "UPDATE Employees SET IsActive = 0 WHERE EmployeeId = @Id";
        await _connection.ExecuteAsync(updateSql, new { Id = id });
        return true;
    }
    public async Task<bool> ApplySalaryIncrementAsync(int id)
    {
        

        string getEmployeeSql = "SELECT * FROM Employees WHERE EmployeeId = @Id AND IsActive = 1";
        var employee = await _connection.QueryFirstOrDefaultAsync<Employee>(getEmployeeSql, new { Id = id });
        if (employee == null) return false;

        
        string getReviewSql = @"
        SELECT TOP 1 * 
        FROM PerformanceReviews
        WHERE EmployeeId = @Id AND IsActive = 1
        ORDER BY ReviewDate DESC";

        var lastReview = await _connection.QueryFirstOrDefaultAsync<PerformanceReview>(getReviewSql, new { Id = id });
        if (lastReview == null || (DateTime.UtcNow - lastReview.ReviewDate).TotalDays > 180)
            return false;

        
        decimal newSalary = employee.CurrentSalary;

        if (lastReview.PerfomanceScore >= 8)
            newSalary *= 1.10M;
        else if (lastReview.PerfomanceScore >= 5)
            newSalary *= 1.05M;
        else
            return false;

        
        string updateSalarySql = "UPDATE Employees SET CurrentSalary = @NewSalary WHERE EmployeeId = @Id";
        await _connection.ExecuteAsync(updateSalarySql, new { NewSalary = newSalary, Id = id });
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


    public async Task<IEnumerable<Object>> GetEmployeeWithReview()
    {
        var result = await (
            from e in _context.Employees
            join r in _context.PerformanceReviews on e.EmployeeId equals r.EmployeeId
            select new
            {
                e.FullName,
                e.Email,
                e.Department,
                e.CurrentSalary,
                r.PerfomanceScore,
                r.Comments,
                r.ReviewDate,
                
            }).ToListAsync();
        //string sql = @"
        //SELECT 
        //    e.FullName,
        //    e.Email,
        //    e.Department,
        //    e.CurrentSalary,
        //    r.PerfomanceScore,
        //    r.Comments
        //FROM Employees e
        //INNER JOIN PerformanceReviews r ON e.EmployeeId = r.EmployeeId
        //WHERE e.IsActive = 1 AND r.IsActive = 1";

        //var result = await _connection.QueryAsync<object>(sql);
        return result;

    }

    public async  Task<object> GetEmployeeWithLeaveRequest()
    {


        var result = await (from e in _context.Employees
                            join l in _context.LeaveRequests on e.EmployeeId equals l.EmployeeId
                            select new
                            {
                                e.FullName,
                                e.Email,
                                l.LeaveType,
                                l.StartgDate,
                                l.EndDate,
                                l.Reason
                            }).ToListAsync();


        var leftJoin = await (
                from e in _context.Employees
                join l in _context.LeaveRequests
                    on e.EmployeeId equals l.EmployeeId into employeeLeaves
                from l in employeeLeaves.DefaultIfEmpty() 
                select new
                {
                    e.FullName,
                    e.Email,
                    LeaveType = l != null ? l.LeaveType : null,
                    StartDate = l != null ? l.StartgDate : (DateTime?)null,
                    EndDate = l != null ? l.EndDate : (DateTime?)null,
                    Reason = l != null ? l.Reason : null

                })
                .ToListAsync();

        var rightJoin = await (
                from l in _context.LeaveRequests
                join e in _context.Employees
                    on l.EmployeeId equals e.EmployeeId into leaveEmployees
                from e in leaveEmployees.DefaultIfEmpty() 
                select new
                {
                    FullName = e != null ? e.FullName : null,
                    Email = e != null ? e.Email : null,
                    l.LeaveType,
                    l.StartgDate,
                    l.EndDate,
                    l.Reason
                })
                .ToListAsync();

        var lembdaJoin = _context.Employees.Join(_context.LeaveRequests,
            e => e.EmployeeId,
            l => l.EmployeeId,
            (e, l) => new
            {
                e.FullName,
                e.Email,
                l.LeaveType,
                l.StartgDate,
                l.EndDate,
                l.Reason
            }).ToList();


        //string sql = @"
        ////SELECT 
        ////    e.FullName,
        ////    e.Email,
        ////    l.LeaveType,
        ////    l.StartgDate,
        ////    l.EndDate,
        ////    l.Reason
        ////FROM Employees e
        ////INNER JOIN LeaveRequests l ON e.EmployeeId = l.EmployeeId
        ////WHERE e.IsActive = 1 AND l.IsActive = 1";

        //var result = await _connection.QueryAsync<object>(sql);

        return result;

    }

    public async Task<Object> GetEmployeeWithReviewAndRequest()
    {
        var fullDetail = await (from e in _context.Employees
                              join r in _context.PerformanceReviews on e.EmployeeId equals r.EmployeeId
                              join l in _context.LeaveRequests on e.EmployeeId equals l.EmployeeId
                              select new
                              {
                                  e.FullName,
                                  e.Email,
                                  e.Department,
                                  e.CurrentSalary,
                                  r.PerfomanceScore,
                                  r.Comments,
                                  r.ReviewDate,
                                  l.LeaveType,
                                  l.StartgDate,
                                  l.EndDate,
                                  l.Reason
                              }).ToListAsync();
        return fullDetail;
    }
}


