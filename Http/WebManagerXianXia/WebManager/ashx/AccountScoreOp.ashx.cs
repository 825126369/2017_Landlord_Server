using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace WebManager.ashx
{
    public class AccountScoreOp : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            RightMgr.getInstance().opCheck(RIGHT.SCORE, context.Session, context.Response);

            GMUser user = (GMUser)context.Session["user"];
            ParamScore param = new ParamScore();
            param.m_op = Convert.ToInt32(context.Request.Form["op"]);
            param.m_toAcc = context.Request.Form["acc"];
            param.m_score = context.Request.Form["param"];
            int targetType = Convert.ToInt32(context.Request.Form["targetType"]);
            if (targetType == AccType.ACC_PLAYER)
            {
                param.scoreToPlayer();
            }
            else
            {
                param.scoreToMgr();
            }

            OpRes res = user.doDyop(param, DyOpType.opTypeDyOpScore);
            string str = OpResMgr.getInstance().getResultString(res);

            long remainScoreFixBug = -1;
            if (targetType == AccType.ACC_SUPER_ADMIN || targetType == AccType.ACC_SUPER_ADMIN_SUB) {
                remainScoreFixBug = 0;
            
            } else {
                remainScoreFixBug = ItemHelp.getRemainMoney(param.m_toAcc, targetType == AccType.ACC_PLAYER, user);
            }

            Dictionary<string, object> ret = new Dictionary<string, object>();
            ret.Add("result", (int)res);
            ret.Add("resultInfo", str);

            string rs = (user.m_accType == AccType.ACC_SUPER_ADMIN ||
                   user.m_accType == AccType.ACC_SUPER_ADMIN_SUB) ? "0" : ItemHelp.toStrByComma(ItemHelp.showMoneyValue(user.m_money));
            ret.Add("remainScoreStr", rs);
            ret.Add("remainScore", ItemHelp.showMoneyValue(user.m_money));
            ret.Add("remainScoreFixBug", ItemHelp.showMoneyValue(remainScoreFixBug));
            str = BaseJsonSerializer.genJsonStr(ret);
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