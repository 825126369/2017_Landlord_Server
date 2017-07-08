using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace WebManager.ashx
{
    /// <summary>
    /// PlayerOp 的摘要说明
    /// </summary>
    public class PlayerOp : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            RightMgr.getInstance().opCheck(RightDef.OP_PLAYER_OP, context.Session, context.Response);
            ParamPlayerOp param = new ParamPlayerOp();
            param.m_op = context.Request.Form["op"];
            param.m_acc = context.Request.Form["playerId"];
            param.m_prop = context.Request.Form["prop"];
            param.m_value = context.Request.Form["value"];

            GMUser user = (GMUser)context.Session["user"];
            OpRes res = user.doDyop(param, DyOpType.opTypeKickPlayer);

            string str = "";
            if (param.m_op == "getLogFishList" ||
                param.m_op == "getLimitDbSendPlayer")
            {
                DyOpMgr mgr = user.getSys<DyOpMgr>(SysType.sysTypeDyOp);
                List<int> dyres = (List<int>)mgr.getResult(DyOpType.opTypeKickPlayer);
                str = string.Join(",", dyres);
            }
            else
            {
                str = OpResMgr.getInstance().getResultString(res);
            }

            context.Response.ContentType = "text/plain";
            context.Response.Write(str);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}