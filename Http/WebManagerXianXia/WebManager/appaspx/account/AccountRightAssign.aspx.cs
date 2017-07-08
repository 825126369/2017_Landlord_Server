using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.account
{
    public partial class AccountRightAssign : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck("modify", Session, Response);
           // genTable();
        }

        void genTable()
        {
            GMUser user = (GMUser)Session["user"];

            ParamMemberInfo param = new ParamMemberInfo();
            param.m_searchDepth = 1;
            param.m_subAcc = 2;
            OpRes res = user.doQuery(param, QueryType.queryTypeGmAccount);
            ViewRightAssign view = new ViewRightAssign();
            view.genTable(m_result, res, user);
        }
    }
}