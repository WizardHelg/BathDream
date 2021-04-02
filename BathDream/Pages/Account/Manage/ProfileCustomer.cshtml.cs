using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using BathDream.Models;
using BathDream.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BathDream.Pages.Account.Manage
{
    [Authorize(Roles = "customer")]
    public class ProfileCustomerModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly DBApplicationaContext _db;

        public ProfileCustomerModel(UserManager<User> userManager, DBApplicationaContext db)
        {
            _userManager = userManager;
            _db = db;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();
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

            public string Id { get; set; }
            public string Role { get; set; }
            //public List<FeedBack> FeedBacks { get; set; }

        }
        public async Task OnGet()
        {
            User user = await _userManager.FindByNameAsync(User.Identity.Name);
            await _db.Entry(user).Reference(u => u.Profile).LoadAsync();
            UserProfile profile = user.Profile;
            Input.Id = user.Id;
            Input.Name = user.UName;
            Input.Famaly = user.UFamaly;
            Input.Patronymic = user.UPatronymic;
            Input.Email = user.Email;
            Input.Phone = user.PhoneNumber;
            Input.Address = profile.Address;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                User user = await _userManager.FindByIdAsync(Input.Id);
                _db.Entry(user).Reference(x => x.Profile).Load();
                UserProfile profile = user.Profile;

                if (user != null)
                {
                    user.UName = Input.Name;
                    user.UFamaly = Input.Famaly;
                    user.UPatronymic = Input.Patronymic;
                    user.PhoneNumber = Input.Phone;
                    profile.Address = Input.Address;

                    var result = await _userManager.UpdateAsync(user);
                    if (!result.Succeeded)
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                    else await _db.SaveChangesAsync();
                }
            }
            return Page();
        }
    }
}