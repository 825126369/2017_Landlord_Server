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
    public partial class CheckIdNickname : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            string playerId = Request.Params["id"];
            string nickname = Request.Params["nickname"];

            if (string.IsNullOrEmpty(nickname) || string.IsNullOrEmpty(playerId))
            {
                ReturnMsg(99999);
                return;
            }
            int iPlayerid = 0;
            bool flag = int.TryParse(playerId, out iPlayerid);
            if (flag == false)
            {
                ReturnMsg(99999);
                return;
            }

            string table = "player_info";
            Dictionary<string, object> data = MongodbPlayer.Instance.ExecuteGetBykey(table, "player_id", iPlayerid, new string[] { "nickname" });
            if (data != null)
            {
                if (data["nickname"].ToString() != nickname)
                {
                    ReturnMsg(10009);
                    return;
                }
                
                ReturnMsg(10000);
            }
            else
            {
                ReturnMsg(10004);
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