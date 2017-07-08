using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.td
{
    public partial class TdDragonBallDaily : System.Web.UI.Page
    {
        private PageGenDailyTask m_gen = new PageGenDailyTask(50);

        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck(RightDef.TD_DAILY_DB, Session, Response);
        }

        protected void onQuery(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];
            m_res.InnerHtml = "";

            ParamDragonBallDaily param = new ParamDragonBallDaily();
            param.m_time = m_time.Text;
            param.m_discount = m_discount.Text;
            param.m_eachValue = m_eachValue.Text;

            OpRes res = user.doQuery(param, QueryType.queryTypeDragonBallDaily);

            TableDragonBallDaily view = new TableDragonBallDaily();
            view.genTable(user, m_result, res);
        }

        protected void onExport(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];
            ParamDragonBallDaily param = new ParamDragonBallDaily();
            param.m_time = m_time.Text;
            param.m_discount = m_discount.Text;
            param.m_eachValue = m_eachValue.Text;

            ExportMgr mgr = user.getSys<ExportMgr>(SysType.sysTypeExport);
            OpRes res = mgr.doExport(param, ExportType.exportTypeDragonBallDaily, user);
            m_res.InnerHtml = OpResMgr.getInstance().getResultString(res);
        }
    }
}