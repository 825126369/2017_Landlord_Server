using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.account
{
    // 搜索具体的玩家
    public partial class AccountSearch : System.Web.UI.Page
    {
        private static string[] s_head = new string[] { "玩家", "注册日期", "售货亭", "售货亭管理", "所持货币" };
        private string[] m_content = new string[s_head.Length];

        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck("", Session, Response);
        }

        protected void onQueryMember(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];
            ParamMemberInfo param = new ParamMemberInfo();
            param.m_time = m_time.Text;

            OpRes res = user.doQuery(param, QueryType.queryTypePlayerMember);
            genTable(m_result, res, user);
        }

        private void genTable(Table table, OpRes res, GMUser user)
        {
            TableRow tr = new TableRow();
            table.Rows.Add(tr);
            TableCell td = null;
            if (res != OpRes.opres_success)
            {
                td = new TableCell();
                tr.Cells.Add(td);
                td.Text = OpResMgr.getInstance().getResultString(res);
                return;
            }

            List<MemberInfo> qresult = (List<MemberInfo>)user.getQueryResult(QueryType.queryTypePlayerMember);
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
                MemberInfo item = qresult[i];
                tr = new TableRow();
                if ((i & 1) == 0)
                {
                    tr.CssClass = "alt";
                }
                table.Rows.Add(tr);

                m_content[0] = item.m_acc;
                m_content[1] = item.m_createTime;
                m_content[2] = item.m_seller;
                m_content[3] = item.m_sellerAdmin;
                m_content[4] = item.m_money.ToString() + StrName.s_moneyType[item.m_moneyType];

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