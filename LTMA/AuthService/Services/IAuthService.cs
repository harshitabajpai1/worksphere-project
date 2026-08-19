using AuthService.DTOs;

namespace AuthService.Services
{
    // Defines the authentication operations provided
    // by the authentication service.
    public interface IAuthService
    {
        // Logs a user into the system.
        Task<LoginResponseDto> LoginAsync(
            LoginRequestDto request);

        // Changes the password of the currently
        // logged-in user.
        Task ChangePasswordAsync(
            int userId,
            ChangePasswordDto request);

        // Gets the details of the currently
        // logged-in user.
        Task<LoginResponseDto> GetUserAsync(
            int userId);
    }
}