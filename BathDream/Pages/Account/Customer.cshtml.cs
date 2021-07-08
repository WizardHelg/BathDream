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
using BathDream.Services;

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
        private readonly EmailSender _emailSender;

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();
        public class InputModel
        {
            public User User { get; set; }
            public bool Signed { get; set; } = false;
            public string SignText { get; set; } = "Не подписан";
            public string UserName { get; set; }
            public string UserFamaly { get; set; }
            public bool Display { get; set; } = false;
            public int OrderNumber { get; set; }
            public string OrderDate { get; set; }
            public List<Room> Rooms { get; set; }
            public List<Work> Works { get; set; }
            public double Total { get; set; }
            public string ContentView { get; set; }
            public DateTime? StartDate { get; set; }
            public string OrderAddress { get; set; }
            public List<FileItem> FileItems { get; set; }
            public List<Order> Orders { get; set; }
            public Order CurrentOrder { get; set; }
            public List<Work> UniqueWorks { get; set; }

            /// <summary>
            /// Аванс
            /// </summary>
            public Payment PrepaidPayment { get; set; }

            /// <summary>
            /// Остаток
            /// </summary>
            public Payment ResidualPayment { get; set; }
            public List<Payment> Payments { get; set; }
            public PaymentHandler PaymentHandler { get; set; }
            public string PaymentMessage { get; set; }

            /// <summary>
            /// 1 для частичных представлений, типа: "<partial></partial>", 2 для модальных окон
            /// </summary>
            public int Flag { get; set; } = 2;

            /// <summary>
            /// Детали заказа - договор
            /// </summary>
            public int FlagContract { get; set; } = 1;

            /// <summary>
            /// Бриф - чат
            /// </summary>
            public int FlagBrief { get; set; } = 0;

            /// <summary>
            /// 0 - нет материалов, 1 - есть
            /// </summary>
            public int FlagMaterials { get; set; } = 0;

            /// <summary>
            /// 0 - нет доп. работ, 1 - есть
            /// </summary>
            public int FlagAddWorks { get; set; } = 0;


            public List<Invoice> Invoices { get; set; }
            public Invoice Invoice { get; set; }

            public bool IsFilled { get; set; }
        }

        public CustomerModel(SignInManager<User> signInManager, UserManager<User> userManager,
            DBApplicationaContext db, IWebHostEnvironment webHostEnvironment, IHubContext<ChatHub> hub, EmailSender emailSender)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _db = db;
            _webHostEnvironment = webHostEnvironment;
            _hub = hub;
            _emailSender = emailSender;
        }

        public async Task<IActionResult> OnGetAsync()
        {

            User user = await _userManager.FindByNameAsync(User.Identity.Name);
            await _db.Entry(user).Reference(u => u.Profile).LoadAsync();

            if (TempData["OrderId"] is int orid
                && await _db.Orders.FirstOrDefaultAsync(o => o.Id == orid) is Order order)
            {
                order.Customer = user.Profile;
                order.Status = Order.Statuses.New;

                user.Profile.CurrentOrderId = order.Id;
#if DEBUG

#else
                _emailSender.Send("order@bath-dream.ru", $"Bath-Dream - Заказ №{order.Id}",
                $"Зарегистрирован новый заказ №{order.Id}. \n" +
                $"Пользователь - {user.FullName}, телефон: {user.PhoneNumber}.");
#endif


                await _db.SaveChangesAsync();
            }

            var orders = await _db.Orders.Where(o => o.Customer.Id == user.Profile.Id).ToListAsync();
            if (!orders.Any())
            {
                return RedirectToPage("/Index");
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

            Input.Invoices = await _db.Invoices.Where(i => i.Order.Id == order.Id && i.Type == 2).ToListAsync();
            if (Input.Invoices?.Count > 0)
            {
                Input.FlagMaterials = 1;
            }
            Input.Invoices = await _db.Invoices.Where(i => i.Order.Id == order.Id && i.Type == 3).ToListAsync();
            if (Input.Invoices?.Count > 0)
            {
                Input.FlagAddWorks = 1;
            }

            Input.PrepaidPayment = _db.Payments.Where(p => p.Invoice.Order.Id == order.Id && p.Invoice.Type == 1)
                                      .Include(p => p.Invoice)
                                      .FirstOrDefault(p => p.PaymentStatus == "2" && p.Description == "Аванс");

            return Page();
        }

        public async Task<IActionResult> OnGetEstimateAsync()
        {
            //Input.ContentView = "./Views/CustomerEstimatePartialView";

            User user = await _userManager.FindByNameAsync(User.Identity.Name);
            await _db.Entry(user).Reference(u => u.Profile).LoadAsync();

            Input.User = user;

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

            Input.User = user;

            int order_id = 0;
            if (await _db.Orders.Where(o => o.Id == user.Profile.CurrentOrderId)
                                .Include(o => o.Estimate)
                                    .ThenInclude(e => e.Works)
                                    .ThenInclude(w => w.WorkType)
                                .FirstOrDefaultAsync() is Order order)
            {
                Input.StartDate = order.StartDate;
                Input.SignText = order.Signed ? "Подписан" : "Не подписан";
                Input.OrderDate = order.Date.ToShortDateString();
                Input.OrderNumber = order.Id;
                Input.OrderAddress = order.ObjectAdress ?? "_";
                Input.Signed = order.Signed;
                order_id = order.Id;
                Input.Works = order.Estimate.Works;
            }
            else
            {
                return RedirectToPage("/Account/Customer");
            }


            Input.PaymentHandler = new PaymentHandler();

            Input.Payments = await _db.Payments.Where(p => p.Invoice.Order.Id == order.Id && p.Invoice.Type == 1).Include(p => p.Invoice).ToListAsync();

            Input.Payments = CheckStatus(Input.Payments);

            await _db.SaveChangesAsync();

            Input.PrepaidPayment = Input.Payments.FirstOrDefault(p => p.PaymentStatus == "2" && p.Description == "Аванс");
            Input.ResidualPayment = Input.Payments.FirstOrDefault(p => p.PaymentStatus == "2" && p.Description == "Остаток");

            Input.IsFilled = user.Profile.IsFilled();

            //if (!Input.Signed && (string.IsNullOrEmpty(Input.OrderAddress) || !user.Profile.IsFilled()))
            //    return RedirectToPage("/OrderDetails", new { OrderId = order_id });


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


            return new PartialViewResult
            {
                ViewName = "./Views/CustomerContractPartialView",
                ViewData = new Microsoft.AspNetCore.Mvc.ViewFeatures.ViewDataDictionary<InputModel>(ViewData, Input)
            };
        }



        /// <summary>
        /// Проверяет статусы оплат в Payments, сверяет их со статусами в Invoice, и, при необходимости, обновляет.
        /// </summary>
        /// <param name="payments">Список загруженных из БД платежей конкретного Invoice.Type (1 - смета, 2 - материалы, 3 - дополнительные работы) </param>
        public List<Payment> CheckStatus(List<Payment> payments)
        {
            foreach (var payment in payments)
            {
                if ((payment.PaymentStatus == "2" || payment.PaymentStatus == "6") && payment.PaymentStatus == payment.Invoice.StatusPayment)
                {
                    continue;
                }
                string status = Input.PaymentHandler.GetPaymentStatus(payment.PaymentId);
                if (payment.PaymentStatus != status
                 || payment.Invoice.StatusPayment != status
                 || payment.PaymentStatus == null)
                {
                    payment.PaymentStatus = status;
                    payment.Invoice.StatusPayment = status;
                }
            }
            return payments;
        }

        public async Task<IActionResult> OnGetSignAsync()
        {
            User user = await _userManager.FindByNameAsync(User.Identity.Name);
            await _db.Entry(user).Reference(u => u.Profile).LoadAsync();
            Order order = await _db.Orders.Where(o => o.Id == user.Profile.CurrentOrderId).FirstOrDefaultAsync();

            if (order != null)
            {
                Input.OrderDate = order.Date.ToShortDateString();
                Input.OrderNumber = order.Id;
                Input.OrderAddress = order.ObjectAdress;
                order.Signed = true;
                Input.SignText = "Подписан";
                Input.Signed = order.Signed;
                await _db.SaveChangesAsync();
            }

            return RedirectToPage();
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

        public async Task<IActionResult> OnGetMaterialAsync(int id)
        {
            Input.CurrentOrder = new Order() { Id = id };
            Input.Invoices = await _db.Invoices.Where(i => i.Order.Id == id && i.Type == 2).Include(i => i.Materials).ToListAsync();

            Input.PaymentHandler = new PaymentHandler();

            Input.Payments = await _db.Payments.Where(p => p.Invoice.Order.Id == id && p.Invoice.Type == 2).Include(p => p.Invoice).ToListAsync();

            Input.Payments = CheckStatus(Input.Payments);

            await _db.SaveChangesAsync();

            return new PartialViewResult
            {
                ViewName = "./Views/MaterialPartialView",
                ViewData = new Microsoft.AspNetCore.Mvc.ViewFeatures.ViewDataDictionary<InputModel>(ViewData, Input)
            };

        }
        public async Task<IActionResult> OnPostMaterialAsync(int id, double total)
        {
            Invoice invoice = await _db.Invoices.Where(i => i.Id == id).Include(i => i.Order).FirstOrDefaultAsync();

            if (invoice == null)
            {
                return NotFound();
            }
            int amount = Convert.ToInt32(total * 100);
            string paymentNumber = await GeneratePaymentNumber(invoice.Order.Id, 2);

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
                        paymentMessage = Input.PaymentMessage = Input.PaymentHandler.AlfabankPayment.ErrorMessage //поправить
                    });
            }

            string paymentUrl = Input.PaymentHandler.AlfabankPayment.PaymentUrl;
            string paymentId = Input.PaymentHandler.AlfabankPayment.PaymentId;

            Payment payment = new Payment()
            {
                PaymentId = paymentId,
                PaymentStatus = "0",
                PaymentNumber = paymentNumber,
                Date = DateTime.Now,
                Amount = amount,
                Invoice = invoice,
                Description = "Материалы"
            };

            await _db.Payments.AddAsync(payment);
            await _db.SaveChangesAsync();

            return Redirect(paymentUrl);
        }

        public IActionResult OnGetDocuments()
        {
            //Input.ContentView = "./Views/DocumentsPartialView";
            return new PartialViewResult
            {
                ViewName = "./Views/DocumentsPartialView",
                ViewData = new Microsoft.AspNetCore.Mvc.ViewFeatures.ViewDataDictionary<InputModel>(ViewData, Input)
            };
        }

        public async Task<IActionResult> OnGetAdditionalWorksAsync(int id)
        {
            Input.CurrentOrder = new Order() { Id = id };
            Input.Invoices = await _db.Invoices.Where(i => i.Order.Id == id && i.Type == 3).Include(i => i.AdditionalWorks).ToListAsync();

            Input.PaymentHandler = new PaymentHandler();

            Input.Payments = await _db.Payments.Where(p => p.Invoice.Order.Id == id && p.Invoice.Type == 3).Include(p => p.Invoice).ToListAsync();

            Input.Payments = CheckStatus(Input.Payments);

            await _db.SaveChangesAsync();

            return new PartialViewResult
            {
                ViewName = "./Views/AdditionalWorksPartialView",
                ViewData = new Microsoft.AspNetCore.Mvc.ViewFeatures.ViewDataDictionary<InputModel>(ViewData, Input)
            };
        }
        
        public async Task<IActionResult> OnPostAdditionalWorksAsync(int id, double total)
        {
            Invoice invoice = await _db.Invoices.Include(i => i.Order).FirstOrDefaultAsync(i => i.Id == id);
 
            if (invoice == null)
            {
                return NotFound();
            }
            int amount = Convert.ToInt32(total * 100);
            string paymentNumber = await GeneratePaymentNumber(invoice.Order.Id, 3);

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
                        paymentMessage = Input.PaymentMessage = Input.PaymentHandler.AlfabankPayment.ErrorMessage //поправить
                    });
            }

            string paymentUrl = Input.PaymentHandler.AlfabankPayment.PaymentUrl;
            string paymentId = Input.PaymentHandler.AlfabankPayment.PaymentId;

            Payment payment = new Payment()
            {
                PaymentId = paymentId,
                PaymentStatus = "0",
                PaymentNumber = paymentNumber,
                Date = DateTime.Now,
                Amount = amount,
                Invoice = invoice,
                Description = "Дополнительные работы"
            };

            await _db.Payments.AddAsync(payment);
            await _db.SaveChangesAsync();

            return Redirect(paymentUrl);
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


            await Send($"Я выбрал(а) вариант дизайна {fileItem.FrendlyName}", fileItem.Order.Customer.UserId, fileItem.Order);

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

        public async Task<IActionResult> OnGetPaymentAsync(int orderId)
        {
            Order order = await _db.Orders.Include(o => o.Estimate).ThenInclude(e => e.Works).FirstOrDefaultAsync(o => o.Id == orderId);
            Input.Works = order.Estimate.Works.OrderBy(w => w.Position).ToList();
            Input.Total = Input.Works.Sum(w => w.Total);

            Invoice invoice = await _db.Invoices.FirstOrDefaultAsync(i => i.Order.Id == orderId && i.Type == 1);
            if (invoice == null)
            {
                invoice = new Invoice
                {
                    Type = 1,
                    Order = order,
                    DateTime = DateTime.Now
                };
                await _db.Invoices.AddAsync(invoice);
            }

            int amount = 0;
            string description = "";

            Payment payment = await _db.Payments.FirstOrDefaultAsync(p => p.Invoice.Order.Id == orderId && p.Description == "Аванс" && p.PaymentStatus == "2");
            if (payment != null)
            {
                amount = Convert.ToInt32(Input.Total * 100) - payment.Amount;

                description = "Остаток";
            }
            else
            {
                int amountBuffer = Convert.ToInt32(Input.Total * 100);
                amount = Convert.ToInt32(Math.Round((double)(amountBuffer / 10)));

                description = "Аванс";
            }

            string paymentNumber = await GeneratePaymentNumber(order.Id, 1);

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
                        paymentMessage = Input.PaymentMessage = Input.PaymentHandler.AlfabankPayment.ErrorMessage //поправить
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
                Invoice = invoice,
                Description = description
            };

            await _db.Payments.AddAsync(payment);
            await _db.SaveChangesAsync();

            return Redirect(paymentUrl);
        }

        public async Task<string> GeneratePaymentNumber(int orderId, int type)
        {
            string result = "";
            int count = await _db.Payments.Where(p => p.Invoice.Order.Id == orderId && p.Invoice.Type == type).CountAsync() + 1;
            if (type == 1)
            {
                result = $"TestOrder{orderId}-{count}";
            }
            else if (type == 2)
            {
                result = $"TestOrder{orderId}-Materials-{count}";
            }
            else if (type == 3)
            {

                result = $"TestOrder{orderId}-AddWork-{count}";
            }
            
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
        public async Task<IActionResult> OnPostDeleteWorkAsync(int workId)
        {
            Work work = await _db.Works.Where(w => w.Id == workId).Include(w => w.Estimate).FirstOrDefaultAsync();
            if (work == null)
            {
                return NotFound();
            }
            _db.Works.Remove(work);
            _db.SaveChanges();

            var works = await _db.Works.Where(w => w.Estimate.Id == work.Estimate.Id).ToListAsync();
            double buf = works.Sum(w => w.Total);
            string totalAll = buf.ToString("# ##0.00");

            if (! await _db.Works.AnyAsync(w => w.Estimate.Id == work.Estimate.Id && w.Group == work.Group))
            {
                return new JsonResult(
                    new
                    {
                        group = work.Group,
                        totalAll = totalAll
                    }
                );
            }

            return new JsonResult(new
            {
                totalAll = totalAll
            });
        }


        public async Task<IActionResult> OnPostEditWorkAsync(int workId, string volume)
        {
            Work work = await _db.Works.Where(w => w.Id == workId).Include(w => w.Estimate).FirstOrDefaultAsync();
            if (work == null)
            {
                return NotFound();
            }

            work.Volume = Convert.ToDouble(volume);

            _db.Works.Update(work);
            _db.SaveChanges();

            string totalRow = work.Total.ToString("# ##0.00");

            var works = await _db.Works.Where(w => w.Estimate.Id == work.Estimate.Id).ToListAsync();
            double buf = works.Sum(w => w.Total);
            string totalAll = buf.ToString("# ##0.00");

            return new JsonResult(new
            {
                totalRow = totalRow,
                totalAll = totalAll
            });
        }
    }
}