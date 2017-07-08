using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Text;

using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Bson;


namespace HttpLogin.Default
{
    public partial class Login : System.Web.UI.Page
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
                data["data"] = info;
            else
                data["error"] = info;

            string jsondata = JsonHelper.ConvertToStr(data);
            Response.Write(Convert.ToBase64String(Encoding.Default.GetBytes(jsondata)));            
        }

        void CheckAccount(Dictionary<string, object> data)
        {
            string sacc = data["n1"].ToString();//account
            string spwd = AESHelper.AESDecrypt(data["n2"].ToString(), AES_KEY);//password
            if (spwd.Length != 32)//md5
            {
                buildReturnMsg("-14");//pwd error
                return;
            }

            string platform = Request.Params["platform"];

            string table = ConfigurationManager.AppSettings["acc_" + platform];
            if (string.IsNullOrEmpty(table))
            {
                buildReturnMsg("-15");//platform error
                return;
            }

            string pattern = @"^[0-9a-zA-Z]{6,30}$";
            if (!Regex.IsMatch(sacc, pattern))
            {
                buildReturnMsg("-15");//account error
                return;
            }
            string remoteIP = Common.Helper.getRemoteIP(Request);
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
                //List<IMongoQuery> imqs2 = new List<IMongoQuery>();
                //imqs2.Add(Query.EQ("acc", sacc));
                //imqs2.Add(Query.EQ("platform", Platform));
                updata["lastip"] = remoteIP;
                string strerr = MongodbAccount.Instance.ExecuteUpdate(table, "acc", sacc, updata);
                if (strerr != "")
                {
                    buildReturnMsg("-11");//server error
                }
                else
                {
                    RSAHelper rsa = new RSAHelper();
                    rsa.setModulus(data["n3"].ToString());

                    string clientkey = randkey.ToString() + ":" + now.Ticks.ToString();
                    buildReturnMsg(rsa.RSAEncryptStr(clientkey), true);//login success

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
                    savelog["ip"] = remoteIP;
                    savelog["time"] = now;
                    savelog["channel"] = channelID;
                    MongodbAccount.Instance.ExecuteInsert("LoginLog", savelog);
                }
            }
            else
            {
               // ReturnMsg("-10");//acc or pwd error
                buildReturnMsg(retCode.ToString());
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
            if (checkpwd )
            {
                if(data.ContainsKey("updatepwd") && !Convert.ToBoolean(data["updatepwd"]))
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