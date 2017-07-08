using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace WebManager.ashx
{
    /// <summary>
    /// ChampionSetting 的摘要说明
    /// </summary>
    public class ChampionSetting : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            RightMgr.getInstance().opCheck(RightDef.OP_WEEK_CHAMPION_SETTING, context.Session, context.Response);

            GMUser user = (GMUser)context.Session["user"];

            ParamGrandPrixWeekChampion p = new ParamGrandPrixWeekChampion();
            p.m_op = Convert.ToInt32(context.Request.Form["op"]);
            p.m_param = Convert.ToString(context.Request.Form["param"]);
            p.m_score = Convert.ToString(context.Request.Form["score"]);

            OpRes res = user.doDyop(p, DyOpType.opTypeWeekChampionControl);
            string str = OpResMgr.getInstance().getResultString(res);

            Dictionary<string, object> retVal = new Dictionary<string, object>();
            retVal.Add("op", p.m_op);
            retVal.Add("param", p.m_param);
            retVal.Add("score", p.m_score);
            retVal.Add("resultStr", str);
            retVal.Add("result", (int)res);

            if (!string.IsNullOrEmpty(p.m_retNickName))
            {
                retVal.Add("nickName", p.m_retNickName);
            }
            context.Response.ContentType = "text/plain";
            context.Response.Write(ItemHelp.genJsonStr(retVal));
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