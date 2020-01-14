using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GalileuszSchool.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        public string Index()
        {
            return "lool";
        }
    }
}