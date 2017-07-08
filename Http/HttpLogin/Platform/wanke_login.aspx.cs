using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Specialized;
using Common;

using System.Configuration;
using System.Text;

namespace HttpLogin.Platform
{
    public partial class wanke_login : System.Web.UI.Page
    {
        static string AES_KEY = "&@*(#kas9081fajk";

        protected void Page_Load(object sender, EventArgs e)
        {
            string platform = Request.Form["platform"];
            string acc = Request.Form["acc"];
            string logintime = Request.Form["logintime"];
            string sign = Request.Form["sign"];
            string loginkey = Request.Form["loginkey"];
            if (string.IsNullOrEmpty(platform))
            {
                Response.Write(Helper.buildLuaReturn(-1, "platform is empty"));
                return;
            }
            if (string.IsNullOrEmpty(acc))
            {
                Response.Write(Helper.buildLuaReturn(-1, "acc is empty"));
                return;
            }
            if (string.IsNullOrEmpty(logintime))
            {
                Response.Write(Helper.buildLuaReturn(-1, "logintime is empty"));
                return;
            }
            if (string.IsNullOrEmpty(sign))
            {
                Response.Write(Helper.buildLuaReturn(-1, "sign is empty"));
                return;
            }
            if (string.IsNullOrEmpty(loginkey))
            {
                Response.Write(Helper.buildLuaReturn(-1, "loginkey is empty"));
                return;
            }

            string rsakey = Encoding.Default.GetString(Convert.FromBase64String(loginkey));
            rsakey = AESHelper.AESDecrypt(rsakey, AES_KEY);

            string source = string.Format("username={0}&appkey=3f2fadb37dd503fe686cdfb33ab8c095&logintime={1}", acc, logintime);
            if (Helper.checkMD5(source, sign))
            {
                string acc_table = "wanke_acc";
                string pwd = Helper.getMD5("123456");
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
                            string deviceID = Request.Params["deviceID"];
                            if (!string.IsNullOrEmpty(deviceID))
                            {
                                savelog["acc_dev"] = deviceID;
                            }
                            savelog["ip"] = Request.ServerVariables.Get("Remote_Addr").ToString();
                            savelog["time"] = now;
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
                        string deviceID = Request.Params["deviceID"];
                        if (!string.IsNullOrEmpty(deviceID))
                        {
                            savelog["acc_dev"] = deviceID;
                        }
                        savelog["ip"] = Request.ServerVariables.Get("Remote_Addr").ToString();
                        savelog["time"] = now;
                        MongodbAccount.Instance.ExecuteInsert("LoginLog", savelog);
                    }
                }
            }
            else
            {
                Response.Write(Helper.buildLuaReturn(-2, "sign is error"));
                return;
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