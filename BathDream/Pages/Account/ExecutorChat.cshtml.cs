using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BathDream.Data;
using BathDream.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BathDream.Pages.Account
{
    public class ExecutorChatModel : PageModel
    {
        private readonly DBApplicationaContext _db;
        private readonly UserManager<User> _user_manager;
        [BindProperty]
        public string UserId { get; set; }
        public string OrderId { get; set; }
        public ExecutorChatModel(DBApplicationaContext db, UserManager<User> user_manager)
        {
            _db = db;
            _user_manager = user_manager;
        }
        public async Task OnGet(string userId, string orderId)
        {
            string id = _user_manager.GetUserId(User);
            UserProfile userProfile = await _db.UserProfiles.FirstOrDefaultAsync(u => u.UserId == id);
            userProfile.CurrentOrderId = Convert.ToInt32(orderId);
            await _db.SaveChangesAsync();

            UserId = userId;
            OrderId = orderId;
        }
    }
}
