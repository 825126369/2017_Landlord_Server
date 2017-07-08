using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.operation
{
    public partial class OperationRank : System.Web.UI.Page
    {
        private static string[] s_head = new string[] { "日期", "排行", "玩家id", "账号", "玩家昵称", "金币净增长" };
        private string[] m_content = new string[s_head.Length];

        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck("operation", Session, Response);
        }

        private void genTable(Table table, OpRes res, GMUser user, QueryMgr mgr)
        {
            m_res.InnerHtml = "";
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

            List<ResultCoinGrowthRank> qresult = (List<ResultCoinGrowthRank>)mgr.getQueryResult(QueryType.queryTypeCoinGrowthRank);
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
                table.Rows.Add(tr);

                m_content[0] = qresult[i].m_time;
                m_content[1] = (i + 1).ToString();
                m_content[2] = qresult[i].m_playerId.ToString();
                m_content[3] = qresult[i].m_acc;
                m_content[4] = qresult[i].m_nickName;
                m_content[5] = qresult[i].m_gold.ToString();

                for (j = 0; j < s_head.Length; j++)
                {
                    td = new TableCell();
                    tr.Cells.Add(td);
                    td.Text = m_content[j];
                }
            }
        }

        protected void onSearch(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];
            QueryMgr mgr = user.getSys<QueryMgr>(SysType.sysTypeQuery);
            OpRes res = mgr.doQuery(m_time.Text, QueryType.queryTypeCoinGrowthRank, user);
            genTable(m_result, res, user, mgr);
        }

        protected void onClearRank(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];
            DBMgr.getInstance().clearTable(TableName.PUMP_COIN_GROWTH, user.getDbServerID(), DbName.DB_PUMP);
        }
    }
}