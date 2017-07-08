using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx
{
    public partial class AddAccount : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RightSys.getInstance().opCheck("addAccount", Session, Response);
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            GMUser user =(GMUser)Session["user"];
            bool res = AccountSys.getInstance().addAccount(txtAccount.Text, txtPwd.Text, txtPwdRep.Text, user);
            OpRes op = OpRes.opres_success;
            if (!res)
            {
                op = OpRes.op_res_failed;
            }

            m_opRes.InnerText = OpResMgr.getInstance().getResultString(op);
        }
    }
}