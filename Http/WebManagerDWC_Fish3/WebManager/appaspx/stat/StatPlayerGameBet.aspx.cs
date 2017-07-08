using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.stat
{
    public partial class StatPlayerGameBet : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck("", Session, Response);
            if (!IsPostBack)
            {
                for (int i = 2; i < StrName.s_onlineGameIdList.Length; i++)
                {
                    int gameId = StrName.s_onlineGameIdList[i];
                    m_gameList.Items.Add(new ListItem(StrName.s_gameName[gameId], gameId.ToString()));
                }
            }
        }
    }
}