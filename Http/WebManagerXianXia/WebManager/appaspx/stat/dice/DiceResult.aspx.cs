using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.stat.dice
{
    public partial class DiceResult : RefreshPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck("stat", Session, Response);

            if (!IsPostBack)
            {
                for (int i = 1; i <= 6; i++ )
                {
                    m_result1.Items.Add(new ListItem(i.ToString(), i.ToString()));
                    m_result2.Items.Add(new ListItem(i.ToString(), i.ToString()));
                    m_result3.Items.Add(new ListItem(i.ToString(), i.ToString()));
                }
            }
        }

        protected void onSetResult(object sender, EventArgs e)
        {
            if (IsRefreshed)
                return;

            GMUser user = (GMUser)Session["user"];
            ParamGameResultDice param = new ParamGameResultDice();
            param.m_roomId = 1;
            param.m_gameId = (int)GameId.dice;
            param.m_dice1 = Convert.ToInt32(m_result1.SelectedValue);
            param.m_dice2 = Convert.ToInt32(m_result2.SelectedValue);
            param.m_dice3 = Convert.ToInt32(m_result3.SelectedValue);

            OpRes res = user.doDyop(param, DyOpType.opTypeDyOpGameResult);
            m_res.InnerText = OpResMgr.getInstance().getResultString(res);
        }
    }
}
