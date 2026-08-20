using AuthService.Data;
using AuthService.DTOs;
using AuthService.Models;
using AuthService.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace AuthService.Tests
{
    // Tests the basic user creation and login features of AuthService.
    public class AuthServiceTests
    {
        private AuthDbContext _context = null!;
        private AuthenticationService _service = null!;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<AuthDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new AuthDbContext(options);

            var passwordHasher = new PasswordHasher<User>();
            var jwtService = new TestJwtService();

            _service = new AuthenticationService(
                _context,
                passwordHasher,
                jwtService);
        }

        // Checks that a user is saved when valid data is provided.
        [Test]
        public async Task CreateUserAsync_ValidData_CreatesUser()
        {
            var request = new CreateUserDto
            {
                Name = "Amit Sharma",
                Email = "amit@worksphere.com",
                Role = "Employee",
                Password = "Amit@123"
            };

            await _service.CreateUserAsync(request);

            var user = await _context.Users
                .FirstOrDefaultAsync(item => item.Email == request.Email);

            Assert.That(user, Is.Not.Null);
            Assert.That(user!.Role, Is.EqualTo("Employee"));
            Assert.That(user.PasswordHash, Is.Not.EqualTo(request.Password));
        }

        // Checks that duplicate email addresses are not allowed.
        [Test]
        public async Task CreateUserAsync_DuplicateEmail_ThrowsException()
        {
            var request = new CreateUserDto
            {
                Name = "Amit Sharma",
                Email = "amit@worksphere.com",
                Role = "Employee",
                Password = "Amit@123"
            };

            await _service.CreateUserAsync(request);

            var exception = Assert.ThrowsAsync<Exception>(async () =>
                await _service.CreateUserAsync(request));

            Assert.That(exception!.Message, Is.EqualTo("User with this email already exists."));
        }

        // Checks that login works with the correct email and password.
        [Test]
        public async Task LoginAsync_CorrectCredentials_ReturnsToken()
        {
            var createRequest = new CreateUserDto
            {
                Name = "Amit Sharma",
                Email = "amit@worksphere.com",
                Role = "Employee",
                Password = "Amit@123"
            };

            await _service.CreateUserAsync(createRequest);

            var loginRequest = new LoginRequestDto
            {
                Email = "amit@worksphere.com",
                Password = "Amit@123"
            };

            var result = await _service.LoginAsync(loginRequest);

            Assert.That(result.Email, Is.EqualTo("amit@worksphere.com"));
            Assert.That(result.Role, Is.EqualTo("Employee"));
            Assert.That(result.Token, Is.EqualTo("test-token"));
        }

        // Checks that login fails when the password is incorrect.
        [Test]
        public async Task LoginAsync_IncorrectPassword_ThrowsException()
        {
            var createRequest = new CreateUserDto
            {
                Name = "Amit Sharma",
                Email = "amit@worksphere.com",
                Role = "Employee",
                Password = "Amit@123"
            };

            await _service.CreateUserAsync(createRequest);

            var loginRequest = new LoginRequestDto
            {
                Email = "amit@worksphere.com",
                Password = "WrongPassword"
            };

            var exception = Assert.ThrowsAsync<Exception>(async () =>
                await _service.LoginAsync(loginRequest));

            Assert.That(exception!.Message, Is.EqualTo("Invalid email or password."));
        }

        // Returns a simple token for login tests.
        private class TestJwtService : IJwtService
        {
            public string GenerateToken(int userId, string name, string email, string role)
            {
                return "test-token";
            }
        }
    }
}
