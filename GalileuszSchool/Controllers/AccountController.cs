using System;
using System.Drawing;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using GalileuszSchool.Areas.Admin.Controllers;
using GalileuszSchool.External;
using GalileuszSchool.Infrastructure;
using GalileuszSchool.Models;
using GalileuszSchool.Models.ModelsForAdminArea;
using GalileuszSchool.Models.ModelsForNormalUsers;
using GalileuszSchool.Models.ModelsForNormalUsers.Calendar;
using GalileuszSchool.Services;
using GalileuszSchool.Services.Facebook;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using WebPWrecover.Services;
using System.Net;
using Microsoft.AspNetCore.Http;

namespace GalileuszSchool.Controllers
{
    //[Authorize]
    public class AccountController : Controller
    {

        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private IPasswordHasher<AppUser> _passwordHasher;
        private readonly GalileuszSchoolContext _context;
        private readonly ILogger<AccountController> _logger;
        private readonly IEmailSender _emailSender;
        private readonly StudentsController _studentsController;
        private readonly TeachersController _teachersController;
        private readonly IFacebookAuthService _facebookAuthService;

        public AccountController(UserManager<AppUser> userManager,
                                SignInManager<AppUser> signInManager,
                                IPasswordHasher<AppUser> passwordHasher,
                                GalileuszSchoolContext context,
                                ILogger<AccountController> logger,
                                IEmailSender emailSender,
                                StudentsController studentsController,
                                TeachersController teachersController,
                                IFacebookAuthService facebookAuthService
                                )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _passwordHasher = passwordHasher;
            _context = context;
            _logger = logger;
            _emailSender = emailSender;
            _studentsController = studentsController;
            _teachersController = teachersController;
            _facebookAuthService = facebookAuthService;
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
                var isEmailAlreadyExists = _context.Users.Any(x => x.Email == user.Email);

                if (isEmailAlreadyExists)
                {
                    ModelState.AddModelError("Email", "User with this email already exists");
                    return View(user);
                }

                var userName = user.FirstName.ToLower() + "-" + user.LastName.ToLower();

                var isUserExists = _context.Users.Any(x => x.UserName == userName);

                if (isUserExists)
                {
                    ModelState.AddModelError("LastName", "User with this credentials already exists");
                    return View(user);
                }

                AppUser appUser = await CreateNewAppUser(user);
               
                IdentityResult result = await _userManager.CreateAsync(appUser, user.Password);

                if (result.Succeeded)
                {
                    await CreateStudentOrTeacher(appUser, user);

                    

                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(appUser);

                    var confirmationLink = Url.Action("ConfirmEmail", "Account",
                                            new { userId = appUser.Id, token = token }, Request.Scheme);

                    _logger.Log(LogLevel.Warning, confirmationLink);


                    await _emailSender.SendEmailAsync(appUser.Email, "Email Confirmation",
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

        private async Task CreateStudentOrTeacher(AppUser appUser, User user)
        {
            if (appUser.IsStudent)
            {
                var currentStudent = await _userManager.FindByNameAsync(appUser.UserName);
                await _userManager.AddToRoleAsync(currentStudent, "student");
                await _studentsController.Create(new Student
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PhoneNumber = "000-000-000",
                    Email = user.Email
                });
            }

            if (appUser.IsTeacher)
            {
                var currentTeacher = await _userManager.FindByNameAsync(appUser.UserName);
                await _userManager.AddToRoleAsync(currentTeacher, "teacher");
                await _teachersController.Create(new Teacher
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PhoneNumber = "000-000-000",
                    Email = user.Email
                });
            }
        }

        private async Task<AppUser> CreateNewAppUser(User user)
        {
            var appUser = new AppUser
            {
                UserName = user.FirstName.ToLower() + "-" + user.LastName.ToLower(),
                Email = user.Email,
                IsStudent = user.IsStudent,
                PhoneNumber = user.PhoneNumber,
                IsTeacher = user.IsTeacher,
                RegistrationDate = DateTime.Now,
                //due to limited resources with email sender provider
                EmailConfirmed = true

        };
            return appUser;
        }

        // /get/account/login
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl)
        {
            Login login = new Login
            {
                ReturnUrl = returnUrl,
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
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
                AppUser appUser = await _userManager.FindByEmailAsync(login.Email);
                
                if (appUser != null)
                {
                    Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync
                        (appUser, login.Password, false, false);

                    if (result.Succeeded)
                    {
                        return Redirect(login.ReturnUrl ?? "/account/edit");
                        //return Ok();
                    }

                }
                TempData["Error"] = "Login failed, wrong credentials or your email is not confirmed";

                //ModelState.AddModelError("", "Login failed, wrong credentials or your email is not confirmed");
            }

            return Redirect("/account/login");
        }

        // /get/account/logout
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Redirect("/account/login");
        }

        // /get/account/edit
        public async Task<IActionResult> Edit()
        {
            AppUser appUser = await _userManager.FindByNameAsync(User.Identity.Name);
            UserEdit user = new UserEdit(appUser);

            return View(user);
        }

        // post account/edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserEdit userEdit)
        {

            AppUser appUser = await _userManager.FindByNameAsync(User.Identity.Name);
            var oldEmail = appUser.Email;
            var oldName = appUser.UserName;
            var fullNameFromModel = userEdit.FirstName.ToLower() + "-" + userEdit.LastName.ToLower();

            if (ModelState.IsValid)
            {
                //email edit
                var isEmailAlreadyExists = _context.Users.Any(x => x.Email == userEdit.Email);
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
                var isNameAlreadyExists = _context.Users.Any(x => x.UserName == fullNameFromModel);
                if(oldName != fullNameFromModel)
                {
                    if (isNameAlreadyExists)
                    {
                        ModelState.AddModelError("LastName", "User with this name already exists");
                        return View(userEdit);
                    }
                    appUser.UserName = fullNameFromModel;
                }
                else { appUser.UserName = oldName; }

                //telephone number edit
                if(userEdit.PhoneNumber != null)
                {
                    appUser.PhoneNumber = userEdit.PhoneNumber;
                }


                //password edit
                if (userEdit.Password != null)
                {
                    appUser.PasswordHash = _passwordHasher.HashPassword(appUser, userEdit.Password);
                }


                if (oldEmail != appUser.Email)
                {

                    appUser.EmailConfirmed = false;
                }

                IdentityResult result = await _userManager.UpdateAsync(appUser);
                if (result.Succeeded)
                {
                    if (oldEmail != appUser.Email) {

                        var token = await _userManager.GenerateEmailConfirmationTokenAsync(appUser);

                        var confirmationLink = Url.Action("ConfirmEmail", "Account",
                                                new { userId = appUser.Id, token = token }, Request.Scheme);

                        _logger.Log(LogLevel.Warning, confirmationLink);

                        await _emailSender.SendEmailAsync(appUser.Email, "Email Confirmation",
                       "Your confirmation link: " + confirmationLink);

                        TempData["Success"] = "We send you an Email with reset password link.";
                    }


                    TempData["Success"] = "Your details have been changed!";
                    return Redirect("/account/edit");
                }
                TempData["Error"] = "An error occures. Please try again.";
            }

            return View();
        }

        // post account/delete

        public async Task<IActionResult> Delete()
        {
            AppUser appUser = await _userManager.FindByNameAsync(User.Identity.Name);

            if (appUser == null)
            {
                TempData["Error"] = "User not found";
                return NotFound();

            }
            else
            {
                IdentityResult result = await _userManager.DeleteAsync(appUser);

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

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                TempData["Error"] = $"The user Id {userId} is invalid";
                return NotFound();
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);

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
                var user = await _userManager.FindByEmailAsync(forgotPassword.Email);
                if (user != null && await _userManager.IsEmailConfirmedAsync(user))
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                    var passwordResetLink = Url.Action("ResetPassword", "Account",
                         new { email = forgotPassword.Email, token = token }, Request.Scheme);

                    _logger.Log(LogLevel.Warning, passwordResetLink);

                    await _emailSender.SendEmailAsync(forgotPassword.Email, "Reset Password",
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
                var user = await _userManager.FindByEmailAsync(resetPassword.Email);

                if (user != null)
                {
                    var result = await _userManager.ResetPasswordAsync(user, resetPassword.Token, resetPassword.Password);

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

        [AllowAnonymous]
        [HttpPost]
        public IActionResult ExternalLogin(string provider, string returnUrl)
        {
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account",
                                                    new { ReturnUrl = returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);

            return new ChallengeResult(provider, properties);
        }

        //[AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            var redirectUrl = returnUrl ?? Url.Content("~/"); //if url is null we intilize it to the root othetwise it is returnUrl

            Login login = new Login
            {
                ReturnUrl = returnUrl,
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };

            if(remoteError != null)//google/fb error
            {
                ModelState.AddModelError(string.Empty, $"Error from external provider: {remoteError}");

                return View("Login", login);
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();

            if(info == null)//no data from provider
            {
                ModelState.AddModelError(string.Empty, $"Error loading external login data.");

                return View("Login", login);
            }

            var signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider,
                info.ProviderKey, isPersistent: false, bypassTwoFactor: true);

            if (signInResult.Succeeded)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);//email from provider

                if (email != null)
                {
                    var user = await _userManager.FindByEmailAsync(email);//check if it is already in AspNetUser table

                    if (user == null)
                    {
                        user = new AppUser
                        {
                            UserName = info.Principal.FindFirstValue(ClaimTypes.Name),
                            Email = info.Principal.FindFirstValue(ClaimTypes.Email),
                        };
                        await _userManager.CreateAsync(user);
                    }

                    await _userManager.AddLoginAsync(user, info);//add row to AspNetUserLogins
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    return LocalRedirect(returnUrl);
                }
            }

            return View("Login", login);

        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> SignInWithFb(string accessToken)
        {
            var validatedToken = await _facebookAuthService.ValidateAccessTokenAsync(accessToken);

            //invalid token
            if (!validatedToken.Data.IsValid)
            {
                TempData["Error"] = "An error occures. Please try again.";
                return Json(new { Text = "An error occures. Please try again." });
            }

            var userInfo = await _facebookAuthService.GetUserInfoAsync(accessToken);
            var user = await _userManager.FindByEmailAsync(userInfo.Email);

            //user is not db we have to register 
            if(user == null)
            {
                await CreateNewUserAndStudentForFbUser(userInfo);
                return RedirectToAction("Edit");
            }

            //user is registered already we just log him in
            await _signInManager.SignInAsync(user, true);
            return RedirectToAction("Edit", "Account");
        }

        private async Task CreateNewUserAndStudentForFbUser(FacebookUserInfoResult userInfo)
        {
            //new student
            await _studentsController.Create(new Student()
            {
                FirstName = userInfo.FirstName,
                LastName = userInfo.LastName,
                //ImageUpload = DownloadImage(userInfo.Picture.Data.Url).Result,
                Email = userInfo.Email,
                PhoneNumber = "000-000-000"
            });
            //new .net user
            var newUser = new AppUser
            {
                UserName = userInfo.FirstName.ToLower() + "-" + userInfo.LastName.ToLower(),
                Email = userInfo.Email,
                RegistrationDate = DateTime.Now,
                IsStudent = true,
                PhoneNumber = "000-000-000"

            };
            var createResult = await _userManager.CreateAsync(newUser);
            if (!createResult.Succeeded)
            {
                TempData["Error"] = "Error: User was't registered. Please try again.";
            }
            await _signInManager.SignInAsync(newUser, false);
            

        }

        // iformfile created is invalid
        private async Task<IFormFile> DownloadImage(Uri fromUrl)
        {
            //using (System.Net.WebClient webClient = new System.Net.WebClient())
            //{
            //    using (Stream stream = webClient.OpenRead(fromUrl))
            //    {
            //        return Image.FromStream
            //    }
            //}
            WebClient wb = new WebClient();
            var byteArr = await wb.DownloadDataTaskAsync(fromUrl);
            var stream = new MemoryStream(byteArr);
            IFormFile file = new FormFile(stream, 0, byteArr.Length, "name", "fileName");
            return file;


        }

        public async Task<JsonResult> GetClasses()
        {
            List<CalendarEventStudent> classes = null;
            try
            {
                var currentUser = await _userManager.GetUserAsync(User);
                var currentStudent = await _context.Students.FirstOrDefaultAsync(x => x.Email == currentUser.Email);
                classes = await _context.CalendarEventStudents
                            .Where(x => x.StudentId == currentStudent.Id)
                            .Include(x => x.CalendarEvent.Course).ToListAsync();
            }
            catch (Exception)
            {
                return Json(new { Text = "Server error!" });
            }

            return Json(classes);
        }
        public async Task<IActionResult> ContactMe()
        {
            return View();
        }
    }


}


