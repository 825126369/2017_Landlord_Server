using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx
{
    public partial class UserLogin : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];
            if (user != null)
            {
                if (user.isLogin)
                {
                    Response.Redirect("~/appaspx/DailyDeviceActivate.aspx");
                }
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];
            bool res = user.doLogin(txtAccount.Text, txtPassword.Text);
            if (res)
            {
                Response.Redirect("~/appaspx/DailyDeviceActivate.aspx");
            }
            else
            {
                m_opRes.InnerText = "账号或密码错";
            }
        }
    }
}