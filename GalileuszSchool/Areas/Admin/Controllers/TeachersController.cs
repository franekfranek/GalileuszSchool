using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GalileuszSchool.Infrastructure;
using GalileuszSchool.Models;
using GalileuszSchool.Repository.Teachers;
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
        private readonly ITeachersRepository _repository;

        public TeachersController(ITeachersRepository repository)
        {
            this._repository = repository;
        }
        //get Admin/Teachers
        public async Task<IActionResult> Index()
        {
            return View(await _repository.GetAll().ToListAsync());
        }

        // /admin/teachers/create
        //public IActionResult Create()
        //{
        //    //ViewBag.CourseId = new SelectList(context.Courses.OrderBy(x => x.Sorting), "Id", "Name");

        //    return View();
        //}

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
                    ModelState.AddModelError("", "That teacher is already in the database");
                    return View(teacher);
                }

                await _repository.Create(teacher);

                TempData["Success"] = "Teacher has been added";

                return RedirectToAction("Index");
            }
            return View(teacher);
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
                    ModelState.AddModelError("", "That teacher is already in the database");
                    return View(teacher);
                }
                await _repository.Update(teacher);

                TempData["Success"] = "Teacher has been edited";

                return RedirectToAction("Index");
            }
            return View(teacher);
        }

        //get/admin/teachers/delete/{id}
        public async Task<IActionResult> Delete(int id)
        {
            Teacher teacher = await _repository.GetById(id);

            if (teacher == null)
            {
                TempData["Error"] = "Teacher does not exist";
            }
            else
            {
                await _repository.Delete(teacher.Id);
                TempData["Success"] = "The teacher has been removed";
            }

            return RedirectToAction("Index");
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