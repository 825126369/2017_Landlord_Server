using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;

using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Bson;


namespace HttpLogin.Default
{
    public partial class Login : System.Web.UI.Page
    {
        const string AES_KEY = "&@*(#kas9081fajk";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.RequestType != "GET")
                return;

            string strdata = Request.Params["data"];
            string strmd5 = Request.Params["sign"];     
            if (string.IsNullOrEmpty(strdata) || string.IsNullOrEmpty(strmd5))
            {
                ReturnMsg(HttpRetCode.RET_PARAM_NOT_VALID.ToString());//data is null
                return;
            }

            strdata = Encoding.Default.GetString(Convert.FromBase64String(strdata));     

            if (strmd5 != AESHelper.MD5Encrypt(strdata + AES_KEY))
            {
                ReturnMsg(HttpRetCode.RET_SIGN_ERROR.ToString());//sign error
                return;
            }

            Dictionary<string, object> data = JsonHelper.ParseFromStr<Dictionary<string, object>>(strdata);
            if (data == null || data.Count < 3)
            {
                ReturnMsg(HttpRetCode.RET_JSON_ERROR.ToString());//json error
                return;
            }
            try
            {
                CheckAccount(data);           
            }
            catch (Exception)
            {
                ReturnMsg(HttpRetCode.RET_HAPPEN_EXCEPTION.ToString());
            }            
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
                ReturnMsg(HttpRetCode.RET_PWD_ERROR.ToString());//pwd error
                return;
            }

            string platform = Request.Params["platform"];

            string table = ConfigurationManager.AppSettings["acc_" + platform];
            if (string.IsNullOrEmpty(table))
            {
                ReturnMsg(HttpRetCode.RET_PLATFORM_ERROR.ToString());//platform error
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
                //List<IMongoQuery> imqs2 = new List<IMongoQuery>();
                //imqs2.Add(Query.EQ("acc", sacc));
                //imqs2.Add(Query.EQ("platform", Platform));
                updata["lastip"] = Request.ServerVariables.Get("Remote_Addr").ToString();
                string strerr = MongodbAccount.Instance.ExecuteUpdate(table, "acc", sacc, updata);
                if (strerr != "")
                {
                    ReturnMsg(HttpRetCode.RET_DB_ERROR.ToString());//server error
                }
                else
                {
                    RSAHelper rsa = new RSAHelper();
                    rsa.setModulus(data["n3"].ToString());

                    string clientkey = randkey.ToString() + ":" + now.Ticks.ToString();                    
                    ReturnMsg(rsa.RSAEncryptStr(clientkey), true);//login success

                    Dictionary<string, object> savelog = new Dictionary<string, object>();
                    savelog["acc"] = sacc;
                    savelog["ip"] = Request.ServerVariables.Get("Remote_Addr").ToString();
                    savelog["time"] = now;                    
                    MongodbAccount.Instance.ExecuteInsert("LoginLog", savelog);
                }
            }
            else
            {
               // ReturnMsg("-10");//acc or pwd error
                ReturnMsg(retCode.ToString());
            }
        }

        // 尝试登录
        // 返回 0正常 
        int tryLogin(string acc, string pwd, string table)
        {
            int retCode = HttpRetCode.RET_ACC_OR_PWD_ERROR;
            Dictionary<string, object> data = MongodbAccount.Instance.ExecuteGetBykey(table, "acc", acc, CONST.LOGIN_FAILED_FIELD);
            if (data == null)
                return retCode;

            bool checkpwd = Convert.ToBoolean(ConfigurationManager.AppSettings["check_pwd"]);
            if (checkpwd )
            {
                if (data.ContainsKey("updatepwd") && !Convert.ToBoolean(data["updatepwd"]))
                    return HttpRetCode.RET_NOT_MODIFY_PWD;
            }

            if (data.ContainsKey("block"))
            {
                bool isBlock = Convert.ToBoolean(data["block"]);
                if (isBlock)
                    return HttpRetCode.RET_ACC_BLOCK;
            }

            int curFailedCnt = 0;
            if (data.ContainsKey("loginFailedCount"))
            {
                if (CONST.USE_LOGIN_FAILED_COUNT_CHECK == 1) // 启用了失败次数检测
                {
                    DateTime cur = DateTime.Now;
                    if (data.ContainsKey("loginFailedDate"))
                    {
                        curFailedCnt = Convert.ToInt32(data["loginFailedCount"]);
                        if (curFailedCnt >= CONST.LOGIN_FAILED_MAX_COUNT) // 账号被冻结了
                        {
                            DateTime Last = Convert.ToDateTime(data["loginFailedDate"]).ToLocalTime();
                            TimeSpan span = cur - Last;
                            if (span.TotalMinutes < CONST.ACC_FREEZE_TIME) // 15分钟内不能再次登录
                            {
                                return HttpRetCode.RET_ACC_FREEZE;
                            }
                            else
                            {
                                curFailedCnt = 0; // 超过了15分钟，清0
                            }
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
                updata.Add("loginFailedDate", DateTime.Now);
                updata.Add("loginFailedCount", curFailedCnt);

                string strerr = MongodbAccount.Instance.ExecuteUpdate(table, "acc", acc, updata);
                if (strerr != "")
                {
                    retCode = HttpRetCode.RET_DB_ERROR;
                }
            }

            return retCode;
        }
    }
}