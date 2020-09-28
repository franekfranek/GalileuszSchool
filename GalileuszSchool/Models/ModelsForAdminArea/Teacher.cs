using GalileuszSchool.Models.ModelsForAdminArea;
using GalileuszSchool.Models.ModelsForNormalUsers;
using GalileuszSchool.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GalileuszSchool.Models.ModelsForAdminArea
{
    public class Teacher : IListItem, IEntity
    {
        public int Id { get; set; }

        [Required, MinLength(2, ErrorMessage = "Minimal length is 2")]
        public string FirstName { get; set; }

        [Required, MinLength(2, ErrorMessage = "Minimal length is 2")]
        public string LastName { get; set; }
        public string Slug { get; set; }

        [Required(ErrorMessage = "You must provide a phone number.")]
        [Display(Name = "Phone Number")]
        [RegularExpression(@"^(\([0-9]{3}\)|[0-9]{3}-)[0-9]{3}-[0-9]{3}$", ErrorMessage = "Pattern is 000-000-000")]
        public string PhoneNumber { get; set; }

        [EmailAddress]
        //[FileExtensions]
        public string Email { get; set; }

        public virtual ICollection<Homework> Homeworks { get; set; }



    }
}
