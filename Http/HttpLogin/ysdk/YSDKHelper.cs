using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using Common;

namespace YSDK
{
    public enum YSDKMethod
    {
        Pay = 0,
        Get_Balance,
    }

    struct KeyValue
    {
        public string key;
        public string value;
        public bool needSign;

        public KeyValue(string key, string value, bool needSign = true)
        {
            this.key = key;
            this.value = value;
            this.needSign = needSign;
        }
    }

    public class BalanceResult
    {
        public int ret;
        public int balance;
        public string msg;
    }
    public class PayResult
    {
        public int ret;
        public string billno;
        public string msg;
    }

    public class YSDKHelper
    {
        static string URL = "https://ysdk.qq.com";
        static string TestURL = "https://ysdktest.qq.com";

        static string AppKey = "BcSU2wtYrwHKKoJfow2fcb3PqClyfZIo";
        static string TestAppKey = "zoWDJJITQ94QTKElkcNZoQIRsMJUj5kV";
        static string AppID = "1105498669";
        static bool isTest = false;

        static Dictionary<YSDKMethod, string> Methods = new Dictionary<YSDKMethod, string>();

        static YSDKHelper()
        {
            Methods.Add(YSDKMethod.Get_Balance, "/mpay/get_balance_m");
            Methods.Add(YSDKMethod.Pay, "/mpay/pay_m");
        }

        static string getMethodString(YSDKMethod method)
        {
            return Methods[method];
        }

        List<KeyValue> mKeyValues = new List<KeyValue>();

        string mAppKey = "";
        string mURL = "";
        string mMode = "GET";

        public YSDKHelper()
        {
            if (isTest)
            {
                mURL = TestURL;
                mAppKey = TestAppKey;
            }
            else
            {
                mURL = URL;
                mAppKey = AppKey;
            }
        }

        public void initKeyValue(string openid, string openkey, string pf, string pfkey)
        {
            addKeyValue("appid", AppID);
            addKeyValue("format", "json");
            addKeyValue("openid", openid);
            addKeyValue("openkey", openkey);
            addKeyValue("pf", pf);
            addKeyValue("pfkey", pfkey);
            addKeyValue("ts", Helper.GetCurTime().ToString());
            addKeyValue("zoneid", "1");
        }

        public void addKeyValue(string key, string value, bool sign = true)
        {
            mKeyValues.Add(new KeyValue(key, value, sign));
        }

        private static int keyCompare(KeyValue value1, KeyValue value2)
        {
            return value1.key.CompareTo(value2.key);
        }

        void sortKeyValue()
        {
            mKeyValues.Sort(keyCompare);
        }

        string getValueString(bool onlySign)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var v in mKeyValues)
            {
                if (onlySign && v.needSign == false)
                    continue;

                if (sb.Length > 0)
                {
                    sb.AppendFormat("&{0}={1}", v.key, v.value);
                }
                else
                {
                    sb.AppendFormat("{0}={1}", v.key, v.value);
                }
            }
            return sb.ToString();
        }

        string getSignString(string mothed)
        {
            string url = "/v3/r" + mothed;
            string urlCode = Helper.UrlEncode(url);
            string valueCode = Helper.UrlEncode(getValueString(true));
            string plainText = string.Format("{0}&{1}&{2}", mMode, urlCode, valueCode);
            string key = mAppKey + "&";
            string sign = Helper.SHA1Encrypt(plainText, key);

            string urlSign = Helper.UrlEncode(sign);
            return string.Format("sig={0}", urlSign);
        }

        string getURL(string mothed)
        {
            return mURL + mothed;
        }

        public string buildURL(YSDKMethod ysdkMethod)
        {
            sortKeyValue();
            string mothed = getMethodString(ysdkMethod);
            string url = getURL(mothed);
            string sig = getSignString(mothed);
            string valueString = getValueString(false);

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0}?{1}", url, valueString);
            sb.AppendFormat("&{0}", sig);
            return sb.ToString();
        }

        public string buildCookie(string platform, YSDKMethod ysdkMethod)
        {
            string mothed = getMethodString(ysdkMethod);
            if (platform == "1")
            {
                return string.Format("session_id=openid;session_type=kp_actoken;org_loc={0}", mothed);                                
            }
            else
            {
                return string.Format("session_id=hy_gameid;session_type=wc_actoken;org_loc={0}", mothed);
            }
        }
    }
}