using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GalileuszSchool.Models
{
    public class User
    {
        public string UserName { get; set; }

        public string Email { get; set; }
        public string Password { get; set; }

        public User(){}

        public User(AppUser appUser)
        {
            UserName = appUser.UserName;
            Email = appUser.Email;
            Password = appUser.PasswordHash;
        }
    }
}
