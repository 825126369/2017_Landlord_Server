using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.stat.cows
{
    public partial class CowsControl : System.Web.UI.Page
    {
        private static string[] s_head = new string[] { "房间", "期望盈利率", "系统总收入", "系统总支出", "盈亏情况",
            "盈利率", "手续费","爆庄", "总盈利率", "机器人收入", "机器人支出", "机器人盈亏情况", "机器人实际盈利率", "当前人数","选择" };
        private string[] m_content = new string[s_head.Length];
        private string m_roomList = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck(RightDef.COW_PARAM_CONTROL, Session, Response);

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
            p.m_rightId = RightDef.COW_PARAM_CONTROL;

            DyOpMgr mgr = user.getSys<DyOpMgr>(SysType.sysTypeDyOp);
            OpRes res = mgr.doDyop(p, DyOpType.opTypeCowsParamAdjust, user);
            m_res.InnerHtml = OpResMgr.getInstance().getResultString(res);
            genExpRateTable(m_expRateTable);
        }

        protected void onReset(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];

            ParamFishlordParamAdjust p = new ParamFishlordParamAdjust();
            p.m_isReset = true;
            p.m_roomList = m_roomList;
            p.m_rightId = RightDef.COW_PARAM_CONTROL;

            DyOpMgr mgr = user.getSys<DyOpMgr>(SysType.sysTypeDyOp);
            OpRes res = mgr.doDyop(p, DyOpType.opTypeCowsParamAdjust, user);
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

            OpRes res = user.doQuery(null, QueryType.queryTypeQueryCowsParam);
            Dictionary<int, ResultParamCows> qresult
                = (Dictionary<int, ResultParamCows>)user.getQueryResult(QueryType.queryTypeQueryCowsParam);

            for (i = 1; i <= 1; i++)
            {
                m_content[0] = StrName.s_roomName[i - 1];
                if (qresult.ContainsKey(i))
                {
                    ResultParamCows r = qresult[i];
                    m_content[1] = r.m_expRate.ToString();
                    m_content[2] = r.m_totalIncome.ToString();
                    m_content[3] = r.m_totalOutlay.ToString();
                    m_content[4] = r.getDelta().ToString();
                    m_content[5] = r.getFactExpRate();
                    m_content[6] = r.m_serviceCharge.ToString();
                    m_content[7] = r.m_burstZhuang.ToString();
                    m_content[8] = r.getTotalRate();

                    m_content[9] = r.m_robotIncome.ToString();
                    m_content[10] = r.m_robotOutlay.ToString();
                    m_content[11] = (r.m_robotIncome - r.m_robotOutlay).ToString();
                    m_content[12] = ItemHelp.getFactExpRate(r.m_robotIncome, r.m_robotOutlay);
                    m_content[13] = r.m_curPlayerCount.ToString();
                }
                else
                {
                    m_content[1] = "0.05";
                    for (int m = 2; m < 14; m++)
                    {
                        m_content[m] = "0";
                    }
                }
                m_content[14] = Tool.getCheckBoxHtml("roomList", i.ToString(), false);

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