using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BathDream.Data;
using BathDream.Models;
using BathDream.Pages.Account;
using BathDream.Services;
using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SelectPdf;

namespace BathDream.Pages.AdminPanel
{
    [Authorize(Roles = "admin")]
    public class OrderListModel : PageModel
    {
        private readonly UserManager<User> _user_manager;
        private readonly DBApplicationaContext _db;
        private readonly PDFConverter _PDFConverter;

        //private readonly IConverter _converter;


        public OrderListModel(UserManager<User> userManager, DBApplicationaContext db, PDFConverter PDFConverter)
        {
            _user_manager = userManager;
            _db = db;
            _PDFConverter = PDFConverter;
            //_converter = converter;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();
        public class InputModel
        {
            public UserProfile UserProfile { get; set; }
            public List<Order> Orders { get; set; }
            public List<Work> Works { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(string userId = "")
        {
            if (userId == "")
            {
                Input.Orders = await _db.Orders.Include(o => o.Customer).Include(o => o.Estimate).ThenInclude(e => e.Works).ToListAsync();

                Input.Orders = Input.Orders.OrderByDescending(o => o.Id).ToList();

                return Page();
            }

            Input.UserProfile = await _db.UserProfiles.Where(u => u.UserId == userId)
                                         .Include(u => u.User)
                                         .Include(u => u.Orders)
                                            .ThenInclude(o => o.Estimate)
                                                .ThenInclude(e => e.Works)
                                         .FirstOrDefaultAsync(u => u.UserId == userId);

            Input.Orders = Input.UserProfile.Orders.OrderByDescending(o => o.Id).ToList();

            return Page();
        }

        public IActionResult OnGetSeeAll(string userId)
        {
            return RedirectToPage("OrderDetails", "ByUser", new
            {
                userId = userId
            });
        }
        
        public IActionResult OnGetDetails(int orderId)
        {
            return RedirectToPage("OrderDetails", "ByOrder", new
            {
                orderId = orderId
            });
        }

        public IActionResult OnGetContract(int orderId)
        {
            string url = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}/AdminPanel/Contract?orderId={orderId}";

            byte[] file = _PDFConverter.Convert(url, $"������� �{orderId}");

            return File(file, "application/pdf", $"������� �{orderId}.pdf");

        }
    }
}
