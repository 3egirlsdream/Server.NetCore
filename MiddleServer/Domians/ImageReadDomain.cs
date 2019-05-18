using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MiddleServer.Domians
{
    public class ImageReadDomain
    {
        public string OcrApi(string base64)
        {
            base64 = base64.Split(',').LastOrDefault();
            //return base64;
            string appKey = "0o4AsvCEAy6sF2ORZy2AGNg8";
            string secretKey = "B4AHl3E6pzSfY36fcm3eATvyR0o5ktLG";
            string Url = "https://aip.baidubce.com/oauth/2.0/token";

            HttpClient client = new HttpClient();
            List<KeyValuePair<string, string>> keys = new List<KeyValuePair<string, string>>();
            keys.Add(new KeyValuePair<string, string>("grant_type", "client_credentials"));
            keys.Add(new KeyValuePair<string, string>("client_id", appKey));
            keys.Add(new KeyValuePair<string, string>("client_secret", secretKey));

            HttpResponseMessage message = client.PostAsync(Url, new FormUrlEncodedContent(keys)).Result;
            string result = message.Content.ReadAsStringAsync().Result;
            dynamic model = JsonConvert.DeserializeObject<dynamic>(result);
            string access_token = (string)model.access_token;// "24.15877ce69fbfcbb481b82f7b504e10eb.2592000.1557745923.282335-16011829";// model.access_token as string;

            //以上是获取token

            ///图像数据，base64编码进行urlencode
            ///是否检测图像朝向
            string api = "https://aip.baidubce.com/rest/2.0/ocr/v1/accurate_basic?access_token=" + access_token;
            Encoding encoding = Encoding.Default;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(api);
            request.Method = "post";
            request.ContentType = "application/x-www-form-urlencoded";
            request.KeepAlive = true;
            String str = "image=" + HttpUtility.UrlEncode(base64) + "&language_type=CHN_ENG";
            byte[] buffer = encoding.GetBytes(str);
            request.ContentLength = buffer.Length;
            request.GetRequestStream().Write(buffer, 0, buffer.Length);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            string res = reader.ReadToEnd();
            var d = JsonConvert.DeserializeObject<Models.Root>(res);

            string s = "";
            if (d.words_result == null)
                return "Empty";
            foreach (var ds in d.words_result)
            {
                s += ds.words as string + "\r\n";
            }
            return s;
        }
    }
}
