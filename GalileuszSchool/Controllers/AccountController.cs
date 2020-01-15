using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GalileuszSchool.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GalileuszSchool.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {

        private readonly UserManager<AppUser> userManager;

        public AccountController(UserManager<AppUser> userManager)
        {
            this.userManager = userManager;
        }
        // get account/register
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        // post account/register
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(User user)
        {
            if (ModelState.IsValid)
            {
                AppUser appUser = new AppUser
                {
                    UserName = user.UserName,
                    Email = user.Email
                };

                IdentityResult result = await userManager.CreateAsync(appUser, user.Password);
                if (result.Succeeded)
                {
                    return RedirectToAction("Login");
                }
                else
                {
                    foreach (IdentityError error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }

            return View(user); 
        }
    }
}