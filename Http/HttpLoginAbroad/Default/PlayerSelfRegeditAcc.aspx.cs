using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace HttpLogin.Default
{
    // 玩家游戏内自注册账号
    public partial class PlayerSelfRegeditAcc : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ParamPlayerSelfRegAcc param = new ParamPlayerSelfRegAcc();
            param.m_strData = Request.QueryString["data"];
            param.m_sign = Request.QueryString["sign"];
            param.m_ip = Request.ServerVariables.Get("Remote_Addr").ToString();

            DyOpPlayerSelfRegAcc dy = new DyOpPlayerSelfRegAcc();
            string resStr = dy.doDyop(param);
            Response.Write(resStr);
        }
    }
}