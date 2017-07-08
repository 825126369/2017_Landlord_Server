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
        private static string[] s_head = new string[] { "ID", "游戏数据库IP", "帐号", "帐号IP", "操作类型", "操作时间", "描述", "备注" };
        private static string[] s_content = new string[s_head.Length];
        private PageViewLog m_gen = new PageViewLog(50);

        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck("viewlog", Session, Response);

            if (!IsPostBack)
            {
                int count = OpLogMgr.getInstance().getOpInfoCount();
                for (int i = -1; i < count; i++)
                {
                    OpInfo info = OpLogMgr.getInstance().getOpInfo(i);
                    if (info != null)
                    {
                        opType.Items.Add(new ListItem(info.m_opName, info.m_opType.ToString()));
                    }
                    else
                    {
                        opType.Items.Add(new ListItem("全部", i.ToString()));
                    }
                }

                GMUser user = (GMUser)Session["user"];
                if (user.m_type != "admin")
                {
                    Button2.Visible = false;
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

            if (data.ContainsKey("comment"))
            {
                s_content[7] = Convert.ToString(data["comment"]);
            }
            else
            {
                s_content[7] = "";
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

        protected void onDelAllLog(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];
            ParamDelData param = new ParamDelData();
            param.m_tableName = TableName.OPLOG;
            OpRes res = user.doDyop(param, DyOpType.opTypeRemoveData);

            LogTable.Rows.Clear();
            TableRow tr = new TableRow();
            LogTable.Rows.Add(tr);
            TableCell td = new TableCell();
            tr.Cells.Add(td);
            td.Text = OpResMgr.getInstance().getResultString(res);
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
            List<Dictionary<string, object>> result =
                (List<Dictionary<string, object>>)mgr.getQueryResult(QueryType.queryTypeOpLog);
            if (result != null)
            {
                bool css = true;

                foreach (Dictionary<string, object> data in result)
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
