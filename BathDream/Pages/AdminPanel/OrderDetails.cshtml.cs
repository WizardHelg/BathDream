using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BathDream.Data;
using BathDream.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BathDream.Pages.AdminPanel
{
    public class OrderDetailsModel : PageModel
    {
        private readonly UserManager<User> _user_manager;
        private readonly DBApplicationaContext _db;
        public OrderDetailsModel(UserManager<User> userManager, DBApplicationaContext db)
        {
            _user_manager = userManager;
            _db = db;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();
        public class InputModel
        {
            public UserProfile UserProfile { get; set; }
            public Order Order { get; set; }
            public List<Order> Orders { get; set; }
            public List<Work> Works { get; set; }
            public string ContentView { get; set; }
        }

        public void OnGet()
        {
            Input.ContentView = "./AdminPanel/Views/_OrderByUserPartialView";
        }

        public async Task<IActionResult> OnGetByUserAsync(string userId = "")
        {
            Input.UserProfile = await _db.UserProfiles
                                         .Where(u => u.UserId == userId)
                                         .Include(u => u.User)
                                         .Include(u => u.Orders)
                                            .ThenInclude(o => o.Estimate)
                                                .ThenInclude(e => e.Works)
                                         .Include(u => u.Orders)
                                            .ThenInclude(o => o.Estimate)
                                                .ThenInclude(e => e.Rooms)
                                         .Include(u => u.Orders)
                                            .ThenInclude(o => o.Invoices)
                                                .ThenInclude(i => i.AdditionalWorks)
                                         .Include(u => u.Orders)
                                            .ThenInclude(o => o.Invoices)
                                                .ThenInclude(o => o.Materials)
                                         .FirstOrDefaultAsync(u => u.UserId == userId);

            Input.Orders = Input.UserProfile.Orders;

            Input.ContentView = "./AdminPanel/Views/_OrderByUserPartialView";

            return Page();
        }

        public async Task<IActionResult> OnGetByOrderAsync(int orderId = 0)
        {
            Input.Order = await _db.Orders
                                   .Where(o => o.Id == orderId)
                                   .Include(o => o.Estimate)
                                       .ThenInclude(e => e.Works)
                                   .Include(o => o.Estimate)
                                       .ThenInclude(e => e.Rooms)
                                   .Include(o => o.Invoices)
                                       .ThenInclude(o => o.Materials)
                                   .Include(o => o.Invoices)
                                       .ThenInclude(o => o.AdditionalWorks)
                                   .FirstOrDefaultAsync(o => o.Id == orderId);

            Input.ContentView = "./AdminPanel/Views/_OrderByIdPartialView";

            return Page();
        }
    }
}
