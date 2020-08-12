using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
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
        private readonly string[] borderColors = new string[] { "#FF756D", "#85DE77", "#CE9DD9", "#88AED0" };

        public CalendarController(GalileuszSchoolContext context)
        {
            _context = context;
        }

        //get calendar/index
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

        //get calendar/getevents
        public async Task<JsonResult> GetEvents()
        {
            var events = await _context.CalendarEvents.Where(x => x.Id != 0).ToListAsync();

            return Json(events);
        }
        //post calendar/create
        public async Task<JsonResult> Create(CalendarEvent calendarEvent)
        {
            if (ModelState.IsValid)
            {
                var course = await _context.Courses.FirstOrDefaultAsync(x => x.Id == calendarEvent.CourseId);
                calendarEvent.Title += " ";
                calendarEvent.Title += course.Name;
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
        //post calendar/edit/event
        public async Task<JsonResult> Edit(CalendarEvent calendarEvent)
        {
            if (ModelState.IsValid)
            {
                if (calendarEvent != null)
                {
                    _context.CalendarEvents.Update(calendarEvent);
                    await _context.SaveChangesAsync();
                    return Json(new { text = "Class edited!" });
                }
            }
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return Json(new { text = "Server error!" });
        }
        //post calendar/delete/ {id}
        public async Task<JsonResult> Delete(int id)
        {
            if (ModelState.IsValid)
            { 
                var classEvent = await _context.CalendarEvents.FirstOrDefaultAsync(x => x.Id == id);
                if (classEvent != null)
                {
                    _context.CalendarEvents.Remove(classEvent);
                    await _context.SaveChangesAsync();
                    return Json(new { text = "Class removed!" });
                }    
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
        
    }
}
