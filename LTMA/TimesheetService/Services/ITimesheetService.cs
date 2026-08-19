using TimesheetService.DTOs;

namespace TimesheetService.Services
{
    // Defines the operations related to timesheets.
    public interface ITimesheetService
    {
        Task<TimesheetResponseDto> CreateTimesheetAsync(CreateTimesheetDto request);

        Task<List<TimesheetResponseDto>> GetMyTimesheetsAsync();

        Task<List<TimesheetResponseDto>> GetTeamTimesheetsAsync();

        Task<TimesheetResponseDto> ApproveTimesheetAsync(int id);

        Task<TimesheetResponseDto> RejectTimesheetAsync(int id);

        Task<List<TimesheetResponseDto>> GetAllTimesheetsAsync();
    }
}
