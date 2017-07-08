using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.account
{
    public partial class ModifyLoginPwd : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck("", Session, Response);
        }

        protected void onModify(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];

            ParamModifyLoginPwd param = new ParamModifyLoginPwd();
            param.m_oriPwd = m_oriPwd.Text;
            param.m_newPwd1 = m_pwd1.Text;
            param.m_newPwd2 = m_pwd2.Text;
            OpRes res = user.doDyop(param, DyOpType.opTypeModifyGmLoginPwd);
            m_res.InnerText = OpResMgr.getInstance().getResultString(res);
        }
    }
}