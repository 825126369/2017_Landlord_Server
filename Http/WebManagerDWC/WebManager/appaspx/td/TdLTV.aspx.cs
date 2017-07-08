using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.td
{
    public partial class TdLTV : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck("operation", Session, Response);

            if (!IsPostBack)
            {
                GMUser user = (GMUser)Session["user"];
                m_channel.Items.Add(new ListItem("全部平均", ""));
                if (user.m_type == "admin" || user.m_viewChannel == "all")
                {
                    Dictionary<string, TdChannelInfo> data = TdChannel.getInstance().getAllData();
                    foreach (var item in data.Values)
                    {
                        m_channel.Items.Add(new ListItem(item.m_channelName, item.m_channelNo));
                    }
                }
                else
                {
                    Dictionary<string, TdChannelInfo> data = TdChannel.getInstance().getAllData();
                    foreach (var item in data.Values)
                    {
                        if (user.m_viewChannel.IndexOf(item.m_channelNo) >= 0)
                        {
                            m_channel.Items.Add(new ListItem(item.m_channelName, item.m_channelNo));
                        }
                    }
                }
            }
        }

        protected void onQuery(object sender, EventArgs e)
        {
            TableTdLTVBase view = TableTdLTVBase.create(m_channel.SelectedIndex);
            if (view == null)
                return;

            GMUser user = (GMUser)Session["user"];
            ParamQuery param = new ParamQuery();
            param.m_param = m_channel.SelectedValue;
            param.m_time = m_time.Text;
            OpRes res = view.query(user, param);
            view.genTable(user, m_result, res);
        }
    }
}