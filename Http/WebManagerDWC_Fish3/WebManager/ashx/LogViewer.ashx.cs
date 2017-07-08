using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace WebManager.ashx
{
    public class LogViewer : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            RightMgr.getInstance().opCheck(RightDef.OTHER_VIEW_LOG, context.Session, context.Response);
            GMUser user = (GMUser)context.Session["user"];

            ParamDelData param = new ParamDelData();
            param.m_tableName = TableName.OPLOG;
            param.m_param = Convert.ToString(context.Request.Form["param"]);
            OpRes res = user.doDyop(param, DyOpType.opTypeRemoveData);
            Dictionary<string, object> ret = new Dictionary<string, object>();
            ret.Add("result", (int)res);
            ret.Add("id", param.m_param);
            string str = ItemHelp.genJsonStr(ret);
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