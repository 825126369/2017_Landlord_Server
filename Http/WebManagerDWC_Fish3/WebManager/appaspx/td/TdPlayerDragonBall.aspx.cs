using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.td
{
    public partial class TdPlayerDragonBall : System.Web.UI.Page
    {
        private PageGenDailyTask m_gen = new PageGenDailyTask(50);

        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck(RightDef.TD_PLAYER_DB_MONITOR, Session, Response);

            if (!IsPostBack)
            {
                if (m_gen.parse(Request))
                {
                    m_time.Text = m_gen.m_time;
                    onQuery(null, null);
                }
            }
            m_res.InnerHtml = "";
        }

        protected void onQuery(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];

            ParamQuery param = new ParamQuery();
            param.m_time = m_time.Text;
            param.m_curPage = m_gen.curPage;
            param.m_countEachPage = m_gen.rowEachPage;

            TablePlayerDragonBall view = new TablePlayerDragonBall();
            OpRes res = user.doStat(param, StatType.statTypePlayerDragonBall);

            m_page.InnerHtml = "";
            m_foot.InnerHtml = "";
            view.genTable(user, m_result, res);

            string page_html = "", foot_html = "";
            m_gen.genPage(param, @"/appaspx/td/TdPlayerDragonBall.aspx", ref page_html, ref foot_html, user);
            m_page.InnerHtml = page_html;
            m_foot.InnerHtml = foot_html;
        }

        protected void onExport(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];
            ParamQuery param = new ParamQuery();
            param.m_time = m_time.Text;
            ExportMgr mgr = user.getSys<ExportMgr>(SysType.sysTypeExport);
            OpRes res = mgr.doExport(param, ExportType.exportTypeStatPlayerDragonBall, user);
            m_res.InnerHtml = OpResMgr.getInstance().getResultString(res);
        }
    }
}