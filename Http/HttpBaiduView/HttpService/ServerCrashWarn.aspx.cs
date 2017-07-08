using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AccountCheck
{
    // 游戏服务器crash报警页面
    public partial class ServerCrashWarn : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ExceptionCheckInfo.doSaveCheckInfo(Request, "crash");
        }
    }
}