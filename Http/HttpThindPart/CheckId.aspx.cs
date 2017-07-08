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
    public partial class CheckId : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string playerId = Request.Params["id"];
            // string sign = Request.Params["sign"];

            int iPlayerid = 0;
            if (string.IsNullOrEmpty(playerId))
            {
                ReturnMsg(99999);
                return;
            }
            else
            {
                bool flag = int.TryParse(playerId, out iPlayerid);
                if (flag == false)
                {
                    ReturnMsg(99999);
                    return;
                }
            }

            // TODO: 验证签名是否正确

            string table = "player_info";
            Dictionary<string, object> data = MongodbPlayer.Instance.ExecuteGetBykey(table, "player_id", iPlayerid, new string[] { "platform" });
            if (data != null)
            {
                if (data["platform"].ToString() == "default")
                {
                    ReturnMsg(10000);
                }
                else
                {
                    ReturnMsg(10002);
                }
            }
            else
            {
                ReturnMsg(10001);
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
    }
}