using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.stat._5dragons
{
    public partial class DragonViewGameModeEarning : System.Web.UI.Page
    {
        private static string[] s_head = new string[] { "游戏模式", "系统总收入", "系统总支出", "盈亏情况", "实际盈利率" };
        private string[] m_content = new string[s_head.Length];

        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck("stat", Session, Response);

            if (!IsPostBack)
            {
                for (int i = 0; i < StrName.s_dragonRoomName.Length; i++)
                {
                    m_room.Items.Add(new ListItem(StrName.s_dragonRoomName[i], (i + 1).ToString()));
                }
            }
        }

        // 查看具体模式下的盈利
        protected void onViewGameMode(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];
            ParamDragonGameModeEarning param = new ParamDragonGameModeEarning();
            param.m_roomId = Convert.ToInt32(m_room.SelectedValue);
            param.m_tableId = m_desk.Text;
            OpRes res = user.doQuery(param, QueryType.queryTypeDragonGameModeEarning);
            genTable(m_gameMode, res, user);
        }

        protected void genTable(Table table, OpRes res, GMUser user)
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

            int i = 0;
            for (; i < s_head.Length; i++)
            {
                td = new TableCell();
                tr.Cells.Add(td);
                td.Text = s_head[i];
            }

            ResultDragonGameModeEarning qresult
                = (ResultDragonGameModeEarning)user.getQueryResult(QueryType.queryTypeDragonGameModeEarning);

            for (i = 0; i < 3; i++)
            {
                ResultExpRateParam item = qresult.getParam(i);
                if (item == null)
                    continue;

                m_content[0] = ResultDragonGameModeEarning.s_mode[i];
                m_content[1] = item.m_totalIncome.ToString();
                m_content[2] = item.m_totalOutlay.ToString();
                m_content[3] = item.getDelta().ToString();
                m_content[4] = item.getFactExpRate().ToString();

                tr = new TableRow();
                table.Rows.Add(tr);
                for (int j = 0; j < s_head.Length; j++)
                {
                    td = new TableCell();
                    tr.Cells.Add(td);
                    td.Text = m_content[j];
                }
            }
        }
    }
}