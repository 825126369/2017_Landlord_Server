using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using System.Web.UI.WebControls;

namespace WebManager.ashx
{
    public class PlayerGameBet : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            RightMgr.getInstance().opCheck("", context.Session, context.Response);

            GMUser user = (GMUser)context.Session["user"];
            ParamPlayerGameBet p = new ParamPlayerGameBet();
            p.m_time = context.Request.Form["time"];
            p.m_param = Convert.ToString(context.Request.Form["playerId"]);
            p.m_gameId = Convert.ToInt32(context.Request.Form["gameId"]);

            OpRes res = user.doQuery(p, QueryType.queryTypePlayerGameBet);
            TablePlayerGameBet view = new TablePlayerGameBet();
            Table t = new Table();
            view.genTable(user, t, res, null);
            string str = ItemHelp.genHTML(t);

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