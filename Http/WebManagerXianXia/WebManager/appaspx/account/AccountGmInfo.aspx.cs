using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Text.RegularExpressions;

namespace WebManager.appaspx.account
{
    public partial class AccountGmInfo : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck("", Session, Response);

            if (!IsPostBack)
            {
                string acc = Request.QueryString["acc"];
                if (string.IsNullOrEmpty(acc))
                    return;

                hSelfTitle.InnerText = "个人信息-" + acc;

                GMUser user = (GMUser)Session["user"];
                if (!user.isAPIAcc())
                {
                    trDevKey.Visible = false;
                    trApiPostfix.Visible = false;
                }

                ParamMemberInfo param = new ParamMemberInfo();
                param.m_acc = acc;

                OpRes res = user.doQuery(param, QueryType.queryTypeQueryGmAccountDetail);

                MemberInfoDetail qresult = (MemberInfoDetail)user.getQueryResult(QueryType.queryTypeQueryGmAccountDetail);
                fillInfo(qresult, user);
                gameOnOff(qresult, user);
                rightDispatch(qresult, user);

                if (qresult.m_accType != AccType.ACC_API || (!user.isAdmin() && !user.isAdminSub()))
                {
                    apiLimit.Visible = false;
                }
            }
        }

        protected void fillInfo(MemberInfoDetail info, GMUser user)
        {
            m_owner.Text = info.m_owner;
            m_acc.Text = info.m_acc;
            m_accType.Text = StrName.s_accountType[info.m_accType];
            m_createTime.Text = info.m_createTime;
            m_devKey.Text = info.m_devKey;
            m_postfix.Text = info.m_apiPostfix;

            m_remainMoney.Text = ItemHelp.showMoneyValue(info.m_money).ToString();
            m_state.Text = StrName.s_gmStateName[info.m_state];

            m_id.Text = info.m_gmId.ToString();
           
            m_aliasName.Text = info.m_aliasName;
            linkModifyAliasName.NavigateUrl = string.Format(DefCC.ASPX_MODIFY_ALIASNAME, info.m_acc);

            if (user.m_user == info.m_acc ||
                info.m_owner == user.m_user)
            {
                linkModifyPwd.NavigateUrl = string.Format(DefCC.ASPX_MODIFY_LOGIN_PWD, info.m_acc);
            }
            else
            {
                linkModifyPwd.Visible = false;
            }

            if (info.m_accType == AccType.ACC_API)
            {
                m_apiHome.Text = info.m_home;
                linkModifyHome.NavigateUrl = string.Format(DefCC.ASPX_MODIFY_HOME, info.m_acc, info.m_home);
            }
            else
            {
                trApiHome.Visible = false;
            }

            //////////////////////////////////////////////////////////////////////////
            RangeValidator2.MaximumValue = (ConstDef.MAX_WASH_RATIO * 100).ToString();
            m_agentRatio.Text = (info.m_agentRatio * 100).ToString();
            m_washRatio.Text = (info.m_washRatio * 100).ToString();
        }

        protected void onSave(object sender, EventArgs e)
        {
            /*if (!Regex.IsMatch(m_birthDay.Text, Exp.DATE_TIME))
            {
                m_res.InnerText = OpResMgr.getInstance().getResultString(OpRes.op_res_param_not_valid);
                return;
            }

            SqlUpdateGenerator gen = new SqlUpdateGenerator();
            gen.addField("name", m_name.Text, FieldType.TypeString);
            gen.addField("city", m_city.Text, FieldType.TypeString);
            gen.addField("mobilePhone", m_phone.Text, FieldType.TypeString);
            gen.addField("selfComment", m_comment.Text, FieldType.TypeString);
            gen.addField("sex", m_sex.SelectedIndex, FieldType.TypeNumber);
            gen.addField("birthDay", m_birthDay.Text, FieldType.TypeString);
            string sql = gen.getResultSql(TableName.GM_ACCOUNT, string.Format(" acc='{0}' ", m_acc.Text));

            GMUser user = (GMUser)Session["user"];
            int count = user.sqlDb.executeOp(sql, user.getMySqlServerID(), MySqlDbName.DB_XIANXIA);
            if (count > 0)
            {
                m_res.InnerText = OpResMgr.getInstance().getResultString(OpRes.opres_success);
            }
            else
            {
                m_res.InnerText = OpResMgr.getInstance().getResultString(OpRes.op_res_db_failed);
            }*/
        }

        protected void gameOnOff(MemberInfoDetail info, GMUser user)
        {
            if (!user.isAPIAcc())
            {
                gameOn.Visible = false;
                return;
            }

            string gameOpen = ItemHelp.getReverseGameList(info.m_gameClose);

            for (int i = 0; i < StrName.s_gameList.Count; i++)
            {
                GameInfo gi = StrName.s_gameList[i];

                CheckBox ck = new CheckBox();
                gameList.Controls.Add(ck);
                ck.Text = gi.m_gameName;
                ck.InputAttributes.Add("value", gi.m_gameId.ToString());

                //if (info.m_gameClose.IndexOf(gi.m_gameId.ToString()) >= 0)
                if (gameOpen.IndexOf(gi.m_gameId.ToString()) >= 0)
                {
                    ck.Checked = true;
                }
            }
        }

        // 权限分配
        protected void rightDispatch(MemberInfoDetail info, GMUser user)
        {
            if (!isShowRightOp(info, user))
            {
                rightOp.Visible = false;
                return;
            }

            var allR = RightMap.getDispatchRight(info.m_accType);
            if (allR == null)
                return;

            foreach (var ritem in allR)
            {
                CheckBox ck = new CheckBox();
                rightGroup.Controls.Add(ck);
                ck.Text = ritem.Value.m_rightName;
                ck.InputAttributes.Add("value", ritem.Key);
                if (RightMap.hasRight(ritem.Key, info.m_gmRight))
                {
                    ck.Checked = true;
                }
            }
        }

        bool isShowRightOp(MemberInfoDetail info, GMUser user)
        {
            return DyOpModifyGmRight.canDispatchRight(user, info.m_accType,
                () => { return info.m_owner == user.m_user; });
        }
    }
}