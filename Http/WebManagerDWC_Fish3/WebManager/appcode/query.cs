using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System.Text.RegularExpressions;
using System.Web.Configuration;
using System.Linq;
using System.Web.UI.WebControls;

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
            LOGW.Info("不存在名称为[{0}]的查询", queryType);
            return OpRes.op_res_failed;
        }
        return m_items[queryType].doQuery(param, user);
    }

    // 返回查询结果
    public object getQueryResult(QueryType queryType)
    {
        if (!m_items.ContainsKey(queryType))
        {
            LOGW.Info("不存在名称为[{0}]的查询", queryType);
            return null;
        }
        return m_items[queryType].getQueryResult();
    }

    public object getQueryResult(object param, QueryType queryType, GMUser user)
    {
        if (!m_items.ContainsKey(queryType))
        {
            LOGW.Info("不存在名称为[{0}]的查询", queryType);
            return null;
        }
        return m_items[queryType].getQueryResult(param, user);
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
        m_items.Add(QueryType.queryTypeMoney, new QueryPlayerMoney());
        m_items.Add(QueryType.queryTypeMoneyDetail, new QueryPlayerMoneyDetail());

        m_items.Add(QueryType.queryTypeMail, new QueryMail());
        m_items.Add(QueryType.queryTypeServiceInfo, new QueryServiceInfo());
        m_items.Add(QueryType.queryTypeRecharge, new QueryRecharge());

        m_items.Add(QueryType.queryTypeAccount, new QueryAccount());
        m_items.Add(QueryType.queryTypeLoginHistory, new QueryLogin());
        m_items.Add(QueryType.queryTypeGift, new QueryGift());
        m_items.Add(QueryType.queryTypeGiftCode, new QueryGiftCode());
        m_items.Add(QueryType.queryTypeExchange, new QueryExchange());

        m_items.Add(QueryType.queryTypeLobby, new QueryLobby());
        m_items.Add(QueryType.queryTypeServerEarnings, new QueryServerEarnings());
        m_items.Add(QueryType.queryTypeIndependentFishlord, new QueryIndependentFishlord());
        m_items.Add(QueryType.queryTypeIndependentCrocodile, new QueryIndependentCrocodile());
        m_items.Add(QueryType.queryTypeIndependentDice, new QueryIndependentDice());

        m_items.Add(QueryType.queryTypeCurNotice, new QueryCurNotice());
        m_items.Add(QueryType.queryTypeFishlordParam, new QueryFishlordParam());
        m_items.Add(QueryType.queryTypeFishParkParam, new QueryFishParkParam());

        m_items.Add(QueryType.queryTypeCrocodileParam, new QueryCrocodileParam());
        m_items.Add(QueryType.queryTypeFishStat, new QueryFish());
        m_items.Add(QueryType.queryTypeFishParkStat, new QueryFishParkStat());
        
        m_items.Add(QueryType.queryTypeMoneyAtMost, new QueryMoneyAtMost());

        m_items.Add(QueryType.queryTypeOldEaringsRate, new QueryOldEarningRate());
        m_items.Add(QueryType.queryTypeFishlordStage, new QueryFishlordStage());
        m_items.Add(QueryType.queryTypeFishParkStage, new QueryFishParkStage());

        m_items.Add(QueryType.queryTypeDiceEarnings, new QueryDiceEarningsParam());
        m_items.Add(QueryType.queryTypeOnlinePlayerCount, new QueryOnlinePlayerCount());
        m_items.Add(QueryType.queryTypeOpLog, new QueryOpLog());

        m_items.Add(QueryType.queryTypePlayerHead, new QueryPlayerHead());
        m_items.Add(QueryType.queryTypeTotalConsume, new QueryTotalConsume());
        m_items.Add(QueryType.queryTypeGameRecharge, new QueryGameRechargeByDay());
        m_items.Add(QueryType.queryTypeBaccaratEarnings, new QueryBaccaratEarningsParam());
        m_items.Add(QueryType.queryTypeCoinGrowthRank, new QueryCoinGrowthRank());

        m_items.Add(QueryType.queryTypeFishlordDeskParam, new QueryFishlordDeskParam());
        m_items.Add(QueryType.queryTypeFishParkDeskParam, new QueryFishParkDeskParam());

        m_items.Add(QueryType.queryTypeAccountCoinLessValue, new QueryAccountCoinLessValue());
        m_items.Add(QueryType.queryTypeFishConsume, new QueryFishConsume());

        m_items.Add(QueryType.queryTypeBaccaratPlayerBanker, new QueryBaccaratPlayerBanker());
        m_items.Add(QueryType.queryTypeCowsPlayerBanker, new QueryCowsPlayerBanker());
        m_items.Add(QueryType.queryTypeIndependentCows, new QueryIndependentCows());
        m_items.Add(QueryType.queryTypeQueryCowsParam, new QueryCowsParam());
        m_items.Add(QueryType.queryTypeCowsCardsType, new QueryCowsCardsType());
        m_items.Add(QueryType.queryTypeGameResultControl, new QueryGameResultControl());

        m_items.Add(QueryType.queryTypeDragonParam, new QueryDragonParam());
        m_items.Add(QueryType.queryTypeDragonGameModeEarning, new QueryDragonGameModeEarning());
        m_items.Add(QueryType.queryTypeShcdParam, new QueryShcdParam());
        m_items.Add(QueryType.queryTypeIndependentShcd, new QueryIndependentShcd());
        m_items.Add(QueryType.queryTypeGameCalfRoping, new QueryGameCalfRoping());
        
        m_items.Add(QueryType.queryTypeInformHead, new QueryInformHead());

        m_items.Add(QueryType.queryTypeTdActivation, new QueryTdActivation());
        m_items.Add(QueryType.queryTypeLTV, new QueryTdLTV());

        m_items.Add(QueryType.queryTypeMaxOnline, new QueryMaxOnline());
        m_items.Add(QueryType.queryTypeTotalPlayerMoney, new QueryTotalPlayerMoney());

        m_items.Add(QueryType.queryTypeGrandPrix, new QueryGrandPrix());
        m_items.Add(QueryType.queryTypeFishBoss, new QueryFishBoss());
        m_items.Add(QueryType.queryTypeExchangeStat, new QueryExchangeStat());
        m_items.Add(QueryType.queryTypeRechargePointStat, new QueryRechargePointStat());
        m_items.Add(QueryType.queryTypeStarLottery, new QueryStarLottery());

        m_items.Add(QueryType.queryTypeRLose, new QueryRLose());

        m_items.Add(QueryType.queryTypeDragonBallDaily, new QueryDragonBallDaily());
        m_items.Add(QueryType.queryTypeRechargePlayerMonitor, new QueryRechargePlayerMonitor());
        m_items.Add(QueryType.queryTypeRechargePerHour, new QueryRechargePerHour());

        m_items.Add(QueryType.queryTypeOnlinePlayerNumPerHour, new QueryOnlinePlayerNumPerHour());
        m_items.Add(QueryType.queryTypeGameTimeDistribution, new QueryGameTimeDistribution());
        m_items.Add(QueryType.queryTypeGameTimePlayerFavor, new QueryGameTimePlayerFavor());
        m_items.Add(QueryType.queryTypeFirstRechargeGameTimeDistribution, new QueryFirstRechargeGameTimeDistribution());
        m_items.Add(QueryType.queryTypeFirstRechargePointDistribution, new QueryFirstRechargePointDistribution());

        m_items.Add(QueryType.queryTypePlayerGameBet, new QueryPlayerGameBet());
        m_items.Add(QueryType.queryTypePlayerIncomeExpenses, new QueryPlayerIncomeExpenses());

        m_items.Add(QueryType.queryTypeNewPlayer, new QueryNewPlayer());
    }
}

///////////////////////////////////////////////////////////////////////////////

public class QueryBase
{
    // 作查询
    public virtual OpRes doQuery(object param, GMUser user) { return OpRes.op_res_failed; }

    // 返回查询结果
    public virtual object getQueryResult() { return null; }

    public virtual object getQueryResult(object param, GMUser user) { return null; }

    public virtual OpRes makeQuery(object param, GMUser user, QueryCondition imq) { return OpRes.op_res_failed; }

    // 通过玩家ID，返回域
    public static Dictionary<string, object> getPlayerProperty(int playerId, GMUser user, string[] fields)
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
    }

    // 通过昵称返回玩家属性
    public static Dictionary<string, object> getPlayerPropertyByNickName(string nickName, GMUser user, string[] fields)
    {
        Dictionary<string, object> ret =
                        DBMgr.getInstance().getTableData(TableName.PLAYER_INFO, "nickname", nickName, fields, user.getDbServerID(), DbName.DB_PLAYER);
        return ret;
    }
}

///////////////////////////////////////////////////////////////////////////////

public enum QueryWay
{
    by_way0,        
    by_way1,   
    by_way2,   
    by_way3,   
    by_way4,   
    by_way5, 
}

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
    public QueryWay m_way;

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

        if (data == null || data.Count <= 1)
        {
            return OpRes.op_res_not_found_data;
        }

        int i = 0;
        for (i = 0; i < data.Count; i++)
        {
            GMAccountItem tmp = new GMAccountItem();
            tmp.m_user = Convert.ToString(data[i]["user"]);
            tmp.m_type = Convert.ToString(data[i]["type"]);
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

// 玩家属性参数
public class PropParam
{
    public int m_playerId = 0;
    public string m_accountName = "";
    public string m_nickName = "";
}

public class MoneyItem
{
    // 生成时间
    public string m_genTime = "";
    // 动作类型
    public int m_actionType;
    // 属性类型
    public int m_propertyType;
    // 初始值
    public int m_startValue;
    // 结束值
    public int m_endValue;
    // 差额
    public int m_deltaValue;
    // 玩家ID
    public int m_playerId;
    // 昵称
    public string m_nickName = "";
    // 游戏id
    public int m_gameId;
    // 额外参数
    public string m_param = "";

    public string getPropertyName()
    {
        if (m_propertyType == (int)PropertyType.property_type_gold)
            return "金币";

        if (m_propertyType == (int)PropertyType.property_type_ticket)
            return "钻石";

        if (m_propertyType == (int)PropertyType.property_type_chip)
            return "话费碎片";

        if (m_propertyType == (int)PropertyType.property_type_dragon_ball)
            return "龙珠";

        return "";
    }

    // 返回动作名称
    public string getActionName()
    {
        XmlConfig xml = ResMgr.getInstance().getRes("money_reason.xml");
        if (xml != null)
        {
            return xml.getString(m_actionType.ToString(), "");
        }
        return "";
    }

    public string getGameName()
    {
        return StrName.s_gameName[m_gameId];
    }
}

// 玩家金币的查询参数
public class ParamMoneyQuery : ParamQuery
{
    // 过滤(动作类型)
    public int m_filter;

    // 属性
    public int m_property;

    // 值的范围
    public string m_range = "";

    public int m_gameId;
}

// 玩家金币查询
public class QueryPlayerMoney : QueryBase
{
    private List<MoneyItem> m_result = new List<MoneyItem>();
    protected static string[] m_field = { "nickname" };
    static string[] m_field1 = { "player_id" };
    private QueryCondition m_cond = new QueryCondition();

    // 作查询
    public override OpRes doQuery(object param, GMUser user)
    {
        m_result.Clear();
        m_cond.startQuery();
       // m_playerAcc = "";
        OpRes res = makeQuery(param, user, m_cond);
        if (res != OpRes.opres_success)
            return res;

       // if (string.IsNullOrEmpty(m_playerAcc))
          //  return OpRes.op_res_not_found_data;

        IMongoQuery imq = m_cond.getImq();
        ParamMoneyQuery p = (ParamMoneyQuery)param;
        return query(p, imq, user);
    }

    // 返回查询结果
    public override object getQueryResult()
    {
        return m_result;
    }

    public override OpRes makeQuery(object p, GMUser user, QueryCondition queryCond)
    {
        ParamMoneyQuery param = (ParamMoneyQuery)p;
        int playerId = -1;

        switch (param.m_way)
        {
            case QueryWay.by_way0:  // 通过ID查询
                {
                    if (param.m_param != "")
                    {
                        if (!int.TryParse(param.m_param, out playerId))
                        {
                            return OpRes.op_res_param_not_valid;
                        }
                    }
                }
                break;
            case QueryWay.by_way1: // 通过账号查询
                {
                    OpRes res = queryByAccount(param, user, ref playerId);
                    if (res != OpRes.opres_success)
                    {
                        return res;
                    }
                }
                break;
            case QueryWay.by_way2: // 通过昵称查询
                {
                    OpRes res = queryByNickName(param, user, ref playerId);
                    if (res != OpRes.opres_success)
                    {
                        return res;
                    }
                }
                break;
        }

        int condCount = 0;

        if (param.m_time != "")
        {
            DateTime mint = DateTime.Now, maxt = DateTime.Now;
            bool res = Tool.splitTimeStr(param.m_time, ref mint, ref maxt);
            if (!res)
                return OpRes.op_res_time_format_error;

            condCount++;
            if (queryCond.isExport())
            {
                queryCond.addCond("time", param.m_time);
            }
            else
            {
                IMongoQuery imq1 = Query.LT("genTime", BsonValue.Create(maxt));
                IMongoQuery imq2 = Query.GTE("genTime", BsonValue.Create(mint));
                queryCond.addImq(Query.And(imq1, imq2));
            }
        }

        if (playerId != -1)
        {
            condCount++;
            queryCond.addQueryCond("playerId", playerId);
        }

        // 过滤
        if (param.m_filter != 0)
        {
            queryCond.addQueryCond("reason", param.m_filter);
        }
        // 属性
        if (param.m_property != (int)PropertyType.property_type_full)
        {
            queryCond.addQueryCond("itemId", param.m_property);
        }

        // 范围
        if (param.m_range != "")
        {
            if (!Tool.isTwoNumValid(param.m_range))
                return OpRes.op_res_param_not_valid;

            if (queryCond.isExport())
            {
                queryCond.addCond("range", param.m_range);
            }
            else
            {
                List<int> range = new List<int>();
                Tool.parseNumList(param.m_range, range);
                IMongoQuery imq1 = Query.LTE("addValue", BsonValue.Create(range[1]));
                IMongoQuery imq2 = Query.GTE("addValue", BsonValue.Create(range[0]));
                queryCond.addImq(Query.And(imq1, imq2));
            }
        }
        if (param.m_gameId != (int)GameId.gameMax)
        {
            queryCond.addQueryCond("gameId", param.m_gameId);
        }
        if (condCount == 0)
            return OpRes.op_res_need_at_least_one_cond;

        return OpRes.opres_success;
    }

    private OpRes queryByAccount(ParamMoneyQuery param, GMUser user, ref int id)
    {
        if (param.m_param != "")
        {
            Dictionary<string, object> data = getPlayerPropertyByAcc(param.m_param, user, m_field1);
            if (data == null)
            {
                return OpRes.op_res_not_found_data;
            }

            if (data.ContainsKey("player_id"))
            {
                id = Convert.ToInt32(data["player_id"]);
            }
        }

        return OpRes.opres_success;
    }

    private OpRes queryByNickName(ParamMoneyQuery param, GMUser user, ref int id)
    {
        if (param.m_param != "")
        {
            Dictionary<string, object> data = getPlayerPropertyByNickName(param.m_param, user, m_field1);
            if (data == null)
            {
                return OpRes.op_res_not_found_data;
            }

            if (data.ContainsKey("player_id"))
            {
                id = Convert.ToInt32(data["player_id"]);
            }
        }

        return OpRes.opres_success;
    }

    // 通过玩家ID查询
    protected virtual OpRes query(ParamMoneyQuery param, IMongoQuery imq, GMUser user)
    {
        // 查看满足条件的记当个数
        user.totalRecord = DBMgr.getInstance().getRecordCount(TableName.PUMP_PLAYER_MONEY, imq, user.getDbServerID(), DbName.DB_PUMP);

        List<Dictionary<string, object>> data =
             DBMgr.getInstance().executeQuery(TableName.PUMP_PLAYER_MONEY, user.getDbServerID(), DbName.DB_PUMP, imq,
                                              (param.m_curPage - 1) * param.m_countEachPage, param.m_countEachPage,
                                              null, "genTime", false);

        if (data == null || data.Count == 0)
        {
            return OpRes.op_res_not_found_data;
        }

        int i = 0;
        for (i = 0; i < data.Count; i++)
        {
            MoneyItem tmp = new MoneyItem();
            tmp.m_genTime = Convert.ToDateTime(data[i]["genTime"]).ToLocalTime().ToString();
            tmp.m_actionType = Convert.ToInt32(data[i]["reason"]);
            tmp.m_startValue = Convert.ToInt32(data[i]["oldValue"]);
            tmp.m_endValue = Convert.ToInt32(data[i]["newValue"]);
            tmp.m_deltaValue = Convert.ToInt32(data[i]["addValue"]);

            tmp.m_playerId = Convert.ToInt32(data[i]["playerId"]);
            Dictionary<string, object> ret = getPlayerProperty(tmp.m_playerId, user, m_field);
            if (ret != null && ret.ContainsKey("nickname"))
            {
                tmp.m_nickName = Convert.ToString(ret["nickname"]);
            }
            tmp.m_propertyType = Convert.ToInt32(data[i]["itemId"]);
            if (data[i].ContainsKey("param"))
            {
                tmp.m_param = Convert.ToString(data[i]["param"]);
            }
            tmp.m_gameId = Convert.ToInt32(data[i]["gameId"]);

            m_result.Add(tmp);
        }
        return OpRes.opres_success;
    }
}

//////////////////////////////////////////////////////////////////////////
public class MoneyItemDetail : MoneyItem
{
    // 投注
    public long m_outlay;
    // 返还
    public long m_income;

    public long m_playerWinBet;

    public string m_exInfo = "";

    public string getExParam(int index)
    {
        if (string.IsNullOrEmpty(m_exInfo))
            return "";

        if (m_gameId > 0)
        {
            URLParam uParam = new URLParam();
            uParam.m_text = "详情";
            uParam.m_key = "gameId";
            uParam.m_value = m_gameId.ToString();
            uParam.m_url = DefCC.ASPX_GAME_DETAIL;
            uParam.m_target = "_blank";
            uParam.addExParam("index", index);
            return Tool.genHyperlink(uParam);
        }
        return "";
    }
}
// 玩家金币查询
public class QueryPlayerMoneyDetail : QueryPlayerMoney
{
    private List<MoneyItemDetail> m_result1 = new List<MoneyItemDetail>();

    // 返回查询结果
    public override object getQueryResult()
    {
        return m_result1;
    }

    // 通过玩家ID查询
    protected override OpRes query(ParamMoneyQuery param, IMongoQuery imq, GMUser user)
    {
        m_result1.Clear();

        // 查看满足条件的记当个数
        user.totalRecord = DBMgr.getInstance().getRecordCount(TableName.PUMP_PLAYER_MONEY, imq, user.getDbServerID(), DbName.DB_PUMP);

        List<Dictionary<string, object>> data =
             DBMgr.getInstance().executeQuery(TableName.PUMP_PLAYER_MONEY, user.getDbServerID(), DbName.DB_PUMP, imq,
                                              (param.m_curPage - 1) * param.m_countEachPage, param.m_countEachPage,
                                              null, "genTime", false);

        if (data == null || data.Count == 0)
        {
            return OpRes.op_res_not_found_data;
        }

//         string nickName = "";
//         Dictionary<string, object> ret = getPlayerPropertyByAcc(m_playerAcc, user, m_field);
//         if (ret != null && ret.ContainsKey("nickname"))
//         {
//             nickName = Convert.ToString(ret["nickname"]);
//         }

        int i = 0;
        for (i = 0; i < data.Count; i++)
        {
            MoneyItemDetail tmp = new MoneyItemDetail();
            tmp.m_genTime = Convert.ToDateTime(data[i]["genTime"]).ToLocalTime().ToString();
            tmp.m_actionType = Convert.ToInt32(data[i]["reason"]);
            tmp.m_startValue = Convert.ToInt32(data[i]["oldValue"]);
            tmp.m_endValue = Convert.ToInt32(data[i]["newValue"]);
            tmp.m_deltaValue = tmp.m_endValue - tmp.m_startValue; //Convert.ToInt64(data[i]["addValue"]);

            tmp.m_playerId = Convert.ToInt32(data[i]["playerId"]);
             Dictionary<string, object> retData = getPlayerProperty(tmp.m_playerId, user, m_field);
             if (retData != null && retData.ContainsKey("nickname"))
            {
                tmp.m_nickName = Convert.ToString(retData["nickname"]);
            }
            tmp.m_propertyType = Convert.ToInt32(data[i]["itemId"]);
            if (data[i].ContainsKey("exInfo"))
            {
                tmp.m_exInfo = Convert.ToString(data[i]["exInfo"]);
            }
            if (data[i].ContainsKey("param"))
            {
                tmp.m_param = Convert.ToString(data[i]["param"]);
            }
            if (data[i].ContainsKey("playerWinBet"))
            {
                tmp.m_playerWinBet = Convert.ToInt64(data[i]["playerWinBet"]);
            }

            tmp.m_gameId = Convert.ToInt32(data[i]["gameId"]);
            //tmp.m_acc = m_playerAcc;

            if (data[i].ContainsKey("playerOutlay"))
            {
                tmp.m_outlay = Convert.ToInt32(data[i]["playerOutlay"]);
            }
            if (data[i].ContainsKey("playerIncome"))
            {
                tmp.m_income = Convert.ToInt32(data[i]["playerIncome"]);
            }
            
            m_result1.Add(tmp);
        }
        return OpRes.opres_success;
    }
}

//////////////////////////////////////////////////////////////////////////

public class LotteryItem
{
    // 生成时间
    public string m_genTime = "";
    public int m_playerId;
    public int m_boxId;
    public int m_cost;
    public List<ParamItem> m_rewardList = new List<ParamItem>();

    public string getRewardList()
    {
        return ItemHelp.getRewardList(m_rewardList);
    }
}

public class ParamLottery : ParamQuery
{
    public string m_playerId;
    public string m_boxId;
}

//////////////////////////////////////////////////////////////////////////

public class MailItem
{
    public string m_genTime = "";
    public int m_playerId;
    public List<ParamItem> m_rewardList = new List<ParamItem>();

    public string getRewardList()
    {
        return ItemHelp.getRewardList(m_rewardList);
    }
}

// 邮件
public class QueryMail : QueryBase
{
    private List<MailItem> m_result = new List<MailItem>();

    public override OpRes doQuery(object param, GMUser user)
    {
        m_result.Clear();
        ParamLottery p = (ParamLottery)param;

        List<IMongoQuery> queryList = new List<IMongoQuery>();
        int condCount = 0;

        if (p.m_time != "")
        {
            DateTime mint = DateTime.Now, maxt = DateTime.Now;
            bool res = Tool.splitTimeStr(p.m_time, ref mint, ref maxt);
            if (!res)
                return OpRes.op_res_time_format_error;

            condCount++;
            IMongoQuery imq1 = Query.LT("genTime", BsonValue.Create(maxt));
            IMongoQuery imq2 = Query.GTE("genTime", BsonValue.Create(mint));
            queryList.Add(Query.And(imq1, imq2));
        }
        if (!string.IsNullOrEmpty(p.m_playerId))
        {
            try
            {
                int id = Convert.ToInt32(p.m_playerId);
                queryList.Add(Query.EQ("playerId", BsonValue.Create(id)));
                condCount++;
            }
            catch (System.Exception ex)
            {
                return OpRes.op_res_param_not_valid;
            }
        }
        
        if (condCount == 0)
            return OpRes.op_res_need_at_least_one_cond;

        IMongoQuery imq = queryList.Count > 0 ? Query.And(queryList) : null;

        return query(p, imq, user);
    }

    // 返回查询结果
    public override object getQueryResult()
    {
        return m_result;
    }

    private OpRes query(ParamLottery param, IMongoQuery imq, GMUser user)
    {
        user.totalRecord = DBMgr.getInstance().getRecordCount(TableName.PUMP_MAIL, imq, user.getDbServerID(), DbName.DB_PUMP);

        List<Dictionary<string, object>> data =
             DBMgr.getInstance().executeQuery(TableName.PUMP_MAIL, user.getDbServerID(), DbName.DB_PUMP, imq,
                                              (param.m_curPage - 1) * param.m_countEachPage, param.m_countEachPage);

        if (data == null || data.Count == 0)
        {
            return OpRes.op_res_not_found_data;
        }

        int i = 0;
        for (i = 0; i < data.Count; i++)
        {
            MailItem tmp = new MailItem();
            tmp.m_genTime = Convert.ToDateTime(data[i]["genTime"]).ToLocalTime().ToString();
            tmp.m_playerId = Convert.ToInt32(data[i]["playerId"]);
            Tool.parseItemFromDic(data[i]["items"] as Dictionary<string, object>, tmp.m_rewardList);
            m_result.Add(tmp);
        }
        return OpRes.opres_success;
    }
}

//////////////////////////////////////////////////////////////////////////

public class ServiceInfoItem
{
    public string m_platChaName = "";
    public string m_platEngName = "";
    public string m_info = "";
}

// 客服信息
public class QueryServiceInfo : QueryBase
{
    private List<ServiceInfoItem> m_result = new List<ServiceInfoItem>();

    public override OpRes doQuery(object param, GMUser user)
    {
        int accServerId = user.getDbServerID();
        if (accServerId == -1)
            return OpRes.op_res_failed;

        m_result.Clear();
        List<Dictionary<string, object>> data = 
            DBMgr.getInstance().executeQuery(TableName.SERVICE_INFO, accServerId, DbName.DB_PLAYER);

        if (data != null)
        {
            for (int i = 0; i < data.Count; i++)
            {
                ServiceInfoItem info = new ServiceInfoItem();
                m_result.Add(info);

                info.m_info = Convert.ToString(data[i]["info"]);
                info.m_platEngName = Convert.ToString(data[i]["plat"]);

                PlatformInfo pi = ResMgr.getInstance().getPlatformInfoByName(info.m_platEngName);
                if (pi != null)
                {
                    info.m_platChaName = pi.m_chaName;
                }
            }
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

// 查询结果
public class ResultQueryAccount
{
    // 账号
    public string m_account = "";
    // ID
    public int m_id;
    // 昵称
    public string m_nickName = "";
    // 平台
    public string m_platForm = "";
    // 角色创建时间
    public string m_createTime = "";
    // VIP等级
    public int m_vipLevel;
    // VIP经验
    public int m_vipExp;
    // 上次登陆时间
    public string m_lastLoginTime = "";
    // 上次登陆IP
    public string m_lastLoginIP = "";
    // 金币
    public int m_gold;
    // 保险箱中的金币
    public int m_safeBoxGold;
    // 礼券
    public int m_ticket;
    // 绑定手机
    public string m_bindMobile = "";
    // 账号状态
    public string m_accountState = "";
}

// 账号查询
public class QueryAccount : QueryBase
{
    private List<ResultQueryAccount> m_result = new List<ResultQueryAccount>();

    private string[] m_playerFields = { "account", "player_id", "nickname", "platform", "create_time", "VipLevel",
                                        "VipExp", "logout_time", "gold", "safeBoxGold", "ticket", "delete", "bindPhone" };

    private string[] m_phoneFields = { "phone", "block" };

    public override OpRes doQuery(object param, GMUser user)
    {
        m_result.Clear();
        ParamQuery p = (ParamQuery)param;

        switch (p.m_way)
        {
            case QueryWay.by_way0:  // 以玩家id查询
                {
                    int id = 0;
                    if (!int.TryParse(p.m_param, out id))
                    {
                        return OpRes.op_res_param_not_valid;
                    }

                    return queryById(id, user);
                }
                break;
            case QueryWay.by_way1:  // 以玩家账号查询
                {
                    return queryByAccount(p.m_param, user);
                }
                break;
            case QueryWay.by_way2:  // 以玩家昵称查询
                {
                    return queryByNickname(p.m_param, user);
                }
                break;
            case QueryWay.by_way3:  // 以登陆IP查询
                {
                    return queryByIP(p.m_param, user);
                }
                break;
        }

        return OpRes.op_res_failed;
    }

    public override object getQueryResult()
    {
        return m_result;
    }

    // 通过玩家ID查询
    private OpRes queryById(int id, GMUser user)
    {
        Dictionary<string, object> ret = DBMgr.getInstance().getTableData(TableName.PLAYER_INFO, "player_id", id, m_playerFields, user.getDbServerID(), DbName.DB_PLAYER);
        return query(ret, user);
    }

    private OpRes queryByAccount(string queryStr, GMUser user)
    {
        Dictionary<string, object> ret = DBMgr.getInstance().getTableData(TableName.PLAYER_INFO, "account", queryStr, m_playerFields, user.getDbServerID(), DbName.DB_PLAYER);
        return query(ret, user);
    }

    private OpRes queryByNickname(string queryStr, GMUser user)
    {
        Dictionary<string, object> ret = DBMgr.getInstance().getTableData(TableName.PLAYER_INFO, "nickname", queryStr, m_playerFields, user.getDbServerID(), DbName.DB_PLAYER);
        return query(ret, user);
    }

    private OpRes queryByIP(string queryStr, GMUser user)
    {
        return OpRes.op_res_failed;
       /* int accServerId = DBMgr.getInstance().getSpecialServerId(DbName.DB_ACCOUNT);
        if (accServerId == -1)
        {
            return OpRes.op_res_failed;
        }

        string acc = "";
        IMongoQuery imq = Query.EQ("ip", queryStr);
        List<Dictionary<string, object>> loginData =
            DBMgr.getInstance().executeQuery(TableName.PLAYER_LOGIN, accServerId, DbName.DB_ACCOUNT, imq, 0, 1, null, "time", false);
        if (loginData != null && loginData.Count > 0)
        {
            acc = Convert.ToString(loginData[0]["acc"]);
        }

        if (acc == "")
            return OpRes.op_res_not_found_data;

        return queryByAccount(acc, user);*/
    }

    private OpRes query(Dictionary<string, object> ret, GMUser user)
    {
        if (ret == null)
        {
            return OpRes.op_res_not_found_data;
        }
        if (!ret.ContainsKey("account"))
        {
            return OpRes.op_res_not_found_data;
        }
        // 账号
        string acc = Convert.ToString(ret["account"]);
        int accServerId = DBMgr.getInstance().getSpecialServerId(DbName.DB_ACCOUNT);
        Dictionary<string, object> data = null;
        if (accServerId != -1)
        {
            data = DBMgr.getInstance().getTableData(TableName.PLAYER_ACCOUNT, "acc", acc, m_phoneFields, accServerId, DbName.DB_ACCOUNT);
        }

        user.totalRecord = 1;
        ResultQueryAccount tmp = new ResultQueryAccount();
        tmp.m_account = acc;
        tmp.m_id = Convert.ToInt32(ret["player_id"]);
        if (ret.ContainsKey("nickname"))
        {
            tmp.m_nickName = Convert.ToString(ret["nickname"]);
        }
        
        tmp.m_platForm = Convert.ToString(ret["platform"]);
        tmp.m_createTime = Convert.ToDateTime(ret["create_time"]).ToLocalTime().ToString();
        tmp.m_vipLevel = Convert.ToInt32(ret["VipLevel"]);
        tmp.m_vipExp = Convert.ToInt32(ret["VipExp"]);
        tmp.m_lastLoginTime = Convert.ToDateTime(ret["logout_time"]).ToLocalTime().ToString();
        tmp.m_gold = Convert.ToInt32(ret["gold"]);

        if (ret.ContainsKey("safeBoxGold"))
        {
            tmp.m_safeBoxGold = Convert.ToInt32(ret["safeBoxGold"]);
        }
        
        tmp.m_ticket = Convert.ToInt32(ret["ticket"]);
        
        bool isBlock = false;
        if (ret.ContainsKey("delete"))
        {
            isBlock = Convert.ToBoolean(ret["delete"]);
        }
        tmp.m_accountState = isBlock ? "停封" : "正常";

        if (ret.ContainsKey("bindPhone"))
        {
            tmp.m_bindMobile = Convert.ToString(ret["bindPhone"]);
        }

//         IMongoQuery imq = Query.EQ("acc", acc);
//         List<Dictionary<string, object>> loginData =
//       DBMgr.getInstance().executeQuery(TableName.PLAYER_LOGIN, accServerId, DbName.DB_ACCOUNT, imq, 0, 1, null, "time", false);
//         if (loginData != null && loginData.Count > 0)
//         {
//             tmp.m_lastLoginIP = Convert.ToString(loginData[0]["ip"]);
//         }
        m_result.Add(tmp);
        return OpRes.opres_success;
    }
}

//////////////////////////////////////////////////////////////////////////

public class LoginItem
{
    public string m_account = "";
    public string m_time = "";
    public string m_ip = "";
}

// 登陆历史
public class QueryLogin : QueryBase
{
    private List<LoginItem> m_result = new List<LoginItem>();

    public override OpRes doQuery(object param, GMUser user)
    {
        m_result.Clear();
        ParamQuery p = (ParamQuery)param;

        List<IMongoQuery> queryList = new List<IMongoQuery>();
        int condCount = 0;
        if (p.m_param != "")
        {
            queryList.Add(Query.EQ("acc", BsonValue.Create(p.m_param)));
        }
        if (p.m_time != "")
        {
            DateTime mint = DateTime.Now, maxt = DateTime.Now;
            bool res = Tool.splitTimeStr(p.m_time, ref mint, ref maxt);
            if (!res)
                return OpRes.op_res_time_format_error;

            condCount++;
            IMongoQuery imq1 = Query.LT("time", BsonValue.Create(maxt));
            IMongoQuery imq2 = Query.GTE("time", BsonValue.Create(mint));
            queryList.Add(Query.And(imq1, imq2));
        }
        
        if (condCount == 0)
            return OpRes.op_res_need_at_least_one_cond;

        IMongoQuery imq = queryList.Count > 0 ? Query.And(queryList) : null;

        return query(p, imq, user);
    }

    // 返回查询结果
    public override object getQueryResult()
    {
        return m_result;
    }

    private OpRes query(ParamQuery param, IMongoQuery imq, GMUser user)
    {
        int serverId = DBMgr.getInstance().getSpecialServerId(DbName.DB_ACCOUNT);
        if (serverId == -1)
            return OpRes.op_res_failed;

        user.totalRecord = DBMgr.getInstance().getRecordCount(TableName.PLAYER_LOGIN, imq, serverId, DbName.DB_ACCOUNT);

        List<Dictionary<string, object>> data =
             DBMgr.getInstance().executeQuery(TableName.PLAYER_LOGIN, serverId, DbName.DB_ACCOUNT, imq,
                                              (param.m_curPage - 1) * param.m_countEachPage, param.m_countEachPage);

        if (data == null || data.Count == 0)
        {
            return OpRes.op_res_not_found_data;
        }

        int i = 0;
        for (i = 0; i < data.Count; i++)
        {
            LoginItem tmp = new LoginItem();
            tmp.m_time = Convert.ToDateTime(data[i]["time"]).ToLocalTime().ToString();
            tmp.m_account = Convert.ToString(data[i]["acc"]);
            tmp.m_ip = Convert.ToString(data[i]["ip"]);
            m_result.Add(tmp);
        }
        return OpRes.opres_success;
    }
}

//////////////////////////////////////////////////////////////////////////

public class GiftItem
{
    public string m_giftId = "";
    public DateTime m_deadTime;
    public List<ParamItem> m_giftList = new List<ParamItem>();

    public string getGiftList()
    {
        return ItemHelp.getRewardList(m_giftList);
    }

    // 返回源串形式,以分号相隔
    public string getSrcGiftList()
    {
        string str = "";
        if (m_giftList.Count > 0)
        {
            str += m_giftList[0].m_itemId + " " + m_giftList[0].m_itemCount;
        }

        for (int i = 1; i < m_giftList.Count; i++)
        {
            str += ";" + m_giftList[i].m_itemId + " " + m_giftList[i].m_itemCount;
        }
        return str;
    }
}

// 查询礼包
public class QueryGift : QueryBase
{
    private List<GiftItem> m_result = new List<GiftItem>();

    public override OpRes doQuery(object param, GMUser user)
    {
        m_result.Clear();
        ParamQuery p = (ParamQuery)param;
        IMongoQuery imq = null;
        switch (p.m_way)
        {
            case QueryWay.by_way0:  // 已过期
                {
                    imq = Query.LTE("deadTime", BsonValue.Create(DateTime.Now));
                }
                break;
            case QueryWay.by_way1:  // 未过期
                {
                    imq = Query.GT("deadTime", BsonValue.Create(DateTime.Now));
                }
                break;
        }
        return query(p, imq, user);
    }

    // 返回查询结果
    public override object getQueryResult()
    {
        return m_result;
    }

    private OpRes query(ParamQuery param, IMongoQuery imq, GMUser user)
    {
        int serverId = DBMgr.getInstance().getSpecialServerId(DbName.DB_ACCOUNT);
        if (serverId == -1)
            return OpRes.op_res_failed;

        user.totalRecord = DBMgr.getInstance().getRecordCount(TableName.GIFT, imq, serverId, DbName.DB_ACCOUNT);

        List<Dictionary<string, object>> data =
             DBMgr.getInstance().executeQuery(TableName.GIFT, serverId, DbName.DB_ACCOUNT, imq,
                                              (param.m_curPage - 1) * param.m_countEachPage, param.m_countEachPage);

        if (data == null || data.Count == 0)
        {
            return OpRes.op_res_not_found_data;
        }

        int i = 0;
        for (i = 0; i < data.Count; i++)
        {
            GiftItem tmp = new GiftItem();
            tmp.m_giftId = Convert.ToString(data[i]["giftId"]);
            Tool.parseItemFromDic(data[i]["item"] as Dictionary<string, object>, tmp.m_giftList);
            tmp.m_deadTime = Convert.ToDateTime(data[i]["deadTime"]).ToLocalTime(); //.ToString();
            m_result.Add(tmp);
        }
        return OpRes.opres_success;
    }
}

//////////////////////////////////////////////////////////////////////////

public class GiftCodeItem
{
    public string m_genTime = "";
    public string m_giftCode = "";
    public string m_giftId = "";
    public string m_plat = "";

    public int m_playerServerId;
    public string playerPlat = "";
    public int m_playerId;
    public string m_playerAcc = "";
    public string m_useTime = "";
}

// 查询礼包码
public class QueryGiftCode : QueryBase
{
    private List<GiftCodeItem> m_result = new List<GiftCodeItem>();
    private QueryCondition m_cond = new QueryCondition();

    public override OpRes doQuery(object param, GMUser user)
    {
        m_result.Clear();
        ParamQuery p = (ParamQuery)param;
        m_cond.startQuery();
        OpRes res = makeQuery(param, user, m_cond);
        if (res != OpRes.opres_success)
            return res;

        IMongoQuery imq = m_cond.getImq();
        return query(p, imq, user);
    }

    // 返回查询结果
    public override object getQueryResult()
    {
        return m_result;
    }

    private OpRes query(ParamQuery param, IMongoQuery imq, GMUser user)
    {
        int serverId = DBMgr.getInstance().getSpecialServerId(DbName.DB_ACCOUNT);
        if (serverId == -1)
            return OpRes.op_res_failed;

        user.totalRecord = DBMgr.getInstance().getRecordCount(TableName.GIFT_CODE, imq, serverId, DbName.DB_ACCOUNT);

        List<Dictionary<string, object>> data =
             DBMgr.getInstance().executeQuery(TableName.GIFT_CODE, serverId, DbName.DB_ACCOUNT, imq,
                                              (param.m_curPage - 1) * param.m_countEachPage, param.m_countEachPage);

        if (data == null || data.Count == 0)
        {
            return OpRes.op_res_not_found_data;
        }

        int i = 0;
        for (i = 0; i < data.Count; i++)
        {
            GiftCodeItem tmp = new GiftCodeItem();
            tmp.m_genTime = Convert.ToDateTime(data[i]["genTime"]).ToLocalTime().ToString();
            tmp.m_giftCode = Convert.ToString(data[i]["giftCode"]);
            tmp.m_giftId = Convert.ToString(data[i]["giftId"]);
            tmp.m_plat = Convert.ToString(data[i]["plat"]);

            tmp.m_playerServerId = Convert.ToInt32(data[i]["playerServerId"]);
            tmp.playerPlat = Convert.ToString(data[i]["playerPlat"]);
            tmp.m_playerId = Convert.ToInt32(data[i]["playerId"]);
            tmp.m_playerAcc = Convert.ToString(data[i]["playerAcc"]);
            tmp.m_useTime = Convert.ToDateTime(data[i]["useTime"]).ToLocalTime().ToString();
            m_result.Add(tmp);
        }
        return OpRes.opres_success;
    }

    public override OpRes makeQuery(object param, GMUser user, QueryCondition queryCond)
    {
        ParamQuery p = (ParamQuery)param;

        if (!string.IsNullOrEmpty(p.m_param))
        {
            queryCond.addQueryCond("giftCode", p.m_param);
        }
        if (p.m_time != "")
        {
            DateTime mint = DateTime.Now, maxt = DateTime.Now;
            bool res = Tool.splitTimeStr(p.m_time, ref mint, ref maxt);
            if (!res)
                return OpRes.op_res_time_format_error;

            if (queryCond.isExport())
            {
                queryCond.addCond("time", p.m_time);
            }
            else
            {
                IMongoQuery imq1 = Query.LT("genTime", BsonValue.Create(maxt));
                IMongoQuery imq2 = Query.GTE("genTime", BsonValue.Create(mint));
                queryCond.addImq(Query.And(imq1, imq2));
            }
        }

        return OpRes.opres_success;
    }
}


//////////////////////////////////////////////////////////////////////////

public class ParamQueryGift : ParamQuery
{
    public int m_state;
}

public enum ExState
{
    success,
    wait,
}

public class ExchangeItem
{
    // 兑换ID
    public string m_exchangeId = "";

    public int m_playerId;

    // 手机号
    public string m_phone = "";
    
    // 道具名称
    public string m_itemName="";
    
    // 兑换时间
    public string m_exchangeTime = "";

    // 发放时间
    public string m_giveOutTime = "";

    public bool m_isReceive;

    public string getStateName()
    {
        if (m_isReceive)
            return "已发放";

        return "未发放";
    }
}

// 兑换查询
public class QueryExchange : QueryBase
{
    private List<ExchangeItem> m_result = new List<ExchangeItem>();

    public override OpRes doQuery(object param, GMUser user)
    {
        m_result.Clear();
        ParamQueryGift p = (ParamQueryGift)param;

        List<IMongoQuery> queryList = new List<IMongoQuery>();
        
        if (p.m_param != "")
        {
            int playerId = 0;
            if (!int.TryParse(p.m_param, out playerId))
            {
                return OpRes.op_res_param_not_valid;
            }

            queryList.Add(Query.EQ("playerId", BsonValue.Create(playerId)));
        }

        bool isReceive = (p.m_state == 0);
        queryList.Add(Query.EQ("isReceive", BsonValue.Create(isReceive)));

        IMongoQuery imq = queryList.Count > 0 ? Query.And(queryList) : null;

        return query(p, imq, user);
    }

    // 返回查询结果
    public override object getQueryResult()
    {
        return m_result;
    }

    private OpRes query(ParamQuery param, IMongoQuery imq, GMUser user)
    {
        user.totalRecord = DBMgr.getInstance().getRecordCount(TableName.EXCHANGE, imq, user.getDbServerID(), DbName.DB_PLAYER);

        List<Dictionary<string, object>> data =
             DBMgr.getInstance().executeQuery(TableName.EXCHANGE, user.getDbServerID(), DbName.DB_PLAYER, imq,
                                              (param.m_curPage - 1) * param.m_countEachPage, param.m_countEachPage);

        if (data == null || data.Count == 0)
        {
            return OpRes.op_res_not_found_data;
        }

        int i = 0;
        for (i = 0; i < data.Count; i++)
        {
            ExchangeItem tmp = new ExchangeItem();
            m_result.Add(tmp);

            tmp.m_exchangeId = Convert.ToString(data[i]["exchangeId"]);
            tmp.m_playerId = Convert.ToInt32(data[i]["playerId"]);
            tmp.m_phone = Convert.ToString(data[i]["phone"]);
            tmp.m_exchangeTime = Convert.ToDateTime(data[i]["genTime"]).ToLocalTime().ToString();
            tmp.m_itemName = Convert.ToString(data[i]["itemName"]);
            tmp.m_isReceive = Convert.ToBoolean(data[i]["isReceive"]);

            if (data[i].ContainsKey("giveOutTime"))
            {
                tmp.m_giveOutTime = Convert.ToDateTime(data[i]["giveOutTime"]).ToLocalTime().ToString();
            }
        }
        return OpRes.opres_success;
    }
}

//////////////////////////////////////////////////////////////////////////

public class ResultLobby
{
    public int m_statType;

    public int[] m_generalStat = new int[(int)DataStatType.stat_max];

    // 赠送礼物
    public Dictionary<int, int> m_sendGift = new Dictionary<int, int>();

    // 相框
    public Dictionary<int, int> m_photoFrame = new Dictionary<int, int>();

    public void reset()
    {
        m_statType = -1;
        for (int i = 0; i < m_generalStat.Length; i++)
        {
            m_generalStat[i] = 0;
        }
        m_sendGift.Clear();
        m_photoFrame.Clear();
    }

    public void addGift(int key, int value)
    {
        if (m_sendGift.ContainsKey(key))
        {
            m_sendGift[key] += value;
        }
        else
        {
            m_sendGift.Add(key, value);
        }
    }

    public void addPhotoFrame(int key, int value)
    {
        if (m_photoFrame.ContainsKey(key))
        {
            m_photoFrame[key] += value;
        }
        else
        {
            m_photoFrame.Add(key, value);
        }
    }

    // 返回统计详情
    public string getValue(int statType)
    {
        if (statType == (int)DataStatType.stat_send_gift)
        {
            return ItemHelp.getRewardList(m_sendGift);
        }

        if (statType == (int)DataStatType.stat_player_vip_level)
            return "";

        if (statType == (int)DataStatType.stat_photo_frame)
        {
            return ItemHelp.getRewardList(m_photoFrame);
        }

        return m_generalStat[statType].ToString();
    }
}

// 查询大厅通用数据，独立数据
public class QueryLobby : QueryBase
{
    private ResultLobby m_result = new ResultLobby();

    public override OpRes doQuery(object param, GMUser user)
    {
        m_result.reset();

        int statType = (int)param;
        if (statType == 1) // 全部
        {
            return queryAll(user);
        }
        return OpRes.op_res_failed;
    }

    // 返回查询结果
    public override object getQueryResult()
    {
        return m_result;
    }

    private OpRes queryAll(GMUser user)
    {
        // 一般统计
        List<Dictionary<string, object>> dataList = DBMgr.getInstance().executeQuery(TableName.PUMP_GENERAL_STAT, user.getDbServerID(), DbName.DB_PUMP);
        for (int i = 0; i < dataList.Count; i++)
        {
            int key = Convert.ToInt32(dataList[i]["key"]);
            int value = Convert.ToInt32(dataList[i]["value"]);
            m_result.m_generalStat[key] += value;
        }

        // 赠送礼物
        dataList = DBMgr.getInstance().executeQuery(TableName.PUMP_SEND_GIFT, user.getDbServerID(), DbName.DB_PUMP);
        for (int i = 0; i < dataList.Count; i++)
        {
            int key = Convert.ToInt32(dataList[i]["key"]);
            int value = Convert.ToInt32(dataList[i]["value"]);
            m_result.addGift(key, value);
        }

        // 相框
        dataList = DBMgr.getInstance().executeQuery(TableName.PUMP_PHOTO_FRAME, user.getDbServerID(), DbName.DB_PUMP);
        for (int i = 0; i < dataList.Count; i++)
        {
            int key = Convert.ToInt32(dataList[i]["key"]);
            int value = Convert.ToInt32(dataList[i]["value"]);
            m_result.addPhotoFrame(key, value);
        }

        m_result.m_statType = 0;

        return OpRes.opres_success;
    }
}

//////////////////////////////////////////////////////////////////////////

public class EarningItem
{
    public string m_time = "";

    // 系统总支出
//    public long m_totalOutlay;

    // 系统总收入
 //   public long m_totalIncome;

    /*
     *      索引0 初级房  索引1 中级房 索引2 高级场 索引3 VIP场  索引4，总计 
     *      索引5 初级场赠予  索引6  中级场赠予
     */
    public long[] m_roomIncome = new long[5];
    public long[] m_roomOutlay = new long[7];
    /* 
     *  索引0  初级房废弹 
     *  索引1  中级房废弹 
     *  索引2  高级场废弹 
     *  索引3  VIP场废弹
     */
    public long[] m_roomAbandonedbullets = new long[5];

    // 返回实际盈利率
    public string getFactExpRate(int roomId)
    {
        if (m_roomIncome[roomId] == 0)
            return "0";

        double factGain = (double)(m_roomIncome[roomId] - m_roomOutlay[roomId]) / m_roomIncome[roomId];
        return Math.Round(factGain, 3).ToString();
    }

    // roomId从0开始
    public void addRoomIncome(int roomId, long addValue)
    {
        m_roomIncome[roomId] += addValue;
    }

    // roomId从0开始
    public void addRoomOutlay(int roomId, long addValue)
    {
        m_roomOutlay[roomId] += addValue;
    }

    public long getRoomIncome(int roomId)
    {
        return m_roomIncome[roomId];
    }

    public long getRoomOutlay(int roomId)
    {
        return m_roomOutlay[roomId];
    }

    public long getDelta(int roomId)
    {
        return m_roomIncome[roomId] - m_roomOutlay[roomId];
    }

    // roomId从0开始
    public void addRoomAbandonedbullets(int roomId, long addValue)
    {
        m_roomAbandonedbullets[roomId] += addValue;
    }

    // roomId从0开始
    public long getRoomAbandonedbullets(int roomId)
    {
        return m_roomAbandonedbullets[roomId];
    }
}

// 收益查询参数
public class ParamServerEarnings : ParamQuery
{
    // 游戏ID
    public int m_gameId;
}

public class ResultServerEarningsTotal : PlayerTypeDataCollect<EarningItem>
{
    public PlayerTypeData<EarningItem> sum(int[] ids)
    {
        PlayerTypeData<EarningItem> result = new PlayerTypeData<EarningItem>();
        foreach (var data in m_data)
        {
            for (int i = 0; i < ids.Length; i++)
            {
                var item = data.Value.getData(ids[i]);
                if (item != null)
                {
                    var res = result.getData(ids[i]);
                    if (res == null)
                    {
                        res = new EarningItem();
                        result.addData(ids[i], res);
                    }

                    res.addRoomIncome(4, item.getRoomIncome(4));
                    res.addRoomOutlay(4, item.getRoomOutlay(4));
                    res.addRoomIncome(0, item.getRoomIncome(0));
                }
            }
        }
        return result;
    }
}

// 服务器收益
public class QueryServerEarnings : QueryBase
{
    private List<EarningItem> m_result = new List<EarningItem>();

    private Dictionary<DateTime, EarningItem> m_total = new Dictionary<DateTime, EarningItem>();

    private ResultServerEarningsTotal m_totalResult = new ResultServerEarningsTotal();

    List<ResultActive> m_activePerson = null;

    public override OpRes doQuery(object param, GMUser user)
    {
        m_result.Clear();
        IMongoQuery imq = null;
        ParamServerEarnings p = (ParamServerEarnings)param;

        if (p.m_time != "")
        {
            DateTime mint = DateTime.Now, maxt = DateTime.Now;
            bool res = Tool.splitTimeStr(p.m_time, ref mint, ref maxt);
            if (!res)
                return OpRes.op_res_time_format_error;

            IMongoQuery imq1 = Query.LT("Date", BsonValue.Create(maxt));
            IMongoQuery imq2 = Query.GTE("Date", BsonValue.Create(mint));
            imq = Query.And(imq1, imq2);
        }

        int gameId = p.m_gameId;
        if (gameId == (int)GameId.fishlord) 
        {
            return queryCommon(user, imq, TableName.PUMP_FISHLORD_EVERY_DAY);
        }
        else if (gameId == (int)GameId.crocodile)
        {
            return queryCommon(user, imq, TableName.PUMP_CROCODILE_EVERY_DAY);
        }
        else if (gameId == (int)GameId.dice)
        {
            return queryCommon(user, imq, TableName.PUMP_DICE_EVERY_DAY);
        }
        else if (gameId == (int)GameId.cows)
        {
            return queryCommon(user, imq, TableName.PUMP_COWS_EVERY_DAY);
        }
        else if (gameId == (int)GameId.baccarat)
        {
            return queryCommon(user, imq, TableName.PUMP_BACCARAT_EVERY_DAY);
        }
        else if (gameId == (int)GameId.dragon)
        {
            return queryCommon(user, imq, TableName.PUMP_DRAGON_EVERY_DAY);
        }
        else if (gameId == (int)GameId.fishpark)
        {
            return queryCommon(user, imq, TableName.PUMP_FISHPARK_EVERY_DAY);
        }
        else if (gameId == (int)GameId.shcd)
        {
            return queryCommon(user, imq, TableName.PUMP_SHCD_EVERY_DAY);
        }
        else if (gameId == (int)GameId.calf_roping)
        {
            return queryCommon(user, imq, TableName.PUMP_CALFROPING_EVERY_DAY);
        }
        else if (gameId == 0)
        {
            statTotal(user, imq, p);
            // queryTotal(user, imq, TableName.PUMP_CALFROPING_EVERY_DAY);

//             var arr = from s in m_total
//                     orderby s.Key
//                     select s.Value;
// 
//             foreach (var r in arr)
//             {
//                 m_result.Add(r);
//             }
            return OpRes.opres_success;
        }
        return OpRes.op_res_failed;
    }

    // 返回查询结果
    public override object getQueryResult()
    {
        return m_result;
    }

    public override object getQueryResult(object param, GMUser user)
    {
        return m_totalResult;
    }

    private OpRes queryCommon(GMUser user, IMongoQuery imq, string tableName)
    {
        List<Dictionary<string, object>> dataList = DBMgr.getInstance().executeQuery(tableName, user.getDbServerID(), DbName.DB_PUMP, imq,
            0, 0, null, "Date", false);
        if (dataList == null)
            return OpRes.op_res_not_found_data;

        for (int i = 0; i < dataList.Count; i++)
        {
            EarningItem item = new EarningItem();
            m_result.Add(item);

            Dictionary<string, object> data = dataList[i];
            item.m_time = Convert.ToDateTime(data["Date"]).ToLocalTime().ToShortDateString();
            if (data.ContainsKey("TodayOutlay"))
            {
                item.addRoomOutlay(4, Convert.ToInt64(data["TodayOutlay"]));
            }
            if (data.ContainsKey("TodayIncome"))
            {
                item.addRoomIncome(4, Convert.ToInt64(data["TodayIncome"]));
            }
            if (data.ContainsKey("room1Protect"))
            {
                item.addRoomOutlay(5, Convert.ToInt64(data["room1Protect"]));
            }
            if (data.ContainsKey("room2Protect"))
            {
                item.addRoomOutlay(6, Convert.ToInt64(data["room2Protect"]));
            }

            if (data.ContainsKey("room1Income"))
            {
                item.addRoomIncome(0, Convert.ToInt64(data["room1Income"]));
            }
            if (data.ContainsKey("room2Income"))
            {
                item.addRoomIncome(1, Convert.ToInt64(data["room2Income"]));
            }
            if (data.ContainsKey("room3Income"))
            {
                item.addRoomIncome(2, Convert.ToInt64(data["room3Income"]));
            }
            if (data.ContainsKey("room4Income"))
            {
                item.addRoomIncome(3, Convert.ToInt64(data["room4Income"]));
            }

            if (data.ContainsKey("room1Outlay"))
            {
                item.addRoomOutlay(0, Convert.ToInt64(data["room1Outlay"]));
            }
            if (data.ContainsKey("room2Outlay"))
            {
                item.addRoomOutlay(1, Convert.ToInt64(data["room2Outlay"]));
            }
            if (data.ContainsKey("room3Outlay"))
            {
                item.addRoomOutlay(2, Convert.ToInt64(data["room3Outlay"]));
            }
            if (data.ContainsKey("room4Outlay"))
            {
                item.addRoomOutlay(3, Convert.ToInt64(data["room4Outlay"]));
            }

            // 废弹相关信息
            if (data.ContainsKey("room1Abandonedbullets"))
            {
                item.addRoomAbandonedbullets(0, Convert.ToInt64(data["room1Abandonedbullets"]));
            }
            if (data.ContainsKey("room2Abandonedbullets"))
            {
                item.addRoomAbandonedbullets(1, Convert.ToInt64(data["room2Abandonedbullets"]));
            }
            if (data.ContainsKey("room3Abandonedbullets"))
            {
                item.addRoomAbandonedbullets(2, Convert.ToInt64(data["room3Abandonedbullets"]));
            }
            if (data.ContainsKey("room4Abandonedbullets"))
            {
                item.addRoomAbandonedbullets(3, Convert.ToInt64(data["room4Abandonedbullets"]));
            }
        }

        return OpRes.opres_success;
    }

    private OpRes queryTotal(GMUser user, IMongoQuery imq, string tableName, int gameId)
    {
        List<Dictionary<string, object>> dataList = DBMgr.getInstance().executeQuery(tableName, user.getDbServerID(), DbName.DB_PUMP, imq,
            0, 0, null, "Date", false);
        if (dataList == null)
            return OpRes.op_res_failed;

        for (int i = 0; i < dataList.Count; i++)
        {
            EarningItem item = null;
            DateTime t = Convert.ToDateTime(dataList[i]["Date"]).ToLocalTime();
            item = new EarningItem();
           
            item.m_time = t.ToShortDateString();
            if (dataList[i].ContainsKey("TodayOutlay"))
            {
                item.addRoomOutlay(4, Convert.ToInt64(dataList[i]["TodayOutlay"]));
            }
            if (dataList[i].ContainsKey("TodayIncome"))
            {
                item.addRoomIncome(4, Convert.ToInt64(dataList[i]["TodayIncome"]));
            }

            item.addRoomIncome(0, getActivePerson(t, gameId));
            m_totalResult.addData(t, gameId, item);
        }

        return OpRes.opres_success;
    }

    void statTotal(GMUser user, IMongoQuery imq, ParamServerEarnings p)
    {
        m_totalResult.reset();
        OpRes res = user.doStat(p.m_time, StatType.statTypeActivePerson);
        m_activePerson = (List<ResultActive>)user.getStatResult(StatType.statTypeActivePerson);

        queryTotal(user, imq, TableName.PUMP_FISHLORD_EVERY_DAY, (int)GameId.fishlord);
        queryTotal(user, imq, TableName.PUMP_CROCODILE_EVERY_DAY, (int)GameId.crocodile);
        queryTotal(user, imq, TableName.PUMP_COWS_EVERY_DAY, (int)GameId.cows);
        queryTotal(user, imq, TableName.PUMP_DRAGON_EVERY_DAY, (int)GameId.dragon);
        queryTotal(user, imq, TableName.PUMP_SHCD_EVERY_DAY, (int)GameId.shcd);
    }

    int getActivePerson(DateTime t, int gameId)
    {
        int res = 0;
        foreach (var item in m_activePerson)
        {
            if (item.m_time == t.ToString())
            {
                res = item.getCount(gameId);
                break;
            }
        }
        return res;
    }
}

//////////////////////////////////////////////////////////////////////////

public class ResultFishLord
{
    // 进入每个房间的次数
    public long[] m_enterRoomCount = new long[5];

    public void reset()
    {
        for (int i = 1; i < m_enterRoomCount.Length; i++)
        {
            m_enterRoomCount[i] = 0;
        }
    }

    public void addCount(int roomId, long count)
    {
        if (roomId >= m_enterRoomCount.Length)
            return;

        m_enterRoomCount[roomId] = count;
    }

    public int getRoomCount()
    {
        return m_enterRoomCount.Length;
    }

    public string getEnterRoomCount(int roomId)
    {
        return m_enterRoomCount[roomId].ToString();
    }

    public string getRoomName(int roomId)
    {
        return StrName.s_roomName[roomId - 1];
    }
}

// 捕鱼独立数据
public class QueryIndependentFishlord : QueryBase
{
    private ResultFishLord m_result = new ResultFishLord();

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
        List<Dictionary<string, object>> dataList = DBMgr.getInstance().executeQuery(TableName.FISHLORD_ROOM, user.getDbServerID(), DbName.DB_GAME);
        for (int i = 0; i < dataList.Count; i++)
        {
            int roomId = Convert.ToInt32(dataList[i]["room_id"]);
            long count = 0;
            if (dataList[i].ContainsKey("EnterCount"))
            {
                count = Convert.ToInt64(dataList[i]["EnterCount"]);
            }
            m_result.addCount(roomId, count);
        }

        return OpRes.opres_success;
    }
}

//////////////////////////////////////////////////////////////////////////

public class CrocodileInfo
{
    // 下注次数
    public long m_betCount;
    // 获奖次数
    public long m_winCount;
    // 总收入
    public long m_income;
    // 总支出
    public long m_outlay;

    // 返回实际盈利率
    public string getFactExpRate()
    {
        if (m_income == 0 && m_outlay == 0)
            return "0";
        if (m_income == 0)
            return "-∞";

        double factGain = (double)(m_income - m_outlay) / m_income;
        return Math.Round(factGain, 3).ToString();
    } 
}

public class ResultIndependent : ResultFishLord
{
    // 每个区域的信息
    public Dictionary<int, CrocodileInfo> m_betInfo = new Dictionary<int, CrocodileInfo>();

    public new void reset()
    {
        base.reset();
        m_betInfo.Clear();
    }

    public void addBetCount(int betId, long betCount, long winCount, long income, long outlay)
    {
        CrocodileInfo info = null;
        if (m_betInfo.ContainsKey(betId))
        {
            info = m_betInfo[betId];
        }
        else
        {
            info = new CrocodileInfo();
            m_betInfo.Add(betId, info);
        }
        info.m_betCount += betCount;
        info.m_winCount += winCount;
        info.m_income += income;
        info.m_outlay += outlay;
    }
}

public class ResultCrocodile : ResultIndependent
{
    public string getAreaName(int areaId)
    {
        Crocodile_RateCFGData data = Crocodile_RateCFG.getInstance().getValue(areaId);
        if (data != null)
            return data.m_name;
        return "";
    }
}

// 鳄鱼独立数据
public class QueryIndependentCrocodile : QueryBase
{
    private ResultCrocodile m_result = new ResultCrocodile();

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
        List<Dictionary<string, object>> dataList = DBMgr.getInstance().executeQuery(TableName.CROCODILE_ROOM, user.getDbServerID(), DbName.DB_GAME);
        for (int i = 0; i < dataList.Count; i++)
        {
            int roomId = Convert.ToInt32(dataList[i]["room_id"]);
            long count = 0;
            if (dataList[i].ContainsKey("enter_count"))
            {
                count = Convert.ToInt64(dataList[i]["enter_count"]);
            }
            m_result.addCount(roomId, count);
        }

        dataList = DBMgr.getInstance().executeQuery(TableName.PUMP_CROCODILE_BET, user.getDbServerID(), DbName.DB_PUMP);
        for (int i = 0; i < dataList.Count; i++)
        {
            int betId = Convert.ToInt32(dataList[i]["BetID"]);
            long count1 = Convert.ToInt64(dataList[i]["BetCount"]);
            long count2 = Convert.ToInt64(dataList[i]["BetWin"]);

            long income = 0, outlay = 0;
            if (dataList[i].ContainsKey("BetInCome"))
            {
                income = Convert.ToInt64(dataList[i]["BetInCome"]);
            }
            if (dataList[i].ContainsKey("BetOutCome"))
            {
                outlay = Convert.ToInt64(dataList[i]["BetOutCome"]);
            }
            m_result.addBetCount(betId, count1, count2, income, outlay);
        }

        return OpRes.opres_success;
    }
}

//////////////////////////////////////////////////////////////////////////

public class ResultDice : ResultIndependent
{
    public string getAreaName(int areaId)
    {
        Dice_BetAreaCFGData data = Dice_BetAreaCFG.getInstance().getValue(areaId);
        if (data != null)
            return data.m_name;
        return "";
    }
}

// 骰宝独立数据
public class QueryIndependentDice : QueryBase
{
    private ResultDice m_result = new ResultDice();

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
        List<Dictionary<string, object>> dataList = DBMgr.getInstance().executeQuery(TableName.DICE_ROOM, user.getDbServerID(), DbName.DB_GAME);
        for (int i = 0; i < dataList.Count; i++)
        {
            int roomId = Convert.ToInt32(dataList[i]["room_id"]);
            long count = 0;
            if (dataList[i].ContainsKey("enter_count"))
            {
                count = Convert.ToInt64(dataList[i]["enter_count"]);
            }
            m_result.addCount(roomId, count);
        }

        dataList = DBMgr.getInstance().executeQuery(TableName.PUMP_DICE, user.getDbServerID(), DbName.DB_PUMP);
        for (int i = 0; i < dataList.Count; i++)
        {
            int betId = Convert.ToInt32(dataList[i]["bet_id"]);
            long count1 = Convert.ToInt64(dataList[i]["bet_count"]);
            long count2 = Convert.ToInt64(dataList[i]["win_count"]);
            long income = 0, outlay = 0;
            if (dataList[i].ContainsKey("Income"))
            {
                income = Convert.ToInt64(dataList[i]["Income"]);
            }
            if (dataList[i].ContainsKey("Outlay"))
            {
                outlay = Convert.ToInt64(dataList[i]["Outlay"]);
            }
            m_result.addBetCount(betId, count1, count2, income, outlay);
        }

        return OpRes.opres_success;
    }
}

//////////////////////////////////////////////////////////////////////////

public class ResultCows : ResultIndependent
{
    public string getAreaName(int areaId)
    {
        return StrName.s_cowsArea[areaId - 1];
    }

    // 返回盈利率
    public string getRate(long income, long outlay)
    {
        if (outlay == 0)
            return "1";

        double factGain = (double)income / outlay;
        return Math.Round(factGain, 3).ToString();
    } 
}

// 牛牛独立数据--各区域的下注，获奖情况
public class QueryIndependentCows : QueryBase
{
    private ResultCows m_result = new ResultCows();

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
        List<Dictionary<string, object>> dataList = DBMgr.getInstance().executeQuery(TableName.COWS_ROOM, 
            user.getDbServerID(), DbName.DB_GAME);

        for (int i = 0; i < dataList.Count; i++)
        {
            for (int k = 1; k <= 4; k++)
            {
                // 总收入
                string totalWin = string.Format("WinGold{0}", k);
                // 总支出
                string totalBetGold = string.Format("LoseGold{0}", k);
                // 盈的总次数
                string totalWinCount = string.Format("WinCount{0}", k);

                long count2 = Convert.ToInt64(dataList[i][totalWinCount]);
                long income = 0, outlay = 0;
                if (dataList[i].ContainsKey(totalWin))
                {
                    income = Convert.ToInt64(dataList[i][totalWin]);
                }
                if (dataList[i].ContainsKey(totalBetGold))
                {
                    outlay = Convert.ToInt64(dataList[i][totalBetGold]);
                }
                m_result.addBetCount(k, 0, count2, income, outlay);
            }
        }

        return OpRes.opres_success;
    }
}

//////////////////////////////////////////////////////////////////////////

public class QueryEarningsParam : QueryBase
{
    protected Dictionary<int, ResultFishlordExpRate> m_result = new Dictionary<int, ResultFishlordExpRate>();

    public override object getQueryResult()
    {
        return m_result;
    }

    protected OpRes query(GMUser user, string tableName)
    {
        List<Dictionary<string, object>> dataList = DBMgr.getInstance().executeQuery(tableName,
            user.getDbServerID(), DbName.DB_GAME);
        if (dataList == null)
            return OpRes.opres_success;

        for (int i = 0; i < dataList.Count; i++)
        {
            ResultFishlordExpRate info = new ResultFishlordExpRate();
            info.m_roomId = Convert.ToInt32(dataList[i]["room_id"]);
            if (dataList[i].ContainsKey("room_income"))
            {
                info.m_totalIncome = Convert.ToInt64(dataList[i]["room_income"]);
            }
            if (dataList[i].ContainsKey("room_outcome"))
            {
                info.m_totalOutlay = Convert.ToInt64(dataList[i]["room_outcome"]);
            }
            if (dataList[i].ContainsKey("player_charge"))
            {
                info.m_playerCharge = Convert.ToInt64(dataList[i]["player_charge"]);
            }

            if (dataList[i].ContainsKey("rob_income"))
            {
                info.m_robotIncome = Convert.ToInt64(dataList[i]["rob_income"]);
            }
            if (dataList[i].ContainsKey("rob_outcome"))
            {
                info.m_robotOutlay = Convert.ToInt64(dataList[i]["rob_outcome"]);
            }
            m_result.Add(info.m_roomId, info);
        }

        return OpRes.opres_success;
    }
}

// 骰宝盈利参数查询
public class QueryDiceEarningsParam : QueryEarningsParam
{
    public override OpRes doQuery(object param, GMUser user)
    {
        m_result.Clear();
        return query(user, TableName.DICE_ROOM);
    }
}

// 百家乐盈利参数查询
public class QueryBaccaratEarningsParam : QueryEarningsParam
{
    public override OpRes doQuery(object param, GMUser user)
    {
        m_result.Clear();
        return query(user, TableName.BACCARAT_ROOM);
    }
}

//////////////////////////////////////////////////////////////////////////

public class ResultNoticeInfo
{
    public string m_id = "";
    public string m_genTime = "";
    public string m_deadTime = "";
    public string m_title = "";
    public string m_content = "";
    public string m_comment = "";

    // 排序字段
    public int m_order;
}

// 当前公告信息
public class QueryCurNotice : QueryBase
{
    private List<ResultNoticeInfo> m_result = new List<ResultNoticeInfo>();

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
        List<Dictionary<string, object>> dataList = DBMgr.getInstance().executeQuery(TableName.OPERATION_NOTIFY, 
            user.getDbServerID(), DbName.DB_PLAYER);

        for (int i = 0; i < dataList.Count; i++)
        {
            ResultNoticeInfo info = new ResultNoticeInfo();
            m_result.Add(info);

            info.m_id = Convert.ToString(dataList[i]["noticeId"]);
            info.m_genTime = Convert.ToDateTime(dataList[i]["genTime"]).ToLocalTime().ToString();
            info.m_deadTime = Convert.ToDateTime(dataList[i]["deadTime"]).ToLocalTime().ToString();
            info.m_title = Convert.ToString(dataList[i]["title"]);
            info.m_content = Convert.ToString(dataList[i]["content"]);
            if (dataList[i].ContainsKey("comment"))
            {
                info.m_comment = Convert.ToString(dataList[i]["comment"]);
            }
            if (dataList[i].ContainsKey("order"))
            {
                info.m_order = Convert.ToInt32(dataList[i]["order"]);
            }
        }

        return OpRes.opres_success;
    }
}

//////////////////////////////////////////////////////////////////////////

public class ResultFishlordExpRate
{
    public int m_roomId;
    public double m_expRate;
    public long m_totalIncome;
    public long m_totalOutlay;

    // 手续费
    public long m_playerCharge = 0;

    // 玩家个数
    public int m_curPlayerCount = 0;

    // 机器人收入
    public long m_robotIncome = 0;
    // 机器人支出
    public long m_robotOutlay = 0;

    // 废弹(经典捕鱼，鳄鱼公园)
    public long m_abandonedbullets;

    // 捕鱼的导弹产出
    public long m_missileCount;

    // 返回实际盈利率
    public string getFactExpRate()
    {
        long totalIncome = getIncome();
        if (totalIncome == 0 && m_totalOutlay == 0)
            return "0";
        if (totalIncome == 0)
            return "-∞";

        double factGain = (double)(totalIncome - m_totalOutlay) / totalIncome;
        return Math.Round(factGain, 3).ToString();
    }

    // 手续费计入总收入内
    public long getDelta()
    {
        return getIncome() - m_totalOutlay;
    }

    public long getIncome()
    {
        return m_totalIncome + m_playerCharge;
    }

    // 返回盈利率
    public string getRate(long income, long outlay)
    {
        if (outlay == 0)
            return "1";

        double factGain = (double)income / outlay;
        return Math.Round(factGain, 3).ToString();
    }
}

public class ParamFishlordBoss
{
    public string m_midTime;
    public string m_highTime;
}

// 捕鱼参数查询
public class QueryFishlordParam : QueryBase
{
    protected Dictionary<int, ResultFishlordExpRate> m_result = new Dictionary<int, ResultFishlordExpRate>();
    protected List<ResultFishlordExpRate> m_result1 = new List<ResultFishlordExpRate>();

    public override OpRes doQuery(object param, GMUser user)
    {
        if (param == null)
        {
            m_result.Clear();
            return query(user, TableName.FISHLORD_ROOM);
        }

        ParamFishlordBoss p = (ParamFishlordBoss)param;
        m_result1.Clear();
        return queryBoss(user, p);
    }

    public override object getQueryResult()
    {
        return m_result;
    }

    public override object getQueryResult(object param, GMUser user) { return m_result1; }

    protected OpRes query(GMUser user, string tableName)
    {
        List<Dictionary<string, object>> dataList = DBMgr.getInstance().executeQuery(tableName,
            user.getDbServerID(), DbName.DB_GAME);
        if (dataList == null)
            return OpRes.opres_success;

        for (int i = 0; i < dataList.Count; i++)
        {
            ResultFishlordExpRate info = new ResultFishlordExpRate();
            info.m_roomId = Convert.ToInt32(dataList[i]["room_id"]);
            if (dataList[i].ContainsKey("EarningsRate"))
            {
                info.m_expRate = Convert.ToDouble(dataList[i]["EarningsRate"]);
            }
            else
            {
                info.m_expRate = 0.05;
            }
            if (dataList[i].ContainsKey("TotalIncome"))
            {
                info.m_totalIncome = Convert.ToInt64(dataList[i]["TotalIncome"]);
            }
            if (dataList[i].ContainsKey("TotalOutlay"))
            {
                info.m_totalOutlay = Convert.ToInt64(dataList[i]["TotalOutlay"]);
            }
            if (dataList[i].ContainsKey("player_count"))
            {
                info.m_curPlayerCount = Convert.ToInt32(dataList[i]["player_count"]);
            }
            if (dataList[i].ContainsKey("Abandonedbullets"))
            {
                info.m_abandonedbullets = Convert.ToInt64(dataList[i]["Abandonedbullets"]);
            }
            if (dataList[i].ContainsKey("MissileCount"))
            {
                info.m_missileCount = Convert.ToInt64(dataList[i]["MissileCount"]);
            }

            m_result.Add(info.m_roomId, info);
        }

        return OpRes.opres_success;
    }

    OpRes queryBoss(GMUser user, ParamFishlordBoss param)
    {
        queryBoss(user, param.m_midTime, 2);
        queryBoss(user, param.m_highTime, 3);
        return OpRes.opres_success;
    }

    void queryBoss(GMUser user, string time, int roomId)
    {
        if (string.IsNullOrEmpty(time))
            return;

        DateTime mint = DateTime.Now, maxt = DateTime.Now;
        bool res = Tool.splitTimeStr(time, ref mint, ref maxt);
        if (!res)
            return;

        DateTime curT = DateTime.Now.Date.AddDays(1);
        IMongoQuery imq1 = Query.LT("date", BsonValue.Create(curT));
        IMongoQuery imq2 = Query.GTE("date", BsonValue.Create(mint));
        IMongoQuery imq = Query.And(imq1, imq2, Query.EQ("roomid", roomId));

        List<Dictionary<string, object>> dataList = DBMgr.getInstance().executeQuery(TableName.PUMP_BOSSINFO,
            user.getDbServerID(), DbName.DB_PUMP, imq, 0, 0, null, "date", false);
        if (dataList == null)
            return;

        ResultFishlordExpRate info = new ResultFishlordExpRate();
        info.m_roomId = roomId;

        for (int i = 0; i < dataList.Count; i++)
        {
            Dictionary<string, object> data = dataList[i];

            info.m_robotIncome += Convert.ToInt64(data["consume_gold"]);

            if (data.ContainsKey("bossReleaseGold"))
            {
                info.m_robotOutlay += Convert.ToInt64(data["bossReleaseGold"]);
            }

            info.m_robotOutlay += Convert.ToInt64(data["dargonball"]) * 5000;
        }

        ResultFishlordExpRate f = m_result[roomId];
        info.m_totalIncome = f.m_totalIncome - info.m_robotIncome;
        info.m_totalOutlay = f.m_totalOutlay - info.m_robotOutlay;

        m_result1.Add(info);
    }
}

// 鳄鱼公园参数查询
public class QueryFishParkParam : QueryFishlordParam
{
    public override OpRes doQuery(object param, GMUser user)
    {
        m_result.Clear();
        return query(user, TableName.FISHPARK_ROOM);
    }
}

//////////////////////////////////////////////////////////////////////////
// 经典捕鱼桌子参数查询
public class QueryFishlordDeskParam : QueryBase
{
    // 以桌子ID为key
    protected List<ResultFishlordExpRate> m_result = new List<ResultFishlordExpRate>();

    public override OpRes doQuery(object param, GMUser user)
    {
        return _doQuery(param, user, TableName.FISHLORD_ROOM_DESK);
    }

    public override object getQueryResult()
    {
        return m_result;
    }

    protected OpRes _doQuery(object param, GMUser user, string tableName)
    {
        m_result.Clear();
        ParamQueryGift p = (ParamQueryGift)param;
        IMongoQuery imq = Query.EQ("room_id", BsonValue.Create(p.m_state));
        return query(user, imq, p, tableName);
    }

    private OpRes query(GMUser user,IMongoQuery imq, ParamQueryGift param, string tableName)
    {
        user.totalRecord = DBMgr.getInstance().getRecordCount(tableName, imq, user.getDbServerID(), DbName.DB_GAME);

        List<Dictionary<string, object>> dataList = DBMgr.getInstance().executeQuery(tableName,
            user.getDbServerID(), DbName.DB_GAME, imq, 
            (param.m_curPage - 1) * param.m_countEachPage,
            param.m_countEachPage, null, "table_id");

        if (dataList == null)
            return OpRes.opres_success;

        for (int i = 0; i < dataList.Count; i++)
        {
            ResultFishlordExpRate info = new ResultFishlordExpRate();
            info.m_roomId = Convert.ToInt32(dataList[i]["table_id"]);
            if (dataList[i].ContainsKey("TotalIncome"))
            {
                info.m_totalIncome = Convert.ToInt64(dataList[i]["TotalIncome"]);
            }
            if (dataList[i].ContainsKey("TotalOutlay"))
            {
                info.m_totalOutlay = Convert.ToInt64(dataList[i]["TotalOutlay"]);
            }
            if (dataList[i].ContainsKey("Abandonedbullets"))
            {
                info.m_abandonedbullets = Convert.ToInt64(dataList[i]["Abandonedbullets"]);
            }

            m_result.Add(info);
        }

        return OpRes.opres_success;
    }
}

public class QueryFishParkDeskParam : QueryFishlordDeskParam
{
    public override OpRes doQuery(object param, GMUser user)
    {
        return _doQuery(param, user, TableName.FISHPARK_ROOM_DESK);
    }
}

//////////////////////////////////////////////////////////////////////////

// 鳄鱼参数查询
public class QueryCrocodileParam : QueryBase
{
    private Dictionary<int, ResultFishlordExpRate> m_result = new Dictionary<int, ResultFishlordExpRate>();

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
        List<Dictionary<string, object>> dataList = DBMgr.getInstance().executeQuery(TableName.CROCODILE_ROOM,
            user.getDbServerID(), DbName.DB_GAME);
        if (dataList == null)
            return OpRes.opres_success;
 
        for (int i = 0; i < dataList.Count; i++)
        {
            ResultFishlordExpRate info = new ResultFishlordExpRate();
            info.m_roomId = Convert.ToInt32(dataList[i]["room_id"]);
            if (dataList[i].ContainsKey("ExpectEarnRate"))
            {
                info.m_expRate = Convert.ToDouble(dataList[i]["ExpectEarnRate"]);
            }
            else
            {
                info.m_expRate = 0.05;
            }
            if (dataList[i].ContainsKey("room_income"))
            {
                info.m_totalIncome = Convert.ToInt64(dataList[i]["room_income"]);
            }
            if (dataList[i].ContainsKey("room_outcome"))
            {
                info.m_totalOutlay = Convert.ToInt64(dataList[i]["room_outcome"]);
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

public class ResultFish
{
    public int m_fishId;
    public string m_fishName = "";
    public long m_hitCount;
    public long m_dieCount;
    public long m_outlay;
    public long m_income;
    public int m_roomId;

    // 死亡/击中
    public string getHit_Die()
    {
        if (m_hitCount == 0)
            return "无穷大";

        double val = (double)m_dieCount / m_hitCount *100;
        return Math.Round(val, 3).ToString() + "%";
    }

    public string getOutlay_Income()
    {
        if (m_income == 0 && m_outlay == 0)
            return "0";
        if (m_income == 0)
            return "-∞";

        double factGain = (double)(m_income - m_outlay) * 100 / m_income;
        return Math.Round(factGain, 3).ToString() + "%";
    }
}

// 鱼
public class QueryFish : QueryBase
{
    private List<ResultFish> m_result = new List<ResultFish>();

    public override OpRes doQuery(object param, GMUser user)
    {
        return _doQuery(param, user, TableName.PUMP_ALL_FISH);
    }

    protected OpRes _doQuery(object param, GMUser user, string tableName)
    {
        m_result.Clear();
        int roomId = (int)param;
        IMongoQuery imq = null;
        if (roomId > 0)
        {
            imq = Query.EQ("roomid", BsonValue.Create(roomId));
        }
        OpRes res = query(user, imq, tableName);
        m_result.Sort(sortFish);
        return res;
    }

    public override object getQueryResult()
    {
        return m_result;
    }

    private OpRes query(GMUser user, IMongoQuery imq, string tableName)
    {
        List<Dictionary<string, object>> dataList = DBMgr.getInstance().executeQuery(tableName,
            user.getDbServerID(), DbName.DB_PUMP, imq);
        if (dataList == null)
            return OpRes.op_res_failed;

        for (int i = 0; i < dataList.Count; i++)
        {
            ResultFish info = new ResultFish();
            info.m_fishId = Convert.ToInt32(dataList[i]["fishid"]);
            info.m_hitCount = Convert.ToInt64(dataList[i]["hitcount"]);
            info.m_dieCount = Convert.ToInt64(dataList[i]["deadcount"]);
            info.m_outlay = Convert.ToInt64(dataList[i]["totaloutlay"]);
            info.m_income = Convert.ToInt64(dataList[i]["totalincome"]);
            if (dataList[i].ContainsKey("roomid"))
            {
                info.m_roomId = Convert.ToInt32(dataList[i]["roomid"]);
            }
            m_result.Add(info);
        }

        return OpRes.opres_success;
    }

    private int sortFish(ResultFish p1, ResultFish p2)
    {
        if(p1.m_roomId == p2.m_roomId)
            return p1.m_fishId - p2.m_fishId;
        return p1.m_roomId - p2.m_roomId;
    }
}

// 鳄鱼公园鱼的统计
public class QueryFishParkStat : QueryFish
{
    public override OpRes doQuery(object param, GMUser user)
    {
        return _doQuery(param, user, TableName.PUMP_ALL_FISH_PARK);
    }
}

//////////////////////////////////////////////////////////////////////////

public class ResultMoneyMost
{
    public int m_playerId;
    public string m_nickName = "";
    public int m_val;
    public int m_safeBox;
}

// 金币最多
public class QueryMoneyAtMost : QueryBase
{
    private List<ResultMoneyMost> m_result = new List<ResultMoneyMost>();
    static string[] s_fieldGold = new string[] { "player_id", "gold", "safeBoxGold" };
    static string[] s_fieldTicket = new string[] { "player_id", "ticket" };
    static string[] s_field = new string[] { "nickname" };

    public override OpRes doQuery(object param, GMUser user)
    {
        m_result.Clear();
        ParamQuery p = (ParamQuery)param;
        IMongoQuery imq = Query.EQ("is_robot", false);
        switch (p.m_way)
        {
            case QueryWay.by_way0:
                {
                    return queryGold(user, p.m_countEachPage, imq);
                }
                break;
            case QueryWay.by_way1:
                {
                    return queryTicket(user, p.m_countEachPage, imq);
                }
                break;
        }
        return OpRes.op_res_failed;
    }

    public override object getQueryResult()
    {
        return m_result;
    }

    private OpRes queryGold(GMUser user, int maxCount, IMongoQuery imq)
    {
        List<Dictionary<string, object>> dataList = DBMgr.getInstance().executeQuery(TableName.PLAYER_INFO,
            user.getDbServerID(), DbName.DB_PLAYER, imq, 0, maxCount, s_fieldGold, "gold", false);

        for (int i = 0; i < dataList.Count; i++)
        {
            ResultMoneyMost info = new ResultMoneyMost();
            info.m_playerId = Convert.ToInt32(dataList[i]["player_id"]);
            info.m_val = Convert.ToInt32(dataList[i]["gold"]);

            Dictionary<string, object> ret = getPlayerProperty(info.m_playerId, user, s_field);
            if (ret != null && ret.ContainsKey("nickname"))
            {
                info.m_nickName = Convert.ToString(ret["nickname"]);
            }
            if (dataList[i].ContainsKey("safeBoxGold"))
            {
                info.m_safeBox = Convert.ToInt32(dataList[i]["safeBoxGold"]);
            }
            m_result.Add(info);
        }

        return OpRes.opres_success;
    }

    private OpRes queryTicket(GMUser user, int maxCount, IMongoQuery imq)
    {
        List<Dictionary<string, object>> dataList = DBMgr.getInstance().executeQuery(TableName.PLAYER_INFO,
            user.getDbServerID(), DbName.DB_PLAYER, imq, 0, maxCount, s_fieldTicket, "ticket", false);

        for (int i = 0; i < dataList.Count; i++)
        {
            ResultMoneyMost info = new ResultMoneyMost();
            info.m_playerId = Convert.ToInt32(dataList[i]["player_id"]);
            info.m_val = Convert.ToInt32(dataList[i]["ticket"]);

            Dictionary<string, object> ret = getPlayerProperty(info.m_playerId, user, s_field);
            if (ret != null && ret.ContainsKey("nickname"))
            {
                info.m_nickName = Convert.ToString(ret["nickname"]);
            }
            m_result.Add(info);
        }

        return OpRes.opres_success;
    }
}

//////////////////////////////////////////////////////////////////////////

public class ResultOldEarningRateItem
{
    public int m_gameId;
    public string m_resetTime = "";
    public int m_roomId;
    public long m_income;
    public long m_outlay;
    public double m_expRate;

    // 返回实际盈利率
    public string getFactExpRate()
    {
        if (m_income == 0 && m_outlay == 0)
            return "0";
        if (m_income == 0)
            return "-∞";

        double factGain = (double)(m_income - m_outlay) / m_income;
        return Math.Round(factGain, 3).ToString();
    }
}

// 旧有盈利率查询
public class QueryOldEarningRate : QueryBase
{
    private List<ResultOldEarningRateItem> m_result = new List<ResultOldEarningRateItem>();

    public override OpRes doQuery(object param, GMUser user)
    {
        m_result.Clear();
        ParamLottery p = (ParamLottery)param;
        List<IMongoQuery> queryList = new List<IMongoQuery>();
        queryList.Add(Query.EQ("gameId", (int)p.m_way));
        IMongoQuery imq = Query.And(queryList);
        return query(user, imq);
    }

    public override object getQueryResult()
    {
        return m_result;
    }

    private OpRes query(GMUser user, IMongoQuery imq)
    {
        List<Dictionary<string, object>> dataList = DBMgr.getInstance().executeQuery(TableName.PUMP_OLD_EARNINGS_RATE,
            user.getDbServerID(), DbName.DB_PUMP, imq);

        if (dataList != null)
        {
            for (int i = 0; i < dataList.Count; i++)
            {
                ResultOldEarningRateItem info = new ResultOldEarningRateItem();
                info.m_gameId = Convert.ToInt32(dataList[i]["gameId"]);
                info.m_resetTime = Convert.ToDateTime(dataList[i]["time"]).ToLocalTime().ToString();
                info.m_roomId = Convert.ToInt32(dataList[i]["roomId"]);
                info.m_income = Convert.ToInt64(dataList[i]["income"]);
                info.m_outlay = Convert.ToInt64(dataList[i]["outlay"]);

                if (dataList[i].ContainsKey("expRate"))
                {
                    info.m_expRate = Convert.ToDouble(dataList[i]["expRate"]);
                }
                m_result.Add(info);
            }
        }
       
        return OpRes.opres_success;
    }
}

//////////////////////////////////////////////////////////////////////////

public class FishlordStageItem
{
    public string m_time = "";
    public int m_roomId;
    public int m_stage;
    public long m_outlay;
    public long m_income;

    // 返回实际盈利率
    public string getFactExpRate()
    {
        if (m_income == 0 && m_outlay == 0)
            return "0";
        if (m_income == 0)
            return "-∞";

        double factGain = (double)(m_income - m_outlay) / m_income;
        return Math.Round(factGain, 3).ToString();
    } 
}

// 经典捕鱼阶段
public class QueryFishlordStage : QueryBase
{
    private List<FishlordStageItem> m_result = new List<FishlordStageItem>();

    public override OpRes doQuery(object param, GMUser user)
    {
        return _doQuery(param, user, TableName.PUMP_FISH_TABLE_LOG);
    }

    protected OpRes _doQuery(object param, GMUser user, string tableName)
    {
        m_result.Clear();
        ParamQueryGift p = (ParamQueryGift)param;

        List<IMongoQuery> queryList = new List<IMongoQuery>();

        if (p.m_param != "")
        {
            DateTime mint = DateTime.Now, maxt = DateTime.Now;
            bool res = Tool.splitTimeStr(p.m_param, ref mint, ref maxt);
            if (!res)
                return OpRes.op_res_time_format_error;

            IMongoQuery imq1 = Query.LT("time", BsonValue.Create(maxt));
            IMongoQuery imq2 = Query.GTE("time", BsonValue.Create(mint));
            queryList.Add(Query.And(imq1, imq2));
        }

        queryList.Add(Query.EQ("roomid", BsonValue.Create(p.m_state + 1)));

        IMongoQuery imq = queryList.Count > 0 ? Query.And(queryList) : null;

        return query(p, imq, user, tableName);
    }

    // 返回查询结果
    public override object getQueryResult()
    {
        return m_result;
    }

    private OpRes query(ParamQueryGift param, IMongoQuery imq, GMUser user, string tableName)
    {
        user.totalRecord = DBMgr.getInstance().getRecordCount(tableName, 
            imq, user.getDbServerID(), DbName.DB_PUMP);

        List<Dictionary<string, object>> data =
             DBMgr.getInstance().executeQuery(tableName, user.getDbServerID(), DbName.DB_PUMP, imq,
                                              (param.m_curPage - 1) * param.m_countEachPage, 
                                              param.m_countEachPage, null, "time", false);

        if (data == null || data.Count == 0)
        {
            return OpRes.op_res_not_found_data;
        }

        int i = 0;
        for (i = 0; i < data.Count; i++)
        {
            FishlordStageItem tmp = new FishlordStageItem();
            m_result.Add(tmp);

            tmp.m_time = Convert.ToDateTime(data[i]["time"]).ToLocalTime().ToString();
            tmp.m_roomId = Convert.ToInt32(data[i]["roomid"]);
            tmp.m_stage = Convert.ToInt32(data[i]["type"]);
            tmp.m_outlay = Convert.ToInt64(data[i]["outlay"]);
            tmp.m_income = Convert.ToInt64(data[i]["income"]);
        }
        return OpRes.opres_success;
    }
}

// 鳄鱼公园阶段分析
public class QueryFishParkStage : QueryFishlordStage
{
    public override OpRes doQuery(object param, GMUser user)
    {
        return _doQuery(param, user, TableName.PUMP_FISH_PARK_TABLE_LOG);
    }
}

//////////////////////////////////////////////////////////////////////////

// 在线人数查询
public class QueryOnlinePlayerCount : QueryBase
{
    private int m_count = 0;

    public override OpRes doQuery(object param, GMUser user)
    {
        m_count = 0;
        Dictionary<string, object> data = DBMgr.getInstance().getTableData(TableName.COMMON_CONFIG, 
            "type", "cur_playercount", user.getDbServerID(), DbName.DB_PLAYER);
        if (data != null)
        {
            m_count = Convert.ToInt32(data["value"]);
        }
        return OpRes.opres_success;
    }

    public override object getQueryResult()
    {
        return m_count;
    }
}

//////////////////////////////////////////////////////////////////////////

public class ParamQueryOpLog : ParamQuery
{
    public int m_logType;
}

// 查询操作日志
public class QueryOpLog : QueryBase
{
    List<Dictionary<string, object>> m_result = new List<Dictionary<string, object>>();
    private QueryCondition m_cond = new QueryCondition();

    public override OpRes doQuery(object param, GMUser user)
    {
        m_result.Clear();
        m_cond.startQuery();
        OpRes res = makeQuery(param, user, m_cond);
        if (res != OpRes.opres_success)
            return res;

        IMongoQuery imq = m_cond.getImq();
        ParamQuery p = (ParamQuery)param;
        return query(p, imq, user);
    }

    // 返回查询结果
    public override object getQueryResult()
    {
        return m_result;
    }

    public override OpRes makeQuery(object param, GMUser user, QueryCondition queryCond)
    {
        ParamQueryOpLog p = (ParamQueryOpLog)param;
        if (p.m_logType != -1)
        {
            queryCond.addQueryCond("OpType", p.m_logType);
        }
        if(!string.IsNullOrEmpty(p.m_time))
        {
            DateTime mint = DateTime.Now, maxt = DateTime.Now;
            bool res = Tool.splitTimeStr(p.m_time, ref mint, ref maxt);
            if (!res)
                return OpRes.op_res_time_format_error;

            IMongoQuery imq1 = Query.LT("OpTime", BsonValue.Create(maxt));
            IMongoQuery imq2 = Query.GTE("OpTime", BsonValue.Create(mint));
            queryCond.addImq(Query.And(imq1, imq2));
        }
        if (!user.isAdmin())
        {
            queryCond.addImq(Query.EQ("account", user.m_user));
        }
        return OpRes.opres_success;
    }

    private OpRes query(ParamQuery param, IMongoQuery imq, GMUser user)
    {
        user.totalRecord = DBMgr.getInstance().getRecordCount(TableName.OPLOG, imq, 0, DbName.DB_ACCOUNT);

        m_result = DBMgr.getInstance().executeQuery(TableName.OPLOG,
                                                     0,
                                                     DbName.DB_ACCOUNT,
                                                     imq,
                                                     (param.m_curPage - 1) * param.m_countEachPage,
                                                     param.m_countEachPage,
                                                     null,
                                                     "OpTime",
                                                     false);
        return OpRes.opres_success;
    }
}

//////////////////////////////////////////////////////////////////////////

// 查询玩家头像
public class QueryPlayerHead : QueryBase
{
    // 头像所在的地址URL
    private string m_headUrl = "";
    private string[] m_retFields = { "icon_custom" };
    public override OpRes doQuery(object param, GMUser user)
    {
        m_headUrl = "#";
        string strId = (string)param;
        int playerId = 0;
        if (!int.TryParse(strId, out playerId))
        {
            return OpRes.op_res_param_not_valid;
        }

        Dictionary<string, object> data = QueryBase.getPlayerProperty(playerId, user, m_retFields);
        if (data == null)
            return OpRes.op_res_not_found_data;
        if (!data.ContainsKey("icon_custom"))
            return OpRes.op_res_not_found_data;
        
        string head = Convert.ToString(data["icon_custom"]);
        if (head == "")
            return OpRes.op_res_not_found_data;

        uint temp = Convert.ToUInt32(playerId) % 10000;
        string url = WebConfigurationManager.AppSettings["headURL"];
        m_headUrl = string.Format(url, temp, head);
        return OpRes.opres_success;
    }

    // 返回查询结果
    public override object getQueryResult()
    {
        return m_headUrl;
    }
}

//////////////////////////////////////////////////////////////////////////

public class ParamTotalConsume
{
    // 货币类型
    public int m_currencyType;

    // 收入or支出
    public int m_changeType;

    // 时间范围
    public string m_time = "";
}

public class ConsumeOneItem
{
    public long m_totalValue; // 总量
    public long m_totalCount; // 总次数
}

public class TotalConsumeItem
{
    // 统计时间
    public DateTime m_time;

    // 原因-->消耗总量
   // public Dictionary<int, long> m_result = new Dictionary<int, long>();
    public Dictionary<int, ConsumeOneItem> m_result = new Dictionary<int, ConsumeOneItem>();

    public void add(int reason, long value, long count = 0)
    {
        ConsumeOneItem item = new ConsumeOneItem();
        item.m_totalValue = value;
        item.m_totalCount = count;
        m_result.Add(reason, item);
    }

    public ConsumeOneItem getValue(int reason)
    {
        if (m_result.ContainsKey(reason))
            return m_result[reason];

        return null;
    }
}

public class ResultTotalConsume
{
    public HashSet<int> m_fields = new HashSet<int>();

    public List<TotalConsumeItem> m_result = new List<TotalConsumeItem>();

    public void addReason(int reason)
    {
        m_fields.Add(reason);
    }

    public void reset()
    {
        m_fields.Clear();
        m_result.Clear();
    }

    public TotalConsumeItem create(DateTime date)
    {
        foreach (var d in m_result)
        {
            if (d.m_time == date)
                return d;
        }

        TotalConsumeItem item = new TotalConsumeItem();
        m_result.Add(item);
        item.m_time = date;
        return item;
    }

    public string getReason(int r)
    {
        XmlConfig xml = ResMgr.getInstance().getRes("money_reason.xml");
        string result = xml.getString(r.ToString(), "");
        if (result == "")
            return r.ToString();
        return result;
        //return xml.getString(r.ToString(), "");
    }

    public string getFishReason(int r)
    {
        // 购买物品
        if (r >= (int)FishLordExpend.fish_buyitem_start && r < (int)FishLordExpend.fish_useskill_start)
        {
            Fish_ItemCFGData data = Fish_ItemCFG.getInstance().getValue(r);
            if (data != null)
            {
                return "购买 " + data.m_itemName;
            }
        }

        // 使用技能
        if (r >= (int)FishLordExpend.fish_useskill_start && r < (int)FishLordExpend.fish_turrent_uplevel_start)
        {
            r = r - (int)FishLordExpend.fish_useskill_start;
            Fish_BuffCFGData data = Fish_BuffCFG.getInstance().getValue(r);
            if (data != null)
            {
                return "使用技能 " + data.m_buffName;
            }
        }

        // 炮台升级
        if (r >= (int)FishLordExpend.fish_turrent_uplevel_start && r < (int)FishLordExpend.fish_unlock_level_start)
        {
            return "解锁房间-" + (r - (int)FishLordExpend.fish_turrent_uplevel_start);
        }

        // 解锁 数据库中fish_turrent_uplevel_start,  fish_unlock_level_start记反了
        if (r >= (int)FishLordExpend.fish_unlock_level_start && r < (int)FishLordExpend.fish_missile) 
        {
            return "炮台升级-" + (r - (int)FishLordExpend.fish_unlock_level_start);
        }

        if (r >= (int)FishLordExpend.fish_missile)
        {
            return string.Format("导弹消耗({0})", StrName.s_fishRoomName[r - (int)FishLordExpend.fish_missile]);
        }
        return "";
    }

    public int getResultCount()
    {
        return m_result.Count;
    }
}

// 总的货币消耗查询
public class QueryTotalConsume : QueryBase
{
    private ResultTotalConsume m_result = new ResultTotalConsume();

    public override OpRes doQuery(object param, GMUser user)
    {
        m_result.reset();
        ParamTotalConsume p = (ParamTotalConsume)param;

        List<IMongoQuery> queryList = new List<IMongoQuery>();

        DateTime mint = DateTime.Now, maxt = DateTime.Now;
        bool res = Tool.splitTimeStr(p.m_time, ref mint, ref maxt);
        if (!res)
            return OpRes.op_res_time_format_error;

        IMongoQuery imq1 = Query.LT("time", BsonValue.Create(maxt));
        IMongoQuery imq2 = Query.GTE("time", BsonValue.Create(mint));
        queryList.Add(Query.And(imq1, imq2));

        queryList.Add(Query.EQ("changeType", BsonValue.Create(p.m_changeType)));
        queryList.Add(Query.EQ("itemId", BsonValue.Create(p.m_currencyType)));

        IMongoQuery imq = Query.And(queryList);

        return query(user, imq);
    }

    // 返回查询结果
    public override object getQueryResult()
    {
        return m_result;
    }

    private OpRes query(GMUser user, IMongoQuery imq)
    {
        List<Dictionary<string, object>> data =
             DBMgr.getInstance().executeQuery(TableName.PUMP_TOTAL_CONSUME, user.getDbServerID(),
             DbName.DB_PUMP, imq, 0, 0, null, "time", false);

        if (data == null || data.Count == 0)
        {
            return OpRes.op_res_not_found_data;
        }

        int i = 0;
        for (i = 0; i < data.Count; i++)
        {
            DateTime t = Convert.ToDateTime(data[i]["time"]).ToLocalTime();
            TotalConsumeItem item = m_result.create(t);

            int reason = Convert.ToInt32(data[i]["reason"]);
            long val = Convert.ToInt64(data[i]["value"]);
            long count = 0;
            if (data[i].ContainsKey("count"))
            {
                count = Convert.ToInt64(data[i]["count"]);
            }
            item.add(reason, val, count);

            m_result.addReason(reason);
        }
        
        return OpRes.opres_success;
    }
}

//////////////////////////////////////////////////////////////////////////

public class GameRechargeItem
{
    // 游戏名称-->充值总额
    public Dictionary<string, long> m_recharge = new Dictionary<string, long>();

    public DateTime m_time;

    // 总的金额
    public long m_totalRecharge = 0;

    public void add(string game, long value)
    {
        m_recharge.Add(game, value);
    }

    public long getValue(string game)
    {
        if (m_recharge.ContainsKey(game))
            return m_recharge[game];

        return 0;
    }
}

public class ResultGameRecharge
{
    public HashSet<string> m_fields = new HashSet<string>();

    public List<GameRechargeItem> m_result = new List<GameRechargeItem>();

    public void addGame(string game)
    {
        m_fields.Add(game);
    }

    public void reset()
    {
        m_fields.Clear();
        m_result.Clear();
    }

    public GameRechargeItem create(DateTime date)
    {
        foreach (var d in m_result)
        {
            if (d.m_time == date)
                return d;
        }

        GameRechargeItem item = new GameRechargeItem();
        m_result.Add(item);
        item.m_time = date;
        return item;
    }
}

// 按天查询每个游戏的充值总额
public class QueryGameRechargeByDay : QueryBase
{
    private ResultGameRecharge m_result = new ResultGameRecharge();

    public override OpRes doQuery(object param, GMUser user)
    {
        m_result.reset();
        string time = (string)param;

        List<IMongoQuery> queryList = new List<IMongoQuery>();

        DateTime mint = DateTime.Now, maxt = DateTime.Now;
        bool res = Tool.splitTimeStr(time, ref mint, ref maxt);
        if (!res)
            return OpRes.op_res_time_format_error;

        IMongoQuery imq1 = Query.LT("date", BsonValue.Create(maxt));
        IMongoQuery imq2 = Query.GTE("date", BsonValue.Create(mint));
        queryList.Add(Query.And(imq1, imq2));

        IMongoQuery imq = Query.And(queryList);

        return query(user, imq);
    }

    // 返回查询结果
    public override object getQueryResult()
    {
        return m_result;
    }

    private OpRes query(GMUser user, IMongoQuery imq)
    {
        int serverId = DBMgr.getInstance().getSpecialServerId(DbName.DB_PAYMENT);
        if (serverId == -1)
            return OpRes.op_res_failed;

        List<Dictionary<string, object>> data =
             DBMgr.getInstance().executeQuery(TableName.GAME_RECHARGE_INFO, serverId,
             DbName.DB_PAYMENT, imq);

        if (data == null || data.Count == 0)
        {
            return OpRes.op_res_not_found_data;
        }

        int i = 0;
        for (i = 0; i < data.Count; i++)
        {
            DateTime t = Convert.ToDateTime(data[i]["date"]).ToLocalTime();
            GameRechargeItem item = m_result.create(t);

            foreach (var game in data[i].Keys)
            {
                if (game == "date" || game == "_id")
                    continue;

                if (game == "total_rmb")
                {
                    item.m_totalRecharge = Convert.ToInt64(data[i]["total_rmb"]);
                }
                else
                {
                    long val = Convert.ToInt64(data[i][game]);
                    item.add(game, val);
                    m_result.addGame(game);
                }
            }
        }

        return OpRes.opres_success;
    }
}

//////////////////////////////////////////////////////////////////////////

public class ResultCoinGrowthRank
{
    public int m_playerId;
    public string m_acc = "";
    public string m_nickName = "";
    public int m_vipLevel;
    public long m_gold;
    public string m_time = "";
}

public class ResultRankItem
{
    public int m_playerId;
    public long m_value;
    public int m_rechargeTotal;
    public DateTime m_lastLogin;
}

public class ResultRank
{
    public Dictionary<DateTime, List<ResultRankItem>> m_result =
        new Dictionary<DateTime, List<ResultRankItem>>();

    public List<DateTime> m_timeList = new List<DateTime>();

    public void reset()
    {
        m_timeList.Clear();
        m_result.Clear();
    }

    public void add(DateTime t, ResultRankItem item)
    {
        List<ResultRankItem> res = null;
        if (m_result.ContainsKey(t))
        {
            res = m_result[t];
        }
        else
        {
            res = new List<ResultRankItem>();
            m_result.Add(t, res);
        }
        res.Add(item);
    }

    public List<ResultRankItem> getRank(DateTime t)
    {
        if (m_result.ContainsKey(t))
        {
            return m_result[t];
        }
        return null;
    }

    public string getJson(GMUser user)
    {
        string str = "";
        Dictionary<string, object> ret = new Dictionary<string, object>();
        for (int i = m_timeList.Count - 1; i >= 0; i--)
        {
            DateTime time = m_timeList[i];
            var r = getRank(time);
            if (r != null)
            {
                Table t = new Table();
                TableRank tr = new TableRank();
                tr.genTable(user, t, OpRes.opres_success, r);
                ret.Add(time.ToShortDateString(), ItemHelp.genHTML(t));
            }
        }
        str = ItemHelp.genJsonStr(ret);
        return str;
    }
}

// 金币增长历史排行
public class QueryCoinGrowthRank : QueryBase
{
    static string[] s_tableName = { "rankGold", "rankGem", "rankDragonBall", "rankChip" };
    ResultRank m_result = new ResultRank();

    public override OpRes doQuery(object param, GMUser user)
    {
        ParamQuery p = (ParamQuery)param;

        DateTime mint = DateTime.Now, maxt = DateTime.Now;
        bool res = Tool.splitTimeStr(p.m_time, ref mint, ref maxt);
        if (!res)
            return OpRes.op_res_time_format_error;

        int rankId = 0; // 排行表格
        if (!int.TryParse(p.m_param, out rankId))
        {
            return OpRes.op_res_param_not_valid;
        }
        
        m_result.reset();
        string fieldName = (p.m_way == QueryWay.by_way0) ? "growth" : "netGrowth";

        while (mint < maxt)
        {
            query(user, s_tableName[rankId], fieldName, mint);
            mint = mint.AddDays(1);
        }
        return OpRes.opres_success;
    }

    public override object getQueryResult()
    {
        return m_result;
    }

    private OpRes query(GMUser user, string tableName, string fieldName, DateTime time)
    {
        IMongoQuery imq = Query.EQ("genTime", time);
        List<Dictionary<string, object>> dataList = DBMgr.getInstance().executeQuery(tableName,
            user.getDbServerID(), DbName.DB_PUMP, imq, 0, 50, null, fieldName, false);
        if (dataList == null || dataList.Count == 0)
            return OpRes.opres_success;

        m_result.m_timeList.Add(time);
        for (int i = 0; i < dataList.Count; i++)
        {
            ResultRankItem info = new ResultRankItem();
            m_result.add(time, info);

            info.m_playerId = Convert.ToInt32(dataList[i]["playerId"]);
            info.m_value = Convert.ToInt64(dataList[i][fieldName]);

            Dictionary<string, object> ret =
                QueryBase.getPlayerProperty(info.m_playerId, user, new string[] { "logout_time", "recharged" });
            if (ret != null)
            {
                if (ret.ContainsKey("logout_time"))
                {
                    info.m_lastLogin = Convert.ToDateTime(ret["logout_time"]).ToLocalTime();
                }
                if (ret.ContainsKey("recharged"))
                {
                    info.m_rechargeTotal = Convert.ToInt32(ret["recharged"]);
                }
            }
        }

        return OpRes.opres_success;
    }
}

//////////////////////////////////////////////////////////////////////////

public class ResultAccountCoinLessValue
{
    // 总数量
    public int m_totalCount;

    // 小于指定数值的数量
    public int m_condCount;

    public void reset()
    {
        m_totalCount = 0;
        m_condCount = 0;
    }
}

// 查询账号金币数量少于指定的值
public class QueryAccountCoinLessValue : QueryBase
{
    private ResultAccountCoinLessValue m_result = new ResultAccountCoinLessValue();

    public override OpRes doQuery(object param, GMUser user)
    {
        ParamQuery p = (ParamQuery)param;
        if (p.m_time == "")
            return OpRes.op_res_time_format_error;

        int val = 0;
        if (!int.TryParse(p.m_param, out val))
            return OpRes.op_res_param_not_valid;

        DateTime mint = DateTime.Now, maxt = DateTime.Now;
        bool res = Tool.splitTimeStr(p.m_time, ref mint, ref maxt);
        if (!res)
            return OpRes.op_res_time_format_error;

        m_result.reset();

        List<IMongoQuery> queryList = new List<IMongoQuery>();

        IMongoQuery imq1 = Query.LT("create_time", BsonValue.Create(maxt));
        IMongoQuery imq2 = Query.GTE("create_time", BsonValue.Create(mint));
        IMongoQuery imqTime = Query.And(imq1, imq2);
        queryList.Add(imqTime);
        queryList.Add(Query.LT("gold", BsonValue.Create(val)));

        IMongoQuery imqGold = Query.And(queryList);

        return query(user, imqTime, imqGold);
    }

    public override object getQueryResult()
    {
        return m_result;
    }

    private OpRes query(GMUser user, IMongoQuery imqTime, IMongoQuery imqGold)
    {
        m_result.m_totalCount = (int)DBMgr.getInstance().getRecordCount(TableName.PLAYER_INFO, imqTime, user.getDbServerID(), DbName.DB_PLAYER);
        m_result.m_condCount = (int)DBMgr.getInstance().getRecordCount(TableName.PLAYER_INFO, imqGold, user.getDbServerID(), DbName.DB_PLAYER);
        return OpRes.opres_success;
    }
}

//////////////////////////////////////////////////////////////////////////
public class CFishItem
{
    public int m_itemId;    // 道具ID
    public long m_buyCount;  // 购买个数(购买后即使用)
    public long m_useCount;  // 使用个数(不花钱的使用次数)

    public string getItemName()
    {
        Fish_ItemCFGData data = Fish_ItemCFG.getInstance().getValue(m_itemId);

        if (data != null)
            return data.m_itemName;

        return "";
    }
}

public class RooomItemConsume
{
    public Dictionary<int, CFishItem> m_dic = new Dictionary<int, CFishItem>();

    public void addItem(int itemId, int moneyType, long value)
    {
        CFishItem tmp = null;
        if (m_dic.ContainsKey(itemId))
        {
            tmp = m_dic[itemId];
        }
        else
        {
            tmp = new CFishItem();
            tmp.m_itemId = itemId;
            m_dic.Add(itemId, tmp);
        }

        if (moneyType > 0) // 这是通过钻石购买的
        {
            tmp.m_buyCount += value;
        }
        else
        {
            tmp.m_useCount += value; // 直接使用的
        }
    }

    public void endOp()
    {
        foreach (var d in m_dic.Values)
        {
            d.m_useCount -= d.m_buyCount;
        }
    }

    public CFishItem getIem(int itemId)
    {
        if (m_dic.ContainsKey(itemId))
            return m_dic[itemId];

        return null;
    }
}

public class ResultConsumeItem
{
    public Dictionary<string, RooomItemConsume> m_dic = new Dictionary<string, RooomItemConsume>();
    private List<DateTime> m_list = new List<DateTime>();

    public void addItem(DateTime time, int roomId, int itemId, int moneyType, long value)
    {
        string key = time.ToString() + "_" + roomId;
        RooomItemConsume ric = null;
        if (m_dic.ContainsKey(key))
        {
            ric = m_dic[key];
        }
        else
        {
            ric = new RooomItemConsume();
            m_dic.Add(key, ric);
        }

        ric.addItem(itemId, moneyType, value);

        if (!m_list.Contains(time))
        {
            m_list.Add(time);
        }
    }

    public void endOp()
    {
        foreach (var d in m_dic.Values)
        {
            d.endOp();
        }
    }

    public void reset()
    {
        m_dic.Clear();
        m_list.Clear();
    }

    public RooomItemConsume getRooomItemConsume(DateTime t, int roomId)
    {
        string key = t.ToString() + "_" + roomId;
        if (m_dic.ContainsKey(key))
            return m_dic[key];

        return null;
    }

    public List<DateTime> timeList
    {
        get { return m_list; }
    }
}

// 经典捕鱼的消耗查询，只查消耗
public class QueryFishConsume : QueryBase
{
    private ResultTotalConsume m_result = new ResultTotalConsume();
    private ResultConsumeItem m_result1 = new ResultConsumeItem();

    public override OpRes doQuery(object param, GMUser user)
    {
        m_result.reset();
        ParamTotalConsume p = (ParamTotalConsume)param;

        List<IMongoQuery> queryList = new List<IMongoQuery>();

        DateTime mint = DateTime.Now, maxt = DateTime.Now;
        bool res = Tool.splitTimeStr(p.m_time, ref mint, ref maxt);
        if (!res)
            return OpRes.op_res_time_format_error;

        IMongoQuery imq1 = Query.LT("time", BsonValue.Create(maxt));
        IMongoQuery imq2 = Query.GTE("time", BsonValue.Create(mint));
        queryList.Add(Query.And(imq1, imq2));

        if (p.m_currencyType != 3) 
        {
            queryList.Add(Query.EQ("moneyType", BsonValue.Create(p.m_currencyType)));
        }

        IMongoQuery imq = Query.And(queryList);

        if (p.m_currencyType == 3)
        {
            return queryItem(user, imq);
        }

        return query(user, imq);
    }

    // 返回查询结果
    public override object getQueryResult()
    {
        return m_result;
    }

    public override object getQueryResult(object param, GMUser user)
    {
        return m_result1;
    }

    private OpRes query(GMUser user, IMongoQuery imq)
    {
        List<Dictionary<string, object>> data =
             DBMgr.getInstance().executeQuery(TableName.FISH_CONSUME, user.getDbServerID(),
             DbName.DB_PUMP, imq, 0, 0, null, "time", true);

        if (data == null || data.Count == 0)
        {
            return OpRes.op_res_not_found_data;
        }

        int i = 0;
        for (i = 0; i < data.Count; i++)
        {
            DateTime t = Convert.ToDateTime(data[i]["time"]).ToLocalTime();
            TotalConsumeItem item = m_result.create(t);

            int reason = Convert.ToInt32(data[i]["consumeType"]);
            long val = Convert.ToInt64(data[i]["value"]);
            item.add(reason, val);

            m_result.addReason(reason);
        }

        return OpRes.opres_success;
    }

    // 查询购买消耗的道具。锁定、急速、散射
    private OpRes queryItem(GMUser user, IMongoQuery imq)
    {
        m_result1.reset();

        List<Dictionary<string, object>> data =
             DBMgr.getInstance().executeQuery(TableName.FISH_CONSUME_ITEM, user.getDbServerID(),
             DbName.DB_PUMP, imq, 0, 0, null, "time", true);

        if (data == null || data.Count == 0)
        {
            return OpRes.op_res_not_found_data;
        }

        int i = 0;
        for (i = 0; i < data.Count; i++)
        {
            DateTime t = Convert.ToDateTime(data[i]["time"]).ToLocalTime();

            int itemId = Convert.ToInt32(data[i]["itemId"]);
            int moneyType = Convert.ToInt32(data[i]["moneyType"]);
            int roomId = Convert.ToInt32(data[i]["roomId"]);
            long value = Convert.ToInt32(data[i]["value"]);
            m_result1.addItem(t, roomId, itemId, moneyType, value);
        }

        m_result1.endOp();
        return OpRes.opres_success;
    }
}

//////////////////////////////////////////////////////////////////////////

public class ResultPlayerBankerInfo
{
    public string m_genTime = "";
    public int m_playerId;
    public string m_nickName = "";
    public int m_bankerCount;
    public int m_beforeGold = 0;
    public int m_nowGold;
    public int m_resultValue;
    public int m_sysGet;

    // 爆庄支出
    public int m_sysLose;
}

// 百家乐上庄情况
public class QueryBaccaratPlayerBanker : QueryBase
{
    List<ResultPlayerBankerInfo> m_result = new List<ResultPlayerBankerInfo>();
    private QueryCondition m_cond = new QueryCondition();

    protected string m_tableName = "";

    public QueryBaccaratPlayerBanker()
    {
        m_tableName = TableName.PUMP_PLAYER_BANKER;
    }

    public override OpRes doQuery(object param, GMUser user)
    {                                                                    
        m_result.Clear();
        m_cond.startQuery();
        OpRes res = makeQuery(param, user, m_cond);
        if (res != OpRes.opres_success)
            return res;

        IMongoQuery imq = m_cond.getImq();
        ParamQuery p = (ParamQuery)param;
        return query(p, imq, user);
    }

    // 返回查询结果
    public override object getQueryResult()
    {
        return m_result;
    }

    public override OpRes makeQuery(object param, GMUser user, QueryCondition queryCond)
    {
        ParamQuery p = (ParamQuery)param;
        bool res = false;
        if (!string.IsNullOrEmpty(p.m_param))
        {
            int playerId = 0;
            res = int.TryParse(p.m_param, out playerId);
            if (!res)
                return OpRes.op_res_param_not_valid;

            queryCond.addQueryCond("playerId", playerId);
        }
      
        DateTime mint = DateTime.Now, maxt = DateTime.Now;
        res = Tool.splitTimeStr(p.m_time, ref mint, ref maxt);
        if (!res)
            return OpRes.op_res_time_format_error;

        IMongoQuery imq1 = Query.LT("genTime", BsonValue.Create(maxt));
        IMongoQuery imq2 = Query.GTE("genTime", BsonValue.Create(mint));
        queryCond.addImq(Query.And(imq1, imq2));

        return OpRes.opres_success;
    }

    private OpRes query(ParamQuery param, IMongoQuery imq, GMUser user)
    {
        user.totalRecord = DBMgr.getInstance().getRecordCount(m_tableName, imq, 
            user.getDbServerID(), DbName.DB_PUMP);

        List<Dictionary<string, object>> dataList = DBMgr.getInstance().executeQuery(m_tableName,
                                                     user.getDbServerID(),
                                                     DbName.DB_PUMP,
                                                     imq,
                                                     (param.m_curPage - 1) * param.m_countEachPage,
                                                     param.m_countEachPage);

        if (dataList == null || dataList.Count == 0)
            return OpRes.op_res_not_found_data;

        for (int i = 0; i < dataList.Count; i++)
        {
            ResultPlayerBankerInfo t = new ResultPlayerBankerInfo();
            m_result.Add(t);

            t.m_genTime = Convert.ToDateTime(dataList[i]["genTime"]).ToLocalTime().ToString();
            t.m_playerId = Convert.ToInt32(dataList[i]["playerId"]);
            t.m_nickName = Convert.ToString(dataList[i]["playerName"]);
            t.m_bankerCount = Convert.ToInt32(dataList[i]["bankerCount"]);
            t.m_beforeGold = Convert.ToInt32(dataList[i]["beforeGold"]);
            t.m_nowGold = Convert.ToInt32(dataList[i]["nowGold"]);
            t.m_resultValue = Convert.ToInt32(dataList[i]["resultValue"]);
            t.m_sysGet = Convert.ToInt32(dataList[i]["sysGet"]);

            if (dataList[i].ContainsKey("sysLose"))
            {
                t.m_sysLose = Convert.ToInt32(dataList[i]["sysLose"]);
            }
        }
        
        return OpRes.opres_success;
    }
}

//////////////////////////////////////////////////////////////////////////

// 牛牛上庄查询
public class QueryCowsPlayerBanker : QueryBaccaratPlayerBanker
{
    public QueryCowsPlayerBanker()
    {
        m_tableName = TableName.PUMP_PLAYER_BANKER_COWS;
    }
}

//////////////////////////////////////////////////////////////////////////
public class ResultnformHeadItem
{
    public string m_time;
    public int m_informerId;
    public int m_dstPlayerId;
    public string m_headURL;
}

public class ParamInformHead
{
    public string m_playerList;
    public int m_opType;

    public ParamInformHead()
    {
        m_opType = 0;
    }

    public bool isView()
    {
        return m_opType == 0;
    }
}

// 举报头像查看
public class QueryInformHead : QueryBase
{
    private List<ResultnformHeadItem> m_result = new List<ResultnformHeadItem>();

    public override OpRes doQuery(object param, GMUser user)
    {
        ParamInformHead p = (ParamInformHead)param;
        if (p.isView())
        {
            return _doQuery(user);
        }

        return _doDel(p, user);
    }

    public override object getQueryResult()
    {
        return m_result;
    }

    private OpRes _doQuery(GMUser user)
    {
        m_result.Clear();
        List<Dictionary<string, object>> dataList =
            DBMgr.getInstance().executeQuery(TableName.INFORM_HEAD,
                                             user.getDbServerID(),
                                             DbName.DB_PLAYER, null, 0, 500);
        if (dataList == null)
            return OpRes.opres_success;

        for (int i = 0; i < dataList.Count; i++)
        {
            ResultnformHeadItem info = new ResultnformHeadItem();
            m_result.Add(info);

            Dictionary<string, object> data = dataList[i];
            info.m_time = Convert.ToDateTime(data["time"]).ToLocalTime().ToString();
            info.m_informerId = Convert.ToInt32(data["informerId"]);
            info.m_dstPlayerId = Convert.ToInt32(dataList[i]["destPlayerId"]);

            OpRes code = user.doQuery(info.m_dstPlayerId.ToString(), QueryType.queryTypePlayerHead);
            if (code == OpRes.opres_success)
            {
                info.m_headURL = (string)user.getQueryResult(QueryType.queryTypePlayerHead);
            }
            else
            {
                info.m_headURL = "";
            }
        }

        return OpRes.opres_success;
    }

    private OpRes _doDel(ParamInformHead param, GMUser user)
    {
        if (string.IsNullOrEmpty(param.m_playerList))
            return OpRes.op_res_param_not_valid;

        string[] arr = Tool.split(param.m_playerList, ',', StringSplitOptions.RemoveEmptyEntries);
        foreach (string id in arr)
        {
            DBMgr.getInstance().remove(TableName.INFORM_HEAD, "destPlayerId", Convert.ToInt32(id),
                user.getDbServerID(), DbName.DB_PLAYER);
        }
        return OpRes.opres_success;
    }
}

//////////////////////////////////////////////////////////////////////////
public class ResultActivationItem : GameStatData
{
    public DateTime m_genTime;
    public string m_channel;
    // 微信公众号充值收入
    public int m_wchatPublicNumIncome;

    public string get2DayRemain()
    {
        return getRemainPercent(m_2DayRemainCount, m_regeditCount);
    }

    public string get3DayRemain()
    {
        return getRemainPercent(m_3DayRemainCount, m_regeditCount);
    }
    public string get7DayRemain()
    {
        return getRemainPercent(m_7DayRemainCount, m_regeditCount);
    }

    public string get30DayRemain()
    {
        return getRemainPercent(m_30DayRemainCount, m_regeditCount);
    }

    //////////////////////////////////////////////////////////////////////////
    public string get2DayDevRemain()
    {
        return getRemainPercent(m_2DayDevRemainCount, m_deviceActivationCount);
    }

    public string get3DayDevRemain()
    {
        return getRemainPercent(m_3DayDevRemainCount, m_deviceActivationCount);
    }
    public string get7DayDevRemain()
    {
        return getRemainPercent(m_7DayDevRemainCount, m_deviceActivationCount);
    }

    public string get30DayDevRemain()
    {
        return getRemainPercent(m_30DayDevRemainCount, m_deviceActivationCount);
    }

    public string getARPU()
    {
        if (m_activeCount == 0)
            return "0";

        double val = (double)m_totalIncome / m_activeCount;
        return Math.Round(val, 2).ToString();
    }

    public string getARPPU()
    {
        if (m_rechargePersonNum == 0)
            return "0";

        double val = (double)m_totalIncome / m_rechargePersonNum;
        return Math.Round(val, 2).ToString();
    }

    // 付费率=付费人数/注册人数
    public string getRechargeRate()
    {
        return getRemainPercent(m_rechargePersonNum, m_activeCount);
    }

    // 新增用户付费率
    public string getNewAccRechargeRate()
    {
        if (m_newAccRechargePersonNum == -1)
            return "";

        return getRemainPercent(m_newAccRechargePersonNum, m_regeditCount);
    }

    public string getAccNumberPerDev()
    {
        return ItemHelp.getRate(m_regeditCount, m_deviceActivationCount, 2);
    }

    public void reset()
    {
        m_genTime = DateTime.MinValue;
        m_channel = "";
        m_2DayDevRemainCount = m_3DayDevRemainCount = m_7DayDevRemainCount = m_30DayDevRemainCount = 0;
    }

    private string getRemainPercent(int up, int down)
    {
        if (down == 0)
            return "0%";

        if (up == -1)
        {
            return "暂无";
        }
        double val = (double)up / down * 100;
        return Math.Round(val, 2).ToString() + "%";
    }
}

// talking data活跃
public class QueryTdActivation: QueryBase
{
    private List<ResultActivationItem> m_result = new List<ResultActivationItem>();

    public override OpRes doQuery(object param, GMUser user)
    {
        m_result.Clear();
        ParamQuery p = (ParamQuery)param;

        List<IMongoQuery> queryList = new List<IMongoQuery>();
        int condCount = 0;
        if (p.m_param != "")
        {
            queryList.Add(Query.EQ("channel", BsonValue.Create(p.m_param)));
        }
        if (p.m_time != "")
        {
            DateTime mint = DateTime.Now, maxt = DateTime.Now;
            bool res = Tool.splitTimeStr(p.m_time, ref mint, ref maxt);
            if (!res)
                return OpRes.op_res_time_format_error;

            condCount++;
            IMongoQuery imq1 = Query.LT("genTime", BsonValue.Create(maxt));
            IMongoQuery imq2 = Query.GTE("genTime", BsonValue.Create(mint));
            queryList.Add(Query.And(imq1, imq2));
        }

        if (condCount == 0)
            return OpRes.op_res_need_at_least_one_cond;

        IMongoQuery imq = queryList.Count > 0 ? Query.And(queryList) : null;

        OpRes code = query(p, imq, user);
        if (p.m_param == "" && code == OpRes.opres_success)
        {
            sum();
        }
        return code;
    }

    // 返回查询结果
    public override object getQueryResult()
    {
        return m_result;
    }

    private OpRes query(ParamQuery param, IMongoQuery imq, GMUser user)
    {
        int serverId = DBMgr.getInstance().getSpecialServerId(DbName.DB_ACCOUNT);
        if (serverId == -1)
            return OpRes.op_res_failed;

        user.totalRecord = DBMgr.getInstance().getRecordCount(TableName.CHANNEL_TD, imq, serverId, DbName.DB_ACCOUNT);

        List<Dictionary<string, object>> dataList =
             DBMgr.getInstance().executeQuery(TableName.CHANNEL_TD, serverId, DbName.DB_ACCOUNT, imq,
             0, 0, null, "genTime", false
                                              /*(param.m_curPage - 1) * param.m_countEachPage, param.m_countEachPage*/);

        if (dataList == null || dataList.Count == 0)
        {
            return OpRes.op_res_not_found_data;
        }

        int i = 0;
        for (i = 0; i < dataList.Count; i++)
        {
            Dictionary<string, object> data = dataList[i];
            ResultActivationItem tmp = new ResultActivationItem();
            m_result.Add(tmp);

            tmp.m_genTime = Convert.ToDateTime(data["genTime"]).ToLocalTime();
            tmp.m_channel = Convert.ToString(data["channel"]);
            
            tmp.m_regeditCount = Convert.ToInt32(data["regeditCount"]);
            tmp.m_deviceActivationCount = Convert.ToInt32(data["deviceActivationCount"]);
            tmp.m_activeCount = Convert.ToInt32(data["activeCount"]);
            
            tmp.m_totalIncome = Convert.ToInt32(data["totalIncome"]);
            tmp.m_rechargePersonNum = Convert.ToInt32(data["rechargePersonNum"]);
            tmp.m_rechargeCount = Convert.ToInt32(data["rechargeCount"]);

          //  tmp.m_2DayRegeditCount = Convert.ToInt32(data["2DayRegeditCount"]);
            tmp.m_2DayRemainCount = Convert.ToInt32(data["2DayRemainCount"]);

           // tmp.m_3DayRegeditCount = Convert.ToInt32(data["3DayRegeditCount"]);
            tmp.m_3DayRemainCount = Convert.ToInt32(data["3DayRemainCount"]);

            //tmp.m_7DayRegeditCount = Convert.ToInt32(data["7DayRegeditCount"]);
            tmp.m_7DayRemainCount = Convert.ToInt32(data["7DayRemainCount"]);

            //tmp.m_30DayRegeditCount = Convert.ToInt32(data["30DayRegeditCount"]);
            tmp.m_30DayRemainCount = Convert.ToInt32(data["30DayRemainCount"]);

            if (data.ContainsKey("Day2DevRemainCount"))
            {
                tmp.m_2DayDevRemainCount = Convert.ToInt32(data["Day2DevRemainCount"]);
            }
            if (data.ContainsKey("Day3DevRemainCount"))
            {
                tmp.m_3DayDevRemainCount = Convert.ToInt32(data["Day3DevRemainCount"]);
            }
            if (data.ContainsKey("Day7DevRemainCount"))
            {
                tmp.m_7DayDevRemainCount = Convert.ToInt32(data["Day7DevRemainCount"]);
            }
            if (data.ContainsKey("Day30DevRemainCount"))
            {
                tmp.m_30DayDevRemainCount = Convert.ToInt32(data["Day30DevRemainCount"]);
            }

            if (data.ContainsKey("newAccIncome"))
            {
                tmp.m_newAccIncome = Convert.ToInt32(data["newAccIncome"]);
            }
            else
            {
                tmp.m_newAccIncome = -1;
            }
            if (data.ContainsKey("newAccRechargePersonNum"))
            {
                tmp.m_newAccRechargePersonNum = Convert.ToInt32(data["newAccRechargePersonNum"]);
            }
            else
            {
                tmp.m_newAccRechargePersonNum = -1;
            }

            IMongoQuery q1 = Query.EQ("genTime", tmp.m_genTime);
            IMongoQuery q2 = Query.EQ("channel", tmp.m_channel);
            IMongoQuery sq = Query.And(q1, q2);
            Dictionary<string, object> payTypeData =
                DBMgr.getInstance().getTableData(TableName.CHANNEL_TD_PAY, serverId, DbName.DB_ACCOUNT, sq);
            if (payTypeData != null)
            {
                if (payTypeData.ContainsKey("1_rmb"))
                {
                    tmp.m_wchatPublicNumIncome = Convert.ToInt32(payTypeData["1_rmb"]);
                }
            }
        }
        return OpRes.opres_success;
    }

    // 选择全部时，进行总和计算
    void sum()
    {
        List<ResultActivationItem> dataList = new List<ResultActivationItem>();

        foreach (var item in m_result)
        {
            ResultActivationItem res = findSameResult(dataList, item.m_genTime);
            res.m_regeditCount += item.m_regeditCount;
            res.m_deviceActivationCount += item.m_deviceActivationCount;
            res.m_activeCount += item.m_activeCount;
            res.m_totalIncome += item.m_totalIncome;
            res.m_rechargePersonNum += item.m_rechargePersonNum;
            res.m_rechargeCount += item.m_rechargeCount;
            res.m_newAccIncome += item.m_newAccIncome;
            res.m_newAccRechargePersonNum += item.m_newAccRechargePersonNum;
            if (item.m_2DayRemainCount > 0)
            {
                res.m_2DayRemainCount += item.m_2DayRemainCount;
            }
            if (item.m_3DayRemainCount > 0)
            {
                res.m_3DayRemainCount += item.m_3DayRemainCount;
            }
            if (item.m_7DayRemainCount > 0)
            {
                res.m_7DayRemainCount += item.m_7DayRemainCount;
            }
            if (item.m_30DayRemainCount > 0)
            {
                res.m_30DayRemainCount += item.m_30DayRemainCount;
            }

            if (item.m_2DayDevRemainCount > 0)
            {
                res.m_2DayDevRemainCount += item.m_2DayDevRemainCount;
            }
            if (item.m_3DayDevRemainCount > 0)
            {
                res.m_3DayDevRemainCount += item.m_3DayDevRemainCount;
            }
            if (item.m_7DayDevRemainCount > 0)
            {
                res.m_7DayDevRemainCount += item.m_7DayDevRemainCount;
            }
            if (item.m_30DayDevRemainCount > 0)
            {
                res.m_30DayDevRemainCount += item.m_30DayDevRemainCount;
            }

            res.m_wchatPublicNumIncome += item.m_wchatPublicNumIncome;
        }

        m_result = dataList;
    }

    ResultActivationItem findSameResult(List<ResultActivationItem> data, DateTime time)
    {
        ResultActivationItem res = null;
        foreach (var d in data)
        {
            if (time == d.m_genTime)
            {
                res = d;
                break;
            }
        }
        if (res == null)
        {
            res = new ResultActivationItem();
            res.reset();
            res.m_genTime = time;
            data.Add(res);
        }
        return res;
    }
}

//////////////////////////////////////////////////////////////////////////
public class ResultMaxOnlineItem
{
    public string m_date;
    public string m_timePoint;
    public int m_playerNum;
}

public class QueryMaxOnline : QueryBase
{
    private List<ResultMaxOnlineItem> m_result = new List<ResultMaxOnlineItem>();

    public override OpRes doQuery(object param, GMUser user)
    {
        m_result.Clear();
        ParamQuery p = (ParamQuery)param;

        List<IMongoQuery> queryList = new List<IMongoQuery>();
        if (p.m_time != "")
        {
            DateTime mint = DateTime.Now, maxt = DateTime.Now;
            bool res = Tool.splitTimeStr(p.m_time, ref mint, ref maxt);
            if (!res)
                return OpRes.op_res_time_format_error;

            IMongoQuery imq1 = Query.LT("date", BsonValue.Create(maxt));
            IMongoQuery imq2 = Query.GTE("date", BsonValue.Create(mint));
            queryList.Add(Query.And(imq1, imq2));
        }

        IMongoQuery imq = queryList.Count > 0 ? Query.And(queryList) : null;

        return query(p, imq, user);
    }

    // 返回查询结果
    public override object getQueryResult()
    {
        return m_result;
    }

    private OpRes query(ParamQuery param, IMongoQuery imq, GMUser user)
    {
        user.totalRecord = DBMgr.getInstance().getRecordCount(TableName.PUMP_MAXONLINE_PLAYER, imq, user.getDbServerID(), DbName.DB_PUMP);

        List<Dictionary<string, object>> dataList =
             DBMgr.getInstance().executeQuery(TableName.PUMP_MAXONLINE_PLAYER, user.getDbServerID(), DbName.DB_PUMP, imq
            /*(param.m_curPage - 1) * param.m_countEachPage, param.m_countEachPage*/);

        if (dataList == null || dataList.Count == 0)
        {
            return OpRes.op_res_not_found_data;
        }

        int i = 0;
        for (i = 0; i < dataList.Count; i++)
        {
            Dictionary<string, object> data = dataList[i];
            ResultMaxOnlineItem tmp = new ResultMaxOnlineItem();
            m_result.Add(tmp);

            tmp.m_date = Convert.ToDateTime(data["date"]).ToLocalTime().ToShortDateString();
            tmp.m_timePoint = Convert.ToDateTime(data["maxTimePoint"]).ToLocalTime().ToString();
            tmp.m_playerNum = Convert.ToInt32(data["playerNum"]);
        }
        return OpRes.opres_success;
    }
}

//////////////////////////////////////////////////////////////////////////
public class ResultTotalPlayerMoneyItem
{
    public string m_date;
    public long m_money;
    public long m_safeBox;
}

public class QueryTotalPlayerMoney : QueryBase
{
    private List<ResultTotalPlayerMoneyItem> m_result = new List<ResultTotalPlayerMoneyItem>();

    public override OpRes doQuery(object param, GMUser user)
    {
        m_result.Clear();
        ParamQuery p = (ParamQuery)param;
        if (string.IsNullOrEmpty(p.m_time))
            return OpRes.op_res_time_format_error;

        List<IMongoQuery> queryList = new List<IMongoQuery>();

        if (p.m_time != "")
        {
            DateTime mint = DateTime.Now, maxt = DateTime.Now;
            bool res = Tool.splitTimeStr(p.m_time, ref mint, ref maxt);
            if (!res)
                return OpRes.op_res_time_format_error;

            IMongoQuery imq1 = Query.LT("genTime", BsonValue.Create(maxt));
            IMongoQuery imq2 = Query.GTE("genTime", BsonValue.Create(mint));
            queryList.Add(Query.And(imq1, imq2));
        }

        IMongoQuery imq = queryList.Count > 0 ? Query.And(queryList) : null;

        return query(p, imq, user);
    }

    // 返回查询结果
    public override object getQueryResult()
    {
        return m_result;
    }

    private OpRes query(ParamQuery param, IMongoQuery imq, GMUser user)
    {
        user.totalRecord = DBMgr.getInstance().getRecordCount(TableName.PUMP_PLAYER_TOTAL_MONEY, 
            imq, 
            user.getDbServerID(), 
            DbName.DB_PUMP);

        List<Dictionary<string, object>> dataList =
             DBMgr.getInstance().executeQuery(TableName.PUMP_PLAYER_TOTAL_MONEY, user.getDbServerID(), DbName.DB_PUMP, imq,
             0, 0, null, "genTime",false
            /*(param.m_curPage - 1) * param.m_countEachPage, param.m_countEachPage*/);

        if (dataList == null || dataList.Count == 0)
        {
            return OpRes.op_res_not_found_data;
        }

        int i = 0;
        for (i = 0; i < dataList.Count; i++)
        {
            Dictionary<string, object> data = dataList[i];
            ResultTotalPlayerMoneyItem tmp = new ResultTotalPlayerMoneyItem();
            m_result.Add(tmp);

            tmp.m_date = Convert.ToDateTime(data["genTime"]).ToLocalTime().ToShortDateString();
            tmp.m_money = Convert.ToInt64(data["money"]);
            if (data.ContainsKey("safeBox"))
            {
                tmp.m_safeBox = Convert.ToInt64(data["safeBox"]);
            }
            else
            {
                tmp.m_safeBox = -1;
            }
        }
        return OpRes.opres_success;
    }
}

//////////////////////////////////////////////////////////////////////////
public class ResultLTVItem : GameStatData
{
    public string m_genTime;
    public string m_channel;

    public string get1DayAveRecharge()
    {
        return getAveRecharge(m_1DayTotalRecharge, m_regeditCount);
    }
    public string get3DayAveRecharge()
    {
        return getAveRecharge(m_3DayTotalRecharge, m_regeditCount);
    }
    public string get7DayAveRecharge()
    {
        return getAveRecharge(m_7DayTotalRecharge, m_regeditCount);
    }
    public string get14DayAveRecharge()
    {
        return getAveRecharge(m_14DayTotalRecharge, m_regeditCount);
    }
    public string get30DayAveRecharge()
    {
        return getAveRecharge(m_30DayTotalRecharge, m_regeditCount);
    }
    public string get60DayAveRecharge()
    {
        return getAveRecharge(m_60DayTotalRecharge, m_regeditCount);
    }
    public string get90DayAveRecharge()
    {
        return getAveRecharge(m_90DayTotalRecharge, m_regeditCount);
    }
    private string getAveRecharge(int up, int down)
    {
        if (down == 0)
            return "0";

        if (up == -1)
        {
            return "暂无";
        }
        double val = (double)up / down;
        return Math.Round(val, 2).ToString();
    }
}

// 平均价值
public class QueryTdLTV : QueryBase
{
    private List<ResultLTVItem> m_result = new List<ResultLTVItem>();

    public override OpRes doQuery(object param, GMUser user)
    {
        m_result.Clear();
        ParamQuery p = (ParamQuery)param;

        List<IMongoQuery> queryList = new List<IMongoQuery>();
        int condCount = 0;
        if (p.m_param != "")
        {
            queryList.Add(Query.EQ("channel", BsonValue.Create(p.m_param)));
        }
        if (p.m_time != "")
        {
            DateTime mint = DateTime.Now, maxt = DateTime.Now;
            bool res = Tool.splitTimeStr(p.m_time, ref mint, ref maxt);
            if (!res)
                return OpRes.op_res_time_format_error;

            condCount++;
            IMongoQuery imq1 = Query.LT("genTime", BsonValue.Create(maxt));
            IMongoQuery imq2 = Query.GTE("genTime", BsonValue.Create(mint));
            queryList.Add(Query.And(imq1, imq2));
        }

        if (condCount == 0)
            return OpRes.op_res_need_at_least_one_cond;

        IMongoQuery imq = queryList.Count > 0 ? Query.And(queryList) : null;

        return query(p, imq, user);
    }

    // 返回查询结果
    public override object getQueryResult()
    {
        return m_result;
    }

    private OpRes query(ParamQuery param, IMongoQuery imq, GMUser user)
    {
        int serverId = DBMgr.getInstance().getSpecialServerId(DbName.DB_ACCOUNT);
        if (serverId == -1)
            return OpRes.op_res_failed;

       // user.totalRecord = DBMgr.getInstance().getRecordCount(TableName.CHANNEL_TD, imq, serverId, DbName.DB_ACCOUNT);

        List<Dictionary<string, object>> dataList =
             DBMgr.getInstance().executeQuery(TableName.CHANNEL_TD, serverId, DbName.DB_ACCOUNT, imq,
             0, 0, null, "genTime", false
            /*(param.m_curPage - 1) * param.m_countEachPage, param.m_countEachPage*/);

        if (dataList == null || dataList.Count == 0)
        {
            return OpRes.op_res_not_found_data;
        }

        int i = 0;
        for (i = 0; i < dataList.Count; i++)
        {
            Dictionary<string, object> data = dataList[i];
            if (!data.ContainsKey("Day1TotalRecharge"))
                continue;
            if (!data.ContainsKey("Day3TotalRecharge"))
                continue;
            if (!data.ContainsKey("Day7TotalRecharge"))
                continue;
            if (!data.ContainsKey("Day14TotalRecharge"))
                continue;
            if (!data.ContainsKey("Day30TotalRecharge"))
                continue;
            if (!data.ContainsKey("Day60TotalRecharge"))
                continue;
            if (!data.ContainsKey("Day90TotalRecharge"))
                continue;

            ResultLTVItem tmp = new ResultLTVItem();
            tmp.m_genTime = Convert.ToDateTime(data["genTime"]).ToLocalTime().ToShortDateString();
            tmp.m_channel = Convert.ToString(data["channel"]);

            tmp.m_regeditCount = Convert.ToInt32(data["regeditCount"]);
            tmp.m_1DayTotalRecharge = Convert.ToInt32(data["Day1TotalRecharge"]);
            tmp.m_3DayTotalRecharge = Convert.ToInt32(data["Day3TotalRecharge"]);
            tmp.m_7DayTotalRecharge = Convert.ToInt32(data["Day7TotalRecharge"]);
            tmp.m_14DayTotalRecharge = Convert.ToInt32(data["Day14TotalRecharge"]);
            tmp.m_30DayTotalRecharge = Convert.ToInt32(data["Day30TotalRecharge"]);
            tmp.m_60DayTotalRecharge = Convert.ToInt32(data["Day60TotalRecharge"]);
            tmp.m_90DayTotalRecharge = Convert.ToInt32(data["Day90TotalRecharge"]);

            m_result.Add(tmp);
        }
        return OpRes.opres_success;
    }
}

//////////////////////////////////////////////////////////////////////////
public class ResultExchangeStat : ResultTotalConsume
{
    public string getExchangeName(int id)
    {
        ExchangeData data = ExchangeCfg.getInstance().getValue(id);
        if (data != null)
            return data.m_name;
        return "";
    }
}

// 兑换统计
public class QueryExchangeStat : QueryBase
{
    ResultExchangeStat m_result = new ResultExchangeStat();

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
             DBMgr.getInstance().executeQuery(TableName.PUMP_EXCHANGE, user.getDbServerID(), DbName.DB_PUMP, imq
            /*(param.m_curPage - 1) * param.m_countEachPage, param.m_countEachPage*/);

        if (dataList == null || dataList.Count == 0)
        {
            return OpRes.op_res_not_found_data;
        }

        int i = 0;
        for (i = 0; i < dataList.Count; i++)
        {
            Dictionary<string, object> data = dataList[i];
            DateTime t = Convert.ToDateTime(data["genTime"]).ToLocalTime();
            TotalConsumeItem item = m_result.create(t);

            int reason = Convert.ToInt32(data["chgId"]);
            long val = Convert.ToInt64(data["value"]);
            item.add(reason, val);

            m_result.addReason(reason);
        }

        return OpRes.opres_success;
    }
}

//////////////////////////////////////////////////////////////////////////
public class RechargePointItem
{
    public DateTime m_time;

    // 渠道-->该渠道数据
    public Dictionary<int, TotalConsumeItem> m_dic = new Dictionary<int, TotalConsumeItem>();

    public void add(int channel, int reason, long value, long count)
    {
        TotalConsumeItem item = null;
        if (m_dic.ContainsKey(channel))
        {
            item = m_dic[channel];
        }
        else
        {
            item = new TotalConsumeItem();
            m_dic.Add(channel, item);
        }
        item.add(reason, value, count);
    }
}

public class ResultRechargePointStat
{
    public HashSet<int> m_fields = new HashSet<int>();

    public List<RechargePointItem> m_result = new List<RechargePointItem>();

    public void addReason(int reason)
    {
        m_fields.Add(reason);
    }

    public void reset()
    {
        m_fields.Clear();
        m_result.Clear();
    }

    public RechargePointItem create(DateTime date)
    {
        foreach (var d in m_result)
        {
            if (d.m_time == date)
                return d;
        }

        RechargePointItem item = new RechargePointItem();
        m_result.Add(item);
        item.m_time = date;
        return item;
    }

    public static string getPayName(int payId)
    {
        RechargeCFGData data = RechargeCFG.getInstance().getValue(payId);
        if (data != null)
        {
            return data.m_name;
        }
        return "";
    }
}

// 付费点统计
public class QueryRechargePointStat : QueryBase
{
    ResultRechargePointStat m_result = new ResultRechargePointStat();

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

        if (!string.IsNullOrEmpty(p.m_param))
        {
            int channelId;
            if(!int.TryParse(p.m_param, out channelId))
                return OpRes.op_res_param_not_valid;

            imq = Query.And(imq, Query.EQ("channel", channelId));
        }
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
             DBMgr.getInstance().executeQuery(TableName.PUMP_RECHARGE, user.getDbServerID(), DbName.DB_PUMP, imq,
             0, 0, null, "genTime",false
            /*(param.m_curPage - 1) * param.m_countEachPage, param.m_countEachPage*/);

        if (dataList == null || dataList.Count == 0)
        {
            return OpRes.op_res_not_found_data;
        }

        int i = 0;
        for (i = 0; i < dataList.Count; i++)
        {
            Dictionary<string, object> data = dataList[i];
            DateTime t = Convert.ToDateTime(data["genTime"]).ToLocalTime();
            RechargePointItem item = m_result.create(t);

            int reason = Convert.ToInt32(data["payId"]);
            long val = Convert.ToInt64(data["value"]);
            long count = Convert.ToInt64(data["count"]);
            int channel = Convert.ToInt32(data["channel"]);
            item.add(channel, reason, val, count);

            m_result.addReason(reason);
        }

        return OpRes.opres_success;
    }
}

//////////////////////////////////////////////////////////////////////////
public class ResultStartLottery : ResultTotalConsume
{
    public string getLevelName(int r)
    {
        return "等级" + r.ToString();
    }
}

// 星星抽奖查询
public class QueryStarLottery : QueryBase
{
    private ResultStartLottery m_result = new ResultStartLottery();

    public override OpRes doQuery(object param, GMUser user)
    {
        m_result.reset();
        ParamQuery p = (ParamQuery)param;

        DateTime mint = DateTime.Now, maxt = DateTime.Now;
        bool res = Tool.splitTimeStr(p.m_time, ref mint, ref maxt);
        if (!res)
            return OpRes.op_res_time_format_error;

        IMongoQuery imq1 = Query.LT("genTime", BsonValue.Create(maxt));
        IMongoQuery imq2 = Query.GTE("genTime", BsonValue.Create(mint));

        IMongoQuery imq = Query.And(imq1, imq2);

        return query(user, imq);
    }

    // 返回查询结果
    public override object getQueryResult()
    {
        return m_result;
    }

    private OpRes query(GMUser user, IMongoQuery imq)
    {
        List<Dictionary<string, object>> data =
             DBMgr.getInstance().executeQuery(TableName.PUMP_STAR_LOTTERY, user.getDbServerID(),
             DbName.DB_PUMP, imq);

        if (data == null || data.Count == 0)
        {
            return OpRes.op_res_not_found_data;
        }

        int i = 0;
        for (i = 0; i < data.Count; i++)
        {
            DateTime t = Convert.ToDateTime(data[i]["genTime"]).ToLocalTime();
            TotalConsumeItem item = m_result.create(t);

            int reason = Convert.ToInt32(data[i]["level"]);
            long val = Convert.ToInt64(data[i]["value"]);
            item.add(reason, val);

            m_result.addReason(reason);
        }

        return OpRes.opres_success;
    }
}

//////////////////////////////////////////////////////////////////////////
public class RLoseItem
{
    public int m_playerId;
    public string m_nickName;
    public int m_vipLevel;
    public int m_gold;
    public int m_gem;
    public int m_dragonBall;
    public string m_lastLoginTime;
}

public class QueryRLose : QueryBase
{
    private List<RLoseItem> m_result = new List<RLoseItem>();

    public override OpRes doQuery(object param, GMUser user)
    {
        m_result.Clear();
        return query(user);
    }

    // 返回查询结果
    public override object getQueryResult()
    {
        return m_result;
    }

    private OpRes query(GMUser user)
    {
        List<Dictionary<string, object>> dataList =
             DBMgr.getInstance().executeQuery(TableName.RLOSE, user.getDbServerID(),
             DbName.DB_PLAYER);

        if (dataList == null || dataList.Count == 0)
        {
            return OpRes.op_res_not_found_data;
        }

        int i = 0;
        string[] fields = { "logout_time" };
        for (i = 0; i < dataList.Count; i++)
        {
            RLoseItem tmp = new RLoseItem();
            m_result.Add(tmp);

            Dictionary<string, object> data = dataList[i];
            tmp.m_playerId = Convert.ToInt32(data["playerId"]);
            tmp.m_nickName = Convert.ToString(data["nickName"]);
            tmp.m_vipLevel = Convert.ToInt32(data["vipLevel"]);
            tmp.m_gold = Convert.ToInt32(data["gold"]);
            tmp.m_gem = Convert.ToInt32(data["gem"]);
            tmp.m_dragonBall = Convert.ToInt32(data["dragonBall"]);

            Dictionary<string, object> pd = QueryBase.getPlayerProperty(tmp.m_playerId, user, fields);
            if (pd != null)
            {
                tmp.m_lastLoginTime = Convert.ToDateTime(pd["logout_time"]).ToLocalTime().ToString();
            }
        }

        return OpRes.opres_success;
    }
}

//////////////////////////////////////////////////////////////////////////
public class ParamDragonBallDaily : ParamQuery
{
    public string m_discount;
    public string m_eachValue;
}

public class DragonBallDailyItem : StatDragonDailyItem
{
    public double m_rmb;
}

// 每日龙珠
public class QueryDragonBallDaily : QueryBase
{
    private List<DragonBallDailyItem> m_result = new List<DragonBallDailyItem>();
    QueryCondition m_cond = new QueryCondition();
    double m_discount = 1, m_eachValue = 1;

    public override OpRes doQuery(object param, GMUser user)
    {
        m_result.Clear();
        m_cond.startQuery();
        OpRes res = makeQuery(param, user, m_cond);
        if (res != OpRes.opres_success)
            return res;

        return query(user, m_discount, m_eachValue, m_cond.getImq());
    }

    // 返回查询结果
    public override object getQueryResult()
    {
        return m_result;
    }

    public override OpRes makeQuery(object param, GMUser user, QueryCondition cond)
    {
        ParamDragonBallDaily p = (ParamDragonBallDaily)param;
        DateTime mint = DateTime.Now, maxt = DateTime.Now;
        bool res = Tool.splitTimeStr(p.m_time, ref mint, ref maxt);
        if (!res)
            return OpRes.op_res_time_format_error;

        if (cond.isExport())
        {
            cond.addQueryCond("time", p.m_time);
        }
        else
        {
            IMongoQuery imq1 = Query.LT("genTime", BsonValue.Create(maxt));
            IMongoQuery imq2 = Query.GTE("genTime", BsonValue.Create(mint));
            cond.addImq(Query.And(imq1, imq2));
        }

        if (!string.IsNullOrEmpty(p.m_discount))
        {
            if (!double.TryParse(p.m_discount, out m_discount))
            {
                return OpRes.op_res_param_not_valid;
            }

            if (cond.isExport())
            {
                cond.addQueryCond("discount", p.m_discount);
            }
        }
        else
        {
            m_discount = 1;
        }
        if (!string.IsNullOrEmpty(p.m_eachValue))
        {
            if (!double.TryParse(p.m_eachValue, out m_eachValue))
            {
                return OpRes.op_res_param_not_valid;
            }

            if (cond.isExport())
            {
                cond.addQueryCond("eachValue", p.m_eachValue);
            }
        }
        else
        {
            m_eachValue = 1;
        }

        return OpRes.opres_success;
    }

    private OpRes query(GMUser user, double discount, double eachValue, IMongoQuery imq)
    {
        List<Dictionary<string, object>> dataList =
             DBMgr.getInstance().executeQuery(TableName.STAT_DRAGON_DAILY, user.getDbServerID(),
             DbName.DB_PUMP, imq);

        if (dataList == null || dataList.Count == 0)
        {
            return OpRes.op_res_not_found_data;
        }

        int i = 0;
        for (i = 0; i < dataList.Count; i++)
        {
            DragonBallDailyItem tmp = new DragonBallDailyItem();
            m_result.Add(tmp);

            Dictionary<string, object> data = dataList[i];
            tmp.m_time = Convert.ToDateTime(data["genTime"]).ToLocalTime();
            tmp.m_todayRecharge = Convert.ToInt32(data["todayRecharge"]);
            tmp.m_dragonBallGen = Convert.ToInt64(data["dragonBallGen"]);
            tmp.m_dragonBallConsume = Convert.ToInt64(data["dragonBallConsume"]);
            tmp.m_dragonBallRemain = Convert.ToInt64(data["dragonBallRemain"]);
            tmp.m_rmb = tmp.m_todayRecharge * discount - (tmp.m_dragonBallGen - tmp.m_dragonBallConsume) * eachValue;
        }

        return OpRes.opres_success;
    }
}

//////////////////////////////////////////////////////////////////////////
public class RechargePlayerMonitorItem : RechargePlayerMonitorItemBase
{
    public string getRechargePoint(int payId)
    {
        return ResultRechargePointStat.getPayName(payId);
    }

    public string getOpenRate(int level)
    {
        Fish_LevelCFGData data = Fish_LevelCFG.getInstance().getValue(level);
        if (data != null)
        {
            return data.m_openRate.ToString();
        }

        return level.ToString();
    }
}

// 付费玩家监控
public class QueryRechargePlayerMonitor : QueryBase
{
    static string[] FIELD_FISH_LEVEL = { "Level" };
    static string[] FIELD_PLAYER = { "recharged", "dragonBall", "GainDragonBallCount", "SendDragonBallCount", "create_time" };
    QueryCondition m_cond = new QueryCondition();
    private List<RechargePlayerMonitorItem> m_result = new List<RechargePlayerMonitorItem>();

    public override OpRes doQuery(object param, GMUser user)
    {
        m_result.Clear();
        m_cond.startQuery();
        OpRes res = makeQuery(param, user, m_cond);
        if (res != OpRes.opres_success)
            return res;

        IMongoQuery imq = m_cond.getImq();

        ParamQuery p = (ParamQuery)param;
        return query(user, imq, p);
    }

    // 返回查询结果
    public override object getQueryResult()
    {
        return m_result;
    }

    public override OpRes makeQuery(object param, GMUser user, QueryCondition cond)
    {
        ParamQuery p = (ParamQuery)param;
        DateTime mint = DateTime.Now, maxt = DateTime.Now;
        bool res = Tool.splitTimeStr(p.m_time, ref mint, ref maxt);
        if (!res)
            return OpRes.op_res_time_format_error;

        if (cond.isExport())
        {
            cond.addQueryCond("time", p.m_time);
        }
        else
        {
            IMongoQuery imq1 = Query.LT("regTime", BsonValue.Create(maxt));
            IMongoQuery imq2 = Query.GTE("regTime", BsonValue.Create(mint));
            cond.addImq(Query.And(imq1, imq2));
        }

        return OpRes.opres_success; 
    }

    private OpRes query(GMUser user, IMongoQuery imq, ParamQuery param)
    {
        user.totalRecord = DBMgr.getInstance().getRecordCount(TableName.PUMP_RECHARGE_FIRST,
            imq,
            user.getDbServerID(),
            DbName.DB_PUMP);

        List<Dictionary<string, object>> dataList =
             DBMgr.getInstance().executeQuery(TableName.PUMP_RECHARGE_FIRST, user.getDbServerID(),
             DbName.DB_PUMP, imq,
             (param.m_curPage - 1) * param.m_countEachPage, param.m_countEachPage, null, "firstRechargeTime", false);

        if (dataList == null || dataList.Count == 0)
        {
            return OpRes.op_res_not_found_data;
        }

        int i = 0;
        for (i = 0; i < dataList.Count; i++)
        {
            RechargePlayerMonitorItem tmp = new RechargePlayerMonitorItem();
            m_result.Add(tmp);

            Dictionary<string, object> data = dataList[i];
            tmp.m_playerId = Convert.ToInt32(data["playerId"]);
            tmp.m_curFishLevel = getFishLevel(tmp.m_playerId, user);

            tmp.m_totalGameTime = getTotalGameTime(tmp.m_playerId, user);

            if (data.ContainsKey("firstRechargeTime"))
            {
                tmp.m_firstRechargeTime = Convert.ToDateTime(data["firstRechargeTime"]).ToLocalTime();
            }
            if (data.ContainsKey("firstRechargeGameTime"))
            {
                tmp.m_firstRechargeGameTime = Convert.ToInt32(data["firstRechargeGameTime"]);
            }
            if (data.ContainsKey("firstRechargePoint"))
            {
                tmp.m_firstRechargePoint = Convert.ToInt32(data["firstRechargePoint"]);
            }
            if (data.ContainsKey("firstRechargeGold"))
            {
                tmp.m_firstRechargeGold = Convert.ToInt32(data["firstRechargeGold"]);
            }
            if (data.ContainsKey("firstRechargeFishLevel"))
            {
                tmp.m_firstRechargeFishLevel = Convert.ToInt32(data["firstRechargeFishLevel"]);
            }
            if (data.ContainsKey("secondRechargeTime"))
            {
                tmp.m_secondRechargeTime = Convert.ToDateTime(data["secondRechargeTime"]).ToLocalTime();
            }
            if (data.ContainsKey("secondRechargeGameTime"))
            {
                tmp.m_secondRechargeGameTime = Convert.ToInt32(data["secondRechargeGameTime"]);
            }
            if (data.ContainsKey("secondRechargePoint"))
            {
                tmp.m_secondRechargePoint = Convert.ToInt32(data["secondRechargePoint"]);
            }
            if (data.ContainsKey("secondRechargeGold"))
            {
                tmp.m_secondRechargeGold = Convert.ToInt32(data["secondRechargeGold"]);
            }
            if (data.ContainsKey("secondRechargeFishLevel"))
            {
                tmp.m_secondRechargeFishLevel = Convert.ToInt32(data["secondRechargeFishLevel"]);
            }

            setOhterInfo(tmp.m_playerId, user, tmp);
        }

        return OpRes.opres_success;
    }

    int getFishLevel(int playerId, GMUser user)
    {
        Dictionary<string, object> data = DBMgr.getInstance().getTableData(TableName.FISHLORD_PLAYER,
            "player_id", playerId,
            FIELD_FISH_LEVEL,
            user.getDbServerID(),
            DbName.DB_GAME);

        if (data != null)
        {
            return Convert.ToInt32(data["Level"]);
        }

        return 0;
    }

    int getTotalGameTime(int playerId, GMUser user)
    {
        Dictionary<string, object> data = DBMgr.getInstance().getTableData(TableName.STAT_PLAYER_GAME_TIME,
            "playerId", playerId,
            null,
            user.getDbServerID(),
            DbName.DB_PLAYER);

        if (data != null)
        {
            return Convert.ToInt32(data["totalGameTime"]);
        }

        return 0;
    }

    void setOhterInfo(int playerId, GMUser user, RechargePlayerMonitorItem item)
    {
        Dictionary<string, object> data = QueryBase.getPlayerProperty(playerId, user, FIELD_PLAYER);
        if (data != null)
        {
            if (data.ContainsKey("recharged"))
            {
                item.m_totalRecharge = Convert.ToInt32(data["recharged"]);
            }
            if (data.ContainsKey("dragonBall"))
            {
                item.m_remainDragon = Convert.ToInt32(data["dragonBall"]);
            }
            if (data.ContainsKey("GainDragonBallCount"))
            {
                item.m_gainDragon = Convert.ToInt64(data["GainDragonBallCount"]);
            }
            if (data.ContainsKey("SendDragonBallCount"))
            {
                item.m_sendDragon = Convert.ToInt64(data["SendDragonBallCount"]);
            }
            if (data.ContainsKey("create_time"))
            {
                item.m_regTime = Convert.ToDateTime(data["create_time"]).ToLocalTime();
            }
        }
    }
}

//////////////////////////////////////////////////////////////////////////
public class DataPerHour<T> where T : struct 
{
    public DateTime m_time;
    public T[] m_data = new T[24];

    // 最小值所在索引
    public int m_minIndex;
    // 最大值所在索引
    public int m_maxIndex;

    public void init()
    {
        for (int i = 0; i < m_data.Length; i++)
        {
            m_data[i] = default(T);
        }
    }

    // index 0-23范围
    public void addData(int index, T d)
    {
        m_data[index] = d;
    }

    public T getData(int index)
    {
        return m_data[index];
    }

    public int getCount() { return m_data.Length; }

    public void calMinMax(Comparison<T> comparison)
    {
        m_minIndex = m_maxIndex = 0;
        for (int i = 1; i < m_data.Length; i++)
        {
            if (comparison(m_data[i], m_data[m_maxIndex]) > 0)
            {
                m_maxIndex = i;
            }
            if (comparison(m_data[i], m_data[m_minIndex]) < 0)
            {
                m_minIndex = i;
            }
        }
    }
}

public class DataEachDay
{
    private List<DataPerHour<int>> m_data = new List<DataPerHour<int>>();

    public void addData(DateTime t, int h, int val)
    {
        DataPerHour<int> dph = null;
        for (int i = 0; i < m_data.Count; i++)
        {
            if (m_data[i].m_time == t)
            {
                dph = m_data[i];
                break;
            }
        }

        if (dph == null)
        {
            dph = new DataPerHour<int>();
            dph.m_time = t;
            dph.init();
            m_data.Add(dph);
        }

        dph.addData(h, val);
    }

    public void reset()
    {
        m_data.Clear();
    }

    public List<DataPerHour<int>> getData()
    {
        return m_data;
    }

    public void calMaxMin()
    {
        foreach (var d in m_data)
        {
            //calMinMax(d);
            d.calMinMax((a, b) => { return a - b; });
        }
    }

    public string average(DataPerHour<int> data)
    {
        long sum = 0;
        for (int i = 0; i < data.getCount(); i++)
        {
            sum += data.getData(i);
        }
        return ItemHelp.getRate(sum, data.getCount(), 2);
    }

    void calMinMax(DataPerHour<int> data)
    {
        data.m_minIndex = data.m_maxIndex = 0;
        for (int i = 1; i < data.m_data.Length; i++)
        {
            if (data.m_data[i] > data.m_data[data.m_maxIndex])
            {
                data.m_maxIndex = i;
            }

            if (data.m_data[i] < data.m_data[data.m_minIndex])
            {
                data.m_minIndex = i;
            }
        }
    }

    public int getCount()
    {
        return m_data.Count;
    }

    public DataPerHour<int> getData(int index)
    {
        return m_data[index];
    }
}

// 每小时的付费累加
public class QueryRechargePerHour : QueryBase
{
    DataEachDay m_result = new DataEachDay();

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
             DBMgr.getInstance().executeQuery(TableName.STAT_RECHARGE_HOUR, user.getDbServerID(), DbName.DB_PUMP, imq,
             0, 0, null, "genTime", false
            /*(param.m_curPage - 1) * param.m_countEachPage, param.m_countEachPage*/);

        if (dataList == null || dataList.Count == 0)
        {
            return OpRes.op_res_not_found_data;
        }

        int i = 0;
        string key = "";

        for (i = 0; i < dataList.Count; i++)
        {
            Dictionary<string, object> data = dataList[i];
            DateTime t = Convert.ToDateTime(data["genTime"]).ToLocalTime();

            for (int k = 0; k < 24; k++)
            {
                key = "h" + k.ToString();
                if (data.ContainsKey(key))
                {
                    int val = Convert.ToInt32(data[key]);
                    m_result.addData(t, k, val);
                }
            }
        }

        return OpRes.opres_success;
    }
}

//////////////////////////////////////////////////////////////////////////
public class ResultOnlinePlayerNumPerHour
{
    // 游戏id->每天的数据
    public Dictionary<int, DataEachDay> m_data = new Dictionary<int, DataEachDay>();

    public void addData(int gameId, DateTime t, int h, int val)
    {
        DataEachDay d = null;
        if (m_data.ContainsKey(gameId))
        {
            d = m_data[gameId];
        }
        else
        {
            d = new DataEachDay();
            m_data.Add(gameId, d);
        }

        d.addData(t, h, val);
    }

    public void reset()
    {
        m_data.Clear();
    }

    public string toJson()
    {
        string str = "";
        Dictionary<string, object> ret = new Dictionary<string, object>();
        foreach(var d in m_data)
        {
            List<Dictionary<string, object>> dList = new List<Dictionary<string, object>>();
            addTodayData(d.Value, dList);
            addYesterdayData(d.Value, dList);
            add7dayData(d.Value, dList);
            add30dayData(d.Value, dList);

            ret.Add(d.Key.ToString(), dList);
        }
        str = ItemHelp.genJsonStr(ret);
        return str;
    }

    void addTodayData(DataEachDay data, List<Dictionary<string, object>> dList)
    {
        int count = data.getCount();
        if (count > 0)
        {
            var s = string.Join<int>(",", data.getData(0).m_data);
            Dictionary<string, object> tmp = new Dictionary<string, object>();
            tmp.Add("onlineList", s);
            dList.Add(tmp);
        }
    }

    void addYesterdayData(DataEachDay data, List<Dictionary<string, object>> dList)
    {
        int count = data.getCount();
        if (count > 1)
        {
            var s = string.Join<int>(",", data.getData(1).m_data);
            Dictionary<string, object> tmp = new Dictionary<string, object>();
            tmp.Add("onlineList", s);
            dList.Add(tmp);
        }
    }

    void add7dayData(DataEachDay data, List<Dictionary<string, object>> dList)
    {
        int count = data.getCount();
        if (count > 7)
        {
            List<double> res = ave(data, 7);
            var s = string.Join<double>(",", res);
            Dictionary<string, object> tmp = new Dictionary<string, object>();
            tmp.Add("onlineList", s);
            dList.Add(tmp);
        }
    }
    
    void add30dayData(DataEachDay data, List<Dictionary<string, object>> dList)
    {
        int count = data.getCount();
        if (count > 30)
        {
            List<double> res = ave(data, 30);
            var s = string.Join<double>(",", res);
            Dictionary<string, object> tmp = new Dictionary<string, object>();
            tmp.Add("onlineList", s);
            dList.Add(tmp);
        }
    }

    List<double> ave(DataEachDay data, int days)
    {
        List<double> res = new List<double>();
        int sum = 0;
        for (int i = 0; i < 24; i++)
        {
            sum = 0;
            for (int k = 1; k <= days; k++)
            {
                sum += data.getData(k).getData(i);
            }

            double r = (double)sum / days;
            res.Add(Math.Round(r, 2));
        }

        return res;
    }
}

// 每小时的实时在线
public class QueryOnlinePlayerNumPerHour : QueryBase
{
    DataEachDay m_result = new DataEachDay();
    ResultOnlinePlayerNumPerHour m_result1 = new ResultOnlinePlayerNumPerHour();

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
        switch (p.m_way)
        {
            case QueryWay.by_way0:
                {
                    int id = 0;
                    if (!int.TryParse(p.m_param, out id))
                    {
                        break;
                    }
                    imq = Query.And(imq, Query.EQ("gameId", id));
                    code = query(p, imq, user);
                }
                break;
            case QueryWay.by_way1:
                {
                    code = query1(p, imq, user);
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

    public override object getQueryResult(object param, GMUser user)
    {
        return m_result1;
    }

    private OpRes query(ParamQuery param, IMongoQuery imq, GMUser user)
    {
        List<Dictionary<string, object>> dataList =
             DBMgr.getInstance().executeQuery(TableName.STAT_ONLINE_HOUR, user.getDbServerID(), DbName.DB_PUMP, imq,
             0, 0, null, "genTime", false
            /*(param.m_curPage - 1) * param.m_countEachPage, param.m_countEachPage*/);

        if (dataList == null || dataList.Count == 0)
        {
            return OpRes.op_res_not_found_data;
        }

        int i = 0;
        string key = "";

        for (i = 0; i < dataList.Count; i++)
        {
            Dictionary<string, object> data = dataList[i];
            DateTime t = Convert.ToDateTime(data["genTime"]).ToLocalTime();

            for (int k = 0; k < 24; k++)
            {
                key = "h" + k.ToString();
                if (data.ContainsKey(key))
                {
                    int val = Convert.ToInt32(data[key]);
                    m_result.addData(t, k, val);
                }
            }
        }

        m_result.calMaxMin();
        return OpRes.opres_success;
    }

    private OpRes query1(ParamQuery param, IMongoQuery imq, GMUser user)
    {
        m_result1.reset();

        List<Dictionary<string, object>> dataList =
             DBMgr.getInstance().executeQuery(TableName.STAT_ONLINE_HOUR, user.getDbServerID(), DbName.DB_PUMP, imq,
             0, 0, null, "genTime", false
            /*(param.m_curPage - 1) * param.m_countEachPage, param.m_countEachPage*/);

        if (dataList == null || dataList.Count == 0)
        {
            return OpRes.op_res_not_found_data;
        }

        int i = 0, gameId = 0, roomId = 0;
        string key = "";

        for (i = 0; i < dataList.Count; i++)
        {
            Dictionary<string, object> data = dataList[i];
            DateTime t = Convert.ToDateTime(data["genTime"]).ToLocalTime();
            if (data.ContainsKey("gameId"))
            {
                gameId = Convert.ToInt32(data["gameId"]);
            }
            else
            {
                gameId = 0;
            }
            if (data.ContainsKey("roomId"))
            {
                roomId = Convert.ToInt32(data["roomId"]);
            }
            else
            {
                roomId = 0;
            }
            if (gameId == (int)GameId.fishlord && roomId > 0)
            {
                gameId = gameId * 1000 + roomId;
            }

            for (int k = 0; k < 24; k++)
            {
                key = "h" + k.ToString();
                if (data.ContainsKey(key))
                {
                    int val = Convert.ToInt32(data[key]);
                    m_result1.addData(gameId, t, k, val);
                }
            }
        }

        return OpRes.opres_success;
    }
}

