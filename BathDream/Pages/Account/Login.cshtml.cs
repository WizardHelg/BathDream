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
using BathDream.Services;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using BathDream.ClassExtensions;
using BathDream.Attributes;

namespace BathDream.Pages.Account
{
    [AutoValidateAntiforgeryToken]
    public class LoginModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly DBApplicationaContext _db;
        private readonly SMSConfirmator _confirmator;

        public LoginModel(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            DBApplicationaContext db,
            SMSConfirmator confirmator)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _db = db;
            _confirmator = confirmator;
        }

        public string ReturnUrl { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Требуется указать телефон")]
            [DataType(DataType.PhoneNumber)]
            [Display(Name = "Телефон")]
            //[RegularExpression(@"[7][0-9]{10}", ErrorMessage = "Введите номер телефона в формате: 7, далее 10 цифр номера без разделителей")]
            [PhoneNumber(ErrorMessage = "Не верный формат номера телефона")]
            public string Phone { get; set; }

            [Required(ErrorMessage = "Требуется указать код")]
            [DataType(DataType.Text)]
            [StringLength(4, MinimumLength = 4, ErrorMessage = "Код должен содержать 4 цифры")]
            [RegularExpression(@"[0-9]*", ErrorMessage = "Код может содержать только цифры")]
            [Display(Name = "Код подтверждения")]
            public string Code { get; set; }

            public string GUID { get; set; }
            public bool CheckMode { get; set; }
            public string ReturnUrl { get; set; }
            public string Role { get; set; }

            [Display(Name = "Код пришедший на телефон")]
            public string TempCode { get; set; }
        }

        public void OnGet(string returnUrl = null, string role = "customer")
        {
            if (Input == null) Input = new InputModel();
            returnUrl ??= Url.Content("~/");
            Input.ReturnUrl = returnUrl;
            Input.CheckMode = false;
            Input.Role = role;
        }

        public IActionResult OnPostSendCodeAsync()
        {
            if (ModelState.TryGetValue("Input.Phone", out ModelStateEntry value)
                && value.ValidationState == ModelValidationState.Valid)
            {
                (string guid, string code) = _confirmator.AddNewSMSConfirmation();
                Input.GUID = guid;
                Input.CheckMode = true;
                Input.TempCode = code;
            }
            
            ModelState["Input.Code"].Errors.Clear();
            return Page();
        }

        public async Task<IActionResult> OnPostCheckCodeAsync()
        {
            if(ModelState.IsValid && _confirmator.TryConfirm(Input.GUID, Input.Code))
            {
                User user = await _db.Users.FirstOrDefaultAsync(u => u.PhoneNumber == Input.Phone);

                if (user is null)
                {
                    user = new()
                    {
                        PhoneNumber = Input.Phone,
                    };

                    user.UserName = user.Id;

                    var result = await _userManager.CreateAsync(user);
                    if (!result.Succeeded)
                    {
                        ModelState.AddModelError(string.Empty, "Ошибка создания пользователя");
                        return Page();
                    }

                    await _userManager.AddToRoleAsync(user, Input.Role);
                    
                    if(Input.Role == "executor")
                    {
                        ExecutorProfile profile = new()
                        {
                            User = user
                        };
                        await _db.ExecutorProfiles.AddAsync(profile);
                    }
                    if (Input.Role == "architector")
                    {
                        UserProfile profile = new()
                        {
                            User = user
                        };
                        await _db.UserProfiles.AddAsync(profile);
                    }
                    else
                    {
                        UserProfile profile = new()
                        {
                            User = user
                        };
                        await _db.UserProfiles.AddAsync(profile);
                    }

                    await _db.SaveChangesAsync();
                }

                //user = await _userManager.FindByIdAsync(user.Id);
                await _signInManager.SignInAsync(user, true);
                return Redirect(Input.ReturnUrl);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Не верный код подтверждения");
            }

            Input.CheckMode = true;
            return Page();
        }

        public async Task<IActionResult> OnGetLogoutAsync(string returnUrl)
        {
            returnUrl ??= Url.Content("~/");
            await _signInManager.SignOutAsync();
            return Redirect(returnUrl);
        }
    }
}
