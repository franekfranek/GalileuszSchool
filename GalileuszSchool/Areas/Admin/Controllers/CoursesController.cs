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
using GalileuszSchool.Repository.Courses;
using SQLitePCL;

namespace GalileuszSchool.Areas.Admin.Controllers
{
    //[Authorize(Roles = "admin")]
    [Area("Admin")]

    public class CoursesController : Controller
    {
        private readonly GalileuszSchoolContext context;
        private readonly ICoursesRepository _repository;

        public CoursesController(GalileuszSchoolContext context, ICoursesRepository repository)
        {
            this.context = context;
            this._repository = repository;
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

        //private async Task<IEnumerable<SelectListItem>> GetSelectListItem<IListItem>(IOrderedQueryable<IListItem> dbData)
        //{
        //    IEnumerable<SelectListItem> selectList = from s in dbData 
        //                                             select new SelectListItem
        //                                             {
        //                                                 Value = s.Id.ToString(),
        //                                                 Text = s.FirstName + " " + s.LastName.ToString()
        //                                             };
        //    return selectList;
        //}

        //POST /admin/courses/create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Course course)
        {
            if (ModelState.IsValid)
            {
                course.Slug = course.Name.ToLower().Replace(" ", "-");
                course.Sorting = 100;

                var slug = await _repository.GetBySlug(course.Slug);
                if (slug != null)
                {
                    TempData["Error"] = "The course already exists";
                    return new JsonResult("error");
                }

                await _repository.Create(course);

                TempData["Success"] = "The course has been added";

                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> FindCourse(int id)
        {
            var course = await _repository.GetById(id);
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


                var slug = await _repository.GetModelByCondition(x => x.Id != course.Id, x => x.Slug == course.Slug);
                if (slug != null)
                {
                    TempData["Error"] = "The course already exists";
                    var r = new JsonResult("The course name already exists!"); ;
                    return r;

                    return RedirectToAction("Index");
                }

                await _repository.Update(course);

                //TempData["Success"] = "The course has been edited";

                return RedirectToAction("Index");
            }

            return new JsonResult("All good");
        }

        // /admin/courses/details/{id}
        public async Task<IActionResult> Details(int id)
        {
            Course course = await _repository.GetById(id);

            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        //get /admin/pages/delete/{id}
        public async Task<IActionResult> Delete(int id)
        {
            Course course = await _repository.GetById(id);

            if (course == null)
            {
                TempData["Error"] = "The course does not exist";
            }
            else
            {
                await _repository.Delete(id);
            }

            return RedirectToAction("Index");
        }

        public async Task<JsonResult> GetCourses()
        {
            List<Course> courses = await _repository.GetAll().OrderByDescending(x => x.Sorting)
                                                            .Include(x => x.Teacher).ToListAsync();
            return Json(courses);
        }
    }
}