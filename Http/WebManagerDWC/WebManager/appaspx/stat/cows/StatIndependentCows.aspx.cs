using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.stat.cows
{
    public partial class StatIndependentCows : System.Web.UI.Page
    {
        private static string[] s_head = new string[] { "区域id", "名称", "总赢次数", "总收入", "总支出", "盈利率" };
        private string[] m_content = new string[s_head.Length];

        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck("stat", Session, Response);
        }

        protected void onStat(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];
            OpRes res = user.doQuery(null, QueryType.queryTypeIndependentCows);
            genTable(m_result, res, user);
        }

        private void genTable(Table table, OpRes res, GMUser user)
        {
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

            ResultCows qresult = (ResultCows)user.getQueryResult(QueryType.queryTypeIndependentCows);

            int i = 0, k = 0;

            for (i = 0; i < s_head.Length; i++)
            {
                td = new TableCell();
                tr.Cells.Add(td);
                td.Text = s_head[i];
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

                m_content[0] = info.Key.ToString();
                m_content[1] = qresult.getAreaName(info.Key);
                m_content[2] = info.Value.m_winCount.ToString();
                m_content[3] = info.Value.m_income.ToString();
                m_content[4] = info.Value.m_outlay.ToString();
                m_content[5] = info.Value.getFactExpRate();

                for (k = 0; k < s_head.Length; k++)
                {
                    td = new TableCell();
                    tr.Cells.Add(td);
                    td.Text = m_content[k];
                }
                i++;
            }
        }
    }
}