using EmployeePerformance.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;
using EmployeePerformance.Dtos;

public interface IEmployeeRepository
{
    Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync(); 
    Task<EmployeeDto> GetEmployeeByIdAsync(int id);
    Task<EmployeeDto> AddEmployeeAsync(CreateEmployeeDto employeeDto);
    Task<EmployeeDto> UpdateEmployeeAsync(int id, UpdateEmployeeDto employeeDto);
    Task<bool> ApplySalaryIncrementAsync(int employeeId);
    Task<bool> DeleteEmployeeAsync(int id);
    
}

