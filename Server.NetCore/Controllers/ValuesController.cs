﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using DotNetCoreServer.Domians;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using DotNetCoreServer.Common;

namespace DotNetCoreServer.Controllers
{
    [WebApi("api/[controller]")]
    public class ValuesController : BaseController
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
    }
}
