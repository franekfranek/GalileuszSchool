using GalileuszSchool.Models.ModelsForAdminArea;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GalileuszSchool.Models.ModelsForNormalUsers.Calendar
{
    public class CalendarEvent
    {
        public int? Id { get; set; }
        public string Title { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Description { get; set; }
        public string BorderColor { get; set; }
        public string Color { get; set; }


        public int CourseId { get; set; }
        [ForeignKey("CourseId")]
        public virtual Course Course { get; set; }

        public IList<CalendarEventStudent> CalendarEventStudents { get; set; }
    }
}
