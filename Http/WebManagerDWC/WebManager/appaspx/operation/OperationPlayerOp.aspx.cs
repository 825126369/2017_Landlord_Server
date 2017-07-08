using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.operation
{
    public partial class OperationPlayerOp : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck("operation", Session, Response);
        }

        protected void onKickPlayer(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];
            ParamPlayerOp param = new ParamPlayerOp();
            param.m_acc = m_playerId.Text;
            OpRes res = user.doDyop(param, DyOpType.opTypeKickPlayer);
            m_res.InnerHtml = OpResMgr.getInstance().getResultString(res);
        }

        protected void onAddNewTask(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];
            ServiceParam param = new ServiceParam();
            param.m_toServerIP = user.m_dbIP;
            OpRes res = RemoteMgr.getInstance().reqDoService(param, ServiceType.serviceTypeUpdatePlayerTask);
            m_res.InnerHtml = OpResMgr.getInstance().getResultString(res);
        }
    }
}