using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BathDream.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BathDream.Pages.Test
{
    [Authorize]
    public class IndexModel : PageModel
    {
        //private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();
        public class InputModel
        {
            public string NameFamaly { get; set; }
            public string About { get; set; }
        }

        public IndexModel(/*SignInManager<User> signInManager,*/ UserManager<User> userManager)
        {
            //_signInManager = signInManager;
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            User user = await _userManager.FindByNameAsync(User.Identity.Name);
            var roles = await _userManager.GetRolesAsync(user);
            if(roles.Count > 0)
                switch (roles[0])
                {
                    case "executor":
                        return RedirectToPage("/Account/Executor");
                    case "customer":
                        return RedirectToPage("/Account/Customer");
                }

            return RedirectToPage("/Index");
        }
    }
}
