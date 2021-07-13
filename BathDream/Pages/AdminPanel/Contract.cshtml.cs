using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BathDream.Data;
using BathDream.Interfaces;
using BathDream.Models;
using BathDream.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SelectPdf;

namespace BathDream.Pages.AdminPanel
{
    public class ContractModel : PageModel
    {
        private readonly DBApplicationaContext _db;
        private readonly PDFConverter _PDFConverter;
        private readonly UserManager<User> _userManager;

        public ContractModel(DBApplicationaContext db, PDFConverter PDFConverter, UserManager<User> userManager)
        {
            _db = db;
            _PDFConverter = PDFConverter;
            _userManager = userManager;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();
        public class InputModel : IContract
        {
            public User User { get; set; }
            public Order Order { get; set; }
            public List<Work> UniqueWorks { get; set; }
        }
        public async Task<IActionResult> OnGetAsync(int orderId)
        {
            if (await _db.Orders.Where(o => o.Id == orderId)
                                .Include(o => o.Estimate)
                                    .ThenInclude(e => e.Works)
                                    .ThenInclude(w => w.WorkType)
                                .Include(o => o.Customer)
                                .FirstOrDefaultAsync() is Order order)
            {
                Input.Order = order;

                UserProfile userProfile = await _db.UserProfiles.FindAsync(order.Customer.Id);

                User user = await _userManager.FindByIdAsync(userProfile.UserId);
                await _db.Entry(user).Reference(u => u.Profile).LoadAsync();

                Input.UniqueWorks = order.Estimate.UniqueWorks();

                Input.User = user;
            }

            return Page();

        }
    }
}
