using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.account
{
    public partial class AccountPlayerOrder : System.Web.UI.Page
    {
        ViewPlayerOrder m_view = new ViewPlayerOrder();

        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck("", Session, Response);

            if (!IsPostBack)
            {
                GMUser user = (GMUser)Session["user"];
                m_orderState.Items.Add(new ListItem("待处理", OrderState.STATE_WAIT.ToString()));
                m_orderState.Items.Add(new ListItem("已取消", OrderState.STATE_CANCEL.ToString()));
                m_orderState.Items.Add(new ListItem("已完成", OrderState.STATE_FINISH.ToString()));
                m_orderState.Items.Add(new ListItem("已提交", OrderState.STATE_HAS_SUB.ToString()));

                if (user.isAdmin())
                {
                    m_forwardOrder.Visible = false;
                }
                else
                {
                    m_forwardOrder.Checked = (user.m_autoForward == 1);
                }
            }

            queryRecord();
        }

        protected void queryRecord()
        {
            GMUser user = (GMUser)Session["user"];
            ParamPlayerOrderQuery param = new ParamPlayerOrderQuery();
            param.m_orderState = Convert.ToInt32(m_orderState.SelectedValue);
            OpRes res = user.doQuery(param, QueryType.queryTypeQueryPlayerOrder);
            m_view.genTable(m_result, res, user, new EventHandler(onScoreOp));
        }

        protected void onScoreOp(object sender, EventArgs e)
        {
            // if (m_isRefreshed)
            //    return;

            ParamPlayerOrder param = new ParamPlayerOrder();
            Button btn = (Button)sender;
            if (btn.CommandName == "exec")
            {
                param.m_op = OrderOp.OP_EXEC;
            }
            else
            {
                param.m_op = OrderOp.OP_CANCEL;
            }

            //param.m_index = Convert.ToInt32(btn.CommandArgument);
            string[] arr = Tool.split(btn.CommandArgument, '$', StringSplitOptions.RemoveEmptyEntries);
            param.m_orderId = arr[0];
            param.m_playerAcc = arr[1];

            GMUser user = (GMUser)Session["user"];

            OpRes res = user.doDyop(param, DyOpType.opTypeDyOpPlayerOrder);

            Label L = (Label)m_result.FindControl("Label" + btn.ID);
            if (L != null)
            {
                L.Text = OpResMgr.getInstance().getResultString(res);
                L.Style.Clear();
                L.Style.Add("color", "red");

                if (res == OpRes.opres_success)
                {
                    m_result.Rows.Clear();
                    queryRecord();
                }
            }
        }

        protected void onAutoForward(object sender, EventArgs e)
        {
            ParamPlayerOrder param = new ParamPlayerOrder();
            param.m_op = OrderOp.OP_AUTO_FORWARD_TO_SUP;
            if (m_forwardOrder.Checked)
            {
                param.m_isAutoForward = 1;
            }
            else
            {
                param.m_isAutoForward = 0;
            }
            GMUser user = (GMUser)Session["user"];
            OpRes res = user.doDyop(param, DyOpType.opTypeDyOpPlayerOrder);
            if (res != OpRes.opres_success)
            {
                m_forwardOrder.Checked = !m_forwardOrder.Checked;
            }
        }
    }
}