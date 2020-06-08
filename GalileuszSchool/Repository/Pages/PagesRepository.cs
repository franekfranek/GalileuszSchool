using GalileuszSchool.Infrastructure;
using GalileuszSchool.Models;
using GalileuszSchool.Models.ModelsForAdminArea;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GalileuszSchool.Repository.Pages
{
    public class PagesRepository: Repository<Page>, IPagesRepository
    {
        public PagesRepository(GalileuszSchoolContext context)
            : base(context)
        {

        }
    }
}
