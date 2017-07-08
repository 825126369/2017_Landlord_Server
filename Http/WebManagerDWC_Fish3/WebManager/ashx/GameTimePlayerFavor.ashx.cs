using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace WebManager.ashx
{
    public class GameTimePlayerFavor : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            RightMgr.getInstance().opCheck("", context.Session, context.Response);
            ParamQuery param = new ParamQuery();
            param.m_time = context.Request.Form["time"];
            param.m_param = context.Request.Form["playerType"];
            GMUser user = (GMUser)context.Session["user"];
            OpRes res = user.doQuery(param, QueryType.queryTypeGameTimePlayerFavor);
            ResultGameTimePlayerFavor result = (ResultGameTimePlayerFavor)user.getQueryResult(QueryType.queryTypeGameTimePlayerFavor);
            string str = "";
            if (res == OpRes.opres_success)
            {
                str = result.toJson();
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