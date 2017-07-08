using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.operation
{
    public partial class OperationSpeaker : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck(RightDef.OP_NOTIFY_MSG, Session, Response);
        }

        protected void btnSend_Click(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];
            ParamSpeaker param = new ParamSpeaker();
            param.m_content = txtContent.Text;
            param.m_sendTime = txtSendTime.Text;
            param.m_repCount = txtRepCount.Text;
           // param.m_interval = txtInterval.Text;

            DyOpMgr mgr = user.getSys<DyOpMgr>(SysType.sysTypeDyOp);
            OpRes res = mgr.doDyop(param, DyOpType.opTypeSpeaker, user);
            m_res.InnerHtml = OpResMgr.getInstance().getResultString(res);
        }
    }
}