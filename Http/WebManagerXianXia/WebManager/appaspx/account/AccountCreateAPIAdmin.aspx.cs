using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.account
{
    public partial class AccountCreateAPIAdmin : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck("", Session, Response);

            GMUser user = (GMUser)Session["user"];
            if (!user.isAPIAcc())
            {
                Server.Transfer(DefCC.ASPX_EMPTY);
            }
        }

        protected void onCreateAccount(object sender, EventArgs e)
        {
            ParamCreateGmAccount param = new ParamCreateGmAccount();
            param.m_accType = AccType.ACC_API_ADMIN;
            param.m_pwd1 = m_pwd1.Text;
            param.m_pwd2 = m_pwd2.Text;
            param.m_aliasName = m_aliasName.Text;
            param.m_agentRatio = "0";
            param.m_washRatio = "0";

            GMUser user = (GMUser)Session["user"];
            OpRes res = user.doDyop(param, DyOpType.opTypeDyOpCreateGmAccount);
            m_res.InnerHtml = OpResMgr.getInstance().getResultString(res);

            if (res == OpRes.opres_success)
            {
                m_res.InnerHtml += "," +
                    OpResMgr.getInstance().getResultString(OpRes.op_res_account_info, param.m_resultAcc, param.m_validatedCode);
            }
        }
    }
}