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
        //m_items.Add(StatType.statTypeRecharge, new StatRecharge());
        //m_items.Add(StatType.statTypeSameOrderId, new StatSameOrderId());
        //m_items.Add(StatType.statTypeActiveCount, new StatActiveCount());
        //m_items.Add(StatType.statTypeActivePerson, new StatActivePerson());
        //m_items.Add(StatType.statTypeVipLevel, new StatVipLevel());

       // m_items.Add(StatType.statTypeSeller, new StatSeller());
        //m_items.Add(StatType.statTypeSellerAdmin, new StatSellerAdmin());
       // m_items.Add(StatType.statTypePlayer, new StatPlayer());
       // m_items.Add(StatType.statTypeSellerStep, new StatSellerStep());
        m_items.Add(StatType.statTypeWinLose, new StatPlayerWinLose());
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

/*public class ResultStatRecharge
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
}*/

//////////////////////////////////////////////////////////////////////////

/*public class ResultSameOrderIdItem
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
}*/

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
        IMongoQuery imqTime = null;
        string time = (string)param;
        if (!string.IsNullOrEmpty(time))
        {
            DateTime mint = DateTime.Now, maxt = DateTime.Now;
            bool res = Tool.splitTimeStr(time, ref mint, ref maxt);
            if (!res)
                return OpRes.op_res_time_format_error;

            m_result.reset();

            IMongoQuery imq1 = Query.LT("create_time", BsonValue.Create(maxt));
            IMongoQuery imq2 = Query.GTE("create_time", BsonValue.Create(mint));
            imqTime = Query.And(imq1, imq2);
        }
        
        m_result.reset();
        MapReduceResult map_result = DBMgr.getInstance().executeMapReduce(TableName.PLAYER_INFO,
                                                                            user.getDbServerID(),
                                                                            DbName.DB_PLAYER,
                                                                            imqTime,
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

//////////////////////////////////////////////////////////////////////////

public enum StatSellerType
{
    stat_seller_type_day,     // 日
    stat_seller_type_month,   // 月
    stat_seller_type_mgr,     // 管理员
    stat_seller_type_seller,  // 售货亭
}

public class ParamStatSeller : ParamMemberInfo
{
    public string m_sellerMgr = "";
    public int m_statType;
}

public class StatResultSellerItem
{
    public string m_time;
    public long m_addScore;
    public long m_desScore;
    public int m_addScoreCount;
    public int m_desScoreCount;
    public string m_seller = "";
}

public class StatResultSeller
{
    // 结果类型
    public int m_resultType;  

    public List<StatResultSellerItem> m_items = new List<StatResultSellerItem>();

    protected Dictionary<string, StatResultSellerItem> m_indexs =
        new Dictionary<string, StatResultSellerItem>();

    public void reset()
    {
        m_items.Clear();
        m_indexs.Clear();
    }

    // 按日期
    public void addItemByTime(string t, int cnt, long sumScore, int opType)
    {
        StatResultSellerItem item = null;
        if (m_indexs.ContainsKey(t))
        {
            item = m_indexs[t];
        }

        if (item == null)
        {
            item = new StatResultSellerItem();
            item.m_time = t;
            m_items.Add(item);
            m_indexs.Add(t, item);
        }

        if (item != null)
        {
            if (opType == 0) // 上分
            {
                item.m_addScore += sumScore;
                item.m_addScoreCount += cnt;
            }
            else // 下分
            {
                item.m_desScore += sumScore;
                item.m_desScoreCount += cnt;
            }
        }
    }

    // 按售货亭
    public StatResultSellerItem addItemBySeller(string seller, int cnt, long sumScore, int opType)
    {
        StatResultSellerItem item = null;
        for (int i = 0; i < m_items.Count; i++)
        {
            if (seller == m_items[i].m_seller)
            {
                item = m_items[i];
                break;
            }
        }

        if (item == null)
        {
            item = new StatResultSellerItem();
            item.m_seller = seller;
            m_items.Add(item);
        }

        if (item != null)
        {
            if (opType == 0) // 上分
            {
                item.m_addScore += sumScore;
                item.m_addScoreCount += cnt;
            }
            else // 下分
            {
                item.m_desScore += sumScore;
                item.m_desScoreCount += cnt;
            }
        }
        return item;
    }

    // 按日期
    public void addItemByTimePlayer(string t, int cnt, long enterMoney, int exitMoney)
    {
        StatResultSellerItem item = null;
        for (int i = 0; i < m_items.Count; i++)
        {
            if (t == m_items[i].m_time)
            {
                item = m_items[i];
                break;
            }
        }

        if (item == null)
        {
            item = new StatResultSellerItem();
            item.m_time = t;
            m_items.Add(item);
        }

        if (item != null)
        {
            item.m_addScore = enterMoney;
            item.m_desScore = exitMoney;
        }
    }
}

// 售货亭统计，售货亭账号查询
/*public class StatSeller : PumpBase
{
    private StatResultSeller m_result = new StatResultSeller();

    // 开始统计
    public override OpRes doStat(object param, GMUser user)
    {
        ParamStatSeller p = (ParamStatSeller)param;
        DateTime mint = DateTime.Now, maxt = mint;
        bool res = Tool.splitTimeStr(p.m_timeRange, ref mint, ref maxt);
        if (!res)
            return OpRes.op_res_time_format_error;

        m_result.reset();
        string userCond = "";
        if (user.m_accType == AccType.ACC_SELLER)
        {
            userCond = user.m_user;
        }
        else if (user.m_accType == AccType.ACC_SELLER_SUB)
        {
            userCond = user.m_owner;
        }

        string cmd = "";
        statData fun = null;
        switch (p.m_statType)
        {
            case (int)StatSellerType.stat_seller_type_day:
                {
                    // 售货亭对下级管理员的存款及提款
                    cmd = string.Format(SqlStr.STR_FTM1, "%Y-%m-%d",
                        TableName.GM_SCORE,
                        mint,
                        maxt,
                        string.Format(" and opSrc='{0}'", userCond)
                        );

                    fun = doStatTypeDay;
                }
                break;
            case (int)StatSellerType.stat_seller_type_month:
                {
                    // 售货亭对下级管理员的存款及提款
                    cmd = string.Format(SqlStr.STR_FTM1, "%Y-%m",
                        TableName.GM_SCORE,
                        mint,
                        maxt,
                        string.Format(" and opSrc='{0}'", userCond)
                        );

                    fun = doStatTypeDay;
                }
                break;
            case (int)StatSellerType.stat_seller_type_mgr:
                {
                    cmd = string.Format(SqlStr.SQL_SELLER_STAT_ADMIN,
                       TableName.GM_SCORE,
                       mint,
                       maxt,
                       TableName.GM_ACCOUNT,
                       userCond,
                       AccType.ACC_SELLER_ADMIN);

                    fun = doStatTypeSellerAdmin;
                }
                break;
            case (int)StatSellerType.stat_seller_type_seller:
                {
                    cmd = string.Format(SqlStr.STR_FTM2, 
                        TableName.GM_SCORE,
                        userCond,
                        mint,
                        maxt);
                    fun = doStatTypeSeller;
                }
                break;
        }

        List<Dictionary<string, object>> dataList
            = user.sqlDb.queryList(cmd, user.getDbServerID(), MySqlDbName.DB_XIANXIA);

        if (fun != null)
        {
            fun(dataList);
        }
        
        return OpRes.opres_success;
    }

    public override object getStatResult() { return m_result; }

    // 按天统计
    private void doStatTypeDay(List<Dictionary<string, object>> dataList)
    {
        if (dataList == null)
            return;

        m_result.m_resultType = (int)StatSellerType.stat_seller_type_day;
        for (int i = 0; i < dataList.Count; i++)
        {
            string dt = Convert.ToString(dataList[i]["dtime"]);
            int cnt = Convert.ToInt32(dataList[i]["cnt"]);
            long sum = Convert.ToInt64(dataList[i]["sum"]);
            int opType = Convert.ToInt32(dataList[i]["opType"]);
            m_result.addItemByTime(dt, cnt, sum, opType);
        }
    }

    // 按售货亭统计
    private void doStatTypeSeller(List<Dictionary<string, object>> dataList)
    {
        if (dataList == null)
            return;

        m_result.m_resultType = (int)StatSellerType.stat_seller_type_seller;
        for (int i = 0; i < dataList.Count; i++)
        {
            string seller = Convert.ToString(dataList[i]["opSrc"]);
            int cnt = Convert.ToInt32(dataList[i]["cnt"]);
            long sum = Convert.ToInt64(dataList[i]["sum"]);
            int opType = Convert.ToInt32(dataList[i]["opType"]);
            m_result.addItemBySeller(seller, cnt, sum, opType);
        }
    }

    // 按售货亭管理统计
    private void doStatTypeSellerAdmin(List<Dictionary<string, object>> dataList)
    {
        if (dataList == null)
            return;

        m_result.m_resultType = (int)StatSellerType.stat_seller_type_seller;
        for (int i = 0; i < dataList.Count; i++)
        {
            string seller = Convert.ToString(dataList[i]["opDst"]);
            int cnt = Convert.ToInt32(dataList[i]["cnt"]);
            long sum = Convert.ToInt64(dataList[i]["sum"]);
            int opType = Convert.ToInt32(dataList[i]["opType"]);
            m_result.addItemBySeller(seller, cnt, sum, opType);
        }
    }
}*/

//////////////////////////////////////////////////////////////////////////

// 售货亭管理员统计，只统计对他所创建的玩家的存款，提款情况
public class StatSellerAdmin : PumpBase
{
    private StatResultSeller m_result = new StatResultSeller();
    private CommonSearchCmdBase m_generator = new CommonSearchCmdBase(CommonSearchCmdBase.SEARCH_TYPE_PLAYER_SCORE);

    // 开始统计
    public override OpRes doStat(object param, GMUser user)
    {
        ParamStatSeller p = (ParamStatSeller)param;
        string cond = "";
        OpRes res = m_generator.genSearchSql(p, user, ref cond);
        if (res != OpRes.opres_success)
            return res;

        m_result.reset();

        string cmd = "";
        statData fun = null;
        switch (p.m_statType)
        {
            case (int)StatSellerType.stat_seller_type_day:
                {
                    // 售货亭对下级管理员的存款及提款
                   // cmd = string.Format(SqlStrCMD.STR_FTM1, "%Y-%m-%d",
                   //     TableName.PLAYER_SCORE,
                    //    cond);

                    fun = doStatTypeDay;
                }
                break;
            case (int)StatSellerType.stat_seller_type_month:
                {
                    // 售货亭对下级管理员的存款及提款
                  //  cmd = string.Format(SqlStrCMD.STR_FTM1, "%Y-%m",
                   //     TableName.PLAYER_SCORE,
                    //    cond);

                    fun = doStatTypeDay;
                }
                break;
        }

        List<Dictionary<string, object>> dataList
            = user.sqlDb.queryList(cmd, user.getMySqlServerID(), MySqlDbName.DB_XIANXIA);

        if (fun != null)
        {
            fun(dataList);
        }

        return OpRes.opres_success;
    }

    public override object getStatResult() { return m_result; }

    // 按天统计
    private void doStatTypeDay(List<Dictionary<string, object>> dataList)
    {
        if (dataList == null)
            return;

        m_result.m_resultType = (int)StatSellerType.stat_seller_type_day;
        for (int i = 0; i < dataList.Count; i++)
        {
            string dt = Convert.ToString(dataList[i]["dtime"]);
            int cnt = Convert.ToInt32(dataList[i]["cnt"]);
            long sum = Convert.ToInt64(dataList[i]["sum"]);
            int opType = Convert.ToInt32(dataList[i]["opType"]);
            m_result.addItemByTime(dt, cnt, sum, opType);
        }
    }
}

//////////////////////////////////////////////////////////////////////////

public class ParamStatSellerStep
{
    public string m_time;
    public string m_creator;
    public int m_statType;  // 统计账号的
}

// 逐级统计玩家存款，提款情况
public class StatSellerStep : PumpBase
{
    private StatResultSeller m_result = new StatResultSeller();
    private CommonSearchCmdBase m_generator = new CommonSearchCmdBase(CommonSearchCmdBase.SEARCH_TYPE_PLAYER_SCORE);

    // 开始统计
    public override OpRes doStat(object param, GMUser user)
    {
        ParamStatSellerStep p = (ParamStatSellerStep)param;
        ParamMemberInfo paramInfo = new ParamMemberInfo();
        if (p.m_statType == 1)
        {
            paramInfo.m_acc = p.m_creator;
        }
        else
        {
            paramInfo.m_creator = p.m_creator;
        }
        
        paramInfo.m_searchDepth = 1;
        m_result.reset();

        OpRes res = user.doQuery(paramInfo, QueryType.queryTypeGmAccount);
        if (res != OpRes.opres_success)
            return res;

        List<MemberInfo> qresult = (List<MemberInfo>)user.getQueryResult(QueryType.queryTypeGmAccount);
        for (int i = 0; i < qresult.Count; i++)
        {
            MemberInfo minfo = qresult[i];
            res = statBySeller(minfo.m_acc, p.m_time, user);
            if (res != OpRes.opres_success)
                return res;
        }
        return OpRes.opres_success;
    }

    public override object getStatResult() { return m_result; }

    private OpRes statBySeller(string creator, string time, GMUser user)
    {
        string cond = "";
        ParamMemberInfo p = new ParamMemberInfo();
        p.m_creator = creator;
        p.m_time = time;
        p.m_searchDepth = 0;
        OpRes res = m_generator.genSearchSql(p, user, ref cond);
        if (res != OpRes.opres_success)
            return res;

        string cmd = "";

        // 售货亭对下级管理员的存款及提款
        cmd = string.Format(SqlStrCMD.SQL_STAT_STEP,
            //TableName.PLAYER_SCORE,
            cond);

        List<Dictionary<string, object>> dataList
            = user.sqlDb.queryList(cmd, user.getMySqlServerID(), MySqlDbName.DB_XIANXIA);

        doStatTypeDay(dataList, creator, time);

        return OpRes.opres_success;
    }

    // 按天统计
    private void doStatTypeDay(List<Dictionary<string, object>> dataList, string seller, string time)
    {
        if (dataList == null)
            return;

        if (dataList.Count == 0)
        {
            m_result.addItemBySeller(seller, 0, 0, 0);
            return;
        }

        for (int i = 0; i < dataList.Count; i++)
        {
            int cnt = Convert.ToInt32(dataList[i]["cnt"]);
            long sum = Convert.ToInt64(dataList[i]["sum"]);
            int opType = Convert.ToInt32(dataList[i]["opType"]);
            StatResultSellerItem pTime = m_result.addItemBySeller(seller, cnt, sum, opType);
            if (pTime != null)
            {
                pTime.m_time = time;
            }
        }
    }
}

//////////////////////////////////////////////////////////////////////////

public class ParamStatPlayer : ParamStatSeller
{
    public int m_moneyType;
}

// 玩家数据的统计
public class StatPlayer : PumpBase
{
    // {1}player_game_score, {2}player_account
    public const string CMD_STAT_PLAYER = "SELECT date_format(genTime,'{0}' ) as dtime, COUNT(*) as cnt, SUM(enterMoney) as enterSum, SUM(exitMoney) as exitSum " +
                                            " from {1},{2}" +
                                            " where genTime >= '{3}' and genTime < '{4}' and {1}.acc={2}.acc and {2}.moneyType={5} " +
                                            " GROUP BY dtime ORDER BY genTime";

    private StatResultSeller m_result = new StatResultSeller();

    // 开始统计
    public override OpRes doStat(object param, GMUser user)
    {
        ParamStatPlayer p = (ParamStatPlayer)param;
        DateTime mint = DateTime.Now, maxt = mint;
        bool res = Tool.splitTimeStr(p.m_time, ref mint, ref maxt);
        if (!res)
            return OpRes.op_res_time_format_error;

        m_result.reset();

        string cmd = "";
        statData fun = null;
        switch (p.m_statType)
        {
            case (int)StatSellerType.stat_seller_type_day:
                {
                    // 售货亭对下级管理员的存款及提款
                    cmd = string.Format(CMD_STAT_PLAYER, "%Y-%m-%d",
                                        TableName.PLAYER_GAME_SCORE,
                                        TableName.PLAYER_ACCOUNT_XIANXIA,
                                        mint,
                                        maxt,
                                        p.m_moneyType);

                    fun = doStatTypeDay;
                }
                break;
            case (int)StatSellerType.stat_seller_type_month:
                {
                    cmd = string.Format(CMD_STAT_PLAYER, "%Y-%m",
                                        TableName.PLAYER_GAME_SCORE,
                                        TableName.PLAYER_ACCOUNT_XIANXIA,
                                        mint,
                                        maxt,
                                        p.m_moneyType);

                    fun = doStatTypeDay;
                }
                break;
        }

        List<Dictionary<string, object>> dataList
            = user.sqlDb.queryList(cmd, user.getMySqlServerID(), MySqlDbName.DB_XIANXIA);

        if (fun != null)
        {
            fun(dataList);
        }

        return OpRes.opres_success;
    }

    public override object getStatResult() { return m_result; }

    // 按天统计
    private void doStatTypeDay(List<Dictionary<string, object>> dataList)
    {
        if (dataList == null)
            return;

        m_result.m_resultType = (int)StatSellerType.stat_seller_type_day;
        for (int i = 0; i < dataList.Count; i++)
        {
            string dt = Convert.ToString(dataList[i]["dtime"]);
            int cnt = Convert.ToInt32(dataList[i]["cnt"]);
            long enterSum = Convert.ToInt64(dataList[i]["enterSum"]);
            int exitSum = Convert.ToInt32(dataList[i]["exitSum"]);
            m_result.addItemByTimePlayer(dt, cnt, enterSum, exitSum);
        }
    }
}

//////////////////////////////////////////////////////////////////////////
public class StatResultWinLoseItem // 输赢统计结果项
{
    public int m_accType;
    public string m_acc;            // 账号，具体玩家 or 子代理
    public long m_totalOutlay;      // 总投注
    public long m_totalIncome;      // 总返还
    public long m_totalWashCount;   // 洗码量

    public double m_washRatio;         // 洗码比
    public double m_agentRatio;        // 代理占成

    public DateTime m_time;

    public void reset()
    {
        m_accType = -1;
        m_acc = "";
        m_totalOutlay = 0;
        m_totalIncome = 0;
        m_totalWashCount = 0;
    }

    // 返回洗码比
    public double getWashRatio() 
    {
        return m_washRatio;
    }

    // 返回洗码佣金
    public double getWashCommission()
    {
        double w = ItemHelp.showMoneyValue(m_totalWashCount);
        return w * getWashRatio();
    }

    // 输赢金额，总盈利
    public double getWinLoseMoney()
    {
        double delta = ItemHelp.showMoneyValue(m_totalOutlay - m_totalIncome);
        return delta;
    }

    // 返回总金额
    public double getTotalMoney()
    {
        double delta = getWinLoseMoney();
        return delta + getWashCommission();
    }

    // 返回代理占成
    public double getAgentRatio()
    {
        return m_agentRatio;
    }

    // 返回代理交公司
    public double getAgentHandInCompany()
    {
        return getTotalMoney() * (1 - getAgentRatio());
    }

    // 返回交公司投注
    public double getOutlayHandInCompany()
    {
        return ItemHelp.showMoneyValue(m_totalOutlay);
    }

    // 返回交公司洗码量
    public double getWashCountHandInCompany()
    {
        return ItemHelp.showMoneyValue(m_totalWashCount);
    }

    // 公司获利比例 = 代理交公司 / 交公司投注金额
    public static double getCompanyProfitRatio(double outlayHandInCompany,
                                               double agentHandInCompany)
    {
        if (outlayHandInCompany == 0.0)
            return 0;

        return Math.Round(agentHandInCompany / outlayHandInCompany, 6);
    }

    // 返回公司获利比
    public double getCompanyProfitRatio()
    {
        return getCompanyProfitRatio(getOutlayHandInCompany(), getAgentHandInCompany());
    }

    public string getCompanyProfitRatioStr()
    {
        return getCompanyProfitRatioStr(getOutlayHandInCompany(), getAgentHandInCompany());
    }

    public static string getCompanyProfitRatioStr(double outlayHandInCompany,
                                                  double agentHandInCompany)
    {
        double r = getCompanyProfitRatio(outlayHandInCompany, agentHandInCompany);
        if (Math.Abs(r) < 0.000001)
            return "0";

        return (r * 100).ToString() + "%";
    }
}

public class StatResultWinLose // 输赢统计结果
{
    // 子代理下属玩家总和
    public StatResultWinLoseItem m_subAgent = new StatResultWinLoseItem();
    // 直属玩家总和
    public StatResultWinLoseItem m_directlyPlayer = new StatResultWinLoseItem();

    public int m_detailType; // 0明细中列的是直属代理  1明细中列的是直属玩家
    // 详情明细列表
    public List<StatResultWinLoseItem> m_detail = new List<StatResultWinLoseItem>();

    public void reset()
    {
        m_subAgent.reset();
        m_directlyPlayer.reset();
        m_detail.Clear();
    }

    public StatResultWinLoseItem addItem(Dictionary<string, object> data, string acc, int accType)
    {
        if (data == null)
            return null;

        StatResultWinLoseItem item = new StatResultWinLoseItem();
        m_detail.Add(item);

        setItem(data, acc, accType, item);
        return item;
    }

    public void setSubAgentItem(Dictionary<string, object> data, string acc, int accType)
    {
        setItem(data, acc, accType, m_subAgent);
    }

    public void setDirectlyPlayerItem(Dictionary<string, object> data, string acc, int accType)
    {
        setItem(data, acc, accType, m_directlyPlayer);
    }

    public bool isDetailSubAgent()
    {
        return m_detailType == 0;
    }

    public StatResultWinLoseItem getWinLoseItem(int index)
    {
        if (index == 0)
            return m_subAgent;

        return m_directlyPlayer;
    }

    public void finish(ParamWinLose param, GMUser user) 
    {
        m_detailType = param.m_detailType;
        m_subAgent.m_acc = param.m_creator;
        m_directlyPlayer.m_acc = param.m_creator;
        m_subAgent.m_agentRatio = m_directlyPlayer.m_agentRatio = user.m_agentRatio;
        m_subAgent.m_washRatio = m_directlyPlayer.m_washRatio = user.m_washRatio;

        if (isDetailSubAgent())
        {
            foreach (var item in m_detail)
            {
                m_subAgent.m_totalIncome += item.m_totalIncome;
                m_subAgent.m_totalOutlay += item.m_totalOutlay;
                m_subAgent.m_totalWashCount += item.m_totalWashCount;
            }
        }
        else
        {
            foreach (var item in m_detail)
            {
                m_directlyPlayer.m_totalIncome += item.m_totalIncome;
                m_directlyPlayer.m_totalOutlay += item.m_totalOutlay;
                m_directlyPlayer.m_totalWashCount += item.m_totalWashCount;
            }
        }
    }

    private void setItem(Dictionary<string, object> data, 
                         string acc, 
                         int accType,
                         StatResultWinLoseItem item)
    {
        if (data == null)
            return;

        item.m_acc = acc;
        item.m_accType = accType;
        if (!(data["playerIncomeSum"] is DBNull))
        {
            item.m_totalIncome = Convert.ToInt64(data["playerIncomeSum"]);
        }
        if (!(data["playerOutlaySum"] is DBNull))
        {
            item.m_totalOutlay = Convert.ToInt64(data["playerOutlaySum"]);
        }
        if (!(data["washCountSum"] is DBNull))
        {
            item.m_totalWashCount = Convert.ToInt64(data["washCountSum"]);
        }

        if (data.ContainsKey("date"))
        {
            if (!(data["date"] is DBNull))
            {
                item.m_time = Convert.ToDateTime(data["date"]);
            }
        }
    }
}

public class ParamWinLose
{
    public string m_time;
    public string m_creator;
    public int m_detailType;

    public bool m_creatorIsSubAcc = false;

    // 某个玩家的具体记录
    public string m_playerAcc;

    public void setDetailPlayer()
    {
        m_detailType = 1;
    }

    public void setDetailSubAgent()
    {
        m_detailType = 0;
    }

    public bool isDetailSubAgent()
    {
        return m_detailType == 0;
    }

    // 是否统计某个具体的玩家
    public bool isStatOnePlayer()
    {
        return !string.IsNullOrEmpty(m_playerAcc);
    }
}

// 玩家输赢情况统计。玩家总投注，总返还，总盈利，洗码量
public class StatPlayerWinLose : PumpBase
{
    private StatResultWinLose m_result = new StatResultWinLose();
    private CommandWinLoseGenerator m_generator = new CommandWinLoseGenerator();

    // 开始统计
    public override OpRes doStat(object param, GMUser user)
    {
        m_result.reset();
        OpRes res = OpRes.op_res_param_not_valid;
        ParamWinLose p = (ParamWinLose)param;

        bool canEmpty = m_generator.timeCanEmpty();
        if (!canEmpty)
        {
            if (string.IsNullOrEmpty(p.m_time))
                return OpRes.op_res_time_format_error;
        }

        if (p.isStatOnePlayer())
        {
            statOnePlayer(user, p);
            return OpRes.opres_success;
        }

        ParamMemberInfo paramInfo = new ParamMemberInfo();
        paramInfo.m_creator = p.m_creator;
        paramInfo.m_searchDepth = 1;

        if (p.isDetailSubAgent()) // 详细列表中列的是子代理
        {
            // 搜索指定创建者的所有直属下线
            res = user.doQuery(paramInfo, QueryType.queryTypeGmAccount);
            if (res != OpRes.opres_success)
                return res;

            List<MemberInfo> qresult = (List<MemberInfo>)user.getQueryResult(QueryType.queryTypeGmAccount);
            for (int i = 0; i < qresult.Count; i++)
            {
                MemberInfo minfo = qresult[i];
               // user.getOpLevelMgr().addSub(p.m_creator, minfo.m_acc);
                addSubLevel(user, p.m_creator, minfo.m_acc, p);

                res = statSubAgent(minfo, p.m_time, user);
                if (res != OpRes.opres_success)
                    return res;
            }

            res = statDirectlyPlayerSum(p.m_creator, p.m_time, user);
        }
        else // 详细列表中列的是直属玩家
        {
            // 搜索指定创建者的所有直属下线
            res = user.doQuery(paramInfo, QueryType.queryTypePlayerMember);
            if (res != OpRes.opres_success)
                return res;

            List<MemberInfo> qresult = (List<MemberInfo>)user.getQueryResult(QueryType.queryTypePlayerMember);
            for (int i = 0; i < qresult.Count; i++)
            {
                MemberInfo minfo = qresult[i];
                res = statDirectlyPlayer(minfo, p.m_time, user);
                if (res != OpRes.opres_success)
                    return res;
            }

            res = statSubAgentSum(p.m_creator, p.m_time, user);
        }

        m_result.finish(p, user);
        return res;
    }

    public override object getStatResult() { return m_result; }

    private OpRes statSubAgent(MemberInfo minfo, string time, GMUser user)
    {
        string cmd = "";
        ParamMemberInfo p = new ParamMemberInfo();
        p.m_creator = minfo.m_acc;
        p.m_time = time;
        p.includeDirectlyPlayer();
        OpRes res = m_generator.genSearchSql(p, user, ref cmd);
        if (res != OpRes.opres_success)
            return res;

        Dictionary<string, object> data
            = user.sqlDb.queryOne(cmd, user.getMySqlServerID(), MySqlDbName.DB_XIANXIA);
        StatResultWinLoseItem sitem = m_result.addItem(data, minfo.m_acc, minfo.m_accType);
        if (sitem != null)
        {
            sitem.m_agentRatio = minfo.m_agentRatio;
            sitem.m_washRatio = minfo.m_washRatio;
        }
        return OpRes.opres_success;
    }

    private OpRes statSubAgentSum(string creator, string time, GMUser user)
    {
        string cmd = "";
        ParamMemberInfo p = new ParamMemberInfo();
        p.m_creator = creator;
        p.m_time = time;
        p.notIncludeDirectlyPlayer();
        OpRes res = m_generator.genSearchSql(p, user, ref cmd);
        if (res != OpRes.opres_success)
            return res;

        Dictionary<string, object> data
            = user.sqlDb.queryOne(cmd, user.getMySqlServerID(), MySqlDbName.DB_XIANXIA);
        m_result.setSubAgentItem(data, creator, 0);
        return OpRes.opres_success;
    }

    private OpRes statDirectlyPlayer(MemberInfo minfo, string time, GMUser user)
    {
        string cmd = "";
        ParamMemberInfo p = new ParamMemberInfo();
        p.m_acc = minfo.m_acc;
        p.m_time = time;
        OpRes res = m_generator.genSearchSql(p, user, ref cmd);
        if (res != OpRes.opres_success)
            return res;

        Dictionary<string, object> data
            = user.sqlDb.queryOne(cmd, user.getMySqlServerID(), MySqlDbName.DB_XIANXIA);
        StatResultWinLoseItem sitem = m_result.addItem(data, minfo.m_acc, AccType.ACC_PLAYER);
        if (sitem != null)
        {
            sitem.m_agentRatio = minfo.m_agentRatio;
            sitem.m_washRatio = minfo.m_washRatio;
        }
        return OpRes.opres_success;
    }

    private OpRes statDirectlyPlayerSum(string creator, string time, GMUser user)
    {
        string cmd = "";
        ParamMemberInfo p = new ParamMemberInfo();
        p.m_creator = creator;
        p.m_time = time;
        p.m_searchDepth = 1;
        OpRes res = m_generator.genSearchSql(p, user, ref cmd);
        if (res != OpRes.opres_success)
            return res;

        Dictionary<string, object> data
            = user.sqlDb.queryOne(cmd, user.getMySqlServerID(), MySqlDbName.DB_XIANXIA);
        m_result.setDirectlyPlayerItem(data, creator, 0);
        return OpRes.opres_success;
    }

    private void addSubLevel(GMUser user, string creator, string acc, ParamWinLose p)
    {
        URLParam uparam = new URLParam();
        uparam.m_key = "acc";
        uparam.m_value = acc;
        uparam.addExParam("time", p.m_time.TrimStart(' ').TrimEnd(' '));
        uparam.addExParam("detail", p.m_detailType);
        uparam.m_url = DefCC.ASPX_WIN_LOSE;

        if (p.m_creatorIsSubAcc)
        {
            user.getOpLevelMgr().addSub(user.m_user, acc, uparam);
        }
        else
        {
            user.getOpLevelMgr().addSub(creator, acc, uparam);
        }
    }

    private OpRes statOnePlayer(GMUser user, ParamWinLose param)
    {
        string cmd = "";
        ParamMemberInfo p = new ParamMemberInfo();
        p.m_acc = param.m_playerAcc;
        p.m_time = param.m_time;
        p.setPlayerWinLoseList();
        OpRes res = m_generator.genSearchSql(p, user, ref cmd);
        if (res != OpRes.opres_success)
            return res;

        List<Dictionary<string, object>> dataList
             = user.sqlDb.queryList(cmd, user.getMySqlServerID(), MySqlDbName.DB_XIANXIA);

        if (dataList != null)
        {
            foreach (var data in dataList)
            {
                m_result.addItem(data, p.m_acc, AccType.ACC_PLAYER);
            }
        }
        // StatResultWinLoseItem sitem = m_result.addItem(data, p.m_acc, AccType.ACC_PLAYER);
        //         if (sitem != null)
        //         {
        //             sitem.m_agentRatio = minfo.m_agentRatio;
        //             sitem.m_washRatio = minfo.m_washRatio;
        //         }
        return OpRes.opres_success;
    }
}







