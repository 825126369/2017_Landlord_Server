using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace dwcapi.appaspx
{
    // 生成一个新的玩家
    public partial class CreatePlayer : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ParamCreatePlayer param = new ParamCreatePlayer();
            param.m_gmAccount = Request.QueryString["gmAcc"];
            param.m_gmPwd = Request.QueryString["gmPwd"];
            param.m_playerAcc = Request.QueryString["playerAcc"];
            param.m_pwd = Request.QueryString["playerPwd"];
            param.m_washRatioStr = Request.QueryString["washRatio"];
            param.m_aliasName = Request.QueryString["aliasName"];
            param.m_sign = Request.QueryString["sign"];

            if (!param.isParamValid())
            {
                Dictionary<string, object> data = new Dictionary<string, object>();
                data.Add("result", RetCode.RET_PARAM_NOT_VALID);
                Response.Write(Helper.genJsonStr(data));
                return;
            }

            DyOpCreatePlayer dy = new DyOpCreatePlayer();
            string retStr = dy.doDyop(param);

            Response.Write(retStr);
        }
    }
}