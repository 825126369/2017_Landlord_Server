using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.service
{
    public partial class ServiceAccountQuery : System.Web.UI.Page
    {
        private static string[] s_head =
            new string[] { "用户账号", "ID", "昵称", "平台", "角色创建时间", "VIP等级", "VIP经验", "上次登陆时间", "上次登陆IP", "金币", "保险箱内金币", "礼券", "绑定手机", "账号状态" };
        private string[] m_content = new string[s_head.Length];
        private PageGenDailyTask m_gen = new PageGenDailyTask(50);

        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck("service", Session, Response);
            if (!IsPostBack)
            {
                m_queryWay.Items.Add("通过玩家id查询");
                m_queryWay.Items.Add("通过账号查询");
                m_queryWay.Items.Add("通过昵称查询");
                m_queryWay.Items.Add("通过登陆IP查询");

                if (m_gen.parse(Request))
                {
                    m_param.Text = m_gen.m_param;
                    m_queryWay.SelectedIndex = m_gen.m_way;
                    onQueryAccount(null, null);
                }
            }
        }

        // 开始查询
        protected void onQueryAccount(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];
            ParamQuery param = new ParamQuery();
            param.m_way = (QueryWay)m_queryWay.SelectedIndex;
            param.m_param = m_param.Text;
            param.m_curPage = m_gen.curPage;
            param.m_countEachPage = m_gen.rowEachPage;

            if (param.m_way == QueryWay.by_way0)
            {
                //BtnBlockAcc.Visible = true;
            }
            else
            {
                //BtnBlockAcc.Visible = false;
            }

            QueryMgr mgr = user.getSys<QueryMgr>(SysType.sysTypeQuery);
            OpRes res = mgr.doQuery(param, QueryType.queryTypeAccount, user);
            genTable(m_result, res, param, user, mgr);
        }

        protected void onBlockAcc(object sender, EventArgs e)
        {
           /* string acc = BtnBlockAcc.CommandArgument;
            ParamBlock p = new ParamBlock();
            p.m_isBlock = true;
            p.m_param = acc;
            GMUser user = (GMUser)Session["user"];
            DyOpMgr mgr = user.getSys<DyOpMgr>(SysType.sysTypeDyOp);
            OpRes res = mgr.doDyop(p, DyOpType.opTypeBlockAcc, user);
            m_res.InnerHtml = OpResMgr.getInstance().getResultString(res);*/
        }

        private void genTable(Table table, OpRes res, ParamQuery param, GMUser user, QueryMgr mgr)
        {
            m_page.InnerHtml = "";
            m_foot.InnerHtml = "";

            m_result.GridLines = GridLines.Both;
            TableRow tr = new TableRow();
            m_result.Rows.Add(tr);
            TableCell td = null;
            if (res != OpRes.opres_success)
            {
                td = new TableCell();
                tr.Cells.Add(td);
                td.Text = OpResMgr.getInstance().getResultString(res);
                return;
            }

            List<ResultQueryAccount> qresult = (List<ResultQueryAccount>)mgr.getQueryResult(QueryType.queryTypeAccount);
            int i = 0, j = 0;
            // 表头
            for (i = 0; i < s_head.Length; i++)
            {
                td = new TableCell();
                tr.Cells.Add(td);
                td.Text = s_head[i];
            }

            if (qresult.Count == 1)
            {
                //BtnBlockAcc.CommandArgument = qresult[0].m_account;
            }

            for (i = 0; i < qresult.Count; i++)
            {
                tr = new TableRow();
                m_result.Rows.Add(tr);

                m_content[0] = qresult[i].m_account;
                m_content[1] = qresult[i].m_id.ToString();
                m_content[2] = qresult[i].m_nickName;
                m_content[3] = qresult[i].m_platForm;
                m_content[4] = qresult[i].m_createTime;

                m_content[5] = qresult[i].m_vipLevel.ToString();
                m_content[6] = qresult[i].m_vipExp.ToString();
                m_content[7] = qresult[i].m_lastLoginTime;
                m_content[8] = qresult[i].m_lastLoginIP; // 上次登陆IP
                m_content[9] = qresult[i].m_gold.ToString();
                m_content[10] = qresult[i].m_safeBoxGold.ToString();

                m_content[11] = qresult[i].m_ticket.ToString();
                m_content[12] = qresult[i].m_bindMobile;
                m_content[13] = qresult[i].m_accountState;

                for (j = 0; j < s_head.Length; j++)
                {
                    td = new TableCell();
                    tr.Cells.Add(td);
                    td.Text = m_content[j];
                }
            }

            string page_html = "", foot_html = "";
            m_gen.genPage(param, @"/appaspx/service/ServiceAccountQuery.aspx", ref page_html, ref foot_html, user);
            m_page.InnerHtml = page_html;
            m_foot.InnerHtml = foot_html;
        }
    }
}
