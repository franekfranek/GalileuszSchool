using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GalileuszSchool.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace GalileuszSchool.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class StudentsController : Controller
    {
        private readonly GalileuszSchoolContext context;

        public StudentsController(GalileuszSchoolContext context)
        {
            this.context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}