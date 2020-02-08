using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GalileuszSchool.Infrastructure;
using GalileuszSchool.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;


namespace GalileuszSchool.Areas.Admin.Controllers
{
    //[Authorize(Roles = "admin, editor")]
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
            //TODO refactoring needed
            var teacherInfo =  context.Teachers.OrderBy(x => x.Id);
            IEnumerable<SelectListItem> selectList = from s in teacherInfo
                                                     select new SelectListItem
                                                     {
                                                         Value = s.Id.ToString(),
                                                         Text = s.FirstName + " " + s.LastName.ToString()
                                                     };
            ViewBag.TeacherId = new SelectList(selectList, "Value", "Text");

            var studentInfo = context.Students.OrderBy(x => x.Id);
            IEnumerable<SelectListItem> selectListStudents = from s in studentInfo
                                                     select new SelectListItem
                                                     {
                                                         Value = s.Id.ToString(),
                                                         Text = s.FirstName + " " + s.LastName.ToString()
                                                     };
            ViewBag.StudentId = new SelectList(selectListStudents, "Value", "Text");


            return View();

        }

        //POST /admin/courses/create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Course course)
        {
            //string courseName, string level,
            //                                    string description, int price, int teacherId
            //var course = new Course
            //{
            //    Name = courseName,
            //    Level = level,
            //    Description = description,
            //    Price = price,
            //    TeacherId = teacherId
            //};

            if (ModelState.IsValid)
            {
                course.Slug = course.Name.ToLower().Replace(" ", "-");
                course.Sorting = 100;

                var slug = await context.Courses.FirstOrDefaultAsync(x => x.Slug == course.Slug);
                if (slug != null)
                {
                    TempData["Error"] = "The course already exists";
                    return RedirectToAction("Index");
                }

                context.Add(course);
                await context.SaveChangesAsync();

                TempData["Success"] = "The course has been added";

                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        public IActionResult FindCourse(int id)
        {
            var course = context.Courses.Find(id);
            return new JsonResult(course);
        }

        //get /admin/courses/edit/{id}
        //public async Task<IActionResult> Edit(int id)
        //{
        //    var teacherInfo = context.Teachers.OrderBy(x => x.Id);
        //    IEnumerable<SelectListItem> selectList = from s in teacherInfo
        //                                             select new SelectListItem
        //                                             {
        //                                                 Value = s.Id.ToString(),
        //                                                 Text = s.FirstName + " " + s.LastName.ToString()
        //                                             };
        //    ViewBag.TeacherId = new SelectList(selectList, "Value", "Text");

        //    Course course = await context.Courses.FindAsync(id);

        //    if (course == null)
        //    {
        //        return NotFound();
        //    }

        //    //ViewBag.TeacherId = new SelectList(context.Teachers.OrderBy(x => x.Id), "Id", "LastName");


        //    return View(course);
        //}

        //POST /admin/courses/edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Course course)
        {
            if (ModelState.IsValid)
            {
                course.Slug = course.Name.ToLower().Replace(" ", "-");


                var slug = await context.Courses.Where(x => x.Id != course.Id).FirstOrDefaultAsync(x => x.Slug == course.Slug);
                if (slug != null)
                {
                    TempData["Error"] = "The course already exists";
                    return RedirectToAction("Index");
                }

                context.Update(course);
                await context.SaveChangesAsync();

                //TempData["Success"] = "The course has been edited";

                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
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
            }

            return RedirectToAction("Index");
        }

        public JsonResult GetCourses()
        {
            List<Course> courses = context.Courses.OrderByDescending(x => x.Sorting).Include(x => x.Teacher)
                                                                                          .ToList();
            return Json(courses);
        }
    }
}