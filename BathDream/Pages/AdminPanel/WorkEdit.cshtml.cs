using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BathDream.Data;
using BathDream.Models;
using System.ComponentModel.DataAnnotations;

namespace BathDream.Pages.AdminPanel
{
    public class WorkEditModel : PageModel
    {
        readonly DBApplicationaContext _db;
        public WorkEditModel(DBApplicationaContext db) => _db = db;

        [BindProperty]
        public InputModel Input { get; set; }
        public class InputModel
        {
            public int Id { get; set; }

            [DataType(DataType.Text)]
            [Display(Name = "Внутреннее имя")]
            public string InnerName { get; set; }

            [DataType(DataType.Text)]
            [Display(Name = "Имя")]
            public string Name { get; set; }

            [DataType(DataType.Text)]
            [Display(Name = "Еденицы измерения")]
            public string Unit { get; set; }

            [DataType(DataType.Currency)]
            [Display(Name = "Цена")]
            public double Price { get; set; }
        }

        public void OnGet()
        {
        }

        public void OnGetEditAsync(int id)
        {
            if (_db.WorkPrices
                    .Where(wp => wp.Id == id)
                    .FirstOrDefault()
                    is WorkPrice work)
            {
                Input = new()
                {
                    Id = work.Id,
                    InnerName = work.InnerName,
                    Name = work.Name,
                    Unit = work.Unit,
                    Price = work.Price
                };
            }
        }

        public async Task<IActionResult> OnPost()
        {
            if(Input.Id == 0)
            {
                WorkPrice work = new()
                {
                    InnerName = Input.InnerName,
                    Name = Input.Name,
                    Unit = Input.Unit,
                    Price = Input.Price
                };

                _db.WorkPrices.Add(work);
            }
            else if (_db.WorkPrices
                    .Where(wp => wp.Id == Input.Id)
                    .FirstOrDefault()
                    is WorkPrice work)
            {
                work.InnerName = Input.InnerName;
                work.Name = Input.Name;
                work.Unit = Input.Unit;
                work.Price = Input.Price;
                _db.WorkPrices.Update(work);
            }
            await _db.SaveChangesAsync();
            return RedirectToPage("./WorkList");
        }
    }
}
