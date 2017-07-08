using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.account.sub
{
    // 链接到某代理或API创建的会员
    public partial class AccountSubPlayer : System.Web.UI.Page
    {
        ViewPlayerInfo m_view = new ViewPlayerInfo();
        private PageAccountSubPlayer m_gen = new PageAccountSubPlayer(50);

        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck("", Session, Response);
            string acc = "";
            if (!IsPostBack)
            {
                if (m_gen.parse(Request))
                {
                    acc = m_gen.m_creator;
                }
                else
                {
                    acc = Request.QueryString["acc"];
                    if (string.IsNullOrEmpty(acc))
                    {
                        return;
                    }
                    m_gen.m_creator = acc;
                }
            }
            GMUser user = (GMUser)Session["user"];
            ParamMemberInfo param = new ParamMemberInfo();
            param.m_creator = acc;
            param.m_searchDepth = 1;
            param.m_curPage = m_gen.curPage;
            param.m_countEachPage = m_gen.rowEachPage;

            OpRes res = user.doQuery(param, QueryType.queryTypePlayerMember);
            m_view.genTable(m_result, res, user, this, param);

            m_levelStr.InnerHtml = user.getOpLevelMgr().getCurLevelStr(acc);
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