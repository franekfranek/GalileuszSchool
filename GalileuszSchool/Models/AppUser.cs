using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GalileuszSchool.Models
{
    public class AppUser : IdentityUser
    {
        public bool IsStudent { get; set; } = false;
    }
}
