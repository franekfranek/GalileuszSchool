using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GalileuszSchool.Infrastructure
{
    public class GalileuszSchoolContext : DbContext
    {
        public GalileuszSchoolContext(DbContextOptions<GalileuszSchoolContext> options) : base(options)
        {
        }



    }
}
