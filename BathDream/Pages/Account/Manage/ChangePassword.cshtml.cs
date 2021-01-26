using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using BathDream.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BathDream.Pages.Account.Manage
{
    public class ChangePasswordModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private string _returnUrl;

        public ChangePasswordModel(UserManager<User> userManager) => _userManager = userManager;

        [BindProperty]
        public InputModel Input { get; set; }
        public class InputModel
        {
            public string Id { get; set; }
            public string ReturnUrl { get; set; }

            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required(ErrorMessage = "���������� ������� ������ ������.")]
            [DataType(DataType.Password)]
            [Display(Name = "������ ������")]
            public string OldPassword { get; set; } = "";

            [Required(ErrorMessage = "���������� ������� ����� ������.")]
            [DataType(DataType.Password)]
            [Display(Name = "������")]
            public string NewPassword { get; set; } = "";

            [Required(ErrorMessage = "���������� ����������� ����� ������.")]
            [Compare("NewPassword", ErrorMessage = "������ �� ���������")]
            [DataType(DataType.Password)]
            [Display(Name = "����������� ������")]
            public string PasswordConfirm { get; set; } = "";
        }
        public async Task<IActionResult> OnGetAsync(string id, string returnUrl)
        {
            User user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            Input = new InputModel()
            {
                Id = user.Id,
                Email = user.Email,
                ReturnUrl = returnUrl
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                User user = await _userManager.FindByIdAsync(Input.Id);
                if (user != null)
                {
                    IdentityResult result =
                        await _userManager.ChangePasswordAsync(user, Input.OldPassword, Input.NewPassword);
                    if (result.Succeeded)
                    {
                        return Redirect(Input.ReturnUrl);
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            if(error.Code == "PasswordMismatch")
                                ModelState.AddModelError(string.Empty, "�� ������ ������");
                            else
                                ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "������������ �� ������");
                }
            }
            return Page();
        }
    }
}
