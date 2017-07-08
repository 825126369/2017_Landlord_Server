using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;
using System.Web.Configuration;

namespace XianXiaWebInterface
{
    // 玩家退出游戏，向后台数据库存钱
    public partial class AccountSaveMoney : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ParamSaveMoney param = new ParamSaveMoney();
            param.m_acc = Request.QueryString["acc"];
            param.m_moneyStr = Request.QueryString["money"];
            param.m_exit = Request.QueryString["exitType"];

            DyOpSaveMoney dy = new DyOpSaveMoney();
            string retStr = dy.doDyop(param);
            Response.Write(retStr);

            // 哪个玩家账号
            /*string acc = Request.Params["acc"];
            string m = Request.Params["money"];
            if (string.IsNullOrEmpty(acc) ||
                string.IsNullOrEmpty(m))
            {
                return;
            }

            // 现在的钱
            long money = Convert.ToInt64(m);

            string cmd =
                string.Format("select state,money from {0} WHERE acc='{1}'", TableName.PLAYER_ACCOUNT_XIANXIA, acc);

            string sqlServer = WebConfigurationManager.AppSettings["mysql"];
            MySqlDbServer db = new MySqlDbServer(sqlServer);
            Dictionary<string, object> r = db.queryOne(cmd, MySqlDbName.DB_XIANXIA);
            if (r != null)
            {
                int state = Convert.ToInt32(r["state"]);
                if (state != PlayerState.STATE_GAME)
                {
                    return;
                }

                long oriMoney = Convert.ToInt64(r["money"]);

                cmd = string.Format("UPDATE {0} set state={1},money={2} where acc='{3}'",
                    TableName.PLAYER_ACCOUNT_XIANXIA,
                    PlayerState.STATE_IDLE,
                    money,
                    acc);
                int cnt = db.executeOp(cmd, MySqlDbName.DB_XIANXIA);
                if (cnt > 0)
                {
                    addGameLog(db, acc, oriMoney, money);
                    Response.Write("ok");
                }
            }  */ 
        }

        // 添加进出日志
        private void addGameLog(MySqlDbServer db, string acc, long oriMoney, long curMoney)
        {
            string cmd = string.Format(SqlCmdStr.SQL_ADD_GAME_LOG,
                                        TableName.PLAYER_GAME_SCORE,
                                        acc,
                                        oriMoney,
                                        curMoney,
                                        DateTime.Now.ToString(ConstDef.DATE_TIME24));

            db.executeOp(cmd, MySqlDbName.DB_XIANXIA);
        }
    }
}