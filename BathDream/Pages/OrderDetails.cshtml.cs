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
            [Required(ErrorMessage = "Не указан город")]
            [DataType(DataType.Text)]
            [Display(Name = "Город")]
            public string City { get; set; }

            [Required(ErrorMessage = "Не указана улица")]
            [DataType(DataType.Text)]
            [Display(Name = "Улица")]
            public string Street { get; set; }

            [Required(ErrorMessage = "Не указан номер дома")]
            [DataType(DataType.Text)]
            [Display(Name = "Номер дома")]
            public string HouseNumber { get; set; }

            [Required(ErrorMessage = "Не указана квартира")]
            [DataType(DataType.Text)]
            [Display(Name = "Квартира")]
            public string ApartmentNumber { get; set; }

            //[Required(ErrorMessage = "Не указан адрес объекта")]
            //[DataType(DataType.Text)]
            //[Display(Name = "Адрес объекта")]
            public string ObjectAddress
            {
                get => $"{City}, {Street}, {HouseNumber}, {ApartmentNumber}";
            }

            [Required(ErrorMessage = "Не указан email")]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Требуется указать телефон")]
            [DataType(DataType.PhoneNumber)]
            [Display(Name = "Телефон")]
            [PhoneNumber(ErrorMessage = "Не верный формат номера телефона")]
            public string Phone { get; set; }

            [Required(ErrorMessage = "Требуется указать имя")]
            [StringLength(64, ErrorMessage = "Длинна имени должна быть от {1} до {2}", MinimumLength = 2)]
            [DataType(DataType.Text)]
            [Display(Name = "Имя")]
            public string Name { get; set; }

            [Required(ErrorMessage = "Требуется указать фамилию")]
            [StringLength(64, ErrorMessage = "Длинна фамилии должна быть от {1} до {2}", MinimumLength = 2)]
            [DataType(DataType.Text)]
            [Display(Name = "Фамилия")]
            public string Famaly { get; set; }

            [StringLength(64, ErrorMessage = "Длинна имени должна быть от {1} до {2}", MinimumLength = 2)]
            [DataType(DataType.Text)]
            [Display(Name = "Отчество")]
            public string Patronymic { get; set; }

            [Required(ErrorMessage = "Не указана серия паспорта")]
            [StringLength(4, MinimumLength = 4, ErrorMessage = "Серия паспорта должна содержать 4 цифры")]
            [RegularExpression(@"[0-9]*", ErrorMessage = "Серия паспорта может сожержать только  цифры")]
            [Display(Name = "Серия паспорта")]
            public string PasportSerial { get; set; }

            [Required(ErrorMessage = "Не указана номер паспорта")]
            [StringLength(6, MinimumLength = 6, ErrorMessage = "Номер паспорта должен содержать 6 цифры")]
            [RegularExpression(@"[0-9]*", ErrorMessage = "Номер паспорта может сожержать только  цифры")]
            [Display(Name = "Номер паспорта")]
            public string PasportNumber { get; set; }

            [Required(ErrorMessage = "Не указано кем выдан паспорт")]
            [DataType(DataType.Text)]
            [Display(Name = "Кем выдан паспорт")]
            public string PasportIssued { get; set; }

            [Required(ErrorMessage = "Не указана дата выдачи паспорта")]
            [DataType(DataType.Date)]
            [Display(Name = "Дата выдачи паспорта")]
            public DateTime? PasportDate { get; set; }

            [Required(ErrorMessage = "Не указан адрес регистрации по паспорту")]
            [DataType(DataType.Text)]
            [Display(Name = "Адрес регистрации по паспорту")]
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
