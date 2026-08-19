using LeaveService.DTOs;

namespace LeaveService.Services
{
    // Defines the operations related to leave requests.
    public interface ILeaveService
    {
        Task<LeaveResponseDto> ApplyLeaveAsync(ApplyLeaveDto request);

        Task<List<LeaveResponseDto>> GetMyLeavesAsync();

        Task<List<LeaveResponseDto>> GetTeamLeavesAsync();

        Task<LeaveResponseDto> ApproveLeaveAsync(int id);

        Task<LeaveResponseDto> RejectLeaveAsync(int id);

        Task<List<LeaveResponseDto>> GetAllLeavesAsync();
    }
}
