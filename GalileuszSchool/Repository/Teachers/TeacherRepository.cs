using GalileuszSchool.Infrastructure;
using GalileuszSchool.Models;
using GalileuszSchool.Models.ModelsForAdminArea;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GalileuszSchool.Repository.Teachers
{
    public class TeacherRepository : Repository<Teacher>, ITeachersRepository
    {
        public TeacherRepository(GalileuszSchoolContext context)
            : base(context)
        {

        }

        public int GetNumber()
        {
            throw new NotImplementedException();
        }
    }
}
