using GalileuszSchool.Models;
using GalileuszSchool.Models.ModelsForAdminArea;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GalileuszSchool.Infrastructure
{
    public class CoursesViewComponent : ViewComponent
    {
        private readonly GalileuszSchoolContext context;
        public CoursesViewComponent(GalileuszSchoolContext context)
        {
            this.context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var courses = await GetCoursesAsync();
            return View(courses);
        }

        private Task<List<Course>> GetCoursesAsync()
        {
            return context.Courses.OrderBy(x => x.Sorting).ToListAsync();
        }
    }
}
