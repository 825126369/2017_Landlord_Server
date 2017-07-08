using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.stat.gamedetail
{
    public partial class GameDetailFishPark : System.Web.UI.Page
    {
        private static string[] s_head = new string[] { "鱼ID", "名称", "击中次数", "死亡次数", "总投入", "总返还" };
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
            GameDetailInfo ginfo = GameDetail.parseGameInfo(GameId.fishpark, index, user);
            genInfoPanel(ginfo);
        }

        private void genInfoPanel(GameDetailInfo ginfo)
        {
            if (ginfo == null)
                return;

            MoneyItem item = ginfo.m_item;
            InfoFishPark info = (InfoFishPark)ginfo.m_detailInfo;
            divHead.InnerText = item.m_genTime;
            // 玩家ID
            tdPlayerCrocodile.InnerText = "玩家ID:" + item.m_playerId.ToString();
            tdAbandonedbullets.InnerText = "废弹:" + ItemHelp.showMoneyValue(info.m_abandonedbullets).ToString();

            genFishTable(tableFish, info);
        }

        protected void genFishTable(Table table, InfoFishPark info)
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

            for (i = 0; i < info.m_fish.Count; i++)
            {
                DbFish item = info.m_fish[i];
                m_content[0] = item.fishid.ToString();
                FishCFGData data = FishParkCFG.getInstance().getValue(item.fishid);
                if (data != null)
                {
                    m_content[1] = data.m_fishName;
                }
                
                m_content[2] = item.hitcount.ToString();
                m_content[3] = item.deadcount.ToString();
                m_content[4] = ItemHelp.showMoneyValue(item.totalincome).ToString();
                m_content[5] = ItemHelp.showMoneyValue(item.totaloutlay).ToString();

                tr = new TableRow();
                table.Rows.Add(tr);
                for (int j = 0; j < s_head.Length; j++)
                {
                    TableCell td = new TableCell();
                    tr.Cells.Add(td);
                    td.Text = m_content[j];
                }
            }
        }
    }
}