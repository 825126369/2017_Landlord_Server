using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.stat.gamedetail
{
    public partial class GameDetailDice : System.Web.UI.Page
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
            GameDetailInfo ginfo = GameDetail.parseGameInfo(GameId.dice, index, user);
            genInfoPanel(ginfo);
        }

        private void genInfoPanel(GameDetailInfo ginfo)
        {
            if (ginfo == null)
                return;

            MoneyItem item = ginfo.m_item;
            InfoDice info = (InfoDice)ginfo.m_detailInfo;
            divHead.InnerText = item.m_genTime;
            // 玩家ID
            tdPlayerCrocodile.InnerText = "玩家ID:" + item.m_playerId.ToString();
            // 骰宝结果
            tdDiceDesc.InnerText = DefCC.s_diceStr[info.getResult()];

            addImg(tdDiceResult.Controls, info);

            genBetTable(tableBet, info);
        }

        // 增加背景图
        private void addImg(ControlCollection parent, InfoDice info)
        {
            foreach(var num in info.m_diceNum)
            {
                Image img = new Image();
                img.CssClass = "cDiceImg";
                img.ImageUrl = "/data/image/dice/Dice_num_" + num + ".png";
                parent.Add(img);
            }
        }

        // 下注表
        protected void genBetTable(Table table, InfoDice info)
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

            bool spanProcess = false;
            int lastSpanCount = 0;
            int k = 1;
            int span = 0;
            long totalBet = 0, totalWin = 0;

            for (i = 1; i < 30; i++)
            {
                Dice_BetAreaCFGData d = Dice_BetAreaCFG.getInstance().getValue(i);
                if (d != null)
                {
                    m_content[0] = d.m_name;
                    m_content[1] = d.m_desc;
                    span = d.m_span;
                }
                else
                {
                    m_content[0] = "";
                    m_content[1] = "";
                    span = 0;
                }

                DbDiceBet item = info.getDiceBet(i);
                if (item != null)
                {
                    m_content[2] = item.bet_gold.ToString();
                    m_content[3] = item.win_gold.ToString();
                    totalBet += item.bet_gold;
                    totalWin += item.win_gold;
                }
                else
                {
                    m_content[2] = m_content[3] = "";
                }

                tr = new TableRow();
                table.Rows.Add(tr);
                for (int j = 0; j < s_head.Length; j++)
                {
                    if (spanProcess && j == 1)
                    {
                        k++;
                        if (k >= lastSpanCount)
                        {
                            spanProcess = false;
                        }
                        continue;
                    }

                    TableCell td = new TableCell();
                    tr.Cells.Add(td);
                    td.Text = m_content[j];

                    if (span > 0 && j == 1)
                    {
                        td.RowSpan = span;
                        spanProcess = true;
                        lastSpanCount = span;
                        k = 1;
                    }
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