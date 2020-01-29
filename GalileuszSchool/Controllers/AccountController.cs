using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GalileuszSchool.Infrastructure;
using GalileuszSchool.Models;
using GalileuszSchool.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebPWrecover.Services;

namespace GalileuszSchool.Controllers
{
    //[Authorize]
    public class AccountController : Controller
    {

        private readonly UserManager<AppUser> userManager;
        private readonly SignInManager<AppUser> signInManager;
        private IPasswordHasher<AppUser> passwordHasher;
        private readonly GalileuszSchoolContext context;
        private readonly ILogger<AccountController> logger;
        private readonly IEmailSender emailSender;

        public AccountController(UserManager<AppUser> userManager,
                                SignInManager<AppUser> signInManager,
                                IPasswordHasher<AppUser> passwordHasher,
                                GalileuszSchoolContext context,
                                ILogger<AccountController> logger,
                                IEmailSender emailSender)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.passwordHasher = passwordHasher;
            this.context = context;
            this.logger = logger;
            this.emailSender = emailSender;
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

                var isNameAlreadyExists = context.Users.Any(x => x.UserName == user.UserName);

                if (isNameAlreadyExists)
                {
                    ModelState.AddModelError("UserName", "User with this name already exists");
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
                    var token = await userManager.GenerateEmailConfirmationTokenAsync(appUser);

                    var confirmationLink = Url.Action("ConfirmEmail", "Account",
                                            new { userId = appUser.Id, token = token }, Request.Scheme);

                    logger.Log(LogLevel.Warning, confirmationLink);


                    await emailSender.SendEmailAsync(appUser.Email, "Email Confirmation",
                        "Your confirmation link: "+ confirmationLink);

                    TempData["Success"] = "You succesufully registered your account! We sent you email confirmation";
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
                //if (appUser != null && appUser.EmailConfirmed)

                if (appUser != null)
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
            var oldEmail = appUser.Email;
            var oldName = appUser.UserName;

            if (ModelState.IsValid)
            {
                //email edit
                var isEmailAlreadyExists = context.Users.Any(x => x.Email == userEdit.Email);
                if (oldEmail != userEdit.Email)
                {
                    if (isEmailAlreadyExists)
                    {
                        ModelState.AddModelError("Email", "User with this email already exists");
                        return View(userEdit);
                    }
                    appUser.Email = userEdit.Email;
                }
                else { appUser.Email = oldEmail; }

                //name edit
                var isNameAlreadyExists = context.Users.Any(x => x.UserName == userEdit.UserName);
                if(oldName != userEdit.UserName)
                {
                    if (isNameAlreadyExists)
                    {
                        ModelState.AddModelError("UserName", "User with this name already exists");
                        return View(userEdit);
                    }
                    appUser.UserName = userEdit.UserName;
                }
                else { appUser.UserName = oldName; }



                //password edit
                if (userEdit.Password != null)
                {
                    appUser.PasswordHash = passwordHasher.HashPassword(appUser, userEdit.Password);
                }


                if (oldEmail != appUser.Email)
                {

                    appUser.EmailConfirmed = false;
                }

                IdentityResult result = await userManager.UpdateAsync(appUser);
                if (result.Succeeded)
                {
                    if (oldEmail != appUser.Email) {

                        var token = await userManager.GenerateEmailConfirmationTokenAsync(appUser);

                        var confirmationLink = Url.Action("ConfirmEmail", "Account",
                                                new { userId = appUser.Id, token = token }, Request.Scheme);

                        logger.Log(LogLevel.Warning, confirmationLink);

                        await emailSender.SendEmailAsync(appUser.Email, "Email Confirmation",
                       "Your confirmation link: " + confirmationLink);

                        TempData["Success"] = "We send you an Email with reset password link.";
                    }


                    TempData["Success"] = "Your details have been changed!";
                    return Redirect("/");
                }
                TempData["Error"] = "An error occures. Please try again.";
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

        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
            {
                TempData["Error"] = $"The user Id {userId} is invalid";
                return NotFound();
            }

            var result = await userManager.ConfirmEmailAsync(user, token);

            if (result.Succeeded)
            {
                Response.Headers.Add("REFRESH", "3;URL=login");
                return View();
            }

            TempData["Error"] = "Email cannot be confirmed";
            return RedirectToAction("Login", "Account");

        }

        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();

        }


        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPassword forgotPassword)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(forgotPassword.Email);
                if (user != null && await userManager.IsEmailConfirmedAsync(user))
                {
                    var token = await userManager.GeneratePasswordResetTokenAsync(user);

                    var passwordResetLink = Url.Action("ResetPassword", "Account",
                         new { email = forgotPassword.Email, token = token }, Request.Scheme);

                    logger.Log(LogLevel.Warning, passwordResetLink);

                    await emailSender.SendEmailAsync(forgotPassword.Email, "Reset Password",
                       "Your reset link: " + passwordResetLink);

                    TempData["Success"] = "We send you an Email with reset password link. Redirecting...";

                    Response.Headers.Add("REFRESH", "3;URL=login");
                }
                else { TempData["Error"] = "An error occures. Please try again."; }
                

            }
            return View(forgotPassword);
        }

        [AllowAnonymous]
        public IActionResult ResetPassword(string token, string email)
        {
            if (token == null || email == null)
            {
                ModelState.AddModelError("", "Invalid password reset token");
            }

            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPassword resetPassword)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(resetPassword.Email);

                if (user != null)
                {
                    var result = await userManager.ResetPasswordAsync(user, resetPassword.Token, resetPassword.Password);

                    if (result.Succeeded)
                    {
                        TempData["Success"] = "You have successfully changed your password. Redirecting...";
                        Response.Headers.Add("REFRESH", "3;URL=login");
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }

                    return View(resetPassword);
                }
                TempData["Error"] = "An error occures. Please try again.";

            }
            return View(resetPassword);
        }
    }


}


