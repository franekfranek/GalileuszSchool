using GalileuszSchool.Infrastructure;
using GalileuszSchool.Models.ModelsForAdminArea;
using GalileuszSchool.Models.ModelsForNormalUsers;
using GalileuszSchool.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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
        private readonly IRepository<Homework> _repository;

        public HomeworkController(GalileuszSchoolContext context,
                                  UserManager<AppUser> userManager,
                                  IRepository<Homework> repository)
        {
            _context = context;
            _userManager = userManager;
            _repository = repository;
        }

        public async Task<IActionResult> Index()
        {
            
            //var teacherInfo = _context.Teachers.OrderBy(x => x.Id);
            //var selectListTeachers = await GetSelectListItem(teacherInfo);
            //ViewBag.TeacherId = new SelectList(selectListTeachers, "Value", "Text");

            return View();
        }

        //private async Task<IEnumerable<SelectListItem>> GetSelectListItem(IOrderedQueryable<IListItem> dbData)
        //{
        //    IEnumerable<SelectListItem> selectList = await dbData.Select(s => new SelectListItem
        //    {
        //        Value = s.Id.ToString(),
        //        Text = s.FirstName + " " + s.LastName.ToString()
        //    }).ToListAsync();
        //    return selectList;
        //}

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Homework homework)
        {
            //var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                var email = user.Email;
                var teacher = await _context.Teachers.FirstOrDefaultAsync(x => x.Email == email);
                var slug = await _repository.GetBySlug(homework.Slug);

                if (slug != null)
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return Json(new { text = "Homework with this title already exists already exists!" });
                }

                var homeworkModel = new Homework()
                {
                    Title = homework.Title,
                    TextContent = homework.TextContent,
                    TeacherId = teacher.Id,
                    Slug = homework.Title.ToLower()
                };
                await _repository.Create(homeworkModel);
                return Json(new { text = "Homework added!" });
            }
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return Json(new { text = "Server error!" });
        }

        public async Task<JsonResult> GetHomeworks(string option)
        {
            var user = await _userManager.GetUserAsync(User);
            var teacher = await _context.Teachers.FirstOrDefaultAsync(x=> x.Email == user.Email);

            Expression<Func<Homework, bool>> whereExpression = null;

            switch (option)
            {
                case "All":
                    whereExpression = x => x.Slug != null;
                    break;
                case "Done":
                    whereExpression = x => x.IsDone == true;
                    break;
                case "Undone":
                    whereExpression = x => x.IsDone == false;
                    break;

            }
            //List<Homework> homeworks = await _repository.GetAll().OrderByDescending(x => x.CreationDate)
            //                                                .Include(x => x.Teacher).ToListAsync();

            List<Homework> homeworks = await _repository.GetAll().Where(whereExpression)
                                                            .Include(x => x.Teacher).ToListAsync();
            if (user.IsTeacher)
            {
                homeworks = homeworks.Where(x => x.TeacherId == teacher.Id).ToList();
            }

            return Json(homeworks);
        }
        public async Task<IActionResult> FindHomework(int id)
        {
            var homework = await _repository.GetById(id);
            return new JsonResult(homework);
        }

        public async Task<IActionResult> IsStudentOrTeacher()
        {
            var user = await _userManager.GetUserAsync(User);
            return Json(new { isTeacher = user.IsTeacher, isStudent = user.IsStudent });
        }
        
    }
}
