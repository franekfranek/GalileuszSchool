using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GalileuszSchool.Infrastructure;
using GalileuszSchool.Models;
using GalileuszSchool.Models.ModelsForAdminArea;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GalileuszSchool.Areas.Admin.Controllers
{
    //[Authorize(Roles = "admin")]
    [Area("Admin")]
    public class StudentCourseConnectionController : Controller
    {
        private readonly GalileuszSchoolContext _context;

        public StudentCourseConnectionController(GalileuszSchoolContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> AddIds(StudentCourseConnection studentCourseConnection,
                                                    int courseId, int studentId)
        {
            studentCourseConnection.CourseId = courseId;
            studentCourseConnection.StudentId= studentId;
            Student student = await _context.Students.FirstOrDefaultAsync(x => x.Id == studentId);

            try
            {
                student.EnrollmentDate = DateTime.Now;
                _context.Add(studentCourseConnection);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                TempData["Error"] = "Error ocurred!/The student is already enrolled for this course!";
                return RedirectToAction("Index", "Courses");
            }

            TempData["Success"] = "The student has been added";
            return RedirectToAction("Index", "Courses");

        }

        // get /admin/StudentCourseConnection/ShowStudentForCourse
        public async Task<IActionResult> ShowAllStudents()
        {

            var students = await _context.Students.OrderByDescending(x => x.Id).ToListAsync();
           
            return View(students);
        }

        // get /admin/StudentCourseConnection/StudentsByCourse/1 (course id) 
        public async Task<IActionResult> StudentsByCourse(int id)
        {
            Course course = await _context.Courses.Where(x => x.Id == id).FirstOrDefaultAsync();

            var studentsByCourse = await _context.StudenCourseConnections
                                                .OrderByDescending(x => x.CourseId)
                                                .Where(x =>x.CourseId == id)
                                                .Include("Student").ToListAsync();

            ViewBag.CourseName = course.Name;

            return View(studentsByCourse);
        }



    }
}