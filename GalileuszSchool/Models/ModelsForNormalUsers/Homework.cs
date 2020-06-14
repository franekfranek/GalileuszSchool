using GalileuszSchool.Models.ModelsForAdminArea;
using GalileuszSchool.Repository;
using Microsoft.AspNetCore.Http;
using ShopCart.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Threading.Tasks;

namespace GalileuszSchool.Models.ModelsForNormalUsers
{
    public class Homework : IEntity
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        public string Slug { get; set; } = null;

        [FileExtension(Extensions = new string[] { "txt" })]
        public string TextContent { get; set; }
        
        [FileExtension(Extensions = new string[] { "txt" })]
        public string SolutionTextContent { get; set; }
        public int TeacherId { get; set; }

        [ForeignKey("TeacherId")]
        public virtual Teacher Teacher { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Submission Date")]
        public DateTime CreationDate { get; set; } = DateTime.Now;

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Submission Date")]
        public DateTime StudentSubmissionDate { get; set; }

        public bool IsDone { get; set; } = false;

        [NotMapped]
        public IFormFile PhotoContent { get; set; }

        [NotMapped]
        public IFormFile PhotoSolution { get; set; }

    }
}
