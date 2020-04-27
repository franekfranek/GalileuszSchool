using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GalileuszSchool.Models
{
    public class UserEdit : AppUser
    {
        [Required]
        [MinLength(4, ErrorMessage = "Minimum lenght is 4")]
        [Display(Name = "Name")]
        public string UserName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [DataType(DataType.Password), MinLength(4, ErrorMessage = "Minimum lenght is 4")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password",
            ErrorMessage = "Password and confrimation password do not match.")]
        public string ConfirmPassword { get; set; }
        public UserEdit(){}

        public UserEdit(AppUser appUser)
        {
            UserName = appUser.UserName;
            Email = appUser.Email;
            Password = appUser.PasswordHash;
        }
    }
}
