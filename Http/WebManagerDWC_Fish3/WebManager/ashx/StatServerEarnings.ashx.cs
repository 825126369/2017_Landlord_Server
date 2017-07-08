using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using System.Web.UI.WebControls;

namespace WebManager.ashx
{
    public class StatServerEarnings : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            RightMgr.getInstance().opCheck(RightDef.DATA_MONEY_FLOW, context.Session, context.Response);

            GMUser user = (GMUser)context.Session["user"];
            ParamServerEarnings param = new ParamServerEarnings();
            param.m_gameId = Convert.ToInt32(context.Request.Form["gameId"]);
            param.m_time = context.Request.Form["time"];
            OpRes res = user.doQuery(param, QueryType.queryTypeServerEarnings);
            string str="";
            if (param.m_gameId == 0)
            {
                TableStatServerEarningsTotal view = new TableStatServerEarningsTotal();
                Table table = new Table();
                view.genTable(user, table, res, null);
                str = ItemHelp.genHTML(table);
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