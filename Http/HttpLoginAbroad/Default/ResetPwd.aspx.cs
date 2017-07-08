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
    public partial class ResetPwd : System.Web.UI.Page
    {
        const string AES_KEY = "959D!@23ia@!#86e";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.RequestType != "GET")
                return;

            string strdata = Request.Params["data"];
            string strmd5 = Request.Params["sign"];
            if (string.IsNullOrEmpty(strdata) || string.IsNullOrEmpty(strmd5))
                return;

            // 改密码的发起源
            string opSrc = Request.Params["opSrc"];

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
                UpdateAccount(data, opSrc);
            }
            catch (Exception ex)
            {
                Response.Write(ex.ToString());
            }
        }

        void UpdateAccount(Dictionary<string, object> cinfo, string opSrc)
        {
            string acc = cinfo["n1"].ToString();
            if (string.IsNullOrEmpty(acc))
            {
                Response.Write("err_data_error");//号码错误
                return;
            }

            string[] field = { "pwd" };
            Dictionary<string, object> data = MongodbAccount.Instance.ExecuteGetBykey("AccountTable", "acc", acc, field);
            if (data == null)
            {
                Response.Write("err_not_acc");//找不到账号 
            }
            else
            {
                // 玩家账号密码不能在游戏客户端修改，但可以通过API接口来改
                if (opSrc != CC.RESET_MODIFY_BY_API)
                {
                    DyOpModifyPlayerPwd dy = new DyOpModifyPlayerPwd();
                    bool canModify = dy.canModifyPwd(acc);
                    if (!canModify)
                    {
                        Response.Write("err_cannot_modify"); // 不能修改密码 
                        return;
                    }

                    string oldpwd = AESHelper.AESDecrypt(cinfo["n2"].ToString(), AES_KEY);//password
                    if (oldpwd != data["pwd"].ToString())
                    {
                        Response.Write("err_pwd_error");//验证码错误
                        return;
                    }
                }

                string spwd = AESHelper.AESDecrypt(cinfo["n3"].ToString(), AES_KEY);//password
                if (spwd.Length != 32)//md5
                {
                    Response.Write("err_pwd_error");//密码错误
                    return;
                }

                Dictionary<string, object> updata = new Dictionary<string, object>();
                updata["pwd"] = spwd;
                updata["updatepwd"] = true;
                string ret = MongodbAccount.Instance.ExecuteUpdate("AccountTable", "acc", acc, updata);
                if (ret == "")
                {                   
                    Response.Write("err_success");
                }
                else
                    Response.Write("err_system_error");
            }
        }
    }
}