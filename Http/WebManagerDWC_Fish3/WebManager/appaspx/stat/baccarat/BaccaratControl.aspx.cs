using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.stat.baccarat
{
    // 百家乐赢利率
    public partial class BaccaratControl : System.Web.UI.Page
    {
        private static string[] s_head = new string[] { "房间", "手续费", "系统总收入", "系统总支出", "盈亏情况", "实际盈利率",
            "机器人收入", "机器人支出", "机器人盈亏情况", "机器人实际盈利率", "选择" };
        private string[] m_content = new string[s_head.Length];
        private string m_roomList = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck(RightDef.BACC_PARAM_CONTROL, Session, Response);

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

        protected void onReset(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];

            ParamFishlordParamAdjust p = new ParamFishlordParamAdjust();
            p.m_isReset = true;
            p.m_roomList = m_roomList;

            DyOpMgr mgr = user.getSys<DyOpMgr>(SysType.sysTypeDyOp);
            OpRes res = mgr.doDyop(p, DyOpType.opTypeBaccaratParamAdjust, user);
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

           // long totalIncome = 0;
           // long totalOutlay = 0;
           // long totalCharge = 0;

            QueryMgr mgr = user.getSys<QueryMgr>(SysType.sysTypeQuery);
            OpRes res = mgr.doQuery(null, QueryType.queryTypeBaccaratEarnings, user);
            Dictionary<int, ResultFishlordExpRate> qresult
                = (Dictionary<int, ResultFishlordExpRate>)mgr.getQueryResult(QueryType.queryTypeBaccaratEarnings);

            for (i = 1; i <= 1/*StrName.s_roomName.Length*/; i++)
            {
                m_content[0] = StrName.s_roomName[i - 1];
                if (qresult.ContainsKey(i))
                {
                    ResultFishlordExpRate r = qresult[i];
                    m_content[1] = r.m_playerCharge.ToString();
                    m_content[2] = r.m_totalIncome.ToString();
                    m_content[3] = r.m_totalOutlay.ToString();
                    m_content[4] = r.getDelta().ToString();
                    m_content[5] = r.getFactExpRate();

                    m_content[6] = r.m_robotIncome.ToString();
                    m_content[7] = r.m_robotOutlay.ToString();
                    m_content[8] = (r.m_robotIncome - r.m_robotOutlay).ToString();
                    m_content[9] = ItemHelp.getFactExpRate(r.m_robotIncome, r.m_robotOutlay);
                   // totalIncome += r.m_totalIncome;
                   // totalOutlay += r.m_totalOutlay;
                   // totalCharge += r.m_playerCharge;
                }
                else
                {
                    for (int idx = 1; i < 10; idx++)
                    {
                        m_content[idx] = "0";
                    }
                }
                m_content[10] = Tool.getCheckBoxHtml("roomList", i.ToString(), false);

                tr = new TableRow();
                table.Rows.Add(tr);
                for (int j = 0; j < s_head.Length; j++)
                {
                    TableCell td = new TableCell();
                    tr.Cells.Add(td);
                    td.Text = m_content[j];

                    if (j == 3)
                    {
                        setColor(td, m_content[j]);
                    }
                }
            }

          //  addStatFoot(table, totalIncome, totalOutlay, totalCharge);
        }

        // 增加统计页脚
        protected void addStatFoot(Table table, long totalIncome, long totalOutlay, long totalCharge)
        {
            TableRow tr = new TableRow();
            table.Rows.Add(tr);
            m_content[0] = "总计";
            m_content[1] = totalCharge.ToString();
            // 总收入
            m_content[2] = totalIncome.ToString();
            // 总支出
            m_content[3] = totalOutlay.ToString();
            // 总盈亏
            m_content[4] = (totalIncome - totalOutlay).ToString();
            m_content[5] = "";
            m_content[6] = "";

            for (int j = 0; j < s_head.Length; j++)
            {
                TableCell td = new TableCell();
                tr.Cells.Add(td);
                td.Text = m_content[j];

                if (j == 3)
                {
                    setColor(td, m_content[j]);
                }
            }
        }

        protected void setColor(TableCell td, string num)
        {
            td.Style.Clear();
            if (num[0] == '-')
            {
                td.Style.Add("color", "red");
            }
            else
            {
                td.Style.Add("color", "green");
            }
        }
    }
}