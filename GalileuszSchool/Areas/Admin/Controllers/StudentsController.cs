using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using GalileuszSchool.Infrastructure;
using GalileuszSchool.Models;
using GalileuszSchool.Models.ModelsForAdminArea;
using GalileuszSchool.Repository;
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
        private readonly IRepository<Student> _repository;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public StudentsController(IWebHostEnvironment env,
                                    IRepository<Student> repository)
        {
            _webHostEnvironment = env;
            _repository = repository;
        }

        //get admin/students
        public async Task<IActionResult> Index()
        {
            
            return View();
            
        }

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
                    return Json(new { text = "Student already exists!" });
                }

                string imageName = "noimage.jpg";
                if (student.ImageUpload != null)
                {
                    string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/students");
                    imageName = Guid.NewGuid().ToString() + "_" + student.ImageUpload.FileName; // this gives unique id so no same image twice uploaded 
                    string filePath = Path.Combine(uploadsDir, imageName);
                    FileStream fileStream = new FileStream(filePath, FileMode.Create);
                    await student.ImageUpload.CopyToAsync(fileStream);
                    fileStream.Close();
                }
                student.Image = imageName;

                await _repository.Create(student);

                return Ok();
            }
            return Json(new { text = "Invalid Student model!" });
        }

        //admin/teachers/edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Student student)
        {
            if (ModelState.IsValid)
            {
                student.Slug = student.FirstName.ToLower().Replace(" ", "-") + student.LastName.ToLower().Replace(" ", "-");

                var slug = await _repository.GetModelByWhereAndFirstConditions(x => x.Id != student.Id, x => x.Slug == student.Slug);
                if (slug != null)
                {
                    return Json(new { text = "Student with that data already exists!" });
                }


                if (student.ImageUpload != null)
                {
                    string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/students");
                    if (!string.Equals(student.Image, "noimage.jpg"))
                    {
                        //string oldImagePath = Path.Combine(uploadsDir, student.Image);
                        //if (System.IO.File.Exists(oldImagePath))
                        //{
                        //    System.IO.File.Delete(oldImagePath);
                        //todo clean
                    }

                    string imageName = Guid.NewGuid().ToString() + "_" + student.ImageUpload.FileName;
                    string filePath = Path.Combine(uploadsDir, imageName);
                    FileStream fileStream = new FileStream(filePath, FileMode.Create);
                    await student.ImageUpload.CopyToAsync(fileStream);
                    fileStream.Close();
                    student.Image = imageName;
                }

                await _repository.Update(student);

                return Ok();
            }
            return Json(new { text = "Invalid Student model!" });
        }

        //get/admin/students/delete/{id}
        public async Task<IActionResult> Delete(int id)
        {
            Student student = await _repository.GetById(id);

            if (student == null)
            {
                return Json(new { text = "Student does not exists!" });
            }
            else
            {
                if (!string.Equals(student.Image, "noimage.jpg"))
                {
                    string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/students");
                    string oldImagePath = Path.Combine(uploadsDir, student.Image);
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }
                await _repository.Delete(id);
            }

            return Ok();
        }
        public async Task<JsonResult> GetStudents()
        {
            List<Student> students = await _repository.GetAll().OrderByDescending(x => x.Id).ToListAsync();
            if (students == null || students.Count == 0)
            {
                return Json(new { text = "No students found!" });
            }
            return Json(students);
        }
        public async Task<JsonResult> FindStudent(int id)
        {
            var student = await _repository.GetById(id);
            if (student == null)
            {
                return Json(new { text = "Server error!" });
            }
            return new JsonResult(student);
        }
    }
}