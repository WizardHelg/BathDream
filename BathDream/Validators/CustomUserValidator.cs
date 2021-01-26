using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BathDream.Models;
using Microsoft.AspNetCore.Identity;

namespace BathDream.Validators
{
    public class CustomUserValidator : IUserValidator<User>
    {
        public string AllowedUserNameCharactersReqEx { get; set; }
        public bool RequireUniqueEmail { get; set; }
        public async Task<IdentityResult> ValidateAsync(UserManager<User> manager, User user)
        {
            List<IdentityError> errors = new List<IdentityError>();

            if (String.IsNullOrEmpty(AllowedUserNameCharactersReqEx) && Regex.IsMatch(user.Email, AllowedUserNameCharactersReqEx))
                errors.Add(new IdentityError() { Description = "Допустимы только буквенно-цифровые симолы а так же символы: @ - _ ." });

            //manager.FindByIdAsync(user.Id)
            bool isById = await manager.FindByIdAsync(user.Id) != null;
            bool isByEmail = await manager.FindByEmailAsync(user.Email) != null;
            if (RequireUniqueEmail && !isById && isByEmail)
                errors.Add(new IdentityError() { Description = "Такой email уже зарегестрирован в системе" });

            return errors.Count == 0 ?
                IdentityResult.Success : IdentityResult.Failed(errors.ToArray());
        }
    }
}
