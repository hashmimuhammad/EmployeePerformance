
using EmployeePerformance.Dtos;
using EmployeePerformance.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IEmployeeRepository
{
    Task<IEnumerable<Employee>> GetAllEmployeesAsync();
    Task<Employee> GetEmployeeByIdAsync(int id);
    Task<Employee> GetEmployeeByEmailAsync(string email);
    Task<Employee> AddEmployeeAsync(CreateEmployeeDto employeeDto);
    Task<Employee> UpdateEmployeeAsync(int id, UpdateEmployeeDto employeeDto);
    Task<bool> DeleteEmployeeAsync(int id);
    Task<bool> ApplySalaryIncrementAsync(int id);
    public Task<bool> AnyAdminExistsAsync();
    Task<IEnumerable<PerformanceReview>> GetPerformanceReviewsByEmployeeIdAsync(int employeeId);
}

