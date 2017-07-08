using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.stat
{
    public partial class StatGeneralPlayerLost : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck(RightDef.DATA_PLAYER_LOSE, Session, Response);
        }

        protected void btnQuery_Click(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];

            ParamQuery param = new ParamQuery();
            param.m_time = m_time.Text;
            param.m_param = m_gold.Text;

            OpRes res = user.doQuery(param, QueryType.queryTypeAccountCoinLessValue);
            if (res != OpRes.opres_success)
            {
                m_res.InnerText = OpResMgr.getInstance().getResultString(res);
                return;
            }

            ResultAccountCoinLessValue rv =
                (ResultAccountCoinLessValue)user.getQueryResult(QueryType.queryTypeAccountCoinLessValue);

            Literal L = new Literal();
            L.Text = string.Format("总共:{0}", rv.m_totalCount);
            divResult.Controls.Add(L);
            
            L = new Literal();
            L.Text = "<br/>";
            divResult.Controls.Add(L);

            L = new Literal();
            
            L.Text = string.Format("小于指定金币共:{0}", rv.m_condCount);
            divResult.Controls.Add(L);
        }
    }
}