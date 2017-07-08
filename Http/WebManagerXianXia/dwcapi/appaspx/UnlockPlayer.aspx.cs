using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace dwcapi.appaspx
{
    // 解锁玩家。当玩家进入游戏，退出时，没有把货币退回到 后台存储，服务器会锁定，会出现无法登录的情况
    public partial class UnlockPlayer : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ParamUnLockPlayer param = new ParamUnLockPlayer();
            param.m_gmAccount = Request.QueryString["gmAcc"];
            param.m_gmPwd = Request.QueryString["gmPwd"];
            param.m_playerAcc = Request.QueryString["playerAcc"];
            param.m_sign = Request.QueryString["sign"];

            if (!param.isParamValid())
            {
                Dictionary<string, object> data = new Dictionary<string, object>();
                data.Add("result", RetCode.RET_PARAM_NOT_VALID);
                Response.Write(Helper.genJsonStr(data));
                return;
            }

            DyOpUnLockPlayer dy = new DyOpUnLockPlayer();
            string retStr = dy.doDyop(param);

            Response.Write(retStr);
        }
    }
}