using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GalileuszSchool.Infrastructure;
using GalileuszSchool.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GalileuszSchool.Controllers
{
    //[Authorize]
    public class AccountController : Controller
    {

        private readonly UserManager<AppUser> userManager;
        private readonly SignInManager<AppUser> signInManager;
        private IPasswordHasher<AppUser> passwordHasher;
        private readonly GalileuszSchoolContext context;

        public AccountController(UserManager<AppUser> userManager,
                                SignInManager<AppUser> signInManager,
                                IPasswordHasher<AppUser> passwordHasher,
                                GalileuszSchoolContext context)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.passwordHasher = passwordHasher;
            this.context = context;
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
                var isEmailAlreadyExists = context.Users.Any(x => x.Email == user.Email);

                if (isEmailAlreadyExists)
                {
                    ModelState.AddModelError("Email", "User with this email already exists");
                    return View(user);
                }
                AppUser appUser = new AppUser
                {
                    UserName = user.UserName,
                    Email = user.Email
                };

                

                IdentityResult result = await userManager.CreateAsync(appUser, user.Password);
                if (result.Succeeded)
                {
                    TempData["Success"] = "You succesufully registered your account!";
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

        // /get/account/login
        [AllowAnonymous]
        public IActionResult Login(string returnUrl)
        {
            Login login = new Login
            {
                ReturnUrl = returnUrl
            };


            return View(login);
        }

        // post account/login
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(Login login)
        {
            if (ModelState.IsValid)
            {
                AppUser appUser = await userManager.FindByEmailAsync(login.Email);
                if(appUser != null)
                {
                    Microsoft.AspNetCore.Identity.SignInResult result = await signInManager.PasswordSignInAsync
                        (appUser, login.Password, false, false);

                    if (result.Succeeded)
                    {
                        return Redirect(login.ReturnUrl ?? "/account/edit");
                    }
                }

                ModelState.AddModelError("", "Login failed, wrong credentials or your email is not confirmed");
            }

            return View(login);
        }

        // /get/account/logout
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return Redirect("/");
        }

        // /get/account/edit
        public async Task<IActionResult> Edit()
        {
            AppUser appUser = await userManager.FindByNameAsync(User.Identity.Name);
            UserEdit user = new UserEdit(appUser);

            return View(user);
        }

        // post account/edit
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Edit(UserEdit userEdit)
        {
            AppUser appUser = await userManager.FindByNameAsync(User.Identity.Name);
            if (ModelState.IsValid)
            {
                appUser.Email = userEdit.Email;
                if(userEdit.Password != null)
                {
                    appUser.PasswordHash = passwordHasher.HashPassword(appUser, userEdit.Password);
                }

                IdentityResult result = await userManager.UpdateAsync(appUser);
                if (result.Succeeded)
                {
                    TempData["Success"] = "Your details have been changed!";
                    return Redirect("/");
                }
              
            }

            return View();
        }

        // post account/delete
        
        public async Task<IActionResult> Delete()
        {
            AppUser appUser = await userManager.FindByNameAsync(User.Identity.Name);

            if (appUser == null)
            {
                TempData["Error"] = "User not found";
                return NotFound();

            }
            else
            {
                IdentityResult result = await userManager.DeleteAsync(appUser);

                if (result.Succeeded)
                {
                    TempData["Success"] = "Your account has been deleted.";
                    await Logout();
                    //return RedirectToAction("Login");
                }
                else
                {
                    foreach (IdentityError error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
                return RedirectToAction("Register");
            }
            
        }
            
        }
}

