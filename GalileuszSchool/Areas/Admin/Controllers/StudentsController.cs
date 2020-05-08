using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GalileuszSchool.Infrastructure;
using GalileuszSchool.Models;
using GalileuszSchool.Repository;
using GalileuszSchool.Repository.Students;
using GalileuszSchool.Repository.Teachers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GalileuszSchool.Areas.Admin.Controllers
{
    //[Authorize(Roles = "admin")]
    [Area("Admin")]
    public class StudentsController : Controller
    {
        private readonly IStudentsRepository _repository;
        private readonly IWebHostEnvironment webHostEnvironment;

        public StudentsController(IWebHostEnvironment env,
                                    IStudentsRepository repository)
        {
            this.webHostEnvironment = env;
            _repository = repository;
        }

        //get admin/students
        public async Task<IActionResult> Index()
        {
            
            return View(await _repository.GetAll().OrderByDescending(x => x.Id).ToListAsync());
            
        }

        // /admin/students/create
        //public IActionResult Create()
        //{
            //ViewBag.TeacherId = new SelectList(context.Teachers.OrderBy(x => x.Id), "Id", "LastName");

        //    return View();
        //}

        //POST /admin/students/create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Student student)
        {
            //ViewBag.CourseId = new SelectList(context.Courses.OrderBy(x => x.Sorting), "Id", "Name");

            if (ModelState.IsValid)
            {
                student.Slug = student.FirstName.ToLower().Replace(" ", "-") + student.LastName.ToLower().Replace(" ", "-");


                var slug = await _repository.GetBySlug(student.Slug);
                if (slug != null)
                {
                    ModelState.AddModelError("", "That student is already in the database");
                    return View(student);
                }

                string imageName = "noimage.jpg";
                if (student.ImageUpload != null)
                {
                    string uploadsDir = Path.Combine(webHostEnvironment.WebRootPath, "media/students");
                    imageName = Guid.NewGuid().ToString() + "_" + student.ImageUpload.FileName; // this gives unique id so no same image twice uploaded 
                    string filePath = Path.Combine(uploadsDir, imageName);
                    FileStream fileStream = new FileStream(filePath, FileMode.Create);
                    await student.ImageUpload.CopyToAsync(fileStream);
                    fileStream.Close();
                }
                student.Image = imageName;

                await _repository.Create(student);

                TempData["Success"] = "Student has been added";

                return RedirectToAction("Index");
            }
            return View(student);
        }

        //admin/students/edit/{id}
        //public async Task<IActionResult> Edit(int id)
        //{
        //    Student student = await _repository.GetById(id);
        //    if (student == null)
        //    {
        //        return NotFound();
        //    }

        //    ViewBag.CourseId = new SelectList(context.Courses.OrderBy(x => x.Sorting), "Id", "Name", student.CourseId);


        //    return View(student);
        //}

        //admin/teachers/edit/{id}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Student student)
        {
            //ViewBag.CourseId = new SelectList(context.Courses.OrderBy(x => x.Sorting), "Id", "Name", student.CourseId);

            if (ModelState.IsValid)
            {
                student.Slug = student.FirstName.ToLower().Replace(" ", "-") + student.LastName.ToLower().Replace(" ", "-");

                var slug = await _repository.GetModelByCondition(x => x.Id != student.Id, x => x.Slug == student.Slug);
                if (slug != null)
                {
                    ModelState.AddModelError("", "That student is already in the database");
                    return View(student);
                }


                if (student.ImageUpload != null)
                {
                    string uploadsDir = Path.Combine(webHostEnvironment.WebRootPath, "media/students");
                    if (!string.Equals(student.Image, "noimage.jpg"))
                    {
                        //string oldImagePath = Path.Combine(uploadsDir, student.Image);
                        //if (System.IO.File.Exists(oldImagePath))
                        //{
                        //    System.IO.File.Delete(oldImagePath);
                        //}
                    }

                    string imageName = Guid.NewGuid().ToString() + "_" + student.ImageUpload.FileName;
                    string filePath = Path.Combine(uploadsDir, imageName);
                    FileStream fileStream = new FileStream(filePath, FileMode.Create);
                    await student.ImageUpload.CopyToAsync(fileStream);
                    fileStream.Close();
                    student.Image = imageName;
                }

                await _repository.Update(student);

                TempData["Success"] = "Student has been edited";

                return RedirectToAction("Index");
            }
            return View(student);
        }

        //get/admin/students/delete/{id}
        public async Task<IActionResult> Delete(int id)
        {
            Student student = await _repository.GetById(id);

            if (student == null)
            {
                TempData["Error"] = "Student does not exist";
            }
            else
            {
                string uploadsDir = Path.Combine(webHostEnvironment.WebRootPath, "media/students");
                if (!string.Equals(student.Image, "noimage.jpg"))
                {
                    string oldImagePath = Path.Combine(uploadsDir, student.Image);
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }
                await _repository.Delete(id);
                TempData["Success"] = "The student has been removed";

            }

            return RedirectToAction("Index");
        }
        public async Task<JsonResult> GetStudents()
        {
            List<Student> students = await _repository.GetAll().OrderByDescending(x => x.Id).ToListAsync();
            return Json(students);
        }
        public async Task<IActionResult> FindStudent(int id)
        {
            var student = await _repository.GetById(id);
            return new JsonResult(student);
        }
    }
}