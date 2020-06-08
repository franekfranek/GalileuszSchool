using GalileuszSchool.Infrastructure;
using GalileuszSchool.Models;
using GalileuszSchool.Models.ModelsForAdminArea;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GalileuszSchool.Repository.Courses
{
    public class CoursesRepository : Repository<Course>, ICoursesRepository
    {
        public CoursesRepository(GalileuszSchoolContext context)
            :base(context)
        {

        }
    }
}
