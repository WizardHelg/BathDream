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

        public List<Order> ExecutingOrders = new List<Order>();

        public int sw = 1;

        private readonly DBApplicationaContext _db;

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();
        public class InputModel
        {
            public Estimate Estimate { get; set; } = new Estimate();
            public Order Order { get; set; } = new Order();
            public string ContentView { get; set; } = "/Pages/Account/Views/ExecutorEstimatePartialView.cshtml";
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
            await OnGetShowAvailableOrdersAsync();
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

            var executingOrders = await _db.Orders.Where(o => o.Executor.UserId == _userManager.GetUserId(User))
                                    .Include(o => o.Customer)
                                    .ThenInclude(c => c.User).ToListAsync();
            foreach (var order in executingOrders)
            {
                ExecutingOrders.Add(order);
            }
            sw = 1;
            return Page();
        }

        public async Task<IActionResult> OnGetShowAcceptedOrdersAsync()
        {
            var executingOrders = await _db.Orders.Where(o => o.Executor.UserId == _userManager.GetUserId(User))
                                    .Include(o => o.Customer)
                                    .ThenInclude(c => c.User).ToListAsync();
            foreach (var order in executingOrders)
            {
                ExecutingOrders.Add(order);
            }
            sw = 2;
            return Page();
        }

        public IActionResult OnGetExecuting(int id)
        {
            if (_db.Orders.FirstOrDefault(o => o.Id == id) is Order order)
            {
                order.Status = Order.Statuses.Executing;
                var executorprofile = _userManager.GetUserId(User);
                var profile = _db.UserProfiles.FirstOrDefault(u => u.UserId == executorprofile);
                order.Executor = (ExecutorProfile)profile;
                _db.SaveChanges();
            }

            return RedirectToPage();
        }
        
        public IActionResult OnGetShowEstimate(int id)
        {
            
            Input.Estimate = _db.Estimates.FirstOrDefault(e => e.OrderId == id);
            Input.Estimate.Rooms =  _db.Rooms.Where(r => r.EstimateId == Input.Estimate.Id).ToList();
            Input.Estimate.Works = _db.Works.Where(w => w.EstimateId == Input.Estimate.Id).ToList();

            Input.Order = _db.Orders.FirstOrDefault(o => o.Id == id);

            return RedirectToPage(Input);
        }
    }
}
