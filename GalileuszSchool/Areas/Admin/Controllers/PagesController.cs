using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GalileuszSchool.Infrastructure;
using GalileuszSchool.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GalileuszSchool.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PagesController : Controller
    {
        private readonly GalileuszSchoolContext context;

        public PagesController(GalileuszSchoolContext context)
        {
            this.context = context;
        }

        public async Task<IActionResult> Index()
        {
            IQueryable<Page> pages = from p in context.Pages orderby p.Sorting select p;
            List<Page> pagesList = await pages.ToListAsync();


            return View(pagesList);
        }
    }
}