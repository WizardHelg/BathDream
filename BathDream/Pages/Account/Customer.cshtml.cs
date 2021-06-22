using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BathDream.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BathDream.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using BathDream.Acquiring;

namespace BathDream.Pages.Account
{
    [Authorize(Roles = "customer")]
    public class CustomerModel : PageModel
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly DBApplicationaContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IHubContext<ChatHub> _hub;

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();
        public class InputModel
        {
            public bool Signed { get; set; } = false;
            public string SignText { get; set; } = "�� ��������";
            public string UserName { get; set; }
            public string UserFamaly { get; set; }
            public string FIO { get; set; }
            public bool Display { get; set; } = false;
            public string CustomerPhone { get; set; }
            public string CustomerEmail { get; set; }
            public int OrderNumber { get; set; }
            public string OrderDate { get; set; }
            public List<Room> Rooms { get; set; }
            public List<Work> Works { get; set; }
            public double Total { get; set; }
            public string ContentView { get; set; }
            public string OrderAddress { get; set; }
            public string PasportAddress { get; set; }
            public string PasportSerial { get; set; }
            public string PasportNumber { get; set; }
            public string PasportIssued { get; set; }
            public string PasportDate { get; set; }
            public List<FileItem> FileItems { get; set; }
            public List<Order> Orders { get; set; }
            public Order CurrentOrder { get; set; }
            public List<Work> UniqueWorks { get; set; }

            /// <summary>
            /// �����
            /// </summary>
            public Payment PrepaidPayment { get; set; }

            /// <summary>
            /// �������
            /// </summary>
            public Payment ResidualPayment { get; set; }
            public List<Payment> Payments { get; set; }
            public PaymentHandler PaymentHandler { get; set; }
            public string PaymentMessage { get; set; }

            /// <summary>
            /// 1 ��� ��������� �������������, ����: "<partial></partial>", 2 ��� ��������� ����
            /// </summary>
            public int Flag { get; set; } = 2;

            /// <summary>
            /// ������ ������ - �������
            /// </summary>
            public int FlagContract { get; set; } = 0;

            /// <summary>
            /// ���� - ���
            /// </summary>
            public int FlagBrief { get; set; } = 0;

            public List<OrderMaterial> OrderMaterials { get; set; }
        }

        public CustomerModel(SignInManager<User> signInManager, UserManager<User> userManager,
            DBApplicationaContext db, IWebHostEnvironment webHostEnvironment, IHubContext<ChatHub> hub)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _db = db;
            _webHostEnvironment = webHostEnvironment;
            _hub = hub;
        }

        public async Task OnGetAsync()
        {

            User user = await _userManager.FindByNameAsync(User.Identity.Name);
            await _db.Entry(user).Reference(u => u.Profile).LoadAsync();

            if (TempData["OrderId"] is int orid
                && await _db.Orders.FirstOrDefaultAsync(o => o.Id == orid) is Order order)
            {
                order.Customer = user.Profile;
                order.Status = Order.Statuses.New;

                user.Profile.CurrentOrderId = order.Id;

                await _db.SaveChangesAsync();
            }

            order = await _db.Orders.Where(o => o.Id == user.Profile.CurrentOrderId).FirstOrDefaultAsync();

            Input.UserName = user.UName;
            Input.UserFamaly = user.UFamaly;

            Input.CurrentOrder = order;

            Input.OrderAddress = order.ObjectAdress;
            if (!Input.Signed && (string.IsNullOrEmpty(Input.OrderAddress) || !user.Profile.IsFilled()))
            {
                Input.FlagContract = 1;
            }
            if ((order.Status & Order.Statuses.Brief) == 0)
            {
                Input.FlagBrief = 1;
            }
        }

        public async Task<IActionResult> OnGetEstimateAsync()
        {
            Input.ContentView = "./Views/CustomerEstimatePartialView";

            User user = await _userManager.FindByNameAsync(User.Identity.Name);
            await _db.Entry(user).Reference(u => u.Profile).LoadAsync();

            Input.UserName = user.UName;
            Input.UserFamaly = user.UFamaly;
            Input.FIO = $"{user.UFamaly} {user.UName} {user.UPatronymic}";
            Input.CustomerPhone = user.PhoneNumber;
            Input.CustomerEmail = user.Email;

            //if (user.Profile.CurrentOrderId == 0)
            //{
            //    user.Profile.CurrentOrderId = order.Id;
            //}

            //Order order = null;

            Order order = await _db.Orders.Where(o => o.Id == user.Profile.CurrentOrderId)
                                    .Include(o => o.Estimate)
                                    .ThenInclude(e => e.Rooms)
                                    .Include(x => x.Estimate)
                                    .ThenInclude(x => x.Works)
                                    .ThenInclude(w => w.WorkType).FirstOrDefaultAsync();

            if (order != null)
            {
                Input.Display = true;
                Input.OrderDate = order.Date.ToShortDateString();
                Input.OrderNumber = order.Id;
                Input.OrderAddress = order.ObjectAdress;
                Input.Rooms = order.Estimate.Rooms;
                Input.Works = order.Estimate.Works.OrderBy(w => w.Position).ToList();
                Input.Total = Input.Works.Sum(w => w.Total);
                Input.Signed = order.Signed;

                Input.UniqueWorks = new List<Work>();
                foreach (var item in Input.Works)
                {
                    if (Input.UniqueWorks.Any(w => w.WorkType.Id == item.WorkType.Id))
                    {
                        continue;
                    }
                    Input.UniqueWorks.Add(item);
                }
                Input.UniqueWorks = Input.UniqueWorks.OrderBy(w => w.WorkType.Priority).ToList();

            }
            return new PartialViewResult
            {
                ViewName = "./Views/CustomerEstimatePartialView",
                ViewData = new Microsoft.AspNetCore.Mvc.ViewFeatures.ViewDataDictionary<InputModel>(ViewData, Input)
            };
        }

        public async Task<IActionResult> OnGetContractAsync(string paymentMessage = "")
        {
            Input.PaymentMessage = paymentMessage;

            Input.ContentView = "./Views/CustomerContractPartialView";
            User user = await _userManager.FindByNameAsync(User.Identity.Name);
            await _db.Entry(user).Reference(u => u.Profile).LoadAsync();
            Input.UserName = user.UName;
            Input.UserFamaly = user.UFamaly;
            Input.FIO = $"{user.UFamaly} {user.UName} {user.UPatronymic}";
            Input.CustomerPhone = user.PhoneNumber;
            Input.CustomerEmail = user.Email;
            Input.PasportAddress = user.Profile.PasportAddress;
            Input.PasportSerial = user.Profile.PasportSerial;
            Input.PasportNumber = user.Profile.PasportNumber;
            Input.PasportIssued = user.Profile.PasportIssued;
            if (user.Profile.PasportDate is DateTime dt)
                Input.PasportDate = dt.ToShortDateString();

            int order_id = 0;
            if (await _db.Orders.Where(o => o.Id == user.Profile.CurrentOrderId).FirstOrDefaultAsync() is Order order)
            {
                Input.SignText = order.Signed ? "��������" : "�� ��������";
                Input.OrderDate = order.Date.ToShortDateString();
                Input.OrderNumber = order.Id;
                Input.OrderAddress = order.ObjectAdress;
                Input.Signed = order.Signed;
                order_id = order.Id;            }
            else
            {
                return RedirectToPage("/Account/Customer");
            }


            Input.PaymentHandler = new PaymentHandler();

            Input.Payments = await _db.Payments.Where(p => p.Order.Id == order.Id).ToListAsync();
            foreach (var payment in Input.Payments)
            {
                if (payment.PaymentStatus == "2" || payment.PaymentStatus == "6")
                {
                    continue;
                }
                string status = Input.PaymentHandler.GetPaymentStatus(payment.PaymentId);
                if (payment.PaymentStatus != status || payment.PaymentStatus == null)
                {
                    payment.PaymentStatus = status;
                }
            }
            await _db.SaveChangesAsync();

            Input.PrepaidPayment = Input.Payments.FirstOrDefault(p => p.PaymentStatus == "2" && p.Description == "�����");
            Input.ResidualPayment = Input.Payments.FirstOrDefault(p => p.PaymentStatus == "2" && p.Description == "�������");


            if (!Input.Signed && (string.IsNullOrEmpty(Input.OrderAddress) || !user.Profile.IsFilled()))
                return RedirectToPage("/OrderDetails", new { OrderId = order_id });

            return new PartialViewResult
            {
                ViewName = "./Views/CustomerContractPartialView",
                ViewData = new Microsoft.AspNetCore.Mvc.ViewFeatures.ViewDataDictionary<InputModel>(ViewData, Input)
            };
        }



        public async Task<IActionResult> OnGetSignAsync()
        {
            Input.ContentView = "./Views/CustomerContractPartialView";
            User user = await _userManager.FindByNameAsync(User.Identity.Name);
            await _db.Entry(user).Reference(u => u.Profile).LoadAsync();
            Order order = await _db.Orders.Where(o => o.Id == user.Profile.CurrentOrderId).FirstOrDefaultAsync();


            Input.FIO = $"{user.UFamaly} {user.UName} {user.UPatronymic}";
            Input.CustomerPhone = user.PhoneNumber;
            Input.CustomerEmail = user.Email;
            Input.PasportAddress = user.Profile.PasportAddress;
            Input.PasportSerial = user.Profile.PasportSerial;
            Input.PasportNumber = user.Profile.PasportNumber;
            Input.PasportIssued = user.Profile.PasportIssued;
            if (user.Profile.PasportDate is DateTime dt)
                Input.PasportDate = dt.ToShortDateString();

            if (order != null)
            {
                Input.OrderDate = order.Date.ToShortDateString();
                Input.OrderNumber = order.Id;
                Input.OrderAddress = order.ObjectAdress;
                order.Signed = true;
                Input.SignText = "��������";
                Input.Signed = order.Signed;
                await _db.SaveChangesAsync();
            }

            return await OnGetBriefAsync();
        }

        public async Task<IActionResult> OnGetDeleteOrderAsync()
        {
            Input.ContentView = "./Views/CustomerEstimatePartialView";

            User user = await _userManager.FindByNameAsync(User.Identity.Name);
            await _db.Entry(user).Reference(u => u.Profile).LoadAsync();

            Order order = await _db.Orders.Where(o => o.Id == user.Profile.CurrentOrderId).FirstOrDefaultAsync();
            _db.Orders.Remove(order);
            await _db.SaveChangesAsync();
            return RedirectToAction("OnGetAsync");
        }

        public async Task<IActionResult> OnGetBriefAsync()
        {
            User user = await _userManager.FindByNameAsync(User.Identity.Name);
            await _db.Entry(user).Reference(u => u.Profile).LoadAsync();

            //Order order = await _db.Orders.Where(o => o.Customer.User.Id == user.Id).FirstOrDefaultAsync();
            if (await _db.Orders.Where(o => o.Id == user.Profile.CurrentOrderId).FirstOrDefaultAsync() is Order order
               && order.Signed)
            {
                Input.UserName = user.UName;
                Input.UserFamaly = user.UFamaly;
                Input.FileItems = await _db.FileItems.Include(o => o.Order).Where(f => f.Order.Id == order.Id).ToListAsync();
                Input.CurrentOrder = order;
                if ((order.Status & Order.Statuses.Brief) == 0)
                    return RedirectToPage("/Brief", new { id = order.Id });
                else
                {
                    Input.Flag = 1;
                    Input.ContentView = "./Views/ChatPartialView";
                    return Page();
                }
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnGetMaterial()
        {
            User user = await _userManager.FindByNameAsync(User.Identity.Name);
            await _db.Entry(user).Reference(u => u.Profile).LoadAsync();
            Order order = await _db.Orders.Where(o => o.Id == user.Profile.CurrentOrderId).FirstOrDefaultAsync();

            Input.OrderMaterials = await _db.OrderMaterials.Where(o => o.Order.Id == order.Id).Include(m => m.Materials).ToListAsync();

            return new PartialViewResult
            {
                ViewName = "./Views/MaterialPartialView",
                ViewData = new Microsoft.AspNetCore.Mvc.ViewFeatures.ViewDataDictionary<InputModel>(ViewData, Input)
            };
        }
        public IActionResult OnGetDocuments()
        {
            Input.ContentView = "./Views/DocumentsPartialView";
            return new PartialViewResult
            {
                ViewName = "./Views/DocumentsPartialView",
                ViewData = new Microsoft.AspNetCore.Mvc.ViewFeatures.ViewDataDictionary<InputModel>(ViewData, Input)
            };
        }

        /// <summary>
        /// ???????
        /// </summary>
        /// <param name="orderId"></param>
        public void OnGetChat(Order orderId)
        {
            Input.ContentView = "./Views/ChatPartialView";
            Input.CurrentOrder = orderId;
        }

        public async Task<IActionResult> OnGetLogoutAsync()
        {
            await _signInManager.SignOutAsync();
            return RedirectToPage("/Index");
        }

        public async Task<IActionResult> OnGetShowOrders()
        {
            Input.Flag = 1;

            Input.ContentView = "./Views/SelectOrderPartialView";

            User user = await _userManager.FindByNameAsync(User.Identity.Name);
            await _db.Entry(user).Reference(u => u.Profile).LoadAsync();

            Input.UserName = user.UName;
            Input.UserFamaly = user.UFamaly;

            Input.Orders = await _db.Orders.Where(o => o.Customer.Id == user.Profile.Id).ToListAsync();
            Input.CurrentOrder = Input.Orders.FirstOrDefault(o => o.Id == user.Profile.CurrentOrderId);

            return Page();
        }
        public async Task<IActionResult> OnGetSelectOrder(int id)
        {
            Order order = await _db.Orders.FirstOrDefaultAsync(o => o.Id == id);

            Input.CurrentOrder = order;

            User user = await _userManager.FindByNameAsync(User.Identity.Name);
            await _db.Entry(user).Reference(u => u.Profile).LoadAsync();

            user.Profile.CurrentOrderId = order.Id;

            _db.SaveChanges();

            return RedirectToPage();
        }

        public IActionResult OnGetDownloadFile(int? id)
        {
            FileItem fileItem = _db.FileItems.FirstOrDefault(f => f.Id == id);
            string path = _webHostEnvironment.WebRootPath + fileItem.Path;

            var net = new System.Net.WebClient();
            var data = net.DownloadData(path);
            var content = new System.IO.MemoryStream(data);
            var contentType = "APPLICATION/octet-stream";
            var fileName = fileItem.FrendlyName;

            return File(content, contentType, fileName);
        }

        public async Task<IActionResult> OnPostSelectFile(int? id)
        {
            Input.ContentView = "./Views/ChatPartialView";

            FileItem fileItem = await _db.FileItems.Include(f => f.Order).ThenInclude(c => c.Customer).FirstOrDefaultAsync(f => f.Id == id);
            if (fileItem == null)
            {
                return NotFound();
            }

            fileItem.Order.SelectedItemId = (int)id;
            _db.SaveChanges();


            await Send($"� ������(�) ������� ������� {fileItem.FrendlyName}", fileItem.Order.Customer.UserId, fileItem.Order);

            return new JsonResult("");
        }

        public async Task Send(string message, string userId, Order order)
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
                    Sender = user,
                    Recipient = arch,
                    Order = order
                };

                await _db.Messages.AddAsync(temp_message);
                await _db.SaveChangesAsync();

                await _hub.Clients.User(user.Id).SendAsync("Send", new { IsMe = 1, Name = user.UName, Message = temp_message.Text, When = temp_message.DateTime });
                await _hub.Clients.User(arch.Id).SendAsync("Send", new { IsMe = 0, Name = user.UName, Message = temp_message.Text, When = temp_message.DateTime });
            }
        }

        public async Task<IActionResult> OnGetPayment(int orderId)
        {
            Order order = await _db.Orders.Include(o => o.Estimate).ThenInclude(e => e.Works).FirstOrDefaultAsync(o => o.Id == orderId);
            Input.Works = order.Estimate.Works.OrderBy(w => w.Position).ToList();
            Input.Total = Input.Works.Sum(w => w.Total);

            int amount = 0;
            string description = "";

            Payment payment = await _db.Payments.FirstOrDefaultAsync(p => p.Order.Id == orderId && p.Description == "�����" && p.PaymentStatus == "2");
            if (payment != null)
            {
                amount = Convert.ToInt32(Input.Total * 100) - payment.Amount;

                description = "�������";
            }
            else
            {
                int amountBuffer = Convert.ToInt32(Input.Total * 100);
                amount = Convert.ToInt32(Math.Round((double)(amountBuffer / 10)));

                description = "�����";
            }

            string paymentNumber = await GeneratePaymentNumber(order.Id);

            string returnUrl = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}/Account/Payments/ReturnUrl";
            string failUrl = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}/Account/Payments/FailUrl";

            Input.PaymentHandler = new PaymentHandler();
            Input.PaymentHandler.CreatePayment(paymentNumber, amount, returnUrl, failUrl);

            if (Input.PaymentHandler.AlfabankPayment.ErrorCode != "0" && Input.PaymentHandler.AlfabankPayment.ErrorCode != null)
            {
                Input.PaymentMessage = Input.PaymentHandler.AlfabankPayment.ErrorMessage;
                return RedirectToPage("/Account/Customer", "Contract",
                    new
                    {
                        paymentMessage = Input.PaymentMessage = Input.PaymentHandler.AlfabankPayment.ErrorMessage //���������
                    });
            }

            string paymentUrl = Input.PaymentHandler.AlfabankPayment.PaymentUrl;
            string paymentId = Input.PaymentHandler.AlfabankPayment.PaymentId;

            payment = new Payment()
            {
                PaymentId = paymentId,
                PaymentStatus = "0",
                PaymentNumber = paymentNumber,
                Date = DateTime.Now,
                Amount = amount,
                Order = await _db.Orders.FirstOrDefaultAsync(o => o.Id == orderId),
                Description = description
            };

            _db.Payments.Add(payment);
            _db.SaveChanges();

            return Redirect(paymentUrl);
        }

        public async Task<string> GeneratePaymentNumber(int orderId)
        {
            int count = await _db.Payments.Where(p => p.Order.Id == orderId).CountAsync() + 1;
            string result = $"Order{orderId}-{count}";
            return result;
        }

        public IActionResult OnGetNewOrder()
        {
            return RedirectToPage("/Index");
        }

        public IActionResult OnGetProject()
        {
            Input.Flag = 2;
            return RedirectToPage();
        }
        public async Task<IActionResult> OnGetDeleteWork(int workId)
        {
            Work work = await _db.Works.FirstOrDefaultAsync(w => w.Id == workId);
            if (work == null)
            {
                return NotFound();
            }
            _db.Works.Remove(work);
            _db.SaveChanges();

            return RedirectToPage();
        }
    }
}