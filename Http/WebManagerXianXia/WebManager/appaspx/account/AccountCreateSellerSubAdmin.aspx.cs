using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.account
{
    public partial class AccountCreateSellerSubAdmin : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck("", Session, Response);

            if (!IsPostBack)
            {
                m_type.Items.Add("创建售货亭管理员");
                m_type.Items.Add("创建售货亭子账号");
            }
        }

        protected void onCreateAccount(object sender, EventArgs e)
        {
            ParamCreateGmAccount param = new ParamCreateGmAccount();
            param.m_accType = AccType.ACC_SELLER_ADMIN + m_type.SelectedIndex;
            param.m_pwd1 = m_pwd1.Text;
            param.m_pwd2 = m_pwd2.Text;
  
            GMUser user = (GMUser)Session["user"];
            DyOpMgr mgr = user.getSys<DyOpMgr>(SysType.sysTypeDyOp);
            OpRes res = mgr.doDyop(param, DyOpType.opTypeDyOpCreateGmAccount, user);
            m_res.InnerHtml = OpResMgr.getInstance().getResultString(res);

            if (res == OpRes.opres_success)
            {
                m_res.InnerHtml += "," +
                    OpResMgr.getInstance().getResultString(OpRes.op_res_account_info, param.m_resultAcc);
            }
        }
    }
}