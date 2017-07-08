using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.operation
{
    public partial class OperationWishCurse : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck("operation", Session, Response);
            if (!IsPostBack)
            {
                m_opType.Items.Add("添加");
                m_opType.Items.Add("去除");

                m_type.Items.Add("祝福");
                m_type.Items.Add("诅咒");
                m_type.SelectedIndex = 0;

                m_game.Items.Add(new ListItem("经典捕鱼", ((int)(GameId.fishlord)).ToString()));
                m_game.Items.Add(new ListItem("鳄鱼公园", ((int)(GameId.fishpark)).ToString()));
            }
        }

        protected void onAddWishCurse(object sender, EventArgs e)
        {
            ParamAddWishCurse param = new ParamAddWishCurse();
            param.m_gameId = Convert.ToInt32(m_game.SelectedValue);
            param.m_opType = m_opType.SelectedIndex;
            param.m_wishType = m_type.SelectedIndex;
            param.m_playerId = m_playerId.Text;
            param.m_rate = m_rate.Text;
            GMUser user = (GMUser)Session["user"];
            OpRes res = user.doDyop(param, DyOpType.opTypeWishCurse);
            m_res.InnerHtml = OpResMgr.getInstance().getResultString(res);
        }
    }
}