using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BathDream.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BathDream.Data;
using Microsoft.EntityFrameworkCore;

namespace BathDream.Pages.Account
{
    [Authorize(Roles = "customer")]
    public class CustomerModel : PageModel
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly DBApplicationaContext _db;

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();
        public class InputModel
        {
            public string NameFamaly { get; set; }
            public string FIO { get; set; }
            public bool Display { get; set; } = false;
            public string CustomerPhone { get; set; }
            public string CustomerEmail { get; set; }
            public int OrderNumber { get; set; }
            public string OrderDate { get; set; }
            public List<Room> Rooms { get; set; }
            public List<Work> Works { get; set; }
            public double Total { get; set; }
            public string ContentView { get; set; }
        }

        public CustomerModel(SignInManager<User> signInManager, UserManager<User> userManager, DBApplicationaContext db)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _db = db;
        }

        public async Task OnGetAsync()
        {
            Input.ContentView = "./Views/CustomerEstimatePartialView";
            User user = await _userManager.FindByNameAsync(User.Identity.Name);
            await _db.Entry(user).Reference(u => u.Profile).LoadAsync();
            
            Input.NameFamaly = $"{user.UName} {user.UFamaly}";
            Input.FIO = $"{user.UFamaly} {user.UName} {user.UPatronymic}";
            Input.CustomerPhone = user.PhoneNumber;
            Input.CustomerEmail = user.Email;

            Order order = _db.Orders.Where(o => o.Customer.User.Id == user.Id)
                            .Include(o => o.Estimate)
                            .ThenInclude(e => e.Rooms)
                            .Include(x => x.Estimate)
                            .ThenInclude(x => x.Works).FirstOrDefault();

            if(order != null)
            {
                Input.Display = true;
                Input.OrderDate = order.Date.ToShortDateString();
                Input.OrderNumber = order.Id;
                Input.Rooms = order.Estimate.Rooms;
                Input.Works = order.Estimate.Works;
                Input.Total = Input.Works.Sum(w => w.Total);
            }
        }

        public async Task<IActionResult> OnGetContract()
        {
            Input.ContentView = "./Views/CustomerContractPartialView";
            User user = await _userManager.FindByNameAsync(User.Identity.Name);
            //await _db.Entry(user).Reference(u => u.Profile).LoadAsync();

            Input.NameFamaly = $"{user.UName} {user.UFamaly}";
            Input.FIO = $"{user.UFamaly} {user.UName} {user.UPatronymic}";
            return Page();
        }

        public async Task OnGetFromConfirmAsync(int tempOrderId)
        {
            User user = await _userManager.FindByNameAsync(User.Identity.Name);
            await _db.Entry(user).Reference(u => u.Profile).LoadAsync();

            Input.NameFamaly = $"{user.UName} {user.UFamaly}";
            Input.CustomerPhone = user.PhoneNumber;
            Input.CustomerEmail = user.Email;

            Order order = _db.Orders.Where(o => o.Id == tempOrderId)
                            .Include(o => o.Estimate)
                            .ThenInclude(e => e.Rooms)
                            .Include(x => x.Estimate)
                            .ThenInclude(x => x.Works).FirstOrDefault();

            if (order != null)
            {
                Input.Display = true;
                Input.OrderDate = order.Date.ToShortDateString();
                Input.OrderNumber = order.Id;
                Input.Rooms = order.Estimate.Rooms;
                Input.Works = order.Estimate.Works;
                Input.Total = Input.Works.Sum(w => w.Total);
            }
        } 

        public async Task<IActionResult> OnGetLogoutAsync()
        {
            await _signInManager.SignOutAsync();
            return RedirectToPage("/Index");
        }
    }
}
