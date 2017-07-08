using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.stat.shcd
{
    public partial class ShcdControl : System.Web.UI.Page
    {
        private static string[] s_head = new string[] { "房间", "期望盈利率", "系统总收入", "系统总支出", "盈亏情况", "盈利率", "当前难度控制", "大小王个数", "作弊起止局数", "当前人数", "选择" };
        private string[] m_content = new string[s_head.Length];
        private string m_roomList = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck(RightDef.SHCD_PARAM_CONTROL, Session, Response);

            if (!IsPostBack)
            {
                genExpRateTable(m_expRateTable);
            }
            else
            {
                m_roomList = Request["roomList"];
                if (m_roomList == null)
                {
                    m_roomList = "";
                }
            }
        }

        protected void onModifyExpRate(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];

            ParamFishlordParamAdjust p = new ParamFishlordParamAdjust();
            p.m_isReset = false;
            p.m_expRate = txtExpRate.Text;
            p.m_roomList = m_roomList;
            p.m_gameId = GameId.shcd;
            p.m_rightId = RightDef.SHCD_PARAM_CONTROL;

            OpRes res = user.doDyop(p, DyOpType.opTypeGameParamAdjust);
            m_res.InnerHtml = OpResMgr.getInstance().getResultString(res);
            genExpRateTable(m_expRateTable);
        }

        protected void onReset(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];

            ParamFishlordParamAdjust p = new ParamFishlordParamAdjust();
            p.m_isReset = true;
            p.m_roomList = m_roomList;
            p.m_gameId = GameId.shcd;
            p.m_rightId = RightDef.SHCD_PARAM_CONTROL;

            OpRes res = user.doDyop(p, DyOpType.opTypeGameParamAdjust);
            m_res.InnerHtml = OpResMgr.getInstance().getResultString(res);
            genExpRateTable(m_expRateTable);
        }

        // 期望盈利率表格
        protected void genExpRateTable(Table table)
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

            OpRes res = user.doQuery(null, QueryType.queryTypeShcdParam);
            List<ResultShcdParam> qresult
                = (List<ResultShcdParam>)user.getQueryResult(QueryType.queryTypeShcdParam);

            for (i = 0; i < qresult.Count; i++)
            {
                ResultShcdParam item = qresult[i];
                m_content[0] = StrName.s_shcdRoomName[item.m_roomId];
                m_content[1] = item.m_expRate.ToString();
                m_content[2] = item.m_totalIncome.ToString();
                m_content[3] = item.m_totalOutlay.ToString();
                m_content[4] = item.getDelta().ToString();
                m_content[5] = item.getFactExpRate().ToString();
                m_content[6] = item.getLevelName();
                m_content[7] = item.m_jokerCount.ToString();
                m_content[8] = item.m_cheatSE;
                m_content[9] = item.m_curPlayerCount.ToString();
                m_content[10] = Tool.getCheckBoxHtml("roomList", item.m_roomId.ToString(), false);

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