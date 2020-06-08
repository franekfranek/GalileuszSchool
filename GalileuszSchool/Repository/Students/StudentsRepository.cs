using GalileuszSchool.Infrastructure;
using GalileuszSchool.Models;
using GalileuszSchool.Models.ModelsForAdminArea;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GalileuszSchool.Repository.Students
{
    public class StudentsRepository : Repository<Student>, IStudentsRepository
    {
        public StudentsRepository(GalileuszSchoolContext context)
            :base(context)
        {

        }
    }
}
