using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.stat
{
    public partial class StatFishlordStage : System.Web.UI.Page
    {
      //  private static string[] s_head = new string[] { "时间", "房间名称", "阶段", "收入", "支出", "盈利率" };
       // private string[] m_content = new string[s_head.Length];
        private PageGift m_gen = new PageGift(50);
        TableStatFishlordStage m_common =
            new TableStatFishlordStage(@"/appaspx/stat/StatFishlordStage.aspx");

        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck("stat", Session, Response);
            if (!IsPostBack)
            {
                for (int i = 0; i < StrName.s_roomName.Length; i++)
                {
                    m_room.Items.Add(StrName.s_roomName[i]);
                }

                if (m_gen.parse(Request))
                {
                    m_time.Text = m_gen.m_playerId;
                    m_room.SelectedIndex = m_gen.m_state;
                    onQuery(null, null);
                }
            }
        }

        protected void onQuery(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];
           /* ParamQueryGift param = new ParamQueryGift();
            param.m_param = m_time.Text;
            param.m_state = m_room.SelectedIndex;
            param.m_curPage = m_gen.curPage;
            param.m_countEachPage = m_gen.rowEachPage;

            QueryMgr mgr = user.getSys<QueryMgr>(SysType.sysTypeQuery);
            OpRes res = mgr.doQuery(param, QueryType.queryTypeFishlordStage, user);
            genTable(m_result, res, param, user, mgr);*/
            m_common.onQuery(user, m_time.Text, m_room.SelectedIndex, m_gen, m_result, QueryType.queryTypeFishlordStage);
            m_page.InnerHtml = m_common.getPage();
            m_foot.InnerHtml = m_common.getFoot();
        }

       /* private void genTable(Table table, OpRes res, ParamQueryGift param, GMUser user, QueryMgr mgr)
        {
            m_page.InnerHtml = "";
            m_foot.InnerHtml = "";
            
            table.GridLines = GridLines.Both;
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

            List<FishlordStageItem> qresult = (List<FishlordStageItem>)mgr.getQueryResult(QueryType.queryTypeFishlordStage);
            int i = 0, j = 0;
            // 表头
            for (i = 0; i < s_head.Length; i++)
            {
                td = new TableCell();
                tr.Cells.Add(td);
                td.Text = s_head[i];
            }

            for (i = 0; i < qresult.Count; i++)
            {
                tr = new TableRow();
                if ((i & 1) == 0)
                {
                    tr.CssClass = "alt";
                }
                table.Rows.Add(tr);

                m_content[0] = qresult[i].m_time;
                m_content[1] = StrName.s_roomName[qresult[i].m_roomId - 1];
                m_content[2] = StrName.s_stageName[qresult[i].m_stage];
                m_content[3] = qresult[i].m_income.ToString();
                m_content[4] = qresult[i].m_outlay.ToString();
                m_content[5] = qresult[i].getFactExpRate();

                for (j = 0; j < s_head.Length; j++)
                {
                    td = new TableCell();
                    tr.Cells.Add(td);
                    td.Text = m_content[j];
                }
            }

            string page_html = "", foot_html = "";
            m_gen.genPage(param, @"/appaspx/stat/StatFishlordStage.aspx", ref page_html, ref foot_html, user);
            m_page.InnerHtml = page_html;
            m_foot.InnerHtml = foot_html;
        }*/
    }
}