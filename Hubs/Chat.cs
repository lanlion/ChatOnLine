using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace lan.Hubs
{
    public class Chat : Hub
    {
        public static List<UserInfo> OnlineUsers = new List<UserInfo>();
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
           

            // await Clients.All.InvokeAsync("Send", $"您的id是：{Context.ConnectionId} ");

        }

        public override async Task OnDisconnectedAsync(Exception ex)
        {
          //  await base.OnDisconnectedAsync(ex);
            await Clients.All.InvokeAsync("Send", $"{Context.ConnectionId} left");
        }

        public Task Send(string message)
        {
            
            return Clients.All.InvokeAsync("Send", $"{Context.ConnectionId}: {message}");
        }

        public Task SendToGroup(string groupName, string message)
        {
            var userinfo = OnlineUsers.FirstOrDefault(user=>user.ConnectionId== Context.ConnectionId);
            if (userinfo == null) return null;
            return Clients.Group(groupName).InvokeAsync("Send", $"{userinfo.UserName}说: {message}");
        }

        public async Task JoinGroup(string groupName,string userName)
        {
            await Groups.AddAsync(Context.ConnectionId, groupName);

            await Clients.Group(groupName).InvokeAsync("Send", $"{userName} 加入聊天室");
            OnlineUsers.Add(new UserInfo
            {
                ConnectionId = Context.ConnectionId,
                UserName = userName,
              
            });
        }

        public async Task LeaveGroup(string groupName)
        {
            await Groups.RemoveAsync(Context.ConnectionId, groupName);
            var userinfo = OnlineUsers.FirstOrDefault(user => user.ConnectionId == Context.ConnectionId);
          
            await Clients.Group(groupName).InvokeAsync("Send", $"{userinfo.UserName} 离开聊天室");
        }

        public Task Echo(string message)
        {
            return Clients.Client(Context.ConnectionId).InvokeAsync("Send", $"{Context.ConnectionId}: {message}");
        }
    }
}
