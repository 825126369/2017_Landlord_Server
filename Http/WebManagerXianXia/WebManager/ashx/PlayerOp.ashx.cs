using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace WebManager.ashx
{
    public class PlayerOp : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            RightMgr.getInstance().opCheck("", context.Session, context.Response);

            GMUser user = (GMUser)context.Session["user"];
            DyOpPlayerOp dyop =
                (DyOpPlayerOp)user.getSys<DyOpMgr>(SysType.sysTypeDyOp).getDyOp(DyOpType.opTypePlayerOp);

            Dictionary<string, object> ret = new Dictionary<string, object>();

            string op = context.Request.Form["op"];
            string acc = context.Request.Form["acc"];
            
            switch (op)
            {
                case "modifyName": // 修改别名
                    {
                        string newName = context.Request.Form["param"];
                        OpRes res = dyop.modifyPlayerAliasName(acc, newName, user);
                        ret.Add("resultMsg", OpResMgr.getInstance().getResultString(res));
                    }
                    break;
                case "resetPwd": // 重置密码
                    {
                        string newPwd = context.Request.Form["param"];
                        OpRes res = dyop.resetPlayerPwd(acc, newPwd, user);
                        ret.Add("resultMsg", OpResMgr.getInstance().getResultString(res));
                    }
                    break;
                case "blockAcc": // 停封解封账号
                    {
                        bool isBlock = Convert.ToBoolean(context.Request.Form["param"]);
                        OpRes res = dyop.blockPlayerAcc(acc, isBlock, user);
                        ret.Add("resultMsg", OpResMgr.getInstance().getResultString(res));
                    }
                    break;
                case "kick": // 踢玩家
                    {
                        OpRes res = dyop.kickPlayerAcc(acc, user);
                        string tmpStr = OpResMgr.getInstance().getResultString(res);
                        if (res == OpRes.opres_success)
                        {
                            tmpStr += ",该玩家10分钟以内不能重新登录";
                        }
                        ret.Add("resultMsg", tmpStr);
                    }
                    break;
                case "unlock": // 解锁玩家
                    {
                        OpRes res = dyop.unlockPlayer(acc, user);
                        string tmpStr = OpResMgr.getInstance().getResultString(res);
                        ret.Add("resultMsg", tmpStr);
                    }
                    break;
                case "clearFail": // 清理登录失败次数
                    {
                        OpRes res = dyop.clearPlayerFailCount(acc, user);
                        string tmpStr = OpResMgr.getInstance().getResultString(res);
                        ret.Add("resultMsg", tmpStr);
                    }
                    break;
                case "affectRate": // 影响盈利率
                    {
                        bool isAffect = Convert.ToBoolean(context.Request.Form["param"]);
                        OpRes res = dyop.playerAffectEarnRate(acc, isAffect, user);
                        string tmpStr = "";
                        if (res == OpRes.op_res_player_in_game)
                        {
                            tmpStr = "需要先下线，再点击才会生效";
                        }
                        else
                        {
                            tmpStr = OpResMgr.getInstance().getResultString(res);
                        }
                        ret.Add("resultMsg", tmpStr);
                    }
                    break;
            }

            string str = BaseJsonSerializer.genJsonStr(ret);
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