using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BathDream.Acquiring;
using BathDream.Data;
using BathDream.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BathDream.Pages.Account.Payments
{
    public class ReturnUrlModel : PageModel
    {
        private readonly DBApplicationaContext _db;
        public ReturnUrlModel(DBApplicationaContext db)
        {
            _db = db;
        }
        public async Task<IActionResult> OnGet(string orderId)
        {
            List<Payment> payments = await _db.Payments.Where(p => p.PaymentId == orderId).Include(p => p.Invoice).ToListAsync();

            payments = Invoice.CheckStatus(payments);

            _db.SaveChanges();

            return Page();
        }
    }
}
