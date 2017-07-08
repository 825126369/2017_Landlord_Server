using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;
using System.Text;
using System.Web.Configuration;

namespace XianXiaWebInterface
{
    // 玩家登录游戏，从后台数据库里面取钱
    public partial class AccountFetchMoney : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ParamFetchMoney param = new ParamFetchMoney();
            param.m_acc = Request.QueryString["acc"];

            DyOpFetchMoney dy = new DyOpFetchMoney();
            string retStr = dy.doDyop(param);
            Response.Write(retStr);

            // 哪个玩家账号
           /* string acc = Request.Params["acc"];
            if (string.IsNullOrEmpty(acc))
            {
                returnMsg(RetCode.RET_PARAM_NOT_VALID, "", 0);
                return;
            }

            string cmd =
                string.Format("select money,enable from {0} WHERE acc='{1}'", TableName.PLAYER_ACCOUNT_XIANXIA, acc);

            string sqlServer = WebConfigurationManager.AppSettings["mysql"];

            MySqlDbServer db = new MySqlDbServer(sqlServer);
            Dictionary<string, object> r = db.queryOne(cmd, MySqlDbName.DB_XIANXIA);
            if (r != null)
            {
                bool enable = true;
                if (!(r["enable"] is DBNull))
                {
                    enable = Convert.ToBoolean(r["enable"]);
                }

                long money = Convert.ToInt64(r["money"]);

                if (enable)
                {
                    cmd = string.Format("UPDATE {0} set state={1},lastLoginDate='{2}' where acc='{3}'",
                        TableName.PLAYER_ACCOUNT_XIANXIA,
                        PlayerState.STATE_GAME,
                        DateTime.Now.ToString(ConstDef.DATE_TIME24),
                        acc);
                    db.executeOp(cmd, MySqlDbName.DB_XIANXIA);

                    returnMsg(RetCode.RET_SUCCESS, acc, money);
                }
                else
                {
                    returnMsg(RetCode.RET_ACC_BLOCKED, acc, money);
                }

                return;
            }

            returnMsg(RetCode.RET_NO_PLAYER, acc, 0);*/
        }

        private  void returnMsg(int retCode, string acc, long money)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data["result"] = retCode;

            if (retCode == RetCode.RET_SUCCESS)
            {
                data["acc"] = acc;
                data["money"] = money;
            }

            string jsondata = JsonHelper.ConvertToStr(data);
            Response.Write(Convert.ToBase64String(Encoding.Default.GetBytes(jsondata)));
        }
    }
}