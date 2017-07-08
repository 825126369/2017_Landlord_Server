using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx
{
    public partial class EditRight : System.Web.UI.Page
    {
        private static string[] s_head = {"勾选修改", "账号", "可查看的渠道" };

        // 所选择的checkbox
        private string m_modifyFlagStr = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];
            RightSys.getInstance().opCheck("edit", Session, Response);

            if (IsPostBack)
            {
                m_modifyFlagStr = Request["flag"];
            }

            genTable(tabRight, user);
        }

        protected void genTable(Table result, GMUser user)
        {
            TableRow tr = new TableRow();
            TableCell td = null;

            int i = 0, k = 0;
            result.Rows.Add(tr);

            // 列标题
            for (i = 0; i < s_head.Length; i++)
            {
                td = new TableCell();
                td.Text = s_head[i];
                tr.Cells.Add(td);
            }

            OpRes res = user.doQuery(null, QueryType.queryTypeGmAccount);
            List<GMAccountItem> qresult = (List<GMAccountItem>)user.getQueryResult(QueryType.queryTypeGmAccount);
            
            // 添加内容
            for (i = 0; i < qresult.Count; i++)
            {
                GMAccountItem item = qresult[i];
                if (item.m_type == "admin")
                    continue;

                tr = new TableRow();
                result.Rows.Add(tr);

                // 第0列
                td = new TableCell();
                td.Text = Tool.getCheckBoxHtml("flag", item.m_user, false);
                tr.Cells.Add(td);

                // 第1列
                td = new TableCell();
                td.Text = item.m_user;
                tr.Cells.Add(td);

                // 第2列
                td = new TableCell();
                CheckBoxList chkList = getCheckBoxList(item);
                td.Controls.Add(chkList);
                tr.Cells.Add(td);
            }
        }

        protected void btnCommit_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(m_modifyFlagStr))
            {
                m_opRes.InnerText = "未选中任何账号";
                return;
            }

            string right = "";
            string[] arr = Tool.split(m_modifyFlagStr, ',', StringSplitOptions.RemoveEmptyEntries);
            foreach (var chkListName in arr)
            {
                right = "";
                CheckBoxList chkList = (CheckBoxList)tabRight.FindControl(chkListName);
                foreach (ListItem item in chkList.Items)
                {
                    if (item.Selected)
                    {
                        right += item.Value + ",";
                    }
                }

                AccountSys.getInstance().modifyViewChannel(chkListName, right);
            }
            m_opRes.InnerText = OpResMgr.getInstance().getResultString(OpRes.opres_success);
        }

        protected CheckBoxList getCheckBoxList(GMAccountItem user)
        {
            CheckBoxList chkList = new CheckBoxList();
            chkList.RepeatDirection = RepeatDirection.Horizontal;
            chkList.ID = user.m_user;

            List<ChannelInfo> cList = Channel.getInstance().m_cList;
            foreach (var cinfo in cList)
            {
                ListItem item = new ListItem();
                if (user.canViewChannel(cinfo.channelNo))
                {
                    item.Selected = true;
                }
                item.Text = cinfo.channelName;
                item.Value = cinfo.channelNo;
                chkList.Items.Add(item);
            }

            return chkList;

        }

        protected void btnDelAccount_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(m_modifyFlagStr))
                return;

            string[] arr = Tool.split(m_modifyFlagStr, ',', StringSplitOptions.RemoveEmptyEntries);
            foreach (var acc in arr)
            {
                AccountSys.getInstance().delAccount(acc);
            }

            tabRight.Rows.Clear();
            GMUser user = (GMUser)Session["user"];
            genTable(tabRight, user);
        }
    }
}