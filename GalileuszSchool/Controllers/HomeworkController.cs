using GalileuszSchool.Infrastructure;
using GalileuszSchool.Models.ModelsForAdminArea;
using GalileuszSchool.Models.ModelsForNormalUsers;
using GalileuszSchool.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GalileuszSchool.Controllers
{
    public class HomeworkController : Controller
    {
        private readonly GalileuszSchoolContext _context;

        //private readonly IRepository<Homework> _repository;

        public HomeworkController(GalileuszSchoolContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            
            var teacherInfo = _context.Teachers.OrderBy(x => x.Id);
            var selectListTeachers = await GetSelectListItem(teacherInfo);
            ViewBag.TeacherId = new SelectList(selectListTeachers, "Value", "Text");

            return View();
        }

        private async Task<IEnumerable<SelectListItem>> GetSelectListItem(IOrderedQueryable<IListItem> dbData)
        {
            IEnumerable<SelectListItem> selectList = await dbData.Select(s => new SelectListItem
            {
                Value = s.Id.ToString(),
                Text = s.FirstName + " " + s.LastName.ToString()
            }).ToListAsync();
            return selectList;
        }

        [HttpPost]
        public async Task<IActionResult> Create(Homework homework)
        {
            //await _repository.Create(homework);



            return Json(new { text = "Server error!" });
        }
    }
}
