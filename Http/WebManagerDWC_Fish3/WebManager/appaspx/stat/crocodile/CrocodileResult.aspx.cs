using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.stat.crocodile
{
    public partial class CrocodileResult : RefreshPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck(RightDef.CROD_RESULT_CONTROL, Session, Response);

            if (!IsPostBack)
            {
                for (int i = 0; i < StrName.s_roomName.Length; i++)
                {
                    m_room.Items.Add(new ListItem(StrName.s_roomName[i], (i + 1).ToString()));
                }

                Dictionary<int, Crocodile_RateCFGData> data = Crocodile_RateCFG.getInstance().getAllData();
                foreach (var item in data.Values)
                {
                    if (item.m_areaId == 16)
                        continue;

                    m_result.Items.Add(new ListItem(item.m_name, item.m_areaId.ToString()));
                }
            }
        }

        protected void onSetResult(object sender, EventArgs e)
        {
            if (IsRefreshed)
                return;

            GMUser user = (GMUser)Session["user"];
            ParamGameResultCrocodile param = new ParamGameResultCrocodile();
            param.m_roomId = Convert.ToInt32(m_room.SelectedValue);
            param.m_gameId = (int)GameId.crocodile; 
            param.m_result = Convert.ToInt32(m_result.SelectedValue);
            OpRes res = user.doDyop(param, DyOpType.opTypeDyOpGameResult);
            m_res.InnerText = OpResMgr.getInstance().getResultString(res);
        }
    }
}

