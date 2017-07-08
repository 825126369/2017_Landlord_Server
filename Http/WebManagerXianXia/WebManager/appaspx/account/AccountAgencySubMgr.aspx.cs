using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.account
{
    // 子账号管理
    public partial class AccountAgencySubMgr : System.Web.UI.Page
    {
        ViewGmSubAgencyAccountInfo m_view = new ViewGmSubAgencyAccountInfo();

        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck("", Session, Response);

            GMUser user = (GMUser)Session["user"];

            ParamMemberInfo param = new ParamMemberInfo();
            param.m_searchDepth = 1;
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