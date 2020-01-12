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
    public class CoursesController : Controller
    {
        private readonly GalileuszSchoolContext context;

        public CoursesController(GalileuszSchoolContext context)
        {
            this.context = context;
        }

        //get Admin/Courses
        public async Task<IActionResult> Index()
        {

            return View(await context.Courses.OrderBy(x => x.Sorting).ToListAsync());
        }

        // /admin/courses/details/{id}
        public async Task<IActionResult> Details(int id)
        {
            Course course = await context.Courses.FirstOrDefaultAsync(x => x.Id == id);

            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // /admin/courses/create
        public IActionResult Create() => View();

        //POST /admin/courses/create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Course course)
        {
            if (ModelState.IsValid)
            {
                course.Slug = course.Name.ToLower().Replace(" ", "-");
                course.Sorting = 100;

                var slug = await context.Courses.FirstOrDefaultAsync(x => x.Slug == course.Slug);
                if (slug != null)
                {
                    ModelState.AddModelError("", "The course already exists");
                    return View(course);
                }

                context.Add(course);
                await context.SaveChangesAsync();

                TempData["Success"] = "The course has been added";

                return RedirectToAction("Index");
            }
            return View(course);
        }
        //get /admin/courses/edit/{id}
        public async Task<IActionResult> Edit(int id)
        {
            Course course = await context.Courses.FindAsync(id);

            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        //POST /admin/courses/edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Course course)
        {
            if (ModelState.IsValid)
            {
                course.Slug = course.Name.ToLower().Replace(" ", "-");


                var slug = await context.Courses.Where(x => x.Id != id).FirstOrDefaultAsync(x => x.Slug == course.Slug);
                if (slug != null)
                {
                    ModelState.AddModelError("", "The course already exists");
                    return View(course);
                }

                context.Update(course);
                await context.SaveChangesAsync();

                TempData["Success"] = "The course has been edited";

                return RedirectToAction("Edit", new { id = id });
            }

            return View(course);
        }

        //get /admin/pages/delete/{id}
        public async Task<IActionResult> Delete(int id)
        {
            Course course = await context.Courses.FindAsync(id);

            if (course == null)
            {
                TempData["Error"] = "The course does not exist";
            }
            else
            {
                context.Courses.Remove(course);
                await context.SaveChangesAsync();
                TempData["Success"] = "The course has been deleted";

            }

            return RedirectToAction("Index");
        }
    }
}