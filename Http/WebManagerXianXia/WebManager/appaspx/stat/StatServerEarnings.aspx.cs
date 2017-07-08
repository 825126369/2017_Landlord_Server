using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.stat
{
    public partial class StatServerEarnings : System.Web.UI.Page
    {
        private static string[] s_head = new string[] { "日期", "系统总收入", "系统总支出", "盈利值", "盈利率" };
      //  private static string[] s_head1 = new string[] { "日期", "初级场收入", "初级场产出", "盈利值", "盈利率" ,
      //          "中级场收入", "中级场产出", "盈利值", "盈利率" ,"高级场收入", "高级场产出", "盈利值", "盈利率" ,
      //          "VIP场收入", "VIP场产出", "盈利值", "盈利率", "{0}总收入", "{0}总支出", "总盈利值", "总盈利率",
      //  "初级房赠予"/*21*/, "中级房赠予"/*22*/}; 

        private static string[] s_head1 = new string[] { "日期", "初级场收入", "初级场产出", "盈利值", "盈利率" };

        // 牛牛显示数据用
        private static string[] s_head2 = new string[] { "日期", "总下注", "净盈利", "盈利率" };

        private string[] m_content = new string[s_head1.Length];

        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck("stat", Session, Response);

            if (!IsPostBack)
            {
                m_game.Items.Add(new ListItem("全部", "0"));
                for (int i = 0; i < StrName.s_gameList.Count; i++)
                {
                    GameInfo info = StrName.s_gameList[i];
                    m_game.Items.Add(new ListItem(info.m_gameName, info.m_gameId.ToString()));
                }
            }
        }

        protected void onStat(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];
            QueryMgr mgr = user.getSys<QueryMgr>(SysType.sysTypeQuery);
            ParamServerEarnings param = new ParamServerEarnings();
            param.m_gameId = Convert.ToInt32(m_game.SelectedValue);
            param.m_time = __time__.Text;
            OpRes res = mgr.doQuery(param, QueryType.queryTypeServerEarnings, user);
            m_result.Rows.Clear();
            if (param.m_gameId == 0)
            {
                genTable(m_result, res, user, mgr);
            }
            else
            {
               /* if (param.m_gameId == (int)GameId.cows)
                {
                    genDetailGameTableCows(m_result, res, user);
                }
                else*/
                {
                    genDetailGameTable(m_result, res, user, mgr, param.m_gameId);
                }
            }
        }

        private void genDetailGameTable(Table table, OpRes res, GMUser user, QueryMgr mgr, int gameId)
        {
            TableRow tr = new TableRow();
            table.Rows.Add(tr);
            TableCell td = null;
            if (res != OpRes.opres_success)
            {
                td = new TableCell();
                tr.Cells.Add(td);
                td.Text = OpResMgr.getInstance().getResultString(res);
                return;
            }

            List<EarningItem> qresult = (List<EarningItem>)mgr.getQueryResult(QueryType.queryTypeServerEarnings);
            int i = 0, j = 0;
            // 表头
            for (i = 0; i < getHead1Length(gameId); i++)
            {
                td = new TableCell();
                tr.Cells.Add(td);

                if (i == 17 || i == 18)
                {
                    td.Text = string.Format(s_head1[i], StrName.s_gameName1[gameId]);
                }
                else
                {
                    td.Text = s_head1[i];
                }
            }

            EarningItem statItem = new EarningItem();

            for (i = 0; i < qresult.Count; i++)
            {
                tr = new TableRow();
                tr.Cells.Clear();
                if ((i & 1) == 0)
                {
                    tr.CssClass = "alt";
                }
                m_result.Rows.Add(tr);

                m_content[0] = qresult[i].m_time;

                m_content[1] = ItemHelp.showMoneyValue(qresult[i].getRoomIncome(0)).ToString();
                m_content[2] = ItemHelp.showMoneyValue(qresult[i].getRoomOutlay(0)).ToString();
                m_content[3] = ItemHelp.showMoneyValue(qresult[i].getDelta(0)).ToString();
                m_content[4] = qresult[i].getFactExpRate(0);
                statItem.addRoomIncome(0, qresult[i].getRoomIncome(0));
                statItem.addRoomOutlay(0, qresult[i].getRoomOutlay(0));

               /* m_content[5] = ItemHelp.showMoneyValue(qresult[i].getRoomIncome(1)).ToString();
                m_content[6] = ItemHelp.showMoneyValue(qresult[i].getRoomOutlay(1)).ToString();
                m_content[7] = ItemHelp.showMoneyValue(qresult[i].getDelta(1)).ToString();
                m_content[8] = qresult[i].getFactExpRate(1);
                statItem.addRoomIncome(1, qresult[i].getRoomIncome(1));
                statItem.addRoomOutlay(1, qresult[i].getRoomOutlay(1));

                m_content[9] = ItemHelp.showMoneyValue(qresult[i].getRoomIncome(2)).ToString();
                m_content[10] = ItemHelp.showMoneyValue(qresult[i].getRoomOutlay(2)).ToString();
                m_content[11] = ItemHelp.showMoneyValue(qresult[i].getDelta(2)).ToString();
                m_content[12] = qresult[i].getFactExpRate(2);
                statItem.addRoomIncome(2, qresult[i].getRoomIncome(2));
                statItem.addRoomOutlay(2, qresult[i].getRoomOutlay(2));

                m_content[13] = ItemHelp.showMoneyValue(qresult[i].getRoomIncome(3)).ToString();
                m_content[14] = ItemHelp.showMoneyValue(qresult[i].getRoomOutlay(3)).ToString();
                m_content[15] = ItemHelp.showMoneyValue(qresult[i].getDelta(3)).ToString();
                m_content[16] = qresult[i].getFactExpRate(3);
                statItem.addRoomIncome(3, qresult[i].getRoomIncome(3));
                statItem.addRoomOutlay(3, qresult[i].getRoomOutlay(3));

                m_content[17] = ItemHelp.showMoneyValue(qresult[i].getRoomIncome(4)).ToString();
                m_content[18] = ItemHelp.showMoneyValue(qresult[i].getRoomOutlay(4)).ToString();
                m_content[19] = ItemHelp.showMoneyValue(qresult[i].getDelta(4)).ToString();
                m_content[20] = qresult[i].getFactExpRate(4);
                statItem.addRoomIncome(4, qresult[i].getRoomIncome(4));
                statItem.addRoomOutlay(4, qresult[i].getRoomOutlay(4));

                if (gameId == (int)GameId.fishlord)
                {
                    m_content[21] = ItemHelp.showMoneyValue(qresult[i].getRoomOutlay(5)).ToString();
                    m_content[22] = ItemHelp.showMoneyValue(qresult[i].getRoomOutlay(6)).ToString();
                    statItem.addRoomOutlay(5, qresult[i].getRoomOutlay(5));
                    statItem.addRoomOutlay(6, qresult[i].getRoomOutlay(6));
                }*/

                for (j = 0; j < getHead1Length(gameId); j++)
                {
                    td = new TableCell();
                    tr.Cells.Add(td);
                    td.Style.Clear();
                    td.Text = m_content[j];
                }

                /*for (int k = 3; k < 20; k += 4)
                {
                    setRedColor(tr.Cells[k]);
                    setRedColor(tr.Cells[k + 1]);
                }*/
            }

            addDetailStatFoot(table, statItem, gameId);
        }

        private void genTable(Table table, OpRes res, GMUser user, QueryMgr mgr)
        {
            table.GridLines = GridLines.Both;
            TableRow tr = new TableRow();
            table.Rows.Add(tr);
            TableCell td = null;
            if (res != OpRes.opres_success)
            {
                td = new TableCell();
                tr.Cells.Add(td);
                td.Text = OpResMgr.getInstance().getResultString(res);
                return;
            }

            List<EarningItem> qresult = (List<EarningItem>)mgr.getQueryResult(QueryType.queryTypeServerEarnings);
            int i = 0, j = 0;
            // 表头
            for (i = 0; i < s_head.Length; i++)
            {
                td = new TableCell();
                tr.Cells.Add(td);
                td.Text = s_head[i];
            }

            EarningItem statItem = new EarningItem();

            for (i = 0; i < qresult.Count; i++)
            {
                tr = new TableRow();
                tr.Cells.Clear();
                if ((i & 1) == 0)
                {
                    tr.CssClass = "alt";
                }
                m_result.Rows.Add(tr);

                m_content[0] = qresult[i].m_time;
                m_content[1] = ItemHelp.showMoneyValue(qresult[i].getRoomIncome(4)).ToString();
                m_content[2] = ItemHelp.showMoneyValue(qresult[i].getRoomOutlay(4)).ToString();
                m_content[3] = ItemHelp.showMoneyValue(qresult[i].getDelta(4)).ToString();
                m_content[4] = qresult[i].getFactExpRate(4);

                statItem.addRoomIncome(4, qresult[i].getRoomIncome(4));
                statItem.addRoomOutlay(4, qresult[i].getRoomOutlay(4));

                for (j = 0; j < s_head.Length; j++)
                {
                    td = new TableCell();
                    tr.Cells.Add(td);
                    td.Text = m_content[j];
                    td.Style.Clear();
                    if (j == 3 || j == 4)
                    {
                        setRedColor(td);
                    }
                }
            }

            addStatFoot(table, statItem);
        }

        // 增加统计页脚
        protected void addStatFoot(Table table, EarningItem item)
        {
            TableRow tr = new TableRow();
            table.Rows.Add(tr);
            m_content[0] = "总计";
            // 总收入
            m_content[1] = ItemHelp.showMoneyValue(item.getRoomIncome(4)).ToString();
            // 总支出
            m_content[2] =ItemHelp.showMoneyValue(item.getRoomOutlay(4)).ToString();
            // 总盈亏
            m_content[3] = ItemHelp.showMoneyValue(item.getDelta(4)).ToString();
            m_content[4] = item.getFactExpRate(4);

            for (int j = 0; j < s_head.Length; j++)
            {
                TableCell td = new TableCell();
                tr.Cells.Add(td);
                td.Text = m_content[j];

                if (j == 3 || j == 4)
                {
                    setRedColor(td);
                }
            }
        }

        // 增加统计页脚
        protected void addDetailStatFoot(Table table, EarningItem item, int gameId)
        {
            TableRow tr = new TableRow();
            table.Rows.Add(tr);
            m_content[0] = "总计";

            m_content[1] = ItemHelp.showMoneyValue(item.getRoomIncome(0)).ToString();
            m_content[2] = ItemHelp.showMoneyValue(item.getRoomOutlay(0)).ToString();
            m_content[3] = ItemHelp.showMoneyValue(item.getDelta(0)).ToString();
            m_content[4] = item.getFactExpRate(0);

           /* m_content[5] = ItemHelp.showMoneyValue(item.getRoomIncome(1)).ToString();
            m_content[6] = ItemHelp.showMoneyValue(item.getRoomOutlay(1)).ToString();
            m_content[7] = ItemHelp.showMoneyValue(item.getDelta(1)).ToString();
            m_content[8] = item.getFactExpRate(1);

            m_content[9] = ItemHelp.showMoneyValue(item.getRoomIncome(2)).ToString();
            m_content[10] = ItemHelp.showMoneyValue(item.getRoomOutlay(2)).ToString();
            m_content[11] = ItemHelp.showMoneyValue(item.getDelta(2)).ToString();
            m_content[12] = item.getFactExpRate(2);

            m_content[13] = ItemHelp.showMoneyValue(item.getRoomIncome(3)).ToString();
            m_content[14] = ItemHelp.showMoneyValue(item.getRoomOutlay(3)).ToString();
            m_content[15] = ItemHelp.showMoneyValue(item.getDelta(3)).ToString();
            m_content[16] = item.getFactExpRate(3);

            m_content[17] = ItemHelp.showMoneyValue(item.getRoomIncome(4)).ToString();
            m_content[18] = ItemHelp.showMoneyValue(item.getRoomOutlay(4)).ToString();
            m_content[19] = ItemHelp.showMoneyValue(item.getDelta(4)).ToString();
            m_content[20] = item.getFactExpRate(4);

            if (gameId == (int)GameId.fishlord)
            {
                m_content[21] = ItemHelp.showMoneyValue(item.getRoomOutlay(5)).ToString();
                m_content[22] = ItemHelp.showMoneyValue(item.getRoomOutlay(6)).ToString();
            }*/

            for (int j = 0; j < getHead1Length(gameId); j++)
            {
                TableCell td = new TableCell();
                tr.Cells.Add(td);
                td.Style.Clear();
                td.Text = m_content[j];
            }

            /*for (int k = 3; k < 20; k += 4)
            {
                setRedColor(tr.Cells[k]);
                setRedColor(tr.Cells[k + 1]);
            }*/
        }

        protected void setRedColor(TableCell td)
        {
            td.Style.Clear();
            if (td.Text[0] == '-')
            {
                td.Style.Add("color", "red");
            }
        }

        protected int getHead1Length(int gameId)
        {
            return s_head1.Length;
            /*if (gameId == (int)GameId.fishlord)
                return s_head1.Length;
            return s_head1.Length - 2;*/
        }

        ///////////////////////////////////牛牛数据///////////////////////////////////////

        private void genDetailGameTableCows(Table table, OpRes res, GMUser user)
        {
            TableRow tr = new TableRow();
            table.Rows.Add(tr);
            TableCell td = null;
            if (res != OpRes.opres_success)
            {
                td = new TableCell();
                tr.Cells.Add(td);
                td.Text = OpResMgr.getInstance().getResultString(res);
                return;
            }

            List<EarningItem> qresult = (List<EarningItem>)user.getQueryResult(QueryType.queryTypeServerEarnings);
            int i = 0, j = 0;
            // 表头
            for (i = 0; i < s_head2.Length; i++)
            {
                td = new TableCell();
                tr.Cells.Add(td);
                td.Text = s_head2[i];
            }

            EarningItem statItem = new EarningItem();

            for (i = 0; i < qresult.Count; i++)
            {
                tr = new TableRow();
                tr.Cells.Clear();
                if ((i & 1) == 0)
                {
                    tr.CssClass = "alt";
                }
                m_result.Rows.Add(tr);

                m_content[0] = qresult[i].m_time;

                m_content[1] = qresult[i].getRoomIncome(0).ToString();   // 总下注
                m_content[2] = qresult[i].getRoomOutlay(0).ToString();   // 净盈利
                m_content[3] = ItemHelp.getRate(qresult[i].getRoomOutlay(0), qresult[i].getRoomIncome(0));

                statItem.addRoomIncome(0, qresult[i].getRoomIncome(0));
                statItem.addRoomOutlay(0, qresult[i].getRoomOutlay(0));

                for (j = 0; j < s_head2.Length; j++)
                {
                    td = new TableCell();
                    tr.Cells.Add(td);
                    td.Text = m_content[j];
                    td.Style.Clear();

                    if (j == 2 || j == 3)
                    {
                        setRedColor(td);
                    }
                }
            }

            addDetailStatFootCows(table, statItem);
        }

        // 增加牛牛统计页脚
        protected void addDetailStatFootCows(Table table, EarningItem item)
        {
            TableRow tr = new TableRow();
            table.Rows.Add(tr);
            m_content[0] = "总计";

            m_content[1] = item.getRoomIncome(0).ToString();
            m_content[2] = item.getRoomOutlay(0).ToString();
            m_content[3] = ItemHelp.getRate(item.getRoomOutlay(0), item.getRoomIncome(0));

            for (int j = 0; j < s_head2.Length; j++)
            {
                TableCell td = new TableCell();
                tr.Cells.Add(td);
                td.Text = m_content[j];

                if (j == 2 || j == 3)
                {
                    setRedColor(td);
                }
            }
        }
    }
}