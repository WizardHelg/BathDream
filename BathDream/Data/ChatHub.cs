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
        UserManager<User> _user_manager;
        DBApplicationaContext _db;
        List<string> _online_userId = new List<string>();

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

        public Message CreateMessage(DateTime dateTime, string message, User sender, User recipient)
        {
            return new Message
            {
                DateTime = dateTime,
                Text = message,
                Sender = sender,
                Recipient = recipient
            };
        }

        //Сообщение от клиента архитектору
        public async Task Send(string message)
        {
            DateTime cur_time = DateTime.Now;
            if (string.IsNullOrWhiteSpace(message))
                return;

            message = message.TrimEnd('\n').Replace("\n", "<br />");

            if (await _user_manager.FindByNameAsync(Context.User.Identity.Name) is User user)
            {
                Order order = await GetOrder(user.Id);

                User arch = await _user_manager.FindByNameAsync(Context.User.Identity.Name);
                if (arch.PhoneNumber == _arch_number)
                {
                    Message temp_message = CreateMessage(cur_time, message, user, arch);

                    if (_online_userId.Contains(arch.Id))
                        temp_message.IsReaded = true;

                    await _db.Messages.AddAsync(temp_message);
                    await _db.SaveChangesAsync();

                    await Clients.User(user.Id).SendAsync("Send", new { IsMe = 1, Name = user.UName, Message = temp_message.Text, When = temp_message.DateTime });
                    await Clients.User(arch.Id).SendAsync("Send", new { IsMe = 0, Name = user.UName, Message = temp_message.Text, When = temp_message.DateTime });
                }
                else if (OnExecution(order))
                {
                    UserProfile userProfile = await _db.UserProfiles.Include(u => u.User).FirstOrDefaultAsync(u => u.Id == order.Executor.Id);
                    User executor = userProfile.User;
                    Message temp_message = CreateMessage(cur_time, message, user, executor);
                    if (_online_userId.Contains(executor.Id))
                        temp_message.IsReaded = true;

                    await _db.Messages.AddAsync(temp_message);
                    await _db.SaveChangesAsync();

                    await Clients.User(user.Id).SendAsync("Send", new { IsMe = 1, Name = user.UName, Message = temp_message.Text, When = temp_message.DateTime });
                    await Clients.User(executor.Id).SendAsync("Send", new { IsMe = 0, Name = user.UName, Message = temp_message.Text, When = temp_message.DateTime });
                }
           
            } 
        }

        //Сообщение от Архитектора к Клиенту
        public async Task SendToClient(string message, string userId)
        {
            DateTime cur_time = DateTime.Now;
            if (string.IsNullOrWhiteSpace(message))
                return;

            message = message.TrimEnd('\n').Replace("\n", "<br />");

            if (await _user_manager.FindByNameAsync(userId) is User user)
            {
                Order order = await GetOrder(user.Id);

                User arch = await _user_manager.FindByNameAsync(Context.User.Identity.Name);
                if (arch.PhoneNumber == _arch_number)
                {
                    Message temp_message = CreateMessage(cur_time, message, arch, user);

                    if (_online_userId.Contains(user.Id))
                        temp_message.IsReaded = true;

                    _db.Messages.Add(temp_message);
                    _db.SaveChanges();

                    await Clients.User(user.Id).SendAsync("Send", new { IsMe = 0, Name = user.UName, Message = temp_message.Text, When = temp_message.DateTime });
                    await Clients.User(arch.Id).SendAsync("Send", new { IsMe = 1, Name = user.UName, Message = temp_message.Text, When = temp_message.DateTime });
                }            
                else if (OnExecution(order))
                {
                    UserProfile userProfile = await _db.UserProfiles.Include(u => u.User).FirstOrDefaultAsync(u => u.Id == order.Executor.Id);
                    User executor = userProfile.User;
                    Message temp_message = CreateMessage(cur_time, message, executor, user);
                    if (_online_userId.Contains(user.Id))
                        temp_message.IsReaded = true;

                    await _db.Messages.AddAsync(temp_message);
                    await _db.SaveChangesAsync();

                    await Clients.User(user.Id).SendAsync("Send", new { IsMe = 0, Name = user.UName, Message = temp_message.Text, When = temp_message.DateTime });
                    await Clients.User(executor.Id).SendAsync("Send", new { IsMe = 1, Name = user.UName, Message = temp_message.Text, When = temp_message.DateTime });
                }
             
            }
        }

        //Получение сообщений Архитектором от конкретного клиента
        public async Task GetCustomerMessages(string id)
        {
            if(await _user_manager.FindByNameAsync(id) is User user)
            {
                Order order = await GetOrder(user.Id);

                User arch = await _user_manager.FindByNameAsync(Context.User.Identity.Name);

                if (arch.PhoneNumber == _arch_number)
                {
                    var messages = (from message in _db.Messages
                                    where message.Sender.Id == id || message.Recipient.Id == id
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
                    UserProfile userProfile = await _db.UserProfiles.Include(u => u.User).FirstOrDefaultAsync(u => u.Id == order.Executor.Id);
                    User executor = userProfile.User;

                    var messages = (from message in _db.Messages
                                    where message.Sender.Id == id || message.Recipient.Id == id
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

                var messanges = (from message in _db.Messages
                                    where message.Recipient.Id == user.Id || message.Sender.Id == user.Id
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
                
            }         
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            if(await _user_manager.FindByNameAsync(Context.User.Identity.Name) is User user)
                if (_online_userId.Contains(user.Id))
                    _online_userId.Remove(user.Id);

            await base.OnDisconnectedAsync(exception);
        }

        //Добавить сообщение о брифе
        public static async Task AddBriefMessage(string message, string userId, UserManager<User> userManager, DBApplicationaContext db)
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
                };
                db.Messages.Add(user_message);

                Message arch_message = new Message
                {
                    DateTime = cur_time,
                    Text = "Спасибо за предоставленный бриф. Скоро свяжусь с Вами и предоставлю варианты дизайна Вашей ванной мечты.",
                    Sender = arch,
                    Recipient = user
                };
                db.Messages.Add(arch_message);
                db.SaveChanges();
            }
        }
    }
}
