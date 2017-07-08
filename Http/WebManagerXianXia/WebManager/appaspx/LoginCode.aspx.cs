using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx
{
    public partial class LoginCode : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            UserVerification.checkLogin(Session, Response, 3);
        }

        protected void onLogin(object sender, EventArgs e)
        {
            bool res = AccountSys.getInstance().onLoginVerification(m_acc.Value, Session);
            if (res)
            {
                try
                {
                    UserVerification verInfo = (UserVerification)Session[DefCC.KEY_VERIFICATION];
                    LoginResult loginRes = AccountSys.getInstance().onLogin(verInfo.m_acc.Trim(),
                        verInfo.m_pwd1);
                    if (loginRes.isSuccess())
                    {
                        string ip = WebManager.Account.Login.getIPAddress();
                        AccountSys.getInstance().onLoginSuccess(Session, loginRes, ip, false);
                        verInfo.clear();
                        Session[DefCC.KEY_VERIFICATION] = null;
                        Response.Redirect(DefCC.ASPX_LOGIN_ENTER);
                    }
                }
                catch (System.Exception ex)
                {
                }
            }
            else
            {
                tdError.InnerText = "验证码错误";
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