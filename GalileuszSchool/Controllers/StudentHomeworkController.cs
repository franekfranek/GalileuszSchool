using GalileuszSchool.Infrastructure;
using GalileuszSchool.Models.ModelsForAdminArea;
using GalileuszSchool.Models.ModelsForNormalUsers;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace GalileuszSchool.Controllers
{
    public class StudentHomeworkController : Controller
    {
        private readonly GalileuszSchoolContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public StudentHomeworkController(GalileuszSchoolContext context,
                                         UserManager<AppUser> userManager,
                                         IWebHostEnvironment env)
        {
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = env;
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
            var studentsByHomework = await _context.Students.Include(x => x.StudentHomeworks).ToListAsync();

            var s = new List<Student>();

            foreach (var student in studentsByHomework)
            {
                foreach (var item in student.StudentHomeworks)
                {
                    if (item.HomeworkId == id)
                    {
                        s.Add(student);
                    }
                }
            }

            //Where(x => x.StudentHomeworks.Where(y => y.HomeworkId == id)
            //    .Select(x => new {
            //        x,
            //        x.Student,
            //        x.Homework
            //    }).AsEnumerable()

            //studentHomework
            //                                .OrderByDescending(x => x.HomeworkId)
            //                                .Where(x => x.HomeworkId == id)
            //                                .Include("Student")
            //                                .Select(x => x.Student).ToListAsync();

            //var q = from s in _context.Students
            //               join sh in _context.studentHomework
            //               on s.Id equals sh.StudentId
            //               where sh.HomeworkId == id
            //               select s;
            //var students = await q.ToListAsync();

            return Json(s);
        }

        public async Task<IActionResult> GetRestOfStudent(List<int> alreadyAssignedStudents)
        {

            var students = await _context.Students.Where(x => !alreadyAssignedStudents.Contains(x.Id)).ToListAsync();

            return Json(students);
        }

        // get /admin/StudentHomework/GetCurrentStudentHomework/1 (homework id) 
        public async Task<IActionResult> GetCurrentStudentHomework(int id)
        {
            var student = GetLoggedStudent().Result;

            var homework  = await _context.StudentHomework
                    .FirstOrDefaultAsync(x => x.StudentId == student.Id && x.HomeworkId == id);

            return Json(homework);
        }

        public async Task<IActionResult> GetAllStudentHomeworkWithFlag(bool isDone)
        {
            var student = GetLoggedStudent().Result;

            var allSubmittedHomeworks = await _context.StudentHomework
                .Where(x => x.StudentId == student.Id && x.IsDone == isDone)
                .ToListAsync();

            return Json(allSubmittedHomeworks);
            
        }

        public async Task<IActionResult> AddStudentSolution(StudentHomework studentHomework)
        {
            if (ModelState.IsValid)
            {
                var student = GetLoggedStudent().Result;

                var homeworkByStudent = await _context.StudentHomework
                    .FirstOrDefaultAsync(x => x.StudentId == student.Id && x.HomeworkId == studentHomework.HomeworkId);

                homeworkByStudent.IsDone = true;
                homeworkByStudent.StudentSubmissionDate = DateTime.Now;
                homeworkByStudent.SolutionTextContent = studentHomework.SolutionTextContent;
                string imageName = null;

                if (studentHomework.PhotoSolution != null)
                {
                    string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/studentHomeworkSolutions");
                    imageName = Guid.NewGuid().ToString() + "_" + studentHomework.PhotoSolution.FileName; // this gives unique id so no same image twice uploaded 
                    string filePath = Path.Combine(uploadsDir, imageName);
                    FileStream fileStream = new FileStream(filePath, FileMode.Create);
                    await studentHomework.PhotoSolution.CopyToAsync(fileStream);
                    fileStream.Close();
                }
                homeworkByStudent.ImageSolution = imageName;
                homeworkByStudent.PhotoSolution = studentHomework.PhotoSolution;

                _context.StudentHomework.Update(homeworkByStudent);
                await _context.SaveChangesAsync();

                return Json(new { text = "You submitted your assignment!" });
            }
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return Json(new { text = "Server error!" });
        }

        public async Task<Student> GetLoggedStudent()
        {
            var user = await _userManager.GetUserAsync(User);
            return await _context.Students.FirstOrDefaultAsync(x => x.Email == user.Email);
        }

        public async Task<IActionResult> GetSolution(int studentId, int homeworkId)
        {
            var studentSolution = await _context.StudentHomework
                .FirstOrDefaultAsync(x => x.HomeworkId == homeworkId && x.StudentId == studentId);

            return Json(studentSolution);
        }
    }
}
