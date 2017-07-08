using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.stat.gamedetail
{
    public partial class GameDetailShcd : System.Web.UI.Page
    {
        private static string[] s_head = new string[] { "押注区域", "赔率", "押注", "得奖" };
        private string[] m_content = new string[s_head.Length];

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
            GameDetailInfo ginfo = GameDetail.parseGameInfo(GameId.shcd, index, user);
            genInfoPanel(ginfo);
        }

        private void genInfoPanel(GameDetailInfo ginfo)
        {
            if (ginfo == null)
                return;

            MoneyItem item = ginfo.m_item;
            InfoShcd info = (InfoShcd)ginfo.m_detailInfo;
            divHead.InnerText = item.m_genTime;
            // 玩家ID
            tdPlayer.InnerText = "玩家ID:" + item.m_playerId.ToString();
            genCardInfo(divNormalResult, info);
            genBetTable(tableBet, info);
        }

        private void genCardInfo(System.Web.UI.HtmlControls.HtmlGenericControl div, InfoShcd info)
        {
            Image img = new Image();
            if (info.card_type == 4)
            {
                img.ImageUrl = "/data/image/poker/" + DefCC.s_pokerShcd[info.card_type] + ".png";
                img.Width = 73; img.Height = 94;
            }
            else
            {
                img.ImageUrl = "/data/image/poker/" + DefCC.s_pokerShcd[info.card_type] + "_" + info.card_value + ".png";
            }
            
            div.Controls.Add(img);
        }

        // 下注表
        protected void genBetTable(Table table, InfoShcd info)
        {
            GMUser user = (GMUser)Session["user"];

            TableRow tr = new TableRow();
            table.Rows.Add(tr);

            int i = 0;
            for (; i < s_head.Length; i++)
            {
                TableCell td = new TableCell();
                tr.Cells.Add(td);
                td.Text = s_head[i];
            }

            long totalBet = 0, totalWin = 0;

            for (i = 0; i < StrName.s_shcdArea.Length; i++)
            {
                BetInfoCrocodile bet = info.getAreaBet(i);
                if (bet == null)
                    continue;

                int c = 0;
                m_content[c++] = StrName.s_shcdArea[i];
                m_content[c++] = ((double)bet.rate / 100).ToString();
                m_content[c++] = bet.bet_count.ToString();
                m_content[c++] = bet.award_count.ToString();
                totalBet += bet.bet_count;
                totalWin += bet.award_count;

                tr = new TableRow();
                table.Rows.Add(tr);
                for (int j = 0; j < s_head.Length; j++)
                {
                    TableCell td = new TableCell();
                    tr.Cells.Add(td);
                    td.Text = m_content[j];
                }
            }

            addStatFoot(table, totalBet, totalWin);
        }

        // 增加统计页脚
        protected void addStatFoot(Table table, long totalBet, long totalWin)
        {
            TableRow tr = new TableRow();
            table.Rows.Add(tr);
            m_content[0] = "总计";
            m_content[1] = "";
            m_content[2] = totalBet.ToString();
            m_content[3] = totalWin.ToString();

            for (int j = 0; j < s_head.Length; j++)
            {
                TableCell td = new TableCell();
                tr.Cells.Add(td);
                td.Text = m_content[j];
            }
        }
    }
}