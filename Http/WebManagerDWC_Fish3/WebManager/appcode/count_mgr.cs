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
    private static CountMgr s_mgr = null;    
    // 操作日志计数
    public const int OP_LOG_COUNT_KEY = 1;
    // 礼包计数
    public const int GIFT_KEY = 2;
    public const int MAX_KEY = 3;

    // 计数
    private Dictionary<int, long> m_count = new Dictionary<int, long>();

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
     *      add   是否增加该计数
     */
    public long getCurId(int key, bool add = true)
    {
        if (!m_count.ContainsKey(key))
            return -1;

        long cur = m_count[key];
        if (add)
        {
            m_count[key] = cur + 1;
            save(key);
        }
        return cur;
    }

    private void save(int key)
    {
        Dictionary<string, object> data = new Dictionary<string, object>();
        data["CurId"] = m_count[key];
        data["key"] = key;
        DBMgr.getInstance().save(TableName.COUNT_TABLE, data, "key", key, 0, DbName.DB_ACCOUNT);
    }

    private void init()
    {
        for (int i = 1; i < MAX_KEY; i++)
        {
            m_count[i] = 0;
        }

        List<Dictionary<string, object>> tmp = DBMgr.getInstance().executeQuery(TableName.COUNT_TABLE, 0, DbName.DB_ACCOUNT);
        foreach (var item in tmp)
        {
            if (item.ContainsKey("CurId"))
            {
                int key = Convert.ToInt32(item["key"]);
                m_count[key] = Convert.ToInt64(item["CurId"]);
            }
        }
    }
}



