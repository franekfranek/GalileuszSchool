using GalileuszSchool.Models.ModelsForNormalUsers;
using GalileuszSchool.Models.ModelsForNormalUsers.Calendar;
using GalileuszSchool.Repository;
using Microsoft.AspNetCore.Http;
using ShopCart.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GalileuszSchool.Models.ModelsForAdminArea
{
    public class Student : IListItem, IEntity
    {
        public int Id { get; set; }

        [Display(Name = "Student image")]
        
        public string Image { get; set; }

        [Required, MinLength(2, ErrorMessage = "Minimal length is 2")]
        public string FirstName { get; set; }

        [Required, MinLength(2, ErrorMessage = "Minimal length is 2")]
        public string LastName { get; set; }

        public string Slug { get; set; }

        //[Required(ErrorMessage = "You must provide a phone number.")]
        [Display(Name = "Phone Number")]
        [RegularExpression(@"^(\([0-9]{3}\)|[0-9]{3}-)[0-9]{3}-[0-9]{3}$", ErrorMessage = "Pattern is 000-000-000")]
        public string PhoneNumber { get; set; }
        [EmailAddress]
        public string Email { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Enrollment Date")]
        public DateTime EnrollmentDate { get; set; }

        [NotMapped]
        [FileExtension(Extensions = new string[] { "jpg", "png" })]
        public IFormFile ImageUpload { get; set; }

        public IList<StudentHomework> StudentHomeworks { get; set; }
        public IList<CalendarEventStudent> CalendarEventStudents { get; set; }


    }
}
