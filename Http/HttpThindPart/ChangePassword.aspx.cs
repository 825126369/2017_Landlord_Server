using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Configuration;
using System.Collections.Specialized;
using System.Security.Cryptography;

using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Bson;

namespace HttpThindPart
{
    public partial class ChangePassword : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            NameValueCollection req = Request.Form;

            if (req.Count <= 0)
            {
                ReturnMsg(99999);
                return;
            }

            Dictionary<string, object> param = new Dictionary<string, object>();
            foreach (string key in req)
            {
                param[key] = req[key];
            }

            try
            {
                string nickname = param["nickname"].ToString();
                string oldpass = param["oldpassword"].ToString();
                string password = param["password"].ToString();
                string sign = param["sign"].ToString();

                if (sign != getSignForAnyValid(Request))
                {
                    ReturnMsg(10099);
                    return;
                }

                if (string.IsNullOrEmpty(nickname) || string.IsNullOrEmpty(oldpass) || string.IsNullOrEmpty(password))
                {
                    ReturnMsg(99999);
                    return;
                }

                // 验证新密码规则
                if (password.Length < 4 || password.Length > 12)
                {
                    ReturnMsg(10006);
                    return;
                }

                // 原密码与新密码相同时返回错误
                String md5pass = MD5Encode(password);
                if (oldpass.ToLower() == md5pass.ToLower())
                {
                    ReturnMsg(10008);
                    return;
                }

                string playerTable = "player_info";

                Dictionary<string, object> data = MongodbPlayer.Instance.ExecuteGetBykey(playerTable, "nickname", nickname, new string[] { "account", "platform" });
                if (data != null)
                {
                    if (data["platform"].ToString() != "default")
                    {
                        ReturnMsg(10002);
                        return;
                    }

                    string accountTable = "AccountTable";

                    Dictionary<string, object> dataPass = MongodbAccount.Instance.ExecuteGetBykey(accountTable, "acc_real", data["account"].ToString(), new string[] { "pwd" });
                    if (dataPass != null)
                    {
                        if (oldpass.ToLower() == dataPass["pwd"].ToString().ToLower())
                        {
                            DateTime now = DateTime.Now;
                            Dictionary<string, object> updata = new Dictionary<string, object>();
                            updata["pwd"] = md5pass.ToUpper();
                            updata["lasttime"] = now.Ticks;
                            string strerr = MongodbAccount.Instance.ExecuteUpdate(accountTable, "acc_real", data["account"].ToString(), updata);

                            ReturnMsg(10000);
                            return;
                        }
                        else
                        {
                            ReturnMsg(10007);
                            return;
                        }
                    }
                }
                ReturnMsg(10004);

            }
            catch (Exception ex)
            {
                ReturnMsg(99999);
                CLOG.Info(ex.ToString());
                return;
            }
        }

        void ReturnMsg(int bret = 10000)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data["result"] = bret;
            data["msg"] = ConfigurationManager.AppSettings["return_" + bret.ToString()];

            string jsonstr = JsonHelper.ConvertToStr(data);
            Response.Write(jsonstr);
        }

        public String getSignForAnyValid(HttpRequest request)
        {
            NameValueCollection requestParams = request.Form;//获得所有的参数名
            List<String> ps = new List<String>();
            foreach (string key in requestParams)
            {
                ps.Add(key);
            }

            sortParamNames(ps);// 将参数名从小到大排序，结果如：adfd,bcdr,bff,zx

            String paramValues = "";
            foreach (string param in ps)
            {//拼接参数值
                if (param == "sign")
                {
                    continue;
                }
                String paramValue = requestParams[param];
                if (paramValue != null)
                {
                    paramValues += paramValue;
                }
            }
            String md5Values = MD5Encode(ConfigurationManager.AppSettings["wechat_key"].ToString() + paramValues).ToLower();
            return md5Values;
        }


        //MD5编码
        public static String MD5Encode(String sourceStr)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] src = Encoding.UTF8.GetBytes(sourceStr);
            byte[] res = md5.ComputeHash(src, 0, src.Length);
            return BitConverter.ToString(res).ToLower().Replace("-", "");
        }

        //将参数名从小到大排序，结果如：adfd,bcdr,bff,zx
        public static void sortParamNames(List<String> paramNames)
        {
            paramNames.Sort((String str1, String str2) => { return str1.CompareTo(str2); });
        }
    }
}