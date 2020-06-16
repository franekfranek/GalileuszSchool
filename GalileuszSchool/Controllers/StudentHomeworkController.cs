using GalileuszSchool.Infrastructure;
using GalileuszSchool.Models.ModelsForAdminArea;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GalileuszSchool.Controllers
{
    public class StudentHomeworkController : Controller
    {
        private readonly GalileuszSchoolContext _context;

        public StudentHomeworkController(GalileuszSchoolContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> AddStudentHomework(StudentHomework studentHomework,
                                                    int homeworkId, int studentId)
        {
            studentHomework.HomeworkId = homeworkId;
            studentHomework.StudentId = studentId;

            try
            {
                _context.Add(studentHomework);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                TempData["Error"] = "Server error ocurred!";
                return RedirectToAction("Index", "Homework");
            }

            TempData["Success"] = "The homework has been added";
            return RedirectToAction("Index", "Homework");

        }

        // get /admin/StudentCourseConnection/ShowStudentForCourse
        //public async Task<IActionResult> ShowAllStudents()
        //{

        //    var students = await _context.Students.OrderByDescending(x => x.Id).ToListAsync();

        //    return View(students);
        //}

        // get /admin/StudentCourseConnection/StudentsByCourse/1 (course id) 
        //public async Task<IActionResult> StudentsByCourse(int id)
        //{
        //    Course course = await _context.Courses.Where(x => x.Id == id).FirstOrDefaultAsync();

        //    var studentsByCourse = await _context.StudenCourseConnections
        //                                        .OrderByDescending(x => x.CourseId)
        //                                        .Where(x => x.CourseId == id)
        //                                        .Include("Student").ToListAsync();

        //    ViewBag.CourseName = course.Name;

        //    return View(studentsByCourse);
        //}
    }
}
