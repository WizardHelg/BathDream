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

namespace BathDream.Pages.AdminPanel
{
    [Authorize(Roles = "admin")]
    public class UserListModel : PageModel
    {
        private readonly UserManager<User> _user_manager;
        private readonly DBApplicationaContext _db;
        public UserListModel(UserManager<User> userManager, DBApplicationaContext db)
        {
            _user_manager = userManager;
            _db = db;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();
        public class InputModel
        {
            public List<UserAndRole> Users { get; set; }
            public List<string> Roles { get; set; }
            public List<Order> Orders { get; set; }
            public User User { get; set; }
        }

        public class UserAndRole
        {
            public User User { get; set; }
            public string Role { get; set; }
        }
        public async Task<IActionResult> OnGet()
        {
            var users = await _user_manager.Users
                .Include(u => u.Profile)
                .ThenInclude(u => u.Orders)
                .ToListAsync();

            Input.Users = new List<UserAndRole>();
            foreach (var user in users)
            {
                var role = await _user_manager.GetRolesAsync(user);
                Input.Users.Add(new UserAndRole
                {
                    User = user,
                    Role = role[0]
                });
            }
           
            return Page();
        }

        public IActionResult OnGetOrdersByUser(string userId)
        { 
            return RedirectToPage("OrderList", 
                new
            {
                userId = userId
            });
        }

    }
}
