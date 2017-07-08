using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.operation
{
    public partial class OperationGiftCodeQuery : System.Web.UI.Page
    {
        private static string[] s_head = new string[] { "生成时间", "礼包码", "目标平台", "对应礼包ID", "玩家所在服务器ID", "玩家平台", "玩家ID", "玩家账号", "使用时间", "是否使用" };
        private string[] m_content = new string[s_head.Length];
        private PageGenDailyTask m_gen = new PageGenDailyTask(50);

        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck("operation", Session, Response);

            if (!IsPostBack)
            {
                if (m_gen.parse(Request))
                {
                    m_giftCode.Text = m_gen.m_param;
                    m_time.Text = m_gen.m_time;
                    onQuery(null, null);
                }
            }
        }

        protected void onQuery(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];
            ParamQuery param = new ParamQuery();
            param.m_param = m_giftCode.Text;
            param.m_time = m_time.Text;
            param.m_curPage = m_gen.curPage;
            param.m_countEachPage = m_gen.rowEachPage;

            QueryMgr mgr = user.getSys<QueryMgr>(SysType.sysTypeQuery);
            OpRes res = mgr.doQuery(param, QueryType.queryTypeGiftCode, user);

            if (res == OpRes.opres_success)
            {
                genTable(m_result, param, user, mgr);
            }
            else
            {
                m_res.InnerHtml = OpResMgr.getInstance().getResultString(res);
            }
        }

        private void genTable(Table table, ParamQuery param, GMUser user, QueryMgr mgr)
        {
            m_page.InnerHtml = "";
            m_foot.InnerHtml = "";
            m_res.InnerHtml = "";
            table.GridLines = GridLines.Both;
            TableRow tr = new TableRow();
            table.Rows.Add(tr);
            TableCell td = null;

            List<GiftCodeItem> qresult = (List<GiftCodeItem>)mgr.getQueryResult(QueryType.queryTypeGiftCode);
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

                m_content[0] = qresult[i].m_genTime;   // 生成时间
                m_content[1] = qresult[i].m_giftCode;  // 礼包码
                m_content[2] = qresult[i].m_plat;      // 目标平台
                m_content[3] = qresult[i].m_giftId;      // 对应礼包ID
                m_content[4] = qresult[i].m_playerServerId.ToString(); // 玩家所在服务器ID
                m_content[5] = qresult[i].playerPlat; // 玩家平台
                m_content[6] = qresult[i].m_playerId.ToString(); // 玩家ID
                m_content[7] = qresult[i].m_playerAcc; // 玩家账号
                m_content[8] = qresult[i].m_playerId > 0 ? qresult[i].m_useTime : ""; // 使用时间
                m_content[9] = qresult[i].m_playerId > 0 ? "已使用" : "未使用"; // 是否使用

                for (j = 0; j < s_head.Length; j++)
                {
                    td = new TableCell();
                    tr.Cells.Add(td);
                    td.Text = m_content[j];
                }
            }

            string page_html = "", foot_html = "";
            m_gen.genPage(param, @"/appaspx/operation/OperationGiftCodeQuery.aspx", ref page_html, ref foot_html, user);
            m_page.InnerHtml = page_html;
            m_foot.InnerHtml = foot_html;
        }
    }
}