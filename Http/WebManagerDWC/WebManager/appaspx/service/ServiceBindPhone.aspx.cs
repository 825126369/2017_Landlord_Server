using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.service
{
    public partial class ServiceBindPhone : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void onModify(object sender, EventArgs e)
        {
            ParamModifyPwd p = new ParamModifyPwd();
            p.m_account = m_account.Text;
            p.m_phone = m_phone.Text;
            GMUser user = (GMUser)Session["user"];
            DyOpMgr mgr = user.getSys<DyOpMgr>(SysType.sysTypeDyOp);
            OpRes res = mgr.doDyop(p, DyOpType.opTypeBindPhone, user);
            m_res.InnerHtml = OpResMgr.getInstance().getResultString(res);
        }
    }
}