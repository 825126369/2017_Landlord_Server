using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.account.sub
{
    // 链接到某代理的子账号
    public partial class AccountAgencySub : System.Web.UI.Page
    {
        ViewGmSubAgencyAccountInfo m_view = new ViewGmSubAgencyAccountInfo();

        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck("", Session, Response);

            string acc = Request.QueryString["acc"];
            string depthStr = Request.QueryString["depth"];
            if (string.IsNullOrEmpty(acc) ||
                string.IsNullOrEmpty(depthStr))
            {
                return;
            }
            int depth;
            if (!int.TryParse(depthStr, out depth))
                return;

            GMUser user = (GMUser)Session["user"];
            ParamMemberInfo param = new ParamMemberInfo();
            param.m_creator = acc;
            param.m_searchDepth = 1;
            param.m_creatorDepth = depth;
            param.m_subAcc = 2;

            OpRes res = user.doQuery(param, QueryType.queryTypeGmAccount);
            m_view.genTable(m_result, res, user, new EventHandler(onSetingAcc));
        }

        protected void onSetingAcc(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];
            ItemHelp.stopStartGmAcc(sender, user, m_result);
        }
    }
}