using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.Text.RegularExpressions;
using BathDream.Models;

namespace BathDream.Validators
{
    public class CustomPasswordValidator : IPasswordValidator<User>
    {
        public int RequiredLength { get; set; }
        public bool RequireNonAlphanumeric { get; set; }
        public bool RequireLowercase { get; set; }
        public bool RequireUppercase { get; set; }
        public bool RequireDigit { get; set; }

        public Task<IdentityResult> ValidateAsync(UserManager<User> manager, User user, string password)
        {
            List<IdentityError> errors = new List<IdentityError>();

            if (String.IsNullOrEmpty(password) || password.Length < RequiredLength)
                errors.Add(new IdentityError { Description = $"Минимальная длинна пароля {RequiredLength}" });

            string pattern = @"\s+";
            if (Regex.IsMatch(password, pattern))
                errors.Add(new IdentityError { Description = $"Пароль не должен содержать пробелы" });

            pattern = @"\W+";
            if (!Regex.IsMatch(password, pattern))
                errors.Add(new IdentityError { Description = $"Пароль должен содержать хотя бы один не буквенно-цифровой символ" });

            pattern = @"[A-ZА-Я]+";
            if(!Regex.IsMatch(password, pattern))
                errors.Add(new IdentityError { Description = $"Пароль должен содержать хотя бы одну букву в верхнем регистре" });

            pattern = @"[a-zа-я]+";
            if (!Regex.IsMatch(password, pattern))
                errors.Add(new IdentityError { Description = $"Пароль должен содержать хотя бы одну букву в нижнем регистре" });

            pattern = @"\d+";
            if (!Regex.IsMatch(password, pattern))
                errors.Add(new IdentityError { Description = $"Пароль должен содержать хотя бы одну цифру" });

            return Task.FromResult(errors.Count == 0 ?
                IdentityResult.Success : IdentityResult.Failed(errors.ToArray()));
        }
    }
}
