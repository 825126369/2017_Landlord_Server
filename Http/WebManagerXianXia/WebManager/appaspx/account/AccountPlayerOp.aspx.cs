using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.account
{
    // 对玩家的相关操作
    public partial class AccountPlayerOp : System.Web.UI.Page
    {
        ViewPlayerInfo m_view = new ViewPlayerInfo();

        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck("", Session, Response);

            GMUser user = (GMUser)Session["user"];
            if (user.isSubAcc())
            {
                Server.Transfer(DefCC.ASPX_EMPTY);
            }

            if (!IsPostBack)
            {
                string acc = Request.QueryString["acc"];
                if (!string.IsNullOrEmpty(acc))
                {
                    m_acc.Text = acc;
                }
            }
           
            m_res.InnerHtml = "";
            m_isAdmin.Text = user.m_accType.ToString();
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];
            m_curMoney.Text = user.m_money.ToString();
        }

        protected void onQueryPlayerInfo(object sender, EventArgs e) 
        {
            GMUser user = (GMUser)Session["user"];
            ParamMemberInfo param = new ParamMemberInfo();
            param.m_acc = m_acc.Text;
            if (!string.IsNullOrEmpty(param.m_acc))
            {
                OpRes res = user.doQuery(param, QueryType.queryTypePlayerMember);
                m_view.genTable(m_result, res, user, null, null);
            }
        }

        protected void onKickPlayer(object sender, EventArgs e) 
        {
            GMUser user = (GMUser)Session["user"];
            ParamPlayerOp param = new ParamPlayerOp();
            param.m_acc = m_acc.Text;
            OpRes res = user.doDyop(param, DyOpType.opTypeKickPlayer);
            m_res.InnerHtml = OpResMgr.getInstance().getResultString(res);
            if (res == OpRes.opres_success)
            {
                m_res.InnerHtml += ",该玩家10分钟以内不能重新登录";
            }
        }

        protected void onUnlockPlayer(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];
            ParamPlayerOp param = new ParamPlayerOp();
            param.m_acc = m_acc.Text;
            OpRes res = user.doDyop(param, DyOpType.opTypeUnlockPlayer);
            m_res.InnerHtml = OpResMgr.getInstance().getResultString(res);
        }

        protected void onClearLoginFailed(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];
            ParamPlayerOp param = new ParamPlayerOp();
            param.m_acc = m_acc.Text;
            OpRes res = user.doDyop(param, DyOpType.opTypeClearLoginFailed);
            m_res.InnerHtml = OpResMgr.getInstance().getResultString(res);
        }

        protected void onResetPlayerPwd(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];

            ParamModifyPwd p = new ParamModifyPwd();
            p.m_account = m_acc1.Text;
            p.m_newPwd = m_pwd1.Text;
            p.m_pwdType = 0; // 更改登录密码
            OpRes res = user.doDyop(p, DyOpType.opTypeModifyPwd);
            m_res1.InnerHtml = OpResMgr.getInstance().getResultString(res);
        }

        protected void onBlockAcc(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];

            ParamBlock param = new ParamBlock();
            param.m_param = m_acc.Text;
            param.m_isBlock = true;
            OpRes res = user.doDyop(param, DyOpType.opTypeBlockAcc);
            m_res.InnerHtml = OpResMgr.getInstance().getResultString(res);
        }

        protected void onUnBlockAcc(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];

            ParamBlock param = new ParamBlock();
            param.m_param = m_acc.Text;
            param.m_isBlock = false;
            OpRes res = user.doDyop(param, DyOpType.opTypeBlockAcc);
            m_res.InnerHtml = OpResMgr.getInstance().getResultString(res);
        }

        protected void onAffectRate(object sender, EventArgs e)
        {
            affectRate(true);
        }

        protected void onUnAffectRate(object sender, EventArgs e)
        {
            affectRate(false);
        }

        void affectRate(bool affect)
        {
            GMUser user = (GMUser)Session["user"];
            ParamPlayerSpecialFlag param = new ParamPlayerSpecialFlag();
            param.m_acc = m_acc.Text;
            param.m_isAffectEarning = affect;
            OpRes res = user.doDyop(param, DyOpType.opTypeSetPlayerSpecialFlag);
            if (res == OpRes.op_res_player_in_game)
            {
                m_res.InnerHtml = "需要先下线，再点击才会生效";
            }
            else
            {
                m_res.InnerHtml = OpResMgr.getInstance().getResultString(res);
            }
        }

        protected void onAddScore(object sender, EventArgs e)
        {
            scoreToPlayer(ScropOpType.ADD_SCORE);
        }

        protected void onDecScore(object sender, EventArgs e)
        {
            scoreToPlayer(ScropOpType.EXTRACT_SCORE);
        }

        void scoreToPlayer(int op)
        {
            ParamScore param = new ParamScore();
            param.m_op = op;
            param.m_toAcc = m_acc.Text;
            param.m_score = m_score.Text;
            param.scoreToPlayer();
            GMUser user = (GMUser)Session["user"];
            OpRes res = user.doDyop(param, DyOpType.opTypeDyOpScore);
            m_scoreRes.InnerText = OpResMgr.getInstance().getResultString(res);
        }
    }
}