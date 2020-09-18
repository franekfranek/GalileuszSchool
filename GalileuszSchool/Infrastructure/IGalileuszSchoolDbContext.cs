using GalileuszSchool.Models.ModelsForAdminArea;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GalileuszSchool.Infrastructure
{
    public interface IGalileuszSchoolDbContext
    {
        DbSet<Course> Courses { get; set; }
        DbSet<Teacher> Teachers { get; set; }
        DbSet<Student> Students { get; set; }
        int SaveChanges();
    }
}
