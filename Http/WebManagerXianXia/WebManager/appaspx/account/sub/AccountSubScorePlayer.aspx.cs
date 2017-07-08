using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.account.sub
{
    // 给会员上分下分的链接页面
    public partial class AccountSubScorePlayer : RefreshPageBase
    {
        ViewPlayerScoreInfo m_view;

        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck("", Session, Response);

            string acc = Request.QueryString["acc"];
            if (string.IsNullOrEmpty(acc))
            {
                Server.Transfer(DefCC.ASPX_EMPTY);
            }
            
            GMUser user = (GMUser)Session["user"];
            m_isAdmin.Text = user.m_accType.ToString();

            ParamMemberInfo param = new ParamMemberInfo();
            param.m_creator = acc;
            param.m_searchDepth = 1;

            m_view = new ViewPlayerScoreInfo(IsRefreshed);
            OpRes res = user.doQuery(param, QueryType.queryTypePlayerMember);
            m_view.genTable(m_result, res, user);

            m_levelStr.InnerHtml = user.getOpLevelMgr().getCurLevelStr(acc);
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];
            m_curMoney.Text = user.m_money.ToString();
        }
    }
}
