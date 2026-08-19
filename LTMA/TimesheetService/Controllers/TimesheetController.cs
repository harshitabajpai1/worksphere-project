using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TimesheetService.DTOs;
using TimesheetService.Services;

namespace TimesheetService.Controllers
{
    // Handles timesheet-related HTTP requests.
    [ApiController]
    [Route("api/[controller]")]
    public class TimesheetController : ControllerBase
    {
        private readonly ITimesheetService _service;

        public TimesheetController(ITimesheetService service)
        {
            _service = service;
        }

        // Creates a timesheet for the logged-in employee.
        // POST: /api/Timesheet
        [HttpPost]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> Create(CreateTimesheetDto request)
        {
            try
            {
                var timesheet = await _service.CreateTimesheetAsync(request);

                return Ok(timesheet);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        // Returns timesheets for the logged-in employee.
        // GET: /api/Timesheet/my
        [HttpGet("my")]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> GetMyTimesheets()
        {
            try
            {
                var timesheets = await _service.GetMyTimesheetsAsync();

                return Ok(timesheets);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        // Returns timesheets for the logged-in manager's team.
        // GET: /api/Timesheet/team
        [HttpGet("team")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> GetTeamTimesheets()
        {
            try
            {
                var timesheets = await _service.GetTeamTimesheetsAsync();

                return Ok(timesheets);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        // Approves a pending timesheet.
        // PUT: /api/Timesheet/1/approve
        [HttpPut("{id}/approve")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Approve(int id)
        {
            try
            {
                var timesheet = await _service.ApproveTimesheetAsync(id);

                return Ok(timesheet);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        // Rejects a pending timesheet.
        // PUT: /api/Timesheet/1/reject
        [HttpPut("{id}/reject")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Reject(int id)
        {
            try
            {
                var timesheet = await _service.RejectTimesheetAsync(id);

                return Ok(timesheet);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        // Returns all timesheets for SuperAdmin.
        // GET: /api/Timesheet
        [HttpGet]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> GetAllTimesheets()
        {
            try
            {
                var timesheets = await _service.GetAllTimesheetsAsync();

                return Ok(timesheets);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }
    }
}
