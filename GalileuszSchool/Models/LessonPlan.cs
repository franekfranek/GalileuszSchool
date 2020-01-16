using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GalileuszSchool.Models
{

    public enum Days
    {
        Monday,
        Tuesday,
        Wednesday,
        Thursday,
        Friday,
        Saturday,
        Sunday


                  
    }
    public class LessonPlan
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "--")]
        public int classroom { get; set; }

        [Required(ErrorMessage = "--")]
        public Days day { get; set; }

        //todo: remove dayId and value from html dayId will be passed to field 'day'. In result day alwaeys will be Monday
        public int dayId { get; set; }

        [Required(ErrorMessage = "--")]
        public int startTime { get; set; }

        [Required(ErrorMessage = "--")]
        public DateTime stopTime { get; set; }

        public int CourseId { get; set; }

        [ForeignKey("CourseId")]
        public virtual Course Course { get; set; }



    }
}
