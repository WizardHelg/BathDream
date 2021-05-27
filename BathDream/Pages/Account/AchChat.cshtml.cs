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
using Microsoft.EntityFrameworkCore;

namespace BathDream.Pages.Account
{
    public class AchChatModel : PageModel
    {
        private readonly DBApplicationaContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;

        const string filePath = @"\files\";

        public AchChatModel(DBApplicationaContext db, IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public class InputModel
        {
            public List<FileItem> FileItems = new List<FileItem>();
            public string UserId { get; set; }
            public string OrderId { get; set; }

        }

        public void OnGet(string id, string orderid)
        {
            Input.FileItems = _db.FileItems.Where(o => o.Order.Id == Convert.ToInt32(orderid)).ToList();
            Input.UserId = id;
            Input.OrderId = orderid;
        }

        public async Task<IActionResult> OnPostSaveFileAsync(IFormFile uploadedFile)
        {
            FileItem fileItem = new FileItem();

            var file = Guid.NewGuid().ToString();
            string extension = Path.GetExtension(uploadedFile.FileName);
            string webRootPath = _webHostEnvironment.WebRootPath;

            string directoryPath = webRootPath + filePath + Input.OrderId + @"\";
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            fileItem.FrendlyName = uploadedFile.FileName;
            fileItem.Path = filePath + Input.OrderId + @"\" + file + extension;

            using (var fileStream = new FileStream(directoryPath + file + extension, FileMode.Create))
            {
                await uploadedFile.CopyToAsync(fileStream);
            }


            fileItem.Description = "test";
            fileItem.Order = _db.Orders.FirstOrDefault(o => o.Id == Convert.ToInt32(Input.OrderId));

            _db.FileItems.Add(fileItem);
            _db.SaveChanges();

            return RedirectToPage(new 
            {
                id = Input.UserId,
                orderid = Input.OrderId
            });
        }

        public IActionResult OnGetDeleteFile(int? id)
        {
            FileItem file = _db.FileItems.Where(f => f.Id == id)
                .Include(f => f.Order).
                ThenInclude(c => c.Customer).FirstOrDefault();

            if (file == null)
            {
                return NotFound();
            }

            var fileitem = _webHostEnvironment.WebRootPath + file.Path;

            if (System.IO.File.Exists(fileitem))
            {
                System.IO.File.Delete(fileitem);
            }

            string folder = _webHostEnvironment.WebRootPath + filePath + file.Order.Id;
            if (!(System.IO.Directory.GetDirectories(folder).Length
                + System.IO.Directory.GetFiles(folder).Length > 0))
            {
                System.IO.Directory.Delete(folder);
            }

            _db.FileItems.Remove(file);
            _db.SaveChanges();

            return RedirectToPage(new
            {
                id = file.Order.Customer.UserId,
                orderid = file.Order.Id
            });
        }
    }
}
