using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.stat.cows
{
    public partial class CowsPlayerBanker : System.Web.UI.Page
    {
        private static string[] s_head = new string[] { "时间", "玩家ID", "昵称", "连庄次数", "上庄时金币", "下庄时金币", "获利情况", "手续费", "爆庄支出" };
        private string[] m_content = new string[s_head.Length];
        private PageGenDailyTask m_gen = new PageGenDailyTask(50);

        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck("stat", Session, Response);

            if (!IsPostBack)
            {
                if (m_gen.parse(Request))
                {
                    txtPlayerId.Text = m_gen.m_param;
                    __time__.Text = m_gen.m_time;
                    onQuery(null, null);
                }
            }
        }

        protected void onQuery(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];
            ParamQuery param = new ParamQuery();
            param.m_curPage = m_gen.curPage;
            param.m_countEachPage = m_gen.rowEachPage;
            param.m_param = txtPlayerId.Text;
            param.m_time = __time__.Text;

            OpRes res = user.doQuery(param, QueryType.queryTypeCowsPlayerBanker);
            genResultTable(m_result, user, res, param);
        }

        protected void genResultTable(Table table, GMUser user, OpRes res, ParamQuery param)
        {
            m_page.InnerHtml = "";
            m_foot.InnerHtml = "";

            TableRow tr = new TableRow();
            table.Rows.Add(tr);

            if (res != OpRes.opres_success)
            {
                TableCell td = new TableCell();
                tr.Cells.Add(td);
                td.Text = OpResMgr.getInstance().getResultString(res);
                return;
            }

            int i = 0;
            for (; i < s_head.Length; i++)
            {
                TableCell td = new TableCell();
                tr.Cells.Add(td);
                td.Text = s_head[i];
            }

            List<ResultPlayerBankerInfo> qresult
                = (List<ResultPlayerBankerInfo>)user.getQueryResult(QueryType.queryTypeCowsPlayerBanker);

            foreach (var info in qresult)
            {
                m_content[0] = info.m_genTime;
                m_content[1] = info.m_playerId.ToString();
                m_content[2] = info.m_nickName;
                m_content[3] = info.m_bankerCount.ToString();
                m_content[4] = ItemHelp.showMoneyValue(info.m_beforeGold).ToString();
                m_content[5] = ItemHelp.showMoneyValue(info.m_nowGold).ToString();
                m_content[6] = ItemHelp.showMoneyValue(info.m_resultValue).ToString();
                m_content[7] = ItemHelp.showMoneyValue(info.m_sysGet).ToString();
                m_content[8] = ItemHelp.showMoneyValue(info.m_sysLose).ToString();

                tr = new TableRow();
                table.Rows.Add(tr);
                for (int j = 0; j < s_head.Length; j++)
                {
                    TableCell td = new TableCell();
                    tr.Cells.Add(td);
                    td.Text = m_content[j];
                }
            }

            string page_html = "", foot_html = "";
            m_gen.genPage(param, @"/appaspx/stat/cows/CowsPlayerBanker.aspx", ref page_html, ref foot_html, user);
            m_page.InnerHtml = page_html;
            m_foot.InnerHtml = foot_html;
        }
    }
}