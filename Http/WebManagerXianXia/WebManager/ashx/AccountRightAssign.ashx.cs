using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace WebManager.ashx
{
    /// <summary>
    /// AccountRightAssign 的摘要说明
    /// </summary>
    public class AccountRightAssign : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            RightMgr.getInstance().opCheck("modify", context.Session, context.Response);
            ParamModifyGmRight param = new ParamModifyGmRight();
            param.m_acc = context.Request.Form["acc"];
            param.m_right = context.Request.Form["rstr"];
            param.m_op = 1;

            GMUser user = (GMUser)context.Session["user"];
            OpRes res = user.doDyop(param, DyOpType.opTypeDyOpModiyGmRight);
            string retStr = OpResMgr.getInstance().getResultString(res);

            context.Response.ContentType = "text/plain";
            context.Response.Write(retStr);
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