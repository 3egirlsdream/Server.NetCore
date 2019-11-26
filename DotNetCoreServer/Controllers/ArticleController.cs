using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DotNetCoreServer.Domians;
using Newtonsoft.Json;

namespace DotNetCoreServer.Controllers
{
    [Route("api/article")]
    [ApiController]
    public class ArticleController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }


        [EnableCors("any")]
        [HttpPost("write")]
        public object Post([FromBody] Object value)
        {
            var result = JsonConvert.DeserializeObject<dynamic>(value.ToString());
            string title = result.title;
            string content = result.content;
            string user = result.user;
            return WriteArticle.Current.newArticle(title, content, user);
        }

        [EnableCors("any")]
        [HttpGet("user={user}")]
        public object GetAllArticle(string user)
        {
            return WriteArticle.Current.getArticle(user);
        }

        [EnableCors("any")]
        [HttpGet("id={id}")]
        public object GetArticleContent(string id)
        {
            return WriteArticle.Current.getArticleConent(id);
        }
    }
}