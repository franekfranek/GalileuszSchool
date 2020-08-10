using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Helpers;
using GalileuszSchool.Infrastructure;
using GalileuszSchool.Models.ModelsForNormalUsers.Calendar;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GalileuszSchool.Controllers
{
    public class CalendarController : Controller
    {
        private readonly GalileuszSchoolContext _context;
        private readonly string[] borderColors = new string[] { "red", "green", "yellow", "blue" };

        public CalendarController(GalileuszSchoolContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var courses = _context.Courses.OrderBy(x => x.Id);
            var selectListTeachers = await courses.Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.Name.ToString()
                }).ToListAsync();
            ViewBag.Courses = new SelectList(selectListTeachers, "Value", "Text");

            return View();
        }

        public async Task<JsonResult> GetEvents()
        {
            var events = await _context.CalendarEvents.Where(x => x.Id != 0).ToListAsync();

            return Json(events);
        }
        public async Task<JsonResult> Create(CalendarEvent calendarEvent)
        {
            GetErrorListFromModelState(ModelState);
            if (ModelState.IsValid)
            {
                calendarEvent.Color = GetRandomColor();
                
                try
                {
                    _context.CalendarEvents.Add(calendarEvent);
                    await _context.SaveChangesAsync();
                }
                catch (Exception)
                {

                    throw;
                }

                return Json(new { text = "Class added!" });
            }
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return Json(new { text = "Server error!" });
        }

        private string GetRandomColor()
        {
            Random random = new Random();
            int indexRandom = random.Next(0, borderColors.Length);
            return borderColors[indexRandom];
        }
        public static List<string> GetErrorListFromModelState
                                              (ModelStateDictionary modelState)
        {
            var query = from state in modelState.Values
                        from error in state.Errors
                        select error.ErrorMessage;

            var errorList = query.ToList();
            return errorList;
        }
    }
}
