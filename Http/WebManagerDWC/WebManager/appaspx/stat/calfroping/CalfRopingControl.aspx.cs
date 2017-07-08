using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.stat.calfroping
{
    public partial class CalfRopingControl : System.Web.UI.Page
    {
        private static string[] s_head = new string[] { "期望盈利率", "系统总收入", "系统总支出", "盈亏情况", "盈利率" };
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
            p.m_expRate = txtExpRate.Text;
            p.m_roomList = "1";
            p.m_gameId = GameId.calf_roping;

            OpRes res = user.doDyop(p, DyOpType.opTypeGameParamAdjust);
            m_res.InnerHtml = OpResMgr.getInstance().getResultString(res);
            genExpRateTable(m_expRateTable);
        }

        protected void onReset(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];

            ParamFishlordParamAdjust p = new ParamFishlordParamAdjust();
            p.m_isReset = true;
            p.m_roomList = "1";
            p.m_gameId = GameId.calf_roping;
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

            ParamGameCalfRoping param = new ParamGameCalfRoping();
            param.m_queryContent = ParamGameCalfRoping.QUERY_CONTROL_PARAM;

            OpRes res = user.doQuery(param, QueryType.queryTypeGameCalfRoping);
            List<ResultExpRateParam> qresult
                = (List<ResultExpRateParam>)user.getQueryResult(param, QueryType.queryTypeGameCalfRoping);

            for (i = 0; i < qresult.Count; i++)
            {
                ResultExpRateParam item = qresult[i];
                m_content[0] = item.m_expRate.ToString();
                m_content[1] = item.m_totalIncome.ToString();
                m_content[2] = item.m_totalOutlay.ToString();
                m_content[3] = item.getDelta().ToString();
                m_content[4] = item.getFactExpRate().ToString();

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