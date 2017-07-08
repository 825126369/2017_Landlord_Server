using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx
{
    public partial class ExitLogin : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // 退出系统
            Session.Abandon();
            Response.Redirect("~/appaspx/UserLogin.aspx");
        }
    }
}