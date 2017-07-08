using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.account
{
    public partial class AccountStatSwitch : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck("", Session, Response);

            GMUser user = (GMUser)Session["user"];
            switch (user.m_accType)
            {
                case AccType.ACC_SUPER_ADMIN: // 超级管理员
                    {
                    }
                    break;
                case AccType.ACC_DEALER: // 经销商
                    {
                        Server.Transfer("/appaspx/account/AccountStatSeller.aspx");
                    }
                    break;
                case AccType.ACC_DEALER_ADMIN: // 经销商管理员
                    {
                        Server.Transfer("/appaspx/account/AccountStatSeller.aspx");
                    }
                    break;
                case AccType.ACC_SELLER: // 售货亭
                case AccType.ACC_SELLER_SUB:  // 售货亭子账号
                    {
                        Server.Transfer("/appaspx/account/AccountStatSeller.aspx");
                    }
                    break;
                case AccType.ACC_SELLER_ADMIN: // 售货亭管理员
                    {
                        Server.Transfer("/appaspx/account/AccountStatSellerAdmin.aspx");
                    }
                    break;
            }
        }
    }
}