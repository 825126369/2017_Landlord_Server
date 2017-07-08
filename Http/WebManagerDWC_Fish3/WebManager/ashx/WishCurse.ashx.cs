using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace WebManager.ashx
{
    public class WishCurse : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            RightMgr.getInstance().opCheck(RightDef.OP_WISH_CURSE, context.Session, context.Response);
            ParamAddWishCurse param = new ParamAddWishCurse();
            param.m_opType = Convert.ToInt32(context.Request.Form["op"]);
            param.m_playerId = context.Request.Form["playerId"];
            param.m_rate = context.Request.Form["value"];

            GMUser user = (GMUser)context.Session["user"];
            OpRes res = user.doDyop(param, DyOpType.opTypeWishCurse);
            string str = "";

            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add("op", param.m_opType);

            switch (param.m_opType)
            {
                case 2:
                    {
                        List<ViewAddWishCurseItem> itemList =
                            (List<ViewAddWishCurseItem>)user.getSys<DyOpMgr>(SysType.sysTypeDyOp).getResult(DyOpType.opTypeWishCurse);
                        data.Add("buffList", BaseJsonSerializer.serialize(itemList));
                        str = ItemHelp.genJsonStr(data);
                    }
                    break;
                case 1:
                    {
                        data.Add("result", (int)res);
                        data.Add("playerId", param.m_playerId);
                        str = ItemHelp.genJsonStr(data);
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