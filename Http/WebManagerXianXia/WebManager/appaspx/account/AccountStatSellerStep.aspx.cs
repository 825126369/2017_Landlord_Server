using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.account
{
    public partial class AccountStatSellerStep : System.Web.UI.Page
    {
        ViewStatSellerStep m_view = new ViewStatSellerStep();
        private string m_acc;

        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck("", Session, Response);

            if (!IsPostBack)
            {
                string time = Request.QueryString["time"];
                if (!string.IsNullOrEmpty(time))
                {
                    __gmAccountCascadeStaticTime.Text = time;
                }
                m_acc = Request.QueryString["acc"];

                if (time != null && m_acc != null)
                {
                    onStat(null, null);
                }
              //  m_way.Items.Add(new ListItem("日", ((int)StatSellerType.stat_seller_type_day).ToString()));
               // m_way.Items.Add(new ListItem("月", ((int)StatSellerType.stat_seller_type_month).ToString()));
               // m_way.SelectedIndex = 0;
            }
        }

        protected void onStat(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];
            ParamStatSellerStep param = new ParamStatSellerStep();
            if (string.IsNullOrEmpty(m_acc))
            {
                param.m_creator = user.m_user;
                param.m_statType = 1;
            }
            else
            {
                param.m_creator = m_acc;
            }
            param.m_time = __gmAccountCascadeStaticTime.Text;

            OpRes res = user.doStat(param, StatType.statTypeSellerStep);
            m_view.genTable(m_result, res, user);
        }
    }
}