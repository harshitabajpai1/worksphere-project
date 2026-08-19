namespace AuthService.DTOs
{
    // This class represents the data that our API
    // will send back to the client after a successful login.
    //
    // We don't return the complete User object because
    // we don't want to expose database-related information
    // unnecessarily.
    public class LoginResponseDto
    {
        // User's name.
        public string Name { get; set; } = string.Empty;

        // User's email.
        public string Email { get; set; } = string.Empty;

        // User's role in the system.
        //
        // Examples:
        // SuperAdmin
        // Manager
        // Employee
        public string Role { get; set; } = string.Empty;

        // JWT token that will be generated after
        // successful authentication.
        //
        // Other services will later use this token
        // to identify and authorize the user.
        public string Token { get; set; } = string.Empty;
    }
}