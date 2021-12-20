using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DotNetCoreServer.Domians;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using DotNetCoreServer.Common;
using StackExchange.Redis;

namespace DotNetCoreServer.Controllers
{
    public class ArticleController : BaseController
    {

        private readonly IDatabase _database;

        public ArticleController(RedisHelper helper)
        {
            _database = helper.GetDatabase();
        }
        [HttpPost]
        public object Write([FromBody] Object value)
        {
            CheckLogin();
            var result = JsonConvert.DeserializeObject<dynamic>(value.ToString());
            string title = result.title;
            string content = result.content;
            string user = result.user;
            string category = result.category;
            string last = result.last;
            string next = result.next;
            var rtl = WriteArticle.Current.NewArticle(title, content, user, category, last, next);
            WriteArticle.Current.StorageInMemory(_database);
            return rtl;
        }

        [HttpGet]
        public object GetAllArticle(string user, string category)
        {
            return WriteArticle.Current.GetArticle(user, category);
        }

        [HttpGet]
        public object GetArticlesToPage(string user, string category, int startIndex, int length)
        {
            var cache = _database.StringGet("all_articles");
            if (!string.IsNullOrEmpty(cache))
            {
                var result = JsonConvert.DeserializeObject<dynamic>(cache);
                List<dynamic> ls = JsonConvert.DeserializeObject<List<dynamic>>(Convert.ToString(result.data));
                var totalCount = ls.Count;
                IEnumerable<dynamic> list = new List<dynamic>();
                if (category == "全部")
                {
                    list = ls.Skip((startIndex - 1) * length).Take(length);
                }
                else
                {
                    list = ls.Where(c => c.ARTICLE_CATEGORY.ToString().Contains(category)).Skip((startIndex - 1) * length).Take(length);
                }

                return new
                {
                    data = list.ToList(),
                    totalCount
                };
            }
            return WriteArticle.Current.GetArticlesToPage(user, category, startIndex, length);
        }

        [HttpGet]
        public object GetArticleCategory()
        {
            return WriteArticle.Current.GetArticleCategory();
        }

        [HttpGet]
        public object GetArticleContent(string id)
        {
            return WriteArticle.Current.GetArticleConent(id);
        }

        [HttpGet]
        public object GetAllArticles(string user)
        {
            return WriteArticle.Current.GetAllArticle(user);
        }


        [HttpPost]
        public object EditArticle(JToken jt)
        {
            CheckLogin();
            var result = WriteArticle.Current.EditArticle(jt);
            WriteArticle.Current.StorageInMemory(_database);
            return result;
        }


        [HttpGet]
        public void Delete(string id)
        {
            CheckLogin();
            WriteArticle.Current.Delete(id);
        }

        [HttpGet]
        public void RefreshCache()
        {
            WriteArticle.Current.StorageInMemory(_database);
        }

        private void CheckLogin()
        {
            if(!BaseController.IS_LOGIN)
            {
                throw new Exception("请先登录");
            }
        }
    }
}