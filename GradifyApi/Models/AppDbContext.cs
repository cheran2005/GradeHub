using Microsoft.EntityFrameworkCore;

namespace GradifyApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Student> Student { get; set; }
        public DbSet<Semester> Semester { get; set; }

        public DbSet<Course> Course { get; set; }


    }
}
