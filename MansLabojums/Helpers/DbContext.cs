using Microsoft.EntityFrameworkCore;
using MansLabojums.Models;
using KMansLabojums.Models;
using MansLabojums3.Models;

namespace MansLabojums.Data // vai jebkur citur, bet namespace jāpievieno projektam
{
    public class MyDbContext : DbContext
    {
        // DbSet īpašības (tabulas) – pielāgo saviem modeļiem:
        public DbSet<Student> Students { get; set; } = null!;
        public DbSet<Teacher> Teachers { get; set; } = null!;
        public DbSet<Course> Courses { get; set; } = null!;
        public DbSet<Assignment> Assignments { get; set; } = null!;
        public DbSet<Submission> Submissions { get; set; } = null!;

        // Konstruktors – EF Core parasti izmanto “options”:
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options) { }

        // Ja vēlaties konfigurēt “no code” (bez DI):
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Lietojam jūsu ConfigHelper pieslēguma virkni
                var connStr = MansLabojums.Helpers.ConfigHelper.GetConnectionString();
                optionsBuilder.UseSqlite(connStr);
            }
            base.OnConfiguring(optionsBuilder);
        }
    }
}
