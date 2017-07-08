using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace dwcapi.appaspx
{
    // 修改玩家密码
    public partial class UpdatePlayerPwd : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ParamModifyPlayerPwd param = new ParamModifyPlayerPwd();
            param.m_gmAccount = Request.QueryString["gmAcc"];
            param.m_gmPwd = Request.QueryString["gmPwd"];
            param.m_playerAcc = Request.QueryString["playerAcc"];
            param.m_oldPwd = Request.QueryString["oldPwd"];
            param.m_newPwd = Request.QueryString["newPwd"];
            param.m_sign = Request.QueryString["sign"];

            if (!param.isParamValid())
            {
                Dictionary<string, object> data = new Dictionary<string, object>();
                data.Add("result", RetCode.RET_PARAM_NOT_VALID);
                Response.Write(Helper.genJsonStr(data));
                return;
            }

            DyOpModifyPlayerPwd dy = new DyOpModifyPlayerPwd();
            string retStr = dy.doDyop(param);

            Response.Write(retStr);
        }
    }
}