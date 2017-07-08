using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

public class DistributionData
{
    // 游戏id-->数据
    public Dictionary<int, GameTimeForDistributionBase> m_data = new Dictionary<int, GameTimeForDistributionBase>();

    public void add(GameTimeForDistributionBase data)
    {
        if (m_data.ContainsKey(data.m_gameId))
            return;

        m_data.Add(data.m_gameId, data);
    }
}

public class ResultDistribution
{
    public Dictionary<DateTime, DistributionData> m_data = new Dictionary<DateTime, DistributionData>();

    public void addData(DateTime time, GameTimeForDistributionBase data)
    {
        DistributionData d = null;
        if (m_data.ContainsKey(time))
        {
            d = m_data[time];
        }
        else
        {
            d = new DistributionData();
            m_data.Add(time, d);
        }
        d.add(data);
    }

    public void reset()
    {
        m_data.Clear();
    }

    public string toJson()
    {
        string str = "";
        Dictionary<string, object> ret = new Dictionary<string, object>();

        foreach (var d in m_data)
        {
            List<Dictionary<string, object>> List = new List<Dictionary<string, object>>();

            foreach (var dn in d.Value.m_data)
            {
                Dictionary<string, object> tmp = new Dictionary<string, object>();
                List.Add(tmp);
                tmp.Add("gameId", dn.Value.m_gameId);
                tmp.Add("Less10s", dn.Value.m_Less10s);
                tmp.Add("Less30s", dn.Value.m_Less30s);
                tmp.Add("Less60s", dn.Value.m_Less60s);
                tmp.Add("Less5min", dn.Value.m_Less5min);
                tmp.Add("Less10min", dn.Value.m_Less10min);
                tmp.Add("Less30min", dn.Value.m_Less30min);
                tmp.Add("Less60min", dn.Value.m_Less60min);
                tmp.Add("GT60min", dn.Value.m_GT60min);
            }

            ret.Add(d.Key.ToShortDateString(), List);
        }

        str = ItemHelp.genJsonStr(ret);
        return str;
    }
}

// 平均游戏时长分布
public class QueryGameTimeDistribution : QueryBase
{
    ResultDistribution m_result = new ResultDistribution();

    public override OpRes doQuery(object param, GMUser user)
    {
        m_result.reset();
        ParamQuery p = (ParamQuery)param;
        if (string.IsNullOrEmpty(p.m_time))
            return OpRes.op_res_time_format_error;

        DateTime mint = DateTime.Now, maxt = DateTime.Now;
        bool res = Tool.splitTimeStr(p.m_time, ref mint, ref maxt);
        if (!res)
            return OpRes.op_res_time_format_error;

        int playerType = 1;
        if (!int.TryParse(p.m_param, out playerType))
        {
            return OpRes.op_res_param_not_valid;
        }

        IMongoQuery imq1 = Query.LT("genTime", BsonValue.Create(maxt));
        IMongoQuery imq2 = Query.GTE("genTime", BsonValue.Create(mint));
        IMongoQuery imq3 = Query.GTE("playerType", playerType);

        IMongoQuery imq = Query.And(imq1, imq2, imq3);

        return query(p, imq, user);
    }

    // 返回查询结果
    public override object getQueryResult()
    {
        return m_result;
    }

    private OpRes query(ParamQuery param, IMongoQuery imq, GMUser user)
    {
        List<Dictionary<string, object>> dataList =
             DBMgr.getInstance().executeQuery(TableName.STAT_GAME_TIME_FOR_DISTRIBUTION_RESULT, user.getDbServerID(), DbName.DB_PUMP, imq,
             0, 0, null, "genTime", false);

        if (dataList == null || dataList.Count == 0)
        {
            return OpRes.op_res_not_found_data;
        }

        int i = 0;

        for (i = 0; i < dataList.Count; i++)
        {
            Dictionary<string, object> data = dataList[i];
            DateTime t = Convert.ToDateTime(data["genTime"]).ToLocalTime();

            GameTimeForDistributionBase tmp = new GameTimeForDistributionBase();
            tmp.m_gameId = Convert.ToInt32(data["gameId"]);
            tmp.m_Less10s = Convert.ToInt32(data["Less10s"]);
            tmp.m_Less30s = Convert.ToInt32(data["Less30s"]);
            tmp.m_Less60s = Convert.ToInt32(data["Less60s"]);
            tmp.m_Less5min = Convert.ToInt32(data["Less5min"]);
            tmp.m_Less10min = Convert.ToInt32(data["Less10min"]);
            tmp.m_Less30min = Convert.ToInt32(data["Less30min"]);
            tmp.m_Less60min = Convert.ToInt32(data["Less60min"]);
            tmp.m_GT60min = Convert.ToInt32(data["GT60min"]);
            m_result.addData(t, tmp);
        }

        return OpRes.opres_success;
    }
}

//////////////////////////////////////////////////////////////////////////
public class ResultGameTimePlayerFavor
{
    public Dictionary<DateTime, GameTimeForPlayerFavorBase> m_data = new Dictionary<DateTime, GameTimeForPlayerFavorBase>();

    public GameTimeForPlayerFavorBase addData(DateTime dt, int playerType, int gameId, long time)
    {
        GameTimeForPlayerFavorBase d = null;
        if (m_data.ContainsKey(dt))
        {
            d = m_data[dt];
        }
        else
        {
            d = new GameTimeForPlayerFavorBase();
            m_data.Add(dt, d);
        }

        d.addGameTime(playerType, gameId, time);
        return d;
    }

    public void reset()
    {
        m_data.Clear();
    }

    public string toJson()
    {
        string str = "";
        Dictionary<string, object> data = new Dictionary<string, object>();
        Dictionary<string, object> ret1 = toJson(1);
        Dictionary<string, object> ret2 = toJson(2);
        data.Add("1", ret1);
        data.Add("2", ret2);
        str = ItemHelp.genJsonStr(data);
        return str;
    }

    Dictionary<string, object> toJson(int playerType)
    {
        Dictionary<string, object> ret = new Dictionary<string, object>();

        foreach (var d in m_data)
        {
            Dictionary<string, object> tmp = new Dictionary<string, object>();
            tmp.Add("playerCount", d.Value.m_playerCount);

            var secData = d.Value.getGameTime(playerType);

            foreach (var dn in secData)
            {
                tmp.Add("game" + dn.Key, dn.Value);
            }

            ret.Add(d.Key.ToShortDateString(), tmp);
        }
        return ret;
    }
}

// 用户喜好--平均在线时长
public class QueryGameTimePlayerFavor : QueryBase
{
    ResultGameTimePlayerFavor m_result = new ResultGameTimePlayerFavor();

    public override OpRes doQuery(object param, GMUser user)
    {
        m_result.reset();
        ParamQuery p = (ParamQuery)param;
        if (string.IsNullOrEmpty(p.m_time))
            return OpRes.op_res_time_format_error;

        DateTime mint = DateTime.Now, maxt = DateTime.Now;
        bool res = Tool.splitTimeStr(p.m_time, ref mint, ref maxt);
        if (!res)
            return OpRes.op_res_time_format_error;

        IMongoQuery imq1 = Query.LT("genTime", BsonValue.Create(maxt));
        IMongoQuery imq2 = Query.GTE("genTime", BsonValue.Create(mint));

        IMongoQuery imq = Query.And(imq1, imq2);

        return query(p, imq, user);
    }

    // 返回查询结果
    public override object getQueryResult()
    {
        return m_result;
    }

    private OpRes query(ParamQuery param, IMongoQuery imq, GMUser user)
    {
        List<Dictionary<string, object>> dataList =
             DBMgr.getInstance().executeQuery(TableName.STAT_GAME_TIME_FOR_PLAYER_FAVOR_RESULT, user.getDbServerID(), DbName.DB_PUMP, imq,
             0, 0, null, "genTime", true);

        if (dataList == null || dataList.Count == 0)
        {
            return OpRes.op_res_not_found_data;
        }

        int i = 0;

        for (i = 0; i < dataList.Count; i++)
        {
            Dictionary<string, object> data = dataList[i];
            DateTime t = Convert.ToDateTime(data["genTime"]).ToLocalTime();
            int playerType = Convert.ToInt32(data["playerType"]);
            int playerCount = Convert.ToInt32(data["playerCount"]);
            GameTimeForPlayerFavorBase gpf = m_result.addData(t, playerType, 0, 0);
            gpf.m_playerCount = playerCount;

            for (int k = 0; k < StrName.s_gameName.Length; k++)
            {
                if (data.ContainsKey("game" + k.ToString()))
                {
                    long count = Convert.ToInt64(data["game" + k.ToString()]);
                    m_result.addData(t, playerType, k, count);
                }
            }
        }

        return OpRes.opres_success;
    }
}

//////////////////////////////////////////////////////////////////////////
public class PlayerTypeData<TData>
{
    // 用户类型-->首付玩家时长人数信息
    public Dictionary<int, TData> m_items = new Dictionary<int, TData>();

    public void addData(int playerType, TData data)
    {
        m_items.Add(playerType, data);
    }

    public TData getData(int playerType)
    {
        if (m_items.ContainsKey(playerType))
            return m_items[playerType];

        return default(TData);
    }
}

public class PlayerTypeDataCollect<TData> where TData : new()
{
    public Dictionary<DateTime, PlayerTypeData<TData>> m_data = new Dictionary<DateTime, PlayerTypeData<TData>>();

    public PlayerTypeData<TData> addData(DateTime dt, int playerType, TData data)
    {
        PlayerTypeData<TData> d = default(PlayerTypeData<TData>);
        if (m_data.ContainsKey(dt))
        {
            d = m_data[dt];
        }
        else
        {
            d = new PlayerTypeData<TData>();
            m_data.Add(dt, d);
        }

        d.addData(playerType, data);
        return d;
    }

    public void reset()
    {
        m_data.Clear();
    }

    public IOrderedEnumerable<KeyValuePair<DateTime, PlayerTypeData<TData>>> getAllDescByTime()
    {
        var arr = from s in m_data
                  orderby s.Key descending
                  select s;
        return arr;
    }
}

public class ResultFirstRechargeGameTimeDistribution : PlayerTypeDataCollect<FirstRechargeGameTimeDistributionBase>
{
    public string toJson()
    {
        string str = "";
        Dictionary<string, object> data = new Dictionary<string, object>();
        Dictionary<string, object> ret1 = toJson(PlayerType.TYPE_NEW);
        Dictionary<string, object> ret2 = toJson(PlayerType.TYPE_ACTIVE);
        data.Add(PlayerType.TYPE_NEW.ToString(), ret1);
        data.Add(PlayerType.TYPE_ACTIVE.ToString(), ret2);
        str = ItemHelp.genJsonStr(data);
        return str;
    }

    Dictionary<string, object> toJson(int playerType)
    {
        Dictionary<string, object> ret = new Dictionary<string, object>();

        foreach (var d in m_data)
        {
            FirstRechargeGameTimeDistributionBase elem = d.Value.getData(playerType);
            if (elem != null)
            {
                Dictionary<string, object> tmp = new Dictionary<string, object>();
                tmp.Add("Less1min", elem.m_Less1min);
                tmp.Add("Less10min", elem.m_Less10min);
                tmp.Add("Less30min", elem.m_Less30min);
                tmp.Add("Less60min", elem.m_Less60min);
                tmp.Add("Less3h", elem.m_Less3h);
                tmp.Add("Less5h", elem.m_Less5h);
                tmp.Add("Less12h", elem.m_Less12h);
                tmp.Add("Less24h", elem.m_Less24h);
                tmp.Add("GT24h", elem.m_GT24h);
                ret.Add(d.Key.ToShortDateString(), tmp);
            }
        }
        return ret;
    }
}

// 首付游戏时长分布
public class QueryFirstRechargeGameTimeDistribution : QueryBase
{
    ResultFirstRechargeGameTimeDistribution m_result = new ResultFirstRechargeGameTimeDistribution();

    public override OpRes doQuery(object param, GMUser user)
    {
        m_result.reset();
        ParamQuery p = (ParamQuery)param;
        if (string.IsNullOrEmpty(p.m_time))
            return OpRes.op_res_time_format_error;

        DateTime mint = DateTime.Now, maxt = DateTime.Now;
        bool res = Tool.splitTimeStr(p.m_time, ref mint, ref maxt);
        if (!res)
            return OpRes.op_res_time_format_error;

        IMongoQuery imq1 = Query.LT("genTime", BsonValue.Create(maxt));
        IMongoQuery imq2 = Query.GTE("genTime", BsonValue.Create(mint));

        IMongoQuery imq = Query.And(imq1, imq2);

        return query(p, imq, user);
    }

    // 返回查询结果
    public override object getQueryResult()
    {
        return m_result;
    }

    private OpRes query(ParamQuery param, IMongoQuery imq, GMUser user)
    {
        List<Dictionary<string, object>> dataList =
             DBMgr.getInstance().executeQuery(TableName.STAT_FIRST_RECHARGE_GAME_TIME_DISTRIBUTION_RESULT, user.getDbServerID(), DbName.DB_PUMP, imq,
             0, 0, null, "genTime", true);

        if (dataList == null || dataList.Count == 0)
        {
            return OpRes.op_res_not_found_data;
        }

        int i = 0;

        for (i = 0; i < dataList.Count; i++)
        {
            Dictionary<string, object> data = dataList[i];
            DateTime t = Convert.ToDateTime(data["genTime"]).ToLocalTime();
            int playerType = Convert.ToInt32(data["playerType"]);

            FirstRechargeGameTimeDistributionBase tmp = new FirstRechargeGameTimeDistributionBase();
            tmp.m_Less1min = Convert.ToInt32(data["Less1min"]);
            tmp.m_Less10min = Convert.ToInt32(data["Less10min"]);
            tmp.m_Less30min = Convert.ToInt32(data["Less30min"]);
            tmp.m_Less60min = Convert.ToInt32(data["Less60min"]);
            tmp.m_Less3h = Convert.ToInt32(data["Less3h"]);
            tmp.m_Less5h = Convert.ToInt32(data["Less5h"]);
            tmp.m_Less12h = Convert.ToInt32(data["Less12h"]);
            tmp.m_Less24h = Convert.ToInt32(data["Less24h"]);
            tmp.m_GT24h = Convert.ToInt32(data["GT24h"]);
            m_result.addData(t, playerType, tmp);
        }

        return OpRes.opres_success;
    }
}

//////////////////////////////////////////////////////////////////////////
public class ResultFirstRechargePointDistribution : PlayerTypeDataCollect<FirstRechargePointDistribution>
{
    public string toJson()
    {
        string str = "";
        Dictionary<string, object> data = new Dictionary<string, object>();
        Dictionary<string, object> ret1 = toJson(PlayerType.TYPE_NEW);
        Dictionary<string, object> ret2 = toJson(PlayerType.TYPE_ACTIVE);
        Dictionary<string, object> ret3 = getRechargePoint();
        data.Add(PlayerType.TYPE_NEW.ToString(), ret1);
        data.Add(PlayerType.TYPE_ACTIVE.ToString(), ret2);
        data.Add("payPoint", ret3);
        str = ItemHelp.genJsonStr(data);
        return str;
    }

    Dictionary<string, object> toJson(int playerType)
    {
        Dictionary<string, object> ret = new Dictionary<string, object>();

        foreach (var d in m_data)
        {
            FirstRechargePointDistribution elem = d.Value.getData(playerType);
            if (elem != null)
            {
                Dictionary<string, object> tmp = new Dictionary<string, object>();

                foreach (var pi in elem.m_point)
                {
                    tmp.Add(pi.Key.ToString(), pi.Value);
                }
                ret.Add(d.Key.ToShortDateString(), tmp);
            }
        }
        return ret;
    }

    Dictionary<string, object> getRechargePoint()
    {
        Dictionary<string, object> ret = new Dictionary<string, object>();
        var allData = RechargeCFG.getInstance().getAllData();
        foreach (var d in allData)
        {
            ret.Add(d.Key.ToString(), d.Value.m_name);
        }
        return ret;
    }
}

// 首次购买计费点分布
public class QueryFirstRechargePointDistribution : QueryBase
{
    ResultFirstRechargePointDistribution m_result = new ResultFirstRechargePointDistribution();

    public override OpRes doQuery(object param, GMUser user)
    {
        m_result.reset();
        ParamQuery p = (ParamQuery)param;
        if (string.IsNullOrEmpty(p.m_time))
            return OpRes.op_res_time_format_error;

        DateTime mint = DateTime.Now, maxt = DateTime.Now;
        bool res = Tool.splitTimeStr(p.m_time, ref mint, ref maxt);
        if (!res)
            return OpRes.op_res_time_format_error;

        IMongoQuery imq1 = Query.LT("genTime", BsonValue.Create(maxt));
        IMongoQuery imq2 = Query.GTE("genTime", BsonValue.Create(mint));

        IMongoQuery imq = Query.And(imq1, imq2);

        return query(p, imq, user);
    }

    // 返回查询结果
    public override object getQueryResult()
    {
        return m_result;
    }

    private OpRes query(ParamQuery param, IMongoQuery imq, GMUser user)
    {
        List<Dictionary<string, object>> dataList =
             DBMgr.getInstance().executeQuery(TableName.STAT_FIRST_RECHARGE_POINT_DISTRIBUTION_RESULT, user.getDbServerID(), DbName.DB_PUMP, imq,
             0, 0, null, "genTime", true);

        if (dataList == null || dataList.Count == 0)
        {
            return OpRes.op_res_not_found_data;
        }

        int i = 0;

        for (i = 0; i < dataList.Count; i++)
        {
            Dictionary<string, object> data = dataList[i];
            DateTime t = Convert.ToDateTime(data["genTime"]).ToLocalTime();
            int playerType = Convert.ToInt32(data["playerType"]);

            FirstRechargePointDistribution tmp = new FirstRechargePointDistribution();
            m_result.addData(t, playerType, tmp);

            data.Remove("genTime");
            data.Remove("playerType");
            data.Remove("_id");
            foreach (var d in data)
            {
                tmp.add(Convert.ToInt32(d.Key), Convert.ToInt32(d.Value));
            }
        }

        return OpRes.opres_success;
    }
}

//////////////////////////////////////////////////////////////////////////
public class ParamPlayerGameBet : ParamQuery
{
    public int m_gameId;
}

public class ResultItemPlayerGameBet
{
    public const int CARRY = 0;
    public const int OUTLAY = 1;
    public const int WIN = 2;
    public const int LOSE = 3;

    public DateTime m_time;
    public ItemBasePlayerGameBet[] m_data = new ItemBasePlayerGameBet[4];
    public int m_round;
    public int m_playerId;
    public int m_gameId;
    public long m_rw;
    public void addData(int type, double sum, long max, long min)
    {
        m_data[type] = new ItemBasePlayerGameBet();
        m_data[type].m_max = max;
        m_data[type].m_min = min;
        m_data[type].m_sum = sum;
    }

    public string getAve(int type)
    {
        return ItemHelp.getRate(m_data[type].m_sum, m_round, 2);
    }

    public string getMax(int type)
    {
        return m_data[type].m_max.ToString();
    }

    public string getMin(int type)
    {
        return m_data[type].m_min.ToString();
    }

    public string getRw()
    {
        if(m_playerId > 0)
            return m_data[OUTLAY].m_sum.ToString();
        return m_rw.ToString();
    }

    public string getPlayerId()
    {
        if (m_playerId > 0)
            return m_playerId.ToString();

        return "总计";
    }
}

// 用户下注情况查询
public class QueryPlayerGameBet : QueryBase
{
    List<ResultItemPlayerGameBet> m_result = new List<ResultItemPlayerGameBet>();

    public override OpRes doQuery(object param, GMUser user)
    {
        m_result.Clear();
        ParamPlayerGameBet p = (ParamPlayerGameBet)param;
        if (string.IsNullOrEmpty(p.m_time))
            return OpRes.op_res_time_format_error;

        DateTime mint = DateTime.Now, maxt = DateTime.Now;
        bool res = Tool.splitTimeStr(p.m_time, ref mint, ref maxt);
        if (!res)
            return OpRes.op_res_time_format_error;

        int playerId = 0;
        if (!string.IsNullOrEmpty(p.m_param))
        {
            if (!int.TryParse(p.m_param, out playerId))
                return OpRes.op_res_param_not_valid;
        }

        IMongoQuery imq1 = Query.LT("genTime", BsonValue.Create(maxt));
        IMongoQuery imq2 = Query.GTE("genTime", BsonValue.Create(mint));
        IMongoQuery imq3 = Query.EQ("itemId", 1);
        IMongoQuery imq = Query.And(imq1, imq2, imq3, Query.EQ("gameId", p.m_gameId));
        if (playerId > 0)
        {
            imq = Query.And(imq, Query.EQ("playerId", playerId));
            return query(p, imq, user);
        }

        return statAve(imq, user);
    }

    // 返回查询结果
    public override object getQueryResult()
    {
        return m_result;
    }

    private OpRes query(ParamQuery param, IMongoQuery imq, GMUser user)
    {
        List<Dictionary<string, object>> dataList =
             DBMgr.getInstance().executeQuery(TableName.STAT_PLAYER_GAME_BET_RESULT, user.getDbServerID(), DbName.DB_PUMP, imq,
             0, 0, null, "genTime", false);

        if (dataList == null || dataList.Count == 0)
        {
            return OpRes.op_res_not_found_data;
        }

        int i = 0;

        for (i = 0; i < dataList.Count; i++)
        {
            ResultItemPlayerGameBet tmp = new ResultItemPlayerGameBet();
            m_result.Add(tmp);

            Dictionary<string, object> data = dataList[i];
            tmp.m_time = Convert.ToDateTime(data["genTime"]).ToLocalTime();
            tmp.m_round = Convert.ToInt32(data["round"]);
            tmp.m_playerId = Convert.ToInt32(data["playerId"]);
            tmp.m_gameId = Convert.ToInt32(data["gameId"]);

            long max = Convert.ToInt64(data["maxCarry"]);
            long min = Convert.ToInt64(data["minCarry"]);
            long sum = Convert.ToInt64(data["sumCarry"]);
            tmp.addData(ResultItemPlayerGameBet.CARRY, sum, max, min);

            max = Convert.ToInt64(data["maxOutlay"]);
            min = Convert.ToInt64(data["minOutlay"]);
            sum = Convert.ToInt64(data["sumOutlay"]);
            tmp.addData(ResultItemPlayerGameBet.OUTLAY, sum, max, min);

            max = Convert.ToInt64(data["maxWin"]);
            min = Convert.ToInt64(data["minWin"]);
            sum = Convert.ToInt64(data["sumWin"]);
            tmp.addData(ResultItemPlayerGameBet.WIN, sum, max, min);

            max = Convert.ToInt64(data["maxLose"]);
            min = Convert.ToInt64(data["minLose"]);
            sum = Convert.ToInt64(data["sumLose"]);
            tmp.addData(ResultItemPlayerGameBet.LOSE, sum, max, min);
        }

        return OpRes.opres_success;
    }

    OpRes statAve(IMongoQuery imq, GMUser user)
    {
        m_result.Clear();
        MapReduceResult mapResult = DBMgr.getInstance().executeMapReduce(TableName.STAT_PLAYER_GAME_BET_RESULT,
                                                                         user.getDbServerID(),
                                                                         DbName.DB_PUMP,
                                                                         imq,
                                                                         MapReduceTable.getMap("userGameBet"),
                                                                         MapReduceTable.getReduce("userGameBet"));
        if (mapResult != null)
        {
            IEnumerable<BsonDocument> bson = mapResult.GetResults();

            foreach (BsonDocument d in bson)
            {
                ResultItemPlayerGameBet tmp = new ResultItemPlayerGameBet();
                m_result.Add(tmp);

                BsonValue resValue = d["value"];

                tmp.m_time = Convert.ToDateTime(d["_id"]).ToLocalTime();
                tmp.m_round = resValue["playerCount"].ToInt32();

                long max = resValue["maxCarry"].ToInt64();
                long min = resValue["minCarry"].ToInt64();
                double sum = resValue["sumCarry"].ToDouble();
                tmp.addData(ResultItemPlayerGameBet.CARRY, sum, max, min);

                max = resValue["maxOutlay"].ToInt64();
                min = resValue["minOutlay"].ToInt64();
                sum = resValue["sumOutlay"].ToDouble();
                tmp.addData(ResultItemPlayerGameBet.OUTLAY, sum, max, min);

                max = resValue["maxWin"].ToInt64();
                min = resValue["minWin"].ToInt64();
                sum = resValue["sumWin"].ToDouble();
                tmp.addData(ResultItemPlayerGameBet.WIN, sum, max, min);

                max = resValue["maxLose"].ToInt64();
                min = resValue["minLose"].ToInt64();
                sum = resValue["sumLose"].ToDouble();
                tmp.addData(ResultItemPlayerGameBet.LOSE, sum, max, min);

                tmp.m_rw = resValue["rw"].ToInt64();
            }
        }

        return OpRes.opres_success;
    }
}

//////////////////////////////////////////////////////////////////////////
public class GameIncomeExpensesItem
{
    // 游戏id
    public int m_gameId;
    // 原因-->收入，支出
    public Dictionary<int, TInfo<long, long>> m_data =
        new Dictionary<int, TInfo<long, long>>();

    public void add(int reason, long income, long outlay)
    {
        TInfo<long, long> tmp = null;
        if (m_data.ContainsKey(reason))
        {
            tmp = m_data[reason];
        }
        else
        {
            tmp = new TInfo<long, long>();
            m_data.Add(reason, tmp);
        }
        tmp.m_first += income;
        tmp.m_second += outlay;
    }

    Dictionary<string, object> _dataReason(long income, long outlay)
    {
        Dictionary<string, object> data = new Dictionary<string, object>();
        data.Add("income", income);
        data.Add("outlay", outlay);
        return data;
    }

    public Dictionary<string, object> dataGame()
    {
        Dictionary<string, object> data = new Dictionary<string, object>();
        foreach (var d in m_data)
        {
            data.Add(d.Key.ToString(), _dataReason(d.Value.m_first, d.Value.m_second));
        }

        return data;
    }
}

public class ResultIncomeExpensesItem
{
    public List<GameIncomeExpensesItem> m_data = new List<GameIncomeExpensesItem>();
    public long m_startValue;
    public long m_remainValue;
    public long m_playerCount;

    public void add(int gameId, int reason, long income, long outlay)
    {
        GameIncomeExpensesItem item = null;
        for (int i = 0; i < m_data.Count; i++)
        {
            if (m_data[i].m_gameId == gameId)
            {
                item = m_data[i];
                break;
            }
        }
        if (item == null)
        {
            item = new GameIncomeExpensesItem();
            item.m_gameId = gameId;
            m_data.Add(item);
        }

        item.add(reason, income, outlay);
    }

    public void reset()
    {
        m_data.Clear();
    }

    public Dictionary<string, object> dataForJson()
    {
        Dictionary<string, object> data = new Dictionary<string, object>();
        foreach (var d in m_data)
        {
            data.Add(d.m_gameId.ToString(), d.dataGame());
        }
        return data;
    }
}

public class ResultPlayerIncomeExpenses
{
    public Dictionary<DateTime, ResultIncomeExpensesItem> m_data =
        new Dictionary<DateTime, ResultIncomeExpensesItem>();

    public void reset()
    {
        m_data.Clear();
    }

    public ResultIncomeExpensesItem add(DateTime time, int gameId, int reason, long income, long outlay)
    {
        ResultIncomeExpensesItem d = null;
        if (m_data.ContainsKey(time))
        {
            d = m_data[time];
        }
        else
        {
            d = new ResultIncomeExpensesItem();
            m_data.Add(time, d);
        }
        d.add(gameId, reason, income, outlay);
        return d;
    }

    public string getJson()
    {
        var arr = from s in m_data
                  orderby s.Key descending
                  select s;

        Dictionary<string, object> ret = new Dictionary<string, object>();

        foreach (var a in arr)
        {
            Dictionary<string, object> tmp = a.Value.dataForJson();
            tmp.Add("start", a.Value.m_startValue);
            tmp.Add("remain", a.Value.m_remainValue);
            tmp.Add("playerCount", a.Value.m_playerCount);
            ret.Add(a.Key.ToShortDateString(), tmp);
        }

        return ItemHelp.genJsonStr(ret);
    }
}

// 查询玩家收支统计
public class QueryPlayerIncomeExpenses : QueryBase
{
    ResultPlayerIncomeExpenses m_result = new ResultPlayerIncomeExpenses();

    static Dictionary<int, string[]> s_se = new Dictionary<int, string[]>();

    static QueryPlayerIncomeExpenses()
    {
        string[] info = new string[3];
        info[0] = "dayGoldStart";
        info[1] = "dayGoldRemain";
        info[2] = "dayPlayerCount";
        s_se.Add(1, info);

        info = new string[3];
        info[0] = "dayGemStart";
        info[1] = "dayGemRemain";
        info[2] = "dayPlayerCount";
        s_se.Add(2, info);

        info = new string[3];
        info[0] = "dayChipStart";
        info[1] = "dayChipRemain";
        info[2] = "dayPlayerCount";
        s_se.Add(11, info);

        info = new string[3];
        info[0] = "dayDbStart";
        info[1] = "dayDbRemain";
        info[2] = "dayPlayerCount";
        s_se.Add(14, info);
    }

    public override OpRes doQuery(object param, GMUser user)
    {
        m_result.reset();
        ParamQuery p = (ParamQuery)param;
        if (string.IsNullOrEmpty(p.m_time))
            return OpRes.op_res_time_format_error;

        DateTime mint = DateTime.Now, maxt = DateTime.Now;
        bool res = Tool.splitTimeStr(p.m_time, ref mint, ref maxt);
        if (!res)
            return OpRes.op_res_time_format_error;

        int itemId = 0;
        if (!int.TryParse(p.m_param, out itemId))
        {
            return OpRes.op_res_param_not_valid;
        }

        IMongoQuery imq1 = Query.LT("genTime", BsonValue.Create(maxt));
        IMongoQuery imq2 = Query.GTE("genTime", BsonValue.Create(mint));

        IMongoQuery imq = Query.And(imq1, imq2, Query.EQ("itemId", itemId));

        return query(p, imq, user, itemId);
    }

    // 返回查询结果
    public override object getQueryResult()
    {
        return m_result;
    }

    private OpRes query(ParamQuery param, IMongoQuery imq, GMUser user, int itemId)
    {
        List<Dictionary<string, object>> dataList =
             DBMgr.getInstance().executeQuery(TableName.STAT_INCOME_EXPENSES_NEW,
             user.getDbServerID(), DbName.DB_PUMP, imq,
             0, 0, null, "genTime", false);

        if (dataList == null || dataList.Count == 0)
        {
            return OpRes.op_res_not_found_data;
        }

        int i = 0;

        for (i = 0; i < dataList.Count; i++)
        {
            Dictionary<string, object> data = dataList[i];
            DateTime time = Convert.ToDateTime(data["genTime"]).ToLocalTime();
            int gameId = Convert.ToInt32(data["gameId"]);
            long income = 0, outlay = 0;
            bool isAdd = false;
            ResultIncomeExpensesItem ritem = null;

            foreach (var item in data)
            {
                income = outlay = 0;
                isAdd = false;
                if (item.Key[item.Key.Length - 1] == 'z')
                {
                    income = Convert.ToInt64(item.Value);
                    isAdd = true;
                }
                else if (item.Key[item.Key.Length - 1] == 'f')
                {
                    outlay = Convert.ToInt64(item.Value);
                    isAdd = true;
                }

                if (isAdd)
                {
                    int reason = Convert.ToInt32(item.Key.Substring(0, item.Key.Length - 1));
                    ritem = m_result.add(time, gameId, reason, income, outlay);
                }
            }

            if (ritem != null)
            {
                if (ritem.m_startValue == 0 &&
                    ritem.m_remainValue == 0)
                {
                    string[] fields = s_se[itemId];
                    Dictionary<string, object> sreamin =
                        DBMgr.getInstance().getTableData(TableName.STAT_INCOME_EXPENSES_REMAIN, "genTime", time,
                           fields, user.getDbServerID(), DbName.DB_PUMP);
                    if (sreamin != null)
                    {
                        if (sreamin.ContainsKey(fields[0]))
                        {
                            ritem.m_startValue = Convert.ToInt64(sreamin[fields[0]]);
                        }
                        if (sreamin.ContainsKey(fields[1]))
                        {
                            ritem.m_remainValue = Convert.ToInt64(sreamin[fields[1]]);
                        }
                        if (sreamin.ContainsKey(fields[2]))
                        {
                            ritem.m_playerCount = Convert.ToInt64(sreamin[fields[2]]);
                        }
                    }
                }
            }
        }

        return OpRes.opres_success;
    }
}

//////////////////////////////////////////////////////////////////////////
public class EnterRoomCount
{
    public DateTime m_time;

    public int[] m_enterCount = null;

    public void init(int count)
    {
        m_enterCount = new int[count];
    }

    public void setCount(int roomId, int count)
    {
        m_enterCount[roomId] = count;
    }

    public Dictionary<string, object> getDataForJson()
    {
        Dictionary<string, object> tmp = new Dictionary<string, object>();
        tmp.Add("time", m_time.ToShortDateString());
        for (int i = 0; i < m_enterCount.Length; i++)
        {
            tmp.Add(i.ToString(), m_enterCount[i]);
        }
        return tmp;
    }
}

public class ResultNewPlayer
{
    public List<EnterRoomCount> m_enterInfo = new List<EnterRoomCount>();

    public void reset()
    {
        m_enterInfo.Clear();
    }

    public void addEnterCount(DateTime time, int roomId, int count)
    {
        EnterRoomCount info = getEnterRoomCount(time, m_enterInfo, 5);
        info.setCount(roomId, count);
    }

    public void addFishLevelInfo(DateTime time, int fishLevel, int count)
    {
        EnterRoomCount info = getEnterRoomCount(time, m_enterInfo, 51);
        info.setCount(fishLevel, count);
    }

    public void addFireCountInfo(DateTime time, int index, int count)
    {
        EnterRoomCount info = getEnterRoomCount(time, m_enterInfo, 7);
        info.setCount(index, count);
    }

    public void addOutlayInfo(DateTime time, int index, int count)
    {
        EnterRoomCount info = getEnterRoomCount(time, m_enterInfo, 11);
        info.setCount(index, count);
    }

    public string toJson(string data)
    {
        string str = jsonForEnterRoom(m_enterInfo);
        return str;
    }

    string jsonForEnterRoom(List<EnterRoomCount> infoList)
    {
        Dictionary<string, object> data = new Dictionary<string, object>();
        List<Dictionary<string, object>> dataList = new List<Dictionary<string, object>>();
        data.Add("data", dataList);

        foreach (var info in infoList)
        {
            var d = info.getDataForJson();
            dataList.Add(d);
        }

        string str = ItemHelp.genJsonStr(data);
        return str;
    }

    EnterRoomCount getEnterRoomCount(DateTime time, List<EnterRoomCount> infoList, int initCount)
    {
        EnterRoomCount info = null;
        foreach (var t in infoList)
        {
            if (t.m_time == time)
            {
                info = t;
                break;
            }
        }
        if (info == null)
        {
            info = new EnterRoomCount();
            info.m_time = time;
            info.init(initCount);
            infoList.Add(info);
        }
        return info;
    }
}

// 新增用户分析
public class QueryNewPlayer : QueryBase
{
    ResultNewPlayer m_result = new ResultNewPlayer();

    static Dictionary<int, string[]> s_se = new Dictionary<int, string[]>();

    public override OpRes doQuery(object param, GMUser user)
    {
        m_result.reset();
        ParamQuery p = (ParamQuery)param;
        if (string.IsNullOrEmpty(p.m_time))
            return OpRes.op_res_time_format_error;

        DateTime mint = DateTime.Now, maxt = DateTime.Now;
        bool res = Tool.splitTimeStr(p.m_time, ref mint, ref maxt);
        if (!res)
            return OpRes.op_res_time_format_error;

        IMongoQuery imq1 = Query.LT("genTime", BsonValue.Create(maxt));
        IMongoQuery imq2 = Query.GTE("genTime", BsonValue.Create(mint));
        IMongoQuery imq = Query.And(imq1, imq2);

        OpRes code = OpRes.op_res_failed;

        switch (p.m_param)
        {
            case "1":
                {
                    code = queryFishLevelDistribution(p, imq, user);
                }
                break;
            case "2":
                {
                    code = queryOutlayDistribution(p, imq, user);
                }
                break;
            case "3":
                {
                    code = queryFireCountDistribution(p, imq, user);
                }
                break;
            case "4":
                {

                }
                break;
            case "5":
                {
                    code = queryEnterRoom(p, imq, user);
                }
                break;
        }

        return code;
    }

    // 返回查询结果
    public override object getQueryResult()
    {
        return m_result;
    }

    private OpRes queryEnterRoom(ParamQuery param, IMongoQuery imq, GMUser user)
    {
        List<Dictionary<string, object>> dataList =
             DBMgr.getInstance().executeQuery(TableName.STAT_NEW_PLAYER_ENTER_ROOM,
             user.getDbServerID(), DbName.DB_PUMP, imq,
             0, 0, null, "genTime", false);

        if (dataList == null || dataList.Count == 0)
        {
            return OpRes.op_res_not_found_data;
        }

        int i = 0;

        for (i = 0; i < dataList.Count; i++)
        {
            Dictionary<string, object> data = dataList[i];
            DateTime time = Convert.ToDateTime(data["genTime"]).ToLocalTime();
            int roomId = Convert.ToInt32(data["roomId"]);
            int enterCount = Convert.ToInt32(data["enterCount"]);
            m_result.addEnterCount(time, roomId, enterCount);
        }

        return OpRes.opres_success;
    }

    // 捕鱼等级分布
    private OpRes queryFishLevelDistribution(ParamQuery param, IMongoQuery imq, GMUser user)
    {
        List<Dictionary<string, object>> dataList =
             DBMgr.getInstance().executeQuery(TableName.STAT_NEW_PLAYER_FISHLEVEL_DISTRIBUTION,
             user.getDbServerID(), DbName.DB_PUMP, imq,
             0, 0, null, "genTime", false);

        if (dataList == null || dataList.Count == 0)
        {
            return OpRes.op_res_not_found_data;
        }

        int i = 0;

        for (i = 0; i < dataList.Count; i++)
        {
            Dictionary<string, object> data = dataList[i];
            DateTime time = Convert.ToDateTime(data["genTime"]).ToLocalTime();

            for (int k = 1; k <= 50; k++)
            {
                int val = Convert.ToInt32(data[k.ToString()]);
                m_result.addFishLevelInfo(time, k, val);
            }
        }

        return OpRes.opres_success;
    }

    // 捕鱼活跃统计分布
    private OpRes queryFireCountDistribution(ParamQuery param, IMongoQuery imq, GMUser user)
    {
        List<Dictionary<string, object>> dataList =
             DBMgr.getInstance().executeQuery(TableName.STAT_NEW_PLAYER_FIRECOUNT_DISTRIBUTION,
             user.getDbServerID(), DbName.DB_PUMP, imq,
             0, 0, null, "genTime", false);

        if (dataList == null || dataList.Count == 0)
        {
            return OpRes.op_res_not_found_data;
        }

        int i = 0;

        for (i = 0; i < dataList.Count; i++)
        {
            Dictionary<string, object> data = dataList[i];
            DateTime time = Convert.ToDateTime(data["genTime"]).ToLocalTime();

            for (int k = 0; k < 7; k++)
            {
                int val = Convert.ToInt32(data[k.ToString()]);
                m_result.addFireCountInfo(time, k, val);
            }
        }

        return OpRes.opres_success;
    }

    // 金币下注分布
    private OpRes queryOutlayDistribution(ParamQuery param, IMongoQuery imq, GMUser user)
    {
        List<Dictionary<string, object>> dataList =
             DBMgr.getInstance().executeQuery(TableName.STAT_NEW_PLAYER_OUTLAY_DISTRIBUTION,
             user.getDbServerID(), DbName.DB_PUMP, imq,
             0, 0, null, "genTime", false);

        if (dataList == null || dataList.Count == 0)
        {
            return OpRes.op_res_not_found_data;
        }

        int i = 0;

        for (i = 0; i < dataList.Count; i++)
        {
            Dictionary<string, object> data = dataList[i];
            DateTime time = Convert.ToDateTime(data["genTime"]).ToLocalTime();

            for (int k = 0; k < 11; k++)
            {
                int val = Convert.ToInt32(data[k.ToString()]);
                m_result.addOutlayInfo(time, k, val);
            }
        }

        return OpRes.opres_success;
    }
}



















