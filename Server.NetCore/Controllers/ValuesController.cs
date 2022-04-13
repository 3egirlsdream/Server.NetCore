using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using DotNetCoreServer.Domians;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using DotNetCoreServer.Common;
using Server.NetCore.Domians;
using System.Threading;

namespace DotNetCoreServer.Controllers
{
    public class ValuesController : BaseController
    {
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpGet]
        public void RabbitMQ(string id)
        {
            RabbitMQDomain mq = new RabbitMQDomain();
            //mq.Consumer();
            mq.Producter(id);
        }

        [HttpGet("id={id}")]
        public object Get(string id)
        {
            ImageReadDomain image = new ImageReadDomain();
            return image.OcrApi(id);
        }

        [HttpPost("image")]
        public object Post([FromBody] Object value)
        {
            ImageReadDomain image = new ImageReadDomain();
            return image.OcrApi((string)value);
        }


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

        [HttpPost]
        public object GetWordCloud([FromBody] Object value)
        {
            var word = value.ToString();
            return ImageReadDomain.GetWord(word);
        }

        [HttpGet("GetChatRecord")]
        public object GetChatRecord(string GroupId)
        {
            return Domains.User.Current.GetChatRecord(GroupId);
        }

        [HttpPost("UpChatRecord")]
        public void UpChatRecord(JToken jt)
        {
            string GroupId = jt["GroupId"]?.ToString();
            string ChatRecord = jt["ChatRecord"]?.ToString();
            Domains.User.Current.UpChatRecord(GroupId, ChatRecord);
        }

        [HttpGet("GetChatList")]
        public object GetChatList(string username)
        {
            return Domains.User.Current.GetChatList(username);
        }

        [HttpGet("UpdateList")]
        public void UpdateList(string groupId, string groupName, string users)
        {
            Domains.User.Current.UpdateList(groupId, groupName, users);
        }

        [HttpGet("GetEmoji")]
        public object GetEmoji(string groupId)
        {
            return Domains.User.Current.GetEmoji(groupId);
        }

        [HttpGet("AddEmoji")]
        public void AddEmoji(string groupId, string url)
        {
            Domains.User.Current.AddEmoji(groupId, url);
        }

        [HttpGet("DeleteEmoji")]
        public void DeleteEmoji(string groupId, string url)
        {
            Domains.User.Current.DeleteEmoji(groupId, url);
        }

        [HttpGet]
        public string Delay()
        {
            Thread.Sleep(4000);
            return "YES";
        }
    }
}
