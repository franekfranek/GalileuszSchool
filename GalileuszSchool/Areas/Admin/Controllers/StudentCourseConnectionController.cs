using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GalileuszSchool.Infrastructure;
using GalileuszSchool.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GalileuszSchool.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class StudentCourseConnectionController : Controller
    {
        private readonly GalileuszSchoolContext context;

        public StudentCourseConnectionController(GalileuszSchoolContext context)
        {
            this.context = context;
        }

        public async Task<IActionResult> AddIds(StudentCourseConnection studentCourseConnection, int courseId, int studentId)
        {
            studentCourseConnection.CourseId = courseId;
            studentCourseConnection.StudentId= studentId;


            context.Add(studentCourseConnection);
            await context.SaveChangesAsync();
            return RedirectToAction("Index", "Courses");
        }

        public async Task<IActionResult> ShowStudentForCourse()
        {
            var connection = await context.StudenCourseConnections.OrderByDescending(x => x.CourseId).ToListAsync();
            //List<Course> courses = new List<Course>();
            //List<Student> students = new List<Student>();
            //foreach (var item in connection)
            //{
            //    Course course = await context.Courses.FindAsync(item.CourseId);
            //    courses.Add(course);

            //    Student student = await context.Students.FindAsync(item.StudentId);
            //    students.Add(student);
            //}
            return View();
        }
    }
}