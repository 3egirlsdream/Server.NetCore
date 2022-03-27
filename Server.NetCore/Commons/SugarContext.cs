using DotNetCoreServer.Models;
using Server.NetCore.Domians;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetCoreServer
{
    public class SugarContext
    {
        private static SqlSugarClient db;
        public SugarContext()
        {
            db = new SqlSugarClient(
            new ConnectionConfig()
            {
                ConnectionString = "Data Source=47.107.186.141;Initial Catalog=db;User ID=sa;Password=jiangxinji.123",
                DbType = DbType.SqlServer,//设置数据库类型
                IsAutoCloseConnection = true,//自动释放数据务，如果存在事务，在事务结束后释放
                InitKeyType = InitKeyType.Attribute //从实体特性中读取主键自增列信息
            });
        }

        public static SqlSugarClient GetInstance()
        {
            var db = new SqlSugarClient(
            new ConnectionConfig()
            {
                ConnectionString = Config.SqlString(), // "Data Source=47.107.186.141;Initial Catalog=db;User ID=sa;Password=jiangxinji.123",
                DbType = DbType.MySql,//设置数据库类型
                IsAutoCloseConnection = true,//自动释放数据务，如果存在事务，在事务结束后释放
                InitKeyType = InitKeyType.Attribute //从实体特性中读取主键自增列信息
            });

            var mq = new RabbitMQDomain();
            db.Aop.OnLogExecuted = (sql, pars) => //SQL执行完事件
            {
                foreach (var p in pars)
                {
                    string s = "";
                    if (p.DbType == System.Data.DbType.String)
                    {
                        s = $"\'{p.Value}\'";
                    }
                    else if (p.DbType == System.Data.DbType.DateTime || p.DbType == System.Data.DbType.Date || p.DbType == System.Data.DbType.DateTime2)
                    {
                        if (string.IsNullOrEmpty(Convert.ToString(p.Value)))
                        {
                            s = $"\'{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}\'";
                        }
                        else
                        {
                            var dt = Convert.ToDateTime(p.Value);
                            s = $"\'{dt.ToString("yyyy-MM-dd HH:mm:ss")}\'";
                        }
                    }
                    else
                    {
                        s = Convert.ToString(p.Value);
                    }
                    if (string.IsNullOrEmpty(s)) s = "\'\'";
                    sql = sql.Replace(p.ParameterName, s);
                }
                mq.Producter(sql);
            };
            try
            {
                db.Open();
            }
            catch
            {
                db = GetInstance2();
            }
            return db;
        }

        public static SqlSugarClient GetInstance2()
        {
            var db = new SqlSugarClient(
            new ConnectionConfig()
            {
                ConnectionString = Config.GetConfig().SqlString2, // "Data Source=47.107.186.141;Initial Catalog=db;User ID=sa;Password=jiangxinji.123",
                DbType = DbType.MySql,//设置数据库类型
                IsAutoCloseConnection = true,//自动释放数据务，如果存在事务，在事务结束后释放
                InitKeyType = InitKeyType.Attribute //从实体特性中读取主键自增列信息
            });
            return db;
        }
    }
}
