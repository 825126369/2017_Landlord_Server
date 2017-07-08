using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.account
{
    public partial class GmInfoViewTree : System.Web.UI.Page
    {
        ViewGmAccountInfo m_view = new ViewGmAccountInfo();

        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck("", Session, Response);

            string acc = Request.QueryString["acc"];
            if (string.IsNullOrEmpty(acc))
            {
                Server.Transfer(DefCC.ASPX_EMPTY);
            }

            GMUser user = (GMUser)Session["user"];
            ParamMemberInfo param = new ParamMemberInfo();
            param.m_creator = acc;
            param.m_searchDepth = 1;

            OpRes res = user.doQuery(param, QueryType.queryTypeGmAccount);
            m_view.genTable(m_result, res, user, new EventHandler(onSetingAcc), param);

            m_creator.InnerHtml = user.getOpLevelMgr().getCurLevelStr(acc);
        }

        protected void onSetingAcc(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];
            ItemHelp.stopStartGmAcc(sender, user, m_result);
        }
    }
}