using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BathDream.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BathDream.Pages.AdminPanel
{
    public class UsersModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        public UsersModel(UserManager<User> userManager) => _userManager = userManager;
        public List<User> Users { get; set; }
        public void OnGet()
        {
            Users = _userManager.Users.ToList();
        }
    }
}
