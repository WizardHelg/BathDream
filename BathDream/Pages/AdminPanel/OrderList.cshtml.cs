using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BathDream.Data;
using BathDream.Models;
using BathDream.Pages.Account;
using jsreport.AspNetCore;
using jsreport.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BathDream.Pages.AdminPanel
{
    [Authorize(Roles = "admin")]
    public class OrderListModel : PageModel
    {
        private readonly UserManager<User> _user_manager;
        private readonly DBApplicationaContext _db;
        public OrderListModel(UserManager<User> userManager, DBApplicationaContext db)
        {
            _user_manager = userManager;
            _db = db;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();
        public class InputModel
        {
            public UserProfile UserProfile { get; set; }
            public List<Order> Orders { get; set; }
            public List<Work> Works { get; set; }
            public User User { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(string userId = "")
        {
            if (userId == "")
            {
                Input.Orders = await _db.Orders.Include(o => o.Customer).Include(o => o.Estimate).ThenInclude(e => e.Works).ToListAsync();
                return Page();
            }

            Input.UserProfile = await _db.UserProfiles.Where(u => u.UserId == userId)
                                         .Include(u => u.User)
                                         .Include(u => u.Orders)
                                            .ThenInclude(o => o.Estimate)
                                                .ThenInclude(e => e.Works)
                                         .FirstOrDefaultAsync(u => u.UserId == userId);

            Input.Orders = Input.UserProfile.Orders;
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

        [MiddlewareFilter(typeof(JsReportPipeline))]
        public IActionResult OnGetContract()
        {
            //HttpContext.JsReportFeature().Recipe(Recipe.ChromePdf);

            return Page();
            //return new PartialViewResult
            //{
            //    ViewName = "./Account/Views/CustomerContractPartialView",
            //    ViewData = new Microsoft.AspNetCore.Mvc.ViewFeatures.ViewDataDictionary<InputModel>(ViewData, )
            //};
        }
    }
}
