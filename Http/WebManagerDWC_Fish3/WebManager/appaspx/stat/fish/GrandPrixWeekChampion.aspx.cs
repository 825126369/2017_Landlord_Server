using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.stat.fish
{
    public partial class GrandPrixWeekChampion : System.Web.UI.Page
    {
        private static string[] s_head = new string[] { "日期", "玩家ID", "昵称", "积分" };
        private string[] m_content = new string[s_head.Length];

        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck(RightDef.FISH_WEEK_CHAMPION, Session, Response);

            onQuery(null, null);
        }

        protected void onQuery(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];
            ParamGrandPrix param = new ParamGrandPrix();
            param.m_queryType = ParamGrandPrix.QUERY_WEEK_CHAMPION;
            OpRes res = user.doQuery(param, QueryType.queryTypeGrandPrix);
            genTable(m_result, res, param, user);
        }

        private void genTable(Table table, OpRes res, ParamGrandPrix param, GMUser user)
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

            int i = 0, k = 0;

            for (i = 0; i < s_head.Length; i++)
            {
                td = new TableCell();
                tr.Cells.Add(td);
                td.Text = s_head[i];
            }

            List<ResultChampionItem> qresult =
                (List<ResultChampionItem>)user.getQueryResult(param, QueryType.queryTypeGrandPrix);

            for (i = 0; i < qresult.Count; i++)
            {
                tr = new TableRow();
                if ((i & 1) == 0)
                {
                    tr.CssClass = "alt";
                }
                m_result.Rows.Add(tr);
                ResultChampionItem item = qresult[i];

                m_content[0] = item.m_time;
                m_content[1] = item.m_playerId.ToString();
                m_content[2] = item.m_nickName;
                m_content[3] = item.m_bestScore.ToString();

                for (k = 0; k < s_head.Length; k++)
                {
                    td = new TableCell();
                    tr.Cells.Add(td);
                    td.Text = m_content[k];
                }
            }
        }
    }
}