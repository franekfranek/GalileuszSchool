using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GalileuszSchool.Infrastructure;
using GalileuszSchool.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GalileuszSchool.Areas.Admin.Controllers
{
    //[Authorize(Roles = "admin, editor")]
    [Area("Admin")]
    public class TeachersController : Controller
    {
        private readonly GalileuszSchoolContext context;

        public TeachersController(GalileuszSchoolContext context)
        {
            this.context = context;
        }
        //get Admin/Teachers
        public async Task<IActionResult> Index()
        {
            return View(await context.Teachers.OrderByDescending(x => x.Id).ToListAsync());
        }

        // /admin/teachers/create
        public IActionResult Create()
        {
            //ViewBag.CourseId = new SelectList(context.Courses.OrderBy(x => x.Sorting), "Id", "Name");

            return View();
        }

        //POST /admin/teacher/create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Teacher teacher)
        {
            if (ModelState.IsValid)
            {
                teacher.Slug = teacher.FirstName.ToLower().Replace(" ", "-") + teacher.LastName.ToLower().Replace(" ", "-");


                var slug = await context.Teachers.FirstOrDefaultAsync(x => x.Slug == teacher.Slug);
                if (slug != null)
                {
                    ModelState.AddModelError("", "That teacher is already in the database");
                    return View(teacher);
                }

                context.Add(teacher);
                await context.SaveChangesAsync();

                TempData["Success"] = "Teacher has been added";

                return RedirectToAction("Index");
            }
            return View(teacher);
        }

        //admin/teachers/edit/{id}
        public async Task<IActionResult> Edit(int id)
        {
            Teacher teacher = await context.Teachers.FindAsync(id);
            if (teacher == null)
            {
                return NotFound();
            }

            //ViewBag.CourseId = new SelectList(context.Courses.OrderBy(x => x.Sorting), "Id", "Name", teacher.CourseId);


            return View(teacher);
        }

        //admin/teachers/edit/{id}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Teacher teacher)
        {
            //ViewBag.CourseId = new SelectList(context.Courses.OrderBy(x => x.Sorting), "Id", "Name", teacher.CourseId);

            if (ModelState.IsValid)
            {
                teacher.Slug = teacher.FirstName.ToLower().Replace(" ", "-") + teacher.LastName.ToLower().Replace(" ", "-");

                var slug = await context.Teachers.Where(x => x.Id != id).FirstOrDefaultAsync(x => x.Slug == teacher.Slug);
                if (slug != null)
                {
                    ModelState.AddModelError("", "That teacher is already in the database");
                    return View(teacher);
                }

                context.Update(teacher);
                await context.SaveChangesAsync();

                TempData["Success"] = "Teacher has been edited";

                return RedirectToAction("Index");
            }
            return View(teacher);
        }

        //get/admin/teachers/delete/{id}
        public async Task<IActionResult> Delete(int id)
        {
            Teacher teacher = await context.Teachers.FindAsync(id);

            if (teacher == null)
            {
                TempData["Error"] = "Teacher does not exist";
            }
            else
            {
                context.Teachers.Remove(teacher);
                await context.SaveChangesAsync();
                TempData["Success"] = "The teacher has been removed";

            }

            return RedirectToAction("Index");
        }


    }
}