using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.stat
{
    public partial class StatPlayerFavor : System.Web.UI.Page
    {
        private string[] m_content = new string[StrName.s_gameName.Length];

        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck(RightDef.DATA_PLAYER_FAVOR, Session, Response);

            if (!IsPostBack)
            {
                m_statWay.Items.Add("活跃次数");
                m_statWay.Items.Add("活跃人数");
            }
        }

        protected void onStat(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];
            StatMgr mgr = user.getSys<StatMgr>(SysType.sysTypeStat);
            if (m_statWay.SelectedIndex == 0)
            {
                OpRes res = mgr.doStat(m_time.Text, StatType.statTypeActiveCount, user);
                genTable(m_result, res, user, mgr, StatType.statTypeActiveCount);
            }
            else
            {
                OpRes res = mgr.doStat(m_time.Text, StatType.statTypeActivePerson, user);
                genTable(m_result, res, user, mgr, StatType.statTypeActivePerson);
            }
        }

        private void genTable(Table table, OpRes res, GMUser user, StatMgr mgr, StatType sType)
        {
            m_result.GridLines = GridLines.Both;
            TableRow tr = new TableRow();
            m_result.Rows.Add(tr);
            TableCell td = null;
            if (res != OpRes.opres_success)
            {
                td = new TableCell();
                tr.Cells.Add(td);
                td.Text = OpResMgr.getInstance().getResultString(res);
                return;
            }

            List<ResultActive> qresult = (List<ResultActive>)mgr.getStatResult(sType);
            int i = 0, j = 0;
            // 表头
            for (i = 0; i < StrName.s_gameName.Length; i++)
            {
                td = new TableCell();
                tr.Cells.Add(td);
                if (i == 0)
                {
                    td.Text = "日期";
                }
                else
                {
                    td.Text = StrName.s_gameName[i];
                }
            }

            int f = 0;
            for (i = 0; i < qresult.Count; i++)
            {
                f = 0;
                tr = new TableRow();
                if ((i & 1) == 0)
                {
                    tr.CssClass = "alt";
                }
                m_result.Rows.Add(tr);

                m_content[f++] = qresult[i].m_time;

                for (int k = 1; k < StrName.s_gameName.Length; k++)
                {
                    m_content[f++] = qresult[i].getCount(k).ToString();
                }

                for (j = 0; j < StrName.s_gameName.Length; j++)
                {
                    td = new TableCell();
                    tr.Cells.Add(td);
                    td.Text = m_content[j];
                }
            }
        }
    }
}