using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.account
{
    public partial class AccountCreateDealer : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck("", Session, Response);

            GMUser user = (GMUser)Session["user"];
            if (user.m_accType != AccType.ACC_SUPER_ADMIN)
            {
                Server.Transfer(DefCC.ASPX_EMPTY);
            }

            if (!IsPostBack)
            {
               /* for (int i = 0; i < StrName.s_moneyInfo.Count; i++)
                {
                    m_moneyType.Items.Add(StrName.getMoneyDesc(i));
                }*/

                RangeValidator2.MaximumValue = (ConstDef.MAX_WASH_RATIO*100).ToString();
                m_agentRatio.Text = (ConstDef.MAX_AGENT_RATIO*100).ToString();
                m_washRatio.Text = (ConstDef.MAX_WASH_RATIO*100).ToString();
            }
        }

        protected void onCreateAccount(object sender, EventArgs e)
        {
            ParamCreateGmAccount param = new ParamCreateGmAccount();
            param.m_accType = AccType.ACC_GENERAL_AGENCY;
            param.m_accName = m_accName.Text;
            param.m_pwd1 = m_pwd1.Text;
            param.m_pwd2 = m_pwd2.Text;
           // param.m_moneyType = m_moneyType.SelectedIndex;
            param.m_aliasName = m_aliasName.Text;
            param.m_agentRatio = m_agentRatio.Text;
            param.m_washRatio = m_washRatio.Text;

            GMUser user = (GMUser)Session["user"];
            DyOpMgr mgr = user.getSys<DyOpMgr>(SysType.sysTypeDyOp);
            OpRes res = mgr.doDyop(param, DyOpType.opTypeDyOpCreateGmAccount, user);
            m_res.InnerHtml = OpResMgr.getInstance().getResultString(res);

            if (res == OpRes.opres_success)
            {
                m_res.InnerHtml += "," +
                    OpResMgr.getInstance().getResultString(OpRes.op_res_account_info, param.m_resultAcc, param.m_validatedCode);
            }
        }
    }
}