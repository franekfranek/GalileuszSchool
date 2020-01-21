using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GalileuszSchool.Infrastructure;
using GalileuszSchool.Models;

namespace GalileuszSchool.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ClassRoomsController : Controller
    {
        private readonly GalileuszSchoolContext _context;

        public ClassRoomsController(GalileuszSchoolContext context)
        {
            _context = context;
        }

        // GET: Admin/ClassRooms
        public async Task<IActionResult> Index()
        {
            return View(await _context.ClassRoom.ToListAsync());
        }

        // GET: Admin/ClassRooms/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var classRoom = await _context.ClassRoom
                .FirstOrDefaultAsync(m => m.Id == id);
            if (classRoom == null)
            {
                return NotFound();
            }

            return View(classRoom);
        }

        // GET: Admin/ClassRooms/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/ClassRooms/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ClassRoomNumber,ClassRoomName,ClassRoomCapacity")] ClassRoom classRoom)
        {
            if (ModelState.IsValid)
            {
                _context.Add(classRoom);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(classRoom);
        }

        // GET: Admin/ClassRooms/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var classRoom = await _context.ClassRoom.FindAsync(id);
            if (classRoom == null)
            {
                return NotFound();
            }
            return View(classRoom);
        }

        // POST: Admin/ClassRooms/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ClassRoomNumber,ClassRoomName,ClassRoomCapacity")] ClassRoom classRoom)
        {
            if (id != classRoom.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(classRoom);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClassRoomExists(classRoom.Id))
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
            return View(classRoom);
        }

        // GET: Admin/ClassRooms/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var classRoom = await _context.ClassRoom
                .FirstOrDefaultAsync(m => m.Id == id);
            if (classRoom == null)
            {
                return NotFound();
            }

            return View(classRoom);
        }

        // POST: Admin/ClassRooms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var classRoom = await _context.ClassRoom.FindAsync(id);
            _context.ClassRoom.Remove(classRoom);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClassRoomExists(int id)
        {
            return _context.ClassRoom.Any(e => e.Id == id);
        }
    }
}
