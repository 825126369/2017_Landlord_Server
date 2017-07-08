using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx
{
    public partial class LoginPwd : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            UserVerification.checkLogin(Session, Response, 2);
        }

        protected void onLogin(object sender, EventArgs e)
        {
            bool res = AccountSys.getInstance().onLoginVerification(m_acc.Value, Session);
            if (res)
            {
                Response.Redirect(DefCC.ASPX_LOGIN_STEP3);
            }
        }

        protected void onReturn(object sender, EventArgs e)
        {
            UserVerification verInfo = (UserVerification)Session[DefCC.KEY_VERIFICATION];
            if (verInfo != null)
            {
                verInfo.loginHasFinishStep = 0;
            }
            Response.Redirect(DefCC.ASPX_LOGIN_STEP1);
        }
    }
}