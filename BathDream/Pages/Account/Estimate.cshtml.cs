using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BathDream.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static BathDream.Pages.Account.ExecutorModel;

namespace BathDream.Pages.Account
{
    public class EstimateModel : PageModel
    {
        private readonly DBApplicationaContext _db;
        public InputModel Input { get; set; } = new InputModel();

        public EstimateModel(DBApplicationaContext db)
        {
            _db = db;
        }

        public IActionResult OnGet(int id)
        {
            Input.Estimate = _db.Estimates.FirstOrDefault(e => e.OrderId == id);
            Input.Estimate.Rooms = _db.Rooms.Where(r => r.EstimateId == Input.Estimate.Id).ToList();
            Input.Estimate.Works = _db.Works.Where(w => w.EstimateId == Input.Estimate.Id).ToList();

            Input.Order = _db.Orders.FirstOrDefault(o => o.Id == id);
            return Page();
        }
    }
}
