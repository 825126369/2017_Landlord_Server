using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;

namespace AccountCheck
{
    // 清理登录失败次数
    public partial class ClearFailedLoginCount : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string acc = Request.QueryString["acc"];

            string platform = Request.QueryString["platform"];

            if (string.IsNullOrEmpty(acc) || string.IsNullOrEmpty(platform))
            {
                Response.Write("1"); // 缺少参数
                return;
            }

            string table = ConfigurationManager.AppSettings["acc_" + platform];
            if (string.IsNullOrEmpty(table))
            {
                Response.Write("2"); // 平台错误
                return;
            }

            Dictionary<string, object> updata = new Dictionary<string, object>();
            updata.Add("loginFailedCount", 0);

            string strerr = MongodbAccount.Instance.ExecuteUpdate(table, "acc", acc, updata);
            if (strerr != "")
            {
                Response.Write("3"); // 数据库失败
            }
            else
            {
                Response.Write("0");
            }
        }
    }
}