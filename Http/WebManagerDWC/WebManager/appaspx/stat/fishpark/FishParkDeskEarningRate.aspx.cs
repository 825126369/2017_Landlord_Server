using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.stat.fishpark
{
    public partial class FishParkDeskEarningRate : System.Web.UI.Page
    {
        private PageGift m_gen = new PageGift(50);
        TableStatFishlordDeskEarningsRate m_common =
            new TableStatFishlordDeskEarningsRate(@"/appaspx/stat/fishpark/FishParkDeskEarningRate.aspx");

        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck("stat", Session, Response);

            if (!IsPostBack)
            {
                for (int i = 0; i < StrName.s_roomName.Length; i++)
                {
                    ListItem item = new ListItem(StrName.s_roomName[i], (i + 1).ToString());
                    m_room.Items.Add(item);
                }

                if (m_gen.parse(Request))
                {
                    m_room.SelectedIndex = m_gen.m_state;
                    btnQuery_Click(null, null);
                }
            }
        }

        protected void btnQuery_Click(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];

            m_common.onQueryDesk(user, m_gen, Convert.ToInt32(m_room.SelectedValue),
                m_result, QueryType.queryTypeFishParkDeskParam);

            m_page.InnerHtml = m_common.getPage();
            m_foot.InnerHtml = m_common.getFoot();
        }

    }
}