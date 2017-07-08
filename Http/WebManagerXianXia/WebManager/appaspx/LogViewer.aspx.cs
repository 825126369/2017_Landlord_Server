using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx
{
    // 查看日志
    public partial class LogViewer : System.Web.UI.Page
    {
        private static string[] s_head = new string[] { "ID", "帐号", "帐号IP", "操作类型", "操作时间", "描述", "备注" };
        private string[] m_content = new string[s_head.Length];
        private PageViewLog m_gen = new PageViewLog(50);

        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck("viewlog", Session, Response);

            if (!IsPostBack)
            {
                int count = OpLogMgr.getInstance().getOpInfoCount();
                opType.Items.Add(new ListItem("全部", "0"));
                for (int i = 1; i < LogType.LOG_TYPE_MAX; i++)
                {
                    OpInfo info = OpLogMgr.getInstance().getOpInfo(i);
                    if (info != null)
                    {
                        opType.Items.Add(new ListItem(info.m_opName, info.m_opType.ToString()));
                    }
                }

                if (m_gen.parse(Request))
                {
                    opType.SelectedIndex = m_gen.m_opType;
                    m_time.Text = m_gen.m_time;
                    onSearchLog(null, null);
                }
            }
        }

        private void genTable(int curpage, GMUser user)
        {
            LogTable.GridLines = GridLines.Both;
            // 添加标题行
            TableRow tr = new TableRow();
            LogTable.Rows.Add(tr);
            int col = s_head.Length;
            int i = 0;
            for (; i < col; i++)
            {
                TableCell td = new TableCell();
                tr.Cells.Add(td);
                td.Text = s_head[i];
            }
        }

        private void fillTable(ResultOpLogItem data, bool css)
        {
            TableRow tr = new TableRow();
            if (css)
            {
                tr.CssClass = "alt";
            }

            m_content[0] = data.m_id.ToString();
            m_content[1] = data.m_opAcc;
            m_content[2] = data.m_opAccIP;
            m_content[3] = data.m_opName;
            m_content[4] = data.m_opDateTime;
            m_content[5] = data.m_opDesc;
            m_content[6] = data.m_comment;

            LogTable.Rows.Add(tr);
            int col = s_head.Length;
            for (int i = 0; i < col; i++)
            {
                TableCell td = new TableCell();
                tr.Cells.Add(td);
                td.Text = m_content[i];
            }
        }

        protected void onSearchLog(object sender, EventArgs e)
        {
            ParamQueryOpLog param = new ParamQueryOpLog();
            param.m_logType = int.Parse(opType.SelectedValue);
            param.m_time = m_time.Text;
            param.m_curPage = m_gen.curPage;
            param.m_countEachPage = m_gen.rowEachPage;

            GMUser user = (GMUser)Session["user"];
            genTable(user, param);
        }

        private void genTable(GMUser user, ParamQueryOpLog param)
        {
            LogTable.GridLines = GridLines.Both;
            // 添加标题行
            TableRow tr = new TableRow();
            LogTable.Rows.Add(tr);
            int col = s_head.Length;
            int i = 0;
            for (; i < col; i++)
            {
                TableCell td = new TableCell();
                tr.Cells.Add(td);
                td.Text = s_head[i];
            }

            QueryMgr mgr = user.getSys<QueryMgr>(SysType.sysTypeQuery);
            OpRes res = mgr.doQuery(param, QueryType.queryTypeOpLog, user);
            List<ResultOpLogItem> result = (List<ResultOpLogItem>)mgr.getQueryResult(QueryType.queryTypeOpLog);
            if (result != null)
            {
                bool css = true;

                foreach (ResultOpLogItem data in result)
                {
                    fillTable(data, css);
                    css = !css;
                }
            }

            m_page.InnerHtml = "";
            m_foot.InnerHtml = "";
            string page_html = "", foot_html = "";
            param.m_logType = opType.SelectedIndex;
            m_gen.genPage(param, @"/appaspx/LogViewer.aspx", ref page_html, ref foot_html, user);
            m_page.InnerHtml = page_html;
            m_foot.InnerHtml = foot_html;
        }
    }
}
