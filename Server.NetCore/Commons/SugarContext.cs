using DotNetCoreServer.Models;
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
                DbType = DbType.SqlServer,//设置数据库类型
                IsAutoCloseConnection = true,//自动释放数据务，如果存在事务，在事务结束后释放
                InitKeyType = InitKeyType.Attribute //从实体特性中读取主键自增列信息
            });

            db.Aop.OnLogExecuted = (sql, pars) => //SQL执行完事件
            {
                foreach (var p in pars)
                {
                    string s = "";
                    if (p.Value is string)
                    {
                        s = $"\'{p.Value}\'";
                    }
                    else if(p.Value is DateTime)
                    {
                        s = $"\'{p.Value}\'";
                    }
                    else
                    {
                        s = Convert.ToString(p.Value);
                    }
                    if (string.IsNullOrEmpty(s)) s = "\'\'";
                    sql = sql.Replace(p.ParameterName, s);
                }
                Debug.WriteLine(sql);
            };
            return db;
        }
    }
}
