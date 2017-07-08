using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace dwcapi.appaspx
{
    // 查输赢统计
    public partial class QueryPlayerWinLose : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ParamQueryPlayerWinLose param = new ParamQueryPlayerWinLose();
            param.m_gmAccount = Request.QueryString["gmAcc"];
            param.m_gmPwd = Request.QueryString["gmPwd"];
            param.m_playerAcc = Request.QueryString["playerAcc"];
            param.m_curPageStr = Request.QueryString["curPage"];
            param.m_countEachPageStr = Request.QueryString["countEachPage"];
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

            QueryPlayerWinLoseReport query = new QueryPlayerWinLoseReport();
            string retStr = query.doQuery(param);

            Response.Write(retStr);
        }
    }
}