using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace dwcapi.appaspx
{
    // 清理登录失败次数。若尝试登录超过3次，账号会被冻结1天，此接口可以清理这个值。
    public partial class ClearLoginFailed : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ParamBase param = new ParamBase();
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

            DyOpClearLoginFailedCount dy = new DyOpClearLoginFailedCount();
            string retStr = dy.doDyop(param);

            Response.Write(retStr);
        }
    }
}