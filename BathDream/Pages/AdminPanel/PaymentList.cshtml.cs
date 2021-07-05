using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BathDream.Data;
using BathDream.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BathDream.Pages.AdminPanel
{
    [Authorize(Roles = "admin")]
    public class PaymentListModel : PageModel
    {
        private readonly DBApplicationaContext _db;
        public PaymentListModel(DBApplicationaContext db)
        {
            _db = db;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();
        public class InputModel
        {
            public List<Payment> Payments { get; set; }
        }

        public async Task<IActionResult> OnGet()
        {
            Input.Payments = await _db.Payments.Include(p => p.Invoice).ToListAsync();
            Input.Payments = Invoice.CheckStatus(Input.Payments);
            return Page();
        }

    }
}
