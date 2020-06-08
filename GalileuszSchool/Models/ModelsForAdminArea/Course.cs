using GalileuszSchool.Models.ModelsForAdminArea;
using GalileuszSchool.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GalileuszSchool.Models.ModelsForAdminArea
{
    public class Course : IEntity
    {
        public int Id { get; set; }
        [Required, MinLength(2, ErrorMessage = "Minimal length is 2")]
        public string Name { get; set; }
        [Required]
        public string Level { get; set; }

        [MinLength(2, ErrorMessage = "Minimal length is 2")]
        public string Description { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Price per hour")]
        public decimal Price { get; set; }
        public string Slug { get; set; }
        public int Sorting { get; set; }

        public int TeacherId { get; set; }

        [ForeignKey("TeacherId")]
        public virtual Teacher Teacher { get; set; }

        
        //public DateTime StartTime { get; set; }
        //public DateTime EndTime { get; set; }
    }
}
