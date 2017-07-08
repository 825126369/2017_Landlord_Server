using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.service
{
    public partial class ServiceCheckMail : System.Web.UI.Page
    {
        private static string[] s_head = new string[] { "生成时间", "发放方式", "标题", "发送者", "内容", "有效天数", "目标玩家", "道具列表", "全服发放条件--下线时间区间", "全服发放条件--VIP等级区间", "备注", "选择" };
        private string[] m_content = new string[s_head.Length];
        private string m_strList = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck(RightDef.SVR_MAIL_CHECK, Session, Response);

            if (!IsPostBack)
            {
                GMUser user = (GMUser)Session["user"];
                DyOpMgr mgr = user.getSys<DyOpMgr>(SysType.sysTypeDyOp);
                genTable(m_result, user, mgr);
            }
            else
            {
                m_strList = Request["sel"];
                if (m_strList == null)
                {
                    m_strList = "";
                }
            }
        }

        protected void onDelMail(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];
            DyOpMgr mgr = user.getSys<DyOpMgr>(SysType.sysTypeDyOp);

            if (m_strList == "")
            {
                genTable(m_result, user, mgr);
                return;
            }
            DyOpSendMail dyip = (DyOpSendMail)mgr.getDyOp(DyOpType.opTypeSendMail);

            string[] str = Tool.split(m_strList, ',');
            for (int i = 0; i < str.Length; i++)
            {
                dyip.removeCheckMail(user, str[i]);
            }

            genTable(m_result, user, mgr);
        }

        protected void onSendMail(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];
            DyOpMgr mgr = user.getSys<DyOpMgr>(SysType.sysTypeDyOp);

            if (m_strList == "")
            {
                genTable(m_result, user, mgr);
                return;
            }
            DyOpSendMail dyip = (DyOpSendMail)mgr.getDyOp(DyOpType.opTypeSendMail);

            string[] str = Tool.split(m_strList, ',');
            for (int i = 0; i < str.Length; i++)
            {
                ParamSendMail param = new ParamSendMail();
                Dictionary<string, object> data = dyip.getCheckMail(user, str[i]);
                if (data != null)
                {
                    param.m_title = Convert.ToString(data["title"]);
                    param.m_sender = Convert.ToString(data["sender"]);
                    param.m_content = Convert.ToString(data["content"]);
                    param.m_validDay = Convert.ToString(data["validDay"]);
                    param.m_toPlayer = Convert.ToString(data["toPlayer"]);
                    param.m_itemList = Convert.ToString(data["itemList"]);
                    param.m_target = Convert.ToInt32(data["target"]);
                    param.m_condVipLevel = Convert.ToString(data["vipLevel"]);
                    param.m_condLogoutTime = Convert.ToString(data["logOutTime"]);
                    if (data.ContainsKey("comment"))
                    {
                        param.m_comment = Convert.ToString(data["comment"]);
                    }
                    param.m_isCheck = false;

                    OpRes res = dyip.doDyop(param, user);
                    if (param.m_result == "" && res == OpRes.opres_success)
                    {
                        dyip.removeCheckMail(user, str[i]);
                        m_res.InnerHtml = OpResMgr.getInstance().getResultString(res);
                    }
                    else
                    {
                        m_res.InnerHtml = OpResMgr.getInstance().getResultString(res);
                    }
                }
            }

            genTable(m_result, user, mgr);
        }

        private void genTable(Table table, GMUser user, DyOpMgr mgr)
        {
            table.GridLines = GridLines.Both;
            TableRow tr = new TableRow();
            table.Rows.Add(tr);
            TableCell td = null;

            int i = 0, j = 0;
            // 表头
            for (i = 0; i < s_head.Length; i++)
            {
                td = new TableCell();
                tr.Cells.Add(td);
                td.Text = s_head[i];
            }

            List<ParamCheckMail> resultList = new List<ParamCheckMail>();
            DyOpSendMail dyip = (DyOpSendMail)mgr.getDyOp(DyOpType.opTypeSendMail);
            dyip.getCheckMailList(user, resultList);

            for (i = 0; i < resultList.Count; i++)
            {
                tr = new TableRow();
                table.Rows.Add(tr);

                m_content[0] = resultList[i].m_result;
                m_content[1] = resultList[i].m_target == 0 ? "给指定玩家发送" : "全服发送";
                m_content[2] = resultList[i].m_title;
                m_content[3] = resultList[i].m_sender;
                m_content[4] = resultList[i].m_content;
                m_content[5] = resultList[i].m_validDay;
                m_content[6] = resultList[i].m_toPlayer;
                m_content[7] = resultList[i].m_itemList;
                m_content[8] = resultList[i].m_condLogoutTime;
                m_content[9] = resultList[i].m_condVipLevel;
                m_content[10] = resultList[i].m_comment;
                m_content[11] = Tool.getCheckBoxHtml("sel", resultList[i].m_id, false);

                for (j = 0; j < s_head.Length; j++)
                {
                    td = new TableCell();
                    tr.Cells.Add(td);
                    td.Text = m_content[j];
                }
            }
        }
    }
}