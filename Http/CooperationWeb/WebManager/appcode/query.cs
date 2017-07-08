using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System.Text.RegularExpressions;
using System.Web.Configuration;
using System.Linq;

public enum QueryType
{
    // GM账号
    queryTypeGmAccount,

    // 每日创建
    queryTypeDailyAccountCreate,

    // 每日设备激活
    queryTypeDailyDeviceActivate,

    // 每日充值
    queryTypeDailyRecharge,
}

// 查询管理
public class QueryMgr : SysBase
{
    private Dictionary<QueryType, QueryBase> m_items = new Dictionary<QueryType, QueryBase>();

    public QueryMgr()
    {
        m_sysType = SysType.sysTypeQuery;
    }

    // 作查询
    public OpRes doQuery(object param, QueryType queryType, GMUser user)
    {
        if (!m_items.ContainsKey(queryType))
        {
           // LOGW.Info("不存在名称为[{0}]的查询", queryType);
            return OpRes.op_res_failed;
        }
        return m_items[queryType].doQuery(param, user);
    }

    // 返回查询结果
    public object getQueryResult(QueryType queryType)
    {
        if (!m_items.ContainsKey(queryType))
        {
          //  LOGW.Info("不存在名称为[{0}]的查询", queryType);
            return null;
        }
        return m_items[queryType].getQueryResult();
    }

    // 构成查询条件
    public OpRes makeQuery(object param, QueryType queryType, GMUser user, QueryCondition imq)
    {
        if (!m_items.ContainsKey(queryType))
        {
            return OpRes.op_res_failed;
        }
        return m_items[queryType].makeQuery(param, user, imq);
    }

    public T getQuery<T>(QueryType queryType) where T : QueryBase
    {
        if (m_items.ContainsKey(queryType))
        {
            return (T)m_items[queryType];
        }
        return default(T);
    }

    public override void initSys()
    {
        m_items.Add(QueryType.queryTypeGmAccount, new QueryGMAccount());
        m_items.Add(QueryType.queryTypeDailyAccountCreate, new QueryDailyAccountCreate());
        m_items.Add(QueryType.queryTypeDailyDeviceActivate, new QueryDailyDeviceActivate());
        m_items.Add(QueryType.queryTypeDailyRecharge, new QueryDailyRecharge());
    }
}

///////////////////////////////////////////////////////////////////////////////

public class QueryBase
{
    // 作查询
    public virtual OpRes doQuery(object param, GMUser user) { return OpRes.op_res_failed; }

    // 返回查询结果
    public virtual object getQueryResult() { return null; }

    public virtual OpRes makeQuery(object param, GMUser user, QueryCondition imq) { return OpRes.op_res_failed; }

    // 通过玩家ID，返回域
 /*   public static Dictionary<string, object> getPlayerProperty(int playerId, GMUser user, string[] fields)
    {
        Dictionary<string, object> ret =
            DBMgr.getInstance().getTableData(TableName.PLAYER_INFO, "player_id", playerId, fields, user.getDbServerID(), DbName.DB_PLAYER);
        return ret;
    }

    // 通过账号返回玩家属性
    public static Dictionary<string, object> getPlayerPropertyByAcc(string acc, GMUser user, string[] fields)
    {
        Dictionary<string, object> ret =
                        DBMgr.getInstance().getTableData(TableName.PLAYER_INFO, "account", acc, fields, user.getDbServerID(), DbName.DB_PLAYER);
        return ret;
    }*/
}

///////////////////////////////////////////////////////////////////////////////

public class ParamQueryBase
{
    // 当前查询第几页，以1开始计数
    public int m_curPage;
    // 每页多少条记录
    public int m_countEachPage;
}

public class ParamQuery : ParamQueryBase
{
    // 查询方式
    public int m_way;

    public string m_param = "";

    public string m_time = "";
}

public class QueryCondition
{
    private bool m_isExport = false;
    private List<IMongoQuery> m_queryList = new List<IMongoQuery>();
    private Dictionary<string, object> m_cond = new Dictionary<string, object>();

    public void startQuery()
    { 
        m_isExport = false;
        m_queryList.Clear();
    }

    public void startExport() 
    {
        m_isExport = true;
        m_cond.Clear();
    }

    public bool isExport() { return m_isExport; }

    public void addCond(string name, object c)
    {
        m_cond.Add(name, c);
    }

    public Dictionary<string, object> getCond() { return m_cond; }

    public IMongoQuery getImq() 
    {
        return m_queryList.Count > 0 ? Query.And(m_queryList) : null;
    }

    public void addImq(IMongoQuery imq)
    {
        m_queryList.Add(imq);
    }

    // 根据情况增加查询条件
    public void addQueryCond(string name, object c)
    {
        if (m_isExport)
        {
            m_cond.Add(name, c);
        }
        else
        {
            m_queryList.Add(Query.EQ(name, BsonValue.Create(c)));
        }
    }
}

//////////////////////////////////////////////////////////////////////////

public class GMAccountItem
{
    // 账号
    public string m_user = "";

    // 所在分组
    public string m_type = "";

    public string m_viewChannel = "";

    // 可否查看渠道
    public bool canViewChannel(string channelId)
    {
        if (m_viewChannel == "all")
            return true;

        int index = m_viewChannel.IndexOf(channelId);
        return index >= 0;
    }
}

// 查询当前所有GM账号
public class QueryGMAccount : QueryBase
{
    private List<GMAccountItem> m_result = new List<GMAccountItem>();

    public override OpRes doQuery(object param, GMUser user)
    {
        m_result.Clear();
        List<Dictionary<string, object>> data =
             DBMgr.getInstance().executeQuery(TableName.GM_ACCOUNT, 0, DbName.DB_ACCOUNT);

        if (data == null || data.Count <= 0)
        {
            return OpRes.op_res_not_found_data;
        }

        int i = 0;
        for (i = 0; i < data.Count; i++)
        {
            GMAccountItem tmp = new GMAccountItem();
            tmp.m_user = Convert.ToString(data[i]["user"]);
            tmp.m_type = Convert.ToString(data[i]["type"]);
            tmp.m_viewChannel = Convert.ToString(data[i]["viewChannel"]);
            m_result.Add(tmp);
        }
        return OpRes.opres_success;
    }

    // 返回查询结果
    public override object getQueryResult()
    {
        return m_result;
    }
}

//////////////////////////////////////////////////////////////////////////

public class ParamSearch : ParamQuery
{
    public DateTime m_minT;
    public DateTime m_maxT;

    // 返回字段
    public string[] m_retFields;
}

public class ResultItem
{
    // 时间
    public string m_time = "";

    private Dictionary<string, long> m_value = new Dictionary<string, long>();

    public void addValue(string key, long val)
    {
        m_value.Add(key, val);
    }

    public int getCount()
    {
        return m_value.Count;
    }

    public long getValue(string channelNo)
    {
        if (m_value.ContainsKey(channelNo))
            return m_value[channelNo];
        return 0;
    }
}

// 每日账号创建查询
public class QueryDailyAccountCreate : QueryBase
{
    protected List<ResultItem> m_result = new List<ResultItem>();

    protected QueryCondition m_cond = new QueryCondition();

    public override OpRes doQuery(object param, GMUser user)
    {
        m_result.Clear();
        m_cond.startQuery();
        OpRes res = makeQuery(param, user, m_cond);
        if (res != OpRes.opres_success)
            return res;

        IMongoQuery imq = m_cond.getImq();

        ParamSearch p = (ParamSearch)param;
        return query(p, imq, user, TableName.DAILY_ACCOUNT_CREATE, DbName.DB_ACCOUNT);
    }

    // 返回查询结果
    public override object getQueryResult()
    {
        return m_result;
    }

    public override OpRes makeQuery(object param, GMUser user, QueryCondition queryCond)
    {
        ParamSearch p = (ParamSearch)param;

        IMongoQuery imq1 = Query.LT("date", BsonValue.Create(p.m_maxT));
        IMongoQuery imq2 = Query.GTE("date", BsonValue.Create(p.m_minT));
        queryCond.addImq(Query.And(imq1, imq2));
        return OpRes.opres_success;
    }

    protected OpRes query(ParamSearch param, IMongoQuery imq, GMUser user, string tableName, int dbName)
    {
        user.totalRecord = DBMgr.getInstance().getRecordCount(tableName,
            imq, user.getDbServerID(), dbName);

        List<Dictionary<string, object>> data =
             DBMgr.getInstance().executeQuery(tableName,
             user.getDbServerID(),
             DbName.DB_ACCOUNT, imq,
             (param.m_curPage - 1) * param.m_countEachPage, 
             param.m_countEachPage,
             param.m_retFields,
             "date",
             false);

        if (data == null || data.Count == 0)
        {
            return OpRes.op_res_not_found_data;
        }

        int i = 0;
        for (i = 0; i < data.Count; i++)
        {
            ResultItem tmp = new ResultItem();
            m_result.Add(tmp);

            tmp.m_time = Convert.ToDateTime(data[i]["date"]).ToLocalTime().ToShortDateString();

            foreach (var d in data[i])
            {
                if (d.Key == "date")
                {
                    continue;
                }

                tmp.addValue(d.Key, Convert.ToInt64(d.Value));
            }
        }
        return OpRes.opres_success;
    } 
}

//////////////////////////////////////////////////////////////////////////

// 每日设备激活
public class QueryDailyDeviceActivate : QueryDailyAccountCreate
{
    public override OpRes doQuery(object param, GMUser user)
    {
        m_result.Clear();
        m_cond.startQuery();
        OpRes res = makeQuery(param, user, m_cond);
        if (res != OpRes.opres_success)
            return res;

        IMongoQuery imq = m_cond.getImq();

        ParamSearch p = (ParamSearch)param;
        return query(p, imq, user, TableName.DAILY_DEVICE_ACTIVATE, DbName.DB_ACCOUNT);
    }
}

//////////////////////////////////////////////////////////////////////////

// 每日充值
public class QueryDailyRecharge : QueryDailyAccountCreate
{
    public override OpRes doQuery(object param, GMUser user)
    {
        m_result.Clear();
        m_cond.startQuery();
        OpRes res = makeQuery(param, user, m_cond);
        if (res != OpRes.opres_success)
            return res;

        IMongoQuery imq = m_cond.getImq();

        ParamSearch p = (ParamSearch)param;
        return query(p, imq, user, TableName.DAILY_RECHARGE_SUM, DbName.DB_PAYMENT);
    }
}

