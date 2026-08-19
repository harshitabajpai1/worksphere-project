using AuthService.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Data
{
    // DbContext is the main class EF Core uses
    // to communicate with SQL Server.
    //
    // it  is like a bridge between:
    //
    // C# application --> dbContext --> SQL Server
    
    //
    public class AuthDbContext : DbContext
    {
        // The constructor receives database configuration
        // from ASP.NET Core's Dependency Injection system.
        public AuthDbContext(
            DbContextOptions<AuthDbContext> options)
            : base(options)
        {
        }

        // This represents the Users table.
        //
        // User class --> Users table
        public DbSet<User> Users { get; set; }
    }
}