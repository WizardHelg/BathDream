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
    [Authorize(Roles = "architect")]
    public class ArchitectorModel : PageModel
    {
        //public List<Order> Orders { get; set; }

        public List<(Order Order, bool IsNewMessages)> Data = new List<(Order Order, bool IsNewMessages)>();

        private readonly DBApplicationaContext _db;
        private readonly UserManager<User> _user_manager;

        public ArchitectorModel(DBApplicationaContext db, UserManager<User> user_manager)
        {
            _db = db;
            _user_manager = user_manager;
        }
        public async Task OnGetAsync()
        {
            var orders = await _db.Orders.Where(o => (o.Status & Order.Statuses.Brief) > 0)
                                   .Include(o => o.Customer)
                                   .ThenInclude(c => c.User).ToListAsync();

            foreach(var order in orders)
            {
                string senderId = order.Customer.User.Id;
                string recipientId = _user_manager.GetUserId(User);
                var is_new_mes = await _db.Messages.AnyAsync(m => (m.Sender.Id == senderId) && (m.Recipient.Id == recipientId) && (m.Order == order ) && !m.IsReaded);

                Data.Add((order, is_new_mes));
            }

            Data.OrderBy(s => s.Order.Status);
        }
        public IActionResult OnGetToExecute(int id)
        {
            if (_db.Orders.FirstOrDefault(o => o.Id == id) is Order order)
            {
                order.Status |= Order.Statuses.ToExecute;
                _db.SaveChanges();
            }

            return RedirectToPage();
        }

    }
}
