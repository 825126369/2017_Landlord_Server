using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.operation
{
    public partial class OperationGenGift : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck("operation", Session, Response);
        }

        protected void onAddGift(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];
            ParamGift p = createParamGift(true);
            DyOpMgr mgr = user.getSys<DyOpMgr>(SysType.sysTypeDyOp);
            OpRes res = mgr.doDyop(p, DyOpType.opTypeGift, user);
            m_res.InnerHtml = OpResMgr.getInstance().getResultString(res);
        }

        protected void onModifyGift(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];
            ParamGift p = createParamGift(false);
            DyOpMgr mgr = user.getSys<DyOpMgr>(SysType.sysTypeDyOp);
            OpRes res = mgr.doDyop(p, DyOpType.opTypeGift, user);
            m_res.InnerHtml = OpResMgr.getInstance().getResultString(res);
        }

        private ParamGift createParamGift(bool isAdd)
        {
            ParamGift p = new ParamGift();
            p.m_isAdd = isAdd;
            p.m_itemList = m_content.Text;
            p.m_deadTime = m_deadTime.Text;
            return p;
        }
    }
}