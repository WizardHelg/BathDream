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
    public class ArchitectorModel : PageModel
    {
        public List<Order> Orders { get; set; }

        private readonly DBApplicationaContext _db;

        public ArchitectorModel(DBApplicationaContext db)
        {
            _db = db;
        }
        public void OnGet()
        {

            Orders = _db.Orders.Where(o => o.Status == Order.Statuses.New)
                .Include(o => o.Customer)
                .ThenInclude(c => c.User).ToList();
        }


    }
}
