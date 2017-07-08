using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.service
{
    public partial class ServiceBlockAccount : System.Web.UI.Page
    {
        private static string[] s_head = new string[] { "停封时间", "账号", "选择" };
        private string[] m_content = new string[s_head.Length];
        private string m_accList = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck("service", Session, Response);
            if (IsPostBack)
            {
                m_accList = Request["sel"];
                if (m_accList == null)
                {
                    m_accList = "";
                }
            }
            else
            {
                GMUser user = (GMUser)Session["user"];
                DyOpMgr mgr = user.getSys<DyOpMgr>(SysType.sysTypeDyOp);
                genTable(m_result, user, mgr);
            }
        }

        protected void onBlockAccount(object sender, EventArgs e)
        {
            ParamBlock p = new ParamBlock();
            p.m_isBlock = true;
            p.m_param = m_acc.Text;
            p.m_comment = m_comment.Text;

            GMUser user = (GMUser)Session["user"];
            DyOpMgr mgr = user.getSys<DyOpMgr>(SysType.sysTypeDyOp);
            OpRes res = mgr.doDyop(p, DyOpType.opTypeBlockAcc, user);
            m_res.InnerHtml = OpResMgr.getInstance().getResultString(res);
            genTable(m_result, user, mgr);
        }

        protected void onUnBlockAccount(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];
            ParamBlock p = new ParamBlock();
            p.m_isBlock = false;
            p.m_param = m_accList;
            DyOpMgr mgr = user.getSys<DyOpMgr>(SysType.sysTypeDyOp);
            if (p.m_param != string.Empty)
            {
                OpRes res = mgr.doDyop(p, DyOpType.opTypeBlockAcc, user);
                m_res.InnerHtml = OpResMgr.getInstance().getResultString(res);
            }
            genTable(m_result, user, mgr);
        }

        private void genTable(Table table, GMUser user, DyOpMgr mgr)
        {
            table.GridLines = GridLines.Both;
            TableRow tr = new TableRow();
            table.Rows.Add(tr);
            TableCell td = null;

            int i = 0, j = 0;
            // 表头
            for (i = 0; i < s_head.Length; i++)
            {
                td = new TableCell();
                tr.Cells.Add(td);
                td.Text = s_head[i];
            }

            DyOpBlockAccount dyacc = (DyOpBlockAccount)mgr.getDyOp(DyOpType.opTypeBlockAcc);
            List<ResultBlock> resultList = new List<ResultBlock>();
            dyacc.getAccountList(user, resultList);

            for (i = 0; i < resultList.Count; i++)
            {
                tr = new TableRow();
                table.Rows.Add(tr);

                m_content[0] = resultList[i].m_time;
                m_content[1] = resultList[i].m_param;
                m_content[2] = Tool.getCheckBoxHtml("sel", m_content[1], false);
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
