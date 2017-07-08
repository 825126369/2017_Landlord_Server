using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.stat.fishpark
{
    // 鳄鱼公园参数
    public partial class FishParkControl : System.Web.UI.Page
    {
        private string m_roomList = "";

        TableStatFishlordControl m_common = new TableStatFishlordControl();

        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck("stat", Session, Response);

            if (!IsPostBack)
            {
                GMUser user = (GMUser)Session["user"];
                m_common.genExpRateTable(m_expRateTable, user, QueryType.queryTypeFishParkParam);
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

            OpRes res = m_common.onModifyExpRate(user, m_expRate.Text, m_roomList, DyOpType.opTypeFishParkParamAdjust);
            m_res.InnerHtml = OpResMgr.getInstance().getResultString(res);
            m_common.genExpRateTable(m_expRateTable, user, QueryType.queryTypeFishParkParam);
        }

        protected void onReset(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];

            OpRes res = m_common.onReset(user, m_roomList, DyOpType.opTypeFishParkParamAdjust);
            m_res.InnerHtml = OpResMgr.getInstance().getResultString(res);
            m_common.genExpRateTable(m_expRateTable, user, QueryType.queryTypeFishParkParam);
        }
    }
}