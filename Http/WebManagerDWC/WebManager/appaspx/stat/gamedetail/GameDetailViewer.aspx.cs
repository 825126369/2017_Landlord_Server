using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.stat.gamedetail
{
    public partial class GameDetailViewer : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck("", Session, Response);
            string gameIdStr = Request.QueryString["gameId"];
            string indexStr = Request.QueryString["index"];
            if (string.IsNullOrEmpty(gameIdStr) || string.IsNullOrEmpty(indexStr))
                return;

            int gameId = Convert.ToInt32(gameIdStr);
            switch (gameId)
            {
                case (int)GameId.baccarat:
                    {
                        Server.Transfer(string.Format(DefCC.ASPX_GAME_DETAIL_BACCARAT, indexStr));
                    }
                    break;
                case (int)GameId.cows:
                    {
                        Server.Transfer(string.Format(DefCC.ASPX_GAME_DETAIL_COWS, indexStr));
                    }
                    break;
                case (int)GameId.crocodile:
                    {
                        Server.Transfer(string.Format(DefCC.ASPX_GAME_DETAIL_CROCODILE, indexStr));
                    }
                    break;
                case (int)GameId.dice:
                    {
                        Server.Transfer(string.Format(DefCC.ASPX_GAME_DETAIL_DICE, indexStr));
                    }
                    break;
                case (int)GameId.fishpark:
                case (int)GameId.fishlord:
                    {
                        Server.Transfer(string.Format(DefCC.ASPX_GAME_DETAIL_FISH_PARK, indexStr));
                    }
                    break;
                case (int)GameId.shcd:
                    {
                        Server.Transfer(string.Format(DefCC.ASPX_GAME_DETAIL_SHCD, indexStr));
                    }
                    break;
            }
        }
    }
}