using DotNetCoreServer.Models;
using Newtonsoft.Json;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetCoreServer.Domains
{
    public class User
    {
        private static User _Current;
        public static User Current => _Current ?? new User();  

        public void RegistUser(object value)
        {
            using(var db = SugarContext.GetInstance())
            {
                try
                {
                    var res = JsonConvert.DeserializeObject<dynamic>(value.ToString());
                    string username = res.username;
                    string oldpassword = res.oldpassword;
                    string isedit = res.isedit;
                    var info = db.Queryable<SYS_USER>().Where(x => x.USER_NAME == username && x.PASSWORD == oldpassword).ToList().FirstOrDefault();
                    if (isedit == "Y")
                    {
                        if (info == null)
                        {
                            throw new Exception("用户不存在或密码错误");
                        }
                        else
                        {
                            info.DISPLAY_NAME = res.displayname;
                            info.PASSWORD = res.password;
                            info.IMG = res.img;
                            db.Updateable(info).ExecuteCommand();
                        }
                    }
                    else
                    {

                        info = new SYS_USER
                        {
                            ID = Guid.NewGuid().ToString().ToUpper(),
                            DATETIME_CREATED = DateTime.Now,
                            USER_CREATED = "SYS",
                            STATE = "A",
                            USER_NAME = res.username,
                            DISPLAY_NAME = res.displayname,
                            PASSWORD = res.password,
                            IMG = res.img
                        };

                        db.Insertable(info).ExecuteCommand();
                    }
                }
                catch(Exception ex)
                {
                    throw ex;
                }
            }
        }

        public object Login(string user, string pwd)
        {
            using(var db = SugarContext.GetInstance())
            {
                try
                {
                    if (!UserIsExist(user, db))
                    {
                        throw new Exception("用户不存在");
                    }

                    if (!PwdIsRight(user, pwd, db))
                    {
                        throw new Exception("密码错误");
                    }

                    var res = db.Queryable<SYS_USER>()
                        .Where(e => e.USER_NAME == user && e.PASSWORD == pwd).First();

                    if (res != null)
                    {
                        return new
                        {
                            Message = "登录成功",
                            data = res
                        };
                    }
                    else
                    {
                        throw new Exception("账号或密码不正确");
                    }
                }
                catch(Exception ex)
                {
                    throw ex;
                }
            }
           
        }

        public object GetUserInfo(string username)
        {
            using (var db = SugarContext.GetInstance())
            {
                try
                {
                    //if (!UserIsExist(username, db))
                    //{
                    //    throw new Exception("用户不存在");
                    //}


                    var res = db.Queryable<SYS_USER>()
                        .Where(e => e.USER_NAME == username).First();

                    if (res != null)
                    {
                        return res;
                    }
                    else
                    {
                        throw new Exception("账号或密码不正确");
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

        }

        /// <summary>
        /// 用户是否存在
        /// </summary>
        /// <param name="user"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        private bool UserIsExist(string user, SqlSugarClient db)
        {
            var res = db.Queryable<SYS_USER>()
                .Where(e => e.USER_NAME == user)
                .Count();
            return res > 0;
        }

        private bool PwdIsRight(string user, string pwd, SqlSugarClient db)
        {
            var res = db.Queryable<SYS_USER>()
                .Where(e => e.USER_NAME == user && e.PASSWORD == pwd)
                .Count();
            return res > 0;
        }

        public object GetChatRecord(string groupId)
        {
            try
            {
                using(var db = SugarContext.GetInstance())
                {
                    var rs = db.Queryable<CHAT_RECORD>().Where(x => x.GROUP_ID == groupId && x.STATE == "A").ToList().FirstOrDefault();
                    return rs?.CHAR_RECORD;
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public void UpChatRecord(string groupId, string chatRecord)
        {
            try
            {
                using (var db = SugarContext.GetInstance())
                {
                    var rs = db.Queryable<CHAT_RECORD>().Where(x => x.GROUP_ID == groupId && x.STATE == "A").ToList().FirstOrDefault();
                    if(rs == null)
                    {
                        var record = JsonConvert.DeserializeObject<dynamic>(chatRecord);
                        var records = new List<dynamic>();
                        records.Add(record);
                        rs = new CHAT_RECORD
                        {
                            ID = Guid.NewGuid().ToString("N").ToUpper(),
                            DATETIME_CREATED = DateTime.Now,
                            USER_CREATED = "SYS",
                            STATE = "A",
                            GROUP_ID = groupId,
                            CHAR_RECORD = JsonConvert.SerializeObject(records)
                        };
                        db.Insertable(rs).ExecuteCommand();
                    }
                    else
                    {
                        try
                        {
                            var record = JsonConvert.DeserializeObject<List<dynamic>>(rs.CHAR_RECORD);
                            var cr = JsonConvert.DeserializeObject<dynamic>(chatRecord);
                            record.Add(cr);
                            if(record.Count > 200)
                            {
                                record = record.Skip(200).Take(record.Count - 200).ToList();
                            }
                            rs.CHAR_RECORD = JsonConvert.SerializeObject(record);
                            db.Updateable(rs).ExecuteCommand();
                        }
                        catch
                        {
                            var record = JsonConvert.DeserializeObject<dynamic>(rs.CHAR_RECORD);
                            var cr = JsonConvert.DeserializeObject<dynamic>(chatRecord);
                            var records = new List<dynamic>();
                            records.Add(record);
                            records.Add(cr);
                            if (records.Count > 200)
                            {
                                records = records.Skip(200).Take(records.Count - 200).ToList();
                            }
                            rs.CHAR_RECORD = JsonConvert.SerializeObject(records);
                            db.Updateable(rs).ExecuteCommand();
                        }
                        
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public object GetChatList(string username)
        {
            try
            {
                using (var db = SugarContext.GetInstance())
                {
                    var rs = db.Queryable<GROUPS>().Where(x =>  x.STATE == "A").ToList();
                    var list = new List<GROUPS>();
                    var group = rs.Where(c => c.GROUP_NAME != "friend").ToList();
                    foreach(var ds in group)
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
                    foreach(var ds in friend)
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
                                    GROUP_NAME = friend_info?.DISPLAY_NAME
                                };
                                list.Add(m);
                            }
                        }
                    }
                    return list;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public void UpdateList(string groupId, string groupName, string users)
        {

            using (var db = SugarContext.GetInstance())
            {
                var g = db.Queryable<GROUPS>().Where(x => x.GROUP_ID == groupId).ToList().FirstOrDefault();
                if (g == null)
                {
                    g = new GROUPS()
                    {
                        ID = Guid.NewGuid().ToString("N").ToUpper(),
                        DATETIME_CREATED = DateTime.Now,
                        USER_CREATED = "SYS",
                        STATE = "A",
                        GROUP_ID = groupId,
                        GROUP_NAME = groupName,
                        USERS = users
                    };
                    db.Insertable(g).ExecuteCommand();
                }
                else
                {
                    var us = g.USERS.Split(';').Distinct().ToList() ;
                    if (!us.Contains(users))
                    {
                        us.Add(users);
                    }
                    g.GROUP_NAME = groupName;
                    g.USERS = string.Join(';', us);
                    db.Updateable(g).ExecuteCommand();
                }
            }
        }
    }
}
