using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace XianXiaWebInterface.gameapi
{
    public partial class UnlockPlayer : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string acc = Request.Params["acc"];
            if (string.IsNullOrEmpty(acc))
            {
                returnMsg(RetCode.RET_PARAM_NOT_VALID);
                return;
            }

            var ret = MongodbPlayer.Instance.ExecuteGetBykey("player_info", "account", acc, new string[] { "SyncLock" , "gold"});
            if (ret == null)
            {
                returnMsg(RetCode.RET_NO_PLAYER);
                return;
            }

            int state = Convert.ToInt32(ret["SyncLock"].ToString());
            if (state == 1)
            {
                returnMsg(RetCode.RET_PLAYER_ONLINE);
                return;
            }
            else if (state == 0)
            {
                returnMsg(RetCode.RET_PLYAER_NOT_LOCKED);
                return;
            }

            Dictionary<string, object> data =new Dictionary<string,object>();
            data["SyncLock"] = (sbyte)0;
            data["gold"] = 0;
            string err = MongodbPlayer.Instance.ExecuteUpdate("player_info", "account", acc, data);
            if(string.IsNullOrEmpty(err))
                returnMsg(RetCode.RET_SUCCESS, ret["gold"].ToString());
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