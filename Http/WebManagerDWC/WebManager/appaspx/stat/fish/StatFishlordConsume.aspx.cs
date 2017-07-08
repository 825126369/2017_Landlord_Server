using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.stat.fish
{
    public partial class StatFishlordConsume : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck("stat", Session, Response);

            if (!IsPostBack)
            {
                moneyType.Items.Add("金币");
                moneyType.Items.Add("钻石");
            }
        }

        protected void genTable(Table table, GMUser user)
        {
            ParamTotalConsume param = new ParamTotalConsume();
            param.m_time = m_time.Text;
            param.m_currencyType = moneyType.SelectedIndex + 1;
            OpRes res = user.doQuery(param, QueryType.queryTypeFishConsume);
            if (res != OpRes.opres_success)
                return;

            ResultTotalConsume qresult = (ResultTotalConsume)user.getQueryResult(QueryType.queryTypeFishConsume);

            var fields = from f in qresult.m_fields orderby f ascending select f;

            int i = 0, k = 0;

            TableCell td = new TableCell();
            TableRow tr = new TableRow();
            tr.Cells.Add(td);
            table.Rows.Add(tr);

            // 生成行标题
            foreach (var reason in fields)
            {
                tr = new TableRow();
                table.Rows.Add(tr);

                td = new TableCell();
                td.Text = qresult.getFishReason(reason);
                tr.Cells.Add(td);
            }

            for (i = 0; i < qresult.getResultCount(); i++)
            {
                TotalConsumeItem item = qresult.m_result[i];

                td = new TableCell();
                td.Text = item.m_time.ToShortDateString();

                k = 0;
                tr = table.Rows[k];
                tr.Cells.Add(td);

                // 生成这个结果
                foreach (var reason in fields)
                {
                    k++;
                    tr = table.Rows[k];

                    td = new TableCell();
                    ConsumeOneItem citem = item.getValue(reason);
                    if (citem != null)
                    {
                        td.Text = citem.m_totalValue.ToString();
                    }
                    tr.Cells.Add(td);
                }
            }
        }

        protected void btnStat_Click(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];
            genTable(tabResult, user);
        }
    }
}