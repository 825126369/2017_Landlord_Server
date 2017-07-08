using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Security.Cryptography;

namespace Common
{
    public class Helper
    {
        static public bool checkMD5(string source, string md5)
        {
            return getMD5(source) == md5;
        }

        static public string buildLuaReturn(int code, string msg)
        {
            return string.Format("local ret = {{code = {0}, msg=\"{1}\"}}; return ret;", code, msg);
        }

        static public string getMD5(string source)
        {
            MD5 md5Service = new MD5CryptoServiceProvider();
            byte[] src = Encoding.UTF8.GetBytes(source);
            byte[] res = md5Service.ComputeHash(src, 0, src.Length);
            return BitConverter.ToString(res).ToLower().Replace("-", "");
        }

        static public string SHA1Encrypt(string plainText, string strKey)
        {
            HMACSHA1 hmacsha1 = new HMACSHA1();
            hmacsha1.Key = Encoding.UTF8.GetBytes(strKey);
            byte[] dataBuffer = Encoding.UTF8.GetBytes(plainText);
            byte[] hashBytes = hmacsha1.ComputeHash(dataBuffer);
            //return System.Text.Encoding.ASCII.GetString(hashBytes);
            return Convert.ToBase64String(hashBytes);
        }
        static public string UrlEncode(string value)
        {
            StringBuilder builder = new StringBuilder();
            foreach (char c in value)
            {
                if (System.Web.HttpUtility.UrlEncode(c.ToString()).Length > 1)
                {
                    builder.Append(System.Web.HttpUtility.UrlEncode(c.ToString()).ToUpper());
                }
                else
                {
                    builder.Append(c);
                }
            }
            return builder.ToString();

            //return System.Web.HttpUtility.UrlEncode(value, Encoding.UTF8);
        }
        /// <summary>  
        /// 时间戳Timestamp  
        /// </summary>  
        /// <returns></returns>  
        static public int GetCurTime()  
        {  
            DateTime DateStart= new DateTime(1970,1,1,8,0,0);  
            return Convert.ToInt32((DateTime.Now - DateStart).TotalSeconds);  
        }

        /// <summary>  
        /// 时间戳转为C#格式时间  
        /// </summary>  
        /// <param name=”timeStamp”></param>  
        /// <returns></returns>  
        static public DateTime ConvertDateTime(string timeStamp)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(timeStamp + "0000000");
            TimeSpan toNow = new TimeSpan(lTime); return dtStart.Add(toNow);
        }
        /// <summary>
        /// 获取web客户端ip
        /// </summary>
        /// <returns></returns>
        public static string GetWebClientIp()
        {
            string userIP = "未获取用户IP";

            try
            {
                if (System.Web.HttpContext.Current == null
            || System.Web.HttpContext.Current.Request == null
            || System.Web.HttpContext.Current.Request.ServerVariables == null)
                    return "";

                string CustomerIP = "";

                //CDN加速后取到的IP   
                CustomerIP = System.Web.HttpContext.Current.Request.Headers["Cdn-Src-Ip"];
                if (!string.IsNullOrEmpty(CustomerIP))
                {
                    return CustomerIP;
                }

                CustomerIP = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];


                if (!String.IsNullOrEmpty(CustomerIP))
                    return CustomerIP;

                if (System.Web.HttpContext.Current.Request.ServerVariables["HTTP_VIA"] != null)
                {
                    CustomerIP = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                    if (CustomerIP == null)
                        CustomerIP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                }
                else
                {
                    CustomerIP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                }

                if (string.Compare(CustomerIP, "unknown", true) == 0)
                    return System.Web.HttpContext.Current.Request.UserHostAddress;
                return CustomerIP;
            }
            catch { }

            return userIP;
        }  
    }
}
