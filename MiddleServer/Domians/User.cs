using MiddleServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiddleServer.Domians
{
    public class User
    {
        private static User _Current;
        public static User Current => _Current ?? new User();  

        public void RegistUser(string user, string pwd)
        {
            using(var db = SugarContext.GetInstance())
            {
                var info = new SYS_USER
                {
                    ID = Guid.NewGuid().ToString().ToUpper(),
                    DATETIME_CREATED = DateTime.Now,
                    USER_CREATED = "SYS",
                    STATE = "A",
                    USER_NAME = user,
                    DISPLAY_NAME = user,
                    PASSWORD = pwd
                };

                db.Insertable(info).ExecuteCommand();
            }
        }
    }
}
