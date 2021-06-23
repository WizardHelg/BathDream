using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BathDream.Data;
using BathDream.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace BathDream.Pages.Account
{
    [Authorize(Roles = "executor")]
    public class ExecutorModel : PageModel
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IHubContext<ChatHub> _hub;

        public List<Order> Data = new List<Order>();

        public List<Order> ExecutingOrders = new List<Order>();

        public int sw = 2;

        private readonly DBApplicationaContext _db;

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();
        public class InputModel
        {
            public Estimate Estimate { get; set; } = new Estimate();
            public Order Order { get; set; } = new Order();
            public string ContentView { get; set; } = "/Pages/Account/Views/ExecutorEstimatePartialView.cshtml";
            public string NameFamaly { get; set; }

            public List<MaterialPrice> MaterialPrices { get; set; }
            public List<MaterialCount> Materials { get; set; } = new List<MaterialCount>();

            public List<WorkPrice> WorkPrices { get; set; }
            public List<AdditionalWorkCount> AdditionalWorks { get; set; } = new List<AdditionalWorkCount>();

        }

        public class MaterialCount
        {
            public MaterialPrice Material { get; set; }
            public int Count { get; set; }
        }
        public class AdditionalWorkCount
        {
            public WorkPrice AdditionalWork { get; set; }
            public int Count { get; set; }
        }

        public ExecutorModel(SignInManager<User> signInManager, UserManager<User> userManager, DBApplicationaContext db, IWebHostEnvironment webHostEnvironment, IHubContext<ChatHub> hub)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _db = db;
            _webHostEnvironment = webHostEnvironment;
            _hub = hub;
        }

        public async Task OnGetAsync(int? flag)
        {
            if (flag != null)
            {
                await OnGetShowAcceptedOrdersAsync();
                return;
            }

            User user = await _userManager.FindByNameAsync(User.Identity.Name);
            Input.NameFamaly = $"{user.UName} {user.UFamaly}";
            await OnGetShowAvailableOrdersAsync();
        }


        public async Task<IActionResult> OnGetLogoutAsync()
        {
            await _signInManager.SignOutAsync();
            return RedirectToPage("/Index");
        }

        public async Task<IActionResult> OnGetShowAvailableOrdersAsync()
        {
            var orders = await _db.Orders.Where(o => (o.Status == Order.Statuses.ToExecute 
                                                   || o.Status == (Order.Statuses.ToExecute | Order.Statuses.New)
                                                   || o.Status == (Order.Statuses.ToExecute | Order.Statuses.Brief)
                                                   || o.Status == (Order.Statuses.ToExecute | Order.Statuses.New | Order.Statuses.Brief)))
                                   .Include(o => o.Customer)
                                   .ThenInclude(c => c.User).ToListAsync();
            foreach (var order in orders)
            {
                Data.Add(order);
            }

            var executingOrders = await _db.Orders.Where(o => o.Executor.UserId == _userManager.GetUserId(User))
                                    .Include(o => o.Customer)
                                    .ThenInclude(c => c.User).ToListAsync();
            foreach (var order in executingOrders)
            {
                ExecutingOrders.Add(order);
            }
            sw = 1;
            return Page();
        }

        public async Task<IActionResult> OnGetShowAcceptedOrdersAsync()
        {
            var executingOrders = await _db.Orders.Where(o => o.Executor.UserId == _userManager.GetUserId(User))
                                    .Include(o => o.Customer)
                                    .ThenInclude(c => c.User).ToListAsync();
            foreach (var order in executingOrders)
            {
                ExecutingOrders.Add(order);
            }
            sw = 2;
            return Page();
        }

        public async Task<IActionResult> OnGetExecutingAsync(int id)
        {
            if (_db.Orders.FirstOrDefault(o => o.Id == id) is Order order)
            {
                _db.Entry(order).Reference(o => o.Customer).Load();
                order.Status ^= Order.Statuses.New;
                order.Status |= Order.Statuses.Executing;
                var executorprofile = _userManager.GetUserId(User);
                var profile = _db.UserProfiles.FirstOrDefault(u => u.UserId == executorprofile);
                order.Executor = (ExecutorProfile)profile;

                if (profile?.User is User user)
                {
                    await SendToClient($"��� �������� ����������� - {profile.User.FullName}", order.Customer.UserId, order);
                }
                else
                {
                    await SendToClient($"��� �������� �����������.", order.Customer.UserId, order);
                }

                _db.SaveChanges();
            }
            return RedirectToPage();
        }
        public IActionResult OnGetShowEstimate(int id)
        {
            string url = Url.Page("/Account/Estimate", new { id = id});
            return Redirect(url);
        }

        public IActionResult OnGetDownloadFile(int id)
        {
            Order order = _db.Orders.FirstOrDefault(o => o.Id == id);

            FileItem fileItem = _db.FileItems.FirstOrDefault(f => f.Id == order.SelectedItemId);
            if (fileItem == null)
            {
                return NotFound();
            }

            string path = _webHostEnvironment.WebRootPath + fileItem.Path;

            var net = new System.Net.WebClient();
            var data = net.DownloadData(path);
            var content = new System.IO.MemoryStream(data);
            var contentType = "APPLICATION/octet-stream";
            var fileName = fileItem.FrendlyName;

            return File(content, contentType, fileName);
        }

        public async Task<IActionResult> OnGetSelectMaterialAsync(int id)
        {
            Input.Order.Id = id;
            Input.MaterialPrices = await _db.MaterialPrices.ToListAsync();

            return new PartialViewResult
            {
                ViewName = "./Views/SelectMaterialPartialView",
                ViewData = new Microsoft.AspNetCore.Mvc.ViewFeatures.ViewDataDictionary<InputModel>(ViewData, Input)
            };
        }

        public async Task<IActionResult> OnPostSelectMaterialAsync()
        {
            Input.Order = await _db.Orders.FirstOrDefaultAsync(o => o.Id == Input.Order.Id);

            Invoice invoice = new Invoice()
            {
                Type = 3,
                Order = Input.Order,
                DateTime = DateTime.Now
            };


            Input.Materials.RemoveAll(m => m.Count == 0);
            if (Input.Materials != null)
            {
                foreach (var item in Input.Materials)
                {
                    Material material = item.Material;
                    material.Count = item.Count;
                    material.Invoice = invoice;
                    _db.Materials.Add(material);
                }
            }
            await _db.SaveChangesAsync();
            return Page();
        }

        public async Task<IActionResult> OnGetSelectAdditionalWork(int id)
        {
            Input.Order.Id = id;
            Input.WorkPrices = await _db.WorkPrices.Include(w => w.WorkType).ToListAsync();

            return new PartialViewResult
            {
                ViewName = "./Views/SelectAdditionalWorkPartialView",
                ViewData = new Microsoft.AspNetCore.Mvc.ViewFeatures.ViewDataDictionary<InputModel>(ViewData, Input)
            };
        }

        public async Task<IActionResult> OnPostSelectAdditionalWork()
        {
            Input.Order = await _db.Orders.FirstOrDefaultAsync(o => o.Id == Input.Order.Id);

            Invoice invoice = new Invoice()
            {
                Type = 3,
                Order = Input.Order,
                DateTime = DateTime.Now
            };


            Input.AdditionalWorks.RemoveAll(m => m.Count == 0);
            if (Input.AdditionalWorks != null)
            {
                await _db.Invoices.AddAsync(invoice);
                foreach (var item in Input.AdditionalWorks)
                {
                    AdditionalWork additionalWork = item.AdditionalWork;
                    additionalWork.WorkType = await _db.WorkTypes.FirstOrDefaultAsync(t => t.Id == item.AdditionalWork.WorkType.Id);
                    additionalWork.Count = item.Count;
                    additionalWork.Invoice = invoice;
                    _db.AdditionalWorks.Add(additionalWork);
                }
            }
            await _db.SaveChangesAsync();

            return Page();
        }

        public async Task SendToClient(string message, string userId, Order order)
        {
            DateTime cur_time = DateTime.Now;
            if (string.IsNullOrWhiteSpace(message))
                return;

            message = message.TrimEnd('\n').Replace("\n", "<br />");

            if (await _userManager.FindByNameAsync(userId) is User user)
            {
                User arch = _db.Users.FirstOrDefault(u => u.PhoneNumber == "70000000000");
                Message temp_message = new Message
                {
                    DateTime = cur_time,
                    Text = message,
                    Sender = arch,
                    Recipient = user,
                    Order = order
                };

                _db.Messages.Add(temp_message);
                _db.SaveChanges();

                await _hub.Clients.User(user.Id).SendAsync("Send", new { IsMe = 0, Name = user.UName, Message = temp_message.Text, When = temp_message.DateTime });
                await _hub.Clients.User(arch.Id).SendAsync("Send", new { IsMe = 1, Name = user.UName, Message = temp_message.Text, When = temp_message.DateTime });
            }
        }
    }
}
