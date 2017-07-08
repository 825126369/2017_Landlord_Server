using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using System.Web.UI.WebControls;
using System.IO;
using System.Web.UI;

namespace WebManager.ashx
{
    public class OnlinePerHour : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            RightMgr.getInstance().opCheck(RightDef.TD_ONLINE_PER_HOUR, context.Session, context.Response);
            int op = Convert.ToInt32(context.Request.Form["op"]);
            string time = context.Request.Form["time"];

            ParamQuery param = new ParamQuery();
            if (op == 0)
            {
                param.m_time = time;
                param.m_param = context.Request.Form["param"];
            }
            else
            {
                param.m_time = DateTime.Now.Date.AddDays(-30).ToShortDateString() + "-" + DateTime.Now.Date.ToShortDateString();
            }
            param.m_way = QueryWay.by_way0 + op;

            GMUser user = (GMUser)context.Session["user"];
            OpRes res = user.doQuery(param, QueryType.queryTypeOnlinePlayerNumPerHour);

            string str = "";

            if (res == OpRes.opres_success)
            {
                if (op == 0)
                {
                    Table table = new Table();
                    TableTdOnlinePlayerNumPerHour view = new TableTdOnlinePlayerNumPerHour();
                    view.genTable(user, table, res);

                    StringWriter sw = new StringWriter();
                    HtmlTextWriter w = new HtmlTextWriter(sw);
                    table.RenderControl(w);
                    str = sw.GetStringBuilder().ToString();
                }
                else
                {
                    ResultOnlinePlayerNumPerHour qresult =
                   (ResultOnlinePlayerNumPerHour)user.getQueryResult(null, QueryType.queryTypeOnlinePlayerNumPerHour);
                    str = qresult.toJson();
                }
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