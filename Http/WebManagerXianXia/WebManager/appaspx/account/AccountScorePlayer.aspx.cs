using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.account
{
    // 给玩家上分下分
    public partial class AccountScorePlayer : RefreshPageBase
    {
        ViewPlayerScoreInfo m_view;
        private PageGen m_gen = new PageGen(50);
        string m_creator; // 玩家的创建者，为空表示创建者是当前操作账号

        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck(RIGHT.SCORE, Session, Response);
            if (!IsPostBack)
            {
                if (m_gen.parse(Request))
                {
                }
            }

            GMUser user = (GMUser)Session["user"];
            m_creator = Request.QueryString["acc"];
            if (string.IsNullOrEmpty(m_creator))
            {
                if (user.m_accType == AccType.ACC_SUPER_ADMIN ||
                user.m_accType == AccType.ACC_GENERAL_AGENCY ||
                user.m_accType == AccType.ACC_AGENCY_SUB)
                {
                    Server.Transfer(DefCC.ASPX_EMPTY);
                }
            }

            onQueryMember(user);
        }

        protected void onQueryMember(GMUser user)
        {
            ParamMemberInfo param = new ParamMemberInfo();
            if (!string.IsNullOrEmpty(m_creator))
            {
                param.m_creator = m_creator;
            }
           
            param.m_searchDepth = 1;
            param.m_curPage = m_gen.curPage;
            param.m_countEachPage = m_gen.rowEachPage;

            m_view = new ViewPlayerScoreInfo(IsRefreshed);
            OpRes res = user.doQuery(param, QueryType.queryTypePlayerMember);
            m_view.genTable(m_result, res, user, this, param);

            if (!string.IsNullOrEmpty(m_creator))
            {
                m_levelStr.InnerHtml = user.getOpLevelMgr().getCurLevelStr(m_creator);
            }
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