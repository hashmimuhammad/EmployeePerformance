namespace EmployeePerformance.Interfaces
{
    public interface ILeaveRepository
    {
        Task<IEnumerable<LeaveRequest>> GetAllAsync();
        Task<IEnumerable<LeaveRequest>> GetByEmployeeIdAsync(int employeeId);
        Task<LeaveRequest> GetByIdAsync(int leaveId);
        Task AddAsync(LeaveRequest leaveRequest);
        Task UpdateAsync(LeaveRequest leaveRequest);
        Task<bool> DeleteAsync(int id);



    }
}
