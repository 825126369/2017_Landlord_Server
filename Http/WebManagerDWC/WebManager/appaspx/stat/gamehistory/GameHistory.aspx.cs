using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.stat.gamehistory
{
    public partial class GameHistory : System.Web.UI.Page
    {
        ViewGameHistoryBase m_view;
        private PageGameHistory m_gen = new PageGameHistory(50);

        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck("", Session, Response);
            if (!IsPostBack)
            {
                //for (int i = 0; i < StrName.s_gameList.Count; i++)
                {
                    //GameInfo info = StrName.s_gameList[i];
                    //if (info.m_gameId != (int)GameId.fishpark && info.m_gameId != (int)GameId.dragon)
                    {
                        m_whichGame.Items.Add(new ListItem("黑红梅方", ((int)GameId.shcd).ToString()));
                    }
                }

                if (m_gen.parse(Request))
                {
                    m_time.Text = m_gen.m_time;
                    m_whichGame.SelectedIndex = m_gen.m_gameIndex;

                    if (m_gen.m_bound > -1)
                    {
                        m_bound.Text = m_gen.m_bound.ToString();
                    }
                    
                    onQueryRecord(null, null);
                }
            }
        }

        protected void onQueryRecord(object sender, EventArgs e)
        {
            m_view = ViewGameHistoryBase.create(Convert.ToInt32(m_whichGame.SelectedValue));
            if (m_view == null)
                return;

            GMUser user = (GMUser)Session["user"];
            ParamGameHistory param = new ParamGameHistory();
            param.m_countEachPage = m_gen.rowEachPage;
            param.m_curPage = m_gen.curPage;
            param.m_gameId = Convert.ToInt32(m_whichGame.SelectedValue);
            param.m_time = m_time.Text;
            param.m_bound = m_bound.Text;

            m_page.InnerHtml = "";
            m_foot.InnerHtml = "";

            OpRes res = user.doQuery(param, QueryType.queryTypeGameHistory);
            m_view.genTable(m_result, res, user);

            param.m_gameId = m_whichGame.SelectedIndex;
            string page_html = "", foot_html = "";
            m_gen.genPage(param, @"/appaspx/stat/gamehistory/GameHistory.aspx", ref page_html, ref foot_html, user);
            m_page.InnerHtml = page_html;
            m_foot.InnerHtml = foot_html;
        }
    }
}
