using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BathDream.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BathDream.Data;
using Microsoft.EntityFrameworkCore;

namespace BathDream.Pages.AdminPanel
{
    public class WorkListModel : PageModel
    {
        readonly DBApplicationaContext _db;

        public WorkListModel(DBApplicationaContext db) => _db = db;

        [BindProperty]
        public InputModel Input { get; set; }
        public class InputModel
        {
            public List<WorkPrice> WorkPrices { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            Input = new();
            Input.WorkPrices = await _db.WorkPrices.ToListAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            if(_db.WorkPrices
                .Where(wp => wp.Id == id)
                .FirstOrDefault()
                is WorkPrice work)
            {
                _db.WorkPrices.Remove(work);
                await _db.SaveChangesAsync();
            }

            return RedirectToPage();
        }
    }
}
