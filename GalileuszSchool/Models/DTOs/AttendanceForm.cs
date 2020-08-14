using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GalileuszSchool.Models.DTOs
{
    public class AttendanceForm
    {
        public int EventId { get; set; }
        public int StudentId { get; set; }
        public bool IsPresent { get; set; }

    }
}
