using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.operation
{
    public partial class OperationMaxOnline : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck(RightDef.OP_MAX_ONLINE, Session, Response);
        }

        protected void onQuery(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];
            TableMaxOnline view = new TableMaxOnline();
            ParamQuery param = new ParamQuery();
            param.m_time = m_time.Text;
            OpRes res = user.doQuery(param, QueryType.queryTypeMaxOnline);
            view.genTable(user, m_result, res);
        }
    }
}
