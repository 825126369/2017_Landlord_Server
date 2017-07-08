using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Web;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System.Reflection;

class CountMgr
{
    public const string API_ADMIN = "apiAdmin";
    public const string SUPER_ADMIN_SUB = "supAdminSub";

    public static string[] s_countKey = { API_ADMIN, SUPER_ADMIN_SUB };
    public const string SQL = "select * from {0} where appType='{1}' ";

    private static CountMgr s_mgr = null;    
    
    // 计数
    private Dictionary<string, long> m_count = new Dictionary<string, long>();

    private object m_lockObj = new object();

    public static CountMgr getInstance()
    {
        if (s_mgr == null)
        {
            s_mgr = new CountMgr();
            s_mgr.init();
        }
        return s_mgr;
    }

    /* 
     *      返回当前key的计数值
     */
    public long genDyId(string key)
    {
        lock (m_lockObj)
        {
            if (m_count.ContainsKey(key))
            {
                save(key);
                return m_count[key];
            }

            return -1;
        }
    }

    private void save(string key)
    {
        long cur = m_count[key];
        m_count[key] = cur + 1;
        MySqlDb sql = new MySqlDb();

        SqlUpdateGenerator gen = new SqlUpdateGenerator();
        gen.addField("curValue", cur + 1, FieldType.TypeNumber);

        string cmd = gen.getResultSql(TableName.CREATE_CONFIG, string.Format("appType='{0}' ", key));
        sql.executeOp(cmd, 0, MySqlDbName.DB_XIANXIA);
    }

    private void init()
    {
        MySqlDb sql = new MySqlDb();

        for (int i = 0; i < s_countKey.Length; i++)
        {
            string cmd = string.Format(SQL, TableName.CREATE_CONFIG, s_countKey[i]);

            Dictionary<string, object> data = sql.queryOne(cmd, 0, MySqlDbName.DB_XIANXIA);
            if (data == null)
            {
                SqlInsertGenerator gen = new SqlInsertGenerator();
                gen.addField("appType", s_countKey[i], FieldType.TypeString);
                gen.addField("curValue", 0, FieldType.TypeNumber);
                string sqlCmd = gen.getResultSql(TableName.CREATE_CONFIG);
                sql.executeOp(sqlCmd, 0, MySqlDbName.DB_XIANXIA);

                m_count.Add(s_countKey[i], 0);
            }
            else
            {
                string key = Convert.ToString(data["appType"]);
                long val = Convert.ToInt64(data["curValue"]);
                m_count.Add(key, val);
            }
        }
    }
}



