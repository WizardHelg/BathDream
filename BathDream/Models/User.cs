using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace BathDream.Models
{
    public class User : IdentityUser
    {
        public string UName { get; set; }
        public string UFamaly { get; set; }
        public string UPatronymic { get; set; }
        public string Address { get; set; }

        [NotMapped]
        public string FullName => $"{UName} {UPatronymic} {UFamaly}";

        [NotMapped]
        public string ShortName => $"{UFamaly} {(String.IsNullOrEmpty(UName) ? "" : $"{UName.First()}.")} {(String.IsNullOrEmpty(UPatronymic) ? "" : $"{UPatronymic.First()}.")}";
    }
}
