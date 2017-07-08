using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace WebManager.ashx
{
    /// <summary>
    /// AccountMaxBetLimit 的摘要说明
    /// </summary>
    public class AccountMaxBetLimit : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            RightMgr.getInstance().opCheck("", context.Session, context.Response);

            ParamMaxBetLimit param = new ParamMaxBetLimit();
            param.m_gameId = Convert.ToInt32(context.Request.Form["gameId"]);
            if (param.m_gameId == (int)GameId.fishpark)
            {
                param.m_rateList = context.Request.Form["newValue"];
            }
            else
            {
                param.m_newValue = Convert.ToInt32(context.Request.Form["newValue"]);
            }
            param.m_areaId = Convert.ToInt32(context.Request.Form["areaId"]);
            param.m_op = Convert.ToInt32(context.Request.Form["op"]);

            GMUser user = (GMUser)context.Session["user"];
            OpRes res = user.doDyop(param, DyOpType.opTypeModifyMaxBetLimit);
            string ret = "";
            if (param.isModify())
            {
                ret = ((int)res).ToString();
                ret += "@" + OpResMgr.getInstance().getResultString(res);
                ret += "@" + param.m_gameId.ToString();
                ret += "@" + param.m_areaId.ToString();
                if (param.m_gameId == (int)GameId.fishpark)
                {
                    ret += "@" + param.m_rateList;
                }
                else
                {
                    ret += "@" + param.m_newValue.ToString();
                }
            }
            else if (param.isQuery())
            {
                Dictionary<string, object> qresult =
                    (Dictionary<string, object>)user.getSys<DyOpMgr>(SysType.sysTypeDyOp).getDyOp(DyOpType.opTypeModifyMaxBetLimit).getResult();
                ret = BaseJsonSerializer.genJsonStr(qresult);
            }
            else
            {
                ret = OpResMgr.getInstance().getResultString(res);
            }
            context.Response.ContentType = "text/plain";
            context.Response.Write(ret);
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