using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BathDream.Data;
using BathDream.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace BathDream.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly BDApplicationaContext _db;

        public RegisterModel(UserManager<User> userManager, BDApplicationaContext db)
        {
            _userManager = userManager;
            _db = db;
        }

        [BindProperty]
        public InputModel Input { get; set; }
        public class InputModel
        {
            [Required(ErrorMessage = "��������� ������� ���")]
            [StringLength(64, ErrorMessage = "������ ����� ������ ���� �� {1} �� {2}", MinimumLength = 2)]
            [DataType(DataType.Text)]
            [Display(Name = "���")]
            public string Name { get; set; }

            [Required(ErrorMessage = "��������� ������� �������")]
            [StringLength(64, ErrorMessage = "������ ������� ������ ���� �� {1} �� {2}", MinimumLength = 2)]
            [DataType(DataType.Text)]
            [Display(Name = "�������")]
            public string Famaly { get; set; }

            [StringLength(64, ErrorMessage = "������ ����� ������ ���� �� {1} �� {2}", MinimumLength = 2)]
            [DataType(DataType.Text)]
            [Display(Name = "��������")]
            public string Patronymic { get; set; }

            [Required(ErrorMessage = "�� ������ email")]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required(ErrorMessage = "��������� ������� �������")]
            [DataType(DataType.PhoneNumber)]
            [Display(Name = "�������")]
            public string Phone { get; set; }

            [Required(ErrorMessage = "��������� ������� ������")]
            [DataType(DataType.Text)]
            [Display(Name = "������")]
            public string Address { get; set; }

            [Required(ErrorMessage = "��������� ������� ������")]
            [DataType(DataType.Password)]
            [Display(Name = "������")]
            public string Password { get; set; }

            [Required(ErrorMessage = "��������� ����������� ������")]
            [Compare("Password", ErrorMessage = "������ �� ���������")]
            [DataType(DataType.Password)]
            [Display(Name = "����������� ������")]
            public string PasswordConfirm { get; set; }

            public string Role { get; set; }
        }
        public void OnGet(string role)
        {
            Input = new InputModel { Role = role };
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    UserName = Input.Email,
                    UName = Input.Name,
                    UFamaly = Input.Famaly,
                    UPatronymic = Input.Patronymic,
                    Email = Input.Email,
                    PhoneNumber = Input.Phone,
                    //Address = Input.Address
                };

                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    switch (Input.Role)
                    {
                        case "executor":
                            ExecutorProfile executor_profile = new()
                            {
                                Address = Input.Address,
                                User = user
                            };
                            await _db.ExecutorProfiles.AddAsync(executor_profile);
                            break;
                        default:
                            UserProfile user_profile = new()
                            {
                                Address = Input.Address,
                                User = user
                            };
                            await _db.UserProfiles.AddAsync(user_profile);
                            break;
                    }
                    

                    await _userManager.AddToRoleAsync(user, Input.Role);
                    await _db.SaveChangesAsync();
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new
                        {
                            userId = user.Id,
                            code
                        },
                        protocol: Request.Scheme);
                    EmailService email = new();
                    await email.SendEmailAsync(user.Email, "����������� ������", $"��� ���������� ����������� ��������� �� ������ <a href='{callbackUrl}'>link</a>");
                    return RedirectToPage("/Account/RegisterConfirm");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return Page();
        }
    }
}
