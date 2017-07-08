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
                    string url = getErrorURL(right);
                    if (url == "")
                    {
                        ErrorInfo.InnerText = "对不起，您没有权限!";
                    }
                    else
                    {
                        Response.Redirect(url);
                    }
                }
            }
        }

        string getErrorURL(string rightId)
        {
            RightBaseInfo info = ResMgr.getInstance().getBaseRightInfo(rightId);
            if (info == null)
                return "";

            string url = "";
            switch (info.m_category)
            {
                case "op":
                    {
                        url = "~/appaspx/operation/Error.aspx";
                    }
                    break;
                case "svr":
                    {
                        url = "~/appaspx/service/Error.aspx";
                    }
                    break;
                case "td":
                    {
                        url = "~/appaspx/td/Error.aspx";
                    }
                    break;
                case "data":
                    {
                        url = "~/appaspx/stat/Error.aspx";
                    }
                    break;
                case "fish":
                    {
                        url = "~/appaspx/stat/fish/Error.aspx";
                    }
                    break;
                case "crod":
                    {
                        url = "~/appaspx/stat/crocodile/Error.aspx";
                    }
                    break;
                case "dice":
                    {
                        url = "~/appaspx/stat/dice/Error.aspx";
                    }
                    break;
                case "bacc":
                    {
                        url = "~/appaspx/stat/baccarat/Error.aspx";
                    }
                    break;
                case "cow":
                    {
                        url = "~/appaspx/stat/cows/Error.aspx";
                    }
                    break;
                case "d5":
                    {
                        url = "~/appaspx/stat/5dragons/Error.aspx";
                    }
                    break;
                case "shcd":
                    {
                        url = "~/appaspx/stat/shcd/Error.aspx";
                    }
                    break;
                case "calf":
                    {
                    }
                    break;
                case "other":
                    {

                    }
                    break;
            }

            return url;
        }
    }
}