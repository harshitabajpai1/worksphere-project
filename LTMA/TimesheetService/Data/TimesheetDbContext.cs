using Microsoft.EntityFrameworkCore;
using TimesheetService.Models;

namespace TimesheetService.Data
{
    // Connects TimesheetService to the WorkSphere Timesheet database.
    public class TimesheetDbContext : DbContext
    {
        public TimesheetDbContext(DbContextOptions<TimesheetDbContext> options)
            : base(options)
        {
        }

        // Timesheets table.
        public DbSet<Timesheet> Timesheets { get; set; }
    }
}
