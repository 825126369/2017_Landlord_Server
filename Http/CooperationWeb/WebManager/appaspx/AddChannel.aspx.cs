using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx
{
    public partial class AddChannel : System.Web.UI.Page
    {
        private static string[] s_head = { "选择", "序号", "渠道编号", "渠道名称" };

        // 所选择的checkbox
        private string m_modifyFlagStr = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            RightSys.getInstance().opCheck("channel", Session, Response);

            if (!IsPostBack)
            {
                genTable(tabChannel);
            }
            else
            {
                m_modifyFlagStr = Request["flag"];
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            bool res = ChannelSys.getInstance().addChannel(txtChannelNo.Text, txtChannelName.Text);
            OpRes op = OpRes.opres_success;
            if (!res)
            {
                op = OpRes.op_res_failed;
            }
            
            genTable(tabChannel);
            m_opRes.InnerText = OpResMgr.getInstance().getResultString(op);
        }

        protected void genTable(Table result)
        {
            int i = 0;
            TableRow tr = new TableRow();
            TableCell td = null;

            result.Rows.Add(tr);
            for (i = 0; i < s_head.Length; i++)
            {
                td = new TableCell();
                td.Text = s_head[i];
                tr.Cells.Add(td);
            }

            List<ChannelInfo> cList = Channel.getInstance().m_cList;
            i = 1;
            foreach (var info in cList)
            {
                tr = new TableRow();
                result.Rows.Add(tr);

                // 勾选
                td = new TableCell();
                td.Text = Tool.getCheckBoxHtml("flag", info.channelNo, false);
                tr.Cells.Add(td);

                // 序号
                td = new TableCell();
                td.Text = i.ToString();
                tr.Cells.Add(td);

                // 渠道编号
                td = new TableCell();
                td.Text = info.channelNo;
                tr.Cells.Add(td);

                // 渠道名称
                td = new TableCell();
                td.Text = info.channelName;
                tr.Cells.Add(td);

                i++;
            }
        }

        protected void btnDel_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(m_modifyFlagStr))
            {
                return;
            }

            string[] arr = Tool.split(m_modifyFlagStr, ',', StringSplitOptions.RemoveEmptyEntries);
            foreach (var channel in arr)
            {
                ChannelSys.getInstance().delChannel(channel);
            }

            genTable(tabChannel);
        }
    }
}