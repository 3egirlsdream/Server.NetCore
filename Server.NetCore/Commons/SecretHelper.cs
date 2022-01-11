using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Server.NetCore.Commons
{
    public class SecretHelper
    {
        readonly static string key = "vvvvvvvvvvvv";
        /// <summary>
        /// DES加密
        /// </summary>
        /// <param name="data">加密数据</param>
        /// <returns></returns>
        public static string Encrypt(string data)
        {
            var cryptoProvider = new DESCryptoServiceProvider
            {
                Key = Encoding.ASCII.GetBytes(key.Substring(0, 8)),
                IV = Encoding.ASCII.GetBytes(key.Substring(0, 8))
            };
            var bytes = Encoding.ASCII.GetBytes(data);
            MemoryStream ms = new MemoryStream();
            CryptoStream cst = new CryptoStream(ms, cryptoProvider.CreateEncryptor(), CryptoStreamMode.Write);
            cst.Write(bytes, 0, bytes.Length);
            cst.FlushFinalBlock();
            var builder = new StringBuilder();
            foreach(byte num in ms.ToArray())
            {
                builder.AppendFormat("{0:X2}", num);
            }
            ms.Close();
            cst.Close();
            return builder.ToString();
        }

        /// <summary>
        /// DES解密
        /// </summary>
        /// <param name="data">解密数据</param>
        /// <returns></returns>
        public static string DESDecrypt(string data)
        {
            var cryptoProvider = new DESCryptoServiceProvider
            {
                Key = Encoding.ASCII.GetBytes(key.Substring(0, 8)),
                IV = Encoding.ASCII.GetBytes(key.Substring(0, 8))
            };
            var buffer = new Byte[data.Length / 2];
            for (int i = 0; i < (data.Length / 2); i++)
            {
                int num2 = Convert.ToInt32(data.Substring(i * 2, 2), 0x10);
                buffer[i] = (byte)num2;
            }

            using (var ms = new MemoryStream())
            {
                using (var cst = new CryptoStream(ms, cryptoProvider.CreateDecryptor(), CryptoStreamMode.Write))
                {

                    cst.Write(buffer, 0, buffer.Length);
                    cst.FlushFinalBlock();
                    return Encoding.ASCII.GetString(ms.ToArray());
                }
            }
        }
    }
}
