using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.stat.baccarat
{
    public partial class BaccaratResult : RefreshPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck("stat", Session, Response);
            if (!IsPostBack)
            {
                m_result.Items.Add(new ListItem("和", "1"));
                m_result.Items.Add(new ListItem("庄", "2"));
                m_result.Items.Add(new ListItem("闲", "3"));
            }
        }

        protected void onSetResult(object sender, EventArgs e)
        {
            if (IsRefreshed)
                return;

            GMUser user = (GMUser)Session["user"];
            ParamGameResultCrocodile param = new ParamGameResultCrocodile();
            param.m_roomId = 1;
            param.m_gameId = (int)GameId.baccarat;
            param.m_result = Convert.ToInt32(m_result.SelectedValue);
            OpRes res = user.doDyop(param, DyOpType.opTypeDyOpGameResult);
            m_res.InnerText = OpResMgr.getInstance().getResultString(res);
        }
    }
}