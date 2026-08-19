namespace AuthService.Models
{
    // This class represents a user in our WorkSphere system.
    //
    // Initially, we will only have the SuperAdmin.
    // Later, Employee Service will create Employee and Manager users.
    public class User
    {
        // Primary key of the User table.
        public int Id { get; set; }

        // Name of the user.
        public string Name { get; set; } = string.Empty;

        // Email will be used for login.
        // We will make this unique in the database.
        public string Email { get; set; } = string.Empty;

        // We NEVER store the actual password.
        //
        // Example:
        // User enters: Admin@123
        // We store:   a hashed version of Admin@123
        public string PasswordHash { get; set; } = string.Empty;

        // This tells us what the user can do.
        //
        // For now:
        // SuperAdmin
        //
        // Later:
        // Manager
        // Employee
        public string Role { get; set; } = string.Empty;

        // If false, the user cannot login.
        public bool IsActive { get; set; }

        // Stores when the account was created.
        public DateTime CreatedAt { get; set; }
    }
}