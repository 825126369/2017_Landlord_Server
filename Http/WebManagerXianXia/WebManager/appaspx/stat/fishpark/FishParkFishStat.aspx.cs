using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.stat.fishpark
{
    public partial class FishParkFishStat : System.Web.UI.Page
    {
        TableStatFish m_common = new TableStatFish();

        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck("stat", Session, Response);
            if (!IsPostBack)
            {
                m_room.Items.Add("全部");
                for (int i = 0; i < StrName.s_roomName.Length; i++)
                {
                    m_room.Items.Add(StrName.s_roomName[i]);
                }
            }
        }

        protected void onQuery(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];
            m_common.onQuery(user, m_result, m_room.SelectedIndex, QueryType.queryTypeFishParkStat);
        }

        protected void onClearFishTable(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];
            m_common.onClearFishTable(user, TableName.PUMP_ALL_FISH_PARK, m_result);
        }
    }
}