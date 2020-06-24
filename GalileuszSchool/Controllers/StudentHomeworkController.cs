using GalileuszSchool.Infrastructure;
using GalileuszSchool.Models.ModelsForAdminArea;
using GalileuszSchool.Models.ModelsForNormalUsers;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

        public async Task<IActionResult> AddStudentHomeworks(int homeworkId, List<int> studentsIds)
        {
            var studentHomeworks = new List<StudentHomework>();

            foreach (int studentId in studentsIds)
            {
                studentHomeworks.Add(new StudentHomework()
                { HomeworkId = homeworkId, StudentId = studentId });
            }

            try
            {
                foreach (var item in studentHomeworks)
                {
                    _context.Add(item);
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { text = "Server error!" });
            }

            return Json(new { text = "Homework assigned!" });
        }

        // get /admin/StudentHomework/StudentsByhomework/1 (homework id) 
        public async Task<IActionResult> StudentsByHomework(int id)
        {
            var studentsByHomework = await _context.studentHomework
                                                .OrderByDescending(x => x.HomeworkId)
                                                .Where(x => x.HomeworkId == id)
                                                .Include("Student").Select(x => x.Student).ToListAsync();

            //var q = from s in _context.Students
            //               join sh in _context.studentHomework
            //               on s.Id equals sh.StudentId
            //               where sh.HomeworkId == id
            //               select s;
            //var students = await q.ToListAsync();

            return Json(studentsByHomework);
        }

        public async Task<IActionResult> GetRestOfStudent(List<int> alreadyAssignedStudents)
        {

            var students = await _context.Students.Where(x => !alreadyAssignedStudents.Contains(x.Id)).ToListAsync();
            
            return Json(students);
        }
    }
}
