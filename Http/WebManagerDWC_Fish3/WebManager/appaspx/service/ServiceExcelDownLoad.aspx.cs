using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

namespace WebManager.appaspx.service
{
    public partial class ExcelDownLoad : System.Web.UI.Page
    {
        private static string[] s_head = new string[] { "表名", "生成时间" };
        private string[] m_content = new string[s_head.Length];

        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck("", Session, Response);

            GMUser user = (GMUser)Session["user"];
            genTable(m_result, user);
        }

        private void genTable(Table table, GMUser user)
        {
            m_result.GridLines = GridLines.Both;
            TableRow tr = new TableRow();
            m_result.Rows.Add(tr);
            TableCell td = null;
           
            int i = 0, j = 0;
            // 表头
            for (i = 0; i < s_head.Length; i++)
            {
                td = new TableCell();
                tr.Cells.Add(td);
                td.Text = s_head[i];
            }

            string dir = HttpRuntime.BinDirectory + "..\\excel\\";

            string path = Path.Combine(dir, user.m_user);
            if (Directory.Exists(path))
            {
                string[] allExcels = Directory.GetFiles(path);

                for (i = 0; i < allExcels.Length; i++)
                {
                    tr = new TableRow();
                    if ((i & 1) == 0)
                    {
                        tr.CssClass = "alt";
                    }
                    m_result.Rows.Add(tr);

                    string fileName = Path.GetFileName(allExcels[i]);
                    string href = string.Format("/excel/{0}/{1}", user.m_user, fileName);
                    m_content[0] = "<a href=\"" + href + "\">" + fileName + "</a>";
                    m_content[1] = Directory.GetLastWriteTime(allExcels[i]).ToString();

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
}
