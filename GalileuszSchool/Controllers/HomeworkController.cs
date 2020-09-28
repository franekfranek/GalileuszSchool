using GalileuszSchool.Infrastructure;
using GalileuszSchool.Models.ModelsForAdminArea;
using GalileuszSchool.Models.ModelsForNormalUsers;
using GalileuszSchool.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace GalileuszSchool.Controllers
{
    public class HomeworkController : Controller
    {
        private readonly GalileuszSchoolContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IRepository<Homework> _repositoryHomework;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public HomeworkController(GalileuszSchoolContext context,
                                  UserManager<AppUser> userManager,
                                  IRepository<Homework> repository,
                                  IWebHostEnvironment env)
        {
            _context = context;
            _userManager = userManager;
            _repositoryHomework = repository;
            _webHostEnvironment = env;
        }

        [Authorize]
        public IActionResult Index()
        {   
            return View();
        }

        [HttpPost]
        [Authorize]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Homework homework)
        {
            //var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                var email = user.Email;
                var teacher = await _context.Teachers.FirstOrDefaultAsync(x => x.Email == email);
                var slug = await _repositoryHomework.GetBySlug(homework.Slug);

                if (slug != null)
                {
                    return Json(new { text = "Homework with this title already exists already exists!" });
                }
                string imageName = null;
                if (homework.PhotoContent != null)
                {
                    string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/homeworks");
                    imageName = Guid.NewGuid().ToString() + "_" + homework.PhotoContent.FileName; // this gives unique id so no same image twice uploaded 
                    string filePath = Path.Combine(uploadsDir, imageName);
                    FileStream fileStream = new FileStream(filePath, FileMode.Create);
                    await homework.PhotoContent.CopyToAsync(fileStream);
                    fileStream.Close();
                }

                var homeworkModel = new Homework()
                {
                    Title = homework.Title,
                    TextContent = homework.TextContent,
                    TeacherId = teacher.Id,
                    Slug = homework.Title.ToLower(),
                    ImageContent = imageName,
                    Course = homework.Course
                };
                await _repositoryHomework.Create(homeworkModel);
                return Json(new { text = "Homework added!" });
            }

            return Json(new { text = "Server error!" });
        }

        //todo: refactor getHomeworks...:(
        [Authorize]
        public async Task<JsonResult> GetHomeworks(string option)
        {
            AppUser user = null;
            Expression<Func<StudentHomework, bool>> whereExpressionForStudent = null;
            Expression<Func<StudentHomework, bool>> whereExpressionForTeacher = null;
            Teacher teacher;
            try
            {
                user = await _userManager.GetUserAsync(User);
                teacher = await _context.Teachers.FirstOrDefaultAsync(x=> x.Email == user.Email);
                var student = await _context.Students.FirstOrDefaultAsync(x=> x.Email == user.Email);

                switch (option)
                {
                    case "All":
                        whereExpressionForStudent = x => x.StudentId == student.Id;
                        whereExpressionForTeacher = x => x.HomeworkId != 0;
                        break;
                    case "Submitted":
                        whereExpressionForStudent = x => x.StudentId == student.Id && x.IsDone == true;
                        whereExpressionForTeacher = x => x.IsDone == true;
                        break;
                    case "Unsubmitted":
                        whereExpressionForStudent = x => x.StudentId == student.Id && x.IsDone == false;
                        whereExpressionForTeacher = x => x.IsDone == false;
                        break;

                }
            }
            catch (Exception e)
            {

                return Json(new { text = "Server error! Error message:" + e.Message });
            }

            List<Homework> homeworks = GetHoweworksForTeachersOrStudents(user, whereExpressionForStudent,
                                                                            whereExpressionForTeacher, teacher, option).Result;
           

            return Json(homeworks);
        }

        private async Task<List<Homework>> GetHoweworksForTeachersOrStudents(AppUser user, Expression<Func<StudentHomework, bool>> whereExpressionForStudent,
                        Expression<Func<StudentHomework, bool>> whereExpressionForTeacher, Teacher teacher, string option)
        {
            List<Homework> homeworks = new List<Homework>();
            if (user != null)
            {
                if (user.IsTeacher)
                {
                    if(option == "All")
                    {
                        homeworks = await _context.Homework.Where(x => x.TeacherId == teacher.Id).Include(x => x.Teacher).ToListAsync();

                    }
                    else
                    {
                        homeworks = await _context.StudentHomework
                                                        .Where(whereExpressionForTeacher)
                                                        .Include(x => x.Homework).ThenInclude(x => x.Teacher)
                                                        .Select(x => x.Homework).Where(x => x.TeacherId == teacher.Id)
                                                        .Distinct()
                                                        .ToListAsync();
                    }

                }
                else
                {
                    homeworks = await _context.StudentHomework
                                                    .Where(whereExpressionForStudent)
                                                    .Include(x => x.Homework).ThenInclude(x => x.Teacher)
                                                    .Select(x => x.Homework)
                                                    .ToListAsync();
                }
            }
            return homeworks;
        }

        [Authorize]
        public async Task<IActionResult> FindHomework(int id)
        {
            var homework = await _repositoryHomework.GetById(id);
            if(homework != null)
            {
                return new JsonResult(homework);
            }
            else
                return Json(new { text = "Server error!" });

        }
        [Authorize]

        public async Task<IActionResult> IsStudentOrTeacher()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                return Json(new { isTeacher = user.IsTeacher, isStudent = user.IsStudent });


            }
            else
                return Json(new { text = "Server error!" });
        }

        [Authorize]
        public async Task<IActionResult> GetCoursesOfTeacher()
        {
            var teacher = GetLoggedTeacher().Result;

            var courses = await _context.Courses.Where(x => x.TeacherId == teacher.Id).ToListAsync();
            if (teacher != null && courses != null)
            {
                return Json(courses);
            }
            else
                return Json(new { text = "Server error!" });

        }

        //make it non-generic 
        private async Task<Teacher> GetLoggedTeacher()
        {
            var user = await _userManager.GetUserAsync(User);
            return await _context.Teachers.FirstOrDefaultAsync(x => x.Email == user.Email);
        }
    }
}
