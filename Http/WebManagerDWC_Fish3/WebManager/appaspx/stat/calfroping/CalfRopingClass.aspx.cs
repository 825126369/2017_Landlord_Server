using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.stat.calfroping
{
    public partial class CalfRopingClass : System.Web.UI.Page
    {
        private static string[] s_head = new string[] { "关卡ID", "牛名称", "命中次数" };
        private string[] m_content = new string[s_head.Length];

        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck(RightDef.CALF_CLASS_STAT, Session, Response);

            onStat(null, null);
        }

        protected void onStat(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];
            ParamGameCalfRoping param = new ParamGameCalfRoping();
            param.m_queryContent = ParamGameCalfRoping.QUERY_CALF;
            OpRes res = user.doQuery(param, QueryType.queryTypeGameCalfRoping);
            genTable(m_result, res, param, user);
        }

        private void genTable(Table table, OpRes res, ParamGameCalfRoping param, GMUser user)
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

            int i = 0, k = 0;

            for (i = 0; i < s_head.Length; i++)
            {
                td = new TableCell();
                tr.Cells.Add(td);
                td.Text = s_head[i];
            }

            List<ResultCalfRopingLogItem> qresult =
                (List<ResultCalfRopingLogItem>)user.getQueryResult(param, QueryType.queryTypeGameCalfRoping);

            for (i = 0; i < qresult.Count; i++)
            {
                tr = new TableRow();
                if ((i & 1) == 0)
                {
                    tr.CssClass = "alt";
                }
                m_result.Rows.Add(tr);
                ResultCalfRopingLogItem item = qresult[i];

                m_content[0] = item.m_passId.ToString();
                m_content[1] = item.getName();
                m_content[2] = item.m_hitCount.ToString();

                for (k = 0; k < s_head.Length; k++)
                {
                    td = new TableCell();
                    tr.Cells.Add(td);
                    td.Text = m_content[k];
                }
            }
        }
    }
}