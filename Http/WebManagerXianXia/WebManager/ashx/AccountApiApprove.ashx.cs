using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace WebManager.ashx
{
    public class AccountApiApprove : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            RightMgr.getInstance().opCheck(RIGHT.APPROVE_API, context.Session, context.Response);

            string op = context.Request.Form["op"];
            string acc = context.Request.Form["acc"];

            Dictionary<string, object> ret = new Dictionary<string, object>();

            GMUser user = (GMUser)context.Session["user"];
            ParamApiApprove param = new ParamApiApprove();
            param.m_apiAcc = acc;
            param.m_isPass = (op == "pass");
            OpRes res = user.doDyop(param, DyOpType.opTypeDyOpApiApprove);
            ret.Add("result", (int)res);
            ret.Add("op", op);
            ret.Add("acc", acc);

            if (res == OpRes.opres_success && param.m_isPass)
            {
                string str = "审批成功," +
                    OpResMgr.getInstance().getResultString(OpRes.op_res_account_info, param.m_resultAcc, param.m_validatedCode);

                ret.Add("resultMsg", str);
            }
            else
            {
                ret.Add("resultMsg", OpResMgr.getInstance().getResultString(res));
            }

            string retStr = BaseJsonSerializer.genJsonStr(ret);

            context.Response.ContentType = "text/plain";
            context.Response.Write(retStr);
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