using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.stat
{
    public partial class StatTotalConsume : System.Web.UI.Page
    {
        private string[] s_head;
        private string[] m_content;

        private PageGenLottery m_gen = new PageGenLottery(50);

        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck(RightDef.DATA_TOTAL_CONSUME, Session, Response);
            if (!IsPostBack)
            {
                m_changeType.Items.Add("系统支出");
                m_changeType.Items.Add("系统收入");
            }
        }

        protected void onStat(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];
            ParamTotalConsume param = new ParamTotalConsume();
            param.m_changeType = m_changeType.SelectedIndex;
            param.m_currencyType = Convert.ToInt32(m_currencyType.SelectedValue);
            param.m_time = m_time.Text;

            QueryMgr mgr = user.getSys<QueryMgr>(SysType.sysTypeQuery);
            OpRes res = mgr.doQuery(param, QueryType.queryTypeTotalConsume, user);
            genTable(m_result, res, user, mgr);
        }

        private void genTable(Table table, OpRes res, GMUser user, QueryMgr mgr)
        {
            table.GridLines = GridLines.Both;
            TableRow tr = new TableRow();
            table.Rows.Add(tr);
            TableCell td = null;
            
            ResultTotalConsume qresult = (ResultTotalConsume)mgr.getQueryResult(QueryType.queryTypeTotalConsume);
            if (qresult.m_fields.Count == 0)
            {
                td = new TableCell();
                tr.Cells.Add(td);
                td.Text = OpResMgr.getInstance().getResultString(res);
                return;
            }

            int i = 0, j = 0;
            s_head = new string[1 + qresult.m_fields.Count];
            m_content = new string[1 + qresult.m_fields.Count];
            s_head[0] = "日期";

            foreach (var r in qresult.m_fields)
            {
                s_head[++i] = qresult.getReason(r);
            }

            // 表头
            for (i = 0; i < s_head.Length; i++)
            {
                td = new TableCell();
                tr.Cells.Add(td);
                td.Text = s_head[i];
            }

            for (i = 0; i < qresult.m_result.Count; i++)
            {
                tr = new TableRow();
                if ((i & 1) == 0)
                {
                    tr.CssClass = "alt";
                }
                table.Rows.Add(tr);

                m_content[0] = qresult.m_result[i].m_time.ToLongDateString();
                j = 1;
                foreach (var reason in qresult.m_fields)
                {
                    ConsumeOneItem citem = qresult.m_result[i].getValue(reason);
                    if (citem != null)
                    {
                        m_content[j] = citem.m_totalValue.ToString();
                        if (citem.m_totalCount > 0)
                        {
                            m_content[j] = m_content[j] + "/" + citem.m_totalCount.ToString();
                        }
                    }
                    else
                    {
                        m_content[j] = "";
                    }
                    j++;
                }

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