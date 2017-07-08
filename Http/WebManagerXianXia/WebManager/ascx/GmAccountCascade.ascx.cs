using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.ascx
{
    public partial class GmAccountCascade : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public void init(GMUser user)
        {
            switch (user.m_accType)
            {
                case AccType.ACC_SUPER_ADMIN:
                    {
                        m_dealer.Items.Add(new ListItem("全部", ""));
                        ItemHelp.fillDropDownList(m_dealer, AccType.ACC_DEALER, user.m_user, user);
                        onDealerChanged(null, null);
                    }
                    break;
                case AccType.ACC_DEALER:
                    {
                        m_dealerAdmin.Items.Add(new ListItem("全部", ""));
                        ItemHelp.fillDropDownList(m_dealerAdmin, AccType.ACC_DEALER_ADMIN, user.m_user, user);
                        onDealerAdminChanged(null, null);
                        m_dealer.Enabled = false;
                    }
                    break;
                case AccType.ACC_DEALER_ADMIN:
                    {
                        m_seller.Items.Add(new ListItem("全部", ""));
                        ItemHelp.fillDropDownList(m_seller, AccType.ACC_SELLER, user.m_user, user);
                        onSellerChanged(null, null);
                        m_dealer.Enabled = false;
                        m_dealerAdmin.Enabled = false;
                    }
                    break;
                case AccType.ACC_SELLER:
                    {
                        m_sellerAdmin.Items.Add(new ListItem("全部", ""));
                        ItemHelp.fillDropDownList(m_sellerAdmin, AccType.ACC_SELLER_ADMIN, user.m_user, user);
                        m_dealer.Enabled = false;
                        m_dealerAdmin.Enabled = false;
                        m_seller.Enabled = false;
                    }
                    break;
                case AccType.ACC_SELLER_ADMIN:
                    {
                        m_dealer.Enabled = false;
                        m_dealerAdmin.Enabled = false;
                        m_seller.Enabled = false;
                        m_sellerAdmin.Enabled = false;
                    }
                    break;
            }
        }

        // canSearchAll是否允许搜索所有信息
        public void fillCondtion(ParamMemberInfo param, GMUser user, bool canSearchAll = false)
        {
            param.m_dealer = m_dealer.SelectedValue;
            param.m_dealerAdmin = m_dealerAdmin.SelectedValue;
            param.m_seller = m_seller.SelectedValue;
            param.m_sellerAdmin = m_sellerAdmin.SelectedValue;
            param.m_acc = m_acc.Text;
            param.m_time = __gmAccountCascadeStaticTime.Text;

            if (canSearchAll)
                return;

            switch (user.m_accType)
            {
                case AccType.ACC_DEALER:
                    {
                        param.m_dealer = user.m_user;
                    }
                    break;
                case AccType.ACC_DEALER_ADMIN:
                    {
                        param.m_dealerAdmin = user.m_user;
                    }
                    break;
                case AccType.ACC_SELLER:
                    {
                        param.m_seller = user.m_user;
                    }
                    break;
                case AccType.ACC_SELLER_ADMIN:
                    {
                        param.m_sellerAdmin = user.m_user;
                    }
                    break;
            }
        }

        public void setEnable(int accType, bool enable)
        {
            if (accType == AccType.ACC_SELLER_ADMIN)
            {
                m_sellerAdmin.Enabled = enable;
            }
        }

        public DropDownList getDropDownList(int accType)
        {
            if (accType == AccType.ACC_SELLER_ADMIN)
            {
                return m_sellerAdmin;
            }
            return null;
        }

        protected void onDealerChanged(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];
            m_dealerAdmin.Items.Clear();
            m_dealerAdmin.Items.Add(new ListItem("全部", ""));
            ItemHelp.fillDropDownList(m_dealerAdmin, AccType.ACC_DEALER_ADMIN, m_dealer.SelectedValue, user);

            onDealerAdminChanged(null, null);
        }

        // 经销商管理员
        protected void onDealerAdminChanged(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];
            m_seller.Items.Clear();
            m_seller.Items.Add(new ListItem("全部", ""));
            ItemHelp.fillDropDownList(m_seller, AccType.ACC_SELLER, m_dealerAdmin.SelectedValue, user);

            onSellerChanged(null, null);
        }

        protected void onSellerChanged(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];
            m_sellerAdmin.Items.Clear();
            m_sellerAdmin.Items.Add(new ListItem("全部", ""));
            ItemHelp.fillDropDownList(m_sellerAdmin, AccType.ACC_SELLER_ADMIN, m_seller.SelectedValue, user);
        }
    }
}