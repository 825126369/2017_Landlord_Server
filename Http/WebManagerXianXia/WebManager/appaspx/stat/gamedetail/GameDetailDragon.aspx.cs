using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.stat.gamedetail
{
    public partial class GameDetailDragon : System.Web.UI.Page
    {
        private static string[] s_head = new string[] { "中奖结果", "倍率", "得奖" };
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
            GameDetailInfo ginfo = GameDetail.parseGameInfo(GameId.dragon, index, user);
            genInfoPanel(ginfo);
        }

        private void genInfoPanel(GameDetailInfo ginfo)
        {
            if (ginfo == null)
                return;

            MoneyItem item = ginfo.m_item;
            InfoDragon info = (InfoDragon)ginfo.m_detailInfo;
            divHead.InnerText = item.m_genTime;
            // 玩家ID
            tdPlayer.InnerText = "玩家ID:" + item.m_playerId.ToString();
            tdIsFreeGame.InnerText = info.isFreeGame == 1 ? "是" : "否";
            tdBetMoney.InnerText = ItemHelp.showMoneyValue(item.m_outlay).ToString();
            genCardInfo(divResult1, info, 0);
            genCardInfo(divResult2, info, 1);
            genCardInfo(divResult3, info, 2);
            genBetTable(tableBet, info, item); ;
        }

        private void genCardInfo(System.Web.UI.HtmlControls.HtmlGenericControl div, InfoDragon info, int row)
        {
            for (int i = 0; i < 5; i++)
            {
                Image img = new Image();
                img.ImageUrl = "/data/image/dragon/" + "Dragons_item_id_" + info.getResult(row, i) + ".png";
                div.Controls.Add(img);
            }
        }

        // 下注表
        protected void genBetTable(Table table, InfoDragon info, MoneyItem item)
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

            for (i = 0; i < StrName.s_dragonArea.Length; i++)
            {
                int c = 0;
                m_content[c++] = StrName.s_dragonArea[i];
                m_content[c++] = info.getOdds(i).ToString();
                m_content[c++] = "";

                tr = new TableRow();
                table.Rows.Add(tr);
                for (int j = 0; j < s_head.Length; j++)
                {
                    TableCell td = new TableCell();
                    tr.Cells.Add(td);
                    td.Text = m_content[j];
                }
            }

            addStatFoot(table, item.m_income, info);
        }

        // 增加统计页脚
        protected void addStatFoot(Table table, long totalWin, InfoDragon info)
        {
            TableRow tr = new TableRow();
            table.Rows.Add(tr);
            m_content[0] = "总计";
            m_content[1] = info.getFinalOdds().ToString();
            m_content[2] = ItemHelp.showMoneyValue(totalWin).ToString();

            for (int j = 0; j < s_head.Length; j++)
            {
                TableCell td = new TableCell();
                tr.Cells.Add(td);
                td.Text = m_content[j];
            }
        }
    }
}