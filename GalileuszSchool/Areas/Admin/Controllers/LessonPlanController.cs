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
using Microsoft.AspNetCore.Authorization;

namespace GalileuszSchool.Areas.Admin.Controllers
{
    //[Authorize(Roles = "admin, editor")]
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
            return View(await _context.LessonPlan.Include(x => x.Course).ToListAsync());


            //return View(await context.Teachers.OrderByDescending(x => x.Id).Include(x => x.Course).ToListAsync());
        }

        // GET: Admin/LessonPlans/Details/5
        public async Task<IActionResult> Details(int? id)
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

        // GET: Admin/LessonPlans/Create
        public IActionResult Create()
        {
            ViewBag.CourseId = new SelectList(_context.Courses.OrderBy(x => x.Id), "Id", "Name");


            return View();
        }

        // POST: Admin/LessonPlans/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( LessonPlan lessonPlan)
            //[Bind("classroom,dayId,startTime,stopTime, course")]
        {


            if (ModelState.IsValid)
            {
            lessonPlan.day = (Days)lessonPlan.dayId;

                _context.Add(lessonPlan);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
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
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,classroom,dayId,startTime,stopTime, course")] LessonPlan lessonPlan)
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
    }
}
