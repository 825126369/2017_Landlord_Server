using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace dwcapi.appaspx
{
    // 玩家存款
    public partial class PlayerSaveMoney : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Helper.doScore(Request, Response, ScropOpType.ADD_SCORE);
        }
    }
}