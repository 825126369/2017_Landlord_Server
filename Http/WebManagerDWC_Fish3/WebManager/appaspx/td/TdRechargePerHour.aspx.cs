using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.td
{
    public partial class TdRechargePerHour : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck(RightDef.TD_RECHARGE_PER_HOUR, Session, Response);
        }

        protected void onQuery(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];

            ParamQuery param = new ParamQuery();
            param.m_time = m_time.Text;
            OpRes res = user.doQuery(param, QueryType.queryTypeRechargePerHour);

            TableTdRechargePerHour view = new TableTdRechargePerHour();
            view.genTable(user, m_result, res);
        }
    }
}