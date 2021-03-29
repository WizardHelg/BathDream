using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using BathDream.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BathDream.Data;

namespace BathDream.Pages.Account
{
    [AutoValidateAntiforgeryToken]
    public class LoginModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly DBApplicationaContext _db;

        public LoginModel(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            DBApplicationaContext db)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _db = db;
        }

        public string ReturnUrl { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Не указан email")]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Требуется указать пароль")]
            [DataType(DataType.Password)]
            [Display(Name = "Пароль")]
            public string Password { get; set; }

            [Display(Name = "Запомнить?")]
            public bool RememberMe { get; set; }

            public int TempOrderId { get; set; } = 0;
        }

        public void OnGet(string returnUrl = null)
        {
            Input = new InputModel();
            returnUrl ??= Url.Content("~/");
            ReturnUrl = returnUrl;
        }

        public void OnGetFromAddOrder(int id)
        {
            Input = new InputModel()
            {
                TempOrderId = id
            };
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, false);
                if (result.Succeeded)
                {
                    if(Input.TempOrderId > 0)
                    {
                        //получить пользователя
                        User user = await _userManager.FindByNameAsync(Input.Email);
                        await _db.Entry(user).Reference(u => u.Profile).LoadAsync();

                        //получить заказ
                        Order order = _db.Orders.Where(o => o.Id == Input.TempOrderId).FirstOrDefault();

                        //связать их узами внешнего ключа
                        if(order != null)
                        {
                            order.Customer = user.Profile;
                            order.Status = "created";
                            _db.Update(order);
                            _db.SaveChanges();
                        }
                    }


                    if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
                        return LocalRedirect(ReturnUrl);
                    else
                        return RedirectToPage("/Test/Index");
                }
                else
                    ModelState.AddModelError(String.Empty, "Неверный логин и.или пароль");
            }
            return Page();
        }
    }
}
