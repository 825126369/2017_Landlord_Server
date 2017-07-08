using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

class FishDelInfo
{
    public FishDelInfo(string data, string viewname)
    {
        m_data = data;
        m_tableName = viewname;
    }

    // 具体名称
    public string m_data;
    // 表名称
    public string m_tableName;
}

public enum StatType
{
    // 充值统计
    statTypeRecharge,

    // 相同订单号的统计
    statTypeSameOrderId,

    // 活跃次数
    statTypeActiveCount,

    // 活跃人数
    statTypeActivePerson,
    
    // vip等级分布
    statTypeVipLevel,
}

class StatMgr : SysBase
{
    // 存储删除统计的相关信息
    private List<FishDelInfo> m_dels = new List<FishDelInfo>();
   
    // 统计实例
    private Dictionary<StatType, PumpBase> m_items = new Dictionary<StatType, PumpBase>();

    public StatMgr()
    {
        m_sysType = SysType.sysTypeStat;
    }

    // 获取待删除列表
    public List<FishDelInfo> getDelList()
    {
        return m_dels;
    }

    // 删除统计数据
    public OpRes delAllStatData(int index, GMUser user)
    {
        if (user == null)
        {
            return OpRes.op_res_failed;
        }
        if (index < 0 || index >= m_dels.Count)
        {
            return OpRes.op_res_failed;
        }

        FishDelInfo info = m_dels[index];
        // 清除指定表中的所有数据
        bool res = DBMgr.getInstance().clearTable(info.m_tableName, user.getDbServerID(), DbName.DB_PUMP);
        if (res) // 添加LOG
        {
            //OpLogMgr.getInstance().addLog(LogType.LOG_TYPE_DEL_STAT_DATA, new ParamDelStatData(info.m_data), user);
            return OpRes.opres_success;
        }
        return OpRes.op_res_failed;
    }

    // 作统计
    public OpRes doStat(object param, StatType statName, GMUser user)
    {
        if (!m_items.ContainsKey(statName))
        {
            LOGW.Info("不存在名称为[{0}]的统计", statName);
            return OpRes.op_res_failed;
        }
        return m_items[statName].doStat(param, user);
    }

    // 返回统计结果
    public object getStatResult(StatType statName)
    {
        if (!m_items.ContainsKey(statName))
        {
            LOGW.Info("不存在名称为[{0}]的统计", statName);
            return null;
        }
        return m_items[statName].getStatResult();
    }

    public PumpBase getPump(StatType statName)
    {
        if (m_items.ContainsKey(statName))
        {
            return m_items[statName];
        }
        return null;
    }

    public override void initSys()
    {
        m_items.Add(StatType.statTypeRecharge, new StatRecharge());
        m_items.Add(StatType.statTypeSameOrderId, new StatSameOrderId());
        m_items.Add(StatType.statTypeActiveCount, new StatActiveCount());
        m_items.Add(StatType.statTypeActivePerson, new StatActivePerson());
        m_items.Add(StatType.statTypeVipLevel, new StatVipLevel());
    }
}

//////////////////////////////////////////////////////////////////////////

public class PumpBase
{
    // 开始统计
    public virtual OpRes doStat(object param, GMUser user) { return OpRes.op_res_failed; }
    // 返回统计结果
    public virtual object getStatResult() { return null; }
}

//////////////////////////////////////////////////////////////////////////

public class ResultStatRecharge
{
    public int m_total = 0;
    // 充值次数
    public int m_rechargeCount = 0;
    // 充值人数
    public int m_rechargePersonNum = 0;
    
    public void reset()
    {
        m_total = 0;
        m_rechargeCount = 0;
        m_rechargePersonNum = 0;
    }
}

// 充值总计
public class StatRecharge : PumpBase
{
    private ResultStatRecharge m_result = new ResultStatRecharge();
    private QueryCondition m_cond = new QueryCondition();

    // 开始统计
    public override OpRes doStat(object param, GMUser user)
    {
        ParamQueryRecharge p = (ParamQueryRecharge)param;
        QueryMgr mgr = user.getSys<QueryMgr>(SysType.sysTypeQuery);
        m_cond.startQuery();
        OpRes res = mgr.makeQuery(param, QueryType.queryTypeRecharge, user, m_cond);
        if (res != OpRes.opres_success)
            return res;

        IMongoQuery imq = m_cond.getImq();
        m_result.reset();
        QueryRecharge rq = mgr.getQuery<QueryRecharge>(QueryType.queryTypeRecharge);
        int serverId = DBMgr.getInstance().getSpecialServerId(DbName.DB_PAYMENT);
        MapReduceResult map_result = DBMgr.getInstance().executeMapReduce(rq.getPlatInfo().m_tableName,
                                                                            serverId,
                                                                            DbName.DB_PAYMENT,
                                                                            imq,
                                                                            MapReduceTable.getMap("recharge"),
                                                                            MapReduceTable.getReduce("recharge"));
        if (map_result != null)
        {
            IEnumerable<BsonDocument> bson = map_result.GetResults();
            foreach (BsonDocument d in bson)
            {
                BsonValue resValue = d["value"];
                m_result.m_total += resValue["total"].ToInt32();
                m_result.m_rechargePersonNum++;
                m_result.m_rechargeCount += resValue["rechargeCount"].ToInt32();
            }
        }
        return OpRes.opres_success;
    }

    public override object getStatResult() { return m_result; }
}

//////////////////////////////////////////////////////////////////////////

public class ResultSameOrderIdItem
{
    public string m_orderId = "";
    
    // 出现次数
    public int m_count = 0;
}

// 相同订单号的统计
public class StatSameOrderId : PumpBase
{
    private List<ResultSameOrderIdItem> m_result = new List<ResultSameOrderIdItem>();
    private QueryCondition m_cond = new QueryCondition();

    // 开始统计
    public override OpRes doStat(object param, GMUser user)
    {
        ParamQueryRecharge p = (ParamQueryRecharge)param;
        QueryMgr mgr = user.getSys<QueryMgr>(SysType.sysTypeQuery);
        m_cond.startQuery();
        OpRes res = mgr.makeQuery(param, QueryType.queryTypeRecharge, user, m_cond);
        if (res != OpRes.opres_success)
            return res;

        IMongoQuery imq = m_cond.getImq();
        QueryDocument qd = (QueryDocument)imq;
        if (!qd.Contains("PayTime"))
        {
            return OpRes.op_res_time_format_error;
        }

        m_result.Clear();
        QueryRecharge rq = mgr.getQuery<QueryRecharge>(QueryType.queryTypeRecharge);
        int serverId = DBMgr.getInstance().getSpecialServerId(DbName.DB_PAYMENT);
        MapReduceResult map_result = DBMgr.getInstance().executeMapReduce(rq.getPlatInfo().m_tableName,
                                                                            serverId,
                                                                            DbName.DB_PAYMENT,
                                                                            imq,
                                                                            MapReduceTable.getMap("sameOrderId"),
                                                                            MapReduceTable.getReduce("sameOrderId"));
        int count = 0;
        if (map_result != null)
        {
            IEnumerable<BsonDocument> bson = map_result.GetResults();
            foreach (BsonDocument d in bson)
            {
                BsonValue resValue = d["value"];
                count = resValue["total"].ToInt32();
                if (count > 1)
                {
                    ResultSameOrderIdItem tmp = new ResultSameOrderIdItem();
                    m_result.Add(tmp);
                    tmp.m_count = count;
                    tmp.m_orderId = Convert.ToString(d["_id"]);
                }
            }
        }
        return OpRes.opres_success;
    }

    public override object getStatResult() { return m_result; }
}

//////////////////////////////////////////////////////////////////////////

public class ResultActive
{
    // 时间
    public string m_time = "";
    // 捕鱼
    public int m_game1 = 0; 
    // 鳄鱼
    public int m_game2 = 0;
    // 押宝
    public int m_game3 = 0;
    // 万人牛牛
    public int m_game4 = 0;
    // 百家乐
    public int m_game5 = 0;
    // 斗地主
    public int m_game6 = 0;
    // 麻将
    public int m_game7 = 0;
    // 梭哈
    public int m_game8 = 0;
}

// 活跃
public class StatActive: PumpBase
{
    private List<ResultActive> m_result = new List<ResultActive>();

    // 开始统计
    public override OpRes doStat(object param, GMUser user)
    {
        string time = (string)param;
        DateTime mint = DateTime.Now, maxt = DateTime.Now;
        bool res = Tool.splitTimeStr(time, ref mint, ref maxt);
        if (!res)
            return OpRes.op_res_time_format_error;

        IMongoQuery imq1 = Query.LT("genTime", BsonValue.Create(maxt));
        IMongoQuery imq2 = Query.GTE("genTime", BsonValue.Create(mint));
        IMongoQuery imq = Query.And(imq1, imq2);

        m_result.Clear();
        MapReduceResult mapResult = getMapReduceResult(imq, user);

        if (mapResult != null)
        {
            IEnumerable<BsonDocument> bson = mapResult.GetResults();
            foreach (BsonDocument d in bson)
            {
                BsonValue resValue = d["value"];
                ResultActive tmp = new ResultActive();
                m_result.Add(tmp);
                tmp.m_time = Convert.ToDateTime(d["_id"]).ToLocalTime().ToString();
                tmp.m_game1 = resValue["game1"].ToInt32();
                tmp.m_game2 = resValue["game2"].ToInt32();
                tmp.m_game3 = resValue["game3"].ToInt32();
                tmp.m_game4 = resValue["game4"].ToInt32();
                tmp.m_game5 = resValue["game5"].ToInt32();
                tmp.m_game6 = resValue["game6"].ToInt32();
                tmp.m_game7 = resValue["game7"].ToInt32();
                tmp.m_game8 = resValue["game8"].ToInt32();
            }
        }
        return OpRes.opres_success;
    }

    public override object getStatResult() { return m_result; }

    protected virtual MapReduceResult getMapReduceResult(IMongoQuery imq, GMUser user)
    {
        return null;
    }
}

// 活跃次数统计
public class StatActiveCount : StatActive
{
    protected override MapReduceResult getMapReduceResult(IMongoQuery imq, GMUser user)
    {
        MapReduceResult mapResult = DBMgr.getInstance().executeMapReduce(TableName.PUMP_ACTIVE_COUNT,
                                                                    user.getDbServerID(),
                                                                    DbName.DB_PUMP,
                                                                    imq,
                                                                    MapReduceTable.getMap("activeCount"),
                                                                    MapReduceTable.getReduce("activeCount"));
        return mapResult;
    }
}

// 活跃人数统计
public class StatActivePerson : StatActive
{
    protected override MapReduceResult getMapReduceResult(IMongoQuery imq, GMUser user)
    {
        MapReduceResult mapResult = DBMgr.getInstance().executeMapReduce(TableName.PUMP_ACTIVE_PERSON,
                                                                    user.getDbServerID(),
                                                                    DbName.DB_PUMP,
                                                                    imq,
                                                                    MapReduceTable.getMap("activePerson"),
                                                                    MapReduceTable.getReduce("activePerson"));
        return mapResult;
    }
}

//////////////////////////////////////////////////////////////////////////

public class StatResultVipLevel
{
    // vip等级分布
    public Dictionary<int, int> m_vipLevel = new Dictionary<int, int>();

    public void reset()
    {
        m_vipLevel.Clear();
    }

    public void addVipLevel(int vip, int count)
    {
        if (m_vipLevel.ContainsKey(vip))
        {
            m_vipLevel[vip] += count;
        }
        else
        {
            m_vipLevel.Add(vip, count);
        }
    }
}

// vip等级的分布统计
public class StatVipLevel : PumpBase
{
    private StatResultVipLevel m_result = new StatResultVipLevel();

    // 开始统计
    public override OpRes doStat(object param, GMUser user)
    {
        m_result.reset();
        MapReduceResult map_result = DBMgr.getInstance().executeMapReduce(TableName.PLAYER_INFO,
                                                                            user.getDbServerID(),
                                                                            DbName.DB_PLAYER,
                                                                            null,
                                                                            MapReduceTable.getMap("vipLevel"),
                                                                            MapReduceTable.getReduce("vipLevel"));
        if (map_result != null)
        {
            IEnumerable<BsonDocument> bson = map_result.GetResults();
            foreach (BsonDocument d in bson)
            {
                try
                {
                    BsonValue resValue = d["value"];
                    int count = resValue["count"].ToInt32();
                    int vip = Convert.ToInt32(d["_id"]);
                    m_result.addVipLevel(vip, count);
                }
                catch (System.Exception ex)
                {
                }
            }
        }
        return OpRes.opres_success;
    }

    public override object getStatResult() { return m_result; }
}
