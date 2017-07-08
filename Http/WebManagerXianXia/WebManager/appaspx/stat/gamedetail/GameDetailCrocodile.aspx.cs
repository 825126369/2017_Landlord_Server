using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.stat.gamedetail
{
    public partial class GameDetailCrocodile : System.Web.UI.Page
    {
        private static string[] s_head = new string[] { "押注区域", "倍率", "押注", "得奖" };
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
            GameDetailInfo ginfo = GameDetail.parseGameInfo(GameId.crocodile, index, user);
            genInfoPanel(ginfo);
        }

        private void genInfoPanel(GameDetailInfo ginfo)
        {
            if (ginfo == null)
                return;

            MoneyItem item = ginfo.m_item;
            InfoCrocodile info = (InfoCrocodile)ginfo.m_detailInfo;
            divHead.InnerText = item.m_genTime;
            // 玩家ID
            tdPlayerCrocodile.InnerText = "玩家ID:" + item.m_playerId.ToString();

            fillResultInfo(info);

            genBetTable(tableBet, info);
        }

        private void fillResultInfo(InfoCrocodile info)
        {
            if (info == null)
                return;

            e_award_type_def type = info.getResultType();
            switch (type)
            {
                case e_award_type_def.e_type_normal:
                    {
                        addResultImg(divNormalResult, info);
                    }
                    break;
                case e_award_type_def.e_type_all_prizes:
                    {
                        tdAllPrizesResult.InnerText = info.getResultParam();
                    }
                    break;
                case e_award_type_def.e_type_spot_light:
                    {
                        addResultImg(divSpotLightResult, info);
                    }
                    break;
                case e_award_type_def.e_type_handsel:
                    {
                        string resultId = info.getHandSelResultId();
                        if (string.IsNullOrEmpty(resultId))
                        {
                            tdHandSelResult.InnerText = info.getResultParam() + "倍";
                        }
                        else
                        {                            
                            Crocodile_RateCFGData data = Crocodile_RateCFG.getInstance().getValue(Convert.ToInt32(resultId) + 1);
                            addImg(tdHandSelResult.Controls, data);

                            Label L = new Label();
                            L.CssClass = "cHand";
                            L.Text = info.getResultParam() + "倍";
                            tdHandSelResult.Controls.Add(L);
                        }
                    }
                    break;
            }
        }

        private void addResultImg(System.Web.UI.HtmlControls.HtmlGenericControl div, InfoCrocodile info)
        {
            foreach (var res in info.m_resultList)
            {
                Crocodile_RateCFGData data = Crocodile_RateCFG.getInstance().getValue(Convert.ToInt32(res.result_id) + 1);
                if (data != null)
                {
                    addImg(div.Controls, data);
                }
            }
        }

        // 下注表
        protected void genBetTable(Table table, InfoCrocodile info)
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

            // 1-12个区域
            for (i = 1; i < 13; i++)
            {
                Crocodile_RateCFGData data = Crocodile_RateCFG.getInstance().getValue(i);

                BetInfoCrocodile item = info.getBetInfo(i);
                if (item != null)
                {
                    m_content[1] = item.rate.ToString();
                    m_content[2] = ItemHelp.showMoneyValue(item.bet_count).ToString();
                    totalBet += item.bet_count;

                    m_content[3] = ItemHelp.showMoneyValue(item.award_count).ToString();
                    totalWin += item.award_count;
                }
                else
                {
                    m_content[1] = m_content[2] = m_content[3] = "";
                }

                tr = new TableRow();
                table.Rows.Add(tr);
                for (int j = 0; j < s_head.Length; j++)
                {
                    TableCell td = new TableCell();
                    tr.Cells.Add(td);
                    if (j == 0)
                    {
                        addBetImg(td, data);
                    }
                    else
                    {
                        td.Text = m_content[j];
                    }
                }
            }

            addStatFoot(table, totalBet, totalWin);
        }

        private void addBetImg(TableCell td, Crocodile_RateCFGData data)
        {
            if (data != null)
            {
                if (string.IsNullOrEmpty(data.m_icon))
                {
                    td.Text = data.m_name;
                }
                else
                {
                    addImg(td.Controls, data, "cDivBg cDivBg1");
                }
            }
        }

        private void addImg(ControlCollection parent, Crocodile_RateCFGData data, string cssName = "")
        {
            if (data != null)
            {
                Panel p = new Panel();
                p.BackImageUrl = "/data/image/crocodile/" + "bg_" + data.m_color + ".png";
                if (string.IsNullOrEmpty(cssName))
                {
                    p.CssClass = "cDivBg";
                }
                else
                {
                    p.CssClass = cssName;
                }
                parent.Add(p);

                Image img = new Image();
                img.ImageUrl = "/data/image/crocodile/" + data.m_icon;
                p.Controls.Add(img);

                Panel tmp = new Panel();
                tmp.CssClass = "clr";
                parent.Add(tmp);
            }
        }

        // 增加统计页脚
        protected void addStatFoot(Table table, long totalBet, long totalWin)
        {
            TableRow tr = new TableRow();
            table.Rows.Add(tr);
            m_content[0] = "总计";
            m_content[1] = "";
            m_content[2] = ItemHelp.showMoneyValue(totalBet).ToString();
            m_content[3] = ItemHelp.showMoneyValue(totalWin).ToString();

            for (int j = 0; j < s_head.Length; j++)
            {
                TableCell td = new TableCell();
                tr.Cells.Add(td);
                td.Text = m_content[j];
            }
        }
    }
}