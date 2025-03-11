using EmployeePerformance.Dtos;
using EmployeePerformance.Interfaces;

namespace EmployeePerformance.Services
{
    public class LeaveRequestService
    {
        private readonly ILeaveRepository _leaveRepo;

        public LeaveRequestService(ILeaveRepository leaveRepo)
        {
            _leaveRepo = leaveRepo;
        }
        public async Task<bool> ValidateLeaveRequest(CreateLeaveRequestDto leaveRequestDto)
        {
            if (leaveRequestDto.StartDate >= leaveRequestDto.EndDate)
            {
                return false;
            }
            if (leaveRequestDto.StartDate < DateTime.UtcNow)
            {
                return false;
            }
            if ((leaveRequestDto.EndDate - leaveRequestDto.StartDate).TotalDays > 30)
            {
                return false;
            }
            return true;
        }
        public async Task ProcessLeaveApproval(int leaveId, string status)
        {
            var leaveRequest = await _leaveRepo.GetByIdAsync(leaveId);
            if(leaveRequest == null)
            {
                throw new Exception("Leave request not found");
            }
            if (leaveRequest.Status != "pending")
            {
                throw new Exception("Leave already process");
            }

            leaveRequest.Status = status;
            await _leaveRepo.UpdateAsync(leaveRequest);
            
        }
    }
}
