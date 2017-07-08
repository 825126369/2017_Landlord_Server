using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace WebManager.ashx
{
    public class GameStandingBook : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            RightMgr.getInstance().opCheck("", context.Session, context.Response);

            string str = "";
            int op = Convert.ToInt32(context.Request.Form["op"]);
            switch (op)
            {
                case 0:
                    {
                        ParamQuery param = new ParamQuery();
                        param.m_time = context.Request.Form["time"];
                        param.m_param = context.Request.Form["itemId"];

                        GMUser user = (GMUser)context.Session["user"];
                        OpRes res = user.doQuery(param, QueryType.queryTypePlayerIncomeExpenses);
                        ResultPlayerIncomeExpenses result =
                            (ResultPlayerIncomeExpenses)user.getQueryResult(QueryType.queryTypePlayerIncomeExpenses);
                        if (res == OpRes.opres_success)
                        {
                            str = result.getJson();
                        }
                    }
                    break;
                case 1:
                    {
                        XmlConfig xml = ResMgr.getInstance().getRes("money_reason.xml");
                        str = ItemHelp.genJsonStr(xml.getData());
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