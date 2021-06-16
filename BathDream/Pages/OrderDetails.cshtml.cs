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
using Microsoft.EntityFrameworkCore;
using BathDream.ClassExtensions;
using BathDream.Attributes;

namespace BathDream.Pages
{
    [Authorize(Roles = "customer")]
    public class OrderDetailsModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly DBApplicationaContext _db;

        public OrderDetailsModel(UserManager<User> userManager, DBApplicationaContext db)
        {
            _userManager = userManager;
            _db = db;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();
        public class InputModel
        {
            [Required(ErrorMessage = "�� ������ �����")]
            [DataType(DataType.Text)]
            [Display(Name = "�����")]
            public string City { get; set; }

            [Required(ErrorMessage = "�� ������� �����")]
            [DataType(DataType.Text)]
            [Display(Name = "�����")]
            public string Street { get; set; }

            [Required(ErrorMessage = "�� ������ ����� ����")]
            [DataType(DataType.Text)]
            [Display(Name = "����� ����")]
            public string HouseNumber { get; set; }

            [Required(ErrorMessage = "�� ������� ��������")]
            [DataType(DataType.Text)]
            [Display(Name = "��������")]
            public string ApartmentNumber { get; set; }

            //[Required(ErrorMessage = "�� ������ ����� �������")]
            //[DataType(DataType.Text)]
            //[Display(Name = "����� �������")]
            public string ObjectAddress
            {
                get => $"{City}, {Street}, {HouseNumber}, {ApartmentNumber}";
            }

            [Required(ErrorMessage = "�� ������ email")]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required(ErrorMessage = "��������� ������� �������")]
            [DataType(DataType.PhoneNumber)]
            [Display(Name = "�������")]
            [PhoneNumber(ErrorMessage = "�� ������ ������ ������ ��������")]
            public string Phone { get; set; }

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

            [Required(ErrorMessage = "�� ������� ����� ��������")]
            [StringLength(4, MinimumLength = 4, ErrorMessage = "����� �������� ������ ��������� 4 �����")]
            [RegularExpression(@"[0-9]*", ErrorMessage = "����� �������� ����� ��������� ������  �����")]
            [Display(Name = "����� ��������")]
            public string PasportSerial { get; set; }

            [Required(ErrorMessage = "�� ������� ����� ��������")]
            [StringLength(6, MinimumLength = 6, ErrorMessage = "����� �������� ������ ��������� 6 �����")]
            [RegularExpression(@"[0-9]*", ErrorMessage = "����� �������� ����� ��������� ������  �����")]
            [Display(Name = "����� ��������")]
            public string PasportNumber { get; set; }

            [Required(ErrorMessage = "�� ������� ��� ����� �������")]
            [DataType(DataType.Text)]
            [Display(Name = "��� ����� �������")]
            public string PasportIssued { get; set; }

            [Required(ErrorMessage = "�� ������� ���� ������ ��������")]
            [DataType(DataType.Date)]
            [Display(Name = "���� ������ ��������")]
            public DateTime? PasportDate { get; set; }

            [Required(ErrorMessage = "�� ������ ����� ����������� �� ��������")]
            [DataType(DataType.Text)]
            [Display(Name = "����� ����������� �� ��������")]
            public string PasportAddress { get; set; }

            public int Id { get; set; }
        }

        public async Task OnGet(int OrderId)
        {
            User user = await _userManager.FindByNameAsync(User.Identity.Name);
            await _db.Entry(user).Reference(u => u.Profile).LoadAsync();
            UserProfile profile = user.Profile;

            Input.Id = OrderId;
            Input.Email = user.Email;
            Input.Phone = user.PhoneNumber.GetPhoneNumberToView();
            Input.Name = user.UName;
            Input.Famaly = user.UFamaly;
            Input.Patronymic = user.UPatronymic;
            Input.PasportSerial = profile.PasportSerial;
            Input.PasportNumber = profile.PasportNumber;
            Input.PasportIssued = profile.PasportIssued;
            Input.PasportDate = profile.PasportDate;
            Input.PasportAddress = profile.PasportAddress;
        }

        public async Task<IActionResult> OnPost()
        {
            if (ModelState.IsValid)
            {
                User user = await _userManager.FindByNameAsync(User.Identity.Name);
                await _db.Entry(user).Reference(u => u.Profile).LoadAsync();
                UserProfile profile = user.Profile;
                Order order = await _db.Orders.Where(o => o.Id == Input.Id).FirstOrDefaultAsync();

                if(user != null && order != null)
                {
                    user.UName = Input.Name;
                    user.UFamaly = Input.Famaly;
                    user.UPatronymic = Input.Patronymic;
                    user.PhoneNumber = Input.Phone.GetPhoneNumber();
                    user.Email = Input.Email;
                    profile.PasportSerial = Input.PasportSerial;
                    profile.PasportNumber = Input.PasportNumber;
                    profile.PasportIssued = Input.PasportIssued;
                    profile.PasportDate = Input.PasportDate;
                    profile.PasportAddress = Input.PasportAddress;

                    var result = await _userManager.UpdateAsync(user);
                    if(!result.Succeeded)
                    {
                        foreach (var error in result.Errors)
                            ModelState.AddModelError(string.Empty, error.Description);

                        return Page();
                    }

                    order.ObjectAdress = Input.ObjectAddress;

                    _db.Orders.Update(order);
                    await _db.SaveChangesAsync();
                    return RedirectToPage("/Account/Customer");
                }
            }
            return Page();
        }
    }
}
