using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.operation
{
    public partial class OperationPlayerTotalMoney : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck("operation", Session, Response);
        }

        protected void onQuery(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];
            TablePlayerTotalMoney view = new TablePlayerTotalMoney();
            ParamQuery param = new ParamQuery();
            param.m_time = m_time.Text;
            OpRes res = user.doQuery(param, QueryType.queryTypeTotalPlayerMoney);
            view.genTable(user, m_result, res);
        }
    }
}