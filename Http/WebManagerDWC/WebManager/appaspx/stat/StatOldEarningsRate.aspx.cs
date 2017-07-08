using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.stat
{
    public partial class StatOldEarningsRate : System.Web.UI.Page
    {
        private static string[] s_head = new string[] { "游戏", "重置时间", "房间", "重置前房间总收入", "重置前房间总支出", "重置前盈利率", "重置前期望盈利率" };
        private string[] m_content = new string[s_head.Length];

        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck("stat", Session, Response);

            if (!IsPostBack)
            {
                for (int i = 1; i < (int)GameId.gameMax; i++)
                {
                    m_game.Items.Add(new ListItem(StrName.s_gameName[i], Convert.ToString(i)));
                }
            }
        }

        protected void onQuery(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];
            ParamLottery p = new ParamLottery();
            ListItem item = m_game.Items[m_game.SelectedIndex];
            p.m_way = (QueryWay)Convert.ToInt32(item.Value);

            QueryMgr mgr = user.getSys<QueryMgr>(SysType.sysTypeQuery);
            OpRes res = mgr.doQuery(p, QueryType.queryTypeOldEaringsRate, user);
            genExpRateTable(m_result, mgr, p);
        }

        // 期望盈利率表格
        protected void genExpRateTable(Table table, QueryMgr mgr, ParamLottery p)
        {
            GMUser user = (GMUser)Session["user"];

            table.GridLines = GridLines.Both;
            TableRow tr = new TableRow();
            table.Rows.Add(tr);

            int i = 0;
            for (; i < s_head.Length; i++)
            {
                TableCell td = new TableCell();
                tr.Cells.Add(td);
                td.Text = s_head[i];
            }

            List<ResultOldEarningRateItem> qresult =
                (List<ResultOldEarningRateItem>)mgr.getQueryResult(QueryType.queryTypeOldEaringsRate);

            for (i = 0; i < qresult.Count; i++)
            {
                if ((i & 1) == 0)
                {
                    tr.CssClass = "alt";
                }

                m_content[0] = StrName.s_gameName[qresult[i].m_gameId];
                m_content[1] = qresult[i].m_resetTime;
                m_content[2] = StrName.s_roomName[qresult[i].m_roomId - 1];
                m_content[3] = qresult[i].m_income.ToString();
                m_content[4] = qresult[i].m_outlay.ToString();
                m_content[5] = qresult[i].getFactExpRate();
                m_content[6] = qresult[i].m_expRate.ToString();

                tr = new TableRow();
                table.Rows.Add(tr);
                for (int j = 0; j < s_head.Length; j++)
                {
                    TableCell td = new TableCell();
                    tr.Cells.Add(td);
                    td.Text = m_content[j];
                }
            }
        }
    }
}