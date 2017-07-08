using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx
{
    public partial class Error : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // 查看参数
            string right = Request["right"];
            if (right != null)
            {
                if (right == "@@")
                {
                    ErrorInfo.InnerText = "需要选择要操作的服务器";
                }
                else
                {
                    ErrorInfo.InnerText = "没有权限查看该页面"; // "你没有" + RightMgr.getInstance().getRrightName(right);
                }
            }
        }
    }
}