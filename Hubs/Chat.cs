using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace lan.Hubs
{
    public class Chat : Hub
    {
        public override Task OnConnected()
        {
            Clients.All.Invoke("Send", $"{Context.ConnectionId} joined");
            return base.OnConnected();
        }
        public override Task OnDisconnected(bool stopCalled)
        {
            Clients.All.Invoke("Send", $"{Context.ConnectionId} left");
            return base.OnDisconnected(stopCalled);
        }
        public Task Send(string message)
        {
            return Clients.All.Invoke("Send", $"{Context.ConnectionId}: {message}");
        }
        public Task SendToGroup(string groupName, string messgae)
        {

            return Clients.Group(groupName).Invoke();
        }
        public Task JoinGroup(string groupName)
        {

            Groups.Add(Context.ConnectionId, groupName);
            return Clients.Group(groupName).Invoke("Send", $"{Context.ConnectionId} joined {groupName}");
        }
        public Task LeaveGroup(string groupName)
        {
            Groups.Remove(Context.ConnectionId, groupName);
            return Clients.Group(groupName).Invoke("Send", $"{Context.ConnectionId} left {groupName}");
        }
        public Task Echo(string message)
        {
            return Clients.Client(Context.ConnectionId).Invoke("Send", $"{Context.ConnectionId}: {message}");
        }
    }
}
