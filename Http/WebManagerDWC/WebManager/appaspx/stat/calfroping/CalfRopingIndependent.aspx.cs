using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.stat.calfroping
{
    public partial class CalfRopingIndependent : System.Web.UI.Page
    {
        private static string[] s_head = new string[] { "进入人次", "鼓励奖获得次数", "鼓励奖发放奖金", "大奖获得次数", "大奖发放奖金", "续关次数" };
        private string[] m_content = new string[s_head.Length];

        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck("stat", Session, Response);
        }

        protected void onStat(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];
            ParamGameCalfRoping param = new ParamGameCalfRoping();
            param.m_queryContent = ParamGameCalfRoping.QUERY_INDEPENDENT;
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

            ResultIndependentCalfRoping qresult = (ResultIndependentCalfRoping)user.getQueryResult(param, QueryType.queryTypeGameCalfRoping);

            int i = 0, k = 0;

            for (i = 0; i < s_head.Length; i++)
            {
                td = new TableCell();
                tr.Cells.Add(td);
                td.Text = s_head[i];
            }

            i = 0;
           // Dictionary<int, CrocodileInfo> dict = qresult.m_betInfo.OrderBy(o => o.Key).ToDictionary(o => o.Key, p => p.Value);
           // foreach (var info in dict)
            {
                tr = new TableRow();
                if ((i & 1) == 0)
                {
                    tr.CssClass = "alt";
                }
                m_result.Rows.Add(tr);
                ResultIndependentCalfRoping item = qresult;

                m_content[0] = item.m_enterCount.ToString();
                m_content[1] = item.m_norRewardNum.ToString();
                m_content[2] = item.m_norRewardGold.ToString();
                m_content[3] = item.m_bigRewardNum.ToString();
                m_content[4] = item.m_bigRewardGold.ToString();
                m_content[5] = item.m_buyLifeNum.ToString();

                for (k = 0; k < s_head.Length; k++)
                {
                    td = new TableCell();
                    tr.Cells.Add(td);
                    td.Text = m_content[k];
                }
                i++;
            }
        }
    }
}