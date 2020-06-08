using GalileuszSchool.Models;
using GalileuszSchool.Models.ModelsForAdminArea;
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
        public List<List<List<LessonPlan>>> LessonsList { get; set; }
    }
}
