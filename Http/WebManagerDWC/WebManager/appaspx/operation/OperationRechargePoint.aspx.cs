using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.operation
{
    public partial class OperationRechargePoint : System.Web.UI.Page
    {
        static string[] s_secondTile = { "日期", "渠道", "充值金额", "充值次数" };
        private string[] s_head;
        private string[] m_content;

        private PageGenLottery m_gen = new PageGenLottery(50);

        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck("operation", Session, Response);
            if (!IsPostBack)
            {
                m_channel.Items.Add(new ListItem("全部", ""));

                Dictionary<string, TdChannelInfo> data = TdChannel.getInstance().getAllData();
                foreach (var item in data.Values)
                {
                    m_channel.Items.Add(new ListItem(item.m_channelName, item.m_channelNo));
                }
            }
        }

        protected void onQuery(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];
            ParamQuery param = new ParamQuery();
            param.m_time = m_time.Text;
            param.m_param = m_channel.SelectedValue;

            OpRes res = user.doQuery(param, QueryType.queryTypeRechargePointStat);
            genTable(m_result, res, user);
        }

        private void genTable(Table table, OpRes res, GMUser user)
        {
            TableRow tr = new TableRow();
            TableCell td = null;

            ResultRechargePointStat qresult = (ResultRechargePointStat)user.getQueryResult(QueryType.queryTypeRechargePointStat);
            if (qresult.m_fields.Count == 0)
            {
                table.Rows.Add(tr);
                td = new TableCell();
                tr.Cells.Add(td);
                td.Text = OpResMgr.getInstance().getResultString(res);
                return;
            }

            genFirstHead(table, qresult);
            genSecondHead(table, qresult);

            int i = 0, j = 0;

            for (i = 0; i < qresult.m_result.Count; i++)
            {
                m_content[0] = qresult.m_result[i].m_time.ToShortDateString();

                // 该日期下的所有渠道
                foreach (var cdata in qresult.m_result[i].m_dic)
                {
                    tr = new TableRow();
                    table.Rows.Add(tr);

                    TdChannelInfo ci = TdChannel.getInstance().getValue(cdata.Key.ToString());
                    if (ci != null)
                    {
                        m_content[1] = ci.m_channelName; // 渠道
                    }
                    else
                    {
                        m_content[1] = "default";
                    }

                    j = 2;
                    foreach (var reason in qresult.m_fields)
                    {
                        ConsumeOneItem citem = cdata.Value.getValue(reason);
                        if (citem != null)
                        {
                            m_content[j++] = citem.m_totalValue.ToString(); // +"/" + citem.m_totalCount.ToString();
                            m_content[j++] = citem.m_totalCount.ToString();
                        }
                        else
                        {
                            m_content[j++] = "";
                            m_content[j++] = "";
                        }
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

        void genFirstHead(Table table, ResultRechargePointStat qresult)
        {
            int i = 0;
            TableCell td = null;
            TableRow tr = new TableRow();
            table.Rows.Add(tr);

            s_head = new string[2 + qresult.m_fields.Count];
            s_head[i++] = "";
            s_head[i++] = "";
            foreach (var r in qresult.m_fields)
            {
                s_head[i++] = ResultRechargePointStat.getPayName(r);
            }

            // 表头
            for (i = 0; i < s_head.Length; i++)
            {
                td = new TableCell();
                tr.Cells.Add(td);
                td.Text = "";
                if (i >= 2)
                {
                    td.Text = s_head[i];
                    td.ColumnSpan = 2;
                }
            }
        }

        void genSecondHead(Table table, ResultRechargePointStat qresult)
        {
            TableCell td = null;
            TableRow tr = new TableRow();
            table.Rows.Add(tr);
            for (int i = 0; i < 2; i++)
            {
                td = new TableCell();
                tr.Cells.Add(td);
                td.Text = s_secondTile[i];
            }

            int count = qresult.m_fields.Count * 2;
            for (int i = 0; i < count; i++)
            {
                td = new TableCell();
                tr.Cells.Add(td);
                td.Text = s_secondTile[2 + i % 2];
            }

            s_head = new string[2 + count];
            m_content = new string[2 + count];
        }
    }
}