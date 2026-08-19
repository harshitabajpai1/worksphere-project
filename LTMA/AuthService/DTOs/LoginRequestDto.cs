namespace AuthService.DTOs
{
    // This class represents the data that the client
    // sends when trying to log in.
    //
    // We use a DTO instead of directly exposing
    // our User database model.
    public class LoginRequestDto
    {
        // Email entered by the user.
        public string Email { get; set; } = string.Empty;

        // Password entered by the user.
        //
        // This is only received during login.
        // We will never store the plain password
        // in the database.
        public string Password { get; set; } = string.Empty;
    }
}