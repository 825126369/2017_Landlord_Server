using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.stat._5dragons
{
    public partial class DragonControl : System.Web.UI.Page
    {
        private static string[] s_head = new string[] { "房间", "期望盈利率", "系统总收入", "系统总支出", "盈亏情况", "实际盈利率", 
            "翻牌收入", "翻牌支出", "选择" };

        private string[] m_content = new string[s_head.Length];
        private string m_roomList = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck("stat", Session, Response);

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
            p.m_expRate = m_expRate.Text;
            p.m_roomList = m_roomList;

            OpRes res = user.doDyop(p, DyOpType.opTypeDragonParamAdjust);
            m_res.InnerHtml = OpResMgr.getInstance().getResultString(res);
            genExpRateTable(m_expRateTable);
        }

        protected void onReset(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];

            ParamFishlordParamAdjust p = new ParamFishlordParamAdjust();
            p.m_isReset = true;
            p.m_roomList = m_roomList;

            OpRes res = user.doDyop(p, DyOpType.opTypeDragonParamAdjust);
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

          //  long totalIncome = 0;
          //  long totalOutlay = 0;

            OpRes res = user.doQuery(null, QueryType.queryTypeDragonParam);
            List<ResultDragonParam> qresult
                = (List<ResultDragonParam>)user.getQueryResult(QueryType.queryTypeDragonParam);

            for (i = 0; i < qresult.Count; i++)
            {
                ResultDragonParam item = qresult[i];

                m_content[0] = StrName.s_dragonRoomName[item.m_roomId - 1];
                m_content[1] = item.m_expRate.ToString();
                m_content[2] = item.m_totalIncome.ToString();
                m_content[3] = item.m_totalOutlay.ToString();
                m_content[4] = item.getDelta().ToString();
                m_content[5] = item.getFactExpRate().ToString();
                m_content[6] = item.m_doubleIncome.ToString();
                m_content[7] = item.m_doubleOutcome.ToString();
                m_content[8] = Tool.getCheckBoxHtml("roomList", item.m_roomId.ToString(), false);

                tr = new TableRow();
                table.Rows.Add(tr);
                for (int j = 0; j < s_head.Length; j++)
                {
                    TableCell td = new TableCell();
                    tr.Cells.Add(td);
                    td.Text = m_content[j];

                   // if (j == 4)
                    //{
                    //    setColor(td, m_content[j]);
                   // }
                }
            }

           // addStatFoot(table, totalIncome, totalOutlay);
        }

        // 增加统计页脚
        protected void addStatFoot(Table table, long totalIncome, long totalOutlay)
        {
            TableRow tr = new TableRow();
            table.Rows.Add(tr);
            m_content[0] = "总计";
            m_content[1] = "";
            // 总收入
            m_content[2] = totalIncome.ToString();
            // 总支出
            m_content[3] = totalOutlay.ToString();
            // 总盈亏
            m_content[4] = (totalIncome - totalOutlay).ToString();
            m_content[5] = "";
            m_content[6] = "";
            m_content[7] = "";

            for (int j = 0; j < s_head.Length; j++)
            {
                TableCell td = new TableCell();
                tr.Cells.Add(td);
                td.Text = m_content[j];

                if (j == 4)
                {
                    setColor(td, m_content[j]);
                }
            }
        }

        protected void setColor(TableCell td, string num)
        {
            if (num[0] == '-')
            {
              //  td.ForeColor = Color.Red;
            }
            else
            {
               // td.ForeColor = Color.Green;
            }
        }
    }
}