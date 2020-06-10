using GalileuszSchool.Models.ModelsForAdminArea;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Threading.Tasks;

namespace GalileuszSchool.Models.ModelsForNormalUsers
{
    public class Homework
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public string TextContent { get; set; }
        public int TeacherId { get; set; }

        [ForeignKey("TeacherId")]
        public virtual Teacher Teacher { get; set; }

        [NotMapped]
        public IFormFile PhotoContent { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Submission Date")]
        public DateTime SubmissionDate { get; set; } = DateTime.Now;

        public bool IsDone { get; set; } = false;
    }
}
