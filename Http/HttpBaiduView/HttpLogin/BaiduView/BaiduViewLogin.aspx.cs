using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;

namespace HttpLogin.Platform
{
    public partial class BaiduViewLogin : System.Web.UI.Page
    {
        public class BaiduData
        {
            public int sex;
            public string userid;
            public string username;
            public string avatar;
        }

        public class BaiduResult
        {
            public int result;
            public string message;
            public BaiduData data;
        }

        static string AES_KEY = "@@baiduviewkey~~";
        static string AES_LOGINKEY = "&@*(#kas9081fajk";
        protected void Page_Load(object sender, EventArgs e)
        {
            string platform = Request.QueryString["platform"];
            string channelID = Request.QueryString["channelID"];
            string loginKey = Request.QueryString["loginkey"];
            if (string.IsNullOrEmpty(loginKey))
            {
                Response.Write(BuildAccount.buildLuaReturn(-1, "data error"));
                return;
            }
            if (string.IsNullOrEmpty(loginKey))
            {
                Response.Write(BuildAccount.buildLuaReturn(-1, "data error"));
                return;
            }
            if (string.IsNullOrEmpty(channelID))
            {
                Response.Write(BuildAccount.buildLuaReturn(-1, "data error"));
                return;
            }
            loginKey = Encoding.Default.GetString(Convert.FromBase64String(loginKey));
            string decryptStr = AESHelper.AESDecrypt(loginKey, AES_KEY);
            string[] subString = decryptStr.Split(':');
            if (subString.Length < 2)
            {
                Response.Write(BuildAccount.buildLuaReturn(-1, "data error"));
                return;
            }
            string token = subString[0];
            string sign = subString[1];
            string remoteIP = Common.Helper.getRemoteIP(Request);

            string url = string.Format("http://zhibo.v.baidu.com/token/tokencheck/?token={0}&ip={1}&sign={2}", token, remoteIP, sign);

            byte[] bytes = HttpPost.Get(new Uri(url));
            if (bytes == null)
            {
                Response.Write(BuildAccount.buildLuaReturn(-2, "http error"));
                return;
            }
            string result = Encoding.UTF8.GetString(bytes);

            BaiduResult baiduResult = JsonHelper.ParseFromStr<BaiduResult>(result);
            if (baiduResult.result == 0)
            {
                checkAccount(baiduResult, channelID);
            }
            else
            {
                Response.Write(BuildAccount.buildLuaReturn(-3, "check error"));
            }
        }

        void checkAccount(BaiduResult result, string channelID)
        {
            Random rd = new Random();
            DateTime now = DateTime.Now;
            int randkey = rd.Next();
            Dictionary<string, object> savedata = new Dictionary<string, object>();

            string remoteIP = Common.Helper.getRemoteIP(Request);

            string acc = "bd_" + result.data.userid;
            savedata["acc"] = acc;
            savedata["username"] = result.data.username;
            savedata["randkey"] = randkey;
            savedata["updatepwd"] = false;
            savedata["platform"] = "baiduview";
            savedata["channelID"] = channelID;
            savedata["lasttime"] = now.Ticks;
            savedata["lastip"] = remoteIP;

            string msg = "";
            if (MongodbAccount.Instance.KeyExistsBykey("AccountTable", "acc", acc) == false)
            {
                savedata["regedittime"] = now;
                savedata["regeditip"] = remoteIP;

                msg = MongodbAccount.Instance.ExecuteStoreBykey("AccountTable", "acc", acc, savedata);

                Dictionary<string, object> savelog = new Dictionary<string, object>();
                savelog["acc"] = acc;
                savelog["acc_real"] = acc;
                savelog["ip"] = remoteIP;
                savelog["time"] = now;
                savelog["channel"] = channelID;

                MongodbAccount.Instance.ExecuteInsert("RegisterLog", savelog);
            }
            else
            {
                msg = MongodbAccount.Instance.ExecuteUpdate("AccountTable", "acc", acc, savedata);
            }
 
            if (msg == "")
            {
                Dictionary<string, object> savelog = new Dictionary<string, object>();
                savelog["acc"] = acc;
                savelog["acc_real"] = acc;
                savelog["ip"] = remoteIP;
                savelog["time"] = now;
                savelog["channel"] = channelID;

                MongodbAccount.Instance.ExecuteInsert("LoginLog", savelog);
            }

            string clientkey = randkey.ToString() + ":" + now.Ticks.ToString();
            string loginKey = AESHelper.AESEncrypt(clientkey, AES_LOGINKEY);

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("local ret = {{code = 0, msg=\"{0}\"}};", loginKey);
            sb.AppendFormat("ret.userid = \"{0}\";", AESHelper.AESEncrypt(acc, AES_KEY));
            sb.AppendFormat("ret.username = \"{0}\";", AESHelper.AESEncrypt(result.data.username, AES_KEY));
            sb.Append("return ret;");

            Response.Write(sb.ToString());
        }
    }
}