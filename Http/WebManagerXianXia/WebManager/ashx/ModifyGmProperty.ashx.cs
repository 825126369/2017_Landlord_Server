using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace WebManager.ashx
{
    public class ModifyGmProperty : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            RightMgr.getInstance().opCheck("", context.Session, context.Response);

            GMUser user = (GMUser)context.Session["user"];
            ParamModifyGmProperty param = new ParamModifyGmProperty();
            param.m_whichProperty = Convert.ToInt32(context.Request.Form["op"]);
            param.m_acc = context.Request.Form["acc"];
            param.m_param = context.Request.Form["param"];
            OpRes res = user.doDyop(param, DyOpType.opTypeDyOpModifyGmProperty);
            string str = OpResMgr.getInstance().getResultString(res);
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