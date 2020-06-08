using GalileuszSchool.Models.ModelsForNormalUsers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GalileuszSchool.Models.ModelsForNormalUsers
{
    public class UserEdit : AppUser
    {
        [Required, MinLength(2, ErrorMessage = "Minimum lenght is 2")]
        [Display(Name = "First name")]
        public string FirstName { get; set; }

        [Required, MinLength(2, ErrorMessage = "Minimum lenght is 2")]
        [Display(Name = "Last name")]
        public string LastName { get; set; }

        [Required, EmailAddress]
        public override string Email { get; set; }

        [DataType(DataType.Password), MinLength(4, ErrorMessage = "Minimum lenght is 4")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password",
            ErrorMessage = "Password and confrimation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "You must provide a phone number.")]
        [Display(Name = "Phone Number")]
        [RegularExpression(@"^(\([0-9]{3}\)|[0-9]{3}-)[0-9]{3}-[0-9]{3}$", ErrorMessage = "Pattern is 000-000-000")]
        public override string PhoneNumber { get; set; }
        public UserEdit(){}

        public UserEdit(AppUser appUser)
        {
            var splitUserName = appUser.UserName.Split('-');
            FirstName = splitUserName[0];
            LastName = splitUserName[1];
            Email = appUser.Email;
            PhoneNumber = appUser.PhoneNumber;
            Password = appUser.PasswordHash;
        }
    }
}
