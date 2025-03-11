using EmployeePerformance.Data;
using EmployeePerformance.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace EmployeePerformance.Repository
{
    public class LeaveRepository : ILeaveRepository

    {
        private readonly AppDbContext _context;

        public LeaveRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(LeaveRequest leaveRequest)
        {
            _context.LeaveRequests.Add(leaveRequest);
            await _context.SaveChangesAsync();

        }

        public async Task<bool> DeleteAsync(int id)
        {
            var result = await _context.LeaveRequests.FirstOrDefaultAsync(e => e.LeaveId == id);
            if(result == null)
            {
                return false; 
            }
            _context.LeaveRequests.Remove(result);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<LeaveRequest>> GetAllAsync()
        {
            return await _context.LeaveRequests.ToListAsync();

        }

        public async Task<IEnumerable<LeaveRequest>> GetByEmployeeIdAsync(int employeeId)
        {
            return await _context.LeaveRequests.Where(e => e.EmployeeId == employeeId).ToListAsync();
        }

        public async Task<LeaveRequest> GetByIdAsync(int leaveId)
        {
            return await _context.LeaveRequests.FirstOrDefaultAsync(l => l.LeaveId == leaveId);
        }

        public async Task UpdateAsync(LeaveRequest leaveRequest)
        {
            _context.LeaveRequests.Update(leaveRequest);
            await _context.SaveChangesAsync();

        }
    }
}
