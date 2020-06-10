using GalileuszSchool.Infrastructure;
using GalileuszSchool.Models;
using GalileuszSchool.Models.ModelsForNormalUsers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace GalileuszSchool.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly GalileuszSchoolContext _context;
        
        public UserController(UserManager<AppUser> userManager,
                                GalileuszSchoolContext context)
        {
            _userManager = userManager;
            _context = context;
        }



        public IActionResult Index()
        {
            return View();
        }

        public async Task<JsonResult> GetUsers()
        {
            List<AppUser> users = await _context.Users.OrderByDescending(x => x.Id).ToListAsync();
            return Json(users);
        }

        public async Task<IActionResult> FindUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            return new JsonResult(user);
        }

        //get/admin/teachers/delete/{id}
        public async Task<IActionResult> Delete(string id)
        {
            AppUser user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { text = "User does not exists!" });
            }
            else
            {
                await _userManager.DeleteAsync(user);
            }

            return Ok();
        }
    }

}
