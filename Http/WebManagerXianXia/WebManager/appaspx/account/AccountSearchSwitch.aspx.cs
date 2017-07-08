using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.account
{
    public partial class AccountSearchSwitch : System.Web.UI.Page
    {
        ViewGmAccountInfo m_view = new ViewGmAccountInfo();

        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck(RIGHT.VIEW_AGENCY, Session, Response);
            GMUser user = (GMUser)Session["user"];

            if (!IsPostBack)
            {
                m_searchCond.setLevelName(false);
                m_searchCond.getViewLevel().Visible = false;
            }
           // else
            {
                onQueryMember1(null, null);
            }
            if (user.isAPIAcc())
            {
                Server.Transfer(DefCC.ASPX_EMPTY);
            }
        }

        protected void onQueryMember(object sender, EventArgs e)
        {
        }

        protected void onQueryMember1(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];
            ParamMemberInfo param = new ParamMemberInfo();
            m_searchCond.fillCondtion(param, user);
            param.m_searchDepth = 1;

            URLParam uparam = new URLParam();
            uparam.m_url = @"/appaspx/account/AccountSearchSwitch.aspx";
            user.getOpLevelMgr().addRootAcc(param.getRootUser(user), uparam);
            m_levelStr.InnerHtml = user.getOpLevelMgr().getCurLevelStr(param.getRootUser(user));
            OpRes res = user.doQuery(param, QueryType.queryTypeGmAccount);
            m_view.genTable(m_result, res, user, new EventHandler(onSetingAcc), param);
        }

        protected void onSetingAcc(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];
            ItemHelp.stopStartGmAcc(sender, user, m_result);
        }
    }
}