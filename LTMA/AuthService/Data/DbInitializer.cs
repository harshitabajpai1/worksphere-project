using AuthService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Data
{
    // This class is responsible for creating the initial
    // SuperAdmin account when the application starts.
    //
    // We are using a hardcoded SuperAdmin because there
    // should always be one administrator available to
    // create and manage other users.
    public static class DbInitializer
    {
        public static async Task InitializeAsync(
            AuthDbContext context,
            IPasswordHasher<User> passwordHasher)
        {
            // Make sure the database exists and all
            // pending migrations have been applied.
            await context.Database.MigrateAsync();


            // Check whether a SuperAdmin already exists.
            //
            // This is important because we don't want to
            // create a new SuperAdmin every time the
            // application starts.
            var superAdminExists =
                await context.Users.AnyAsync(
                    user => user.Role == "SuperAdmin");


            // If a SuperAdmin already exists,
            // there is nothing more to do.
            if (superAdminExists)
            {
                return;
            }


            // Create the SuperAdmin user.
            var superAdmin = new User
            {
                Name = "WorkSphere Super Admin",

                Email = "admin@worksphere.com",

                Role = "SuperAdmin",

                IsActive = true,

                CreatedAt = DateTime.UtcNow
            };


           
            // PASSWORD HASHING
            // The initial password is:
            //
            // Admin@123
            //
            // PasswordHasher converts it into a secure hash.
            //
            superAdmin.PasswordHash =
                passwordHasher.HashPassword(
                    superAdmin,
                    "Admin@123");


            // Add the new SuperAdmin to the Users table.
            context.Users.Add(superAdmin);


            // Save the changes to SQL Server.
            await context.SaveChangesAsync();
        }
    }
}