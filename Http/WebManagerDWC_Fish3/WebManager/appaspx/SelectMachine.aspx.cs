using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx
{
    public partial class SelectMachine : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string db = Request["db"];
            if (db != null)
            {
                GMUser user = (GMUser)Session["user"];
                if (user == null)
                {
                    Response.Redirect("~/Account/Login.aspx");
                }
                else
                {
                    bool res = user.changeGameDb(db);
                    if (res)
                    {
                        // 切换DB成功后，返回原来的页面，可以继续操作。
                        if (user.preURL != "")
                        {
                            Response.Redirect(user.preURL);
                        }
                        else
                        {
                            Label1.Text = "切换到DB服务器：" + db;
                        }
                    }
                    else
                    {
                        Label1.Text = "切换到 " + db + " 时出错， 没有连接数据库!";
                    }
                }
            }
        }
    }
}
