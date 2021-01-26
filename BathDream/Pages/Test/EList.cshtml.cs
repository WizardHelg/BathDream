using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BathDream.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BathDream.Pages.Test
{
    public class EListModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        public EListModel(UserManager<User> userManager) => _userManager = userManager;
        public List<User> Users { get; set; }
        public async Task OnGetAsync()
        {
            var users = await _userManager.GetUsersInRoleAsync("executor");
            Users = users.ToList();
        }

        public IActionResult OnPostProfile(string id)
        {
            return RedirectToPage("/Account/Views/ViewExecutorProfile", new { Id = id });
        }
    }
}
