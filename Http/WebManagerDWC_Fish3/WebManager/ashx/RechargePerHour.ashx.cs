using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace WebManager.ashx
{
    public class RechargePerHour : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            RightMgr.getInstance().opCheck(RightDef.TD_RECHARGE_PER_HOUR, context.Session, context.Response);
            ParamQuery param = new ParamQuery();
            param.m_time = DateTime.Now.Date.AddDays(-1).ToShortDateString() + "-" + DateTime.Now.Date.ToShortDateString();

            GMUser user = (GMUser)context.Session["user"];
            OpRes res = user.doQuery(param, QueryType.queryTypeRechargePerHour);
            Dictionary<string, object> retData = new Dictionary<string, object>();

            if (res == OpRes.opres_success)
            {
                DataEachDay qresult = (DataEachDay)user.getQueryResult(QueryType.queryTypeRechargePerHour);
                var allData = qresult.getData();
                foreach (var data in allData)
                {
                    var s = string.Join<int>(",", data.m_data);
                    retData.Add(data.m_time.ToShortDateString(), s);
                }
            }

            string str = ItemHelp.genJsonStr(retData);
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