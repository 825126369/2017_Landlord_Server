using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace WebManager.ashx
{
    /// <summary>
    /// GameOpen 的摘要说明
    /// </summary>
    public class GameOpen : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            RightMgr.getInstance().opCheck("", context.Session, context.Response);
            GMUser user = (GMUser)context.Session["user"];
            string gList = context.Request.Form["gameList"];
            OpRes res = user.doDyop(gList, DyOpType.opTypeOpenGame);
            string ret = OpResMgr.getInstance().getResultString(res);
            context.Response.ContentType = "text/plain";
            context.Response.Write(ret);
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