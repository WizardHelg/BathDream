using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BathDream.Pages.AdminPanel
{
    public class IndexModel : PageModel
    {
        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            TempData["huita"] = "Каое же говно";
            //TempData["huita2"] = new BathDream.Models.Order() { Status = "Редкий еблан" };
            return RedirectToPage("./TestTempData");
        }
    }
}
