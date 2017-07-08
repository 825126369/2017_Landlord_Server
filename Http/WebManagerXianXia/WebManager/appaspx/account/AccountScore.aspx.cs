using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.account
{
    // 给管理员上分下分，可以越级，直到会员一级
    public partial class AccountScore : RefreshPageBase
    {
        // 搜索具体的账号
        public const int ACTION_SPECIAL_ACC = 0;
        // 通常情况
        public const int ACTION_NORMAL = 1;

        ViewScoreStepByStep m_view = new ViewScoreStepByStep();

        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck(RIGHT.SCORE, Session, Response);

            GMUser user = (GMUser)Session["user"];
            if (user.m_accType == AccType.ACC_API ||
               user.m_accType == AccType.ACC_AGENCY_SUB)
            {
                Server.Transfer(DefCC.ASPX_EMPTY);
            }

            if (!IsPostBack)
            {
                m_type.Items.Add(new ListItem("代理", AccType.ACC_AGENCY.ToString()));
                m_type.Items.Add(new ListItem("会员", AccType.ACC_PLAYER.ToString()));
            }

            int action = getAction();
            switch (action)
            {
                case ACTION_SPECIAL_ACC:
                    {
                        onSearchSpecialAcc();
                    }
                    break;
                case ACTION_NORMAL:
                    {
                        string acc = Request.QueryString["acc"];
                        if (string.IsNullOrEmpty(acc))
                        {
                            initQueryMember(user);
                        }
                        else
                        {
                            queryNextLevelMember(acc, user);
                        }
                    }
                    break;
            }
        }

        protected void initQueryMember(GMUser user)
        {
            ParamMemberInfo param = new ParamMemberInfo();
            param.m_searchDepth = 1;

            URLParam uparam = new URLParam();
            uparam.m_url = DefCC.ASPX_SCORE_GM;

            user.getOpLevelMgr().addRootAcc(param.getRootUser(user), uparam);
            m_levelStr.InnerHtml = user.getOpLevelMgr().getCurLevelStr(param.getRootUser(user));
            OpRes res = user.doQuery(param, QueryType.queryTypeGmAccount);
            m_view.genTable(m_result, res, user, new EventHandler(onScoreOp), param);
        }

        protected void queryNextLevelMember(string acc, GMUser user)
        {
            ParamMemberInfo param = new ParamMemberInfo();
            param.m_searchDepth = 1;
            param.m_creator = acc;
            OpRes res = user.doQuery(param, QueryType.queryTypeGmAccount);
            m_view.genTable(m_result, res, user, new EventHandler(onScoreOp), param);

            m_levelStr.InnerHtml = user.getOpLevelMgr().getCurLevelStr(acc);
        }

        protected void onScoreOp(object sender, EventArgs e)
        {
            if (IsRefreshed)
                return;

            GMUser user = (GMUser)Session["user"];
            ScoreOp op = new ScoreOp(user, true, m_result);
            op.onScoreOp(sender);
        }

        protected void onSearchSpecialAcc()
        {
            GMUser user = (GMUser)Session["user"];
            ParamMemberInfo param = new ParamMemberInfo();
            param.m_searchDepth = 1;
            param.m_acc = m_acc.Text;

            int sel = Convert.ToInt32(m_type.SelectedValue);
            switch (sel)
            {
                case AccType.ACC_AGENCY:
                    {
                        OpRes res = user.doQuery(param, QueryType.queryTypeGmAccount);
                        m_view.genTable(m_result, res, user, new EventHandler(onScoreOp), param);
                    }
                    break;
                case AccType.ACC_PLAYER:
                    {
                        ViewPlayerScoreInfo view = new ViewPlayerScoreInfo(IsRefreshed);
                        OpRes res = user.doQuery(param, QueryType.queryTypePlayerMember);
                        view.genTable(m_result, res, user);
                    }
                    break;
            }
            Label L = (Label)m_result.FindControl("Label" + param.m_acc);
            if (L != null)
            {
                L.Text = "";
            }
        }

        // 返回动作类型
        int getAction()
        {
            string str = Request.Form["__search"];
            if (!string.IsNullOrEmpty(str))
                return ACTION_SPECIAL_ACC;

            str = Request.QueryString["action"];
            if (!string.IsNullOrEmpty(str))
                return ACTION_SPECIAL_ACC;

            return ACTION_NORMAL;
        }
    }
}