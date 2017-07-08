using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.account
{
    // api管理员上分下分
    public partial class AccountScoreAPIAdmin : RefreshPageBase
    {
        ViewAPIAdminScoreInfo m_view;

        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck("", Session, Response);

            GMUser user = (GMUser)Session["user"];
            if (user.m_accType != AccType.ACC_API)
            {
                Server.Transfer(DefCC.ASPX_EMPTY);
            }

            m_isAdmin.Text = user.m_accType.ToString();

            onQueryMember(user);
        }

        protected void onQueryMember(GMUser user)
        {
            ParamMemberInfo param = new ParamMemberInfo();
            // param.m_creator = user.m_user;
            param.m_searchDepth = 1;
            param.m_subAcc = 2;

            m_view = new ViewAPIAdminScoreInfo(IsRefreshed);
            OpRes res = user.doQuery(param, QueryType.queryTypeGmAccount);
            m_view.genTable(m_result, res, user);
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];
            m_curMoney.Text = user.m_money.ToString();
        }
    }
}