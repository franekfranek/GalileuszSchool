using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GalileuszSchool.Infrastructure;
using GalileuszSchool.Models;
using GalileuszSchool.Models.ModelsForAdminArea;
using GalileuszSchool.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GalileuszSchool.Areas.Admin.Controllers
{
    //[Authorize(Roles = "admin")]
    [Area("Admin")]
    public class PagesController : Controller
    {
        private readonly IRepository<Page> _repository;

        public PagesController(IRepository<Page> repository)
        {
            _repository = repository;
        }
        // /admin/pages
        public async Task<IActionResult> Index()
        {
            var pages = await _repository.GetAll().OrderBy(x => x.Sorting).ToListAsync();

            return View(pages);
        }
        // /admin/pages/details/{id}
        public async Task<IActionResult> Details(int id)
        {
            Page page = await _repository.GetById(id);

            if (page == null)
            {
                return NotFound();
            }

            return View(page);
        }
        // /admin/pages/create
        public IActionResult Create() => View();

        //POST /admin/pages/create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Page page)
        {
            if (ModelState.IsValid)
            {
                page.Slug = page.Title.ToLower().Replace(" ", "-");
                page.Sorting = 100;

                var slug = await _repository.GetBySlug(page.Slug);
                if (slug != null)
                {
                    ModelState.AddModelError("", "The title already exists");
                    return View(page);
                }

                await _repository.Create(page);
                
                TempData["Success"] = "The page has been added";

                return RedirectToAction("Index");
            }
            return View(page);
        }
        //get /admin/pages/edit/{id}
        public async Task<IActionResult> Edit(int id)
        {
            Page page = await _repository.GetById(id);

            if (page == null)
            {
                return NotFound();
            }

            return View(page);
        }

        //POST /admin/pages/edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Page page)
        {
            if (ModelState.IsValid)
            {
                page.Slug = page.Id == 1 ? "home" : page.Title.ToLower().Replace(" ", "-");


                var slug = await _repository.GetModelByWhereAndFirstConditions(x => x.Id != page.Id, x => x.Slug == page.Slug);
                if (slug != null)
                {
                    ModelState.AddModelError("", "The page already exists");
                    return View(page);
                }

                await _repository.Update(page);

                TempData["Success"] = "The page has been edited";

                return RedirectToAction("Index");
            }

            return View(page);
        }

        //get(can be post too) /admin/pages/delete/{id}
        public async Task<IActionResult> Delete(int id)
        {
            Page page = await _repository.GetById(id);

            if (page == null)
            {
                TempData["Error"] = "The page does not exist";
            }
            else
            {
                await _repository.Delete(id);
                TempData["Success"] = "The page has been deleted";
            }

            return RedirectToAction("Index");
        }
        
    }
}