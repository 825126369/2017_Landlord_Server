using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Configuration;

using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Bson;

namespace HttpThindPart
{
    public partial class AccountVerify : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string playerId = Request.Params["id"];
            string nickname = Request.Params["nickname"];
            string password = Request.Params["password"];
            // string sign = Request.Params["sign"];

            if (string.IsNullOrEmpty(nickname) || string.IsNullOrEmpty(playerId) || string.IsNullOrEmpty(password))
            {
                ReturnMsg(99999);
                return;
            }

            int iPlayerid;
            bool flag = int.TryParse(playerId, out iPlayerid);
            if (flag == false)
            {
                ReturnMsg(99999);
                return;
            }

            // TODO: 验证签名是否正确
            
            string playerTable = "player_info";
            
            Dictionary<string, object> data = MongodbPlayer.Instance.ExecuteGetBykey(playerTable, "player_id", iPlayerid, new string[] { "nickname", "account", "platform" });
            if (data != null)
            {
                if (data["platform"].ToString() != "default")
                {
                    ReturnMsg(10002);
                    return;
                }

                if (nickname == data["nickname"].ToString())
                {
                    string accountTable = "AccountTable";

                    Dictionary<string, object> dataPass = MongodbAccount.Instance.ExecuteGetBykey(accountTable, "acc_real", data["account"].ToString(), new string[] { "pwd" });
                    if (dataPass != null)
                    {
                        if (password.ToLower() == dataPass["pwd"].ToString().ToLower())
                        {
                            ReturnMsg(10000);
                            return;
                        }
                    }
                    ReturnMsg(10007);
                    return;
                }
                else
                {
                    ReturnMsg(10004);
                    return;
                }
            }
            ReturnMsg(10004);
        }

        void ReturnMsg(int bret = 10000)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data["result"] = bret;
            data["msg"] = ConfigurationManager.AppSettings["return_" + bret.ToString()];

            string jsonstr = JsonHelper.ConvertToStr(data);
            Response.Write(jsonstr);
        }
    }
}