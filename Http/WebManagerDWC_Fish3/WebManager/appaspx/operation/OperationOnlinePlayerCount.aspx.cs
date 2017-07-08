using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.operation
{
    public partial class OperationOnlinePlayerCount : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck(RightDef.OP_CUR_ONLINE_NUM, Session, Response);

            GMUser user = (GMUser)Session["user"];
            QueryMgr mgr = user.getSys<QueryMgr>(SysType.sysTypeQuery);
            OpRes res = mgr.doQuery(null, QueryType.queryTypeOnlinePlayerCount, user);
            int c = (int)mgr.getQueryResult(QueryType.queryTypeOnlinePlayerCount);
            m_count.Text = c.ToString();
        }
    }
}