using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace XianXiaWebInterface.appaspx
{
    // 玩家游戏内提交订单
    public partial class PlayerCommitOrder : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ParamCommitOrder param = new ParamCommitOrder();
            param.m_playerAcc = Request.QueryString["playerAcc"];
            param.m_moneyStr = Request.QueryString["money"];
            param.m_orderTypeStr = Request.QueryString["orderType"];
            param.m_sign = Request.QueryString["sign"];

            DyOpPlayerCommitOrder dy = new DyOpPlayerCommitOrder();
            string retStr = dy.doDyop(param);
            Response.Write(retStr);
        }
    }
}