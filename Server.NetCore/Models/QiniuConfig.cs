using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetCoreServer.Models
{
    public class QiniuConfig
    {
        /// <summary>
        /// 密钥-AccessKey
        /// </summary>
        public string AccessKey { set; get; }

        /// <summary>
        /// 密钥-SecretKey
        /// </summary>
        public string SecretKey { set; get; }

        /// <summary>
        /// 初始化密钥AK/SK
        /// </summary>
        /// <param name="accessKey">AccessKey</param>
        /// <param name="secretKey">SecretKey</param>
        public QiniuConfig(string accessKey, string secretKey)
        {
            this.AccessKey = accessKey;
            this.SecretKey = secretKey;
        }
    }
    public class Base64
    {
        /// <summary>
        /// 获取字符串Url安全Base64编码值
        /// </summary>
        /// <param name="text">源字符串</param>
        /// <returns>已编码字符串</returns>
        public static string UrlSafeBase64Encode(string text)
        {
            return UrlSafeBase64Encode(Encoding.UTF8.GetBytes(text));
        }

        /// <summary>
        /// URL安全的base64编码
        /// </summary>
        /// <param name="data">需要编码的字节数据</param>
        /// <returns></returns>
        public static string UrlSafeBase64Encode(byte[] data)
        {
            return Convert.ToBase64String(data).Replace('+', '-').Replace('/', '_');
        }

        /// <summary>
        /// bucket:key 编码
        /// </summary>
        /// <param name="bucket">空间名称</param>
        /// <param name="key">文件key</param>
        /// <returns>编码</returns>
        public static string UrlSafeBase64Encode(string bucket, string key)
        {
            return UrlSafeBase64Encode(bucket + ":" + key);
        }

        /// <summary>
        /// Base64解码
        /// </summary>
        /// <param name="text">待解码的字符串</param>
        /// <returns>已解码字符串</returns>
        public static byte[] UrlsafeBase64Decode(string text)
        {
            return Convert.FromBase64String(text.Replace('-', '+').Replace('_', '/'));
        }
    }


    public class PutPolicy
    {
        /// <summary>
        /// [必需]bucket或者bucket:key
        /// </summary>
        [JsonProperty("scope")]
        public string Scope { get; set; }

        /// <summary>
        /// [可选]若为 1，表示允许用户上传以 scope 的 keyPrefix 为前缀的文件。
        /// </summary>
        [JsonProperty("isPrefixalScope", NullValueHandling = NullValueHandling.Ignore)]
        public int? isPrefixalScope { get; set; }

        /// <summary>
        /// [必需]上传策略失效时刻，请使用SetExpire来设置它
        /// </summary>
        [JsonProperty("deadline")]
        public int Deadline { get; private set; }

        /// <summary>
        /// [可选]"仅新增"模式
        /// </summary>
        [JsonProperty("insertOnly", NullValueHandling = NullValueHandling.Ignore)]
        public int? InsertOnly { get; set; }

        /// <summary>
        /// [可选]保存文件的key
        /// </summary>
        [JsonProperty("saveKey", NullValueHandling = NullValueHandling.Ignore)]
        public string SaveKey { get; set; }

        /// <summary>
        /// [可选]终端用户
        /// </summary>
        [JsonProperty("endUser", NullValueHandling = NullValueHandling.Ignore)]
        public string EndUser { get; set; }

        /// <summary>
        /// [可选]返回URL
        /// </summary>
        [JsonProperty("returnUrl", NullValueHandling = NullValueHandling.Ignore)]
        public string ReturnUrl { get; set; }

        /// <summary>
        /// [可选]返回内容
        /// </summary>
        [JsonProperty("returnBody", NullValueHandling = NullValueHandling.Ignore)]
        public string ReturnBody { get; set; }

        /// <summary>
        /// [可选]回调URL
        /// </summary>
        [JsonProperty("callbackUrl", NullValueHandling = NullValueHandling.Ignore)]
        public string CallbackUrl { get; set; }

        /// <summary>
        /// [可选]回调内容
        /// </summary>
        [JsonProperty("callbackBody", NullValueHandling = NullValueHandling.Ignore)]
        public string CallbackBody { get; set; }

        /// <summary>
        /// [可选]回调内容类型
        /// </summary>
        [JsonProperty("callbackBodyType", NullValueHandling = NullValueHandling.Ignore)]
        public string CallbackBodyType { get; set; }

        /// <summary>
        /// [可选]回调host
        /// </summary>
        [JsonProperty("callbackHost", NullValueHandling = NullValueHandling.Ignore)]
        public string CallbackHost { get; set; }

        /// <summary>
        /// [可选]回调fetchkey
        /// </summary>
        [JsonProperty("callbackFetchKey", NullValueHandling = NullValueHandling.Ignore)]
        public int? CallbackFetchKey { get; set; }

        /// <summary>
        /// [可选]上传预转持久化
        /// </summary>
        [JsonProperty("persistentOps", NullValueHandling = NullValueHandling.Ignore)]
        public string PersistentOps { get; set; }

        /// <summary>
        /// [可选]持久化结果通知
        /// </summary>
        [JsonProperty("persistentNotifyUrl", NullValueHandling = NullValueHandling.Ignore)]
        public string PersistentNotifyUrl { get; set; }

        /// <summary>
        /// [可选]私有队列
        /// </summary>
        [JsonProperty("persistentPipeline", NullValueHandling = NullValueHandling.Ignore)]
        public string PersistentPipeline { get; set; }

        /// <summary>
        /// [可选]上传文件大小限制：最小值
        /// </summary>
        [JsonProperty("fsizeMin", NullValueHandling = NullValueHandling.Ignore)]
        public int? FsizeMin { get; set; }

        /// <summary>
        /// [可选]上传文件大小限制：最大值
        /// </summary>
        [JsonProperty("fsizeLimit", NullValueHandling = NullValueHandling.Ignore)]
        public int? FsizeLimit { get; set; }

        /// <summary>
        /// [可选]上传时是否自动检测MIME
        /// </summary>
        [JsonProperty("detectMime", NullValueHandling = NullValueHandling.Ignore)]
        public int? DetectMime { get; set; }

        /// <summary>
        /// [可选]上传文件MIME限制
        /// </summary>
        [JsonProperty("mimeLimit", NullValueHandling = NullValueHandling.Ignore)]
        public string MimeLimit { get; set; }

        /// <summary>
        /// [可选]文件上传后多少天后自动删除
        /// </summary>
        [JsonProperty("deleteAfterDays", NullValueHandling = NullValueHandling.Ignore)]
        public int? DeleteAfterDays { get; set; }

        /// <summary>
        /// [可选]文件的存储类型，默认为普通存储，设置为1为低频存储
        /// </summary>
        [JsonProperty("fileType", NullValueHandling = NullValueHandling.Ignore)]
        public int? FileType { get; set; }

        /// <summary>
        /// 设置上传凭证有效期（配置Deadline属性）
        /// </summary>
        /// <param name="expireInSeconds"></param>
        public void SetExpires(int expireInSeconds)
        {
            this.Deadline = (int)Util.UnixTimestamp.GetUnixTimestamp(expireInSeconds);
        }

        /// <summary>
        /// 转换为JSON字符串
        /// </summary>
        /// <returns>JSON字符串</returns>
        public string ToJsonString()
        {
            if (this.Deadline == 0)
            {
                //默认一个小时有效期
                this.SetExpires(3600);
            }
            return JsonConvert.SerializeObject(this);
        }

    }
}

namespace Util
{
    /// <summary>
    /// 时间戳与日期时间转换
    /// </summary>
    public class UnixTimestamp
    {
        /// <summary>
        /// 基准时间
        /// </summary>
        private static DateTime dtBase = new DateTime(1970, 1, 1).ToLocalTime();

        /// <summary>
        /// 时间戳末尾7位(补0或截断)
        /// </summary>
        private const long TICK_BASE = 10000000;

        /// <summary>
        /// 从现在(调用此函数时刻)起若干秒以后那个时间点的时间戳
        /// </summary>
        /// <param name="secondsAfterNow">从现在起多少秒以后</param>
        /// <returns>Unix时间戳</returns>
        public static long GetUnixTimestamp(long secondsAfterNow)
        {
            DateTime dt = DateTime.Now.AddSeconds(secondsAfterNow).ToLocalTime();
            TimeSpan tsx = dt.Subtract(dtBase);
            return tsx.Ticks / TICK_BASE;
        }

        /// <summary>
        /// 日期时间转换为时间戳
        /// </summary>
        /// <param name="dt">日期时间</param>
        /// <returns>时间戳</returns>
        public static long ConvertToTimestamp(DateTime dt)
        {
            TimeSpan tsx = dt.Subtract(dtBase);
            return tsx.Ticks / TICK_BASE;
        }

        /// <summary>
        /// 从UNIX时间戳转换为DateTime
        /// </summary>
        /// <param name="timestamp">时间戳字符串</param>
        /// <returns>日期时间</returns>
        public static DateTime ConvertToDateTime(string timestamp)
        {
            long ticks = long.Parse(timestamp) * TICK_BASE;
            return dtBase.AddTicks(ticks);
        }

        /// <summary>
        /// 从UNIX时间戳转换为DateTime
        /// </summary>
        /// <param name="timestamp">时间戳</param>
        /// <returns>日期时间</returns>
        public static DateTime ConvertToDateTime(long timestamp)
        {
            long ticks = timestamp * TICK_BASE;
            return dtBase.AddTicks(ticks);
        }

        /// <summary>
        /// 检查Ctx是否过期，我们给当前时间加上一天来看看是否超过了过期时间
        /// 而不是直接比较是否超过了过期时间，是给这个文件最大1天的上传持续时间
        /// </summary>
        /// <param name="expiredAt"></param>
        /// <returns></returns>
        public static bool IsContextExpired(long expiredAt)
        {
            if (expiredAt == 0)
            {
                return false;
            }
            bool expired = false;
            DateTime now = DateTime.Now.AddDays(1);
            long nowTs = ConvertToTimestamp(now);
            if (nowTs > expiredAt)
            {
                expired = true;
            }
            return expired;
        }

    }
}