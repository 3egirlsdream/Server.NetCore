using DotNetCoreServer;
using DotNetCoreServer.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.NetCore.Controllers
{
    [ApiController]
    [Middleware("any", "聊天")]
    [Route("api/[controller]/[action]")]
    public class ChatController : ControllerBase
    {

        [HttpGet]
        public object GetRecentChatedFriend(string username)
        {
            try
            {
                using (var db = SugarContext.GetInstance())
                {
                    //获取全部最近聊天记录
                    var records = db.Queryable<NEWEST_CHAT_RECORD>().Where(c=>c.GROUP_ID.Contains(username)).ToList();
                    var dic = new Dictionary<string, string>();
                    foreach(var ds in records)
                    {
                        var users = ds.GROUP_ID.Split(';');
                        var friend_id = users.First(x => x != username);
                        dic[friend_id] = ds.NEWEST_CHAR_RECORD.Replace("\r\n", "").Replace("\n", "").Substring(0, ds.NEWEST_CHAR_RECORD.Length > 20 ? 20 : ds.NEWEST_CHAR_RECORD.Length);
                    }

                    var rs = db.Queryable<GROUPS>().Where(x => x.STATE == "A").ToList();
                    var list = new List<GROUPS>();
                    var group = rs.Where(c => c.GROUP_NAME != "friend").ToList();
                    foreach (var ds in group)
                    {
                        var user = ds.USERS.Split(';');
                        if (user.Contains(username))
                        {
                            var m = new GROUPS
                            {
                                USERS = username,
                                GROUP_ID = ds.GROUP_ID,
                                GROUP_NAME = ds.GROUP_NAME
                            };
                            list.Add(m);
                        }

                    }

                    var friend = rs.Where(x => x.GROUP_NAME == "friend").ToList();
                    foreach (var ds in friend)
                    {
                        var user = ds.USERS.Split(';');
                        if (user.Contains(username))
                        {
                            var friends = ds.GROUP_ID.Split(';').ToList();
                            var friend_id = friends.Where(c => c != username).FirstOrDefault();
                            var friend_info = db.Queryable<SYS_USER>().Where(e => e.USER_NAME == friend_id).ToList().FirstOrDefault();
                            if (friend_info != null)
                            {
                                var m = new GROUPS
                                {
                                    USERS = username,
                                    GROUP_ID = string.Join(';', friends.OrderBy(c => c.Length).OrderBy(c => c[0])),
                                    GROUP_NAME = friend_info?.DISPLAY_NAME,
                                    ID = friend_info?.IMG,
                                    USER_CREATED = dic.ContainsKey(friend_id) ? dic[friend_id] : ""//NEWEST_CHAT_RECORD
                                };
                                list.Add(m);
                            }
                        }
                    }
                    return list.Where(c=>!string.IsNullOrEmpty(c.USER_CREATED)).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
