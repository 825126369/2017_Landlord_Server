using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.account.sub
{
    public partial class AccountSubModifyAliasName : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck("", Session, Response);
            if (!IsPostBack)
            {
                string acc = Request.QueryString["acc"];
                m_acc.Text = acc;
            }
        }

        protected void onModifyRight(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];
            ParamModifyGmProperty param = new ParamModifyGmProperty();
            param.m_whichProperty = ParamModifyGmProperty.MODIFY_ALIASNAME;
            param.m_acc = m_acc.Text;
            param.m_param = m_aliasName.Text;
            OpRes res = user.doDyop(param, DyOpType.opTypeDyOpModifyGmProperty);
            m_res.InnerText = OpResMgr.getInstance().getResultString(res);
        }
    }
}