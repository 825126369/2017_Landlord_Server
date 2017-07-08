using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx
{
    public partial class Right : System.Web.UI.Page
    {
        private static string[] s_head = new string[] { "权限名称", "程序", "策划", "运营", "客服", "运营总监", "CEO" };
        private static string[] s_type = new string[] { "program", "plan", "operation", "service", "opDirector", "ceo" };
        private string[] s_content = new string[s_head.Length];
        private string[] m_commit = new string[s_type.Length];

        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck(RightDef.GM_TYPE_EDIT, Session, Response);
            if (IsPostBack)
            {
                for (int i = 0; i < s_type.Length; i++)
                {
                    m_commit[i] = Request[s_type[i]];
                }
            }
            else
            {
                genTable();
            }
        }

        private void genTable()
        {
            m_right.GridLines = GridLines.Both;
            // 添加标题行
            TableRow tr = new TableRow();
            m_right.Rows.Add(tr);
            int col = s_head.Length;
            int i = 0;
            for (; i < col; i++)
            {
                TableCell td = new TableCell();
                tr.Cells.Add(td);
                td.Text = s_head[i];
            }

            List<Dictionary<string, object>> rdb = RightMgr.getInstance().getAllRight();
            // 找出所有权限名称
            Dictionary<string, string> right_map = RightMgr.getInstance().getRightMap();
            if (right_map != null)
            {
               foreach(var right in right_map) // 循环行，以所有权限作为行
               {
                   s_content[0] = right.Value;
                   tr = new TableRow();
                   m_right.Rows.Add(tr);
                   for (i = 0; i < col; i++)  // 列
                   {
                       TableCell td = new TableCell();
                       tr.Cells.Add(td);
                       if (i != 0)
                       {
                           bool res = isHasRight(s_type[i-1], right.Key, rdb);
                           td.Text = "<input type= \"checkbox\" name=" + s_type[i-1] + getChecked(res) + " value= " + "\"" + right.Key + "\"" + " runat=\"server\" />";
                       }
                       else
                       {
                           td.Text = s_content[i];
                       }
                   }
                }
            }
        }

        private string getChecked(bool issel)
        {
            return issel ? " checked=\"true\"" : "";
        }

        // 某种类型的人员是否具备某个权限
        private bool isHasRight(string type, string rightname, List<Dictionary<string, object>> rdb)
        {
            if (rdb == null)
                return false;

            foreach (Dictionary<string, object> data in rdb)
            {
                if (Convert.ToString(data["type"]) == type)
                {
                    string str = Convert.ToString(data["right"]);
                    if (str.IndexOf(rightname) >= 0)
                        return true;
                }
            }
            return false;
        }

        // 提交人员权限的修改
        protected void onCommitRight(object sender, EventArgs e)
        {
            bool res = true;
            for (int i = 0; i < m_commit.Length; i++)
            {
                if (m_commit[i] != null)
                {
                    if (RightMgr.getInstance().modifyRight(s_type[i], m_commit[i]) == false)
                    {
                        res = false;
                    }
                }
            }
            m_res.InnerHtml = OpResMgr.getInstance().getResultString(res ? OpRes.opres_success : OpRes.op_res_failed);
            genTable();
        }
    }
}
