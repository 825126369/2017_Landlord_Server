using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Text.RegularExpressions;
using System.Configuration;
using Common;

using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace HttpLogin.thirdsdk
{
    public partial class ThirdLogin : System.Web.UI.Page
    {
        const string AES_KEY = "&@*(#kas9081fajk";

        protected void Page_Load(object sender, EventArgs e)
        {
            string strdata = Request.Params["data"];
            string strmd5 = Request.Params["sign"];
            if (string.IsNullOrEmpty(strdata) || string.IsNullOrEmpty(strmd5))
            {
                ReturnLuaMsg("-1");//data is null
                return;
            }

            strdata = Encoding.Default.GetString(Convert.FromBase64String(strdata));

            if (strmd5 != AESHelper.MD5Encrypt(strdata + AES_KEY))
            {
                ReturnLuaMsg("-2");//sign error
                return;
            }

            Dictionary<string, object> data = JsonHelper.ParseFromStr<Dictionary<string, object>>(strdata);
            if (data == null || data.Count < 3)
            {
                ReturnLuaMsg("-3");//json error
                return;
            }
            try
            {
                CheckAccount(data);
            }
            catch (Exception)
            {
                ReturnLuaMsg("-4");
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

        void CheckAccount(Dictionary<string, object> data)
        {
            string sacc = data["n1"].ToString();//account
            string spwd = AESHelper.AESDecrypt(data["n2"].ToString(), AES_KEY);//password
            if (spwd.Length != 32)//md5
            {
                ReturnLuaMsg("-14");//pwd error
                return;
            }

            string platform = Request.Params["platform"];

            string table = ConfigurationManager.AppSettings["acc_" + platform];
            if (string.IsNullOrEmpty(table))
            {
                ReturnLuaMsg("-15");//platform error
                return;
            }

            List<IMongoQuery> imqs = new List<IMongoQuery>();
            imqs.Add(Query.EQ("acc", sacc));
            imqs.Add(Query.EQ("pwd", spwd));
            int retCode = tryLogin(sacc, spwd, table);
            //if (MongodbAccount.Instance.KeyExistsByQuery(table, Query.And(imqs)))
            if (retCode == 0)
            {
                Random rd = new Random();
                int randkey = rd.Next();
                Dictionary<string, object> updata = new Dictionary<string, object>();
                DateTime now = DateTime.Now;
                updata["randkey"] = randkey;
                updata["lasttime"] = now.Ticks;
                updata["lastip"] = Request.ServerVariables.Get("Remote_Addr").ToString();
                string strerr = MongodbAccount.Instance.ExecuteUpdate(table, "acc", sacc, updata);
                if (strerr != "")
                {
                    ReturnLuaMsg("-11");//server error
                }
                else
                {
                    string channelID = null;
                    if (data.ContainsKey("n4"))
                    {
                        channelID = data["n4"].ToString();
                    }

                    Dictionary<string, object> savelog = new Dictionary<string, object>();
                    savelog["acc"] = sacc;
                    savelog["acc_real"] = sacc;
                    string deviceID = Request.Params["deviceID"];
                    if (!string.IsNullOrEmpty(deviceID))
                    {
                        savelog["acc_dev"] = deviceID;
                    }
                    savelog["ip"] = Request.ServerVariables.Get("Remote_Addr").ToString();
                    savelog["time"] = now;
                    savelog["channel"] = channelID;
                    MongodbAccount.Instance.ExecuteInsert("LoginLog", savelog);

                    string clientkey = randkey.ToString() + ":" + now.Ticks.ToString();
                    string loginKey = AESHelper.AESEncrypt(clientkey, AES_KEY);
                    ReturnLuaMsg(loginKey, true);
                }
            }
            else
            {
                // ReturnMsg("-10");//acc or pwd error
                ReturnLuaMsg(retCode.ToString());
            }
        }

        // 尝试登录
        // 返回 0正常  -10 账号或密码错误 -12 账号被冻结 -11 db服务器出错 
        // -16 未修改密码 -17 账号被停封
        int tryLogin(string acc, string pwd, string table)
        {
            int retCode = -10;
            Dictionary<string, object> data = MongodbAccount.Instance.ExecuteGetBykey(table, "acc", acc, CONST.LOGIN_FAILED_FIELD);
            if (data == null)
                return retCode;

            bool checkpwd = Convert.ToBoolean(ConfigurationManager.AppSettings["check_pwd"]);
            if (checkpwd)
            {
                if (data.ContainsKey("updatepwd") && !Convert.ToBoolean(data["updatepwd"]))
                    return -16;//未修改密码
            }

            if (data.ContainsKey("block"))
            {
                bool isBlock = Convert.ToBoolean(data["block"]);
                if (isBlock)
                    return -17;
            }

            int curFailedCnt = 0;
            if (data.ContainsKey("loginFailedCount"))
            {
                if (CONST.USE_LOGIN_FAILED_COUNT_CHECK == 1) // 启用了失败次数检测
                {
                    DateTime cur = DateTime.Now.Date;
                    if (data.ContainsKey("loginFailedDate"))
                    {
                        DateTime Last = Convert.ToDateTime(data["loginFailedDate"]).ToLocalTime();

                        if (cur == Last)
                        {
                            curFailedCnt = Convert.ToInt32(data["loginFailedCount"]);
                            if (curFailedCnt >= CONST.LOGIN_FAILED_MAX_COUNT) // 账号被冻结了
                                return -21;
                        }
                    }
                }
            }

            string dbPwd = Convert.ToString(data["pwd"]);
            if (pwd == dbPwd)
            {
                retCode = 0;
                curFailedCnt = 0;
            }
            else
            {
                curFailedCnt++;
            }

            if (CONST.USE_LOGIN_FAILED_COUNT_CHECK == 1)
            {
                Dictionary<string, object> updata = new Dictionary<string, object>();
                updata.Add("loginFailedDate", DateTime.Now.Date);
                updata.Add("loginFailedCount", curFailedCnt);

                string strerr = MongodbAccount.Instance.ExecuteUpdate(table, "acc", acc, updata);
                if (strerr != "")
                {
                    retCode = -11;
                }
            }

            return retCode;
        }
    }
}