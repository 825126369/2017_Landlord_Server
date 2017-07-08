using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.stat
{
    public partial class StatLobby : System.Web.UI.Page
    {
        private static string[] s_head = new string[] { "类型", "明细" };
        private string[] m_content = new string[s_head.Length];

        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck("stat", Session, Response);
            if (!IsPostBack)
            {
                m_statWay.Items.Add("vip等级分布");
                m_statWay.Items.Add("其他");
            }
        }

        protected void onStat(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];
            if (m_statWay.SelectedIndex == 0)
            {
                StatMgr mgr = user.getSys<StatMgr>(SysType.sysTypeStat);
                mgr.doStat(null, StatType.statTypeVipLevel, user);
                genVipLevelTable(m_result, user, mgr);
            }
            else
            {
                QueryMgr mgr = user.getSys<QueryMgr>(SysType.sysTypeQuery);
                mgr.doQuery(m_statWay.SelectedIndex, QueryType.queryTypeLobby, user);
                genGeneralTable(m_result, user, mgr);
            }
        }

        private void genGeneralTable(Table table, GMUser user, QueryMgr mgr)
        {
            table.GridLines = GridLines.Both;
            TableRow tr = new TableRow();
            table.Rows.Add(tr);
            TableCell td = null;

            ResultLobby qresult = (ResultLobby)mgr.getQueryResult(QueryType.queryTypeLobby);

            if (qresult.m_statType == -1)
            {
                td = new TableCell();
                tr.Cells.Add(td);
                td.Text = OpResMgr.getInstance().getResultString(OpRes.op_res_failed);
                return;
            }

            int i = 0, j = 0;
            // 表头
            for (i = 0; i < s_head.Length; i++)
            {
                td = new TableCell();
                tr.Cells.Add(td);
                td.Text = s_head[i];
            }

            for (i = 1; i < (int)DataStatType.stat_max; i++)
            {
                tr = new TableRow();
                if ((i & 1) != 0)
                {
                    tr.CssClass = "alt";
                }
                m_result.Rows.Add(tr);

                m_content[0] = StrName.s_statLobbyName[i];
                m_content[1] = qresult.getValue(i);

                for (j = 0; j < s_head.Length; j++)
                {
                    td = new TableCell();
                    tr.Cells.Add(td);
                    td.Text = m_content[j];
                }
            }
        }

        private void genVipLevelTable(Table table, GMUser user, StatMgr mgr)
        {
            table.GridLines = GridLines.Both;
            TableRow tr = new TableRow();
            table.Rows.Add(tr);
            TableCell td = null;

            StatResultVipLevel qresult = (StatResultVipLevel)mgr.getStatResult(StatType.statTypeVipLevel);

            int i = 0, j = 0;
            // 表头
            for (i = 0; i < s_head.Length; i++)
            {
                td = new TableCell();
                tr.Cells.Add(td);
                td.Text = s_head[i];
            }

            i = 0;
            foreach(var d in qresult.m_vipLevel)
            {
                tr = new TableRow();
                if ((i & 1) == 0)
                {
                    tr.CssClass = "alt";
                }
                m_result.Rows.Add(tr);

                m_content[0] = "VIP" + d.Key.ToString();
                m_content[1] = d.Value.ToString();

                for (j = 0; j < s_head.Length; j++)
                {
                    td = new TableCell();
                    tr.Cells.Add(td);
                    td.Text = m_content[j];
                }
                i++;
            }
        }
    }
}