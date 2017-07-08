using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections.Specialized;
using System.Text;
using Common;

namespace _360Game
{
    public class HandlerHelper
    {
        public static Dictionary<string, string> convertParams(NameValueCollection collection)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            foreach (var key in collection.AllKeys)
            {
                data.Add(key, collection[key]);
            }
            return data;
        }

        public static bool checkSign(Dictionary<string, string> data, string[] checkOrder, string checkKey, string signKey = "sign")
        {
            bool check = checkData(data, checkOrder);
            if (!check)
                return false;

            StringBuilder sb = new StringBuilder();
            foreach(var s in checkOrder)
            {
                sb.AppendFormat("{0}", data[s]);
            }
            sb.AppendFormat("#{0}", checkKey);
            return Helper.checkMD5(sb.ToString(), data[signKey]);
        }

        public static bool checkData(Dictionary<string, string> data, string[] checkOrder, string signKey = "sign")
        {
            foreach (var s in checkOrder)
            {
                if (!data.ContainsKey(s))
                    return false;
            }
            if (!data.ContainsKey(signKey))
                return false;

            return true;
        }

        public static string getRemoteIP(HttpContext context)
        {
            string user_IP = string.Empty;

            try
            {
                if (context.Request.Headers["X-Forward-For"] != null)
                {
                    user_IP = context.Request.Headers["X-Forward-For"].ToString();
                }
                else if (context.Request.ServerVariables["HTTP_VIA"] != null)
                {
                    if (context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != null)
                    {
                        user_IP = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
                    }
                    else
                    {
                        user_IP = context.Request.ServerVariables["REMOTE_ADDR"].ToString();
                    }
                }
                else
                {
                    user_IP = context.Request.ServerVariables["REMOTE_ADDR"].ToString();
                }
            }
            catch
            {

            }
            return user_IP;
        }

        public static string UrlEncode(string str)
        {
            return HttpUtility.UrlEncode(str);
        }

        public static string UrlDecode(string str)
        {
            return HttpUtility.UrlDecode(str);
        }

        public static void log(HttpContext context, string msg, string uid)
        {
            if (uid == null)
            {
                CLOG.Info(string.Format("{0} ip:{1} uid:null", msg, HandlerHelper.getRemoteIP(context)));
            }
            else
            {
                CLOG.Info(string.Format("{0} ip:{1} uid:{2}", msg, HandlerHelper.getRemoteIP(context), uid));
            }
            
        }
    }
}