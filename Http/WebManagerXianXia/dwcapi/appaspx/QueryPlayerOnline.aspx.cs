using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace dwcapi.appaspx
{
    // 查看玩家是否在游戏中
    public partial class QueryPlayerOnline : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ParamOnline param = new ParamOnline();
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

            DyOpOnline dy = new DyOpOnline();
            string retStr = dy.doDyop(param);

            Response.Write(retStr);
        }
    }
}