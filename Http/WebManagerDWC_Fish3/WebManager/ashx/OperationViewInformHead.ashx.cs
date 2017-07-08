using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace WebManager.ashx
{
    public class OperationViewInformHead : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            RightMgr.getInstance().opCheck(RightDef.OP_INFORM_HEAD, context.Session, context.Response);

            GMUser user = (GMUser)context.Session["user"];

            string str = "";
            int op = Convert.ToInt32(context.Request.Form["op"]);
            switch (op)
            {
                case 0:
                    {
                        ParamInformHead param = new ParamInformHead();
                        param.m_playerList = context.Request.Form["playerList"];
                        param.m_opType = 1;
                        OpRes res = user.doQuery(param, QueryType.queryTypeInformHead);
                        Dictionary<string, object> ret = new Dictionary<string, object>();
                        ret.Add("result", (int)res);
                        ret.Add("playerId", param.m_playerList);
                        str = ItemHelp.genJsonStr(ret);
                    }
                    break;
                case 1:
                    {
                        ParamFreezeHeadInfo p = new ParamFreezeHeadInfo();
                        p.m_playerId = context.Request.Form["playerList"];
                        p.m_freezeDays = "";
                        DyOpMgr mgr = user.getSys<DyOpMgr>(SysType.sysTypeDyOp);
                        OpRes res = mgr.doDyop(p, DyOpType.opTypeFreezeHead, user);
                        str = OpResMgr.getInstance().getResultString(res);
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