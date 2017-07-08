using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.account
{
    // 超级管理员搜索所创建的经销商
    public partial class AccountSearchBySuperAdmin : System.Web.UI.Page
    {
        private static string[] s_head = new string[] { "经销商", "注册日期", "所持货币" };
        private string[] m_content = new string[s_head.Length];

        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck("", Session, Response);
        }

        protected void onQueryMember(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];
            ParamGmAccount param = new ParamGmAccount();
            param.m_time = m_time.Text;

            OpRes res = user.doQuery(param, QueryType.queryTypeGmAccount);
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

            List<MemberInfo> qresult = (List<MemberInfo>)user.getQueryResult(QueryType.queryTypeGmAccount);
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
                m_content[2] = item.m_money.ToString() + StrName.s_moneyType[item.m_moneyType];
                
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