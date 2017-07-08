using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Configuration;

namespace HttpLogin.Default
{
    public partial class AutoRegeditAcc : System.Web.UI.Page
    {
        const string AES_KEY = "&@*(#kas9081fajk";

        protected void Page_Load(object sender, EventArgs e)
        {
            string platform = Request.Params["platform"];
            if (string.IsNullOrEmpty(platform))
            {
                Response.Write("local ret = {code = -1}; return ret;"); 
                return;
            }

            string table = ConfigurationManager.AppSettings["acc_" + platform];
            if (string.IsNullOrEmpty(table))
            {
                Response.Write("local ret = {code = -15}; return ret;"); 
                return;
            }            

            string acc = BuildAccount.getAutoAccount(table);
            if (string.IsNullOrEmpty(acc))
            {
                Response.Write("local ret = {code = -12}; return ret;");
                return;
            }
            string encrypt = Request.Params["encrypt"];
            bool pwd_encrypt = false;
            if (!string.IsNullOrEmpty(encrypt) && encrypt == "true")
            {
                pwd_encrypt = true;
            }
            string pwd = null;
            string out_pwd = null;
            string save_pwd = null;
            if (pwd_encrypt)
            {
                pwd = BuildAccount.getAutoPassword(6);
                out_pwd = AESHelper.AESEncrypt(pwd, AES_KEY);
                save_pwd = AESHelper.MD5Encrypt(pwd);
            }
            else
            {
                out_pwd = BuildAccount.getAutoPassword(20);
                pwd = string.Format("{0}{1}{2}{3}{4}{5}", out_pwd[8], out_pwd[16], out_pwd[4], out_pwd[11], out_pwd[2], out_pwd[9]);//password
                save_pwd = AESHelper.MD5Encrypt(pwd);
            }
            string deviceID = Request.Params["deviceID"];
            string channelID = Request.Params["channelID"];

            string remoteIP = Common.Helper.getRemoteIP(Request);
            Random rd = new Random();
            int randkey = rd.Next();
            Dictionary<string, object> updata = new Dictionary<string, object>();
            updata["acc"] = acc;
            updata["pwd"] = save_pwd;
            DateTime now = DateTime.Now;
            updata["randkey"] = randkey;
            updata["regedittime"] = now;
            updata["regeditip"] = remoteIP;
            updata["updatepwd"] = false;
            updata["platform"] = platform;
            updata["channelID"] = channelID;
            updata["lasttime"] = now.Ticks;
            updata["lastip"] = remoteIP;

            string strerr = MongodbAccount.Instance.ExecuteStoreBykey(table, "acc", acc, updata);
            if (strerr != "")
            {
                Response.Write("local ret = {code = -11}; return ret;"); 
            }
            else
            {
                Dictionary<string, object> savelog = new Dictionary<string, object>();
                savelog["acc"] = acc;
                savelog["acc_real"] = acc;
                if (!string.IsNullOrEmpty(deviceID))
                {
                    savelog["acc_dev"] = deviceID;
                }
                savelog["ip"] = remoteIP;
                savelog["time"] = now;
                savelog["channel"] = channelID;
                MongodbAccount.Instance.ExecuteInsert("RegisterLog", savelog);

                //渠道每日注册
                if (string.IsNullOrEmpty(channelID) == false)
                {
                    MongodbAccount.Instance.ExecuteIncBykey("day_regedit", "date", DateTime.Now.Date, channelID, 0);
                }
                string ret = string.Format("local ret = {{code = 0, acc=\"{0}\", pwd=\"{1}\"}}; return ret;", acc, out_pwd);
                Response.Write(ret); 
            }   

            //Response.Write("local ret = {code = 0, acc=\"fish000001\", pwd=\"123456\"};"); 
        }
    }
}