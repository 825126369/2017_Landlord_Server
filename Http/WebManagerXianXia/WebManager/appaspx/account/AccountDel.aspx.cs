using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx
{
    public partial class AccountDel : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck(RIGHT.DEL_ACCOUNT, Session, Response);
        }
    }
}