using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.account
{
    // 搜索具体的玩家
    public partial class AccountSearchPlayer : System.Web.UI.Page
    {
        ViewPlayerInfo m_view = new ViewPlayerInfo();
        private PageGen m_gen = new PageGen(50);

        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck(RIGHT.VIEW_AGENCY, Session, Response);
            if (!IsPostBack)
            {
                m_searchCond.setLevelName(true);
                m_gen.parse(Request);
                onQueryMember(null, null);
            }
        }

        protected void onQueryMember(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];
            ParamMemberInfo param = new ParamMemberInfo();
            m_searchCond.fillCondtion(param, user);
            param.m_curPage = m_gen.curPage;
            param.m_countEachPage = m_gen.rowEachPage;

            OpRes res = user.doQuery(param, QueryType.queryTypePlayerMember);
            m_view.genTable(m_result, res, user, this, param);
        }

        public global::System.Web.UI.HtmlControls.HtmlGenericControl getPage()
        {
            return m_page;
        }

        public global::System.Web.UI.HtmlControls.HtmlGenericControl getFoot()
        {
            return m_foot;
        }

        public PageGen getGen()
        {
            return m_gen;
        }
    }
}