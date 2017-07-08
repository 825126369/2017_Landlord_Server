using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.stat.gamedetail
{
    public partial class GameDetailCows : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck("", Session, Response);
            string indexStr = Request.QueryString["index"];
            if (string.IsNullOrEmpty(indexStr))
                return;

            int index = 0;
            if (!int.TryParse(indexStr, out index))
            {
                return;
            }

            GMUser user = (GMUser)Session["user"];
            GameDetailInfo ginfo = GameDetail.parseGameInfo(GameId.cows, index, user);
            genInfoPanel(ginfo);
        }

        private void genInfoPanel(GameDetailInfo ginfo)
        {
            if (ginfo == null)
                return;

            MoneyItem item = ginfo.m_item;
            InfoCows info = (InfoCows)ginfo.m_detailInfo;
            divHead.InnerText = item.m_genTime;
            // 玩家ID
            tdPlayer.InnerText = "玩家ID:" + item.m_playerId.ToString();
            // 玩家是否上庄
            tdIsBanker.InnerText = info.isBanker() ? DefCC.s_isBanker[0] : DefCC.s_isBanker[1];
            if (info.isBanker())
            {
                tdServiceChargeRatio.InnerText = info.getServiceChargeRatio() + "%";
                tdServiceCharge.InnerText = info.m_betInfo.costgold.ToString();
            }
            else
            {
                trServiceCharge.Visible = false;
            }

            // 庄家牌型
            genCardInfo(divBankerCard, tdBankerCardType, info.m_bankerCard);
            // 东牌型
            genCardInfo(divEastCard, tdEastCardType, info.m_eastCard);
            // 南牌型
            genCardInfo(divSouthCard, tdSouthCardType, info.m_southCard);
            // 西牌型
            genCardInfo(divWestCard, tdWestCardType, info.m_westCard);
            // 北牌型
            genCardInfo(divNorthCard, tdNorthCardType, info.m_northCard);
            
            // 押注信息
            genBetInfo(info);
        }

        private void genCardInfo(System.Web.UI.HtmlControls.HtmlGenericControl div,
                                 System.Web.UI.HtmlControls.HtmlTableCell cell,
                                 CowsCard cards)
        {
            if (cards == null)
                return;

            Cows_CardsCFGData d = Cows_CardsCFG.getInstance().getValue(cards.m_cardType);
            if (d != null)
            {
                cell.InnerText = d.m_cardName;
            }
            foreach (var card in cards.m_cards)
            {
                Image img = new Image();
                img.ImageUrl = "/data/image/poker/" + DefCC.s_pokerCows[card.flower] + "_" + card.point + ".png";
                div.Controls.Add(img);
            }
        }

        private void genBetInfo(InfoCows info)
        {
            DbCowsBet bet = info.m_betInfo;
            tdEastBet.InnerText = bet.betgold0.ToString();
            tdEastWin.InnerText = bet.wingold0.ToString();

            tdSouthBet.InnerText = bet.betgold1.ToString();
            tdSouthWin.InnerText = bet.wingold1.ToString();

            tdWestBet.InnerText = bet.betgold2.ToString();
            tdWestWin.InnerText = bet.wingold2.ToString();

            tdNorthBet.InnerText = bet.betgold3.ToString();
            tdNorthWin.InnerText = bet.wingold3.ToString();

            tdSumBet.InnerText = bet.sumBet().ToString();
            tdSumWin.InnerText = bet.sumWin().ToString();
        }
    }
}