using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.account
{
    public partial class AccountStatSeller : System.Web.UI.Page
    {
        private static string[] s_head1 = new string[] { "日期", "存款", "存款次数", "提款", "提款次数" };
        private static string[] s_head2 = new string[] { "售货亭", "存款", "存款次数", "提款", "提款次数" };
        private static string[] s_head3 = new string[] { "售货亭管理", "存款", "存款次数", "提款", "提款次数" };

        private string[] m_content = new string[s_head1.Length];

        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck("", Session, Response);

            if (!IsPostBack)
            {
                m_way.Items.Add(new ListItem("日", ((int)StatSellerType.stat_seller_type_day).ToString()));
                m_way.Items.Add(new ListItem("月", ((int)StatSellerType.stat_seller_type_month).ToString()));
                m_way.Items.Add(new ListItem("管理员", ((int)StatSellerType.stat_seller_type_mgr).ToString()));
                m_way.Items.Add(new ListItem("售货亭", ((int)StatSellerType.stat_seller_type_seller).ToString()));
                m_way.SelectedIndex = 0;
               /* GMUser user = (GMUser)Session["user"];
                if (user.m_accType == AccType.ACC_SELLER_ADMIN)
                {
                    divSeller.Visible = false;
                }
                else if (user.m_accType == AccType.ACC_SELLER)
                {
                    string cmd = string.Format("select acc from {0} where owner='{1}' and accType={2}",
                        TableName.GM_ACCOUNT,
                        user.m_user,
                        AccType.ACC_SELLER_ADMIN);
                    List<Dictionary<string, object>> dataList =
                        user.sqlDb.queryList(cmd, user.getDbServerID(), MySqlDbName.DB_XIANXIA);
                    if (dataList != null)
                    {
                        m_sellerMgr.Items.Add(new ListItem("全部", "@@"));
                        for (int i = 0; i < dataList.Count; i++)
                        {
                            string str = Convert.ToString(dataList[i]["acc"]);
                            m_sellerMgr.Items.Add(new ListItem(str, str));
                        }
                    }
                }*/
            }
        }

        protected void onStat(object sender, EventArgs e)
        {
            ParamStatSeller param = new ParamStatSeller();
            param.m_timeRange = m_time.Text;
            param.m_sellerMgr = "";
            param.m_statType = Convert.ToInt32(m_way.SelectedValue);

            GMUser user = (GMUser)Session["user"];
            OpRes res = user.doStat(param, StatType.statTypeSeller);
            switch (param.m_statType)
            {
                case (int)StatSellerType.stat_seller_type_day:
                case (int)StatSellerType.stat_seller_type_month:
                    {
                        genTableByDay(m_result, res, user);
                    }
                    break;
                case (int)StatSellerType.stat_seller_type_seller:
                    {
                        genTableBySeller(m_result, res, user, s_head2);
                    }
                    break;
                case (int)StatSellerType.stat_seller_type_mgr:
                    {
                        genTableBySeller(m_result, res, user, s_head3);
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

            StatResultSeller qresult = (StatResultSeller)user.getStatResult(StatType.statTypeSeller);
            int i = 0, j = 0;
            // 表头
            for (i = 0; i < s_head1.Length; i++)
            {
                td = new TableCell();
                tr.Cells.Add(td);
                td.Text = s_head1[i];
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

                for (j = 0; j < s_head1.Length; j++)
                {
                    td = new TableCell();
                    tr.Cells.Add(td);
                    td.Text = m_content[j];
                }
            }
        }

        private void genTableBySeller(Table table, OpRes res, GMUser user, string[] head)
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

            StatResultSeller qresult = (StatResultSeller)user.getStatResult(StatType.statTypeSeller);
            int i = 0, j = 0;
            // 表头
            for (i = 0; i < head.Length; i++)
            {
                td = new TableCell();
                tr.Cells.Add(td);
                td.Text = head[i];
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

                m_content[0] = item.m_seller;
                m_content[1] = item.m_addScore.ToString();
                m_content[2] = item.m_addScoreCount.ToString();
                m_content[3] = item.m_desScore.ToString();
                m_content[4] = item.m_desScoreCount.ToString();

                for (j = 0; j < head.Length; j++)
                {
                    td = new TableCell();
                    tr.Cells.Add(td);
                    td.Text = m_content[j];
                }
            }
        }
    }
}