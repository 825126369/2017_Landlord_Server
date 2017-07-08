using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Specialized;
using System.Text;
using System.IO;
using System.Net;
using System.Configuration;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace HttpLogin.BaiduView
{
    public partial class AnySDKLogin : System.Web.UI.Page
    {
        static string login = "http://oauth.anysdk.com/api/User/LoginOauth/";
        static string AES_KEY = "&@*(#kas9081fajk";

        class LoginInfo
        {
            public string isPayLogin = null;
            public string deviceID = null;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            NameValueCollection req = Request.Form;
            string args = "";
            foreach (string key in req.AllKeys)
            {
                args += key + "=" + req[key] + "&";
            }
            args = args.Substring(0, args.Length - 1);

            LoginInfo loginInfo = null;
            try
            {
                loginInfo = JsonHelper.ParseFromStr<LoginInfo>(req["server_ext_for_login"]);
            }
            catch (Exception)
            {

            }
            string msg = "";
            try
            {
                Uri uri = new Uri(login);
                HttpWebRequest requester = WebRequest.Create(uri) as HttpWebRequest;
                requester.Method = "POST";
                requester.Timeout = 3000;
                byte[] bs = Encoding.UTF8.GetBytes(args);
                requester.ContentType = "application/x-www-form-urlencoded";
                requester.ContentLength = bs.Length;
                using (Stream reqStream = requester.GetRequestStream())
                {
                    reqStream.Write(bs, 0, bs.Length);
                }

                HttpWebResponse responser = requester.GetResponse() as HttpWebResponse;
                StreamReader reader = new StreamReader(responser.GetResponseStream(), Encoding.UTF8);
                msg = reader.ReadToEnd();

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                Dictionary<string, object> rets = serializer.Deserialize<Dictionary<string, object>>(msg);
                if (rets["status"].ToString() == "ok")
                {
                    Dictionary<string, object> commondata = (Dictionary<string, object>)rets["common"];

                    Dictionary<string, object> savedata = new Dictionary<string, object>();

                    Random rd = new Random();
                    DateTime now = DateTime.Now;
                    int randkey = rd.Next();
                    string tempacc = "anysdk_" + commondata["channel"].ToString() + "_" + commondata["uid"].ToString();
                    string channelID = commondata["channel"].ToString();

                    string remoteIP = Common.Helper.getRemoteIP(Request);
                    savedata["acc"] = tempacc;
                    savedata["randkey"] = randkey;
                    savedata["regedittime"] = now;
                    savedata["regeditip"] = remoteIP;
                    savedata["updatepwd"] = false;
                    savedata["platform"] = "anysdk";
                    savedata["channelID"] = channelID;
                    savedata["lasttime"] = now.Ticks;
                    savedata["lastip"] = remoteIP;
                    string clientkey = randkey.ToString() + ":" + now.Ticks.ToString();
                    rets["ext"] = AESHelper.AESEncrypt(clientkey, AES_KEY);

                    if (MongodbAccount.Instance.KeyExistsBykey("AccountTable", "acc", tempacc) == false)
                    {
                        Dictionary<string, object> savelog = new Dictionary<string, object>();
                        savelog["acc"] = tempacc;
                        savelog["acc_real"] = tempacc;
                        savelog["ip"] = remoteIP;
                        savelog["time"] = now;
                        savelog["channel"] = channelID;

                        MongodbAccount.Instance.ExecuteInsert("RegisterLog", savelog);
                    }

                    msg = MongodbAccount.Instance.ExecuteStoreBykey("AccountTable", "acc", tempacc, savedata);
                    if (msg == "")
                    {
                        msg = serializer.Serialize(rets);

                        Dictionary<string, object> savelog = new Dictionary<string, object>();
                        savelog["acc"] = tempacc;
                        savelog["acc_real"] = tempacc;
                        if (loginInfo != null && !string.IsNullOrEmpty(loginInfo.deviceID))
                        {
                            savelog["acc_dev"] = loginInfo.deviceID;
                        }
                        savelog["ip"] = remoteIP;
                        savelog["time"] = now;
                        savelog["channel"] = channelID;

                        MongodbAccount.Instance.ExecuteInsert("LoginLog", savelog);
                    }
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }

            Response.Write(msg);
        }
    }
}