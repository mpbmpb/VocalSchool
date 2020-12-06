using Microsoft.EntityFrameworkCore;
using VocalSchool.Models;

namespace VocalSchool.Data
{
    public class SchoolContext : DbContext
    {
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Day> Days { get; set; }
        public DbSet<DaySubject> DaySubjects { get; set; }
        public DbSet<Seminar> Seminars { get; set; }
        public DbSet<SeminarDay> SeminarDays { get; set; }
        public DbSet<CourseDesign> CourseDesigns { get; set; }
        public DbSet<CourseSeminar> CourseSeminars { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<CourseDate> CourseDates { get; set; }
        public DbSet<Venue> Venues { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        
        public SchoolContext(DbContextOptions<SchoolContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DaySubject>()
                .HasKey(d => new { d.DayId, d.SubjectId });

            modelBuilder.Entity<SeminarDay>()
                .HasKey(s => new { s.SeminarId, s.DayId });

            modelBuilder.Entity<CourseSeminar>()
                .HasKey(cs => new { cs.CourseDesignId, cs.SeminarId });
        }
    }
}
