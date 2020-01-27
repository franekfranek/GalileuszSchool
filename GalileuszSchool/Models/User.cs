using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GalileuszSchool.Models
{
    public class User
    {
        [Required, MinLength(2, ErrorMessage = "Minimum lenght is 2")]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [DataType(DataType.Password), Required, MinLength(4, ErrorMessage = "Minimum lenght is 4")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password",
            ErrorMessage = "Password and confrimation password do not match.")]
        public string ConfirmPassword { get; set; }

    }
}
