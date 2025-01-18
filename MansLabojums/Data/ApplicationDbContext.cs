
using Microsoft.EntityFrameworkCore;
using MansLabojums.Models;

namespace MansLabojums.Data
{
    public class ApplicationDbContext : DbContext
    {
       
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // Katram modeļa tipam DbSet, ja izmanto EF Core
        public DbSet<Teacher> Teachers { get; set; } = null!;
        public DbSet<Student> Students { get; set; } = null!;
        public DbSet<Course> Courses { get; set; } = null!;
        public DbSet<Assignment> Assignments { get; set; } = null!;
        public DbSet<Submission> Submissions { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            

            modelBuilder.Entity<Teacher>().HasKey(t => t.Id);
            modelBuilder.Entity<Student>().HasKey(s => s.Id);
            modelBuilder.Entity<Course>().HasKey(c => c.Id);
            modelBuilder.Entity<Assignment>().HasKey(a => a.Id);
            modelBuilder.Entity<Submission>().HasKey(x => x.Id);

          

            base.OnModelCreating(modelBuilder);
        }
    }
}
