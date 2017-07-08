using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace WebManager.appaspx
{
    public partial class AddAccount : System.Web.UI.Page
    {
        private int m_selIndex = 0;
        private static string[] s_head = new string[] { "账号", "人员类型", "", "删除", "可查看渠道", "" };
        private string[] m_content = new string[s_head.Length];
        private List<GMAccountItem> m_gmList = new List<GMAccountItem>();
        // 所选择的checkbox
        private string m_modifyFlagStr = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck("addAccount", Session, Response);

            GMUser user = (GMUser)Session["user"];
            if (IsPostBack)
            {
                m_selIndex = m_type.SelectedIndex;

                m_modifyFlagStr = Request["flag"];
                if (m_modifyFlagStr == null)
                {
                    m_modifyFlagStr = "";
                }

                if (m_modifyFlagStr != "")
                {
                    string[] arr = Tool.split(m_modifyFlagStr, ',', StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < arr.Length; i++)
                    {
                        GMAccountItem item = new GMAccountItem();
                        item.m_user = arr[i];
                        item.m_type = Request[item.m_user];
                        m_gmList.Add(item);
                    }
                }
            }
            else
            {
                List<AccountType> account = AccountMgr.getInstance().getAccountTypeList();
                foreach (AccountType acc in account)
                {
                    m_type.Items.Add(acc.m_name);
                }

                genTable(m_curAccount, user);
            }
        }

        // 添加账号
        protected void onAddAccount(object sender, EventArgs e)
        {
            string account = m_accountName.Text;
            string key1 = m_key1.Text;
            string key2 = m_key2.Text;
            GMUser user = (GMUser)Session["user"];
            AccountMgr.getInstance().addAccount(account, key1, key2, m_selIndex, user);
            m_res.InnerHtml = user.getOpResultString();

            genTable(m_curAccount, user);
        }

        // 修改GM人员类型
        protected void onModifyGMType(object sender, EventArgs e)
        {
            AccountMgr.getInstance().updateAccount(m_gmList);
            GMUser user = (GMUser)Session["user"];
            genTable(m_curAccount, user);
        }

        protected void onDelAccount(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];
            for (int i = 0; i < m_gmList.Count; i++)
            {
                AccountMgr.getInstance().delAccount(m_gmList[i].m_user, user);
            }
            if (m_gmList.Count > 0)
            {
                genTable(m_curAccount, user);
            }
        }

        private void genTable(Table table, GMUser user)
        {
            table.GridLines = GridLines.Both;
            TableRow tr = new TableRow();
            table.Rows.Add(tr);
            TableCell td = null;
            QueryMgr mgr = user.getSys<QueryMgr>(SysType.sysTypeQuery);
            OpRes res = mgr.doQuery(null, QueryType.queryTypeGmAccount, user);

            if (res != OpRes.opres_success)
            {
                td = new TableCell();
                tr.Cells.Add(td);
                td.Text = OpResMgr.getInstance().getResultString(res);
                return;
            }

            int i = 0, j = 0;
            // 表头
            for (i = 0; i < s_head.Length; i++)
            {
                td = new TableCell();
                tr.Cells.Add(td);
                td.Text = s_head[i];
            }

            List<GMAccountItem> qresult = (List<GMAccountItem>)mgr.getQueryResult(QueryType.queryTypeGmAccount);

            for (i = 0; i < qresult.Count; i++)
            {
                if (qresult[i].m_type == "admin")
                    continue;

                tr = new TableRow();
                table.Rows.Add(tr);

                m_content[0] = qresult[i].m_user;
                //m_content[1] = getText(m_content[0], qresult[i].m_type);
                //m_content[2] = Tool.getCheckBoxHtml("flag", m_content[0], false);

                for (j = 0; j < s_head.Length; j++)
                {
                    td = new TableCell();
                    tr.Cells.Add(td);
                    if (j == 1) // 人员类型
                    {
                        DropDownList dp = getGmTypeList(qresult[i]);
                        td.Controls.Add(dp);
                    }
                    else if (j == 2) // 修改类型
                    {
                        HtmlGenericControl c1 = new HtmlGenericControl();
                        c1.TagName = "input";
                        c1.Attributes.Add("type", "button");
                        c1.Attributes.Add("class", "btn btn-primary");
                        c1.Attributes.Add("value", "修改类型");
                        c1.Attributes.Add("id", "btnType_" + m_content[0]);
                        td.Controls.Add(c1);
                    }
                    else if (j == 3) // 删除
                    {
                        HtmlGenericControl c1 = new HtmlGenericControl();
                        c1.TagName = "input";
                        c1.Attributes.Add("type", "button");
                        c1.Attributes.Add("class", "btn btn-primary");
                        c1.Attributes.Add("value", "删除");
                        c1.Attributes.Add("id", "btnDel_" + m_content[0]);
                        td.Controls.Add(c1);
                    }
                    else if (j == 4)
                    {
                        var dp = getChannelList(qresult[i]);
                        td.Controls.Add(dp);
                    }
                    else if (j == 5)
                    {
                        HtmlGenericControl c1 = new HtmlGenericControl();
                        c1.TagName = "input";
                        c1.Attributes.Add("type", "button");
                        c1.Attributes.Add("class", "btn btn-success");
                        c1.Attributes.Add("value", "修改可查看渠道");
                        c1.Attributes.Add("id", "btnChannel_" + m_content[0]);
                        td.Controls.Add(c1);
                    }
                    else
                    {
                        td.Text = m_content[j];
                    }
                }
            }
        }

        private string getText(string name, string cur_value)
        {
            string ret_text = "";
            List<AccountType> at_list = AccountMgr.getInstance().getAccountTypeList();
            int i = 0;
            for (; i < at_list.Count; i++)
            {
                bool res = false;
                if (cur_value == at_list[i].m_type)
                {
                    res = true;
                }
                ret_text += Tool.getRadioHtml(name, at_list[i].m_type, res, at_list[i].m_name);
                ret_text += "";
            }
            return ret_text;
        }

        HtmlGenericControl getChannelList(GMAccountItem gmInfo)
        {
            HtmlGenericControl dp = new HtmlGenericControl();
            dp.TagName = "select";
            dp.Attributes.Add("multiple", "multiple");
            dp.Attributes.Add("class", "form-control");
            dp.ID = "channel_" + gmInfo.m_user;
            dp.ClientIDMode = ClientIDMode.Static;
            Dictionary<string, TdChannelInfo> data = TdChannel.getInstance().getAllData();

            foreach (var item in data.Values)
            {
                HtmlGenericControl option = new HtmlGenericControl();
                option.TagName = "option";
                option.Attributes.Add("value", item.m_channelNo);
                option.InnerText = item.m_channelName;
                dp.Controls.Add(option);

                if (gmInfo.m_viewChannel.IndexOf(item.m_channelNo) >= 0)
                {
                    option.Attributes.Add("selected", "selected");
                }
            }
            
            return dp;
        }

        DropDownList getGmTypeList(GMAccountItem gmInfo)
        {
            DropDownList dp = new DropDownList();
            dp.ID = "gmType_" + gmInfo.m_user;
            dp.ClientIDMode = ClientIDMode.Static;

            List<AccountType> at_list = AccountMgr.getInstance().getAccountTypeList();

            foreach (var item in at_list)
            {
                dp.Items.Add(new ListItem(item.m_name, item.m_type));
            }

            for (int i = 0; i < dp.Items.Count; i++)
            {
                if (gmInfo.m_type == dp.Items[i].Value)
                {
                    dp.SelectedIndex = i;
                    break;
                }
            }

            return dp;
        }
    }
}
