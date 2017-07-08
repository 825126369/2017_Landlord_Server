using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.stat.gamedetail
{
    public partial class GameDetailBaccarat : System.Web.UI.Page
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
            GameDetailInfo ginfo = GameDetail.parseGameInfo(GameId.baccarat, index, user);
            genInfoPanel(ginfo);
        }

        private void genInfoPanel(GameDetailInfo ginfo)
        {
            if (ginfo == null)
                return;

            MoneyItem item = ginfo.m_item;
            InfoBaccarat info = (InfoBaccarat)ginfo.m_detailInfo;
            divHead.InnerText = item.m_genTime;
            // 玩家ID
            tdPlayer.InnerText = "玩家ID:" + item.m_playerId.ToString();
            // 玩家是否上庄
            tdIsBanker.InnerText = info.isBanker() ? DefCC.s_isBanker[0] : DefCC.s_isBanker[1];

            if (info.isBanker())
            {
                tdServiceChargeRatio.InnerText = info.m_serviceChargeRatio + "%";
                tdServiceCharge.InnerText = ItemHelp.showMoneyValue(info.m_serviceCharge).ToString();
                tdTotalIncome.InnerText = ItemHelp.showMoneyValue(info.sumBet() - info.sumAward()).ToString();
            }
            else
            {
                trServiceCharge.Visible = false;
                trTotalIncome.Visible = false;
            }

            // 庄家牌型
            genCardInfo(divBankerCard, info.m_bankerCard);
            // 闲家牌型
            genCardInfo(divPlayerCard, info.m_playerCard);

            // 押注信息
            genBetInfo(info);
        }

        private void genCardInfo(System.Web.UI.HtmlControls.HtmlGenericControl div, List<CardInfo> cards)
        {
            foreach (var card in cards)
            {
                Image img = new Image();
                img.ImageUrl = "/data/image/poker/" + DefCC.s_poker[card.flower] + "_" + card.point + ".png";
                div.Controls.Add(img);
            }
        }

        private void genBetInfo(InfoBaccarat info)
        {
            BetInfo bet = info.getBetInfo(4); // 庄
            tdZhuangBet.InnerText = ItemHelp.showMoneyValue(bet.bet_count).ToString();
            tdZhuangWin.InnerText = ItemHelp.showMoneyValue(bet.award_count).ToString();

            bet = info.getBetInfo(1); // 闲
            tdXianBet.InnerText = ItemHelp.showMoneyValue(bet.bet_count).ToString();
            tdXianWin.InnerText = ItemHelp.showMoneyValue(bet.award_count).ToString();

            bet = info.getBetInfo(0); // 和
            tdHeBet.InnerText = ItemHelp.showMoneyValue(bet.bet_count).ToString();
            tdHeWin.InnerText = ItemHelp.showMoneyValue(bet.award_count).ToString();

            bet = info.getBetInfo(3); // 庄对
            tdZhuangDuiBet.InnerText = ItemHelp.showMoneyValue(bet.bet_count).ToString();
            tdZhuangDuiWin.InnerText = ItemHelp.showMoneyValue(bet.award_count).ToString();

            bet = info.getBetInfo(2); // 闲对
            tdXianDuiBet.InnerText = ItemHelp.showMoneyValue(bet.bet_count).ToString();
            tdXianDuiWin.InnerText = ItemHelp.showMoneyValue(bet.award_count).ToString();

            // 总和
            tdSumBet.InnerText = ItemHelp.showMoneyValue(info.sumBet()).ToString();
            tdSumWin.InnerText = ItemHelp.showMoneyValue(info.sumAward()).ToString();
        }
    }
}