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
    public class IndexModel : PageModel
    {
        private readonly SignInManager<User> _signInManager;

        public IndexModel(SignInManager<User> signInManager) => _signInManager = signInManager;

        public void OnGet()
        {
            
        }

        public async Task<IActionResult> OnPostLogout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToPage("/Test/Index");
        }
    }
}
