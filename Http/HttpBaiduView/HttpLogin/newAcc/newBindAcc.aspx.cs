using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Bson;

namespace HttpLogin.newAcc
{
    public partial class newBindAcc : System.Web.UI.Page
    {
        const string AES_KEY = "&@*(#kas9081fajk";
        const string ACC_CHECK = "N(FA*&$12flaoi78";
        const string RAND_KEY = "abcdefghijklmnopqrstuvwxyz0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
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
                BindAccount(data);
            }
            catch (Exception e2)
            {
                buildReturnMsg("-4");
            }
        }

        void buildReturnMsg(string info, bool ret = false)
        {
            if (returnLua)
            {
                ReturnLuaMsg(info, ret);
            }
            else
            {
                ReturnMsg(info, ret);
            }
        }

        void ReturnLuaMsg(string info, bool ret = false)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("ret = {};");
            sb.AppendFormat("ret.result = {0};", ret.ToString().ToLower());
            sb.AppendFormat("ret.data = \"{0}\";", info);
            sb.Append("return ret;");
            Response.Write(sb.ToString());
        }

        void ReturnMsg(string info, bool ret = false)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data["result"] = ret;
            if (ret)
            {
                data["data"] = info;
            }
            else
                data["error"] = info;

            string jsondata = JsonHelper.ConvertToStr(data);
            Response.Write(Convert.ToBase64String(Encoding.Default.GetBytes(jsondata)));
        }

        void BindAccount(Dictionary<string, object> data)
        {
            string HardwareID = data["n4"].ToString();//HardwareID

            if (string.IsNullOrEmpty(HardwareID))
            {
                buildReturnMsg("-12");//account error
                return;
            }

            string platform = Request.Params["platform"];

            string table = ConfigurationManager.AppSettings["acc_" + platform];
            if (string.IsNullOrEmpty(table))
            {
                buildReturnMsg("-15");//platform error
                return;
            }

            var retdata = MongodbAccount.Instance.ExecuteGetByQuery(table, Query.EQ("acc_dev", HardwareID), new string[] { "acc", "lasttime" });

            if (retdata != null)
            {
                string sacc = data["n1"].ToString();//account

                string pattern = @"^[0-9a-zA-Z]{6,30}$";

                if (!Regex.IsMatch(sacc, pattern))
                {
                    buildReturnMsg("-12");//account error
                    return;
                }

                //判断此账号已经被注册
                if (MongodbAccount.Instance.KeyExistsBykey(table, "acc", sacc))
                {
                    buildReturnMsg("-13");//account exists
                    return;
                }

                string spwd = AESHelper.AESDecrypt(data["n2"].ToString(), AES_KEY);//password
                if (spwd.Length != 32)//md5
                {
                    buildReturnMsg("-14");//pwd error
                    return;
                }

                //判断是否绑定过
                if (retdata.ContainsKey("acc"))
                {
                    string findacc = retdata["acc"].ToString();
                    if (!string.IsNullOrEmpty(findacc))
                    {
                        buildReturnMsg("-13");//account is error
                        return;
                    }
                }

                //校验玩家必须登录成功后才能绑定
                string token = data["n3"].ToString();
                if (retdata.ContainsKey("lasttime"))
                {
                    string lasttime = retdata["lasttime"].ToString();
                    lasttime = AESHelper.MD5Encrypt(lasttime + ACC_CHECK);
                    token = AESHelper.AESDecrypt(token, ACC_CHECK);
                    if (token != lasttime || string.IsNullOrEmpty(lasttime))
                    {
                        buildReturnMsg("-14");//pwd error
                        return;
                    }
                }
                else
                {
                    buildReturnMsg("-14");//pwd error
                    return;
                }
                   
                
                Dictionary<string, object> updata = new Dictionary<string, object>();
                DateTime now = DateTime.Now;
                updata["acc"] = sacc;
                updata["pwd"] = spwd;
                updata["bindtime"] = now;
                string strerr = MongodbAccount.Instance.ExecuteUpdate(table, "acc_dev", HardwareID, updata);
                if (strerr != "")
                {
                    buildReturnMsg("-11");//server error
                }
                else
                {                    
                    buildReturnMsg("ok", true);//login success                    

                    Dictionary<string, object> savelog = new Dictionary<string, object>();
                    savelog["acc_dev"] = HardwareID;
                    savelog["acc"] = sacc;
                    savelog["time"] = now;
                    savelog["ip"] = Request.ServerVariables.Get("Remote_Addr").ToString();
                    MongodbAccount.Instance.ExecuteInsert("BindLog", savelog);
                }
            }
            else
            {
                // ReturnMsg("-10");//acc or pwd error
                buildReturnMsg("-12");
            }
        }

       
    }
}