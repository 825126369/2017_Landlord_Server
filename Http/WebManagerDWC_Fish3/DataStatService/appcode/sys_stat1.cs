using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

//////////////////////////////////////////////////////////////////////////
public class IncomeExInfo
{
    public long m_start;
    public long m_end;
}

// 总收支表 新
class StatPlayerTotalIncomeExpensesNew : StatByDayBase
{
    public override string getStatKey()
    {
        return StatKey.KEY_INCOME_EXPENSES_NEW;
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
        DateTime startTime = m_statDay.AddDays(-1);
        DateTime endTime = m_statDay;
        IMongoQuery imq1 = Query.LT("genTime", BsonValue.Create(endTime));
        IMongoQuery imq2 = Query.GTE("genTime", BsonValue.Create(startTime));
        IMongoQuery imq = Query.And(imq1, imq2);

        beginStat("StatPlayerTotalIncomeExpensesNew MapReduce开始统计");

        MapReduceResult mapResult = MongodbLog.Instance.executeMapReduce(TableName.PUMP_PLAYER_MONEY,
                                                                        imq,
                                                                        MapReduceTable.getMap("incomeExpensesNew"),
                                                                        MapReduceTable.getReduce("incomeExpensesNew"));
        endStat("StatPlayerTotalIncomeExpensesNew MapReduce结束统计");

        if (mapResult != null)
        {
            beginStat("StatPlayerTotalIncomeExpensesNew 开始写入数据");
            IEnumerable<BsonDocument> bson = mapResult.GetResults();
            StatIncomeExpensesItemBase item = new StatIncomeExpensesItemBase();
            foreach (BsonDocument d in bson)
            {
                BsonValue resValue = d["value"];
                BsonDocument bd = (BsonDocument)resValue;
                foreach (var gameName in bd.Names)
                {
                    int gameId = Convert.ToInt32(gameName);
                 
                    BsonDocument gameData = (BsonDocument)bd[gameName];
                    foreach (var itemId in gameData.Names)
                    {
                        Dictionary<string, object> upData = new Dictionary<string, object>();
                        upData.Add("genTime", startTime);
                        upData.Add("gameId", gameId);
                        upData.Add("itemId", Convert.ToInt32(itemId));

                        BsonDocument itemData =(BsonDocument) gameData[itemId];
                        
                        bool isAdd = false;
                        foreach (var ulkey in itemData.Names)
                        {
                            BsonDocument uldate = (BsonDocument)itemData[ulkey];
                            long income = uldate["income"].ToInt64();
                            long outlay = uldate["outlay"].ToInt64();
                            if (income > 0)
                            {
                                upData.Add(ulkey + "z", income);
                                isAdd = true;
                            }
                            if (outlay > 0)
                            {
                                upData.Add(ulkey + "f", outlay);
                                isAdd = true;
                            }
                        }

                        if (isAdd)
                        {
                            MongodbLog.Instance.ExecuteInsert(TableName.STAT_INCOME_EXPENSES_NEW, upData);
                        }
                    }
                }
            }

            endStat("StatPlayerTotalIncomeExpensesNew 结束写入数据");
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

                Dictionary<int, IncomeExInfo> incomeInfo = calStartRemain(imq);

                addDataRemain(item, startTime, incomeInfo);
            }
        }
    }

    void addDataRemain(StatIncomeExpensesItemBase item, DateTime time, Dictionary<int, IncomeExInfo> incomeInfo)
    {
        IMongoQuery imq = Query.EQ("genTime", BsonValue.Create(time));

        Dictionary<string, object> data = new Dictionary<string, object>();

        data.Add("goldRemain", item.m_goldRemain);
        data.Add("gemRemain", item.m_gemRemain);
        data.Add("dbRemain", item.m_dbRemain);
        data.Add("chipRemain", item.m_chipRemain);

        IncomeExInfo info = incomeInfo[1];
        data.Add("dayGoldStart", info.m_start);
        data.Add("dayGoldRemain", info.m_end);

        info = incomeInfo[2];
        data.Add("dayGemStart", info.m_start);
        data.Add("dayGemRemain", info.m_end);

        info = incomeInfo[11];
        data.Add("dayChipStart", info.m_start);
        data.Add("dayChipRemain", info.m_end);

        info = incomeInfo[14];
        data.Add("dayDbStart", info.m_start);
        data.Add("dayDbRemain", info.m_end);
        
        info = incomeInfo[-1];
        data.Add("dayPlayerCount", info.m_start);

        MongodbLog.Instance.ExecuteUpdateByQuery(TableName.STAT_INCOME_EXPENSES_REMAIN, imq, data);
    }


    Dictionary<int, IncomeExInfo> calStartRemain(IMongoQuery imq)
    {
        Dictionary<int, IncomeExInfo> ret = new Dictionary<int, IncomeExInfo>();

        MapReduceResult mapResult = MongodbLog.Instance.executeMapReduce(TableName.PUMP_PLAYER_MONEY,
                                                                imq,
                                                                MapReduceTable.getMap("iteratorPlayer"),
                                                                MapReduceTable.getReduce("iteratorPlayer"));
        if (mapResult != null)
        {
            IEnumerable<BsonDocument> bson = mapResult.GetResults();
            foreach (BsonDocument d in bson)
            {
                int playerId = Convert.ToInt32(d["_id"]);

                sr(playerId, 1, imq, ret);
                sr(playerId, 2, imq, ret);
                sr(playerId, 11, imq, ret);
                sr(playerId, 14, imq, ret);
            }

            IncomeExInfo info = new IncomeExInfo();
            info.m_start = bson.Count();
            ret[-1] = info;
        }

        return ret;
    }

    void sr(int playerId, int itemId, IMongoQuery imq, Dictionary<int, IncomeExInfo> ret)
    {
        int delta = 0;
        IncomeExInfo info = null;
        if (ret.ContainsKey(itemId))
        {
            info = ret[itemId];
        }
        else
        {
            info = new IncomeExInfo();
            ret.Add(itemId, info);
        }

        if (itemId == 1)
        {
            info.m_start += getGoldStartVal(imq, playerId);
            info.m_end += geGoldtLastVal(imq, playerId);
        }
        else
        {
            info.m_start += StatPlayerDragonBall.getStartValNew(imq, itemId, playerId);
            info.m_end += StatPlayerDragonBall.getLastValNew(imq, itemId, playerId, ref delta);
        }
    }

    public static string[] FIELDS = { "newValue", "oldValue", "gameId", "reason" };

    public static int getGoldStartVal(IMongoQuery imqCond, int playerId)
    {
        IMongoQuery imq = Query.And(imqCond, Query.EQ("itemId", 1), Query.EQ("playerId", playerId));

        List<Dictionary<string, object>> dataList1 = MongodbLog.Instance.ExecuteGetListByQuery(TableName.PUMP_PLAYER_MONEY,
            imq, FIELDS, "genTime", true, 0, 1);
        if (!valid(dataList1))
            return 0;

        int gameId = Convert.ToInt32(dataList1[0]["gameId"]);
        if (gameId != (int)GameId.fishlord)
        {
            return Convert.ToInt32(dataList1[0]["oldValue"]);
        }

        int reason = Convert.ToInt32(dataList1[0]["reason"]);
        if (reason == (int)PropertyReasonType.type_reason_single_round_balance)
        {
            return Convert.ToInt32(dataList1[0]["oldValue"]);
        }

        imq = Query.And(imq, Query.EQ("gameId", (int)GameId.fishlord), Query.EQ("reason", (int)PropertyReasonType.type_reason_single_round_balance));

        List<Dictionary<string, object>> dataList2 = MongodbLog.Instance.ExecuteGetListByQuery(TableName.PUMP_PLAYER_MONEY,
            imq, FIELDS, "genTime", true, 0, 1);
        if (!valid(dataList2))
        {
            return Convert.ToInt32(dataList1[0]["oldValue"]);
        }

        return Convert.ToInt32(dataList2[0]["oldValue"]);
    }

    public static int geGoldtLastVal(IMongoQuery imqCond, int playerId)
    {
        IMongoQuery imq = Query.And(imqCond, Query.EQ("itemId", 1), Query.EQ("playerId", playerId));

        List<Dictionary<string, object>> dataList1 = MongodbLog.Instance.ExecuteGetListByQuery(TableName.PUMP_PLAYER_MONEY,
             imq, FIELDS, "genTime", false, 0, 1);

        if (!valid(dataList1))
            return 0;

        int gameId = Convert.ToInt32(dataList1[0]["gameId"]);
        if (gameId != (int)GameId.fishlord)
        {
            return Convert.ToInt32(dataList1[0]["oldValue"]);
        }

        int reason = Convert.ToInt32(dataList1[0]["reason"]);
        if (reason == (int)PropertyReasonType.type_reason_single_round_balance)
        {
            return Convert.ToInt32(dataList1[0]["oldValue"]);
        }

        imq = Query.And(imq, Query.EQ("gameId", (int)GameId.fishlord), Query.EQ("reason", (int)PropertyReasonType.type_reason_single_round_balance));

        List<Dictionary<string, object>> dataList2 = MongodbLog.Instance.ExecuteGetListByQuery(TableName.PUMP_PLAYER_MONEY,
            imq, FIELDS, "genTime", true, 0, 1);
        if (!valid(dataList2))
        {
            return Convert.ToInt32(dataList1[0]["oldValue"]);
        }

        return Convert.ToInt32(dataList2[0]["oldValue"]);
    }

    static bool valid(List<Dictionary<string, object>> dataList)
    {
        if (dataList == null || dataList.Count == 0)
            return false;

        return true;
    }
}

//////////////////////////////////////////////////////////////////////////
// 当日新增用户金币下注分布
class StatNewPlayerOutlayDistribution : StatByDayBase
{
    public override void init()
    {
        base.init();
    }

    public override string getStatKey()
    {
        return StatKey.KEY_NEW_PLAYER_OUTLAY_DISTRIBUTION;
    }

    public override void update(double delta)
    {
        if (DateTime.Now.Date < m_statDay)
            return;

        beginStat("StatNewPlayerOutlayDistribution开始统计");

        DateTime startTime = m_statDay.Date.AddDays(-1);
        IMongoQuery imq1 = Query.LT("genTime", m_statDay);
        IMongoQuery imq2 = Query.GTE("genTime", startTime);
        // pumpPlayerMoney表，需新增playerCreateTime字段
        IMongoQuery imq3 = Query.LT("playerCreateTime", m_statDay);
        IMongoQuery imq4 = Query.GTE("playerCreateTime", startTime);
        IMongoQuery imq = Query.And(imq1, imq2, imq3, imq4,
            Query.EQ("reason", (int)PropertyReasonType.type_reason_single_round_balance));

        MapReduceResult mapResult = MongodbLog.Instance.executeMapReduce(TableName.PUMP_PLAYER_MONEY,
                                                                    imq,
                                                                    MapReduceTable.getMap("newPlayerOutlay"),
                                                                    MapReduceTable.getReduce("newPlayerOutlay"),
                                                                    TableName.STAT_NEW_PLAYER_OUTLAY_DISTRIBUTION + "_map");
        if (mapResult != null)
        {
            MapReduceResult mapResult1 = MongodbLog.Instance.executeMapReduce(TableName.STAT_NEW_PLAYER_OUTLAY_DISTRIBUTION + "_map",
                                                                                null,
                                                                                MapReduceTable.getMap("newPlayerOutlayDistribution"),
                                                                                MapReduceTable.getReduce("newPlayerOutlayDistribution"));
            if (mapResult1 != null)
            {
                IEnumerable<BsonDocument> bson = mapResult1.GetResults();
                foreach (BsonDocument d in bson)
                {
                    BsonDocument resValue = (BsonDocument)d["value"];
                    Dictionary<string, object> upData = new Dictionary<string, object>();
                    upData.Add("genTime", startTime);
                    foreach (var key in resValue.Names)
                    {
                        int num = resValue[key].ToInt32();
                        upData.Add(key, num);
                    }

                    MongodbLog.Instance.ExecuteInsert(TableName.STAT_NEW_PLAYER_OUTLAY_DISTRIBUTION, upData);
                }
            }
        }

        addStatDay();

        endStat("StatNewPlayerOutlayDistribution结束统计");
    }
}

//////////////////////////////////////////////////////////////////////////
// 当日新增炮数成长，捕鱼活跃分布
class StatNewPlayerLevelFishActivity : StatByDayBase
{
    public override void init()
    {
        base.init();
    }

    public override string getStatKey()
    {
        return StatKey.KEY_NEW_PLAYER_LEVEL_FISH_ACTIVITY;
    }

    public override void update(double delta)
    {
        if (DateTime.Now.Date < m_statDay)
            return;

        beginStat("StatNewPlayerLevelFishActivity开始统计");

        DateTime startTime = m_statDay.Date.AddDays(-1);
        IMongoQuery imq1 = Query.LT("genTime", m_statDay);
        IMongoQuery imq2 = Query.GTE("genTime", startTime);
        IMongoQuery imq = Query.And(imq1, imq2);

        MapReduceResult mapResult = MongodbLog.Instance.executeMapReduce(TableName.PUMP_NEW_PLAYER_FIRECOUNT_FISHLEVEL,
                                                                    imq,
                                                                    MapReduceTable.getMap("newPlayerFireCountDistribution"),
                                                                    MapReduceTable.getReduce("newPlayerFireCountDistribution"));
        if (mapResult != null)
        {
            IEnumerable<BsonDocument> bson = mapResult.GetResults();
            foreach (BsonDocument d in bson)
            {
                BsonDocument resValue = (BsonDocument)d["value"];
                Dictionary<string, object> upData = new Dictionary<string, object>();
                upData.Add("genTime", startTime);
                foreach (var key in resValue.Names)
                {
                    int num = resValue[key].ToInt32();
                    upData.Add(key, num);
                }

                MongodbLog.Instance.ExecuteInsert(TableName.STAT_NEW_PLAYER_FIRECOUNT_DISTRIBUTION, upData);
            }
        }

        //////////////////////////////////////////////////////////////////////////
        MapReduceResult mapResult1 = MongodbLog.Instance.executeMapReduce(TableName.PUMP_NEW_PLAYER_FIRECOUNT_FISHLEVEL,
                                                                    imq,
                                                                    MapReduceTable.getMap("newPlayerFishLevelDistribution"),
                                                                    MapReduceTable.getReduce("newPlayerFishLevelDistribution"));
        if (mapResult1 != null)
        {
            IEnumerable<BsonDocument> bson = mapResult1.GetResults();
            foreach (BsonDocument d in bson)
            {
                BsonDocument resValue = (BsonDocument)d["value"];
                Dictionary<string, object> upData = new Dictionary<string, object>();
                upData.Add("genTime", startTime);
                foreach (var key in resValue.Names)
                {
                    int num = resValue[key].ToInt32();
                    upData.Add(key, num);
                }

                MongodbLog.Instance.ExecuteInsert(TableName.STAT_NEW_PLAYER_FISHLEVEL_DISTRIBUTION, upData);
            }
        }

        addStatDay();

        endStat("StatNewPlayerLevelFishActivity结束统计");
    }
}

