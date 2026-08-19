using LeaveService.DTOs;
using LeaveService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LeaveService.Controllers
{
    // Handles leave-related HTTP requests.
    [ApiController]
    [Route("api/[controller]")]
    public class LeaveController : ControllerBase
    {
        private readonly ILeaveService _service;

        public LeaveController(ILeaveService service)
        {
            _service = service;
        }

        // Creates a leave request for the logged-in employee.
        // POST: /api/Leave
        [HttpPost]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> Apply(ApplyLeaveDto request)
        {
            var leave = await _service.ApplyLeaveAsync(request);

            return Ok(leave);
        }

        // Returns leave requests for the logged-in employee.
        // GET: /api/Leave/my
        [HttpGet("my")]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> GetMyLeaves()
        {
            var leaves = await _service.GetMyLeavesAsync();

            return Ok(leaves);
        }

        // Returns leave requests for the logged-in manager's team.
        // GET: /api/Leave/team
        [HttpGet("team")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> GetTeamLeaves()
        {
            var leaves = await _service.GetTeamLeavesAsync();

            return Ok(leaves);
        }

        // Approves a pending leave request.
        // PUT: /api/Leave/1/approve
        [HttpPut("{id}/approve")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Approve(int id)
        {
            var leave = await _service.ApproveLeaveAsync(id);

            return Ok(leave);
        }

        // Rejects a pending leave request.
        // PUT: /api/Leave/1/reject
        [HttpPut("{id}/reject")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Reject(int id)
        {
            var leave = await _service.RejectLeaveAsync(id);

            return Ok(leave);
        }

        // Returns all leave requests for SuperAdmin.
        // GET: /api/Leave
        [HttpGet]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> GetAll()
        {
            var leaves = await _service.GetAllLeavesAsync();

            return Ok(leaves);
        }
    }
}
