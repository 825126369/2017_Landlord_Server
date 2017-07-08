using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.operation
{
    public partial class OperationMoneyWarn : System.Web.UI.Page
    {
        private static string[] s_head = new string[] { "玩家ID", "玩家昵称", "金币", "保险箱" };
        private static string[] s_head1 = new string[] { "玩家ID", "玩家昵称", "钻石" };
        private string[] m_content = new string[s_head.Length];

        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck("operation", Session, Response);
            if (!IsPostBack)
            {
                m_currency.Items.Add("金币");
                m_currency.Items.Add("钻石");
            }
        }

        protected void onQuery(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];
            ParamQuery param = createParam();

            QueryMgr mgr = user.getSys<QueryMgr>(SysType.sysTypeQuery);
            OpRes res = mgr.doQuery(param, QueryType.queryTypeMoneyAtMost, user);
            genTable(m_result, param, user, mgr);
        }

        protected void onExport(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];
            ParamQuery param = createParam();
            ExportMgr mgr = user.getSys<ExportMgr>(SysType.sysTypeExport);
            OpRes res = mgr.doExport(param, ExportType.exportTypeMoneyWarn, user);
            m_res.InnerHtml = OpResMgr.getInstance().getResultString(res);
        }

        private ParamQuery createParam()
        {
            ParamQuery param = new ParamQuery();
            param.m_way = (QueryWay)m_currency.SelectedIndex;
            if (m_currency.SelectedIndex == 0)
            {
                param.m_countEachPage = 300; // 金币300位
            }
            else
            {
                param.m_countEachPage = 50; // 礼券50位
            }
            return param;
        }

        private void genTable(Table table, ParamQuery param, GMUser user, QueryMgr mgr)
        {
            table.GridLines = GridLines.Both;
            TableRow tr = new TableRow();
            table.Rows.Add(tr);
            TableCell td = null;

            List<ResultMoneyMost> qresult = (List<ResultMoneyMost>)mgr.getQueryResult(QueryType.queryTypeMoneyAtMost);
            int i = 0, j = 0;
            string[] head = null;
            if (param.m_way == QueryWay.by_way0)
            {
                head = s_head;
            }
            else
            {
                head = s_head1;
            }

            // 表头
            for (i = 0; i < head.Length; i++)
            {
                td = new TableCell();
                tr.Cells.Add(td);
                td.Text = head[i];
            }

            for (i = 0; i < qresult.Count; i++)
            {
                tr = new TableRow();
                if ((i & 1) == 0)
                {
                    tr.CssClass = "alt";
                }
                table.Rows.Add(tr);

                m_content[0] = qresult[i].m_playerId.ToString();
                m_content[1] = qresult[i].m_nickName;
                m_content[2] = qresult[i].m_val.ToString();
                m_content[3] = qresult[i].m_safeBox.ToString();

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
