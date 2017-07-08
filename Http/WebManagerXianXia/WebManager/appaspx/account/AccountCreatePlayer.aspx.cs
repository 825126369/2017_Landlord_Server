using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.account
{
    public partial class AccountCreatePlayer : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck("", Session, Response);

            GMUser user = (GMUser)Session["user"];
            if (user.m_accType != AccType.ACC_AGENCY &&
                user.m_accType != AccType.ACC_API &&
                user.m_accType != AccType.ACC_API_ADMIN) 
            {
                Server.Transfer(DefCC.ASPX_EMPTY);
            }

            if (user.isAPIAcc() || user.isAPIAdminAcc())
            {
                m_prefix.Text = user.m_postfix;
            }

            if (!IsPostBack)
            {
               // m_washRatio.Text = user.m_washRatio.ToString();
               // RangeValidator1.MaximumValue = user.m_washRatio.ToString();
            }
        }

        protected void onCreateAccount(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];
            ParamCreateGmAccount param = new ParamCreateGmAccount();
            param.m_pwd1 = m_pwd1.Text;
            param.m_pwd2 = m_pwd2.Text;
            param.m_accName = m_accName.Text;
            param.m_aliasName = m_aliasName.Text;
           // param.m_washRatio = m_washRatio.Text;
            if (m_hasWashRation.Checked)
            {
                param.m_washRatio = (user.m_washRatio * 100).ToString();
            }
            else
            {
                param.m_washRatio = "0";
            }
            DyOpMgr mgr = user.getSys<DyOpMgr>(SysType.sysTypeDyOp);
            OpRes res = mgr.doDyop(param, DyOpType.opTypeDyOpCreatePlayer, user);
            m_res.InnerHtml = OpResMgr.getInstance().getResultString(res);

            if (res == OpRes.opres_success)
            {
                m_res.InnerHtml += "," +
                    OpResMgr.getInstance().getResultString(OpRes.op_res_player_account_info, param.m_resultAcc);
            }
            else if (res == OpRes.op_res_failed)
            {
                m_res.InnerHtml += "," +
                    OpResMgr.getInstance().getResultString(OpRes.op_res_error_code, param.m_resultAcc);
            }
        }
    }
}