using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace HttpLogin.BaiduView
{
    public partial class IPTest : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Write("headers:\n");
            foreach(var s in Request.Headers.AllKeys)
            {
                string str = string.Format("{0}={1}\n", s, Request.Headers[s].ToString());
                Response.Write(str);
            }
            Response.Write("ServerVariables:\n");
            foreach (var s in Request.ServerVariables.AllKeys)
            {
                string str = string.Format("{0}={1}\n", s, Request.ServerVariables[s].ToString());
                Response.Write(str);
            }
        }
    }
}