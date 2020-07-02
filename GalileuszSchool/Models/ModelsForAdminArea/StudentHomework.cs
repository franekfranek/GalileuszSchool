using GalileuszSchool.Models.ModelsForNormalUsers;
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
    public class StudentHomework
    {
        public int StudentId { get; set; }
        public Student Student { get; set; }
        public int HomeworkId { get; set; }
        public Homework Homework { get; set; }

        public bool IsDone { get; set; } = false;

        [FileExtension(Extensions = new string[] { "txt" })]
        public string SolutionTextContent { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Submission Date")]
        public DateTime StudentSubmissionDate { get; set; }

        public string ImageSolution { get; set; }

        [NotMapped]
        public IFormFile PhotoSolution { get; set; }
    }
}
