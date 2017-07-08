using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace WebManager.ashx
{
    /// <summary>
    /// AccountDel 的摘要说明
    /// </summary>
    public class AccountDel : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            RightMgr.getInstance().opCheck(RIGHT.DEL_ACCOUNT, context.Session, context.Response);
            ParamDelAcc param = new ParamDelAcc();
            param.m_acc = context.Request.QueryString["acc"];
            param.m_op = Convert.ToInt32(context.Request.QueryString["op"]);
            GMUser user = (GMUser)context.Session["user"];
            OpRes res = user.doDyop(param, DyOpType.opTypeDelAccount);
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