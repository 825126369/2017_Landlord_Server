using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.account
{
    // 玩家在线时，实时上下分订单
    public partial class AccountRealTimeOrder : System.Web.UI.Page
    {
        ViewRealTimeOrder m_view = new ViewRealTimeOrder();
        private PageGmScoreLog m_gen = new PageGmScoreLog(50);

        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck("", Session, Response);

            if (!IsPostBack)
            {
                if (m_gen.parse(Request))
                {
                    m_time.Text = m_gen.m_time;
                    m_opAcc.Text = m_gen.m_opAcc;
                    onQueryRecord(null, null);
                }
            }
        }

        protected void onQueryRecord(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];
            ParamRealTimeOrder param = new ParamRealTimeOrder();
            param.m_countEachPage = m_gen.rowEachPage;
            param.m_curPage = m_gen.curPage;
            param.m_time = m_time.Text;
            param.m_opAcc = m_opAcc.Text;
            param.m_dstAcc = m_dstAcc.Text;

            m_page.InnerHtml = "";
            m_foot.InnerHtml = "";

            OpRes res = user.doQuery(param, QueryType.queryTypeQueryRealTimeOrder);
            m_view.genTable(m_result, res, user);

            string page_html = "", foot_html = "";
            m_gen.genPage(param, @"/appaspx/account/AccountRealTimeOrder.aspx", ref page_html, ref foot_html, user);
            m_page.InnerHtml = page_html;
            m_foot.InnerHtml = foot_html;
        }
    }
}