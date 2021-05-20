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
    //Authorize(Roles = "architector")]
    public class ArchitectorModel : PageModel
    {
        //public List<Order> Orders { get; set; }

        public List<(Order Order, bool IsNewMessages)> Data = new List<(Order Order, bool IsNewMessages)>();

        private readonly DBApplicationaContext _db;

        public ArchitectorModel(DBApplicationaContext db)
        {
            _db = db;
        }
        public async Task OnGetAsync()
        {
            var orders = await _db.Orders.Where(o => (o.Status & Order.Statuses.Brief) > 0)
                                   .Include(o => o.Customer)
                                   .ThenInclude(c => c.User).ToListAsync();

            foreach(var order in orders)
            {
                string id = order.Customer.User.Id;
                var is_new_mes = _db.Messages.Any(m => m.Sender.Id == id && !m.IsReaded);

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
