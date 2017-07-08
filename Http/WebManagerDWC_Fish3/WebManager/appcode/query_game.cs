using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System.Text.RegularExpressions;
using System.Web.Configuration;
using System.Linq;

// 牛牛的参数查询结果
public class ResultParamCows : ResultFishlordExpRate
{
    public long m_burstZhuang;   // 爆庄支出

    public long m_serviceCharge; // 手续费

    // 总盈利率
    public string getTotalRate()
    {
        long totalIncome = m_totalIncome + m_serviceCharge;
        long totalOutlay = m_totalOutlay + m_burstZhuang;

        if (totalIncome == 0 && totalOutlay == 0)
            return "0";
        if (totalIncome == 0)
            return "-∞";

        double factGain = (double)(totalIncome - totalOutlay) / totalIncome;
        return Math.Round(factGain, 3).ToString();
    }
}

// 牛牛参数查询
public class QueryCowsParam : QueryBase
{
    private Dictionary<int, ResultParamCows> m_result = new Dictionary<int, ResultParamCows>();

    public override OpRes doQuery(object param, GMUser user)
    {
        m_result.Clear();
        return query(user);
    }

    public override object getQueryResult()
    {
        return m_result;
    }

    private OpRes query(GMUser user)
    {
        List<Dictionary<string, object>> dataList = DBMgr.getInstance().executeQuery(TableName.COWS_ROOM,
            user.getDbServerID(), DbName.DB_GAME);
        if (dataList == null)
            return OpRes.opres_success;
 
        for (int i = 0; i < dataList.Count; i++)
        {
            ResultParamCows info = new ResultParamCows();
            info.m_roomId = Convert.ToInt32(dataList[i]["room_id"]);
            if (dataList[i].ContainsKey("ExpectEarnRate"))
            {
                info.m_expRate = Convert.ToDouble(dataList[i]["ExpectEarnRate"]);
            }
            else
            {
                info.m_expRate = 0.05;
            }

            if (dataList[i].ContainsKey("room_income")) // 总收入
            {
                info.m_totalIncome = Convert.ToInt64(dataList[i]["room_income"]);
            }
            if (dataList[i].ContainsKey("room_outcome"))  // 总支出
            {
                info.m_totalOutlay = Convert.ToInt64(dataList[i]["room_outcome"]);
            }
            if (dataList[i].ContainsKey("BankerAddGold")) // 上庄手续费
            {
                info.m_serviceCharge = Convert.ToInt64(dataList[i]["BankerAddGold"]);
            }
            if (dataList[i].ContainsKey("BankerSubGold")) // 爆庄支出
            {
                info.m_burstZhuang = Convert.ToInt64(dataList[i]["BankerSubGold"]);
            }
            if (dataList[i].ContainsKey("TotalRobotWinGold")) // 机器人收入
            {
                info.m_robotIncome = Convert.ToInt64(dataList[i]["TotalRobotWinGold"]);
            }
            if (dataList[i].ContainsKey("TotalRobotLoseGold")) // 机器人支出
            {
                info.m_robotOutlay = Convert.ToInt64(dataList[i]["TotalRobotLoseGold"]);
            }
            if (dataList[i].ContainsKey("player_count"))
            {
                info.m_curPlayerCount = Convert.ToInt32(dataList[i]["player_count"]);
            }
            m_result.Add(info.m_roomId, info);
        }

        return OpRes.opres_success;
    }
}

//////////////////////////////////////////////////////////////////////////

public class ResultCowsCard : ParamAddCowsCard
{
    public string m_key = "";
    public string m_time = "";
}

// 牛牛牌型查询
public class QueryCowsCardsType : QueryBase
{
    private List<ResultCowsCard> m_result = new List<ResultCowsCard>();

    public override OpRes doQuery(object param, GMUser user)
    {
        m_result.Clear();
        return query(user);
    }

    public override object getQueryResult()
    {
        return m_result;
    }

    private OpRes query(GMUser user)
    {
        List<Dictionary<string, object>> dataList = DBMgr.getInstance().executeQuery(TableName.COWS_CARDS,
            user.getDbServerID(), DbName.DB_GAME, null, 0, 0, null, "insert_time", false);
        if (dataList == null)
            return OpRes.opres_success;

        for (int i = 0; i < dataList.Count; i++)
        {
            ResultCowsCard info = new ResultCowsCard();
            info.m_key = Convert.ToString(dataList[i]["key"]);
            info.m_time = Convert.ToDateTime(dataList[i]["insert_time"]).ToLocalTime().ToString();
            info.m_bankerType = Convert.ToInt32(dataList[i]["banker_cards"]);
            info.m_other1Type = Convert.ToInt32(dataList[i]["other_cards1"]);
            info.m_other2Type = Convert.ToInt32(dataList[i]["other_cards2"]);
            info.m_other3Type = Convert.ToInt32(dataList[i]["other_cards3"]);
            info.m_other4Type = Convert.ToInt32(dataList[i]["other_cards4"]);
            m_result.Add(info);
        }

        return OpRes.opres_success;
    }
}

//////////////////////////////////////////////////////////////////////////
// 游戏结果控制
public class ParamGameResultControl
{
    public GameId m_gameId;
}

// 查询游戏控制结果
public class QueryGameResultControl : QueryBase
{
    private Dictionary<GameId, QueryBase> m_games = new Dictionary<GameId, QueryBase>();

    public QueryGameResultControl()
    {
        m_games.Add(GameId.shcd, new QueryGameResultShcd());
    }

    public override OpRes doQuery(object param, GMUser user)
    {
        ParamGameResultControl p = (ParamGameResultControl)param;
        if (m_games.ContainsKey(p.m_gameId))
        {
            return m_games[p.m_gameId].doQuery(p, user);
        }
        return OpRes.op_res_failed;
    }

    public override object getQueryResult(object param, GMUser user)
    {
        ParamGameResultControl p = (ParamGameResultControl)param;
        if (m_games.ContainsKey(p.m_gameId))
        {
            return m_games[p.m_gameId].getQueryResult();
        }
        return null;
    }
}

public class GameResultShcd
{
    public string m_insertTime;
    public int m_result;
    public int m_roomId = 1; // 默认是金币场
}

public class QueryGameResultShcd : QueryBase
{
    List<GameResultShcd> m_result = new List<GameResultShcd>();

    public override OpRes doQuery(object param, GMUser user)
    {
        m_result.Clear();
        List<Dictionary<string, object>> dataList = DBMgr.getInstance().executeQuery(TableName.SHCD_RESULT,
              user.getDbServerID(), DbName.DB_GAME, null, 0, 0, null, "insert_time", false);
        if (dataList == null)
            return OpRes.opres_success;

        for (int i = 0; i < dataList.Count; i++)
        {
            Dictionary<string, object> data = dataList[i];
            GameResultShcd info = new GameResultShcd();
            info.m_insertTime = Convert.ToDateTime(data["insert_time"]).ToLocalTime().ToString();
            info.m_result = Convert.ToInt32(data["next_card_type"]);
            if (data.ContainsKey("room_id"))
            {
                info.m_roomId = Convert.ToInt32(data["room_id"]);
            }
            m_result.Add(info);
        }

        return OpRes.opres_success;
    }

    public override object getQueryResult()
    {
        return m_result;
    }
}

//////////////////////////////////////////////////////////////////////////
public class ResultExpRateParam
{
    public long m_totalIncome;
    public long m_totalOutlay;
    public double m_expRate;

    // 返回实际盈利率
    public double getFactExpRate()
    {
        if (m_totalIncome == 0)
            return 0;

        double factGain = (double)(m_totalIncome - m_totalOutlay) / m_totalIncome;
        return Math.Round(factGain, 3);
    }

    // 总盈亏
    public long getDelta()
    {
        return m_totalIncome - m_totalOutlay;
    }
}

public class ResultDragonParam : ResultExpRateParam
{
    public int m_roomId;
    public long m_doubleIncome;
    public long m_doubleOutcome;

    // 玩家个数
    public int m_curPlayerCount = 0;
}

// 五龙参数查询
public class QueryDragonParam : QueryBase
{
    private List<ResultDragonParam> m_result = new List<ResultDragonParam>();

    public override OpRes doQuery(object param, GMUser user)
    {
        m_result.Clear();
        return query(user);
    }

    public override object getQueryResult()
    {
        return m_result;
    }

    private OpRes query(GMUser user)
    {
        List<Dictionary<string, object>> dataList = DBMgr.getInstance().executeQuery(TableName.DRAGON_ROOM,
            user.getDbServerID(), DbName.DB_GAME);
        if (dataList == null)
            return OpRes.opres_success;

        for (int i = 0; i < dataList.Count; i++)
        {
            ResultDragonParam info = new ResultDragonParam();
            m_result.Add(info);

            info.m_roomId = Convert.ToInt32(dataList[i]["room_id"]);
            if (dataList[i].ContainsKey("expect_earn_rate"))
            {
                info.m_expRate = Convert.ToDouble(dataList[i]["expect_earn_rate"]);
            }
            else
            {
                info.m_expRate = 0.05;
            }

            if (dataList[i].ContainsKey("room_income")) // 总收入
            {
                info.m_totalIncome = Convert.ToInt64(dataList[i]["room_income"]);
            }
            if (dataList[i].ContainsKey("room_outcome"))  // 总支出
            {
                info.m_totalOutlay = Convert.ToInt64(dataList[i]["room_outcome"]);
            }
            if (dataList[i].ContainsKey("double_income"))  // 翻倍收入
            {
                info.m_doubleIncome = Convert.ToInt64(dataList[i]["double_income"]);
            }
            if (dataList[i].ContainsKey("double_outcome"))  // 翻倍支出
            {
                info.m_doubleOutcome = Convert.ToInt64(dataList[i]["double_outcome"]);
            }
            if (dataList[i].ContainsKey("player_count"))
            {
                info.m_curPlayerCount = Convert.ToInt32(dataList[i]["player_count"]);
            }
        }

        return OpRes.opres_success;
    }
}

//////////////////////////////////////////////////////////////////////////
public class ParamDragonGameModeEarning
{
    public int m_roomId;
    public string m_tableId;

    public ParamDragonGameModeEarning()
    {
        m_roomId = -1;
    }
}

public class ResultDragonGameModeEarning
{
    public const int MODE_NORMAL = 0;
    public const int MODE_FREE_GAME = 1;
    public const int MODE_DOUBLE = 2;
    public static string[] s_mode = { "普通模式", "freegame模式", "翻倍游戏" };

    private Dictionary<int, ResultExpRateParam> m_result =
        new Dictionary<int, ResultExpRateParam>();

    public void reset()
    {
        m_result.Clear();
    }

    public void addModeData(int mode, long income, long outlay)
    {
        ResultExpRateParam r = null;
        if (m_result.ContainsKey(mode))
        {
            r = m_result[mode];
        }
        else
        {
            r = new ResultExpRateParam();
            m_result.Add(mode, r);
        }

        r.m_totalIncome += income;
        r.m_totalOutlay += outlay;
    }

    public ResultExpRateParam getParam(int mode)
    {
        if (m_result.ContainsKey(mode))
            return m_result[mode];

        return null;
    }
}

// 五龙各游戏模式下的盈利率查询
public class QueryDragonGameModeEarning : QueryBase
{
    private ResultDragonGameModeEarning m_result = new ResultDragonGameModeEarning();

    public override OpRes doQuery(object param, GMUser user)
    {
        ParamDragonGameModeEarning p =(ParamDragonGameModeEarning)param;
        int tableId = -1;
        if (!string.IsNullOrEmpty(p.m_tableId) && 
            !int.TryParse(p.m_tableId, out tableId))
            return OpRes.op_res_param_not_valid;

        List<IMongoQuery> queryList = new List<IMongoQuery>();
        if (p.m_roomId >= 0)
        {
            queryList.Add(Query.EQ("room_id", BsonValue.Create(p.m_roomId)));
        }
        if (tableId >= 0)
        {
            queryList.Add(Query.EQ("table_id", BsonValue.Create(tableId)));
        }
     
        IMongoQuery imq = queryList.Count > 0 ? Query.And(queryList) : null;

        m_result.reset();
        return query(user, imq);
    }

    public override object getQueryResult()
    {
        return m_result;
    }

    private OpRes query(GMUser user, IMongoQuery imq)
    {
        List<Dictionary<string, object>> dataList = DBMgr.getInstance().executeQuery(TableName.DRAGON_TABLE,
            user.getDbServerID(), DbName.DB_GAME, imq);
        if (dataList == null)
            return OpRes.opres_success;

        long income = 0, outlay = 0;
        for (int i = 0; i < dataList.Count; i++)
        {
            income = outlay = 0;
            Dictionary<string, object> data = dataList[i];
            if (data.ContainsKey("table_income"))
            {
                income = Convert.ToInt64(data["table_income"]);
            }
            if (data.ContainsKey("normal_outcome"))
            {
                outlay = Convert.ToInt64(data["normal_outcome"]);
            }
            m_result.addModeData(ResultDragonGameModeEarning.MODE_NORMAL, income, outlay);

            income = outlay = 0;
            if (data.ContainsKey("free_outcome"))
            {
                outlay = Convert.ToInt64(data["free_outcome"]);
            }
            m_result.addModeData(ResultDragonGameModeEarning.MODE_FREE_GAME, income, outlay);

            income = outlay = 0;
            if (data.ContainsKey("double_income"))
            {
                income = Convert.ToInt64(data["double_income"]);
            }
            if (data.ContainsKey("double_outcome"))
            {
                outlay = Convert.ToInt64(data["double_outcome"]);
            }
            m_result.addModeData(ResultDragonGameModeEarning.MODE_DOUBLE, income, outlay);
        }

        return OpRes.opres_success;
    }
}

//////////////////////////////////////////////////////////////////////////
// 黑红梅方参数查询
public class ResultShcdParam : ResultExpRateParam
{
    public int m_roomId;
    public int m_level;
    public static string[] s_levelName = { "自动控制", "天堂", "普通", "困难", "超难", "最难", };
    public int m_jokerCount;
    public string m_cheatSE;
    // 玩家个数
    public int m_curPlayerCount = 0;

    public string getLevelName()
    {
        return s_levelName[m_level];
    }
}

public class QueryShcdParam : QueryBase
{
    private List<ResultShcdParam> m_result = new List<ResultShcdParam>();

    public override OpRes doQuery(object param, GMUser user)
    {
        m_result.Clear();
        return query(user);
    }

    public override object getQueryResult()
    {
        return m_result;
    }

    private OpRes query(GMUser user)
    {
        List<Dictionary<string, object>> dataList = DBMgr.getInstance().executeQuery(TableName.SHCDCARDS_ROOM,
            user.getDbServerID(), DbName.DB_GAME, null, 0, 0, null, "room_id", true);
        if (dataList == null)
            return OpRes.opres_success;

        for (int i = 0; i < dataList.Count; i++)
        {
            ResultShcdParam info = new ResultShcdParam();
            m_result.Add(info);

            info.m_roomId = Convert.ToInt32(dataList[i]["room_id"]);
            if (dataList[i].ContainsKey("ExpectEarnRate"))
            {
                info.m_expRate = Convert.ToDouble(dataList[i]["ExpectEarnRate"]);
            }
            else
            {
                info.m_expRate = 0.05;
            }

            if (dataList[i].ContainsKey("room_income")) // 总收入
            {
                info.m_totalIncome = Convert.ToInt64(dataList[i]["room_income"]);
            }
            if (dataList[i].ContainsKey("room_outcome"))  // 总支出
            {
                info.m_totalOutlay = Convert.ToInt64(dataList[i]["room_outcome"]);
            }

            if (dataList[i].ContainsKey("EarnRateControl"))
            {
                info.m_level = Convert.ToInt32(dataList[i]["EarnRateControl"]);
            }
            else
            {
                info.m_level = 0;
            }

            if (dataList[i].ContainsKey("next_joker_count"))
            {
                info.m_jokerCount = Convert.ToInt32(dataList[i]["next_joker_count"]);
            }
            else
            {
                info.m_jokerCount = 0;
            }

            if (dataList[i].ContainsKey("beginCheatIndex") && dataList[i].ContainsKey("endCheatIndex"))
            {
                info.m_cheatSE = Convert.ToString(dataList[i]["beginCheatIndex"]) + "-" +
                    Convert.ToString(dataList[i]["endCheatIndex"]);
            }
            else
            {
                info.m_cheatSE = "";
            }

            if (dataList[i].ContainsKey("player_count"))
            {
                info.m_curPlayerCount = Convert.ToInt32(dataList[i]["player_count"]);
            }
        }

        return OpRes.opres_success;
    }
}

public class ResultIndependentShcd
{
    Dictionary<int, ResultIndependent> m_roomData = new Dictionary<int, ResultIndependent>();

    public string getAreaName(int areaId)
    {
        return StrName.s_shcdArea[areaId - 1];
    }

    public ResultIndependent addRoom(int roomId)
    {
        if (m_roomData.ContainsKey(roomId))
            return m_roomData[roomId];

        ResultIndependent d = new ResultIndependent();
        m_roomData.Add(roomId, d);
        return d;
    }

    public ResultIndependent getRoomData(int roomId)
    {
        if (m_roomData.ContainsKey(roomId))
            return m_roomData[roomId];

        return null;
    }

    public void reset()
    {
        m_roomData.Clear();
    }
}

// 黑红梅方独立数据--各区域的下注，获奖情况
public class QueryIndependentShcd : QueryBase
{
    private ResultIndependentShcd m_result = new ResultIndependentShcd();

    public override OpRes doQuery(object param, GMUser user)
    {
        m_result.reset();
        return query(user);
    }

    public override object getQueryResult()
    {
        return m_result;
    }

    private OpRes query(GMUser user)
    {
        List<Dictionary<string, object>> dataList = DBMgr.getInstance().executeQuery(TableName.SHCDCARDS_ROOM,
            user.getDbServerID(), DbName.DB_GAME);

        for (int i = 0; i < dataList.Count; i++)
        {
            for (int k = 1; k <= StrName.s_shcdArea.Length; k++)
            {
                // 总收入
                string totalWinKey = string.Format("WinGold{0}", k);
                // 总支出
                string totalLoseGoldKey = string.Format("LoseGold{0}", k);
                // 押注次数
                string betCountKey = string.Format("BetCount{0}", k);

                long income = 0, outlay = 0, betCount = 0;
                if (dataList[i].ContainsKey(totalWinKey))
                {
                    income = Convert.ToInt64(dataList[i][totalWinKey]);
                }
                if (dataList[i].ContainsKey(totalLoseGoldKey))
                {
                    outlay = Convert.ToInt64(dataList[i][totalLoseGoldKey]);
                }
                if (dataList[i].ContainsKey(betCountKey))
                {
                    betCount = Convert.ToInt64(dataList[i][betCountKey]);
                }

                int roomId = Convert.ToInt32(dataList[i]["room_id"]);
                m_result.addRoom(roomId).addBetCount(k, betCount, 0, income, outlay);
            }
        }

        return OpRes.opres_success;
    }
}

//////////////////////////////////////////////////////////////////////////
public class ParamGameCalfRoping
{
    // 查询套牛的数据参数
    public const int QUERY_CONTROL_PARAM = 0;
    // 独立数据
    public const int QUERY_INDEPENDENT = 1;
    // 关卡数据
    public const int QUERY_LEVEL = 2;
    // 套中的牛的种类
    public const int QUERY_CALF = 3;

    // 查询内容 
    public int m_queryContent;
}

// 套牛独立数据
public class ResultIndependentCalfRoping
{
    // 进入房间次数
    public long m_enterCount;
    // 鼓励奖获得次数
    public long m_norRewardNum;
    // 鼓励奖奖金
    public long m_norRewardGold;
    // 大奖获得次数
    public long m_bigRewardNum;
    // 大奖发放奖金
    public long m_bigRewardGold;
    // 续关次数
    public long m_buyLifeNum;

    public void reset()
    {
        m_enterCount = 0;
        m_norRewardNum = 0;
        m_norRewardGold = 0;
        m_bigRewardNum = 0;
        m_bigRewardGold = 0;
        m_buyLifeNum = 0;
    }
}

// 套牛牛的分类统计
public class ResultCalfRopingLogItem
{
    // 关卡ID
    public int m_passId;
    // 牛的ID
    public int m_calfId;
    public long m_hitCount;

    public string getName()
    {
        CalfRoping_CalfCFGData val = CalfRoping_CalfCFG.getInstance().getValue(m_calfId);
        if (val != null)
        {
            return val.m_calfName;
        }
        return m_calfId.ToString();
    }
}

public class ResultCalfRopingLevelItem
{
    // 关卡ID
    public int m_passId;
    // 丢绳次数
    public long allcount;
    // 套中次数
    public long m_hitCount;

    // 返回命中率
    public string getHitRate()
    {
        return ItemHelp.getRate(m_hitCount, allcount);
    }
}

// 游戏套牛的相关查询
public class QueryGameCalfRoping : QueryBase
{
    // 套牛盈利率等参数
    private List<ResultExpRateParam> m_result = new List<ResultExpRateParam>();

    ResultIndependentCalfRoping m_result1 = new ResultIndependentCalfRoping();

    private List<ResultCalfRopingLogItem> m_result2 = new List<ResultCalfRopingLogItem>();

    private List<ResultCalfRopingLevelItem> m_result3 = new List<ResultCalfRopingLevelItem>();

    public override OpRes doQuery(object param, GMUser user)
    {
        OpRes res = OpRes.op_res_failed;
        ParamGameCalfRoping p = (ParamGameCalfRoping)param;
        switch (p.m_queryContent)
        {
            case ParamGameCalfRoping.QUERY_CONTROL_PARAM:
                {
                    m_result.Clear();
                    res = queryParam(user);
                }
                break;
            case ParamGameCalfRoping.QUERY_INDEPENDENT:
                {
                    m_result1.reset();
                    res = queryIndependent(user);
                }
                break;
            case ParamGameCalfRoping.QUERY_CALF:
                {
                    m_result2.Clear();
                    res = queryCalfStat(user);
                    m_result2.Sort(sortLevel);
                }
                break;
            case ParamGameCalfRoping.QUERY_LEVEL:
                {
                    m_result3.Clear();
                    res = queryLevel(user);
                    m_result3.Sort(sortLevel);
                }
                break;
        }

        return res;
    }

    public override object getQueryResult(object param, GMUser user)
    {
        ParamGameCalfRoping p = (ParamGameCalfRoping)param;
        switch (p.m_queryContent)
        {
            case ParamGameCalfRoping.QUERY_CONTROL_PARAM:
                {
                    return m_result;
                }
                break;
            case ParamGameCalfRoping.QUERY_INDEPENDENT:
                {
                    return m_result1;
                }
                break;
            case ParamGameCalfRoping.QUERY_CALF:
                {
                    return m_result2;
                }
                break;
            case ParamGameCalfRoping.QUERY_LEVEL:
                {
                    return m_result3;
                }
                break;
        }
        return null;
    }

    private OpRes queryParam(GMUser user)
    {
        List<Dictionary<string, object>> dataList = DBMgr.getInstance().executeQuery(TableName.CALF_ROPING_ROOM,
            user.getDbServerID(), DbName.DB_GAME);
        if (dataList == null)
            return OpRes.opres_success;

        for (int i = 0; i < dataList.Count; i++)
        {
            ResultExpRateParam info = new ResultExpRateParam();
            m_result.Add(info);

            if (dataList[i].ContainsKey("ExpectEarnRate"))
            {
                info.m_expRate = Convert.ToDouble(dataList[i]["ExpectEarnRate"]);
            }
            else
            {
                info.m_expRate = 0.05;
            }

            if (dataList[i].ContainsKey("lobby_income")) // 总收入
            {
                info.m_totalIncome = Convert.ToInt64(dataList[i]["lobby_income"]);
            }
            if (dataList[i].ContainsKey("lobby_outcome"))  // 总支出
            {
                info.m_totalOutlay = Convert.ToInt64(dataList[i]["lobby_outcome"]);
            }
        }

        return OpRes.opres_success;
    }

    private OpRes queryIndependent(GMUser user)
    {
        List<Dictionary<string, object>> dataList = DBMgr.getInstance().executeQuery(TableName.CALF_ROPING_ROOM,
            user.getDbServerID(), DbName.DB_GAME);
        if (dataList == null)
            return OpRes.opres_success;

        for (int i = 0; i < dataList.Count; i++)
        {
            Dictionary<string, object> data = dataList[i];

            if (data.ContainsKey("enter_count"))
            {
                m_result1.m_enterCount = Convert.ToInt64(data["enter_count"]);
            }
            if (data.ContainsKey("NorRewardNum"))
            {
                m_result1.m_norRewardNum = Convert.ToInt64(data["NorRewardNum"]);
            }
            if (data.ContainsKey("NorRewardGold"))
            {
                m_result1.m_norRewardGold = Convert.ToInt64(data["NorRewardGold"]);
            }
            if (data.ContainsKey("BigRewardNum"))
            {
                m_result1.m_bigRewardNum = Convert.ToInt64(data["BigRewardNum"]);
            }
            if (data.ContainsKey("BigRewardGold"))
            {
                m_result1.m_bigRewardGold = Convert.ToInt64(data["BigRewardGold"]);
            }
            if (data.ContainsKey("BuyLifeNum"))
            {
                m_result1.m_buyLifeNum = Convert.ToInt64(data["BuyLifeNum"]);
            }
        }

        return OpRes.opres_success;
    }

    // 套牛牛的分类统计
    private OpRes queryCalfStat(GMUser user)
    {
        List<Dictionary<string, object>> dataList = DBMgr.getInstance().executeQuery(TableName.CALF_ROPING_LOG,
            user.getDbServerID(), DbName.DB_PUMP);
        if (dataList == null)
            return OpRes.opres_success;

        for (int i = 0; i < dataList.Count; i++)
        {
            ResultCalfRopingLogItem item = new ResultCalfRopingLogItem();
            m_result2.Add(item);

            Dictionary<string, object> data = dataList[i];

            if (data.ContainsKey("passid"))
            {
                item.m_passId = Convert.ToInt32(data["passid"]);
            }
            if (data.ContainsKey("calfid"))
            {
                item.m_calfId = Convert.ToInt32(data["calfid"]);
            }
            if (data.ContainsKey("hitcount"))
            {
                item.m_hitCount = Convert.ToInt64(data["hitcount"]);
            }
        }

        return OpRes.opres_success;
    }

    private int sortLevel(ResultCalfRopingLogItem p1, ResultCalfRopingLogItem p2)
    {
        return p1.m_passId - p2.m_passId;
    }

    private int sortLevel(ResultCalfRopingLevelItem p1, ResultCalfRopingLevelItem p2)
    {
        return p1.m_passId - p2.m_passId;
    }

    private OpRes queryLevel(GMUser user)
    {
        List<Dictionary<string, object>> dataList = DBMgr.getInstance().executeQuery(TableName.CALF_ROPING_PASS_LOG,
            user.getDbServerID(), DbName.DB_PUMP);
        if (dataList == null)
            return OpRes.opres_success;

        for (int i = 0; i < dataList.Count; i++)
        {
            ResultCalfRopingLevelItem item = new ResultCalfRopingLevelItem();
            m_result3.Add(item);

            Dictionary<string, object> data = dataList[i];

            if (data.ContainsKey("passid"))
            {
                item.m_passId = Convert.ToInt32(data["passid"]);
            }
            if (data.ContainsKey("hitcount"))
            {
                item.m_hitCount = Convert.ToInt64(data["hitcount"]);
            }
            if (data.ContainsKey("allcount"))
            {
                item.allcount = Convert.ToInt64(data["allcount"]);
            }
        }

        return OpRes.opres_success;
    }
}

//////////////////////////////////////////////////////////////////////////
public class ParamGrandPrix
{
    public const int QUERY_WEEK_CHAMPION = 1;
    public const int QUERY_MATCH_DAY = 2;

    public int m_queryType;
    public object m_param;
}

public class ParamMatchDay
{
    public string m_matchDay;
    public string m_playerId;
    public bool m_isTop100;
}

public class ResultChampionItem
{
    public string m_time = "";
    public int m_playerId;
    public string m_nickName;
    public int m_bestScore;
}

public class ResultMatchDayItem : ResultChampionItem
{
    public int m_rank;
}

// 大奖赛相关查询
public class QueryGrandPrix : QueryBase
{
    private List<ResultChampionItem> m_result1 = new List<ResultChampionItem>();

    private List<ResultMatchDayItem> m_result2 = new List<ResultMatchDayItem>();

    public override OpRes doQuery(object param, GMUser user)
    {
        ParamGrandPrix p = (ParamGrandPrix)param;
        OpRes res = OpRes.op_res_failed;
        switch (p.m_queryType)
        {
            case ParamGrandPrix.QUERY_WEEK_CHAMPION:
                {
                    m_result1.Clear();
                    res = queryWeekChampion(user);
                }
                break;
            case ParamGrandPrix.QUERY_MATCH_DAY:
                {
                    m_result2.Clear();
                    ParamMatchDay pd = (ParamMatchDay)p.m_param;
                    if (pd.m_isTop100)
                    {
                        res = queryMatchDayTop100(user, pd);
                        m_result2.Sort(sortRank);
                    }
                    else
                    {
                        res = queryMatchDayForSpecialPlayer(user, pd);
                    }
                }
                break;
        }

        return res;
    }

    public override object getQueryResult(object param, GMUser user)
    {
        ParamGrandPrix p = (ParamGrandPrix)param;
        switch (p.m_queryType)
        {
            case ParamGrandPrix.QUERY_WEEK_CHAMPION:
                {
                    return m_result1;
                }
                break;
            case ParamGrandPrix.QUERY_MATCH_DAY:
                {
                    return m_result2;
                }
                break;
        }
        return null;
    }

    private OpRes queryWeekChampion(GMUser user)
    {
        List<Dictionary<string, object>> dataList = DBMgr.getInstance().executeQuery(TableName.MATCH_GRAND_PRIX_WEEK_CHAMPION,
            user.getDbServerID(), DbName.DB_GAME, null, 0, 0, null, "matchTime", false);
        if (dataList == null)
            return OpRes.opres_success;

        for (int i = 0; i < dataList.Count; i++)
        {
            ResultChampionItem info = new ResultChampionItem();
            info.m_time = Convert.ToDateTime(dataList[i]["matchTime"]).ToLocalTime().ToShortDateString();
            info.m_playerId = Convert.ToInt32(dataList[i]["playerId"]);
            info.m_nickName = Convert.ToString(dataList[i]["nickName"]);
            info.m_bestScore = Convert.ToInt32(dataList[i]["bestScore"]);
            m_result1.Add(info);
        }

        return OpRes.opres_success;
    }

    private OpRes queryMatchDayForSpecialPlayer(GMUser user, ParamMatchDay p)
    {
        DateTime mint = DateTime.Now, maxt = DateTime.Now;
        bool res = Tool.splitTimeStr(p.m_matchDay, ref mint, ref maxt);
        if (!res)
            return OpRes.op_res_time_format_error;

        int playerId = 0;
        if (!int.TryParse(p.m_playerId, out playerId))
            return OpRes.op_res_param_not_valid;

        IMongoQuery imq1 = Query.LT("matchTime", BsonValue.Create(maxt));
        IMongoQuery imq2 = Query.GTE("matchTime", BsonValue.Create(mint));
        IMongoQuery imq3 = Query.EQ("playerId", BsonValue.Create(playerId));
        var cond = Query.And(imq1, imq2, imq3);

        return queryMatchDay(user, cond);
    }

    private OpRes queryMatchDayTop100(GMUser user, ParamMatchDay p)
    {
        DateTime mint = DateTime.Now, maxt = DateTime.Now;
        bool res = Tool.splitTimeStr(p.m_matchDay, ref mint, ref maxt);
        if (!res)
            return OpRes.op_res_time_format_error;

        IMongoQuery imq = Query.EQ("matchTime", BsonValue.Create(mint));

        return queryMatchDay(user, imq, 100);
    }

    private OpRes queryMatchDay(GMUser user, IMongoQuery cond, int maxCount = 0)
    {
        List<Dictionary<string, object>> dataList = DBMgr.getInstance().executeQuery(TableName.MATCH_GRAND_PRIX_DAY,
            user.getDbServerID(), DbName.DB_GAME, cond, 0, maxCount);
        if (dataList == null)
            return OpRes.opres_success;

        for (int i = 0; i < dataList.Count; i++)
        {
            ResultMatchDayItem info = new ResultMatchDayItem();
            info.m_time = Convert.ToDateTime(dataList[i]["matchTime"]).ToLocalTime().ToShortDateString();
            info.m_playerId = Convert.ToInt32(dataList[i]["playerId"]);
            info.m_nickName = Convert.ToString(dataList[i]["nickName"]);
            info.m_bestScore = Convert.ToInt32(dataList[i]["bestScore"]);
            info.m_rank = Convert.ToInt32(dataList[i]["rank"]);
            m_result2.Add(info);
        }

        return OpRes.opres_success;
    }

    int sortRank(ResultMatchDayItem left, ResultMatchDayItem right)
    {
        return left.m_rank - right.m_rank;
    }
}

//////////////////////////////////////////////////////////////////////////
public class ParamFishBoss
{
    public int m_roomId;
    public string time;
}

public class ResultFishBoss
{
    public string m_date;
    public int m_roomId;
    public int m_bossCount;
    public int m_dragonBall;
    public int m_consumeGold;
    public int m_bossDieCount;
    public int m_lock;
    public int m_rapid;
    public int m_scattering;
    public int m_laser;

    // BOSS攻击次数
    public int m_bossHitCount;
    // BOSS攻击人次
    public int m_bossPersonTime;
    // BOSS释放金币
    public int m_bossReleaseGold;

    public long getBossZheKouTotal()
    {
        return m_bossReleaseGold + m_dragonBall * 5000;
    }

    // 返回每boss盈利
    public string getEarnEachBoss()
    {
        return ItemHelp.getRate(m_consumeGold - getBossZheKouTotal(), m_bossDieCount);
    }
}

// 捕鱼BOSS
public class QueryFishBoss : QueryBase
{
    private List<ResultFishBoss> m_result = new List<ResultFishBoss>();

    public override OpRes doQuery(object param, GMUser user)
    {
        ParamFishBoss p = (ParamFishBoss)param;
        m_result.Clear();

        DateTime mint = DateTime.Now, maxt = DateTime.Now;
        bool res = Tool.splitTimeStr(p.time, ref mint, ref maxt);
        if (!res)
            return OpRes.op_res_time_format_error;

        IMongoQuery imq1 = Query.LT("date", BsonValue.Create(maxt));
        IMongoQuery imq2 = Query.GTE("date", BsonValue.Create(mint));
        IMongoQuery imq3 = Query.EQ("roomid", BsonValue.Create(p.m_roomId));

        var imq = Query.And(imq1, imq2, imq3);

        return query(user, imq);
    }

    public override object getQueryResult() { return m_result; }

    private OpRes query(GMUser user, IMongoQuery imq)
    {
        List<Dictionary<string, object>> dataList = DBMgr.getInstance().executeQuery(TableName.PUMP_BOSSINFO,
            user.getDbServerID(), DbName.DB_PUMP, imq, 0, 0, null, "date", false);
        if (dataList == null)
            return OpRes.opres_success;

        for (int i = 0; i < dataList.Count; i++)
        {
            Dictionary<string, object> data = dataList[i];

            ResultFishBoss info = new ResultFishBoss();
            info.m_date = Convert.ToDateTime(data["date"]).ToLocalTime().ToShortDateString();
            info.m_roomId = Convert.ToInt32(data["roomid"]);
            info.m_bossCount = Convert.ToInt32(data["boss_count"]);
            info.m_dragonBall = Convert.ToInt32(data["dargonball"]);
            info.m_consumeGold = Convert.ToInt32(data["consume_gold"]);
            info.m_bossDieCount = Convert.ToInt32(data["bossDieCount"]);
            info.m_lock = Convert.ToInt32(data["bossItemLock"]);
            info.m_rapid = Convert.ToInt32(data["bossItemRapid"]);
            info.m_scattering = Convert.ToInt32(data["bossItemScattering"]);
            info.m_laser = Convert.ToInt32(data["bossItemLaser"]);

            if (data.ContainsKey("bossHitCount"))
            {
                info.m_bossHitCount = Convert.ToInt32(data["bossHitCount"]);
            }
            if (data.ContainsKey("bossHitPersonTime"))
            {
                info.m_bossPersonTime = Convert.ToInt32(data["bossHitPersonTime"]);
            }
            if (data.ContainsKey("bossReleaseGold"))
            {
                info.m_bossReleaseGold = Convert.ToInt32(data["bossReleaseGold"]);
            }
            m_result.Add(info);
        }

        return OpRes.opres_success;
    }
}








