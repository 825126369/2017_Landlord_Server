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
    public string getCowsTotalRate()
    {
        long totalIncome = m_totalIncome + m_serviceCharge + m_robotIncome;
        long totalOutlay = m_totalOutlay + m_burstZhuang + m_robotOutlay;

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
            GameResultShcd info = new GameResultShcd();
            info.m_insertTime = Convert.ToDateTime(dataList[i]["insert_time"]).ToLocalTime().ToString();
            info.m_result = Convert.ToInt32(dataList[i]["next_card_type"]);
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
        ParamDragonGameModeEarning p = (ParamDragonGameModeEarning)param;
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
            user.getDbServerID(), DbName.DB_GAME);
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
        }

        return OpRes.opres_success;
    }
}

public class ResultIndependentShcd : ResultIndependent
{
    public string getAreaName(int areaId)
    {
        return StrName.s_shcdArea[areaId - 1];
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
                m_result.addBetCount(k, betCount, 0, income, outlay);
            }
        }

        return OpRes.opres_success;
    }
}
