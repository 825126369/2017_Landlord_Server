using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.td
{
    public partial class TdPlayerIncomeExpenses : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck(RightDef.TD_INCOME_EXPENSES, Session, Response);
            if (!IsPostBack)
            {
                m_property.Items.Add(new ListItem("所有", "-1"));
                m_property.Items.Add(new ListItem("金币", "0"));
                m_property.Items.Add(new ListItem("钻石", "1"));
                m_property.Items.Add(new ListItem("龙珠", "2"));
                m_property.Items.Add(new ListItem("话费券", "3"));

                m_player.Items.Add(new ListItem("所有", "0"));
                m_player.Items.Add(new ListItem("打到过龙珠", "1"));
                m_player.Items.Add(new ListItem("没有打到龙珠", "2"));
            }
        }

        protected void onQuery(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];

            ParamIncomeExpenses param = new ParamIncomeExpenses();
            param.m_time = m_time.Text;
            param.m_playerGainDb = Convert.ToInt32(m_player.SelectedValue);
            param.m_property = Convert.ToInt32(m_property.SelectedValue);

            TablePlayerIncomeExpenses view = new TablePlayerIncomeExpenses();
            OpRes res = user.doStat(param, StatType.statTypePlayerIncomeExpenses);

            view.genTable(user, m_result, res, param);
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