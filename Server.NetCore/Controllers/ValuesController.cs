using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using DotNetCoreServer.Domians;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DotNetCoreServer.Controllers
{
    [Middleware("any", "初始类")]
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("id={id}")]
        public object Get(string id)
        {
            //return id;
            ImageReadDomain image = new ImageReadDomain();
            return image.OcrApi(id);
        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("user={user}&pwd={pwd}")]
        public void Register(string user, string pwd)
        {
            //Domains.User.Current.RegistUser(user, pwd);
        }

        [HttpGet("login/user={user}&pwd={pwd}", Name ="Login")]
        public IActionResult Login(string user, string pwd)
        {
            var res = Ok(Domains.User.Current.Login(user, pwd));
            return res;
        }

        [HttpGet("GetUserInfo")]
        public object GetUserInfo(string username)
        {
            return Domains.User.Current.GetUserInfo(username);
        }


        [HttpGet("test", Name = "test")]
        public IActionResult Test(string user, string pwd)
        {
            throw new Exception("ERRPR");
        }


        // POST api/values
        [HttpPost("image")]
        public object Post([FromBody] Object value)
        {
            ImageReadDomain image = new ImageReadDomain();
            return image.OcrApi((string)value);
        }

        // POST api/values
        [HttpPost("SignUp")]
        public void SignUp([FromBody] Object value)
        {
            Domains.User.Current.RegistUser(value);
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        // POST api/values
        [HttpPost("UpLoadImage")]
        public object UpLoadImg([FromBody] Object value)
        {
            return ImageReadDomain.UpLoadImage(value.ToString());
        }


        [HttpGet("getimage={getimage}")]
        public object GetImage(string getimage)
        {
            return ImageReadDomain.GetImage(getimage);
        }

        [HttpGet("output/input={input}")]
        public object Output(string input)
        {
            return MIDDLE_SERVER.Domians.Cmd.TestCmd(input);
        }


        [HttpPost("getword/word")]
        public object Getword([FromBody] Object value)
        {
            var word = value.ToString();
            return ImageReadDomain.GetWord(word);
        }
    }
}
