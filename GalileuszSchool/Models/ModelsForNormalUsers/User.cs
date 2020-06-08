using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GalileuszSchool.Models.ModelsForNormalUsers
{
    public class User
    {
        [Required, MinLength(2, ErrorMessage = "Minimum lenght is 2")]
        [Display(Name = "First name")]
        public string FirstName { get; set; }

        [Required, MinLength(2, ErrorMessage = "Minimum lenght is 2")]
        [Display(Name = "Last name")]
        public string LastName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [DataType(DataType.Password), Required, MinLength(4, ErrorMessage = "Minimum lenght is 4")]
        public string Password { get; set; }
        public bool IsStudent { get; set; } = false;
        public bool IsTeacher { get; set; } = false;

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Registration Date")]
        public DateTime RegistrationDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "You must provide a phone number.")]
        [Display(Name = "Phone Number")]
        [RegularExpression(@"^(\([0-9]{3}\)|[0-9]{3}-)[0-9]{3}-[0-9]{3}$", ErrorMessage = "Pattern is 000-000-000")]
        public string PhoneNumber { get; set; } = "000-000-000";


    }
}
