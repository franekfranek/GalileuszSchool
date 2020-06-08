using GalileuszSchool.Infrastructure;
using GalileuszSchool.Models;
using GalileuszSchool.Models.ModelsForAdminArea;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GalileuszSchool.Repository.Classrooms
{
    public class ClassroomsRepository: Repository<ClassRoom>, IClassroomsRepository
    {
        public ClassroomsRepository(GalileuszSchoolContext context)
            :base(context)
        {

        }
    }
}
