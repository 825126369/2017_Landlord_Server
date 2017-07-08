using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.service
{
    public partial class ServiceBenefitHelp : System.Web.UI.Page
    {
        private static string[] s_head = new string[] { "道具ID", "道具名称" };

        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck("service", Session, Response);
            genTable(m_result);
        }

        private void genTable(Table table)
        {
            table.GridLines = GridLines.Both;
            TableRow tr = new TableRow();
            table.Rows.Add(tr);
            TableCell td = null;

            int i = 0;
            for (i = 0; i < s_head.Length; i++)
            {
                td = new TableCell();
                tr.Cells.Add(td);
                td.Text = s_head[i];
            }

//             Dictionary<Int32, ItemData> all_item = ViSealedDB<ItemData>.Datas;
// 
//             foreach (var item in all_item)
//             {
//                 tr = new TableRow();
//                 table.Rows.Add(tr);
// 
//                 td = new TableCell();
//                 tr.Cells.Add(td);
//                 td.Text = item.Key.ToString();
// 
//                 td = new TableCell();
//                 tr.Cells.Add(td);
//                 td.Text = item.Value.Name;
//             }
        }
    }
}