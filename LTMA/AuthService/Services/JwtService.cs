using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace AuthService.Services
{
    // Responsible for creating JWT tokens.
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(
            int userId,
            string name,
            string email,
            string role)
        {
            // Read JWT configuration from appsettings.json.
            var key = _configuration["Jwt:Key"];
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];

            var expiryMinutes =
                Convert.ToDouble(
                    _configuration["Jwt:ExpiryMinutes"]);

            // Claims are information about the logged-in user.
            var claims = new List<Claim>
            {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    userId.ToString()),

                new Claim(
                    ClaimTypes.Name,
                    name),

                new Claim(
                    ClaimTypes.Email,
                    email),

                new Claim(
                    ClaimTypes.Role,
                    role)
            };

            // Convert the secret key into a security key.
            var securityKey =
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(key!));

            // Use the key to digitally sign the JWT.
            var credentials =
                new SigningCredentials(
                    securityKey,
                    SecurityAlgorithms.HmacSha256);

            // Create the token.
            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    expiryMinutes),
                signingCredentials: credentials);

            // Convert the token object into the string
            // that will be returned to the client.
            return new JwtSecurityTokenHandler()
                .WriteToken(token);
        }
    }
}