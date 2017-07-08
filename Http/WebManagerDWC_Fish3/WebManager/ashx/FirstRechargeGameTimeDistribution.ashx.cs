using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace WebManager.ashx
{
    public class FirstRechargeGameTimeDistribution : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            RightMgr.getInstance().opCheck("", context.Session, context.Response);
            ParamQuery param = new ParamQuery();
            param.m_time = context.Request.Form["time"];
            int op = Convert.ToInt32(context.Request.Form["op"]);
            GMUser user = (GMUser)context.Session["user"];
            string str = "";

            switch (op)
            {
                case 0:
                    {
                        OpRes res = user.doQuery(param, QueryType.queryTypeFirstRechargeGameTimeDistribution);
                        ResultFirstRechargeGameTimeDistribution result = 
                            (ResultFirstRechargeGameTimeDistribution)user.getQueryResult(QueryType.queryTypeFirstRechargeGameTimeDistribution);
                        str = result.toJson();
                    }
                    break;
                case 1:
                    {
                        OpRes res = user.doQuery(param, QueryType.queryTypeFirstRechargePointDistribution);
                        ResultFirstRechargePointDistribution result = 
                            (ResultFirstRechargePointDistribution)user.getQueryResult(QueryType.queryTypeFirstRechargePointDistribution);
                        str = result.toJson();
                    }
                    break;
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