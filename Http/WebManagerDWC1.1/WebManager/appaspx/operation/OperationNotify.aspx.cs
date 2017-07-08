using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.operation
{
    public partial class OperationNotify : System.Web.UI.Page
    {
        private static string[] s_head = new string[] { "公告ID", "发送时间", "截止日期", "标题", "内容", "选择" };
        private string[] m_tableContent = new string[s_head.Length];
        private string m_selectStr = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck("operation", Session, Response);

            if (!IsPostBack)
            {
                genTable(m_result, (GMUser)Session["user"]);
            }
            else
            {
                m_selectStr = Request["sel"];
                if (m_selectStr == null)
                {
                    m_selectStr = "";
                }
            }
        }

        protected void onPublishNotice(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];
            ParamNotify param = new ParamNotify();
            param.m_id = m_noticeId.Text;
            param.m_title = m_title.Text;
            param.m_content = m_content.Text;
            param.m_day = m_day.Text;
            DyOpMgr mgr = user.getSys<DyOpMgr>(SysType.sysTypeDyOp);
            OpRes res = mgr.doDyop(param, DyOpType.opTypeNotify, user);
            m_res.InnerHtml = OpResMgr.getInstance().getResultString(res);

            genTable(m_result, user);
        }

        protected void onCancelNotice(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];
            if (m_selectStr != "")
            {
                ParamNotify param = new ParamNotify();
                param.m_opType = NoticeOpType.del;
                param.m_id = m_selectStr;
                DyOpMgr mgr = user.getSys<DyOpMgr>(SysType.sysTypeDyOp);
                OpRes res = mgr.doDyop(param, DyOpType.opTypeNotify, user);
            }

            genTable(m_result, user);
        }

        private void genTable(Table table, GMUser user)
        {
            table.GridLines = GridLines.Both;
            TableRow tr = new TableRow();
            table.Rows.Add(tr);
            TableCell td = null;

            QueryMgr mgr = user.getSys<QueryMgr>(SysType.sysTypeQuery);
            mgr.doQuery(null, QueryType.queryTypeCurNotice, user);
            List<ResultNoticeInfo> qresult = (List<ResultNoticeInfo>)mgr.getQueryResult(QueryType.queryTypeCurNotice);

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

                m_tableContent[0] = qresult[i].m_id;
                m_tableContent[1] = qresult[i].m_genTime;
                m_tableContent[2] = qresult[i].m_deadTime;
                m_tableContent[3] = qresult[i].m_title;
                m_tableContent[4] = qresult[i].m_content;
                m_tableContent[5] = Tool.getCheckBoxHtml("sel", m_tableContent[0], false);

                for (j = 0; j < s_head.Length; j++)
                {
                    td = new TableCell();
                    tr.Cells.Add(td);
                    td.Text = m_tableContent[j];
                }
            }
        }
    }
}