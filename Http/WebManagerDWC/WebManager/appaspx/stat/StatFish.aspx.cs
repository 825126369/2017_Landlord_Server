using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.stat
{
    public partial class StatFish : System.Web.UI.Page
    {
      //  private static string[] s_head = new string[] { "鱼ID", "名称", "击中次数", "死亡次数", "命中率", "支出", "收入", "盈利率", "房间" };
       // private string[] m_content = new string[s_head.Length];
        TableStatFish m_common = new TableStatFish();

        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck("stat", Session, Response);
            if (!IsPostBack)
            {
                m_room.Items.Add("全部");
                for (int i = 0; i < StrName.s_roomName.Length; i++)
                {
                    m_room.Items.Add(StrName.s_roomName[i]);
                }
            }
            //genTable(m_result);
        }

        protected void onQuery(object sender, EventArgs e)
        {
            //genTable(m_result);
            GMUser user = (GMUser)Session["user"];
            m_common.onQuery(user, m_result, m_room.SelectedIndex, QueryType.queryTypeFishStat);
        }

        protected void onClearFishTable(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];
            m_common.onClearFishTable(user, TableName.PUMP_ALL_FISH, m_result);
            /*DyOpMgr mgr = user.getSys<DyOpMgr>(SysType.sysTypeDyOp);
            OpRes res = mgr.doDyop(null, DyOpType.opTypeClearFishTable, user);
            m_result.Rows.Clear();*/
            //genTable(m_result);
        }

       /* protected void genTable(Table table)
        {
            GMUser user = (GMUser)Session["user"];

            table.GridLines = GridLines.Both;
            TableRow tr = new TableRow();
            table.Rows.Add(tr);

            int i = 0;
            for (; i < s_head.Length; i++)
            {
                TableCell td = new TableCell();
                tr.Cells.Add(td);
                td.Text = s_head[i];
            }

            QueryMgr mgr = user.getSys<QueryMgr>(SysType.sysTypeQuery);
            OpRes res = mgr.doQuery(m_room.SelectedIndex, QueryType.queryTypeFishStat, user);
            List<ResultFish> qresult
                = (List<ResultFish>)mgr.getQueryResult(QueryType.queryTypeFishStat);

            foreach(var data in qresult)
            {
                m_content[0] = data.m_fishId.ToString();
                var fishInfo = FishCFG.getInstance().getValue(data.m_fishId);
                if (fishInfo != null)
                {
                    m_content[1] = fishInfo.m_fishName;
                }
                else
                {
                    m_content[1] = "";
                }
                m_content[2] = data.m_hitCount.ToString();
                m_content[3] = data.m_dieCount.ToString();
                m_content[4] = data.getHit_Die();
                m_content[5] = data.m_outlay.ToString();
                m_content[6] = data.m_income.ToString();
                m_content[7] = data.getOutlay_Income();
                if (data.m_roomId > 0)
                {
                    m_content[8] = StrName.s_roomName[data.m_roomId - 1];
                }
                else
                {
                    m_content[8] = "";
                }

                tr = new TableRow();
                table.Rows.Add(tr);
                for (int j = 0; j < s_head.Length; j++)
                {
                    TableCell td = new TableCell();
                    tr.Cells.Add(td);
                    td.Text = m_content[j];
                }
            }
        }*/
    }
}