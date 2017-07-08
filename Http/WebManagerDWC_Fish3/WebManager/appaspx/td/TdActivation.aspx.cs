using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.td
{
    public partial class TdActivation : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck(RightDef.TD_ACTIVE_DATA, Session, Response);

            if (!IsPostBack)
            {
                m_channel.Items.Add(new ListItem("全部", ""));

                Dictionary<string, TdChannelInfo> data = TdChannel.getInstance().getAllData();
                foreach (var item in data.Values)
                {
                    m_channel.Items.Add(new ListItem(item.m_channelName, item.m_channelNo));
                }
            }
        }

        protected void onQuery(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];

            TableTdActivation view = new TableTdActivation();
            ParamQuery param = new ParamQuery();
            param.m_param = m_channel.SelectedValue;
            param.m_time = m_time.Text;
            OpRes res = user.doQuery(param, QueryType.queryTypeTdActivation);
            view.genTable(user, m_result, res);
        }
    }
}