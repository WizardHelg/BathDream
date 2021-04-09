using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BathDream.Models;
using BathDream.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BathDream.Validators
{
    public class CustomUserValidator : IUserValidator<User>
    {
        public string AllowedUserNameCharactersReqEx { get; set; }
        public bool RequireUniqueEmail { get; set; }

        public async Task<IdentityResult> ValidateAsync(UserManager<User> manager, User user)
        {
            List<IdentityError> errors = new();

            if (!string.IsNullOrEmpty(user.Email)
                && !string.IsNullOrEmpty(AllowedUserNameCharactersReqEx)
                && !Regex.IsMatch(user.Email, AllowedUserNameCharactersReqEx))
                    errors.Add(new IdentityError() { Description = "Формат E-Mail должен быть a@a.a. Допустимы только буквенно-цифровые симолы а так же символы: @ - _ ." });

            //manager.FindByIdAsync(user.Id)
            bool isByPhone = await manager.Users
                .FirstOrDefaultAsync(u => u.PhoneNumber == user.PhoneNumber
                                     && u.Id != user.Id) != null;

            bool isById = await manager.FindByIdAsync(user.Id) != null;
            //bool isByEmail = await manager.FindByEmailAsync(user.Email) != null;
            if (isByPhone)
                errors.Add(new IdentityError() { Description = "Такой номер телефона уже зарегестрирован в системе" });

            return errors.Count == 0 ?
                IdentityResult.Success : IdentityResult.Failed(errors.ToArray());
        }
    }
}
