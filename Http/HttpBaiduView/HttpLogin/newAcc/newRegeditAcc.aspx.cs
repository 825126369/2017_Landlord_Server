using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Text;
using System.Configuration;

using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Bson;

namespace HttpLogin.Default
{
    public partial class newRegeditAcc : System.Web.UI.Page
    {
        const string AES_KEY = "&@*(#kas9081fajk";
        bool returnLua = false;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.RequestType != "GET")
                return;

            //结果用lua返回
            string retLua = Request.Params["returnLua"];
            if (!string.IsNullOrEmpty(retLua) && retLua == "true")
            {
                returnLua = true;
            }
            string strdata = Request.Params["data"];
            string strmd5 = Request.Params["sign"];
            if (string.IsNullOrEmpty(strdata) || string.IsNullOrEmpty(strmd5))
            {
                buildReturnMsg("-1");//data is null
                return;
            }

            strdata = Encoding.Default.GetString(Convert.FromBase64String(strdata));

            if (strmd5 != AESHelper.MD5Encrypt(strdata + AES_KEY))
            {
                buildReturnMsg("-2");//sign error
                return;
            }

            Dictionary<string, object> data = JsonHelper.ParseFromStr<Dictionary<string, object>>(strdata);
            if (data == null || data.Count < 3)
            {
                buildReturnMsg("-3");//json error
                return;
            }
            try
            {
                CheckAccount(data);
            }
            catch (Exception)
            {
                buildReturnMsg("-4");
            }
        }


        void buildReturnMsg(string info, bool ret = false, string acc = "")
        {
            if (returnLua)
            {
                ReturnLuaMsg(info, ret, acc);
            }
            else
            {
                ReturnMsg(info, ret, acc);
            }
        }

        void ReturnLuaMsg(string info, bool ret = false, string acc = "")
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("ret = {};");
            sb.AppendFormat("ret.result = {0};", ret.ToString().ToLower());
            sb.AppendFormat("ret.data = \"{0}\";", info);
            sb.AppendFormat("ret.acc_real = \"{0}\";", acc);
            sb.Append("return ret;");
            Response.Write(sb.ToString());
        }

        void ReturnMsg(string info, bool ret = false, string acc = "")
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data["result"] = ret;
            if (ret)
            {
                data["data"] = info;
                data["acc_real"] = acc;
            }
            else
                data["error"] = info;

            string jsondata = JsonHelper.ConvertToStr(data);
            Response.Write(Convert.ToBase64String(Encoding.Default.GetBytes(jsondata)));
        }

        void CheckAccount(Dictionary<string, object> data)
        {
            string platform = Request.Params["platform"];
            if (string.IsNullOrEmpty(platform))
            {
                buildReturnMsg("-1");//data error
                return;
            }

            string table = ConfigurationManager.AppSettings["acc_" + platform];
            if (string.IsNullOrEmpty(table))
            {
                buildReturnMsg("-15");//platform error
                return;
            }

            string acc_reg = data["n1"].ToString();//account
            
            string pattern = @"^[0-9a-zA-Z]{6,30}$";

            if (!Regex.IsMatch(acc_reg, pattern))
            {
                buildReturnMsg("-12");//account error
                return;
            }

            if (MongodbAccount.Instance.KeyExistsBykey(table, "acc", acc_reg))
            {
                buildReturnMsg("-12");//account exists
                return;
            }

            string spwd = AESHelper.AESDecrypt(data["n2"].ToString(), AES_KEY);//password
            if (spwd.Length != 32)//md5
            {
                buildReturnMsg("-14");//pwd error
                return;
            }

            Random rd = new Random();
            int randkey = rd.Next();
            Dictionary<string, object> updata = new Dictionary<string, object>();
            updata["acc"] = acc_reg;
            updata["acc_real"] = Guid.NewGuid().ToString().Replace("-","");
            updata["pwd"] = spwd;
            DateTime now = DateTime.Now;
            updata["randkey"] = randkey;
            updata["lasttime"] = now.Ticks;
            updata["regedittime"] = now;
            updata["regeditip"] = Request.ServerVariables.Get("Remote_Addr").ToString();
            updata["updatepwd"] = false;
            //updata["platform"] = Platform;


            string strerr = MongodbAccount.Instance.ExecuteStoreBykey(table, "acc", acc_reg, updata);
            if (strerr != "")
            {
                buildReturnMsg("-11");//server error
            }
            else
            {
                RSAHelper rsa = new RSAHelper();
                rsa.setModulus(data["n3"].ToString());

                string clientkey = randkey.ToString() + ":" + now.Ticks.ToString();
                buildReturnMsg(rsa.RSAEncryptStr(clientkey), true, updata["acc_real"].ToString());//login success

                string channelID = null;
                if (data.ContainsKey("n4"))
                {
                    channelID = data["n4"].ToString();
                }

                Dictionary<string, object> savelog = new Dictionary<string, object>();
                savelog["acc_real"] = updata["acc_real"].ToString();
                savelog["acc"] = acc_reg;
                string deviceID = Request.Params["deviceID"];
                if (!string.IsNullOrEmpty(deviceID))
                {
                    savelog["acc_dev"] = deviceID;
                }
                savelog["ip"] = Request.ServerVariables.Get("Remote_Addr").ToString();
                savelog["time"] = now;
                savelog["channel"] = channelID;
                MongodbAccount.Instance.ExecuteInsert("LoginLog", savelog);
                MongodbAccount.Instance.ExecuteInsert("RegisterLog", savelog);

                //渠道每日注册
                if (data.ContainsKey("n4"))
                {
                    MongodbAccount.Instance.ExecuteIncBykey("day_regedit", "date", DateTime.Now.Date, data["n4"].ToString(), 0);
                }
            }
        }
    }
}