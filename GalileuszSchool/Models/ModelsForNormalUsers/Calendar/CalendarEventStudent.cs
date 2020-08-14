using GalileuszSchool.Models.ModelsForAdminArea;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GalileuszSchool.Models.ModelsForNormalUsers.Calendar
{
    public class CalendarEventStudent
    {
        public int CalendarEventId { get; set; }
        public CalendarEvent CalendarEvent { get; set; }

        public int StudentId { get; set; }
        public Student Student { get; set; }

        public bool IsPaid { get; set; } 
        public bool IsPresent { get; set; }
    }
}
