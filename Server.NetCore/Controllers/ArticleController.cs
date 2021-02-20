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

namespace DotNetCoreServer.Controllers
{
    [Middleware("any", "文章类")]
    [Route("api/article")]
    [ApiController]
    public class ArticleController : ControllerBase
    {
        // GET api/values
        //[HttpGet]
        //public ActionResult<IEnumerable<string>> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}


        [HttpPost("write")]
        public object Post([FromBody] Object value)
        {
            var result = JsonConvert.DeserializeObject<dynamic>(value.ToString());
            string title = result.title;
            string content = result.content;
            string user = result.user;
            string category = result.category;
            string last = result.last;
            string next = result.next;
            return WriteArticle.Current.NewArticle(title, content, user, category, last, next);
        }

        [HttpGet("user={user}&category={category}")]
        public object GetAllArticle(string user, string category)
        {
            return WriteArticle.Current.GetArticle(user, category);
        }

        [HttpGet("page/user={user}&category={category}&startIndex={startIndex}&length={length}")]
        public object GetArticlesToPage(string user, string category, int startIndex, int length)
        {
            return WriteArticle.Current.GetArticlesToPage(user, category, startIndex, length);
        }

        [HttpGet("getarticlecategory")]
        public object GetArticleCategory()
        {
            return WriteArticle.Current.GetArticleCategory();
        }

        [HttpGet("id={id}")]
        public object GetArticleContent(string id)
        {
            return WriteArticle.Current.GetArticleConent(id);
        }

        [HttpGet("GetAllArticle")]
        public object GetAllArticle(string user)
        {
            return WriteArticle.Current.GetAllArticle(user);
        }


        [HttpPost("EditArticle")]
        public object EditArticle(JToken jt)
        {
            return WriteArticle.Current.EditArticle(jt);
        }


        [HttpGet("Delete")]
        public void Delete(string id)
        {
            WriteArticle.Current.Delete(id);
        }
    }
}