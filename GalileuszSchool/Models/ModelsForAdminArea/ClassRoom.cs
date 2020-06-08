using GalileuszSchool.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GalileuszSchool.Models.ModelsForAdminArea
{
    public class ClassRoom : IEntity
    {
        public int Id {set; get;}
        public int ClassRoomNumber { get; set; }
        public string ClassRoomName { get; set; }
        public int ClassRoomCapacity { get; set; }
        public string Slug { get; set; } = null;
    }
}
