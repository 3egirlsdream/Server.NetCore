using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IM
{
    public class ChatHub : Hub
    {
        public static List<User> UsersId = new List<User>();
        public static List<MsgCache> UnReadMsg = new List<MsgCache>();
        public Task SendMsg(string username, string sendto, string message)
        {
            var id = UsersId.FirstOrDefault(c => c.userName == sendto);
            //表示当前离线，信息保存在缓存中
            if (id == null)
            {
                var msgCache = UnReadMsg.FirstOrDefault(c => c.User?.userName == sendto);
                if (msgCache == null)
                {
                    msgCache = new MsgCache()
                    {
                        User = new User(null, sendto),
                        Msgs = new List<MsgInfo>() { new MsgInfo() { sendFrom = username, Msg = message } }
                    };
                    UnReadMsg.Add(msgCache);
                }
                else
                {
                    msgCache.Msgs.Add(new MsgInfo { sendFrom = username, Msg = message });
                }
                return Task.CompletedTask;
            }
            else //表示在线
            {
                return Clients.Client(id.connectionId).SendAsync("SendMsg", username, message);
            }
        }


        public Task SendMsg2Group(string username, List<string> groupMember, string message)
        {
            var ls = UsersId.Where(c => groupMember.Contains(c.userName)).Select(c => c.connectionId);
            return Clients.Clients(ls).SendAsync("SendMsg2Group", username, message);
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

        [HubMethodName("GetName")]
        public void GetName(string name)
        {
            var user = UsersId.FirstOrDefault(c => c.userName == name);
            if (user != null)
            {
                user.connectionId = Context.ConnectionId;
            }
            else
            {
                UsersId.Add(new User(Context.ConnectionId, name));
            }
        }

        [HubMethodName("GetUnReadMsg")]
        public void GetUnReadMsg(string receiveUser, string sendUser)
        {
            SendUnReadMsg(receiveUser, sendUser);
        }



        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }


        private void SendUnReadMsg(string receiveUser, string sendFrom)
        {
            var id = UsersId.FirstOrDefault(c => c.userName == receiveUser);
            if (id == null) return;
            //首先看有没有未读信息
            var unRead = UnReadMsg.FirstOrDefault(c => c.User?.userName == receiveUser);
            if (unRead != null)
            {
                var waitForSendMsg = unRead.Msgs.Where(c => c.sendFrom == sendFrom);
                if (waitForSendMsg.Any())
                {
                    foreach (var item in waitForSendMsg)
                    {
                        Clients.Client(id.connectionId).SendAsync("SendMsg", sendFrom, item.Msg);
                    }
                }
                unRead.Msgs.RemoveAll(c => c.sendFrom == sendFrom);
                if (unRead.Msgs.Count == 0)
                {
                    //清除历史记录
                    UnReadMsg.Remove(unRead);
                }
            }
        }

    }


    public class User
    {

        public User(string id, string user)
        {
            connectionId = id;
            userName = user;
        }

        public string connectionId { get; set; }
        public string userName { get; set; }

    }

    public class MsgCache
    {
        public User User { get; set; }
        public List<MsgInfo> Msgs { get; set; }

        public MsgCache()
        {
            Msgs = new List<MsgInfo>();
        }
    }

    public class MsgInfo
    {
        public string sendFrom { get; set; }
        public string Msg { get; set; }
    }

}
