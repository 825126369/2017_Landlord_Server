//#define _OLD_RIGHT_
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
        private static string[] s_head = new string[] { "账号", "人员类型", "修改标记" };
        private static string[] s_head1 = new string[] { "账号", "当前类型", "新类型", "操作" };
        private string[] m_content = new string[s_head1.Length];
        private List<GMAccountItem> m_gmList = new List<GMAccountItem>();
        // 所选择的checkbox
        private string m_modifyFlagStr = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck(RightDef.GM_TYPE_EDIT, Session, Response);

            GMUser user = (GMUser)Session["user"];
            if (IsPostBack)
            {
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
#if _OLD_RIGHT_
                List<AccountType> account = AccountMgr.getInstance().getAccountTypeList();
#else
                List<AccountType> account = RightMgr.getInstance().getAllGmType();
#endif
                foreach (AccountType acc in account)
                {
                    m_type.Items.Add(new ListItem(acc.m_name, acc.m_type));
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
            AccountMgr.getInstance().addAccount(account, key1, key2, m_type.SelectedValue, user);
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
            for (i = 0; i < s_head1.Length; i++)
            {
                td = new TableCell();
                tr.Cells.Add(td);
                td.Text = s_head1[i];
            }

            List<GMAccountItem> qresult = (List<GMAccountItem>)mgr.getQueryResult(QueryType.queryTypeGmAccount);

            for (i = 0; i < qresult.Count; i++)
            {
                if (qresult[i].m_type == "admin")
                    continue;

                GMAccountItem item = qresult[i];

                tr = new TableRow();
                table.Rows.Add(tr);

                m_content[0] = item.m_user;
#if _OLD_RIGHT_
                AccountType att = AccountMgr.getInstance().getAccountTypeByType(item.m_type);
                if (att != null)
                {
                    m_content[1] = att.m_name;
                }
                else
                {
                    m_content[1] = "";
                }
                
#else
                m_content[1] = RightMgr.getInstance().getGmTypeName(item.m_type);
#endif

                for (j = 0; j < s_head1.Length; j++)
                {
                    td = new TableCell();
                    tr.Cells.Add(td);

                    if (j == 2)
                    {
                        td.Controls.Add(getGmTypeList(i));
                    }
                    else if (j == 3)
                    {
                        HtmlInputButton btn2 = new HtmlInputButton();
                        btn2.Attributes.Add("value", "修改类型");
                        btn2.Attributes.Add("id", "btnm" +i.ToString());
                        btn2.Attributes.Add("acc", item.m_user);
                        td.Controls.Add(btn2);

                        HtmlInputButton btn1 = new HtmlInputButton();
                        btn1.Attributes.Add("value", "删除");
                        btn1.Attributes.Add("id", "btnd" + i.ToString());
                        btn1.Attributes.Add("acc", item.m_user);
                        td.Controls.Add(btn1);
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

        DropDownList getGmTypeList(int j)
        {
            DropDownList dt = new DropDownList();
            dt.ID = "newTypeList" + j.ToString();
#if _OLD_RIGHT_
            List<AccountType> account = AccountMgr.getInstance().getAccountTypeList();
#else
            List<AccountType> account = RightMgr.getInstance().getAllGmType();
#endif

            foreach (AccountType acc in account)
            {
                dt.Items.Add(new ListItem(acc.m_name, acc.m_type));
            }
            return dt;
        }
    }
}
