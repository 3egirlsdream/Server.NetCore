using MiddleServer.Models;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiddleServer
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
                ConnectionString = DB.SqlString, // "Data Source=47.107.186.141;Initial Catalog=db;User ID=sa;Password=jiangxinji.123",
                DbType = DbType.SqlServer,//设置数据库类型
                IsAutoCloseConnection = true,//自动释放数据务，如果存在事务，在事务结束后释放
                InitKeyType = InitKeyType.Attribute //从实体特性中读取主键自增列信息
            });
            return db;
        }
    }
}
