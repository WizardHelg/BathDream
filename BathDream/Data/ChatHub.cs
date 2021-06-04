using BathDream.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BathDream.Data
{
    [Authorize]
    public class ChatHub : Hub
    {
        const string _arch_number = "70000000000";
        private readonly UserManager<User> _user_manager;
        private readonly DBApplicationaContext _db;
        private readonly List<string> _online_userId = new List<string>();

        public ChatHub(UserManager<User> userManager, DBApplicationaContext db)
        {
            _user_manager = userManager;
            _db = db;
        }

        public async Task<Order> GetOrder(string id)
        {
            UserProfile userProfile = await _db.UserProfiles.FirstOrDefaultAsync(u => u.UserId == id);
            Order order = await _db.Orders.Include(o => o.Customer).Include(o => o.Executor).FirstOrDefaultAsync(o => o.Customer.Id == userProfile.Id);
            return order;
        }

        public bool OnExecution(Order order)
        {
            if ((order.Status & Order.Statuses.Executing) > 0) return true;
            return false;
        }

        public Message CreateMessage(DateTime dateTime, string message, User sender, User recipient, Order order)
        {
            return new Message
            {
                DateTime = dateTime,
                Text = message,
                Sender = sender,
                Recipient = recipient,
                Order = order
            };
        }

        //Сообщение от клиента архитектору
        public async Task Send(string message, string orderId)
        {
            DateTime cur_time = DateTime.Now;
            if (string.IsNullOrWhiteSpace(message))
                return;

            message = message.TrimEnd('\n').Replace("\n", "<br />");

            if (await _user_manager.FindByNameAsync(Context.User.Identity.Name) is User user)
            {
                Order order = await _db.Orders.Where(o => o.Id == Convert.ToInt32(orderId)).Include(e => e.Executor).FirstOrDefaultAsync();

                User arch = await _db.Users.Where(u => u.PhoneNumber == _arch_number).Include(u => u.Profile).FirstOrDefaultAsync();
                if (OnExecution(order))
                {
                    UserProfile userProfile = await _db.UserProfiles.Where(u => u.Id == order.Executor.Id).Include(u => u.User).FirstOrDefaultAsync();
                    User executor = userProfile.User;
                    Message temp_message = CreateMessage(cur_time, message, user, executor, order);
                    if (_online_userId.Contains(executor.Id))
                        temp_message.IsReaded = true;

                    await _db.Messages.AddAsync(temp_message);
                    await _db.SaveChangesAsync();

                    if (executor.Profile.CurrentOrderId == Convert.ToInt32(orderId))
                    {
                        await Clients.User(executor.Id).SendAsync("Send", new { IsMe = 0, Name = user.UName, Message = temp_message.Text, When = temp_message.DateTime });
                    }

                    await Clients.User(user.Id).SendAsync("Send", new { IsMe = 1, Name = user.UName, Message = temp_message.Text, When = temp_message.DateTime });
                }
                else
                {
                    Message temp_message = CreateMessage(cur_time, message, user, arch, order);

                    if (_online_userId.Contains(arch.Id))
                        temp_message.IsReaded = true;

                    await _db.Messages.AddAsync(temp_message);
                    await _db.SaveChangesAsync();

                    if (arch.Profile.CurrentOrderId == Convert.ToInt32(orderId))
                    {
                        await Clients.User(arch.Id).SendAsync("Send", new { IsMe = 0, Name = user.UName, Message = temp_message.Text, When = temp_message.DateTime });
                    }

                    await Clients.User(user.Id).SendAsync("Send", new { IsMe = 1, Name = user.UName, Message = temp_message.Text, When = temp_message.DateTime });
                }
           
            } 
        }

        //Сообщение от Архитектора к Клиенту
        public async Task SendToClient(string message, string userId, string orderId)
        {
            DateTime cur_time = DateTime.Now;
            if (string.IsNullOrWhiteSpace(message))
                return;

            message = message.TrimEnd('\n').Replace("\n", "<br />");

            if (await _user_manager.FindByNameAsync(userId) is User user)
            {
                await _db.Entry(user).Reference(u => u.Profile).LoadAsync();
                Order order = await _db.Orders.Where(o => o.Id == Convert.ToInt32(orderId)).Include(e => e.Executor).FirstOrDefaultAsync();

                User arch = await _user_manager.FindByNameAsync(Context.User.Identity.Name);
                if (arch.PhoneNumber == _arch_number)
                {
                    Message temp_message = CreateMessage(cur_time, message, arch, user, order);

                    if (_online_userId.Contains(user.Id))
                        temp_message.IsReaded = true;

                    _db.Messages.Add(temp_message);
                    _db.SaveChanges();

                    if (user.Profile.CurrentOrderId == Convert.ToInt32(orderId))
                    {
                        await Clients.User(user.Id).SendAsync("Send", new { IsMe = 0, Name = user.UName, Message = temp_message.Text, When = temp_message.DateTime });
                    }
                    
                    await Clients.User(arch.Id).SendAsync("Send", new { IsMe = 1, Name = user.UName, Message = temp_message.Text, When = temp_message.DateTime });
                }            
                else if (OnExecution(order))
                {
                    UserProfile userProfile = await _db.UserProfiles.Where(u => u.Id == order.Executor.Id).Include(u => u.User).FirstOrDefaultAsync();
                    User executor = userProfile.User;
                    Message temp_message = CreateMessage(cur_time, message, executor, user, order);
                    if (_online_userId.Contains(user.Id))
                        temp_message.IsReaded = true;

                    await _db.Messages.AddAsync(temp_message);
                    await _db.SaveChangesAsync();

                    if (user.Profile.CurrentOrderId == Convert.ToInt32(orderId))
                    {
                        await Clients.User(user.Id).SendAsync("Send", new { IsMe = 0, Name = user.UName, Message = temp_message.Text, When = temp_message.DateTime });
                    }

                    await Clients.User(executor.Id).SendAsync("Send", new { IsMe = 1, Name = user.UName, Message = temp_message.Text, When = temp_message.DateTime });
                }
             
            }
        }

        //Получение сообщений Архитектором от конкретного клиента
        public async Task GetCustomerMessages(string id, string orderId)
        {
            if(await _user_manager.FindByNameAsync(id) is User user)
            {
                Order order = await _db.Orders.Where(o => o.Id == Convert.ToInt32(orderId)).Include(e => e.Executor).FirstOrDefaultAsync();

                User arch = await _user_manager.FindByNameAsync(Context.User.Identity.Name);
                

                if (arch.PhoneNumber == _arch_number)
                {
                    var messages = (from message in _db.Messages
                                    where ((message.Sender.Id == arch.Id && message.Recipient.Id == id) 
                                       || (message.Sender.Id == id && message.Recipient.Id == arch.Id))
                                       && (message.Order.Id == order.Id)
                                    orderby message.DateTime
                                    select message)
                                   .Include(message => message.Sender)
                                   .Include(message => message.Recipient);

                    foreach (var message in messages)
                    {
                        if (message.Sender.Id != arch.Id)
                            message.IsReaded = true;

                        //await Clients.User(user.Id).SendAsync("Send", new { IsMe = 1, Name = user.UName, Message = message.Text, When = message.DateTime });
                        // await Clients.User(arch.Id).SendAsync("Send", new { IsMe = 0, Name = user.UName, Message = message.Text, When = message.DateTime });

                        await Clients.User(arch.Id).SendAsync("Send", new { IsMe = message.Sender.Id == arch.Id ? 1 : 0, Name = user.UName, Message = message.Text, When = message.DateTime });
                    }

                    _db.SaveChanges();
                }
                else if(OnExecution(order))
                {
                    UserProfile userProfile = await _db.UserProfiles.Where(u => u.Id == order.Executor.Id).Include(u => u.User).FirstOrDefaultAsync();
                    User executor = userProfile.User;

                    var messages = (from message in _db.Messages
                                    where ((message.Sender.Id == executor.Id && message.Recipient.Id == id) 
                                       || (message.Sender.Id == id && message.Recipient.Id == executor.Id))
                                       && (message.Order.Id == order.Id)
                                    orderby message.DateTime
                                    select message)
                                   .Include(message => message.Sender)
                                   .Include(message => message.Recipient);
                    foreach (var message in messages)
                    {
                        if (message.Sender.Id != executor.Id)
                            message.IsReaded = true;

                        //await Clients.User(user.Id).SendAsync("Send", new { IsMe = 1, Name = user.UName, Message = message.Text, When = message.DateTime });
                        // await Clients.User(arch.Id).SendAsync("Send", new { IsMe = 0, Name = user.UName, Message = message.Text, When = message.DateTime });

                        await Clients.User(executor.Id).SendAsync("Send", new { IsMe = message.Sender.Id == executor.Id ? 1 : 0, Name = user.UName, Message = message.Text, When = message.DateTime });
                    }

                    _db.SaveChanges();
                }               
            }
        }

        public async Task GetMessagesForCustomer(string orderId)
        {
            User user = await _user_manager.FindByNameAsync(Context.User.Identity.Name);
            var messanges = (from message in _db.Messages
                             where (message.Recipient.Id == user.Id || message.Sender.Id == user.Id)
                                   && (message.Order.Id == Convert.ToInt32(orderId))
                             orderby message.DateTime
                             select message)
                               .Include(message => message.Sender)
                               .Include(message => message.Recipient);

            foreach (var message in messanges)
            {
                if (message.Sender.Id != user.Id)
                    message.IsReaded = true;

                await Clients.User(user.Id).SendAsync("Send", new { IsMe = message.Sender.Id == user.Id ? 1 : 0, Name = user.UName, Message = message.Text, When = message.DateTime });
            }

            _db.SaveChanges();
            await base.OnConnectedAsync();
        }
        //Получить сообщение Клиентом
        public override async Task OnConnectedAsync()
        {
            if (await _user_manager.FindByNameAsync(Context.User.Identity.Name) is User user)
            {
                if (!_online_userId.Contains(user.Id))
                    _online_userId.Add(user.Id);

                if (Context.User.IsInRole("architect"))
                {
                    await Clients.Caller.SendAsync("GetId");
                    return;
                }
                else if (Context.User.IsInRole("executor"))
                {
                    await Clients.Caller.SendAsync("GetId");
                    return;
                }
                else if (Context.User.IsInRole("customer"))
                {
                    await Clients.Caller.SendAsync("GetItCustomer");
                    return;
                }     
            }
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            if(await _user_manager.FindByNameAsync(Context.User.Identity.Name) is User user)
                if (_online_userId.Contains(user.Id))
                    _online_userId.Remove(user.Id);

            await base.OnDisconnectedAsync(exception);
        }

        //Добавить сообщение о брифе
        public static async Task AddBriefMessage(string message, string userId, UserManager<User> userManager, DBApplicationaContext db, Order order)
        {
            DateTime cur_time = DateTime.Now;

            if (await userManager.FindByNameAsync(userId) is User user)
            {
                User arch = db.Users.FirstOrDefault(u => u.PhoneNumber == _arch_number);
                Message user_message = new Message
                {
                    DateTime = cur_time,
                    Text = message,
                    Sender = user,
                    Recipient = arch,
                    Order = order
                };
                db.Messages.Add(user_message);

                Message arch_message = new Message
                {
                    DateTime = cur_time,
                    Text = "Спасибо за предоставленный бриф. Скоро свяжусь с Вами и предоставлю варианты дизайна Вашей ванной мечты.",
                    Sender = arch,
                    Recipient = user,
                    Order = order
                };
                db.Messages.Add(arch_message);
                db.SaveChanges();
            }
        }
    }
}
