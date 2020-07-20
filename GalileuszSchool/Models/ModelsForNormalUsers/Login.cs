using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GalileuszSchool.Models.ModelsForNormalUsers
{
    public class Login
    {
       
        [Required, EmailAddress]
        public string Email { get; set; }

        [DataType(DataType.Password), Required, MinLength(4, ErrorMessage = "Minimum lenght is 4")]
        public string Password { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }


        public Login() { }

        public Login(AppUser appUser)
        {
            Email = appUser.Email;
            Password = appUser.PasswordHash;
        }
    }
}
