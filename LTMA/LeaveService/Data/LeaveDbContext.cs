using LeaveService.Models;
using Microsoft.EntityFrameworkCore;

namespace LeaveService.Data
{
    // Connects LeaveService to the WorkSphere Leave database.
    public class LeaveDbContext : DbContext
    {
        public LeaveDbContext(DbContextOptions<LeaveDbContext> options)
            : base(options)
        {
        }

        // Leave requests table.
        public DbSet<Leave> Leaves { get; set; }
    }
}
