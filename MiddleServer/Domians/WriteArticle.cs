using MiddleServer.Models;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiddleServer.Domians
{
    public class WriteArticle
    {
        private static WriteArticle _Current;
        public static WriteArticle Current => _Current ?? new WriteArticle();

        public object newArticle(string title, string content, string user)
        {
            using(var db = SugarContext.GetInstance())
            {
                db.Ado.BeginTran();
                try
                {
                    ARTICLE atc = new ARTICLE();
                    atc.ID = Guid.NewGuid().ToString().ToUpper();
                    atc.USER_CREATED = user;
                    atc.DATETIME_CREATED = DateTime.Now;
                    atc.ARTICLE_CODE = atc.ID;
                    atc.ARTICLE_NAME = title;
                    atc.CONTENT = content;

                    db.Insertable(atc).ExecuteCommand();
                    db.Ado.CommitTran();
                    return new
                    {
                        success = true
                    };
                }
                catch(Exception ex)
                {
                    db.Ado.RollbackTran();
                    return new
                    {
                        success = false,
                        content = ex.Message
                    };
                }
            }
        }

        public object getArticle(string user)
        {
            using(var db = SugarContext.GetInstance())
            {
                var result = db.Queryable<ARTICLE>()
                    .OrderBy(e=>e.DATETIME_CREATED, OrderByType.Desc)
                    .Where(e => e.USER_CREATED == user).ToList();
                return new
                {
                    success = true,
                    data = result
                };

            }
        }

        public object getArticleConent(string id)
        {
            using (var db = SugarContext.GetInstance())
            {
                var result = db.Queryable<ARTICLE>()
                    .Where(e => e.ID == id).First();
                return new
                {
                    success = true,
                    data = result
                };

            }
        }
    }
}
