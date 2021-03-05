using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IM
{
    public class ChatHub:Hub
    {
        public List<string> Users = new List<string>();
        public Task SendMsg(string username,string message)
        {
            return Clients.All.SendAsync("Show", username ,message);
        }

        public Task ComeIn(string displayname, string room_number)
        {
            return Clients.All.SendAsync("ComeIn", displayname, room_number);
        }

        public Task PlaceOrder(string orderId, string displagname, string username, string content)
        {
            return Clients.All.SendAsync("ShowOrder", orderId, displagname, username, content);
        }
        
        public Task AddFriend(string applyPeople, string to)
        {
            return Clients.All.SendAsync("AddFriend", applyPeople, to);
        }

        
    }
}
