using GalileuszSchool.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GalileuszSchool.Areas.Admin.ViewModels
{
    public class LessonPlanViewModel
    {

        public int Id { get; set; }

        [Required(ErrorMessage = "--")]
        public int classroom { get; set; }

        [Required(ErrorMessage = "--")]

        public int startTime { get; set; }

        [Required(ErrorMessage = "--")]
        public int stopTime { get; set; }

        //[Required(ErrorMessage = "--")]
        //[ForeignKey("CourseId")]
        //public virtual Course Course { get; set; }
        public virtual int course { get; set; }

        public int dayId { set; get; }
        public IEnumerable<SelectListItem> day { get; set; }
    }
}
