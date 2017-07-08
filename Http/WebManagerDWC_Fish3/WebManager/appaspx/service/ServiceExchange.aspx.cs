using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.service
{
    public partial class ServiceExchange : System.Web.UI.Page
    {
        private static string[] s_head = new string[] { "选择", "玩家ID", "手机号", "道具名称", "领取时间", "成功时间", "兑换状态" };
        private string[] m_content = new string[s_head.Length];
        // 所选择的checkbox
        private string m_selectStr = "";
        private PageGift m_gen = new PageGift(50);

        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck(RightDef.SVR_EXCHANGE_MGR, Session, Response);
            if (IsPostBack)
            {
                m_selectStr = Request["aaa"];
                if (m_selectStr == null)
                {
                    m_selectStr = "";
                }
                return;
            }
            else
            {
                m_filter.Items.Add("已兑现");
                m_filter.Items.Add("未兑现");

                if (m_gen.parse(Request))
                {
                    m_filter.SelectedIndex = m_gen.m_state;
                    m_param.Text = m_gen.m_playerId;
                    onSearch(null, null);
                }
            }
        }

        protected void onActivateGift(object sender, EventArgs e)
        {
            if (m_selectStr != "")
            {
                GMUser user = (GMUser)Session["user"];
                DyOpMgr mgr = user.getSys<DyOpMgr>(SysType.sysTypeDyOp);
                OpRes res = mgr.doDyop(m_selectStr, DyOpType.opTypeExchange, user);
                onSearch(null, null);
            }
        }

        protected void onSearch(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];
            ParamQueryGift param = new ParamQueryGift();
            param.m_curPage = m_gen.curPage;
            param.m_countEachPage = m_gen.rowEachPage;
            param.m_state = m_filter.SelectedIndex;
            param.m_param = m_param.Text;

            QueryMgr mgr = user.getSys<QueryMgr>(SysType.sysTypeQuery);
            OpRes res = mgr.doQuery(param, QueryType.queryTypeExchange, user);

            genGiftTable(GiftTable, res, param, user, mgr);

            if (param.m_state == (int)ExState.wait)
            {
                m_btnActive.Visible = true;
            }
            else
            {
                m_btnActive.Visible = false;
            }
        }

        private void genGiftTable(Table table, OpRes res, ParamQueryGift param, GMUser user, QueryMgr mgr)
        {
            m_page.InnerHtml = "";
            m_foot.InnerHtml = "";

            table.GridLines = GridLines.Both;
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

            List<ExchangeItem> qresult = (List<ExchangeItem>)mgr.getQueryResult(QueryType.queryTypeExchange);

            int i = 0, j = 0;
            if (param.m_state != 1)
            {
                i++;
            }
            // 表头
            for (; i < s_head.Length; i++)
            {
                td = new TableCell();
                tr.Cells.Add(td);
                td.Text = s_head[i];
            }

            for (i = 0; i < qresult.Count; i++)
            {
                tr = new TableRow();
                if ((i & 1) == 0)
                {
                    tr.CssClass = "alt";
                }
                table.Rows.Add(tr);

                if (param.m_state == 1)
                {
                    m_content[0] = Tool.getCheckBoxHtml("aaa", qresult[i].m_exchangeId, false);
                }
                m_content[1] = qresult[i].m_playerId.ToString();
                m_content[2] = qresult[i].m_phone;
                m_content[3] = qresult[i].m_itemName;
                m_content[4] = qresult[i].m_exchangeTime;
                m_content[5] = qresult[i].m_giveOutTime;
                m_content[6] = qresult[i].getStateName();

                if (param.m_state != (int)ExState.wait)
                {
                    j = 1;
                }
                else
                {
                    j = 0;
                }
                for (; j < s_head.Length; j++)
                {
                    td = new TableCell();
                    tr.Cells.Add(td);
                    td.Text = m_content[j];
                }
            }

            string page_html = "", foot_html = "";
            m_gen.genPage(param, @"/appaspx/service/ServiceExchange.aspx", ref page_html, ref foot_html, user);
            m_page.InnerHtml = page_html;
            m_foot.InnerHtml = foot_html;
        }
    }
}