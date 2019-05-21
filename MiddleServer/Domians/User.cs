using MiddleServer.Models;
using Newtonsoft.Json;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiddleServer.Domains
{
    public class User
    {
        private static User _Current;
        public static User Current => _Current ?? new User();  

        public void RegistUser(object value)
        {
            using(var db = SugarContext.GetInstance())
            {
                var res = JsonConvert.DeserializeObject<dynamic>(value.ToString());

                var info = new SYS_USER
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

        public object Login(string user, string pwd)
        {
            using(var db = SugarContext.GetInstance())
            {
                if (!UserIsExist(user, db))
                {
                    return new
                    {
                        success = false,
                        content = "用户不存在"
                    };
                }

                if (!PwdIsRight(user, pwd, db))
                {
                    return new
                    {
                        success = false,
                        content = "密码错误"
                    };
                }

                var res = db.Queryable<SYS_USER>()
                    .Where(e => e.USER_NAME == user && e.PASSWORD == pwd).Count();
                
                if(res > 0)
                {
                    return new
                    {
                        success = true,
                        content = "登录成功"
                    };
                }
                else
                {
                    return new
                    {
                        success = false,
                        content = "账号或密码不正确"
                    };
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
    }
}
