using DotNetCoreServer.Models;
using Newtonsoft.Json.Linq;
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

        public object GetArticle(string user, string category)
        {
            using(var db = SugarContext.GetInstance())
            {
                var result = db.Queryable<ARTICLE>().OrderBy(a => a.DATETIME_CREATED, OrderByType.Desc)
                    .Where(a => a.USER_CREATED == user && a.STATE == "A")
                    .WhereIF(!string.IsNullOrEmpty(category) && category != "全部", (a)=>a.ARTICLE_CATEGORY.Contains(category))
                    .Select((a)=> new
                    {
                        a.IMG_CODE,
                        a.ID,
                        DATETIME_CREATED = Convert.ToDateTime(a.DATETIME_CREATED.ToString("yyyy-MM-dd HH:mm:ss")),
                        CONTENT = a.CONTENT.Substring(0, 200),
                        a.ARTICLE_NAME,
                        a.ARTICLE_CODE,
                        a.ARTICLE_CATEGORY
                    }).ToList();
                return result;

            }
        }


        public object GetArticlesToPage(string user, string category, int startIndex, int length)
        {
            using (var db = SugarContext.GetInstance())
            {
                var result = db.Queryable<ARTICLE>().OrderBy(a => a.DATETIME_CREATED, OrderByType.Desc)
                    .Where(a => a.USER_CREATED == user && a.STATE == "A")
                    .WhereIF(!string.IsNullOrEmpty(category) && category != "全部", (a) => a.ARTICLE_CATEGORY.Contains(category))
                    .Select((a) => new
                    {
                        a.IMG_CODE,
                        a.ID,
                        DATETIME_CREATED = a.DATETIME_CREATED.ToString("yyyy-MM-dd"),
                        //a.CONTENT,
                        a.ARTICLE_NAME,
                        a.ARTICLE_CODE,
                        a.ARTICLE_CATEGORY
                    }).ToPageList(startIndex, length);
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

        public object GetAllArticle(string user)
        {
            using (var db = SugarContext.GetInstance())
            {
                var result = db.Queryable<ARTICLE>().OrderBy(a => a.DATETIME_CREATED, OrderByType.Desc)
                    .Where(a => a.USER_CREATED == user && a.STATE == "A")
                    .Select((a) => new
                    {
                        a.IMG_CODE,
                        a.ID,
                        CONTENT = a.CONTENT,
                        a.ARTICLE_NAME,
                        a.ARTICLE_CODE,
                        a.ARTICLE_CATEGORY
                    }).ToList();
                return result;

            }
        }

        public object EditArticle(JToken jt)
        {
            var id = jt["ID"]?.ToString();
            var name = jt["NAME"]?.ToString();
            var content = jt["CONTENT"]?.ToString();
            var category = jt["CATEGORY"]?.ToString();
            using (var db = SugarContext.GetInstance())
            {
                var article = db.Queryable<ARTICLE>().Where(x => x.ID == id).ToList().FirstOrDefault();
                article.CONTENT = content;
                article.ARTICLE_NAME = name;
                article.ARTICLE_CATEGORY = category;
                article.DATETIME_MODIFIED = DateTime.Now;
                db.Updateable(article).Where(x=>x.ID == article.ID).ExecuteCommand();
            }
            return true;
        }


        public void Delete(string id)
        {
            using (var db = SugarContext.GetInstance())
            {
                db.Ado.ExecuteCommand($"update ARTICLE set state = 'D', datetime_modified = GetDate() where id = '{id}'");
            }
        }
    }
}
