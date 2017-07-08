using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.stat.fishpark
{
    public partial class FishParkStage : System.Web.UI.Page
    {
        private PageGift m_gen = new PageGift(50);
        TableStatFishlordStage m_common =
            new TableStatFishlordStage(@"/appaspx/stat/fishpark/FishParkStage.aspx");

        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck("stat", Session, Response);
            if (!IsPostBack)
            {
                for (int i = 0; i < StrName.s_roomName.Length; i++)
                {
                    m_room.Items.Add(StrName.s_roomName[i]);
                }

                if (m_gen.parse(Request))
                {
                    m_time.Text = m_gen.m_playerId;
                    m_room.SelectedIndex = m_gen.m_state;
                    onQuery(null, null);
                }
            }
        }

        protected void onQuery(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];
            m_common.onQuery(user, m_time.Text, m_room.SelectedIndex, m_gen, m_result, QueryType.queryTypeFishParkStage);
            m_page.InnerHtml = m_common.getPage();
            m_foot.InnerHtml = m_common.getFoot();
        }
    }
}