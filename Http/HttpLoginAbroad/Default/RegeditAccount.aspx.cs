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

namespace HttpLogin
{
    public partial class RegeditAccount : System.Web.UI.Page
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
            string platform = Request.Params["platform"];
            if (string.IsNullOrEmpty(platform))
            {
                ReturnMsg(HttpRetCode.RET_PARAM_NOT_VALID.ToString());//data error
                return;
            }

            string table = ConfigurationManager.AppSettings["acc_" + platform];
            if (string.IsNullOrEmpty(table))
            {
                ReturnMsg(HttpRetCode.RET_PLATFORM_ERROR.ToString());//platform error
                return;
            }

            string sacc = data["n1"].ToString();//account

            //List<IMongoQuery> imqs = new List<IMongoQuery>();
            //imqs.Add(Query.EQ("acc", sacc));
            //imqs.Add(Query.EQ("platform", Platform));

           // string pattern = @"^[0-9a-zA-Z]{6,30}$";

            if (!Regex.IsMatch(sacc, Exp.ACCOUNT_PLAYER))
            {
                ReturnMsg(HttpRetCode.RET_ACC_ERROR.ToString());//account error
                return;
            }

            if (MongodbAccount.Instance.KeyExistsBykey(table, "acc", sacc))
            {
                ReturnMsg(HttpRetCode.RET_ACC_EXISTS.ToString());//account exists
                return;
            }

            string spwd = AESHelper.AESDecrypt(data["n2"].ToString(), AES_KEY);//password
            if (spwd.Length != 32)//md5
            {
                ReturnMsg(HttpRetCode.RET_PWD_ERROR.ToString());//pwd error
                return;
            }
            
            Random rd = new Random();
            int randkey = rd.Next();
            Dictionary<string, object> updata = new Dictionary<string, object>();
            updata["acc"] = sacc;
            updata["pwd"] = spwd;
            DateTime now = DateTime.Now;
            updata["randkey"] = randkey;
            updata["lasttime"] = now.Ticks;
            updata["regedittime"] = now;
            updata["regeditip"] = Request.ServerVariables.Get("Remote_Addr").ToString();
            updata["updatepwd"] = false;
            //updata["platform"] = Platform;


            string strerr = MongodbAccount.Instance.ExecuteStoreBykey(table, "acc", sacc, updata);
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

                //渠道每日注册
                if (data.ContainsKey("n4"))
                {
                    MongodbAccount.Instance.ExecuteIncBykey("day_regedit", "date", DateTime.Now.Date, data["n4"].ToString(), 0);
                }
            }          
        }
    }
}