using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;

public class App
{
    public static void initApp()
    {
        MySqlDb sql = new MySqlDb();
        bool res = sql.keyStrExists(TableName.GM_ACCOUNT, "acc", "admin", 0, MySqlDbName.DB_XIANXIA);
        if (!res)
        {
            /*string sqlCmd = string.Format("INSERT into {0} (acc,pwd,accType,createTime,owner) VALUES ('admin','{1}', {2},'{3}','')",
                TableName.GM_ACCOUNT,
                Tool.getMD5Hash("123456"),
                AccType.ACC_SUPER_ADMIN,
                DateTime.Now.ToString(ConstDef.DATE_TIME24));
            sql.executeOp(sqlCmd, 0, MySqlDbName.DB_XIANXIA);*/

            ValidatedCodeGenerator vg = new ValidatedCodeGenerator();
            vg.CodeSerial = DefCC.CODE_SERIAL;

            SqlInsertGenerator gen = new SqlInsertGenerator();
            gen.addField("acc", "admin", FieldType.TypeString);
            gen.addField("pwd", Tool.getMD5Hash("123456"), FieldType.TypeString);
            gen.addField("accType", AccType.ACC_SUPER_ADMIN, FieldType.TypeNumber);
            gen.addField("createTime", DateTime.Now.ToString(ConstDef.DATE_TIME24), FieldType.TypeString);
            gen.addField("owner", "", FieldType.TypeString);
            gen.addField("validatedCode", "7006", FieldType.TypeString);
            string sqlCmd = gen.getResultSql(TableName.GM_ACCOUNT);
            sql.executeOp(sqlCmd, 0, MySqlDbName.DB_XIANXIA);
        }
    }
}

