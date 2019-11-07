using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using MiddleServer.Domians;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MiddleServer.Controllers
{
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
        [EnableCors("any")]
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
        [EnableCors("any")]
        [HttpGet("user={user}&pwd={pwd}")]
        public void register(string user, string pwd)
        {
            //Domains.User.Current.RegistUser(user, pwd);
        }

        [EnableCors("any")]
        [HttpGet("login/user={user}&pwd={pwd}")]
        public object Login(string user, string pwd)
        {
            return Domains.User.Current.Login(user, pwd);
        }



        // POST api/values
        [EnableCors("any")]
        [HttpPost("image")]
        public object Post([FromBody] Object value)
        {
            ImageReadDomain image = new ImageReadDomain();
            return image.OcrApi((string)value);
        }

        // POST api/values
        [EnableCors("any")]
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
        [EnableCors("any")]
        [HttpPost("UpLoadImage")]
        public object UpLoadImg([FromBody] Object value)
        {
            return ImageReadDomain.UpLoadImage(value.ToString());
        }


        [EnableCors("any")]
        [HttpGet("getimage={getimage}")]
        public object getImage(string getimage)
        {
            return ImageReadDomain.getImage(getimage);
        }

        [EnableCors("any")]
        [HttpGet("output/input={input}")]
        public object Output(string input)
        {
            return MIDDLE_SERVER.Domians.Cmd.TestCmd(input);
        }
    }
}
