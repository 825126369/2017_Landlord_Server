using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace WebManager.ashx
{
    /// <summary>
    /// ShcdControl 的摘要说明
    /// </summary>
    public class ShcdControl : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            RightMgr.getInstance().opCheck("stat", context.Session, context.Response);

            GMUser user = (GMUser)context.Session["user"];

            ParamFishlordParamAdjust p = new ParamFishlordParamAdjust();
            p.m_isReset = false;
            p.m_roomList = "1";
            p.m_gameId = GameId.shcd;
            p.m_op = Convert.ToInt32(context.Request.Form["op"]);
            p.m_expRate = context.Request["level"];

            OpRes res = user.doDyop(p, DyOpType.opTypeGameParamAdjust);
            switch (p.m_op)
            {
                case 1:
                    {
                        string str = OpResMgr.getInstance().getResultString(res);
                        context.Response.ContentType = "text/plain";
                        context.Response.Write(str + "#" + ResultShcdParam.s_levelName[Convert.ToInt32(p.m_expRate)]);
                    }
                    break;
                case 2:
                case 3:
                    {
                        string str = OpResMgr.getInstance().getResultString(res);
                        context.Response.ContentType = "text/plain";
                        context.Response.Write((int)res + "#" + str + "#" + p.m_expRate.ToString());
                    }
                    break;
            }
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