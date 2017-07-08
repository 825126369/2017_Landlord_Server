using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;

namespace WebManager
{
    public partial class About : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            return;

            MySqlDb d = new MySqlDb();
            string sql = "SELECT * from city";
            MySqlDataReader r = d.startQuery(sql, 0, MySqlDbName.DB_WORLD);

            while (r.Read())
            {
                int id = Convert.ToInt32(r["ID"]);
                string name = Convert.ToString(r["Name"]);
            }
            d.end(0);

            //////////////////////////////////////////////////////////////////////////
            string op1 = "update test set name='eeeeee' where id=1";
            int num = d.executeOp(op1, 0, MySqlDbName.DB_XIANXIA);

            //////////////////////////////////////////////////////////////////////////
            string op2 = "INSERT into test (name) VALUES ('ggggggggggggg')";
            num = d.executeOp(op2, 0, MySqlDbName.DB_XIANXIA);

            //////////////////////////////////////////////////////////////////////////
            string op3 = "DELETE FROM test WHERE id=3";
            num = d.executeOp(op3, 0, MySqlDbName.DB_XIANXIA);
        }
    }
}
