using GalileuszSchool.Models.ModelsForNormalUsers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GalileuszSchool.Infrastructure
{
    [HtmlTargetElement("td", Attributes = "user-role")] //it targets html element
    public class RolesTagHelper : TagHelper
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<AppUser> userManager;

        public RolesTagHelper(RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;

        }
        [HtmlAttributeName("user-role")]
        public string RoleId { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            //List<string> names = new List<string>();
            //IdentityRole role = await roleManager.FindByIdAsync(RoleId);

            //if (role != null)
            //{
            //    foreach (var user in userManager.Users)
            //    {
            //        if(user != null && await userManager.IsInRoleAsync(user, role.Name))
            //        {
            //            names.Add(user.UserName);
            //        }
            //    }
            //}
            //output.Content.SetContent(names.Count == 0 ? "No users" : string.Join(", ", names));
        }
    }
}
