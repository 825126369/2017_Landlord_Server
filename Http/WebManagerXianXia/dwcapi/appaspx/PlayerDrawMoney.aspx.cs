using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace dwcapi.appaspx
{
    public partial class PlayerDrawMoney : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Helper.doScore(Request, Response, ScropOpType.EXTRACT_SCORE);
        }
    }
}