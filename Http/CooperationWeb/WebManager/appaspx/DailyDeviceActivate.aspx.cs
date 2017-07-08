using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx
{
    public partial class DailyDeviceActivate : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RightSys.getInstance().opCheck("", Session, Response);
        }

        protected void onQuery(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];
            bool res = viewResult.setTimeRange(txtTime.Text);
            if (!res)
            {
                m_opRes.InnerText = OpResMgr.getInstance().getResultString(OpRes.op_res_time_format_error);
                return;
            }
            m_opRes.InnerText = "";
            viewResult.queryType = QueryType.queryTypeDailyDeviceActivate;
            List<string> channelList = user.getViewChannelList();
            viewResult.channelList = channelList;

            viewResult.startQuery();
        }
    }
}