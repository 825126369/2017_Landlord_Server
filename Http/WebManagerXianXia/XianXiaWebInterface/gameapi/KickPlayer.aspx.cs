using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace XianXiaWebInterface.gameapi
{
    public partial class KickPlayer : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string acc = Request.Params["acc"];
            string time = Request.Params["time"];
            if (string.IsNullOrEmpty(acc) || string.IsNullOrEmpty(time))
            {
                returnMsg(RetCode.RET_PARAM_NOT_VALID);
                return;
            }

            var ret = MongodbPlayer.Instance.ExecuteGetBykey("player_info", "account", acc, new string[]{"SyncLock"});
            if (ret == null)
            {
                returnMsg(RetCode.RET_NO_PLAYER);
                return;
            }

            int state = Convert.ToInt32(ret["SyncLock"]);
            if (state != 1)
            {
                returnMsg(RetCode.RET_PLAYER_OFFLINE);
                return;
            }

            int nt =  Convert.ToInt32(time);
            if(nt < 300) nt = 300;

            Dictionary<string, object> data = new Dictionary<string,object>();
            data["time"] = nt;

            string err = MongodbPlayer.Instance.ExecuteUpdate("KickPlayer", "key", acc, data);
            if (string.IsNullOrEmpty(err))
                returnMsg(RetCode.RET_SUCCESS);
            else
                returnMsg(RetCode.RET_DB_ERROR, err);
        }

        private void returnMsg(int retCode, string info = "")
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data["result"] = retCode;

            if (!string.IsNullOrEmpty(info))
                data["info"] = info;

            Response.Write(JsonHelper.ConvertToStr(data));
        }
    }
}