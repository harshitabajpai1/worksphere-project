using AuthService.Data;
using AuthService.DTOs;
using AuthService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Services
{
    // Contains the business logic related to authentication.
    public class AuthenticationService : IAuthService
    {
        private readonly AuthDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IJwtService _jwtService;

        public AuthenticationService(
            AuthDbContext context,
            IPasswordHasher<User> passwordHasher,
            IJwtService jwtService)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _jwtService = jwtService;
        }

        public async Task<LoginResponseDto> LoginAsync(
            LoginRequestDto request)
        {
            // Find the user using the email address.
            var user = await _context.Users
                .FirstOrDefaultAsync(x =>
                    x.Email == request.Email);

            // Do not reveal whether the email or password
            // was incorrect.
            if (user == null)
            {
                throw new Exception(
                    "Invalid email or password.");
            }

            // An inactive user is not allowed to log in.
            if (!user.IsActive)
            {
                throw new Exception(
                    "User account is inactive.");
            }

            // Compare the entered password with the
            // password hash stored in the database.
            var passwordResult =
                _passwordHasher.VerifyHashedPassword(
                    user,
                    user.PasswordHash,
                    request.Password);

            if (passwordResult ==
                PasswordVerificationResult.Failed)
            {
                throw new Exception(
                    "Invalid email or password.");
            }

            // The credentials are valid, so generate a JWT.
            var token = _jwtService.GenerateToken(
                user.Id,
                user.Name,
                user.Email,
                user.Role);

            return new LoginResponseDto
            {
                Name = user.Name,
                Email = user.Email,
                Role = user.Role,
                Token = token
            };
        }

        public async Task ChangePasswordAsync(
            int userId,
            ChangePasswordDto request)
        {
            // Find the currently logged-in user.
            var user = await _context.Users
                .FirstOrDefaultAsync(x =>
                    x.Id == userId);

            if (user == null)
            {
                throw new Exception("User not found.");
            }

            // Verify the current password before allowing
            // the user to choose a new password.
            var passwordResult =
                _passwordHasher.VerifyHashedPassword(
                    user,
                    user.PasswordHash,
                    request.CurrentPassword);

            if (passwordResult ==
                PasswordVerificationResult.Failed)
            {
                throw new Exception(
                    "Current password is incorrect.");
            }

            // Hash the new password before storing it.
            user.PasswordHash =
                _passwordHasher.HashPassword(
                    user,
                    request.NewPassword);

            await _context.SaveChangesAsync();
        }

        public async Task<LoginResponseDto> GetUserAsync(
            int userId)
        {
            // Find the currently logged-in user.
            var user = await _context.Users
                .FirstOrDefaultAsync(x =>
                    x.Id == userId);

            if (user == null)
            {
                throw new Exception("User not found.");
            }

            // We don't return the password hash.
            return new LoginResponseDto
            {
                Name = user.Name,
                Email = user.Email,
                Role = user.Role,
                Token = string.Empty
            };
        }
    }
}