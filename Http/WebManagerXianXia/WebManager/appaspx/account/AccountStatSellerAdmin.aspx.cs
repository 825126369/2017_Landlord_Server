using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.account
{
    public partial class AccountStatSellerAdmin : System.Web.UI.Page
    {
        private static string[] s_head = new string[] { "日期", "存款", "存款次数", "提款", "提款次数", "(收入)存款 - 提款", "活跃玩家", "投注", "盈利", "净亏损" };
        private string[] m_content = new string[s_head.Length];

        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck("", Session, Response);

            if (!IsPostBack)
            {
                m_way.Items.Add(new ListItem("日", ((int)StatSellerType.stat_seller_type_day).ToString()));
                m_way.Items.Add(new ListItem("月", ((int)StatSellerType.stat_seller_type_month).ToString()));
                m_way.SelectedIndex = 0;

                m_searchCond.setLevelName(true);
            }
        }

        protected void onStat(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];
            ParamStatSeller param = new ParamStatSeller();
            param.m_statType = Convert.ToInt32(m_way.SelectedValue);
            m_searchCond.fillCondtion(param, user);

            OpRes res = user.doStat(param, StatType.statTypeSellerAdmin);
            switch (param.m_statType)
            {
                case (int)StatSellerType.stat_seller_type_day:
                case (int)StatSellerType.stat_seller_type_month:
                    {
                        genTableByDay(m_result, res, user);
                    }
                    break;
            }
        }

        private void genTableByDay(Table table, OpRes res, GMUser user)
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

            StatResultSeller qresult = (StatResultSeller)user.getStatResult(StatType.statTypeSellerAdmin);
            int i = 0, j = 0;
            // 表头
            for (i = 0; i < s_head.Length; i++)
            {
                td = new TableCell();
                tr.Cells.Add(td);
                td.Text = s_head[i];
            }

            for (i = 0; i < qresult.m_items.Count; i++)
            {
                StatResultSellerItem item = qresult.m_items[i];
                tr = new TableRow();
                if ((i & 1) == 0)
                {
                    tr.CssClass = "alt";
                }
                table.Rows.Add(tr);

                m_content[0] = item.m_time;
                m_content[1] = item.m_addScore.ToString();
                m_content[2] = item.m_addScoreCount.ToString();
                m_content[3] = item.m_desScore.ToString();
                m_content[4] = item.m_desScoreCount.ToString();
                m_content[5] = (item.m_addScore - item.m_desScore).ToString();

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