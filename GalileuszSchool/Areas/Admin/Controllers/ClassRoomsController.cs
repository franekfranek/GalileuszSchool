using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GalileuszSchool.Infrastructure;
using GalileuszSchool.Models;
using Microsoft.AspNetCore.Authorization;
using GalileuszSchool.Repository;
using GalileuszSchool.Models.ModelsForAdminArea;

namespace GalileuszSchool.Areas.Admin.Controllers
{
    //[Authorize(Roles = "admin")]
    [Area("Admin")]
    public class ClassRoomsController : Controller
    {
        private readonly IRepository<ClassRoom> _repository;

        public ClassRoomsController(IRepository<ClassRoom> repository)
        {
            _repository = repository;
        }

        // GET: Admin/ClassRooms
        public async Task<IActionResult> Index()
        {
            return View(await _repository.GetAll().ToListAsync());
        }

        // GET: Admin/ClassRooms/Details/5
        public async Task<IActionResult> Details(int id)
        {
            ClassRoom classRoom = await _repository.GetById(id);

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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
                [Bind("Id,ClassRoomNumber,ClassRoomName,ClassRoomCapacity")] ClassRoom classRoom)
        {
            if (ModelState.IsValid)
            {
                await _repository.Create(classRoom);
                return RedirectToAction(nameof(Index));
            }
            return View(classRoom);
        }

        // GET: Admin/ClassRooms/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var classRoom = await _repository.GetById(id);
            if (classRoom == null)
            {
                return NotFound();
            }
            return View(classRoom);
        }

        // POST: Admin/ClassRooms/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
             [Bind("Id,ClassRoomNumber,ClassRoomName,ClassRoomCapacity")] ClassRoom classRoom)
        {
            if (id != classRoom.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _repository.Update(classRoom);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!(await ClassRoomExists(classRoom.Id)))
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
        public async Task<IActionResult> Delete(int id)
        {
            var classRoom = await _repository.GetById(id);
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
            var classRoom = await _repository.GetById(id);
            await _repository.Delete(id);

            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> ClassRoomExists(int id)
        {
            return await _repository.IsInDB(id);
        }
    }
}
