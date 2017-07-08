using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Web.Configuration;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections.Specialized;

namespace HttpLogin.Default
{
    public partial class UpdatePwd : System.Web.UI.Page
    {
        const string AES_KEY = "&@*(#kas9081fajk";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.RequestType != "GET")
                return;

            string strdata = Request.Params["data"];
            string strmd5 = Request.Params["sign"];
            if (string.IsNullOrEmpty(strdata) || string.IsNullOrEmpty(strmd5))            
                return;            

            strdata = Encoding.Default.GetString(Convert.FromBase64String(strdata));   

            if (strmd5 != AESHelper.MD5Encrypt(strdata + AES_KEY))
            {
                Response.Write("err_sign_error");//sign error
                return;
            }

            Dictionary<string, object> data = JsonHelper.ParseFromStr<Dictionary<string, object>>(strdata);
            if (data == null || data.Count < 3)
            {
                Response.Write("err_data_error");//json error
                return;
            }
            try
            {
                UpdateAccount(data);
            }
            catch (Exception ex)
            {
                Response.Write(ex.ToString());
            }
        }

        void UpdateAccount(Dictionary<string, object> cinfo)
        {
            string strphone = cinfo["n1"].ToString();
            if (strphone.Length != 11 || !Regex.IsMatch(strphone, @"^\d{11}$"))
            {
                Response.Write("err_not_phone");//号码错误
                return;
            }

            string[] field = { "acc", "pwdcode" };
            Dictionary<string, object> data = MongodbAccount.Instance.ExecuteGetBykey("AccountTable", "bindPhone", strphone, field);
            if (data == null || data.Count != 2)
            {
                Response.Write("err_not_bind");//未绑定  
            }
            else
            {
                if (cinfo["n2"].ToString() != data["pwdcode"].ToString())
                {
                    Response.Write("err_code_error");//验证码错误
                    return;
                }

                string spwd = AESHelper.AESDecrypt(cinfo["n3"].ToString(), AES_KEY);//password
                if (spwd.Length != 32)//md5
                {
                    Response.Write("err_pwd_error");//密码错误
                    return;
                }

                Dictionary<string, object> updata = new Dictionary<string,object>();
                updata["pwd"] = spwd;
                updata["pwdcode"] = "";
                string ret = MongodbAccount.Instance.ExecuteUpdate("AccountTable", "acc", data["acc"].ToString(), updata);
                if (ret == "")
                {
                    sendMsgToPhone(strphone, data["acc"].ToString());
                    Response.Write("err_success");
                }
                else
                    Response.Write("err_system_error");
            }
        }

        // 向手机phone发送验证码code
        private string sendMsgToPhone(string phone, string account)
        {
            return PhoneCom.sendSearchAccount(phone, account);
        }    
    }
}