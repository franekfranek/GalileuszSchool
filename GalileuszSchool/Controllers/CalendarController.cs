using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Web.Helpers;
using GalileuszSchool.Infrastructure;
using GalileuszSchool.Models.DTOs;
using GalileuszSchool.Models.ModelsForAdminArea;
using GalileuszSchool.Models.ModelsForNormalUsers;
using GalileuszSchool.Models.ModelsForNormalUsers.Calendar;
using GalileuszSchool.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GalileuszSchool.Controllers
{
    public class CalendarController : Controller
    {
        private readonly GalileuszSchoolContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly string[] borderColors = new string[] { "#FF756D", "#85DE77", "#CE9DD9", "#88AED0" };

        public CalendarController(GalileuszSchoolContext context,
                                    UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize]
        //get calendar/index
        public async Task<IActionResult> Index()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                var currentTeacher = await _context.Teachers.FirstOrDefaultAsync(x => x.Email == user.Email);

                var courses = _context.Courses.Where(x => x.TeacherId == currentTeacher.Id);
                var selectListTeachers = await courses.Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.Name.ToString()
                }).ToListAsync();
                ViewBag.Courses = new SelectList(selectListTeachers, "Value", "Text");

            }
            catch (Exception e)
            {

            }

            return View();
        }

        //get calendar/getevents
        [Authorize]
        public async Task<JsonResult> GetEvents()
        {
            List<CalendarEvent> events = new List<CalendarEvent>();
            try
            {
                var user = await _userManager.GetUserAsync(User);
                var currentTeacher = await _context.Teachers.FirstOrDefaultAsync(x => x.Email == user.Email);
                var courses = await _context.Courses.Where(x => x.TeacherId == currentTeacher.Id).Select(x => x.Id).ToListAsync();
                events = await _context.CalendarEvents.Where(x => courses.Contains(x.CourseId)).ToListAsync();
            }
            catch(Exception e) 
            {
                //return Json(new { text = "Server error! });
            }
            

            return Json(events);
        }
        //get calendar/GetEventsForStudent
        [Authorize]
        public async Task<JsonResult> GetEventsForStudents()
        {
            List<CalendarEvent> events;
            try
            {
                var user = await _userManager.GetUserAsync(User);
                var currentStudent = await _context.Students.FirstOrDefaultAsync(x => x.Email == user.Email);
                events = await _context.CalendarEventStudents
                            .Where(x => x.StudentId == currentStudent.Id)
                            .Include(x => x.CalendarEvent).Select(x => x.CalendarEvent).ToListAsync();
            }
            catch (Exception e)
            {
                return Json(new { text = "Server error!" });
            }
            return Json(events);
        }
        //post calendar/create
        [HttpPost]
        [Authorize]
        public async Task<JsonResult> Create(CalendarEvent calendarEvent)
        {
            if (ModelState.IsValid)
            {
                if(calendarEvent != null)
                {
                    var course = await _context.Courses.FirstOrDefaultAsync(x => x.Id == calendarEvent.CourseId);
                    calendarEvent.Title += " ";
                    calendarEvent.Title += course.Name;
                    calendarEvent.Color = GetRandomColor();

                    _context.CalendarEvents.Add(calendarEvent);
                    var save = await _context.SaveChangesAsync();

                    if (course != null && save > 0)
                    {
                        await SaveStudentsByEvent(calendarEvent.CourseId, (int)calendarEvent.Id);
                    }
                }

                return Json(new { text = "Class added!" });
            }
            return Json(new { text = "Server error!" });
        }

        //it is void but it's async so it returns Task 
        private async Task SaveStudentsByEvent(int courseId, int eventId)
        {
            try
            {
                var studentsByCourse = await _context.StudenCourseConnections.Where(x => x.CourseId == courseId).ToListAsync();
                foreach (var item in studentsByCourse)
                {
                    _context.CalendarEventStudents.Add(new CalendarEventStudent { CalendarEventId = eventId, StudentId = item.StudentId });
                }
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {

                throw e;
            }            
        }

        //post calendar/edit/event
        [HttpPost]
        [Authorize]
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
            return Json(new { text = "Server error!" });
        }
        //post calendar/delete/ {id}
        [HttpPost]
        [Authorize]
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
            return Json(new { text = "Server error!" });
        }

        private string GetRandomColor()
        {
            Random random = new Random();
            int indexRandom = random.Next(0, borderColors.Length);
            return borderColors[indexRandom];
        }

        //get /calendar/GetStudentsByEvent/ {id}
        [Authorize]
        public async Task<JsonResult> GetStudentsByEvent(int eventId)
        {
            var students = await _context.CalendarEventStudents
                        .OrderBy(x => x.CalendarEventId)
                        .Where(x => x.CalendarEventId == eventId)
                        .Include("Student").ToListAsync();
            if(students != null)
            {
                return Json(students);

            }else
                return Json(new { text = "Server error!" });

        }

        //post
        [HttpPost]
        [Authorize]
        public async Task<JsonResult> CheckAttendance(AttendanceForm[] attendanceForms)
        {
            if (ModelState.IsValid)
            {
                foreach (AttendanceForm attendance in attendanceForms)
                {
                    var studentEvent = await _context.CalendarEventStudents
                        .FirstOrDefaultAsync(x => x.StudentId == attendance.StudentId && x.CalendarEventId == attendance.EventId);

                    studentEvent.IsPresent = attendance.IsPresent;
                }
                await _context.SaveChangesAsync();

                return Json(new { info = "Attendance saved!" });
            }

            return Json(new { info = "Server error" });
        }
    }
}
