using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BathDream.Pages.Account.Payments
{
    public class ReturnUrlModel : PageModel
    {
        public void OnGet(string orderId)
        {
            string test = orderId;
        }
    }
}
