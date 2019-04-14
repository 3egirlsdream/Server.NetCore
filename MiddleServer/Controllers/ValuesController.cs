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
            return id;
            ImageReadDomain image = new ImageReadDomain();
            return image.OcrApi(id);
        }

        // POST api/values
        [EnableCors("any")]
        [HttpPost("image")]
        public object Post([FromBody] Object value)
        {
            ImageReadDomain image = new ImageReadDomain();
            return image.OcrApi((string)value);
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
    }
}
