using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.service
{
    public partial class ServiceModifyPwd : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck(RightDef.SVR_RESET_PLAYER_PWD, Session, Response);
        }

        protected void onModify(object sender, EventArgs e)
        {
            ParamModifyPwd p = new ParamModifyPwd();
            p.m_playerId = m_account.Text;
            p.m_newPwd = m_newPwd.Text;
            p.m_pwdType = Convert.ToInt32(m_pwdType.SelectedValue);

            GMUser user = (GMUser)Session["user"];
            DyOpMgr mgr = user.getSys<DyOpMgr>(SysType.sysTypeDyOp);
            OpRes res = mgr.doDyop(p, DyOpType.opTypeModifyPwd, user);
            m_res.InnerHtml = OpResMgr.getInstance().getResultString(res);
        }
    }
}
