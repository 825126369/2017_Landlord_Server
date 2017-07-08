using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace WebManager.ashx
{
    public class TdNewPlayerAnalyze : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            RightMgr.getInstance().opCheck("", context.Session, context.Response);

            ParamQuery param = new ParamQuery();
            param.m_param = context.Request.Form["data"];
            param.m_time = context.Request.Form["time"];

            string str = "";
            if (param.m_param == "4")
            {
                Dictionary<string, object> ret = new Dictionary<string, object>();
                var data = Fish_LevelCFG.getInstance().getAllData();
                foreach (var d in data)
                {
                    ret.Add(d.Key.ToString(), d.Value.m_openRate);
                }
                str = ItemHelp.genJsonStr(ret);
            }
            else
            {
                GMUser user = (GMUser)context.Session["user"];
                OpRes res = user.doQuery(param, QueryType.queryTypeNewPlayer);
                ResultNewPlayer result = (ResultNewPlayer)user.getQueryResult(QueryType.queryTypeNewPlayer);
                str = result.toJson(param.m_param);
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