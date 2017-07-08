using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.stat
{
    public partial class StatFishlordDeskEarningsRate : System.Web.UI.Page
    {
        //private static string[] s_head = new string[] { "桌子ID", "系统总收入", "系统总支出", "盈亏情况", "实际盈利率" };
       // private string[] m_content = new string[s_head.Length];

        private PageGift m_gen = new PageGift(50);
        TableStatFishlordDeskEarningsRate m_common =
            new TableStatFishlordDeskEarningsRate(@"/appaspx/stat/StatFishlordDeskEarningsRate.aspx");

        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck("stat", Session, Response);

            if (!IsPostBack)
            {
                for (int i = 0; i < StrName.s_roomName.Length; i++)
                {
                    ListItem item = new ListItem(StrName.s_roomName[i], (i + 1).ToString());
                    m_room.Items.Add(item);
                }

                if (m_gen.parse(Request))
                {
                    m_room.SelectedIndex = m_gen.m_state;
                    btnQuery_Click(null, null);
                }
            }
        }

        protected void btnQuery_Click(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];

            /*ParamQueryGift param = new ParamQueryGift();
            param.m_curPage = m_gen.curPage;
            param.m_countEachPage = m_gen.rowEachPage;
            param.m_state = Convert.ToInt32(m_room.SelectedValue);

            user.doQuery(param, QueryType.queryTypeFishlordDeskParam);
            genDeskTable(m_result, user, param);*/
            m_common.onQueryDesk(user, m_gen, Convert.ToInt32(m_room.SelectedValue),
                m_result, QueryType.queryTypeFishlordDeskParam);

            m_page.InnerHtml = m_common.getPage();
            m_foot.InnerHtml = m_common.getFoot();
        }

        // 桌子的盈利率表格
       /* protected void genDeskTable(Table table, GMUser user, ParamQueryGift param)
        {
            m_page.InnerHtml = "";
            m_foot.InnerHtml = "";

            TableRow tr = new TableRow();
            table.Rows.Add(tr);

            int i = 0;
            for (; i < s_head.Length; i++)
            {
                TableCell td = new TableCell();
                tr.Cells.Add(td);
                td.Text = s_head[i];
            }

            List<ResultFishlordExpRate> qresult
                = (List<ResultFishlordExpRate>)user.getQueryResult(QueryType.queryTypeFishlordDeskParam);

            bool alt = true;
            foreach (var info in qresult)
            {
                tr = new TableRow();
                table.Rows.Add(tr);

                if (alt)
                {
                    tr.CssClass = "alt";
                }
                alt = !alt;

                m_content[0] = info.m_roomId.ToString();
                m_content[1] = info.m_totalIncome.ToString();
                m_content[2] = info.m_totalOutlay.ToString();
                m_content[3] = info.getDelta().ToString();
                m_content[4] = info.getFactExpRate();

                for (int j = 0; j < s_head.Length; j++)
                {
                    TableCell td = new TableCell();
                    tr.Cells.Add(td);
                    td.Text = m_content[j];
                }
            }

            param.m_state--;
            string page_html = "", foot_html = "";
            m_gen.genPage(param, @"/appaspx/stat/StatFishlordDeskEarningsRate.aspx", ref page_html, ref foot_html, user);
            m_page.InnerHtml = page_html;
            m_foot.InnerHtml = foot_html;
        }*/
    }
}