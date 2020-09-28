using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using GalileuszSchool.Infrastructure;
using GalileuszSchool.Models;
using GalileuszSchool.Models.ModelsForAdminArea;
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
            _repository = repository;
        }
        //get Admin/Teachers
        public async Task<IActionResult> Index()
        {
            //var teachers = await _repository.GetAll().ToListAsync();
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

                var slug = await _repository.GetBySlug(teacher.Slug);

                if (slug != null)
                {
                    //Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return Json(new { text = "Teacher already exists!" });
                }

                await _repository.Create(teacher);

                return Ok();
            }
            //TODO: Response is null ???, hence not possible to set the code 
            //Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return Json(new { text = "Invalid Techer model!" });
        }

        //admin/teachers/edit/{id}

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Teacher teacher)
        {
            //ViewBag.CourseId = new SelectList(context.Courses.OrderBy(x => x.Sorting), "Id", "Name", teacher.CourseId);

            if (ModelState.IsValid)
            {
                teacher.Slug = teacher.FirstName.ToLower().Replace(" ", "-") + teacher.LastName.ToLower().Replace(" ", "-");

                //Explanation: we want to check if there is already teacher we want to edit 
                // we want to consider all teachers except the one passed and find if any other has slug like it 
                // in short it checks if edited Teacher DOES NOT have the same name as somebody in the DB already....
                var slug = await _repository.GetModelByWhereAndFirstConditions(x => x.Id != teacher.Id, x => x.Slug == teacher.Slug);

                if (slug != null)
                {
                    //Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return Json(new { text = "Teacher with that name already exists!" });
                }
                await _repository.Update(teacher);

                return Ok();
            }
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return Json(new { text = "Invalid teacher model!" });
        }

        //get/admin/teachers/delete/{id}
        public async Task<IActionResult> Delete(int id)
        {
            Teacher teacher = await _repository.GetById(id);

            if (teacher == null)
            {
                //Response.StatusCode = (int)HttpStatusCode.BadRequest;
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
            List<Teacher> teachers = await _repository.GetAll().ToListAsync();
            if(teachers == null || teachers.Count == 0)
            {
                return Json(new { text = "No teachers found!" });
            }
            return Json(teachers);
        }

        public async Task<JsonResult> FindTeacher(int id)
        {
            var teacher = await _repository.GetById(id);
            if (teacher == null)
            {
                return Json(new { text = "Server error!" });
            }
            return new JsonResult(teacher);
        }
    }
}