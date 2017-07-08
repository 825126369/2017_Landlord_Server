using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.stat
{
    public partial class StatExchange : System.Web.UI.Page
    {
        private string[] s_head;
        private string[] m_content;

        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck(RightDef.DATA_SHOP_EXCHANGE, Session, Response);
        }

        protected void onQuery(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];
            ParamQuery param = new ParamQuery();
            param.m_time = m_time.Text;

            OpRes res = user.doQuery(param, QueryType.queryTypeExchangeStat);
            genTable(m_result, res, user);
        }

        private void genTable(Table table, OpRes res, GMUser user)
        {
            TableRow tr = new TableRow();
            table.Rows.Add(tr);
            TableCell td = null;

            ResultExchangeStat qresult = (ResultExchangeStat)user.getQueryResult(QueryType.queryTypeExchangeStat);
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
                s_head[++i] = qresult.getExchangeName(r);
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

                m_content[0] = qresult.m_result[i].m_time.ToShortDateString();
                j = 1;
                foreach (var reason in qresult.m_fields)
                {
                    ConsumeOneItem citem = qresult.m_result[i].getValue(reason);
                    if (citem != null)
                    {
                        m_content[j] = citem.m_totalValue.ToString();
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