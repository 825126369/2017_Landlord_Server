using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace dwcapi.appaspx
{
    // 查询API输赢总计
    public partial class QueryWinLoseSum : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ParamQueryWinLoseSum param = new ParamQueryWinLoseSum();
            param.m_gmAccount = Request.QueryString["gmAcc"];
            param.m_gmPwd = Request.QueryString["gmPwd"];
            param.m_startTime = Request.QueryString["startTime"];
            param.m_endTime = Request.QueryString["endTime"];
            param.m_sign = Request.QueryString["sign"];

            if (!param.isParamValid())
            {
                Dictionary<string, object> data = new Dictionary<string, object>();
                data.Add("result", RetCode.RET_PARAM_NOT_VALID);
                Response.Write(Helper.genJsonStr(data));
                return;
            }

            QueryWinLoseSumAPI query = new QueryWinLoseSumAPI();
            string retStr = query.doQuery(param);

            Response.Write(retStr);
        }
    }
}