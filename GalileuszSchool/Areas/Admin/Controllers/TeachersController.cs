using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using GalileuszSchool.Infrastructure;
using GalileuszSchool.Models;
using GalileuszSchool.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GalileuszSchool.Areas.Admin.Controllers
{
    //[Authorize(Roles = "admin")]
    [Area("Admin")]
    public class TeachersController : Controller
    {
        private readonly IRepository<Teacher> _repository;

        public TeachersController(IRepository<Teacher> repository)
        {
            this._repository = repository;
        }
        //get Admin/Teachers
        public async Task<IActionResult> Index()
        {
            return View(await _repository.GetAll().ToListAsync());
        }

        //POST /admin/teacher/create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Teacher teacher)
        {
            if (ModelState.IsValid)
            {
                teacher.Slug = teacher.FirstName.ToLower().Replace(" ", "-") + teacher.LastName.ToLower().Replace(" ", "-");

                var slug = await _repository.GetBySlug(teacher.Slug);

                if (slug != null)
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return Json(new { text = "Teacher already exists!" });
                }

                await _repository.Create(teacher);

                return Ok();
            }
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return Json(new { text = "Server error!" });
        }

        //admin/teachers/edit/{id}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Teacher teacher)
        {
            //ViewBag.CourseId = new SelectList(context.Courses.OrderBy(x => x.Sorting), "Id", "Name", teacher.CourseId);

            if (ModelState.IsValid)
            {
                teacher.Slug = teacher.FirstName.ToLower().Replace(" ", "-") + teacher.LastName.ToLower().Replace(" ", "-");

                var slug = await _repository.GetModelByCondition(x => x.Id != teacher.Id, x => x.Slug == teacher.Slug);

                if (slug != null)
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return Json(new { text = "Teacher already exists!" });
                }
                await _repository.Update(teacher);

                return Ok();
            }
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return Json(new { text = "Server error!" });
        }

        //get/admin/teachers/delete/{id}
        public async Task<IActionResult> Delete(int id)
        {
            Teacher teacher = await _repository.GetById(id);

            if (teacher == null)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { text = "Teacher does not exists!" });
            }
            else
            {
                await _repository.Delete(teacher.Id);
            }

            return Ok();
        }

        public async Task<JsonResult> GetTeachers()
        {
            List<Teacher> teachers = await _repository.GetAll().OrderByDescending(x => x.Id).ToListAsync();
            return Json(teachers);
        }

        public async Task<IActionResult> FindTeacher(int id)
        {
            var teacher = await _repository.GetById(id);
            return new JsonResult(teacher);
        }
    }
}