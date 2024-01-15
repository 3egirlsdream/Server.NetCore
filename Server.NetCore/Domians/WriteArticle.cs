using DotNetCoreServer.Models;
using Newtonsoft.Json.Linq;
using Server.NetCore.Models;
using SqlSugar;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetCoreServer.Domians
{
    public class WriteArticle
    {
        private static WriteArticle _Current;
        public static WriteArticle Current => _Current ?? new WriteArticle();

        public object NewArticle(string title, string content, string user, string category, string last, string next)
        {
            using(var db = SugarContext.GetInstance())
            {
                db.Ado.BeginTran();
                try
                {
                    ARTICLE atc = new ARTICLE();
                    atc.ID = string.Join("", Guid.NewGuid().ToString().ToUpper().Where(c => char.IsDigit(c)));
                    atc.USER_CREATED = user;
                    atc.DATETIME_CREATED = DateTime.Now;
                    atc.ARTICLE_CODE = atc.ID;
                    atc.ARTICLE_NAME = title;
                    atc.CONTENT = content;
                    atc.ARTICLE_CATEGORY = category;
                    atc.LAST_ESSAY = last;
                    atc.NEXT_ESSAY = next;

                    db.Insertable(atc).ExecuteCommand();

                    var lastEssay = db.Queryable<ARTICLE>().Where(x => x.ID == last).ToList().FirstOrDefault();
                    if(lastEssay != null)
                    {
                        lastEssay.NEXT_ESSAY = atc.ID;
                        db.Updateable(lastEssay).ExecuteCommand();
                    }

                    var nextEssay = db.Queryable<ARTICLE>().Where(x => x.ID == next).ToList().FirstOrDefault();
                    if(nextEssay != null)
                    {
                        nextEssay.LAST_ESSAY = atc.ID;
                        db.Updateable(nextEssay).ExecuteCommand();
                    }

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
                var query = db.Queryable<ARTICLE, ARTICLE, ARTICLE>((a, a2, a3) => new
                (
                    JoinType.Left, a.LAST_ESSAY == a2.ID,
                    JoinType.Left, a.NEXT_ESSAY == a3.ID
                )).OrderBy((a, a2, a3) => a.DATETIME_CREATED, OrderByType.Desc)
                    .Where((a, a2, a3) => a.USER_CREATED == user && a.STATE == "A")
                    .WhereIF(!string.IsNullOrEmpty(category) && category != "全部", (a, a2, a3) => a.ARTICLE_CATEGORY.Contains(category))
                    .Select((a, a2, a3) => new
                    {
                        a.IMG_CODE,
                        a.ID,
                        DATETIME_CREATED = Convert.ToDateTime(a.DATETIME_CREATED.ToString("yyyy-MM-dd HH:mm:ss")),
                        CONTENT = a.CONTENT.Substring(0, 200),
                        a.ARTICLE_NAME,
                        a.ARTICLE_CODE,
                        a.ARTICLE_CATEGORY,
                        a.LAST_ESSAY,
                        LAST_ESSAY_NAME = a2.ARTICLE_NAME,
                        a.NEXT_ESSAY,
                        NEXT_ESSAY_NAME = a3.ARTICLE_NAME
                    });
                var result = query.ToList();
                return result;

            }
        }


        public object GetArticlesToPage(string user, string category, int startIndex, int length)
        {
            using (var db = SugarContext.GetInstance())
            {
                int total = 0;
                var result = db.Queryable<ARTICLE, ARTICLE, ARTICLE, SYS_USER>((a, a2, a3, su) => new
                (
                    JoinType.Left, a.LAST_ESSAY == a2.ID,
                    JoinType.Left, a.NEXT_ESSAY == a3.ID,
                    JoinType.Left, a.USER_CREATED == su.USER_NAME
                )).OrderBy((a, a2, a3) => a.DATETIME_CREATED, OrderByType.Desc)
                    .Where((a, a2, a3) => a.USER_CREATED == user && a.STATE == "A")
                    .WhereIF(!string.IsNullOrEmpty(category) && category != "全部", (a, a2, a3) => a.ARTICLE_CATEGORY.Contains(category))
                    .Select((a, a2, a3, su) => new
                    {
                        a.IMG_CODE,
                        a.ID,
                        su.DISPLAY_NAME,
                        su.IMG,
                        DATETIME_CREATED = a.DATETIME_CREATED.ToString("yyyy-MM-dd"),
                        CONTENT = a.CONTENT,
                        a.ARTICLE_NAME,
                        a.ARTICLE_CODE,
                        a.ARTICLE_CATEGORY,
                        a.LAST_ESSAY,
                        LAST_ESSAY_NAME = a2.ARTICLE_NAME,
                        a.NEXT_ESSAY,
                        NEXT_ESSAY_NAME = a3.ARTICLE_NAME
                    }).ToPageList(startIndex, length, ref total);
                return new
                {
                    totalCount = total,
                    data = result
                };

            }
        }

        public object Search(string parameter)
        {
            using (var db = SugarContext.GetInstance())
            {
                var result = db.Queryable<ARTICLE, ARTICLE, ARTICLE, SYS_USER>((a, a2, a3, su) => new
                (
                    JoinType.Left, a.LAST_ESSAY == a2.ID,
                    JoinType.Left, a.NEXT_ESSAY == a3.ID,
                    JoinType.Left, a.USER_CREATED == su.USER_NAME
                )).OrderBy((a, a2, a3) => a.DATETIME_CREATED, OrderByType.Desc)
                    .Where((a, a2, a3, su) =>  a.STATE == "A" 
                    && (a.USER_CREATED == parameter 
                    || a.CONTENT.Contains(parameter) 
                    || a.ARTICLE_NAME.Contains(parameter) 
                    || su.DISPLAY_NAME.Contains(parameter)
                    ))
                    .Select((a, a2, a3, su) => new
                    {
                        a.IMG_CODE,
                        a.ID,
                        su.DISPLAY_NAME,
                        su.IMG,
                        DATETIME_CREATED = a.DATETIME_CREATED.ToString("yyyy-MM-dd"),
                        CONTENT = a.CONTENT,
                        a.ARTICLE_NAME,
                        a.ARTICLE_CODE,
                        a.ARTICLE_CATEGORY,
                        a.LAST_ESSAY,
                        LAST_ESSAY_NAME = a2.ARTICLE_NAME,
                        a.NEXT_ESSAY,
                        NEXT_ESSAY_NAME = a3.ARTICLE_NAME
                    }).ToList();
                return result;

            }
        }

        public object GetArticleCategory()
        {
            using(var db = SugarContext.GetInstance())
            {
                var res = db.Queryable<ARTICLE>().Where(x=> !string.IsNullOrEmpty(x.ARTICLE_CATEGORY) && x.STATE == "A").Select(c => c.ARTICLE_CATEGORY).ToList() ;
                var ls = new List<string>();
                res.ForEach(c =>
                {
                    c.Split(';').ToList().ForEach(x => ls.Add(x));
                });
                return ls.Distinct().ToList();
            }
        }

        public object GetArticleConent(string id)
        {
            using (var db = SugarContext.GetInstance())
            {
                var result = db.Queryable<ARTICLE, ARTICLE, ARTICLE>((a, a2, a3) => new
                (
                    JoinType.Left, a.LAST_ESSAY == a2.ID && a.STATE == a2.STATE,
                    JoinType.Left, a.NEXT_ESSAY == a3.ID && a.STATE == a3.STATE
                )).Where((a, a2, a3) => a.ID == id && a.STATE == "A").Select((a, a2, a3)=>new
                {
                    a.IMG_CODE,
                    a.ID,
                    DATETIME_CREATED = Convert.ToDateTime(a.DATETIME_CREATED.ToString("yyyy-MM-dd HH:mm:ss")),
                    a.CONTENT,
                    a.ARTICLE_NAME,
                    a.ARTICLE_CODE,
                    a.ARTICLE_CATEGORY,
                    LAST_ESSAY = a2.ID,
                    LAST_ESSAY_NAME = a2.ARTICLE_NAME,
                    NEXT_ESSAY = a3.ID,
                    NEXT_ESSAY_NAME = a3.ARTICLE_NAME
                }).First();
                return result;

            }
        }

        public object GetAllArticle(string user)
        {
            using (var db = SugarContext.GetInstance())
            {
                var result = db.Queryable<ARTICLE, ARTICLE, ARTICLE>((a, a2, a3)=> new
                (
                    JoinType.Left, a.LAST_ESSAY == a2.ID,
                    JoinType.Left, a.NEXT_ESSAY == a3.ID
                )).OrderBy((a, a2, a3) => a.DATETIME_CREATED, OrderByType.Desc)
                    .Where((a, a2, a3) => a.USER_CREATED == user && a.STATE == "A")
                    .Select((a, a2, a3) => new
                    {
                        a.IMG_CODE,
                        a.ID,
                        CONTENT = a.CONTENT,
                        a.ARTICLE_NAME,
                        a.ARTICLE_CODE,
                        a.ARTICLE_CATEGORY,
                        a.LAST_ESSAY,
                        LAST_ESSAY_NAME = a2.ARTICLE_NAME,
                        a.NEXT_ESSAY,
                        NEXT_ESSAY_NAME = a3.ARTICLE_NAME
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
            var last = jt["last"]?.ToString();
            var next = jt["next"]?.ToString();
            using (var db = SugarContext.GetInstance())
            {
                db.Ado.BeginTran();
                try
                {
                    var article = db.Queryable<ARTICLE>().Where(x => x.ID == id).ToList().FirstOrDefault();
                    article.CONTENT = content;
                    article.ARTICLE_NAME = name;
                    article.ARTICLE_CATEGORY = category;
                    article.DATETIME_MODIFIED = DateTime.Now;
                    article.LAST_ESSAY = last;
                    article.NEXT_ESSAY = next;
                    db.Updateable(article).Where(x => x.ID == article.ID).ExecuteCommand();


                    var lastEssay = db.Queryable<ARTICLE>().Where(x => x.ID == last).ToList().FirstOrDefault();
                    if (lastEssay != null)
                    {
                        lastEssay.NEXT_ESSAY = article.ID;
                        db.Updateable(lastEssay).ExecuteCommand();
                    }

                    var nextEssay = db.Queryable<ARTICLE>().Where(x => x.ID == next).ToList().FirstOrDefault();
                    if (nextEssay != null)
                    {
                        nextEssay.LAST_ESSAY = article.ID;
                        db.Updateable(nextEssay).ExecuteCommand();
                    }
                    db.Ado.CommitTran();
                }
                catch(Exception ex)
                {
                    db.Ado.RollbackTran();
                    throw ex;
                }
            }
            return true;
        }


        public void Delete(string id)
        {
            using (var db = SugarContext.GetInstance())
            {
                var article = db.Queryable<ARTICLE>().Where(x => x.ID == id).ToList().FirstOrDefault();
                article.STATE = "D";
                article.DATETIME_MODIFIED = DateTime.Now;
                db.Updateable(article).ExecuteCommand();
            }
        }


        public void StorageInMemory(IDatabase database)
        {
            var result = GetArticlesToPage("cxk", "全部", 1, 99999);
            database.StringSet("all_articles", Newtonsoft.Json.JsonConvert.SerializeObject(result));
        }
    }
}
