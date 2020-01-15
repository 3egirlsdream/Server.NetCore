using DotNetCoreServer.Models;
using SqlSugar;
using SugarModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetCoreServer.Domians
{
    public class WriteArticle
    {
        private static WriteArticle _Current;
        public static WriteArticle Current => _Current ?? new WriteArticle();

        public object NewArticle(string title, string content, string user, string category)
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
                    atc.ARTICLE_CATEGORY = category;

                    db.Insertable(atc).ExecuteCommand();
                    db.Ado.CommitTran();
                    return true;
                }
                catch(Exception ex)
                {
                    db.Ado.RollbackTran();
                    throw ex;
                }
            }
        }

        public object GetArticle(string user)
        {
            using(var db = SugarContext.GetInstance())
            {
                var result = db.Queryable<ARTICLE, ARTICLE_CATEGORY>((a, c)=> new object[]
                {
                    JoinType.Inner,
                    a.ARTICLE_CATEGORY == c.CATEGORY_CODE &&
                    a.STATE == c.STATE
                }).OrderBy(a=>a.DATETIME_CREATED, OrderByType.Desc)
                    .Where(a => a.USER_CREATED == user && a.STATE == "A")
                    .Select((a, c)=> new
                    {
                        a.IMG_CODE,
                        a.ID,
                        DATETIME_CREATED = a.DATETIME_CREATED.ToString("yyyy-MM-dd"),
                        a.CONTENT,
                        a.ARTICLE_NAME,
                        a.ARTICLE_CODE,
                        c.CATEGORY_CODE,
                        c.CATEGORY_NAME
                    }).ToList();
                return result;

            }
        }

        public object GetArticleCategory()
        {
            using(var db = SugarContext.GetInstance())
            {
                var res = db.Queryable<ARTICLE_CATEGORY>().ToList();
                return res;
            }
        }

        public object GetArticleConent(string id)
        {
            using (var db = SugarContext.GetInstance())
            {
                var result = db.Queryable<ARTICLE>()
                    .Where(e => e.ID == id).First();
                return result;

            }
        }
    }
}
