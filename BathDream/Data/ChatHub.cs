using BathDream.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BathDream.Data
{
    [Authorize]
    public class ChatHub : Hub
    {
        UserManager<User> _user_manager;
        DBApplicationaContext _db;

        public ChatHub(UserManager<User> userManager, DBApplicationaContext db)
        {
            _user_manager = userManager;
            _db = db;
        }

        public async Task Send(string message)
        {
            DateTime cur_time = DateTime.Now;
            
            if(await _user_manager.FindByNameAsync(Context.User.Identity.Name) is User user)
            {
                User arch = _db.Users.FirstOrDefault(u => u.PhoneNumber == "70000000000");
                Message temp_message = new Message
                {
                    DateTime = cur_time,
                    Text = message,
                    Sender = user,
                    Recipient = arch
                };
                await _db.Messages.AddAsync(temp_message);
                await _db.SaveChangesAsync();

                await Clients.User(user.Id).SendAsync("Send", $"Вы: {temp_message.Text}");
                await Clients.User(arch.Id).SendAsync("Send", $"Архитектор: {temp_message.Text}");

                
            } 
        }

        public async Task SendToClient(string message, string userId)
        {
            DateTime cur_time = DateTime.Now;

            if (await _user_manager.FindByNameAsync(userId) is User user)
            {
                User arch = _db.Users.FirstOrDefault(u => u.PhoneNumber == "70000000000");
                Message temp_message = new Message
                {
                    DateTime = cur_time,
                    Text = message,
                    Sender = arch,
                    Recipient = user
                };
                _db.Messages.Add(temp_message);
                _db.SaveChanges();

                await Clients.User(user.Id).SendAsync($"Клиент: {temp_message.Text}");
                await Clients.User(arch.Id).SendAsync($"Вы: {temp_message.Text}");
            }
        }

        public override async Task OnConnectedAsync()
        {
            if (await _user_manager.FindByNameAsync(Context.User.Identity.Name) is User user)
            {
                string recipient = Context.User.IsInRole("architect") ? "Клиент" : "Архитектор";

                var messanges = from message in _db.Messages
                                where message.Recipient.Id == user.Id || message.Sender.Id == user.Id
                                orderby message.DateTime
                                select new
                                {
                                    Text = $"{(message.Sender.Id == user.Id ? "Вы" : recipient)}: {message.Text}"
                                };

                foreach (var message in messanges)
                    await Clients.User(user.Id).SendAsync("Send", message.Text);
            }

            await base.OnConnectedAsync();
        }
    }
}
