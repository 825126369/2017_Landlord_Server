using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace dwcapi.appaspx
{
    // 查询玩家在游戏内的金币变化情况
    public partial class QueryPlayerMoneyChange : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ParamQueryPlayerMoneyChange param = new ParamQueryPlayerMoneyChange();
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

            QueryPlayerMoneyChangeInGame query = new QueryPlayerMoneyChangeInGame();
            string retStr = query.doQuery(param);

            Response.Write(retStr);
        }
    }
}