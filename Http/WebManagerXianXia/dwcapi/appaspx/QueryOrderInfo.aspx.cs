using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace dwcapi.appaspx
{
    public partial class QueryOrderInfo : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ParamQueryOrderResult param = new ParamQueryOrderResult();
            param.m_gmAccount = Request.QueryString["gmAcc"];
            param.m_gmPwd = Request.QueryString["gmPwd"];
            //param.m_playerAcc = Request.QueryString["playerAcc"];
            param.m_sign = Request.QueryString["sign"];
            param.m_orderId = Request.QueryString["orderId"];

            if (!param.isParamValid())
            {
                Dictionary<string, object> data = new Dictionary<string, object>();
                data.Add("result", RetCode.RET_PARAM_NOT_VALID);
                Response.Write(Helper.genJsonStr(data));
                return;
            }

            QueryOrderResult query = new QueryOrderResult();
            string retStr = query.doQuery(param);

            Response.Write(retStr);
        }
    }
}