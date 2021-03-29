using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BathDream.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using BathDream.Data;

namespace BathDream.Pages.Account
{
    public class ConfirmEmailModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly DBApplicationaContext _db;
        public ConfirmEmailModel(UserManager<User> userManager, SignInManager<User> signInManager, DBApplicationaContext db)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _db = db;
        }

        [TempData]
        public string StatusMessage { get; set; }


        public async Task<IActionResult> OnGetAsync(string userId, string code, int tempOrderId)
        {
            if (userId == null || code == null)
            {
                return RedirectToPage("/Test/Index");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"���������� ��������� ������������ � ��������������� '{userId}'.");
            }

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ConfirmEmailAsync(user, code);

            if(result.Succeeded)
            {
                //���� tempOrderId > 0 �� ��������� ����� � ������
                if (tempOrderId > 0)
                {
                    await _db.Entry(user).Reference(u => u.Profile).LoadAsync();
                    Order order = _db.Orders.Where(o => o.Id == tempOrderId).FirstOrDefault();
                    if (order != null)
                    {
                        order.Customer = user.Profile;
                        order.Status = "created";
                        _db.Update(order);
                        await _db.SaveChangesAsync();
                    }
                }

                //��������� � "������� ����"
                await _signInManager.SignInAsync(user, true);

                //�������� ����.
                var user_roles = await _userManager.GetRolesAsync(user);

                //���������� �� �������� ������������?
                switch (user_roles.FirstOrDefault())
                {
                    case "executor":
                        return RedirectToPage("/Account/Executor");
                    case "customer":
                        return RedirectToPage("/Account/Customer");
                    default:
                        StatusMessage = "������� �� ������������� Email";
                        return Page();
                }
            }
            else
            {
                StatusMessage = "������ ������������� �����";
            }
            
            //StatusMessage = result.Succeeded ? "������� �� ������������� Email" : "������ ������������� �����";
            return Page();
        }
    }
}
