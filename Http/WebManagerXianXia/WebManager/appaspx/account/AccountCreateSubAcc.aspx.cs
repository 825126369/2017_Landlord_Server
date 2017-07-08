using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.account
{
    public partial class AccountCreateSubAcc : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //RightMgr.getInstance().opCheck(RIGHT.CREATE_SUB_ACC, Session, Response);
            GMUser user = (GMUser)Session["user"];
            switch (user.m_accType)
            {
                case AccType.ACC_SUPER_ADMIN:
                    break;
                case AccType.ACC_GENERAL_AGENCY: // 总代理
                case AccType.ACC_AGENCY: // 代理
                    {
                        Server.Transfer("/appaspx/account/AccountCreateDealerSubAdmin.aspx?isCreateSub=true");
                    }
                    break;
                case AccType.ACC_API:
                    {
                        Server.Transfer("/appaspx/account/AccountCreateAPIAdmin.aspx");
                    }
                    break;
                default:
                    {
                        Server.Transfer(DefCC.ASPX_EMPTY);
                    }
                    break;
            }
        }
    }
}