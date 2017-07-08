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
    DateTime m_last = DateTime.Now;

    public override void init() 
    {
        initStatModule();
        initChannel();
    }

    public override void update(double delta)
    {
        bool isNeed = true;
        List<ChannelInfo> channelList = ResMgr.getInstance().getChannelList();
        foreach (var info in channelList)
        {
            bool res = statChannel(info);
            if (res)
            {
                resetChannelStatDay(info);
                isNeed = false;
            }
        }

        if (isNeed)
        {
            TimeSpan span = DateTime.Now - m_last;
            if (span.TotalMinutes >= 5)
            {
                m_last = DateTime.Now;

                foreach (var info in channelList)
                {
                    statChannelByInterval(info);
                }   
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

        IMongoQuery imq = null;
        Dictionary<string, object> newData = getData(result, info, ref imq);

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

    private bool statChannelByInterval(ChannelInfo info)
    {
        //beginStat("渠道[{0}]开始统计", info.m_channelName);

        ParamStat param = new ParamStat();
        param.m_channel = info;

        StatResult result = new StatResult();

        foreach (var stat in m_statModule)
        {
            if (stat.Key == StatFlag.STAT_FLAG_REMAIN)
                continue;
            if (stat.Key == StatFlag.STAT_LTV)
                continue;

            stat.Value.doStat(param, result);
        }

        IMongoQuery imq = null;
        Dictionary<string, object> newData = getData(result, info, ref imq);

        string str = MongodbAccount.Instance.ExecuteStoreByQuery(TableName.CHANNEL_TD, imq, newData);

        //endStat("渠道[{0}]结束统计", info.m_channelName);
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
                        data.Add("2DayRemainCountRecharge", result.m_2DayRemainCountRechargeTmp);
                    }
                    break;
                case 3:
                    {
                        data.Add("3DayRemainCount", result.m_3DayRemainCountTmp);
                        data.Add("3DayRemainCountRecharge", result.m_3DayRemainCountRechargeTmp);
                    }
                    break;
                case 7:
                    {
                        data.Add("7DayRemainCount", result.m_7DayRemainCountTmp);
                        data.Add("7DayRemainCountRecharge", result.m_7DayRemainCountRechargeTmp);
                    }
                    break;
                case 30:
                    {
                        data.Add("30DayRemainCount", result.m_30DayRemainCountTmp);
                        data.Add("30DayRemainCountRecharge", result.m_30DayRemainCountRechargeTmp);
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

    Dictionary<string, object> getData(StatResult result, ChannelInfo info, ref IMongoQuery imq)
    {
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
        imq = Query.And(imq1, imq2);

        return newData;
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
       // DateTime now = DateTime.Now;
        //IMongoQuery imq1 = Query.GTE("logout_time", now.AddDays(-7));
        //IMongoQuery imq2 = Query.EQ("is_robot", false);
        //IMongoQuery imq = Query.And(imq1, imq2);
        IMongoQuery imq = Query.EQ("is_robot", false);
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
