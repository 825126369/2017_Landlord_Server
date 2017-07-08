using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace dwcapi.appaspx
{
    public partial class PlayerOp : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ParamPlayerOp param = new ParamPlayerOp();
            param.m_gmAccount = Request.QueryString["gmAcc"];
            param.m_gmPwd = Request.QueryString["gmPwd"];
            param.m_opStr = Request.QueryString["op"];
            param.m_playerAcc = Request.QueryString["playerAcc"];
            param.m_sign = Request.QueryString["sign"];

            if (!param.isParamValid())
            {
                Dictionary<string, object> data = new Dictionary<string, object>();
                data.Add("result", RetCode.RET_PARAM_NOT_VALID);
                Response.Write(Helper.genJsonStr(data));
                return;
            }

            DyOpPlayerOp dy = new DyOpPlayerOp();
            string retStr = dy.doDyop(param);

            Response.Write(retStr);
        }
    }
}