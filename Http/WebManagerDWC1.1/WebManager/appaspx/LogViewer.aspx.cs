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
        private static string[] s_head = new string[] {"ID", "游戏数据库IP", "帐号", "帐号IP", "操作类型", "操作时间", "描述"};
        private static string[] s_content = new string[s_head.Length];
        // 每页显示多少条数据
        private const int ROW_EACH_PAGE = 50;
        private int m_reqPage = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck("viewlog", Session, Response);

            try
            {
                m_reqPage = Convert.ToInt32(Request["page"]);
                if (m_reqPage == 0)
                {
                    m_reqPage = 1;
                }
            }
            catch (System.Exception ex)
            {
                m_reqPage = 1;
            }
            GMUser user = (GMUser)Session["user"];
            genTable(m_reqPage, user);
            long total = 0;
            PageBrowseGenerator pg = new PageBrowseGenerator();
           // user.totalRecord = OpLogMgr.getInstance().totalRecord;
            m_page.InnerHtml = pg.genPageFoot(m_reqPage, ROW_EACH_PAGE, @"/appaspx/LogViewer.aspx", ref total, user);
            if (total != 0)
            {
                m_foot.InnerHtml = m_reqPage + "/" + total;
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
            bool css = true;

            ParamQuery param = new ParamQuery();
            param.m_curPage = curpage;
            param.m_countEachPage = ROW_EACH_PAGE;
            QueryMgr mgr = user.getSys<QueryMgr>(SysType.sysTypeQuery);
            OpRes res = mgr.doQuery(param, QueryType.queryTypeOpLog, user);
            //List<Dictionary<string, object>> result = OpLogMgr.getInstance().getAllOpLog((curpage - 1) * ROW_EACH_PAGE, ROW_EACH_PAGE);
            List<Dictionary<string, object>> result =
                (List<Dictionary<string, object>>)mgr.getQueryResult(QueryType.queryTypeOpLog);
            if (result != null)
            {
                foreach (Dictionary<string, object> data in result)
                {
                    fillTable(data, css);
                    css = !css;
                }
            }
        }

        private void fillTable(Dictionary<string, object> data, bool css)
        {
            if (data == null)
            {
                return;
            }

            s_content[0] = Convert.ToString(data["id"]);
            s_content[1] = Convert.ToString(data["OpDbIP"]);
            s_content[2] = Convert.ToString(data["account"]);
            s_content[3] = Convert.ToString(data["accountIP"]);
            OpInfo opinfo = OpLogMgr.getInstance().getOpInfo(Convert.ToInt32(data["OpType"])); 
            if(opinfo != null)
            {
                s_content[4] = opinfo.m_opName;
            }
            else
            {
                s_content[4] = "未知";
            }

            s_content[5] = (Convert.ToDateTime(data["OpTime"])).ToLocalTime().ToString();
            if (opinfo.m_param != null)
            {
                s_content[6] = opinfo.m_param.getDescription(opinfo, Convert.ToString(data["OpParam"]));
            }
            else
            {
                s_content[6] = "未知";
            }
            TableRow tr = new TableRow();

            if (css)
            {
                tr.CssClass = "alt";
            }
            
            LogTable.Rows.Add(tr);
            int col = s_head.Length;
            for (int i = 0; i < col; i++)
            {
                TableCell td = new TableCell();
                tr.Cells.Add(td);
                td.Text = s_content[i];
            }
        }
    }
}