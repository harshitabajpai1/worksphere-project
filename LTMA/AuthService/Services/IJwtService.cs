namespace AuthService.Services
{
    // Defines the operation required to generate
    // a JWT token for an authenticated user.
    public interface IJwtService
    {
        string GenerateToken(
            int userId,
            string name,
            string email,
            string role);
    }
}