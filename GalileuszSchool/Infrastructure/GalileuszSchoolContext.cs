using GalileuszSchool.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GalileuszSchool.Infrastructure
{
    public class GalileuszSchoolContext : IdentityDbContext<AppUser>
    {
        public GalileuszSchoolContext(DbContextOptions<GalileuszSchoolContext> options) : base(options)
        {
        }


        public DbSet<Page> Pages { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<StudentCourseConnection> StudenCourseConnections { get; set; }
        public DbSet<LessonPlan> LessonPlan { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<StudentCourseConnection>()
                .HasKey(o => new { o.StudentId, o.CourseId });
        }

        

        public DbSet<GalileuszSchool.Models.ClassRoom> ClassRoom { get; set; }

    }
}
