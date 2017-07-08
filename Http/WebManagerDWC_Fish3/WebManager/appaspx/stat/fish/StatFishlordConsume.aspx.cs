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
            RightMgr.getInstance().opCheck(RightDef.FISH_CONSUME, Session, Response);

            if (!IsPostBack)
            {
                moneyType.Items.Add("金币");
                moneyType.Items.Add("钻石");
                moneyType.Items.Add("物品");
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
            
            TableRow tr = new TableRow();
            table.Rows.Add(tr);
            TableCell td = new TableCell();
            tr.Cells.Add(td);
            td.Text = "";

            // 生成行标题
            foreach (var reason in fields)
            {
                tr = new TableRow();
                table.Rows.Add(tr);

                td = new TableCell();
                tr.Cells.Add(td);
                td.Text = qresult.getFishReason(reason);
            }

            for (i = 0; i < qresult.getResultCount(); i++)
            {
                TotalConsumeItem item = qresult.m_result[i];

                td = new TableCell();

                k = 0;
                tr = table.Rows[k];
                tr.Cells.Add(td);
                td.Text = item.m_time.ToShortDateString();

                // 生成这个结果
                foreach (var reason in fields)
                {
                    k++;
                    tr = table.Rows[k];

                    td = new TableCell();
                    tr.Cells.Add(td);
                    ConsumeOneItem citem = item.getValue(reason);
                    if (citem != null)
                    {
                        td.Text = citem.m_totalValue.ToString();
                    }
                    else
                    {
                        td.Text = "";
                    }
                }
            }
        }

        protected void btnStat_Click(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];
            if (moneyType.SelectedIndex == 2)
            {
                ParamTotalConsume param = new ParamTotalConsume();
                param.m_time = m_time.Text;
                param.m_currencyType = moneyType.SelectedIndex + 1;
                OpRes res = user.doQuery(param, QueryType.queryTypeFishConsume);
                TableFishConsumeItem view = new TableFishConsumeItem();
                view.genTable(user, tabResult, res);
            }
            else
            {
                genTable(tabResult, user);
            }
        }
    }
}