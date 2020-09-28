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
using SQLitePCL;
using Microsoft.VisualBasic;
using GalileuszSchool.Repository;
using System.Net;
using GalileuszSchool.Models.ModelsForAdminArea;

namespace GalileuszSchool.Areas.Admin.Controllers
{
    //[Authorize(Roles = "admin")]
    [Area("Admin")]
    public class CoursesController : Controller
    {
        private readonly IRepository<Course> _repository;

        public CoursesController(IRepository<Course> repository)
        {
            _repository = repository;
        }

        //get Admin/Courses
        public async Task<IActionResult> Index()
        {
            //TODO refactoring needed: how to move db queries to GetSelectListItem or/and use _repository
            // 
            var teacherInfo = _repository.GetAllTeachers();
            var selectListTeachers = await GetSelectListItem(teacherInfo);
            ViewBag.TeacherId = new SelectList(selectListTeachers, "Value", "Text");

            var studentInfo = _repository.GetAllStudents();
            var selectListStudents = await GetSelectListItem(studentInfo);
            ViewBag.StudentId = new SelectList(selectListStudents, "Value", "Text");

            return View();
        }

        private async Task<IEnumerable<SelectListItem>> GetSelectListItem(IOrderedQueryable<IListItem> dbData)
        {
            if(dbData != null)
            {
                return await dbData.Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.FirstName + " " + s.LastName.ToString()
                }).ToListAsync();
            }

            return new List<SelectListItem>();
        }

        //POST /admin/courses/create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Course course)
        {
            //ModelState.AddModelError("key", "hej");
            if (ModelState.IsValid) //should try block be here
            {
                course.Slug = course.Name.ToLower().Replace(" ", "-");
                course.Sorting = 100;

                var slug = await _repository.GetBySlug(course.Slug);
                if (slug != null)
                {
                    return Json(new { text = "Course already exists!" });
                }

                await _repository.Create(course);

                //return StatusCode(200);
                return Ok();
            }
            return Json(new { text = "Invalid Course model!" });
        }

        //POST /admin/courses/edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Course course)
        {
            if (ModelState.IsValid)
            {
                course.Slug = course.Name.ToLower().Replace(" ", "-");


                var slug = await _repository.GetModelByWhereAndFirstConditions(x => x.Id != course.Id, x => x.Slug == course.Slug);
                if (slug != null)
                {
                    return Json(new { text = "Course with that name already exists!" });
                }

                await _repository.Update(course);

                return Ok();
            }
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return Json(new { text = "Invalid Course model!" });
        }

        //get /admin/pages/delete/{id}
        public async Task<IActionResult> Delete(int id)
        {
            Course course = await _repository.GetById(id);

            if (course == null)
            {
                return Json(new { text = "Course does not exists!" });
            }
            else
            {
                await _repository.Delete(id);
            }

            return Ok();
        }

        public async Task<JsonResult> GetCourses()
        {
            List<Course> courses = await _repository.GetAll().OrderByDescending(x => x.Sorting)
                                                            .Include(x => x.Teacher).ToListAsync();
            if(courses == null || courses.Count == 0)
            {
                return Json(new { text = "No courses found!" });
            }
            return Json(courses);
        }
        public async Task<IActionResult> FindCourse(int id)
        {
            var course = await _repository.GetById(id);
            if (course == null)
            {
                return Json(new { text = "Server error!" });
            }
            return new JsonResult(course);
        }
    }
}