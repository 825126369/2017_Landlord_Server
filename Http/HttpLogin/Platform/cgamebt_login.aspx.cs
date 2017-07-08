using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Configuration;

using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace HttpLogin.Platform
{
    //用于cgamebt登陆
    public partial class cgamebt_login : System.Web.UI.Page
    {
        static string AES_KEY = "&@*(#kas9081fajk";

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                string platform = Request.Form["platform"];
                string loginKey = Request.Form["loginkey"];
                string deviceID = Request.Form["DeviceID"];
                if (string.IsNullOrEmpty(deviceID))
                {
                    deviceID = "";
                }
                if (string.IsNullOrEmpty(loginKey))
                {
                    Response.Write(BuildAccount.buildLuaReturn(-1, "data error"));
                    return;
                }
                string dataStr = Encoding.Default.GetString(Convert.FromBase64String(loginKey));
                dataStr = AESHelper.AESDecrypt(dataStr, AES_KEY);
                string[] data = dataStr.Split(':');
                if (data.Length < 3)
                {
                    Response.Write(BuildAccount.buildLuaReturn(-2, "data error"));
                    return;
                }

                string acc = data[0];
                string pwd = data[1];
                string rsakey = data[2];

                //帐号表
                string acc_table = "cgamebt_acc";//ConfigurationManager.AppSettings["acc_cgamebt"];

                //List<IMongoQuery> imqs = new List<IMongoQuery>();
                //imqs.Add(Query.EQ("acc", acc));
                //imqs.Add(Query.EQ("platform", platform));

                //判断是否存在帐号
                if (MongodbAccount.Instance.KeyExistsBykey(acc_table, "acc", acc))
                {
                    //检测帐号是否能登陆
                    int retCode = tryLogin(acc, pwd, acc_table);
                    if (retCode == 0)
                    {
                        Random rd = new Random();
                        int randkey = rd.Next();
                        Dictionary<string, object> updata = new Dictionary<string, object>();
                        DateTime now = DateTime.Now;
                        updata["randkey"] = randkey;
                        updata["lasttime"] = now.Ticks;
                        updata["lastip"] = Request.ServerVariables.Get("Remote_Addr").ToString();
                        string strerr = MongodbAccount.Instance.ExecuteUpdate(acc_table, "acc", acc, updata);
                        if (strerr != "")
                        {
                            Response.Write(BuildAccount.buildLuaReturn(-11, "server error"));
                        }
                        else
                        {
                            RSAHelper rsa = new RSAHelper();
                            rsa.setModulus(rsakey);

                            string clientkey = randkey.ToString() + ":" + now.Ticks.ToString();
                            Response.Write(BuildAccount.buildLuaReturn(0, AESHelper.AESEncrypt(clientkey, AES_KEY)));

                            Dictionary<string, object> savelog = new Dictionary<string, object>();
                            savelog["acc"] = acc;
                            savelog["acc_real"] = acc;
                            savelog["ip"] = Request.ServerVariables.Get("Remote_Addr").ToString();
                            savelog["time"] = now;
                            savelog["DeviceID"] = deviceID;
                            MongodbAccount.Instance.ExecuteInsert("LoginLog", savelog);
                        }
                    }
                    else
                    {
                        Response.Write(BuildAccount.buildLuaReturn(retCode, "acc error"));
                    }
                }
                else
                {
                    //注册新帐号
                    Random rd = new Random();
                    int randkey = rd.Next();
                    Dictionary<string, object> updata = new Dictionary<string, object>();
                    updata["acc"] = acc;
                    updata["pwd"] = pwd;
                    DateTime now = DateTime.Now;
                    updata["randkey"] = randkey;
                    updata["lasttime"] = now.Ticks;
                    updata["regedittime"] = now;
                    updata["regeditip"] = Request.ServerVariables.Get("Remote_Addr").ToString();
                    updata["updatepwd"] = false;
                    updata["platform"] = platform;

                    string strerr = MongodbAccount.Instance.ExecuteStoreBykey(acc_table, "acc", acc, updata);
                    if (strerr != "")
                    {
                        Response.Write(BuildAccount.buildLuaReturn(-11, "server error"));
                    }
                    else
                    {
                        RSAHelper rsa = new RSAHelper();
                        rsa.setModulus(rsakey);

                        string clientkey = randkey.ToString() + ":" + now.Ticks.ToString();
                        Response.Write(BuildAccount.buildLuaReturn(0, AESHelper.AESEncrypt(clientkey, AES_KEY)));

                        Dictionary<string, object> savelog = new Dictionary<string, object>();
                        savelog["acc"] = acc;
                        savelog["acc_real"] = acc;
                        savelog["ip"] = Request.ServerVariables.Get("Remote_Addr").ToString();
                        savelog["time"] = now;
                        savelog["DeviceID"] = deviceID;
                        MongodbAccount.Instance.ExecuteInsert("LoginLog", savelog);
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Write(BuildAccount.buildLuaReturn(-1, ex.Message));
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
                                return -12;
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