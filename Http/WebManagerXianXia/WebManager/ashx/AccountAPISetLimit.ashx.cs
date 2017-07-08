using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace WebManager.ashx
{
    // api可设置的最大上限
    public class AccountAPISetLimit : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            RightMgr.getInstance().opCheck("", context.Session, context.Response);

            ParamAPISetLimit param = new ParamAPISetLimit();
            param.m_gameId = Convert.ToInt32(context.Request.Form["gameId"]);
            param.m_apiAcc = Convert.ToString(context.Request.Form["acc"]);
            param.m_setLimit = Convert.ToInt32(context.Request.Form["value"]);
            param.m_roomId = Convert.ToInt32(context.Request.Form["roomId"]);
            param.m_op = Convert.ToInt32(context.Request.Form["op"]);

            GMUser user = (GMUser)context.Session["user"];
            OpRes res = user.doDyop(param, DyOpType.opTypeModifyAPISetLimit);
            string ret = OpResMgr.getInstance().getResultString(res);

            switch (param.m_op)
            {
                case 0:
                    {
                        Dictionary<string, object> data = new Dictionary<string, object>();
                        data.Add("gameId", param.m_gameId);
                        data.Add("roomId", param.m_roomId);
                        data.Add("value", param.m_setLimit);
                        data.Add("result", (int)res);
                        data.Add("resStr", OpResMgr.getInstance().getResultString(res));

                        ret = BaseJsonSerializer.genJsonStr(data);
                    }
                    break;
                case 1:
                    {
                        List<Dictionary<string, object>> dataList =
                            (List<Dictionary<string, object>>)user.getSys<DyOpMgr>(SysType.sysTypeDyOp).getResult(DyOpType.opTypeModifyAPISetLimit);

                        Dictionary<string, object> data = new Dictionary<string, object>();
                        data.Add("limitList", dataList);
                        data.Add("base", DefCC.MONEY_BASE);
                        ret = BaseJsonSerializer.genJsonStr(data);
                    }
                    break;
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