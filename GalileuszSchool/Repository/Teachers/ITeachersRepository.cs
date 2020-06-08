using GalileuszSchool.Models;
using GalileuszSchool.Models.ModelsForAdminArea;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GalileuszSchool.Repository.Teachers
{
    public interface ITeachersRepository : IRepository<Teacher>
    {
        int GetNumber();
    }
}
