/******************************************************
 *  MansLabojums/Data/ApplicationDbContext.cs
 ******************************************************/
using Microsoft.EntityFrameworkCore;
using MansLabojums.Models;

namespace MansLabojums.Data
{
    public class ApplicationDbContext : DbContext
    {
        // Konstruktorā vari, ja vajag, injicēt DbContextOptions<ApplicationDbContext>
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
            // Ja modeļos nav definētas navigācijas prop (piem., Course -> Teacher),
            // nav jālieto .HasOne(...) .WithMany(...) 
            // Piemēram, var atstāt tukšu, vai tikai definēt primārās atslēgas, ja vajag:

            modelBuilder.Entity<Teacher>().HasKey(t => t.Id);
            modelBuilder.Entity<Student>().HasKey(s => s.Id);
            modelBuilder.Entity<Course>().HasKey(c => c.Id);
            modelBuilder.Entity<Assignment>().HasKey(a => a.Id);
            modelBuilder.Entity<Submission>().HasKey(x => x.Id);

            // Ja tomēr vēlies EF navigācijas (Course -> Teacher, Assignment -> Course, utt.),
            // tad modeļos jābūt definētām publiskām prop, piemēram:
            // 
            // public Teacher Teacher { get; set; }
            // public Course Course { get; set; }
            // public Assignment Assignment { get; set; }
            // public Student Student { get; set; }
            //
            // un tad var pievienot:
            //
            // modelBuilder.Entity<Course>()
            //     .HasOne<Teacher>()        // or .HasOne(c => c.Teacher)
            //     .WithMany()
            //     .HasForeignKey(c => c.TeacherId);
            //
            // Pagaidām noņemam, lai nebūtu CS1061.

            base.OnModelCreating(modelBuilder);
        }
    }
}
