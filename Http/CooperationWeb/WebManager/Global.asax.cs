using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace WebManager
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            SysMgr mgr = new SysMgr();
            mgr.addSys(new AccountSys());
            mgr.addSys(new RightSys());
            mgr.addSys(new ChannelSys());
            mgr.initSys();

            Application["sys"] = mgr;
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            Session["user"] = new GMUser();
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];
            if (user != null)
            {
                user.exitLogin();
                user = null;
            }
        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}