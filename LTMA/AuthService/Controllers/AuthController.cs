using System.Security.Claims;
using AuthService.DTOs;
using AuthService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers
{
    // Handles authentication-related HTTP requests.
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // Logs a user into the system.
        //
        // POST: /api/Auth/login
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(
            LoginRequestDto request)
        {
            var result =
                await _authService.LoginAsync(request);

            return Ok(result);
        }

        // Changes the password of the logged-in user.
        //
        // POST: /api/Auth/change-password
        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword(
            ChangePasswordDto request)
        {
            // The JWT contains the user's ID as a claim.
            var userIdClaim =
                User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            var userId =
                int.Parse(userIdClaim.Value);

            await _authService.ChangePasswordAsync(
                userId,
                request);

            return Ok(new
            {
                message = "Password changed successfully."
            });
        }

        // Returns information about the currently
        // logged-in user.
        //
        // GET: /api/Auth/me
        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userIdClaim =
                User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            var userId =
                int.Parse(userIdClaim.Value);

            var result =
                await _authService.GetUserAsync(userId);

            return Ok(result);
        }
    }
}