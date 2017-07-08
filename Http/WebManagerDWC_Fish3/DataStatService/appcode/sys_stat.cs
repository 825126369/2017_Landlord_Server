using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

class SysStatRemain : SysBase
{
    // 统计模块
    private Dictionary<int, StatBase> m_statModule = new Dictionary<int, StatBase>();
    static int[] s_remain = { 1, 3, 7, 30 };
    static int[] s_totalRecharge = { 1, 3, 7, 14, 30, 60, 90 };

    public override void init() 
    {
        initStatModule();
        initChannel();
    }

    public override void update(double delta)
    {
        List<ChannelInfo> channelList = ResMgr.getInstance().getChannelList();
        foreach (var info in channelList)
        {
            bool res = statChannel(info);
            if (res)
            {
                resetChannelStatDay(info);
            }
        }
    }

    private void initStatModule()
    {
        m_statModule.Add(StatFlag.STAT_FLAG_ACTIVE, new StatUnitActive());
        m_statModule.Add(StatFlag.STAT_FLAG_RECHARGE, new StatUnitRecharge());
        m_statModule.Add(StatFlag.STAT_FLAG_REMAIN, new StatUnitRemain());
        m_statModule.Add(StatFlag.STAT_FLAG_COUNT, new StatUnitCount());
        m_statModule.Add(StatFlag.STAT_LTV, new StatLTV());
    }

    private void initChannel()
    {
        List<ChannelInfo> channelList = ResMgr.getInstance().getChannelList();
        foreach (var info in channelList)
        {
            Dictionary<string, object> data =
                MongodbAccount.Instance.ExecuteGetOneBykey(TableName.CHANNEL_STAT_DAY, "channel", info.m_channelNum);
            if (data != null)
            {
                info.m_statDay = Convert.ToDateTime(data["statDay"]).ToLocalTime();
            }
            else
            {
                DateTime now = DateTime.Now.Date;
                info.m_statDay = now.AddDays(1);

                Dictionary<string, object> newData = new Dictionary<string, object>();
                newData.Add("statDay", info.m_statDay);
                newData.Add("channel", info.m_channelNum);
                MongodbAccount.Instance.ExecuteStoreBykey(TableName.CHANNEL_STAT_DAY, "channel", info.m_channelNum, newData);
            }
        }
    }

    private bool statChannel(ChannelInfo info)
    {
        if (!StatBase.canStat(info))
            return false;

        beginStat("渠道[{0}]开始统计", info.m_channelName);

        ParamStat param = new ParamStat();
        param.m_channel = info;

        StatResult result = new StatResult();

        foreach (var stat in m_statModule.Values)
        {
            stat.doStat(param, result);
        }

        Dictionary<string, object> newData = new Dictionary<string, object>();
        newData.Add("genTime", info.m_statDay.Date.AddDays(-1));
        newData.Add("channel", info.m_channelNum);

        newData.Add("regeditCount", result.m_regeditCount);
        newData.Add("deviceActivationCount", result.m_deviceActivationCount);
        newData.Add("activeCount", result.m_activeCount);

        newData.Add("totalIncome", result.m_totalIncome);
        newData.Add("rechargePersonNum", result.m_rechargePersonNum);
        newData.Add("rechargeCount", result.m_rechargeCount);

        newData.Add("newAccIncome", result.m_newAccIncome);
        newData.Add("newAccRechargePersonNum", result.m_newAccRechargePersonNum);

        newData.Add("2DayRemainCount", result.m_2DayRemainCount);

        newData.Add("3DayRemainCount", result.m_3DayRemainCount);

        newData.Add("7DayRemainCount", result.m_7DayRemainCount);

        newData.Add("30DayRemainCount", result.m_30DayRemainCount);

        //////////////////////////////////////////////////////////////////////////
        newData.Add("Day2DevRemainCount", result.m_2DayDevRemainCount);
        newData.Add("Day3DevRemainCount", result.m_3DayDevRemainCount);
        newData.Add("Day7DevRemainCount", result.m_7DayDevRemainCount);
        newData.Add("Day30DevRemainCount", result.m_30DayDevRemainCount);

        newData.Add("Day1TotalRecharge", result.m_1DayTotalRecharge);
        newData.Add("Day3TotalRecharge", result.m_3DayTotalRecharge);
        newData.Add("Day7TotalRecharge", result.m_7DayTotalRecharge);
        newData.Add("Day14TotalRecharge", result.m_14DayTotalRecharge);
        newData.Add("Day30TotalRecharge", result.m_30DayTotalRecharge);
        newData.Add("Day60TotalRecharge", result.m_60DayTotalRecharge);
        newData.Add("Day90TotalRecharge", result.m_90DayTotalRecharge);

        IMongoQuery imq1 = Query.EQ("genTime", BsonValue.Create(info.m_statDay.Date.AddDays(-1)));
        IMongoQuery imq2 = Query.EQ("channel", BsonValue.Create(info.m_channelNum));
        IMongoQuery imq = Query.And(imq1, imq2);

        foreach (var d in s_remain)
        {
            updateRemain(info, d, result);
            updateDevRemain(info, d, result);
        }
        foreach (var d in s_totalRecharge)
        {
            updateTotalRecharge(info, d, result);
        }

        string str = MongodbAccount.Instance.ExecuteStoreByQuery(TableName.CHANNEL_TD, imq, newData);
        
        endStat("渠道[{0}]结束统计", info.m_channelName);
        return str == string.Empty;
    }

    private void resetChannelStatDay(ChannelInfo info)
    {
        Dictionary<string, object> upData = new Dictionary<string, object>();
        upData.Add("statDay", info.m_statDay.Date.AddDays(1));
        MongodbAccount.Instance.ExecuteStoreBykey(TableName.CHANNEL_STAT_DAY, "channel", info.m_channelNum, upData);
        info.m_statDay = info.m_statDay.Date.AddDays(1);
    }

    // 更新往日留存
    private void updateRemain(ChannelInfo info, int days, StatResult result)
    {
        IMongoQuery imq1 = Query.EQ("genTime", StatBase.getRemainRegTime(info, days));
        IMongoQuery imq2 = Query.EQ("channel", BsonValue.Create(info.m_channelNum));
        IMongoQuery imq = Query.And(imq1, imq2);

        bool res = MongodbAccount.Instance.KeyExistsByQuery(TableName.CHANNEL_TD, imq);
        if (res)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            switch (days)
            {
                case 1:
                    {
                        data.Add("2DayRemainCount", result.m_2DayRemainCountTmp);
                    }
                    break;
                case 3:
                    {
                        data.Add("3DayRemainCount", result.m_3DayRemainCountTmp);
                    }
                    break;
                case 7:
                    {
                        data.Add("7DayRemainCount", result.m_7DayRemainCountTmp);
                    }
                    break;
                case 30:
                    {
                        data.Add("30DayRemainCount", result.m_30DayRemainCountTmp);
                    }
                    break;
            }

            MongodbAccount.Instance.ExecuteUpdateByQuery(TableName.CHANNEL_TD, imq, data);
        }
    }

    // 更新往日设备留存
    private void updateDevRemain(ChannelInfo info, int days, StatResult result)
    {
        IMongoQuery imq1 = Query.EQ("genTime", StatBase.getRemainRegTime(info, days));
        IMongoQuery imq2 = Query.EQ("channel", BsonValue.Create(info.m_channelNum));
        IMongoQuery imq = Query.And(imq1, imq2);

        bool res = MongodbAccount.Instance.KeyExistsByQuery(TableName.CHANNEL_TD, imq);
        if (res)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            switch (days)
            {
                case 1:
                    {
                        data.Add("Day2DevRemainCount", result.m_2DayDevRemainCountTmp);
                    }
                    break;
                case 3:
                    {
                        data.Add("Day3DevRemainCount", result.m_3DayDevRemainCountTmp);
                    }
                    break;
                case 7:
                    {
                        data.Add("Day7DevRemainCount", result.m_7DayDevRemainCountTmp);
                    }
                    break;
                case 30:
                    {
                        data.Add("Day30DevRemainCount", result.m_30DayDevRemainCountTmp);
                    }
                    break;
            }

            MongodbAccount.Instance.ExecuteUpdateByQuery(TableName.CHANNEL_TD, imq, data);
        }
    }

    // 更新往日LTV价值
    private void updateTotalRecharge(ChannelInfo info, int days, StatResult result)
    {
        IMongoQuery imq1 = Query.EQ("genTime", StatBase.getRemainRegTime(info, days));
        IMongoQuery imq2 = Query.EQ("channel", BsonValue.Create(info.m_channelNum));
        IMongoQuery imq = Query.And(imq1, imq2);

        bool res = MongodbAccount.Instance.KeyExistsByQuery(TableName.CHANNEL_TD, imq);
        if (res)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            switch (days)
            {
                case 1:
                    {
                        data.Add("Day1TotalRecharge", result.m_1DayTotalRechargeTmp);
                    }
                    break;
                case 3:
                    {
                        data.Add("Day3TotalRecharge", result.m_3DayTotalRechargeTmp);
                    }
                    break;
                case 7:
                    {
                        data.Add("Day7TotalRecharge", result.m_7DayTotalRechargeTmp);
                    }
                    break;
                case 14:
                    {
                        data.Add("Day14TotalRecharge", result.m_14DayTotalRechargeTmp);
                    }
                    break;
                case 30:
                    {
                        data.Add("Day30TotalRecharge", result.m_30DayTotalRechargeTmp);
                    }
                    break;
                case 60:
                    {
                        data.Add("Day60TotalRecharge", result.m_60DayTotalRechargeTmp);
                    }
                    break;
                case 90:
                    {
                        data.Add("Day90TotalRecharge", result.m_90DayTotalRechargeTmp);
                    }
                    break;
            }

            MongodbAccount.Instance.ExecuteUpdateByQuery(TableName.CHANNEL_TD, imq, data);
        }
    }
}

//////////////////////////////////////////////////////////////////////////
// 玩家拥有的金币总量
class SysStatPlayerTotalMoney : SysBase
{
    DateTime m_statDay;
    long m_moneyTotal; // 玩家金币
    long m_safeBox;    // 保险箱内金币

    public override void init()
    {
        Dictionary<string, object> data =
                MongodbPlayer.Instance.ExecuteGetOneBykey(TableName.TOTAL_MONEY_STAT_DAY, "key", "playerTotalMoney");
        if (data != null)
        {
            m_statDay = Convert.ToDateTime(data["statDay"]).ToLocalTime();
        }
        else
        {
            DateTime now = DateTime.Now.Date;
            m_statDay = now.AddDays(1);
            resetStatDay(m_statDay);
        }
    }

    public override void update(double delta)
    {
        if (DateTime.Now.Date < m_statDay)
            return;
        
        beginStat("SysStatPlayerTotalMoney开始统计");

        m_moneyTotal = -1;
        m_safeBox = -1;
        stat();
        if (m_moneyTotal >= 0)
        {
            Dictionary<string, object> newData = new Dictionary<string, object>();
            newData.Add("genTime", DateTime.Now.Date);
            newData.Add("money", m_moneyTotal);
            newData.Add("safeBox", m_safeBox);
            MongodbLog.Instance.ExecuteInsert(TableName.PUMP_PLAYER_TOTAL_MONEY, newData);
        }

        m_statDay = m_statDay.AddDays(1);
        resetStatDay(m_statDay);

        endStat("SysStatPlayerTotalMoney结束统计");
    }

    void stat()
    {
        DateTime now = DateTime.Now;
        IMongoQuery imq1 = Query.GTE("logout_time", now.AddDays(-7));
        IMongoQuery imq2 = Query.EQ("is_robot", false);
        IMongoQuery imq = Query.And(imq1, imq2);
        MapReduceResult mapResult = MongodbPlayer.Instance.executeMapReduce(TableName.PLAYER_INFO,
                                                                             imq,
                                                                             MapReduceTable.getMap("totalMoney"),
                                                                             MapReduceTable.getReduce("totalMoney"));

        if (mapResult != null)
        {
            IEnumerable<BsonDocument> bson = mapResult.GetResults();
            foreach (BsonDocument d in bson)
            {
                BsonValue resValue = d["value"];
                m_moneyTotal = resValue["total"].ToInt64();
                m_safeBox = resValue["box"].ToInt64();
            }
        }
    }

    void resetStatDay(DateTime statDay)
    {
        Dictionary<string, object> upData = new Dictionary<string, object>();
        upData.Add("statDay", statDay);
        MongodbPlayer.Instance.ExecuteStoreBykey(TableName.TOTAL_MONEY_STAT_DAY,
            "key", "playerTotalMoney", upData);
    }
}

//////////////////////////////////////////////////////////////////////////

class StatByDayBase : SysBase
{
    protected DateTime m_statDay;

    public override void init()
    {
        Dictionary<string, object> data =
                MongodbPlayer.Instance.ExecuteGetOneBykey(TableName.TOTAL_MONEY_STAT_DAY, "key", 
                getStatKey());
        if (data != null)
        {
            m_statDay = Convert.ToDateTime(data["statDay"]).ToLocalTime();
        }
        else
        {
            DateTime now = DateTime.Now.Date;
            m_statDay = now.AddDays(1);
            resetStatDay(m_statDay);
        }
    }

    protected void resetStatDay(DateTime statDay)
    {
        Dictionary<string, object> upData = new Dictionary<string, object>();
        upData.Add("statDay", statDay);
        MongodbPlayer.Instance.ExecuteStoreBykey(TableName.TOTAL_MONEY_STAT_DAY,
            "key", getStatKey(), upData);
    }

    public virtual string getStatKey()
    {
        throw new Exception();
    }

    protected void addStatDay()
    {
        m_statDay = m_statDay.AddDays(1);
        resetStatDay(m_statDay);
    }
}

//////////////////////////////////////////////////////////////////////////
// 大R流失
class SysStatLose : StatByDayBase
{
    static string[] PLAYER_FIELDS = { "player_id", "nickname", "VipLevel", "gold", "ticket", "dragonBall" };
    static string[] PLAYER_FIELDS_1 = { "account" };
    IMongoQuery m_queryCond = null;

    public override void init()
    {
        base.init();

        IMongoQuery imq1 = Query.GTE("VipLevel", BsonValue.Create(5));
        IMongoQuery imq2 = Query.EQ("is_robot", BsonValue.Create(false));
        m_queryCond = Query.And(imq1, imq2);
    }

    public override string getStatKey()
    {
        return StatKey.KEY_LOSE;
    }

    public override void update(double delta)
    {
        if (DateTime.Now.Date < m_statDay)
            return;

        beginStat("SysStatLose开始统计");

        MongodbPlayer.Instance.ExecuteRemoveAll(TableName.RLOSE);
        DateTime mint = m_statDay.Date.AddDays(-1), maxt = m_statDay.Date;

        int skip = 0;
        List<Dictionary<string, object>> dataList = null;
        while (true)
        {
            dataList = QueryTool.nextData(MongodbPlayer.Instance,
                                      TableName.PLAYER_INFO,
                                      m_queryCond,
                                      ref skip,
                                      1000,
                                      PLAYER_FIELDS_1);
            if (dataList == null)
                break;

            for (int i = 0; i < dataList.Count; i++)
            {
                Dictionary<string, object> data = dataList[i];
                if (data.ContainsKey("account"))
                {
                    bool code = QueryTool.isLogin(Convert.ToString(data["account"]), mint, maxt);
                    if (!code)
                    {
                        addLosePlayer(Convert.ToString(data["account"]));
                    }
                }
            }
        }

        m_statDay = m_statDay.AddDays(1);
        resetStatDay(m_statDay);

        endStat("SysStatLose结束统计");
    }

    void addLosePlayer(string acc)
    {
        Dictionary<string, object> data = MongodbPlayer.Instance.ExecuteGetBykey(TableName.PLAYER_INFO, "account", acc, PLAYER_FIELDS);
        if (data == null)
            return;

        Dictionary<string, object> newData = new Dictionary<string, object>();
        newData.Add("playerId", Convert.ToInt32(data["player_id"]));
        newData.Add("nickName", Convert.ToString(data["nickname"]));
        newData.Add("vipLevel", Convert.ToInt32(data["VipLevel"]));
        newData.Add("gold", Convert.ToInt32(data["gold"]));
        newData.Add("gem", Convert.ToInt32(data["ticket"]));

        if (data.ContainsKey("dragonBall"))
        {
            newData.Add("dragonBall", Convert.ToInt32(data["dragonBall"]));
        }
        else
        {
            newData.Add("dragonBall", 0);
        }

        MongodbPlayer.Instance.ExecuteInsert(TableName.RLOSE, newData);
    }
}

//////////////////////////////////////////////////////////////////////////
// 根据一定时间间隔统计
class StatByIntervalBase : SysBase
{
    protected DateTime m_statTime;

    public override void init()
    {
        Dictionary<string, object> data =
                MongodbPlayer.Instance.ExecuteGetOneBykey(TableName.TOTAL_MONEY_STAT_DAY, "key",
                getStatKey());
        if (data != null)
        {
            m_statTime = Convert.ToDateTime(data["statDay"]).ToLocalTime();
        }
        else
        {
            DateTime now = DateTime.Now.Date.AddMinutes(getStatInterval());
            m_statTime = now;
            resetStatDay(m_statTime);
        }
    }

    protected void resetStatDay(DateTime statDay)
    {
        Dictionary<string, object> upData = new Dictionary<string, object>();
        upData.Add("statDay", statDay);
        MongodbPlayer.Instance.ExecuteStoreBykey(TableName.TOTAL_MONEY_STAT_DAY,
            "key", getStatKey(), upData);
    }

    protected void addStatTime()
    {
        m_statTime = m_statTime.AddMinutes(getStatInterval());
        resetStatDay(m_statTime);
    }

    public virtual string getStatKey()
    {
        throw new Exception();
    }

    // 返回统计间隔(分钟)
    public virtual int getStatInterval()
    {
        throw new Exception();
    }
}

//////////////////////////////////////////////////////////////////////////
// 根据时间间隔统计玩家龙珠
class StatPlayerDragonBall : StatByDayBase
{
    static string[] FIELDS_NEW_VALUE = { "newValue" };
    static string[] FIELDS_GOLD_REMAIN = { "goldRemain" };
    static string[] FIELDS_GEM_REMAIN = { "gemRemain" };
    static string[] FIELDS_DRAGON_REMAIN = { "dbRemain" };

    static string[] FIELDS_OLD_VALUE = { "oldValue" };

    public override string getStatKey()
    {
        return StatKey.KEY_DRAGON;
    }

    public override void update(double delta)
    {
        if (DateTime.Now.Date < m_statDay)
            return;

        beginStat("StatPlayerDragonBall开始统计");

        stat();

        addStatDay();

        endStat("StatPlayerDragonBall结束统计");
    }

    void stat()
    {
        DateTime startTime = m_statDay.AddDays(-1);
        DateTime endTime = m_statDay;
        IMongoQuery imq1 = Query.LT("genTime", BsonValue.Create(endTime));
        IMongoQuery imq2 = Query.GTE("genTime", BsonValue.Create(startTime));
        IMongoQuery imq = Query.And(imq1, imq2);

        MapReduceResult mapResult = MongodbLog.Instance.executeMapReduce(TableName.PUMP_PLAYER_MONEY,
                                                                             imq,
                                                                             MapReduceTable.getMap("dragonBallPlayer"),
                                                                             MapReduceTable.getReduce("dragonBallPlayer"));

        if (mapResult != null)
        {
            IEnumerable<BsonDocument> bson = mapResult.GetResults();
            StatDragonItem item = new StatDragonItem();
            foreach (BsonDocument d in bson)
            {
                item.m_playerId = Convert.ToInt32(d["_id"]);
                BsonValue resValue = d["value"];

                item.m_dbgain = resValue["dbgain"].ToInt64();
                item.m_dbsend = resValue["dbsend"].ToInt64();
                item.m_dbaccept = resValue["dbaccept"].ToInt64();
                item.m_dbexchange = resValue["dbexchange"].ToInt64();
                item.m_dbRemain = getLastVal(endTime, 14, item.m_playerId, FIELDS_DRAGON_REMAIN);
                item.m_dbStart = getStartVal(startTime, 14, item.m_playerId, FIELDS_DRAGON_REMAIN);

                item.m_goldByRecharge = resValue["goldByRecharge"].ToInt64();
                item.m_goldByOther = resValue["goldByOther"].ToInt64();
                item.m_goldConsume = resValue["goldConsume"].ToInt64();
                item.m_goldRemain = getLastVal(endTime, 1, item.m_playerId, FIELDS_GOLD_REMAIN);
                item.m_goldStart = getStartVal(startTime, 1, item.m_playerId, FIELDS_GOLD_REMAIN);

                item.m_gemByRecharge = resValue["gemByRecharge"].ToInt64();
                item.m_gemByOther = resValue["gemByOther"].ToInt64();
                item.m_gemConsume = resValue["gemConsume"].ToInt64();
                item.m_gemRemain = getLastVal(endTime, 2, item.m_playerId, FIELDS_GEM_REMAIN);
                item.m_gemStart = getStartVal(startTime, 2, item.m_playerId, FIELDS_GEM_REMAIN);

                item.m_todayRecharge = getTotalRecharge(startTime, endTime, item.m_playerId);
                addData(item, startTime);
            }
        }
    }

    void addData(StatDragonItem item, DateTime time)
    {
        IMongoQuery imq1 = Query.EQ("playerId", BsonValue.Create(item.m_playerId));
        IMongoQuery imq2 = Query.EQ("genTime", BsonValue.Create(time));
        IMongoQuery imq = Query.And(imq1, imq2);

        Dictionary<string, object> data = new Dictionary<string, object>();

        data.Add("dbgain", item.m_dbgain);
        data.Add("dbsend", item.m_dbsend);
        data.Add("dbaccept", item.m_dbaccept);
        data.Add("dbexchange", item.m_dbexchange);
        data.Add("dbRemain", item.m_dbRemain);
        data.Add("dbStart", item.m_dbStart);

        data.Add("goldByRecharge", item.m_goldByRecharge);
        data.Add("goldByOther", item.m_goldByOther);
        data.Add("goldConsume", item.m_goldConsume);
        data.Add("goldRemain", item.m_goldRemain);
        data.Add("goldStart", item.m_goldStart);

        data.Add("gemByRecharge", item.m_gemByRecharge);
        data.Add("gemByOther", item.m_gemByOther);
        data.Add("gemConsume", item.m_gemConsume);
        data.Add("gemRemain", item.m_gemRemain);
        data.Add("gemStart", item.m_gemStart);

        data.Add("todayRecharge", item.m_todayRecharge);

        MongodbLog.Instance.ExecuteUpdateByQuery(TableName.STAT_PLAYER_DRAGON, imq, data);
    }

    // 返回时间段imq的最后一条关于itemId的数据
    public static int getLastVal(DateTime endTime, int itemId, int playerId, string[] remainFields)
    {
        IMongoQuery imq1 = Query.LT("genTime", BsonValue.Create(endTime));
        IMongoQuery imq = Query.And(imq1, Query.EQ("itemId", itemId), Query.EQ("playerId", playerId));

        List<Dictionary<string, object>> dataList = MongodbLog.Instance.ExecuteGetListByQuery(TableName.PUMP_PLAYER_MONEY,
             imq, FIELDS_NEW_VALUE, "genTime", false, 0, 1);

        int retVal = 0;
        if (dataList != null && dataList.Count > 0)
        {
            if (dataList[0].ContainsKey("newValue"))
            {
                retVal = Convert.ToInt32(dataList[0]["newValue"]);
            }
        }

        return retVal;
    }

    // 得到初始值
    public static int getStartVal(DateTime startTime, int itemId, int playerId, string[] remainFields)
    {
        int retVal = 0;
        IMongoQuery imq1 = Query.LT("genTime", startTime);
        IMongoQuery imq = Query.And(imq1, Query.EQ("itemId", itemId), Query.EQ("playerId", playerId));

        List<Dictionary<string, object>> dataList = MongodbLog.Instance.ExecuteGetListByQuery(TableName.PUMP_PLAYER_MONEY,
            imq, FIELDS_NEW_VALUE, "genTime", false, 0, 1);
        if (dataList != null && dataList.Count > 0)
        {
            if (dataList[0].ContainsKey("newValue"))
            {
                retVal = Convert.ToInt32(dataList[0]["newValue"]);
            }
        }

        return retVal;
    }

    // 返回总的充值
    int getTotalRecharge(DateTime startTime, DateTime endTime, int playerId)
    {
        int retVal = 0;
        IMongoQuery imq1 = Query.LT("time", BsonValue.Create(endTime));
        IMongoQuery imq2 = Query.GTE("time", BsonValue.Create(startTime));

        Dictionary<string, object> qr = MongodbPlayer.Instance.ExecuteGetOneBykey(TableName.PLAYER_INFO,
            "player_id",
            playerId, new string[] { "account" });
        if (qr != null)
        {
            string acc = Convert.ToString(qr["account"]);
            IMongoQuery imq = Query.And(imq1, imq2, Query.EQ("acc", acc));

            MapReduceResult mapResult = MongodbPayment.Instance.executeMapReduce("PayLog",
                                                                             imq,
                                                                             MapReduceTable.getMap("recharge"),
                                                                             MapReduceTable.getReduce("recharge"));
            if (mapResult != null)
            {
                IEnumerable<BsonDocument> bson = mapResult.GetResults();
                foreach (BsonDocument d in bson)
                {
                    BsonValue resValue = d["value"];
                    retVal += resValue["total"].ToInt32();
                }
            }
        }

        return retVal;
    }

    public static int getLastValNew(IMongoQuery imqCond, int itemId, int playerId, ref int addValue)
    {
        IMongoQuery imq1 = imqCond; // Query.LT("genTime", BsonValue.Create(endTime));
        IMongoQuery imq = Query.And(imq1, Query.EQ("itemId", itemId), Query.EQ("playerId", playerId));

        List<Dictionary<string, object>> dataList = MongodbLog.Instance.ExecuteGetListByQuery(TableName.PUMP_PLAYER_MONEY,
             imq, FIELDS_NEW_VALUE, "genTime", false, 0, 1);

        int retVal = 0;
        //int oldValue = 0;
        //addValue = 0;
        if (dataList != null && dataList.Count > 0)
        {
            if (dataList[0].ContainsKey("newValue"))
            {
                retVal = Convert.ToInt32(dataList[0]["newValue"]);
            }
//             if (dataList[0].ContainsKey("oldValue"))
//             {
//                 oldValue = Convert.ToInt32(dataList[0]["oldValue"]);
//             }
        }
        //addValue = retVal - oldValue;
        return retVal;
    }

    public static int getStartValNew(IMongoQuery imqCond, int itemId, int playerId)
    {
        int retVal = 0;
        IMongoQuery imq1 = imqCond; // Query.GTE("genTime", startTime);
        IMongoQuery imq = Query.And(imq1, Query.EQ("itemId", itemId), Query.EQ("playerId", playerId));

        // 比startTime大的第一条记录
        List<Dictionary<string, object>> dataList = MongodbLog.Instance.ExecuteGetListByQuery(TableName.PUMP_PLAYER_MONEY,
            imq, FIELDS_OLD_VALUE, "genTime", true, 0, 1);
        if (dataList != null && dataList.Count > 0)
        {
            if (dataList[0].ContainsKey("oldValue"))
            {
                retVal = Convert.ToInt32(dataList[0]["oldValue"]);
            }
        }

        return retVal;
    }
}

//////////////////////////////////////////////////////////////////////////
// 龙珠每日总计
class StatDragonBallTotal : StatByDayBase
{
    public override string getStatKey()
    {
        return StatKey.KEY_DRAGON_DAILY;
    }

    public override void update(double delta)
    {
        if (DateTime.Now.Date < m_statDay)
            return;

        beginStat("StatDragonBallTotal开始统计");

        stat();

        addStatDay();

        endStat("StatDragonBallTotal结束统计");
    }

    void stat()
    {
        DateTime startTime = m_statDay.AddDays(-1);
        DateTime endTime = m_statDay;

        StatDragonDailyItem item = new StatDragonDailyItem();
        statTodayDragonBall(startTime, endTime, item);
        statTodayRecharge(startTime, endTime, item);
        statTodayDragonRemin(startTime, endTime, item);
        addData(item, startTime);
    }

    void addData(StatDragonDailyItem item, DateTime time)
    {
        IMongoQuery imq = Query.EQ("genTime", BsonValue.Create(time));

        Dictionary<string, object> data = new Dictionary<string, object>();
        data.Add("todayRecharge", item.m_todayRecharge);
        data.Add("dragonBallGen", item.m_dragonBallGen);
        data.Add("dragonBallConsume", item.m_dragonBallConsume);
        data.Add("dragonBallRemain", item.m_dragonBallRemain);

        MongodbLog.Instance.ExecuteUpdateByQuery(TableName.STAT_DRAGON_DAILY, imq, data);
    }

    // 今日产出龙珠
    void statTodayDragonBall(DateTime startTime, DateTime endTime, StatDragonDailyItem item)
    {
        IMongoQuery imq1 = Query.LT("genTime", BsonValue.Create(endTime));
        IMongoQuery imq2 = Query.GTE("genTime", BsonValue.Create(startTime));
        IMongoQuery imq = Query.And(imq1, imq2);

        MapReduceResult mapResult = MongodbLog.Instance.executeMapReduce(TableName.PUMP_PLAYER_MONEY,
                                                                             imq,
                                                                             MapReduceTable.getMap("dragonBallDaily"),
                                                                             MapReduceTable.getReduce("dragonBallDaily"));

        if (mapResult != null)
        {
            IEnumerable<BsonDocument> bson = mapResult.GetResults();
            foreach (BsonDocument d in bson)
            {
                BsonValue resValue = d["value"];
                item.m_dragonBallGen = resValue["dbGen"].ToInt64();
                item.m_dragonBallConsume = resValue["dbConsume"].ToInt64();
            }
        }
    }

    // 今日总充值
    void statTodayRecharge(DateTime startTime, DateTime endTime, StatDragonDailyItem item)
    {
        IMongoQuery imq1 = Query.LT("time", BsonValue.Create(endTime));
        IMongoQuery imq2 = Query.GTE("time", BsonValue.Create(startTime));
        IMongoQuery imq = Query.And(imq1, imq2);

        MapReduceResult mapResult = MongodbPayment.Instance.executeMapReduce("PayLog",
                                                                         imq,
                                                                         MapReduceTable.getMap("recharge"),
                                                                         MapReduceTable.getReduce("recharge"));
        if (mapResult != null)
        {
            IEnumerable<BsonDocument> bson = mapResult.GetResults();
            foreach (BsonDocument d in bson)
            {
                BsonValue resValue = d["value"];
                item.m_todayRecharge += resValue["total"].ToInt32();
            }
        }
    }

    // 统计剩余龙珠
    void statTodayDragonRemin(DateTime startTime, DateTime endTime, StatDragonDailyItem item)
    {
        IMongoQuery imq = Query.EQ("is_robot", false);
        MapReduceResult mapResult = MongodbPlayer.Instance.executeMapReduce(TableName.PLAYER_INFO,
                                                                         imq,
                                                                         MapReduceTable.getMap("dragonBallSum"),
                                                                         MapReduceTable.getReduce("dragonBallSum"));
        if (mapResult != null)
        {
            IEnumerable<BsonDocument> bson = mapResult.GetResults();
            foreach (BsonDocument d in bson)
            {
                BsonValue resValue = d["value"];
                item.m_dragonBallRemain += resValue["total"].ToInt64();
            }
        }
    }
}

//////////////////////////////////////////////////////////////////////////
// 在线游戏时间统计
class StatOnlineGameTime : StatByIntervalBase
{
    public override string getStatKey()
    {
        return StatKey.KEY_ONLINE_GAME_TIME;
    }

    // 返回统计间隔(分钟)
    public override int getStatInterval()
    {
        return 10;
    }

    public override void update(double delta)
    {
        if (DateTime.Now < m_statTime)
            return;

        beginStat("StatOnlineGameTime开始统计");
        stat();

        addStatTime();
        endStat("StatOnlineGameTime结束统计");
    }

    void stat()
    {
        IMongoQuery imq1 = Query.LT("loginTime", m_statTime);
        IMongoQuery imq2 = Query.GTE("loginTime", m_statTime.AddMinutes(-getStatInterval()));
        IMongoQuery imq = Query.And(imq1, imq2);

        MapReduceResult mapResult = MongodbLog.Instance.executeMapReduce(TableName.PUMP_PLAYER_ONLINE_TIME,
                                                                        null,
                                                                        MapReduceTable.getMap("playerOnlineTime"),
                                                                        MapReduceTable.getReduce("playerOnlineTime"));
        if (mapResult != null)
        {
            IEnumerable<BsonDocument> bson = mapResult.GetResults();
            foreach (BsonDocument d in bson)
            {
                int playerId = Convert.ToInt32(d["_id"]);
                BsonValue resValue = d["value"];
                double totalTime = resValue["onlineTimeSum"].ToInt64();

                addData(playerId, totalTime);
            }
        }

        // 删除原有数据
        MongodbLog.Instance.ExecuteRemoveAll(TableName.PUMP_PLAYER_ONLINE_TIME);
    }

    void addData(int playerId, double totalTime)
    {
        IMongoQuery imq = Query.EQ("playerId", playerId);

        Dictionary<string, object> data = new Dictionary<string, object>();
        data.Add("totalGameTime", (long)(totalTime/1000));

        MongodbPlayer.Instance.ExecuteIncByQuery(TableName.STAT_PLAYER_GAME_TIME, imq, data);
    }
}

//////////////////////////////////////////////////////////////////////////
// 总收支表
class StatPlayerTotalIncomeExpenses : StatByDayBase
{
    List<BsonDocument> m_remain = new List<BsonDocument>();
    List<BsonDocument> m_error = new List<BsonDocument>();

    public override string getStatKey()
    {
        return StatKey.KEY_INCOME_EXPENSES;
    }

    public override void update(double delta)
    {
        if (DateTime.Now.Date < m_statDay)
            return;

        stat();

        addStatDay();
    }

    void stat()
    {
        m_remain.Clear();
        m_error.Clear();

        DateTime startTime = m_statDay.AddDays(-1);
        DateTime endTime = m_statDay;
        IMongoQuery imq1 = Query.LT("genTime", BsonValue.Create(endTime));
        IMongoQuery imq2 = Query.GTE("genTime", BsonValue.Create(startTime));
        IMongoQuery imq = Query.And(imq1, imq2);

        beginStat("StatPlayerTotalIncomeExpenses MapReduce开始统计");

        MapReduceResult mapResult = MongodbLog.Instance.executeMapReduce(TableName.PUMP_PLAYER_MONEY,
                                                                             imq,
                                                                             MapReduceTable.getMap("incomeExpenses"),
                                                                             MapReduceTable.getReduce("incomeExpenses"));
        endStat("StatPlayerTotalIncomeExpenses MapReduce结束统计");

        if (mapResult != null)
        {
            beginStat("StatPlayerTotalIncomeExpenses 开始写入数据");
            IEnumerable<BsonDocument> bson = mapResult.GetResults();
            StatIncomeExpensesItemBase item = new StatIncomeExpensesItemBase();
            foreach (BsonDocument d in bson)
            {
                item.m_playerId = Convert.ToInt32(d["_id"]);
                BsonValue resValue = d["value"];
                int delta = 0;

                item.m_goldFreeGain = resValue["goldFreeGain"].ToInt64();
                item.m_goldRechargeGain = resValue["goldRechargeGain"].ToInt64();
                item.m_goldConsume = resValue["goldConsume"].ToInt64();
                item.m_goldRemain = StatPlayerDragonBall.getLastValNew(imq, 1, item.m_playerId, ref delta);
                item.m_goldStart = StatPlayerDragonBall.getStartValNew(imq, 1, item.m_playerId);

                item.m_gemFreeGain = resValue["gemFreeGain"].ToInt64();
                item.m_gemRechargeGain = resValue["gemRechargeGain"].ToInt64();
                item.m_gemConsume = resValue["gemConsume"].ToInt64();
                item.m_gemRemain = StatPlayerDragonBall.getLastValNew(imq, 2, item.m_playerId, ref delta);
                item.m_gemStart = StatPlayerDragonBall.getStartValNew(imq, 2, item.m_playerId);

                item.m_dbFreeGain = resValue["dbFreeGain"].ToInt64();
                item.m_dbConsume = resValue["dbConsume"].ToInt64();
                item.m_dbRemain = StatPlayerDragonBall.getLastValNew(imq, 14, item.m_playerId, ref delta);
                item.m_dbStart = StatPlayerDragonBall.getStartValNew(imq, 14, item.m_playerId);

                item.m_chipFreeGain = resValue["chipFreeGain"].ToInt64();
                item.m_chipConsume = resValue["chipConsume"].ToInt64();
                item.m_chipRemain = StatPlayerDragonBall.getLastValNew(imq, 11, item.m_playerId, ref delta);
                item.m_chipStart = StatPlayerDragonBall.getStartValNew(imq, 11, item.m_playerId);

                long dropDb = resValue["dbDrop"].ToInt64();

                addData(item, startTime, dropDb > 0);
            }

            end();

            endStat("StatPlayerTotalIncomeExpenses 结束写入数据");
        }

        IMongoQuery imq3 = Query.EQ("is_robot", false);

        MapReduceResult mapResult1 = MongodbPlayer.Instance.executeMapReduce(TableName.PLAYER_INFO,
                                                                             imq3,
                                                                             MapReduceTable.getMap("incomeExpensesRemain"),
                                                                             MapReduceTable.getReduce("incomeExpensesRemain"));

        if (mapResult1 != null)
        {
            IEnumerable<BsonDocument> bson = mapResult1.GetResults();
            StatIncomeExpensesItemBase item = new StatIncomeExpensesItemBase();
            foreach (BsonDocument d in bson)
            {
                BsonValue resValue = d["value"];

                item.m_goldRemain = resValue["goldRemain"].ToInt64();
                item.m_gemRemain = resValue["gemRemain"].ToInt64();
                item.m_dbRemain = resValue["dbRemain"].ToInt64();
                item.m_chipRemain = resValue["chipRemain"].ToInt64();

                addData1(item, startTime);
            }
        }
    }

    void addData(StatIncomeExpensesItemBase item, DateTime time, bool dropDb)
    {
        //IMongoQuery imq1 = Query.EQ("playerId", BsonValue.Create(item.m_playerId));
       // IMongoQuery imq2 = Query.EQ("genTime", BsonValue.Create(time));
        //IMongoQuery imq = Query.And(imq1, imq2);

        Dictionary<string, object> data = new Dictionary<string, object>();
        data.Add("playerId", BsonValue.Create(item.m_playerId));
        data.Add("genTime", BsonValue.Create(time));

        data.Add("goldFreeGain", item.m_goldFreeGain);
        data.Add("goldRechargeGain", item.m_goldRechargeGain);
        data.Add("goldConsume", item.m_goldConsume);
        data.Add("goldRemain", item.m_goldRemain);
        data.Add("goldStart", item.m_goldStart);

        data.Add("gemFreeGain", item.m_gemFreeGain);
        data.Add("gemRechargeGain", item.m_gemRechargeGain);
        data.Add("gemConsume", item.m_gemConsume);
        data.Add("gemRemain", item.m_gemRemain);
        data.Add("gemStart", item.m_gemStart);

        data.Add("dbFreeGain", item.m_dbFreeGain);
        data.Add("dbConsume", item.m_dbConsume);
        data.Add("dbRemain", item.m_dbRemain);
        data.Add("dbStart", item.m_dbStart);

        data.Add("chipFreeGain", item.m_chipFreeGain);
        data.Add("chipConsume", item.m_chipConsume);
        data.Add("chipRemain", item.m_chipRemain);
        data.Add("chipStart", item.m_chipStart);

        data.Add("isDropDb", dropDb);

        m_remain.Add(data.ToBsonDocument());

        //MongodbLog.Instance.ExecuteUpdateByQuery(TableName.STAT_INCOME_EXPENSES, imq, data);

        //////////////////////////////////////////////////////////////////////////
        bool goldError = false, gemError = false, dbError = false, chipError = false;
        goldError = 
            isRelativeErrorBeyond(item.m_goldStart + item.m_goldRechargeGain +
            item.m_goldFreeGain - item.m_goldConsume, item.m_goldRemain);

        gemError =
           isRelativeErrorBeyond(item.m_gemStart + item.m_gemRechargeGain +
           item.m_gemFreeGain - item.m_gemConsume, item.m_gemRemain);

        dbError =
           isRelativeErrorBeyond(item.m_dbStart + item.m_dbFreeGain -
           item.m_dbConsume, item.m_dbRemain);

        chipError =
           isRelativeErrorBeyond(item.m_chipStart + item.m_chipFreeGain -
           item.m_chipConsume, item.m_chipRemain);

        addError(goldError, gemError, dbError, chipError, item.m_playerId, time);
    }

    void addData1(StatIncomeExpensesItemBase item, DateTime time)
    {
        IMongoQuery imq = Query.EQ("genTime", BsonValue.Create(time));

        Dictionary<string, object> data = new Dictionary<string, object>();

        data.Add("goldRemain", item.m_goldRemain);
        data.Add("gemRemain", item.m_gemRemain);
        data.Add("dbRemain", item.m_dbRemain);
        data.Add("chipRemain", item.m_chipRemain);

        MongodbLog.Instance.ExecuteUpdateByQuery(TableName.STAT_INCOME_EXPENSES_REMAIN, imq, data);
    }

    bool isRelativeErrorBeyond(double cur, double accuracy)
    {
        if (accuracy == 0.0)
            return false;

        double delta = Math.Abs(cur - accuracy);
        double e = delta / accuracy;
        return e > 0.01;
    }

    void addError(bool goldError, bool gemError, bool dbError, bool chipError, int playerId, DateTime time)
    {
        if (!(goldError || gemError || dbError || chipError))
            return;

        //IMongoQuery imq1 = Query.EQ("playerId", playerId);
        //IMongoQuery imq2 = Query.EQ("genTime", BsonValue.Create(time));
        //IMongoQuery imq = Query.And(imq1, imq2);

        Dictionary<string, object> data = new Dictionary<string, object>();
        data.Add("playerId", playerId);
        data.Add("genTime", time);
        data.Add("goldError", goldError);
        data.Add("gemError", gemError);
        data.Add("dbError", dbError);
        data.Add("chipError", chipError);

        m_error.Add(data.ToBsonDocument());
       // MongodbLog.Instance.ExecuteUpdateByQuery(TableName.STAT_INCOME_EXPENSES + "_error", imq, data);
    }

    void end()
    {
        if (m_remain.Count > 0)
        {
            MongodbLog.Instance.ExecuteInsterList(TableName.STAT_INCOME_EXPENSES, m_remain);
            m_remain.Clear();
        }
        if (m_error.Count > 0)
        {
            MongodbLog.Instance.ExecuteInsterList(TableName.STAT_INCOME_EXPENSES + "_error", m_error);
            m_error.Clear();
        }
    }
}

//////////////////////////////////////////////////////////////////////////
// 统计每小时的付费
class StatRechargePerHour : StatByIntervalBase
{
    public override string getStatKey()
    {
        return StatKey.KEY_RECHARGE_HOUR;
    }

    // 返回统计间隔(分钟)
    public override int getStatInterval()
    {
        return 60;
    }

    public override void update(double delta)
    {
        if (DateTime.Now < m_statTime)
            return;

        beginStat("StatRechargePerHour开始统计");

        stat();

        addStatTime();

        endStat("StatRechargePerHour结束统计");
    }

    void stat()
    {
        IMongoQuery imq1 = Query.LT("time", BsonValue.Create(m_statTime));
        IMongoQuery imq2 = Query.GTE("time", m_statTime.AddMinutes(-getStatInterval()));
        IMongoQuery imq = Query.And(imq1, imq2);

        int totalRecharge = 0;

        MapReduceResult mapResult = MongodbPayment.Instance.executeMapReduce("PayLog",
                                                                         imq,
                                                                         MapReduceTable.getMap("recharge"),
                                                                         MapReduceTable.getReduce("recharge"));
        if (mapResult != null)
        {
            IEnumerable<BsonDocument> bson = mapResult.GetResults();
            foreach (BsonDocument d in bson)
            {
                BsonValue resValue = d["value"];
                totalRecharge += resValue["total"].ToInt32();
            }
        }

        addData(totalRecharge, m_statTime.AddMinutes(-getStatInterval()));
    }

    void addData(int totalRecharge, DateTime curTime)
    {
        int h = curTime.Hour;
        IMongoQuery imq = Query.EQ("genTime", curTime.Date);

        Dictionary<string, object> data = new Dictionary<string, object>();
        data.Add("h" + h, totalRecharge);

        MongodbLog.Instance.ExecuteUpdateByQuery(TableName.STAT_RECHARGE_HOUR, imq, data);
    }
}

//////////////////////////////////////////////////////////////////////////
// 统计每小时在线人数
class StatOnlinePlayerNumPerHour : StatByIntervalBase
{
    public override string getStatKey()
    {
        return StatKey.KEY_ONLINE_HOUR;
    }

    // 返回统计间隔(分钟)
    public override int getStatInterval()
    {
        return 60;
    }

    public override void update(double delta)
    {
        if (DateTime.Now < m_statTime)
            return;

        beginStat("StatOnlinePlayerNumPerHour开始统计");

        stat();

        addStatTime();

        endStat("StatOnlinePlayerNumPerHour结束统计");
    }

    void stat()
    {
        int online = 0;
        Dictionary<string, object> data = MongodbPlayer.Instance.ExecuteGetOneBykey(TableName.COMMON_CONFIG, "type", "cur_playercount");
        if (data != null)
        {
            online = Convert.ToInt32(data["value"]);
        }

        addData(online, m_statTime, 0, 0);

        for (int i = 1; i < StrName.s_onlineGameIdList.Length; i++)
        {
            if (StrName.s_onlineGameIdList[i] == (int)GameId.fishlord)
            {
                for (int k = 1; k <= 3; k++)
                {
                    online = getOnlineNum(TableName.FISHLORD_ROOM, k);
                    addData(online, m_statTime, StrName.s_onlineGameIdList[i], k);
                }

                online = getOnlineNum(TableName.FISHLORD_ROOM);
                addData(online, m_statTime, StrName.s_onlineGameIdList[i]);
            }
            else
            {
                online = getOnlineNum(getTableName(StrName.s_onlineGameIdList[i]));
                addData(online, m_statTime, StrName.s_onlineGameIdList[i]);
            }
        }
    }

    void addData(int onlineNum, DateTime curTime, int gameId = 0, int roomId = 0)
    {
        int h = curTime.Hour;
        IMongoQuery imq1 = Query.EQ("genTime", curTime.Date);
        IMongoQuery imq2 = Query.EQ("gameId", gameId);
        IMongoQuery imq3 = Query.EQ("roomId", roomId);
        IMongoQuery imq = Query.And(imq1, imq2, imq3);

        Dictionary<string, object> data = new Dictionary<string, object>();
        data.Add("h" + h, onlineNum);

        MongodbLog.Instance.ExecuteUpdateByQuery(TableName.STAT_ONLINE_HOUR, imq, data);
    }

    int getOnlineNum(string table, int roomId = 0)
    {
        int count = 0;
        if (roomId == 0) // 获取全部房间的数据
        {
            List<Dictionary<string, object>> dataList =
                MongodbGame.Instance.ExecuteGetAll(table, new string[] { "player_count" });
            for (int i = 0; i < dataList.Count; i++)
            {
                if (dataList[i].ContainsKey("player_count"))
                {
                    count += Convert.ToInt32(dataList[i]["player_count"]);
                }
            }
        }
        else
        {
            Dictionary<string, object> data =
                MongodbGame.Instance.ExecuteGetOneBykey(table, "room_id", roomId, new string[] { "player_count" });
            if (data != null && data.ContainsKey("player_count"))
            {
                count = Convert.ToInt32(data["player_count"]);
            }
        }

        return count;
    }

    string getTableName(int gameId)
    {
        string name = "";
        switch (gameId)
        {
            case (int)GameId.crocodile:
                {
                    name = TableName.CROCODILE_ROOM;
                }
                break;
            case (int)GameId.cows:
                {
                    name = TableName.COWS_ROOM;
                }
                break;
            case (int)GameId.dragon:
                {
                    name = TableName.DRAGON_ROOM;
                }
                break;
            case (int)GameId.shcd:
                {
                    name = TableName.SHCDCARDS_ROOM;
                }
                break;
            default:
                break;
        }
        return name;
    }
}

//////////////////////////////////////////////////////////////////////////
// 统计用户活跃--用户各游戏在线时长，及平均游戏时长分布
class StatGameTimeForPlayerActive : StatByDayBase
{
    public override void init()
    {
        base.init();
    }

    public override string getStatKey()
    {
        return StatKey.KEY_GAME_TIME_FOR_PLAYER_ACTIVE;
    }

    public override void update(double delta)
    {
        if (DateTime.Now.Date < m_statDay)
            return;

        beginStat("StatGameTimeForPlayerActive开始统计");

        DateTime time = m_statDay.Date.AddDays(-1);
        IMongoQuery imq = Query.EQ("genTime", time);

        // 各游戏平均在线时间
        MapReduceResult mapResult = MongodbLog.Instance.executeMapReduce(TableName.PUMP_GAME_TIME_FOR_PLAYER,
                                                                        imq,
                                                                        MapReduceTable.getMap("gameTimeForPlayerFavor"),
                                                                        MapReduceTable.getReduce("gameTimeForPlayerFavor"));
        if (mapResult != null)
        {
            GameTimeForPlayerFavorBase stat = new GameTimeForPlayerFavorBase();
            stat.m_time = time;
            IEnumerable<BsonDocument> bson = mapResult.GetResults();

            foreach (BsonDocument d in bson)
            {
                stat.reset();
                int playerType = Convert.ToInt32(d["_id"]); // 用户类型 1活跃用户  2付费用户
                BsonValue resValue = d["value"];
                stat.m_playerCount = resValue["playerCount"].ToInt32();

                for (int k = 0; k < StrName.s_gameName.Length; k++)
                {
                    stat.addGameTime(playerType, k, resValue["game" + k].ToInt64());
                }

                addFavorData(playerType, stat);
            }
        }

        statDistributionData(imq, PlayerType.TYPE_ACTIVE, time);
        statDistributionData(imq, PlayerType.TYPE_RECHARGE, time);
        statDistributionData(imq, PlayerType.TYPE_NEW, time);

        addStatDay();

        endStat("StatGameTimeForPlayerActive结束统计");
    }

    void addFavorData(int playerType, GameTimeForPlayerFavorBase stat)
    {
        Dictionary<string, object> newData = new Dictionary<string, object>();
        newData.Add("genTime", stat.m_time);
        newData.Add("playerType", playerType);
        newData.Add("playerCount", stat.m_playerCount);

        Dictionary<int, long> gtime = stat.getGameTime(playerType);
        foreach(var d in gtime)
        {
            newData.Add("game" + d.Key.ToString(), d.Value);
        }

        MongodbLog.Instance.ExecuteInsert(TableName.STAT_GAME_TIME_FOR_PLAYER_FAVOR_RESULT, newData);
    }

    void statDistributionData(IMongoQuery imq, int playerType, DateTime time)
    {
        IMongoQuery cond = imq;
        switch (playerType)
        {
            case PlayerType.TYPE_ACTIVE:
                break;
            case PlayerType.TYPE_RECHARGE:
                {
                    cond = Query.And(cond, Query.EQ("isRecharge", true));
                }
                break;
            case PlayerType.TYPE_NEW:
                {
                    IMongoQuery imq1 = Query.LT("createTime", m_statDay);
                    IMongoQuery imq2 = Query.GTE("createTime", time);
                    cond = Query.And(cond, imq1, imq2);
                }
                break;
        }
        // 平均游戏时长分布统计
        MapReduceResult mapResult1 = MongodbLog.Instance.executeMapReduce(TableName.PUMP_GAME_TIME_FOR_PLAYER,
                                                                        cond,
                                                                        MapReduceTable.getMap("gameTimeDistribution"),
                                                                        MapReduceTable.getReduce("gameTimeDistribution"));
        if (mapResult1 != null)
        {
            GameTimeForDistributionBase stat = new GameTimeForDistributionBase();
            stat.m_time = time;
            IEnumerable<BsonDocument> bson = mapResult1.GetResults();

            foreach (BsonDocument d in bson)
            {
                stat.m_gameId = Convert.ToInt32(d["_id"]);
                BsonValue resValue = d["value"];
                stat.m_Less10s = resValue["Less10s"].ToInt32();
                stat.m_Less30s = resValue["Less30s"].ToInt32();
                stat.m_Less60s = resValue["Less60s"].ToInt32();
                stat.m_Less5min = resValue["Less5min"].ToInt32();
                stat.m_Less10min = resValue["Less10min"].ToInt32();
                stat.m_Less30min = resValue["Less30min"].ToInt32();
                stat.m_Less60min = resValue["Less60min"].ToInt32();
                stat.m_GT60min = resValue["GT60min"].ToInt32();

                addDistributionData(stat, playerType);
            }
        }
    }

    // 在线时长分布数据
    void addDistributionData(GameTimeForDistributionBase stat, int playerType)
    {
        Dictionary<string, object> newData = new Dictionary<string, object>();
        newData.Add("genTime", stat.m_time);
        newData.Add("playerType", playerType);
        newData.Add("gameId", stat.m_gameId);
        newData.Add("Less10s", stat.m_Less10s);
        newData.Add("Less30s", stat.m_Less30s);
        newData.Add("Less60s", stat.m_Less60s);
        newData.Add("Less5min", stat.m_Less5min);
        newData.Add("Less10min", stat.m_Less10min);
        newData.Add("Less30min", stat.m_Less30min);
        newData.Add("Less60min", stat.m_Less60min);
        newData.Add("GT60min", stat.m_GT60min);

        MongodbLog.Instance.ExecuteInsert(TableName.STAT_GAME_TIME_FOR_DISTRIBUTION_RESULT, newData);
    }
}

//////////////////////////////////////////////////////////////////////////
// 首付行为 -- 首付游戏时长分布, 首次购买计费点分布
class StatFirstRecharge : StatByDayBase
{
    public override void init()
    {
        base.init();
    }

    public override string getStatKey()
    {
        return StatKey.KEY_FIRST_RECHARGE_DISTRIBUTION;
    }

    public override void update(double delta)
    {
        if (DateTime.Now.Date < m_statDay)
            return;

        beginStat("StatFirstRecharge开始统计");

        DateTime startTime = m_statDay.Date.AddDays(-1);
        IMongoQuery imq1 = Query.LT("firstRechargeTime", m_statDay);
        IMongoQuery imq2 = Query.GTE("firstRechargeTime", startTime);
        IMongoQuery imq = Query.And(imq1, imq2);

        // 首付游戏时长分布
        MapReduceResult mapResult = MongodbLog.Instance.executeMapReduce(TableName.PUMP_RECHARGE_FIRST,
                                                                        imq,
                                                                        MapReduceTable.getMap("firstRechargeGameTimeDistribution"),
                                                                        MapReduceTable.getReduce("firstRechargeGameTimeDistribution"));
        if (mapResult != null)
        {
            FirstRechargeGameTimeDistributionBase stat = new FirstRechargeGameTimeDistributionBase();
            IEnumerable<BsonDocument> bson = mapResult.GetResults();

            foreach (BsonDocument d in bson)
            {
                int playerType = Convert.ToInt32(d["_id"]); // 用户类型 1活跃用户  2付费用户
                BsonValue resValue = d["value"];
                stat.m_Less1min = resValue["Less1min"].ToInt32();
                stat.m_Less10min = resValue["Less10min"].ToInt32();
                stat.m_Less30min = resValue["Less30min"].ToInt32();
                stat.m_Less60min = resValue["Less60min"].ToInt32();
                stat.m_Less3h = resValue["Less3h"].ToInt32();
                stat.m_Less5h = resValue["Less5h"].ToInt32();
                stat.m_Less12h = resValue["Less12h"].ToInt32();
                stat.m_Less24h = resValue["Less24h"].ToInt32();
                stat.m_GT24h = resValue["GT24h"].ToInt32();

                addGameGameTimeDistribution(playerType, stat, startTime);
            }
        }

        // 首次购买计费点分布
        MapReduceResult mapResult1 = MongodbLog.Instance.executeMapReduce(TableName.PUMP_RECHARGE_FIRST,
                                                                            imq,
                                                                            MapReduceTable.getMap("firstRechargePointDistribution"),
                                                                            MapReduceTable.getReduce("firstRechargePointDistribution"));
        if (mapResult1 != null)
        {
            FirstRechargePointDistribution stat = new FirstRechargePointDistribution();
            IEnumerable<BsonDocument> bson = mapResult1.GetResults();

            foreach (BsonDocument d in bson)
            {
                stat.reset();
                int playerType = Convert.ToInt32(d["_id"]); 
                BsonValue resValue = d["value"];
                BsonDocument bdoc = (BsonDocument)resValue;
                foreach (var elem in bdoc.Elements)
                {
                    int payPoint = Convert.ToInt32(elem.Name);
                    int playerCount = elem.Value.ToInt32();
                    stat.add(payPoint, playerCount);
                }
                addFirstRechargePointDistribution(playerType, stat, startTime);
            }
        }

        addStatDay();

        endStat("StatFirstRecharge结束统计");
    }

    void addGameGameTimeDistribution(int playerType, FirstRechargeGameTimeDistributionBase stat, DateTime curTime)
    {
        Dictionary<string, object> newData = new Dictionary<string, object>();
        newData.Add("genTime", curTime);
        newData.Add("playerType", playerType);
        newData.Add("Less1min", stat.m_Less1min);
        newData.Add("Less10min", stat.m_Less10min);
        newData.Add("Less30min", stat.m_Less30min);
        newData.Add("Less60min", stat.m_Less60min);
        newData.Add("Less3h", stat.m_Less3h);
        newData.Add("Less5h", stat.m_Less5h);
        newData.Add("Less12h", stat.m_Less12h);
        newData.Add("Less24h", stat.m_Less24h);
        newData.Add("GT24h", stat.m_GT24h);

        MongodbLog.Instance.ExecuteInsert(TableName.STAT_FIRST_RECHARGE_GAME_TIME_DISTRIBUTION_RESULT, newData);
    }

    void addFirstRechargePointDistribution(int playerType, FirstRechargePointDistribution stat, DateTime curTime)
    {
        Dictionary<string, object> newData = new Dictionary<string, object>();
        newData.Add("genTime", curTime);
        newData.Add("playerType", playerType);
        foreach (var d in stat.m_point)
        {
            newData.Add(d.Key.ToString(), d.Value);
        }

        MongodbLog.Instance.ExecuteInsert(TableName.STAT_FIRST_RECHARGE_POINT_DISTRIBUTION_RESULT, newData);
    }
}

//////////////////////////////////////////////////////////////////////////
// 用户下注情况统计
// 用户的游戏币携带量（查看每局携带量和最大携带量、最小携带量）、下注量（平均、最大、最小）、当日流水。
class StatPlayerGameBet : StatByDayBase
{
    public override void init()
    {
        base.init();
    }

    public override string getStatKey()
    {
        return StatKey.KEY_PLAYER_GAME_BET;
    }

    public override void update(double delta)
    {
        if (DateTime.Now.Date < m_statDay)
            return;

        beginStat("StatPlayerGameBet开始统计");

        DateTime startTime = m_statDay.Date.AddDays(-1);
        IMongoQuery imq1 = Query.LT("genTime", m_statDay);
        IMongoQuery imq2 = Query.GTE("genTime", startTime);
        IMongoQuery imq = Query.And(imq1, imq2, Query.EQ("reason", (int)PropertyReasonType.type_reason_single_round_balance));

        for (int i = 2; i < StrName.s_onlineGameIdList.Length; i++)
        {
            int gameId = StrName.s_onlineGameIdList[i];
            IMongoQuery tmpImq = Query.And(imq, Query.EQ("gameId", gameId));
            MapReduceResult mapResult = MongodbLog.Instance.executeMapReduce(TableName.PUMP_PLAYER_MONEY,
                                                                       tmpImq,
                                                                       MapReduceTable.getMap("userGameBet"),
                                                                       MapReduceTable.getReduce("userGameBet"));
            if (mapResult != null)
            {
                IEnumerable<BsonDocument> bson = mapResult.GetResults();

                foreach (BsonDocument d in bson)
                {
                    int playerId = Convert.ToInt32(d["_id"]);
                    BsonValue resValue = d["value"];

                    try
                    {
                        stat(resValue["1"], gameId, playerId, startTime, 1);
                    }
                    catch (System.Exception ex)
                    {	
                    }
                    try
                    {
                        stat(resValue["14"], gameId, playerId, startTime, 14);
                    }
                    catch (System.Exception ex)
                    {
                    }
                }
            }
        }
       
        addStatDay();

        endStat("StatPlayerGameBet结束统计");
    }

    void stat(BsonValue value, int gameId, int playerId, DateTime curTime, int itemId)
    {
        int round = value["round"].ToInt32();
        if (round == 0)
            return;

        Dictionary<string, object> newData = new Dictionary<string, object>();
        newData.Add("sumCarry", value["sumStart"].ToInt64());
        newData.Add("maxCarry", value["maxStart"].ToInt64());
        newData.Add("minCarry", value["minStart"].ToInt64());

        newData.Add("sumOutlay", value["sumOutlay"].ToInt64());
        newData.Add("maxOutlay", value["maxOutlay"].ToInt64());
        newData.Add("minOutlay", value["minOutlay"].ToInt64());

        newData.Add("sumWin", value["sumWin"].ToInt64());
        newData.Add("maxWin", value["maxWin"].ToInt64());
        newData.Add("minWin", value["minWin"].ToInt64());

        newData.Add("sumLose", value["sumLose"].ToInt64());
        newData.Add("maxLose", value["maxLose"].ToInt64());
        newData.Add("minLose", value["minLose"].ToInt64());

        newData.Add("round", round);

        IMongoQuery imq1 = Query.EQ("genTime", curTime);
        IMongoQuery imq2 = Query.EQ("playerId", playerId);
        IMongoQuery imq3 = Query.EQ("gameId", gameId);
        IMongoQuery imq4 = Query.EQ("itemId", itemId);
        IMongoQuery imq = Query.And(imq1, imq2, imq3, imq4);

        MongodbLog.Instance.ExecuteUpdateByQuery(TableName.STAT_PLAYER_GAME_BET_RESULT, imq, newData);
    }
}
