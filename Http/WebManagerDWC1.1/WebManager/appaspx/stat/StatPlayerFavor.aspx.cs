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
        private static string[] s_head = new string[] { "日期", "经典捕鱼", "鳄鱼大亨", "欢乐骰宝", "万人年牛", "百家乐", "斗地主", "麻将" };
        private string[] m_content = new string[s_head.Length];

        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck("stat", Session, Response);

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
            for (i = 0; i < s_head.Length; i++)
            {
                td = new TableCell();
                tr.Cells.Add(td);
                td.Text = s_head[i];
            }

            for (i = 0; i < qresult.Count; i++)
            {
                tr = new TableRow();
                if ((i & 1) == 0)
                {
                    tr.CssClass = "alt";
                }
                m_result.Rows.Add(tr);

                m_content[0] = qresult[i].m_time;
                m_content[1] = qresult[i].m_game1.ToString();
                m_content[2] = qresult[i].m_game2.ToString();
                m_content[3] = qresult[i].m_game3.ToString();
                m_content[4] = qresult[i].m_game4.ToString();
                m_content[5] = qresult[i].m_game5.ToString();
                m_content[6] = qresult[i].m_game6.ToString();
                m_content[7] = qresult[i].m_game7.ToString();
                //m_content[8] = qresult[i].m_game8.ToString();

                for (j = 0; j < s_head.Length; j++)
                {
                    td = new TableCell();
                    tr.Cells.Add(td);
                    td.Text = m_content[j];
                }
            }
        }
    }
}