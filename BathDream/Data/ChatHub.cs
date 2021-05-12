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

        //Сообщение от клиента архитектору
        public async Task Send(string message)
        {
            DateTime cur_time = DateTime.Now;
            if (string.IsNullOrWhiteSpace(message))
                return;

            message = message.TrimEnd('\n').Replace("\n", "<br />");

            if (await _user_manager.FindByNameAsync(Context.User.Identity.Name) is User user)
            {
                User arch = _db.Users.FirstOrDefault(u => u.PhoneNumber == _arch_number);
                Message temp_message = new Message
                {
                    DateTime = cur_time,
                    Text = message,
                    Sender = user,
                    Recipient = arch
                };

                if (_online_userId.Contains(arch.Id))
                    temp_message.IsReaded = true;

                await _db.Messages.AddAsync(temp_message);
                await _db.SaveChangesAsync();

                await Clients.User(user.Id).SendAsync("Send", $"Вы: {temp_message.Text}");
                await Clients.User(arch.Id).SendAsync("Send", $"{user.UName} {user.UFamaly}: {temp_message.Text}");
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
                User arch = _db.Users.FirstOrDefault(u => u.PhoneNumber == _arch_number);
                Message temp_message = new Message
                {
                    DateTime = cur_time,
                    Text = message,
                    Sender = arch,
                    Recipient = user,
                };

                if (_online_userId.Contains(user.Id))
                    temp_message.IsReaded = true;

                _db.Messages.Add(temp_message);
                _db.SaveChanges();

                await Clients.User(user.Id).SendAsync("Send", $"Архитектор: {temp_message.Text}");
                await Clients.User(arch.Id).SendAsync("Send", $"Вы: {temp_message.Text}");
            }
        }

        //Получение сообщений Архитектором от конкретного клиента
        public async Task GetCustomerMessages(string id)
        {
            if(await _user_manager.FindByNameAsync(id) is User user)
            {
                User arch = _db.Users.FirstOrDefault(u => u.PhoneNumber == _arch_number);
                var messages = from message in _db.Messages
                               where message.Sender.Id == id || message.Recipient.Id == id
                               orderby message.DateTime
                               select message;

                foreach (var message in messages)
                {
                    if (message.Sender.Id != arch.Id)
                        message.IsReaded = true;

                    await Clients.User(arch.Id).SendAsync("Send", $"{(message.Sender.Id == user.Id ? $"{user.UName} {user.UFamaly}" : "Вы")}: {message.Text}");
                }

                _db.SaveChanges();
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

                var messanges = from message in _db.Messages
                                where message.Recipient.Id == user.Id || message.Sender.Id == user.Id
                                orderby message.DateTime
                                select message;

                foreach (var message in messanges)
                {
                    if(message.Sender.Id != user.Id) 
                        message.IsReaded = true;

                    await Clients.User(user.Id).SendAsync("Send", $"{(message.Sender.Id == user.Id ? "Вы" : "Архитектор")}: {message.Text}");
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
                Message temp_message = new Message
                {
                    DateTime = cur_time,
                    Text = message,
                    Sender = user,
                    Recipient = arch,
                };
                db.Messages.Add(temp_message);
                db.SaveChanges();
            }
        }
    }
}
