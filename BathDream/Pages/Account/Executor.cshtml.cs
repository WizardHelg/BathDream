using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BathDream.Data;
using BathDream.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BathDream.Pages.Account
{
    [Authorize(Roles = "executor")]
    public class ExecutorModel : PageModel
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;

        public List<Order> Data = new List<Order>();
        public int sw = 0;

        private readonly DBApplicationaContext _db;

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();
        public class InputModel
        {
            public string NameFamaly { get; set; }
        }

        public ExecutorModel(SignInManager<User> signInManager, UserManager<User> userManager, DBApplicationaContext db)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _db = db;
        }

        public async Task OnGetAsync()
        {
            User user = await _userManager.FindByNameAsync(User.Identity.Name);
            Input.NameFamaly = $"{user.UName} {user.UFamaly}";
        }

        public async Task<IActionResult> OnGetLogoutAsync()
        {
            await _signInManager.SignOutAsync();
            return RedirectToPage("/Index");
        }

        public async Task<IActionResult> OnGetShowAvailableOrdersAsync()
        {
            var orders = await _db.Orders.Where(o => (o.Status == Order.Statuses.New || o.Status == Order.Statuses.ToExecute || o.Status == (Order.Statuses.New | Order.Statuses.ToExecute)))
                                   .Include(o => o.Customer)
                                   .ThenInclude(c => c.User).ToListAsync();

            foreach (var order in orders)
            {
                Data.Add(order);
            }
            sw = 1;
            return Page();
        }

        public IActionResult OnGetShowAddress()
        {
            return Page();
        }
    }
}
