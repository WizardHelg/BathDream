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
using Microsoft.AspNetCore.SignalR;

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
            public bool Signed { get; set; } = false;
            public string SignText { get; set; } = "Не подписан";
            public string UserName { get; set; }
            public string UserFamaly { get; set; }
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
            public string OrderAddress { get; set; }
            public string PasportAddress { get; set; }
            public string PasportSerial { get; set; }
            public string PasportNumber { get; set; }
            public string PasportIssued { get; set; }
            public string PasportDate { get; set; }
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
            
            Input.UserName = user.UName;
            Input.UserFamaly = user.UFamaly;
            Input.FIO = $"{user.UFamaly} {user.UName} {user.UPatronymic}";
            Input.CustomerPhone = user.PhoneNumber;
            Input.CustomerEmail = user.Email;

            //Order order = null;
            if (TempData["OrderId"] is int orid
                && await _db.Orders.FirstOrDefaultAsync(o => o.Id == orid) is Order order)
            {
                //int orid = (int)TempData["OrderId"];
                //order = await _db.Orders.FirstOrDefaultAsync(o => o.Id == orid);
                order.Customer = user.Profile;
                order.Status = Order.Statuses.New;
                await _db.SaveChangesAsync();
            }
            
            order = await _db.Orders.Where(o => o.Customer.User.Id == user.Id)
                                    .Include(o => o.Estimate)
                                    .ThenInclude(e => e.Rooms)
                                    .Include(x => x.Estimate)
                                    .ThenInclude(x => x.Works).FirstOrDefaultAsync();

            if(order != null)
            {
                Input.Display = true;
                Input.OrderDate = order.Date.ToShortDateString();
                Input.OrderNumber = order.Id;
                Input.OrderAddress = order.ObjectAdress;
                Input.Rooms = order.Estimate.Rooms;
                Input.Works = order.Estimate.Works.OrderBy(w => w.Position).ToList();
                Input.Total = Input.Works.Sum(w => w.Total);
                Input.Signed = order.Signed;
            }
        }

        public async Task<IActionResult> OnGetContractAsync()
        {
            Input.ContentView = "./Views/CustomerContractPartialView";
            User user = await _userManager.FindByNameAsync(User.Identity.Name);
            await _db.Entry(user).Reference(u => u.Profile).LoadAsync();

            Input.FIO = $"{user.UFamaly} {user.UName} {user.UPatronymic}";
            Input.CustomerPhone = user.PhoneNumber;
            Input.CustomerEmail = user.Email;
            Input.PasportAddress = user.Profile.PasportAddress;
            Input.PasportSerial = user.Profile.PasportSerial;
            Input.PasportNumber = user.Profile.PasportNumber;
            Input.PasportIssued = user.Profile.PasportIssued;
            if (user.Profile.PasportDate is DateTime dt)
                Input.PasportDate = dt.ToShortDateString();

            int order_id = 0;
            if (await _db.Orders.Where(o => o.Customer.User.Id == user.Id).FirstOrDefaultAsync() is Order order)
            {
                Input.SignText = order.Signed ? "Подписан" : "Не подписан";
                Input.OrderDate = order.Date.ToShortDateString();
                Input.OrderNumber = order.Id;
                Input.OrderAddress = order.ObjectAdress;
                Input.Signed = order.Signed;
                order_id = order.Id;
            }
            else
            {
                return RedirectToPage("/Account/Customer");
            }

            if (!Input.Signed && (string.IsNullOrEmpty(Input.OrderAddress) || !user.Profile.IsFilled()))
                return RedirectToPage("/OrderDetails", new { OrderId = order_id });

            return Page();
        }

        public async Task<IActionResult> OnGetSignAsync()
        {
            Input.ContentView = "./Views/CustomerContractPartialView";
            User user = await _userManager.FindByNameAsync(User.Identity.Name);
            Order order = await _db.Orders.Where(o => o.Customer.User.Id == user.Id).FirstOrDefaultAsync();
            await _db.Entry(user).Reference(u => u.Profile).LoadAsync();

            Input.FIO = $"{user.UFamaly} {user.UName} {user.UPatronymic}";
            Input.CustomerPhone = user.PhoneNumber;
            Input.CustomerEmail = user.Email;
            Input.PasportAddress = user.Profile.PasportAddress;
            Input.PasportSerial = user.Profile.PasportSerial;
            Input.PasportNumber = user.Profile.PasportNumber;
            Input.PasportIssued = user.Profile.PasportIssued;
            if (user.Profile.PasportDate is DateTime dt)
                Input.PasportDate = dt.ToShortDateString();

            if (order != null)
            {
                Input.OrderDate = order.Date.ToShortDateString();
                Input.OrderNumber = order.Id;
                Input.OrderAddress = order.ObjectAdress;
                order.Signed = true;
                Input.SignText = "Подписан";
                Input.Signed = order.Signed;
                await _db.SaveChangesAsync();
            }

            return Page();
        }

        public async Task<IActionResult> OnGetDeleteOrderAsync()
        {
            Input.ContentView = "./Views/CustomerEstimatePartialView";
            User user = await _userManager.FindByNameAsync(User.Identity.Name);
            Order order = await _db.Orders.Where(o => o.Customer.User.Id == user.Id).FirstOrDefaultAsync();
            _db.Orders.Remove(order);
            await _db.SaveChangesAsync();
            return RedirectToAction("OnGetAsync");
        }

        public async Task<IActionResult> OnGetBriefAsync()
        {
            User user = await _userManager.FindByNameAsync(User.Identity.Name);
            //Order order = await _db.Orders.Where(o => o.Customer.User.Id == user.Id).FirstOrDefaultAsync();
            if(await _db.Orders.Where(o => o.Customer.User.Id == user.Id).FirstOrDefaultAsync() is Order order
               && order.Signed)
                return RedirectToPage("/Brief", new { id = order.Id });

            return Page();
        }

        public void OnGetChat()
        {
            Input.ContentView = "./Views/ChatPartialView";
        }

        public async Task<IActionResult> OnGetLogoutAsync()
        {
            await _signInManager.SignOutAsync();
            return RedirectToPage("/Index");
        }
    }
}
