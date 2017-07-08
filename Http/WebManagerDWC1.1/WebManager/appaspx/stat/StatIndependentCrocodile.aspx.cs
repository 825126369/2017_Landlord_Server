using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.stat
{
    public partial class StatIndependentCrocodile : System.Web.UI.Page
    {
        private static string[] s_head1 = new string[] { "进入房间次数", "房间名称", "次数" };
        private static string[] s_head2 = new string[] { "押注信息", "id", "名称", "下注次数", "获奖次数", "总收入", "总支出", "盈利率" };
        private string[] m_content = new string[s_head2.Length];

        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck("stat", Session, Response);
        }

        protected void onStat(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];
            QueryMgr mgr = user.getSys<QueryMgr>(SysType.sysTypeQuery);
            OpRes res = mgr.doQuery(null, QueryType.queryTypeIndependentCrocodile, user);
            genTable(m_result, res, user, mgr);
        }

        private void genTable(Table table, OpRes res, GMUser user, QueryMgr mgr)
        {
            table.GridLines = GridLines.Both;
            TableRow tr = new TableRow();
            table.Rows.Add(tr);
            TableCell td = null;

            if (res != OpRes.opres_success)
            {
                td = new TableCell();
                tr.Cells.Add(td);
                td.Text = OpResMgr.getInstance().getResultString(res);
                return;
            }

            ResultCrocodile qresult = (ResultCrocodile)mgr.getQueryResult(QueryType.queryTypeIndependentCrocodile);

            m_content[0] = "";

            // 进入房间次数
            int i = 0, j = 0;
            for (i = 0; i < s_head1.Length; i++)
            {
                td = new TableCell();
                tr.Cells.Add(td);
                td.Text = s_head1[i];
            }

            for (i = 1; i < qresult.getRoomCount(); i++)
            {
                tr = new TableRow();
                if ((i & 1) != 0)
                {
                    tr.CssClass = "alt";
                }
                m_result.Rows.Add(tr);

                m_content[1] = qresult.getRoomName(i);
                m_content[2] = qresult.getEnterRoomCount(i);

                for (j = 0; j < s_head1.Length; j++)
                {
                    td = new TableCell();
                    tr.Cells.Add(td);
                    td.Text = m_content[j];
                }
            }

            // 押注情况
            tr = new TableRow();
            table.Rows.Add(tr);
            for (i = 0; i < s_head2.Length; i++)
            {
                td = new TableCell();
                tr.Cells.Add(td);
                td.Text = s_head2[i];
            }

            i = 0;
            Dictionary<int, CrocodileInfo> dict = qresult.m_betInfo.OrderBy(o => o.Key).ToDictionary(o => o.Key, p => p.Value);
            foreach (var info in dict)
            {
                tr = new TableRow();
                if ((i & 1) == 0)
                {
                    tr.CssClass = "alt";
                }
                m_result.Rows.Add(tr);

                m_content[1] = (info.Key + 1).ToString();
                m_content[2] = qresult.getAreaName(info.Key + 1);
                m_content[3] = info.Value.m_betCount.ToString();
                m_content[4] = info.Value.m_winCount.ToString();
                m_content[5] = info.Value.m_income.ToString();
                m_content[6] = info.Value.m_outlay.ToString();
                m_content[7] = info.Value.getFactExpRate();

                for (j = 0; j < s_head2.Length; j++)
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