using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx
{
    public partial class LoginAccount : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];
            if (user != null)
            {
                Response.Redirect(DefCC.ASPX_LOGIN_ENTER);
            }

            // 号被占用
            object obj = Session["occupy"];
            if (obj != null && Convert.ToBoolean(obj) == true)
            {
                Session.Clear();
               // LoginUser.FailureText = "你的账号在别处登录";
            }

            UserVerification verInfo = (UserVerification)Session[DefCC.KEY_VERIFICATION];
            if (verInfo != null)
            {
                verInfo.reset();
            }

            Image1.ImageUrl = "/ashx/ValidatedCode.ashx?r=" + new Random().NextDouble();
        }

        protected void onLogin(object sender, EventArgs e)
        {
            bool res = AccountSys.getInstance().onLoginVerification(m_acc.Value, Session, m_ver.Value);
            if (res)
            {
                Response.Redirect(DefCC.ASPX_LOGIN_STEP2);
            }
        }
    }
}