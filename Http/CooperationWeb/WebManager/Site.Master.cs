using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager
{
    public partial class Site : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        // 整个菜单呈现前要执行的
        protected void onMenuPreRender(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];
            if (user.m_type != "admin")
            {
                foreach (MenuItem item in menuMain.Items)
                {
                    MenuItemCollection mc = item.ChildItems;
                    for (int i = mc.Count - 1; i >= 4 && i < mc.Count; i--)
                    {
                        mc.RemoveAt(i);
                    }
                }
            }
        }
    }
}