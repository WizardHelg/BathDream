using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BathDream.Pages.Account
{
    public class ExecutorChatModel : PageModel
    {
        [BindProperty]
        public string UserId { get; set; }
        public void OnGet(string id)
        {
            UserId = id;
        }
    }
}
