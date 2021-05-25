using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BathDream.Data;
using BathDream.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BathDream.Pages.Account
{
    public class AchChatModel : PageModel
    {
        private readonly DBApplicationaContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AchChatModel(DBApplicationaContext db, IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public class InputModel
        {
            public string UserId { get; set; }
            public string OrderId { get; set; }

        }

        public void OnGet(string id, string orderid)
        {
            Input.UserId = id;
            Input.OrderId = orderid;
        }

        public async Task<IActionResult> OnPostSaveFileAsync(IFormFile uploadedFile)
        {
            FileItem fileItem = new FileItem();

            var file = Guid.NewGuid().ToString();

            fileItem.FrendlyName = uploadedFile.FileName;

            string extension = Path.GetExtension(uploadedFile.FileName);

            string webRootPath = _webHostEnvironment.WebRootPath;

            fileItem.Path = webRootPath + "/files/" + Input.OrderId + "/" + file + extension;
            using (var fileStream = new FileStream(fileItem.Path, FileMode.Create))
            {
                await uploadedFile.CopyToAsync(fileStream);
            }


            fileItem.Description = "test";
            fileItem.Order = _db.Orders.FirstOrDefault(o => o.Id == Convert.ToInt32(Input.OrderId));

            _db.FileItems.Add(fileItem);
            _db.SaveChanges();
            

            return Page();
        }
    }
}
