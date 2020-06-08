using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GalileuszSchool.Infrastructure;
using GalileuszSchool.Models;
using GalileuszSchool.Areas.Admin.ViewModels;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using GalileuszSchool.Models.ModelsForAdminArea;

namespace GalileuszSchool.Areas.Admin.Controllers
{
    //[Authorize(Roles = "admin")]
    [Area("Admin")]
    public class LessonPlanController : Controller
    {
        private readonly GalileuszSchoolContext _context;

        public LessonPlanController(GalileuszSchoolContext context)
        {
            _context = context;
        }

        // GET: Admin/LessonPlans
        public async Task<IActionResult> Index()
        {
            List<ClassRoom> classRooms = new List<ClassRoom>();
            foreach (var classRoom in _context.ClassRoom)
            {
                classRooms.Add(classRoom);
            }
            ViewBag.ClassRoomsList = classRooms;

            await _context.LessonPlan.Include(x => x.Course).ToListAsync();
            await _context.LessonPlan.Include(x => x.ClassRoom).ToListAsync();

            List<List<List<LessonPlan>>> datas = new List<List<List<LessonPlan>>>();
            lessonListGenerator(datas);
            var model = new LessonPlanViewModel() { LessonsList = datas };
            //return View(_context.LessonPlan.OrderBy(a => a.startTime));
            return View(model);


            //return View(await context.Teachers.OrderByDescending(x => x.Id).Include(x => x.Course).ToListAsync());
        }

        // GET: Admin/LessonPlans/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            List<Course> coursesList = new List<Course>();
            foreach (var classRoom in _context.Courses)
            {
                coursesList.Add(classRoom);
            }
            ViewBag.ClassRoomsList = coursesList;


            if (id == null)
            {
                return NotFound();
            }

            var lessonPlan = await _context.LessonPlan
                .FirstOrDefaultAsync(m => m.Id == id);
            if (lessonPlan == null)
            {
                return NotFound();
            }

            return View(lessonPlan);
        }

        // GET: Admin/LessonPlans/Create
        public IActionResult Create()
        {
            ViewBag.Test = new SelectList(_context.ClassRoom.OrderBy(x => x.ClassRoomNumber), "Id", "ClassRoomName");
            ViewBag.CourseId = new SelectList(_context.Courses.OrderBy(x => x.Id), "Id", "Name");
            
            return View();
        }

        // POST: Admin/LessonPlans/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( LessonPlan lessonPlan)
        {
            if (ModelState.IsValid)
            {
            lessonPlan.day = (Days)lessonPlan.dayId;

                if (!TimeValidation(lessonPlan))
                {
                    ModelState.AddModelError("", "The course already exists");
                    return View(lessonPlan);
                }
                
                _context.Add(lessonPlan);
                await _context.SaveChangesAsync();
                TempData["Success"] = "The lesson has been added";
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError("", "The course already exists");
            return View(lessonPlan);
        }

        // GET: Admin/LessonPlans/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            LessonPlan lessonPlan = await _context.LessonPlan.FindAsync(id);
            if (id == null)
            {
                return NotFound();
            }

            //var lessonPlan = await _context.LessonPlan.FindAsync(id);
            if (lessonPlan == null)
            {
                return NotFound();
            }

            ViewBag.CourseId = new SelectList(_context.Courses.OrderBy(x => x.Sorting), "Id", "Name", lessonPlan.CourseId);         
            return View(lessonPlan);
        }

        // POST: Admin/LessonPlans/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,  LessonPlan lessonPlan)
            //[Bind("Id,classroom,dayId,startTime,stopTime, course")]
        {
            if (id != lessonPlan.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                lessonPlan.day = (Days)lessonPlan.dayId;
                try
                {
                    _context.Update(lessonPlan);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LessonPlanExists(lessonPlan.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(lessonPlan);
        }

        // GET: Admin/LessonPlans/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lessonPlan = await _context.LessonPlan
                .FirstOrDefaultAsync(m => m.Id == id);
            if (lessonPlan == null)
            {
                return NotFound();
            }

            return View(lessonPlan);
        }

        // POST: Admin/LessonPlans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var lessonPlan = await _context.LessonPlan.FindAsync(id);
            _context.LessonPlan.Remove(lessonPlan);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LessonPlanExists(int id)
        {
            return _context.LessonPlan.Any(e => e.Id == id);
        }


        private Boolean TimeValidation(LessonPlan lessonPlan)
        {
            if (lessonPlan.startTime >= lessonPlan.stopTime)
            {
                ModelState.AddModelError("", "Course duration is 0");
                return false;
            }
            foreach (LessonPlan existingLesson in _context.LessonPlan)
            {
                if(!(existingLesson.startTime >= lessonPlan.stopTime ||
                    existingLesson.stopTime <= lessonPlan.startTime) &&
                    existingLesson.dayId == lessonPlan.dayId &&
                    existingLesson.ClassRoomId == lessonPlan.ClassRoomId)
                {

                    return false;
                }
                Console.WriteLine(existingLesson.startTime);
            }
            return true;
        }

        private void lessonListGenerator(List<List<List<LessonPlan>>> datas)
        {
            var sortedLessonContext =_context.Set<LessonPlan>().OrderBy(LessonPlan => LessonPlan.startTime);

            //_context.LessonPlan.OrderBy(p => p.startTime);


            int maxClassRoomId = 0;

            int numberOfDays = Enum.GetNames(typeof(Days)).Length;

            foreach (LessonPlan lesson in sortedLessonContext)
            {
                if (lesson.ClassRoomId > maxClassRoomId)
                {
                    maxClassRoomId = lesson.ClassRoomId - 1;
                }
            }

            for (int i = 0; i < maxClassRoomId + 1; i++)
            {
                datas.Add(new List<List<LessonPlan>>());
                for (int j = 0; j < numberOfDays; j++)
                {
                    datas[i].Add(new List<LessonPlan>());
                }
            }

            foreach (LessonPlan lesson in sortedLessonContext)
            {
                datas[lesson.ClassRoomId - 1][lesson.dayId].Add(lesson);
            }
            //ViewData
            datas = addBrake(datas, maxClassRoomId, numberOfDays);
            ViewBag.ListOfLessons = datas;
            Debug.WriteLine("55");
        }

        private List<List<List<LessonPlan>>> addBrake(List<List<List<LessonPlan>>> datas,
                                                            int maxClassRoomId, int numberOfDays)
        {
            for (int roomId = 0; roomId <= maxClassRoomId; roomId++)
            {
                TimeSpan? firstLessonInWeek = null;
                firstLessonInWeek = checkFirstLessonInWeek(datas, firstLessonInWeek, numberOfDays, roomId);
                datas = addFirstGap(datas, roomId, numberOfDays, firstLessonInWeek);
                datas = addEveryNextGap(datas, roomId, numberOfDays);
            }
            return datas;
        }

        private TimeSpan? checkFirstLessonInWeek(List<List<List<LessonPlan>>> datas,
                                    TimeSpan? firstLessonInWeek, int numberOfDays, int roomId)
        {
            for (int dayId = 0; dayId < numberOfDays; dayId++)
            {
                foreach (LessonPlan lesson in datas[roomId][dayId])
                {
                    if (firstLessonInWeek == null)
                    {
                        firstLessonInWeek = lesson.startTime;
                    }
                    else if (firstLessonInWeek > lesson.startTime)
                    {
                        firstLessonInWeek = lesson.startTime;
                    }
                }
            }
            return firstLessonInWeek;
        }

        private List<List<List<LessonPlan>>> addFirstGap(List<List<List<LessonPlan>>> datas,
                                    int roomId, int numberOfDays, TimeSpan? firstLessonInWeek)
        {
            for (int dayId = 0; dayId < numberOfDays; dayId++)
            {
                int indexOfLessonInDay = 0;
                Boolean firstLessonCheck = true;
                for (int lessonId = 0; lessonId < datas[roomId][dayId].Count; lessonId++)
                {
                    if (firstLessonCheck)
                    {
                        firstLessonCheck = false;
                        if (datas[roomId][dayId][lessonId].startTime > firstLessonInWeek)
                        {
                            datas[roomId][dayId].Insert(indexOfLessonInDay, new LessonPlan { startTime =
                                (TimeSpan)firstLessonInWeek,
                                stopTime = datas[roomId][dayId][lessonId].startTime, isGap = true });
                        }
                    }
                }
            }
            return datas;
        }

        private List<List<List<LessonPlan>>> addEveryNextGap(List<List<List<LessonPlan>>> datas,
                                                                        int roomId, int numberOfDays)
        {
            for (int dayId = 0; dayId < numberOfDays; dayId++)
            {
                TimeSpan? tempStopTime = null;
                for (int lessonId = 0; lessonId < datas[roomId][dayId].Count; lessonId++)
                {
                    if (tempStopTime == null)
                    {
                        tempStopTime = datas[roomId][dayId][lessonId].stopTime;
                    } else
                    {
                        if (datas[roomId][dayId][lessonId].startTime > tempStopTime)
                        {
                            datas[roomId][dayId].Insert(lessonId, new LessonPlan { startTime = (TimeSpan)tempStopTime,
                                stopTime = datas[roomId][dayId][lessonId].startTime , isGap = true });
                            tempStopTime = datas[roomId][dayId][lessonId].stopTime;
                        }
                    }

                }
            }
            return datas;
        }
    }
}
