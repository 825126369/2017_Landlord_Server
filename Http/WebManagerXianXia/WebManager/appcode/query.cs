using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System.Text.RegularExpressions;
using System.Web.Configuration;
using System.Linq;
using System.Web;

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
      //  m_items.Add(QueryType.queryTypeGmAccount, new QueryGMAccount());
        m_items.Add(QueryType.queryTypeMoney, new QueryPlayerMoney());

       // m_items.Add(QueryType.queryTypeMail, new QueryMail());
       // m_items.Add(QueryType.queryTypeServiceInfo, new QueryServiceInfo());
       // m_items.Add(QueryType.queryTypeRecharge, new QueryRecharge());

        m_items.Add(QueryType.queryTypeAccount, new QueryAccount());
       // m_items.Add(QueryType.queryTypeLoginHistory, new QueryLogin());
       // m_items.Add(QueryType.queryTypeGift, new QueryGift());
       // m_items.Add(QueryType.queryTypeGiftCode, new QueryGiftCode());
       // m_items.Add(QueryType.queryTypeExchange, new QueryExchange());

       // m_items.Add(QueryType.queryTypeLobby, new QueryLobby());
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
        m_items.Add(QueryType.queryTypeGameHistory, new QueryGameHistory());

        m_items.Add(QueryType.queryTypeDragonParam, new QueryDragonParam());
        m_items.Add(QueryType.queryTypeDragonGameModeEarning, new QueryDragonGameModeEarning());
        m_items.Add(QueryType.queryTypeShcdParam, new QueryShcdParam());
        m_items.Add(QueryType.queryTypeIndependentShcd, new QueryIndependentShcd());

        m_items.Add(QueryType.queryTypePlayerMember, new QueryPlayerMember());
        m_items.Add(QueryType.queryTypeGmAccount, new QueryGmAccount());
        m_items.Add(QueryType.queryTypeGmAccountCascade, new QueryGmAccountCascade());
        m_items.Add(QueryType.queryTypeQueryGmAccountDetail, new QueryGmAccountDetail());
        m_items.Add(QueryType.queryTypeQueryScoreOpRecord, new QueryScoreOpRecord());

        m_items.Add(QueryType.queryTypeQueryApiApprove, new QueryApiApprove());
        m_items.Add(QueryType.queryTypeQueryPlayerOrder, new QueryPlayerOrder());
        m_items.Add(QueryType.queryTypeQueryRealTimeOrder, new QueryRealTimeOrder());
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
        reset();
    }

    public void startExport() 
    {
        m_isExport = true;
        reset();
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

    public List<IMongoQuery> getQueryList()
    {
        return m_queryList;
    }

    void reset()
    {
        m_queryList.Clear();
        m_cond.Clear();
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
  //  public int m_propertyType;
    // 初始值
    public long m_startValue;
    // 结束值
    public long m_endValue;
    // 差额
    public long m_deltaValue;
    // 玩家ID
    public int m_playerId;
    // 昵称
    public string m_nickName = "";
    // 游戏id
    public int m_gameId;
    // 额外参数
    public string m_param = "";
    public string m_acc;

    // 投注
    public long m_outlay;
    // 返还
    public long m_income;

    public long m_playerWinBet;

    public string m_id;

    public string getPropertyName()
    {
        /*if (m_propertyType == (int)PropertyType.property_type_gold)
            return "金币";

        if (m_propertyType == (int)PropertyType.property_type_ticket)
            return "礼券";
        */
        return "";
    }

    // 返回动作名称
    public string getActionName()
    {
        /*XmlConfig xml = ResMgr.getInstance().getRes("money_reason.xml");
        if (xml != null)
        {
            return xml.getString(m_actionType.ToString(), "");
        }*/
        return "";
    }

    public string getGameName()
    {
        return StrName.s_gameName[m_gameId];
    }

    public string getExParam(int index)
    {
        if (string.IsNullOrEmpty(m_param))
            return "";

        URLParam uParam = new URLParam();
        uParam.m_text = "详情";
        uParam.m_key = "gameId";
        uParam.m_value = m_gameId.ToString();
        uParam.m_url = DefCC.ASPX_GAME_DETAIL;
        uParam.m_target = "_blank";
        uParam.addExParam("index", index);
        return Tool.genHyperlink(uParam);
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
    static string[] m_field = { "nickname", "account" };
    static string[] m_field1 = { "player_id" };
    private QueryCondition m_cond = new QueryCondition();
    private string m_playerAcc = "";
    private IMongoQuery m_obscureQuery = null;

    // 作查询
    public override OpRes doQuery(object param, GMUser user)
    {
        m_result.Clear();
        m_cond.startQuery();
        m_playerAcc = "";
        OpRes res = makeQuery(param, user, m_cond);
        if (res != OpRes.opres_success)
            return res;

//         if (string.IsNullOrEmpty(m_playerAcc))
//             return OpRes.op_res_not_found_data;

        IMongoQuery imq = m_cond.getImq();

        if (m_obscureQuery == null)
        {
            Regex reg = new Regex("^" + transCode(ItemHelp.getCreateCodeSpecial(user)) + ".*", RegexOptions.IgnoreCase);
            BsonRegularExpression regexp = new BsonRegularExpression(reg);
            m_obscureQuery = Query.Matches("creator", regexp);
        }

        imq = Query.And(imq, m_obscureQuery);
        ParamMoneyQuery p = (ParamMoneyQuery)param;
        return query(p, imq, user);
    }

    string transCode(string code)
    {
        code = code.Replace("(", "\\(");
        code = code.Replace(")", "\\)");
        return code;
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
            case QueryWay.by_way1:  // 通过ID查询
                {
                    if (param.m_param != "")
                    {
                        if (!int.TryParse(param.m_param, out playerId))
                        {
                            return OpRes.op_res_param_not_valid;
                        }

                        Dictionary<string, object> ret =
                                QueryBase.getPlayerProperty(playerId, user, new string[] { "account" });
                        if (ret == null)
                            return OpRes.op_res_not_found_data;
                        if (!ret.ContainsKey("account"))
                            return OpRes.op_res_not_found_data;
                        m_playerAcc = Convert.ToString(ret["account"]);
                    }
                    else
                    {
                       // return OpRes.op_res_param_not_valid;
                    }
                }
                break;
            case QueryWay.by_way0: // 通过账号查询
                {
                    OpRes res = queryByAccount(param, user, ref playerId);
                    if (res != OpRes.opres_success)
                    {
                        return res;
                    }

                    m_playerAcc = param.m_param; // 记于玩家账号
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

//             if (playerId == -1 && (maxt - mint).TotalDays > 8)
//                 return OpRes.op_res_time_beyond_range;

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

        if (!queryCond.isExport())
        {
            queryCond.getQueryList().Add(Query.NE("gameId", BsonValue.Create((int)GameId.fish_practice)));
        }

        if (queryCond.isExport())
        {
            string tc = transCode(ItemHelp.getCreateCodeSpecial(user));
            queryCond.addCond("transCode", tc);
            queryCond.addCond("moneyBase", DefCC.MONEY_BASE);
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
                return OpRes.opres_success;
            }
        }

        return OpRes.opres_success;
    }

    // 通过玩家ID查询
    private OpRes query(ParamMoneyQuery param, IMongoQuery imq, GMUser user)
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

//         string nickName = "";
//         Dictionary<string, object> ret = getPlayerPropertyByAcc(m_playerAcc, user, m_field);
//         if (ret != null && ret.ContainsKey("nickname"))
//         {
//             nickName = Convert.ToString(ret["nickname"]);
//         }

        int i = 0;
        for (i = 0; i < data.Count; i++)
        {
            MoneyItem tmp = new MoneyItem();
            tmp.m_genTime = Convert.ToDateTime(data[i]["genTime"]).ToLocalTime().ToString();
            if (data[i].ContainsKey("reason"))
            {
                tmp.m_actionType = Convert.ToInt32(data[i]["reason"]);
            }
            tmp.m_startValue = Convert.ToInt64(data[i]["oldValue"]);
            tmp.m_endValue = Convert.ToInt64(data[i]["newValue"]);
            tmp.m_deltaValue = tmp.m_endValue - tmp.m_startValue; //Convert.ToInt64(data[i]["addValue"]);

            tmp.m_playerId = Convert.ToInt32(data[i]["playerId"]);
            Dictionary<string, object> ret = getPlayerProperty(tmp.m_playerId, user, m_field);
            if (ret != null && ret.ContainsKey("nickname"))
            {
                tmp.m_nickName = Convert.ToString(ret["nickname"]);
                tmp.m_acc = Convert.ToString(ret["account"]);
            }
          //  tmp.m_propertyType = Convert.ToInt32(data[i]["itemId"]);
            if (data[i].ContainsKey("exInfo"))
            {
                tmp.m_param = Convert.ToString(data[i]["exInfo"]);
            }

            if (data[i].ContainsKey("playerWinBet"))
            {
                tmp.m_playerWinBet = Convert.ToInt64(data[i]["playerWinBet"]);
            }

            tmp.m_gameId = Convert.ToInt32(data[i]["gameId"]);

            if (data[i].ContainsKey("playerOutlay"))
            {
                tmp.m_outlay = Convert.ToInt64(data[i]["playerOutlay"]);
            }
            if (data[i].ContainsKey("playerIncome"))
            {
                tmp.m_income = Convert.ToInt64(data[i]["playerIncome"]);
            }

            tmp.m_id = Convert.ToString(data[i]["_id"]);
            m_result.Add(tmp);
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
}

// 收益查询参数
public class ParamServerEarnings : ParamQuery
{
    // 游戏ID
    public int m_gameId;
}

// 服务器收益
public class QueryServerEarnings : QueryBase
{
    private List<EarningItem> m_result = new List<EarningItem>();

    private Dictionary<DateTime, EarningItem> m_total = new Dictionary<DateTime, EarningItem>();

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
        else if (gameId == 0)
        {
            m_total.Clear();
            queryTotal(user, imq, TableName.PUMP_FISHLORD_EVERY_DAY);
            queryTotal(user, imq, TableName.PUMP_CROCODILE_EVERY_DAY);
            queryTotal(user, imq, TableName.PUMP_DICE_EVERY_DAY);
            queryTotal(user, imq, TableName.PUMP_COWS_EVERY_DAY);
            queryTotal(user, imq, TableName.PUMP_BACCARAT_EVERY_DAY);
            queryTotal(user, imq, TableName.PUMP_FISHPARK_EVERY_DAY);
            queryTotal(user, imq, TableName.PUMP_DRAGON_EVERY_DAY);
            queryTotal(user, imq, TableName.PUMP_SHCD_EVERY_DAY);

            var arr = from s in m_total
                    orderby s.Key
                    select s.Value;

            foreach (var r in arr)
            {
                m_result.Add(r);
            }
            return OpRes.opres_success;
        }
        return OpRes.op_res_failed;
    }

    // 返回查询结果
    public override object getQueryResult()
    {
        return m_result;
    }

    private OpRes queryCommon(GMUser user, IMongoQuery imq, string tableName)
    {
        List<Dictionary<string, object>> dataList = DBMgr.getInstance().executeQuery(tableName, user.getDbServerID(), DbName.DB_PUMP, imq);
        if (dataList == null)
            return OpRes.op_res_not_found_data;

        for (int i = 0; i < dataList.Count; i++)
        {
            EarningItem item = new EarningItem();
            m_result.Add(item);

            item.m_time = Convert.ToDateTime(dataList[i]["Date"]).ToLocalTime().ToShortDateString();
            if (dataList[i].ContainsKey("TodayOutlay"))
            {
                item.addRoomOutlay(4, Convert.ToInt64(dataList[i]["TodayOutlay"]));
            }
            if (dataList[i].ContainsKey("TodayIncome"))
            {
                item.addRoomIncome(4, Convert.ToInt64(dataList[i]["TodayIncome"]));
            }
            if (dataList[i].ContainsKey("room1Protect"))
            {
                item.addRoomOutlay(5, Convert.ToInt64(dataList[i]["room1Protect"]));
            }
            if (dataList[i].ContainsKey("room2Protect"))
            {
                item.addRoomOutlay(6, Convert.ToInt64(dataList[i]["room2Protect"]));
            }

            if (dataList[i].ContainsKey("room1Income"))
            {
                item.addRoomIncome(0, Convert.ToInt64(dataList[i]["room1Income"]));
            }
            if (dataList[i].ContainsKey("room2Income"))
            {
                item.addRoomIncome(1, Convert.ToInt64(dataList[i]["room2Income"]));
            }
            if (dataList[i].ContainsKey("room3Income"))
            {
                item.addRoomIncome(2, Convert.ToInt64(dataList[i]["room3Income"]));
            }
            if (dataList[i].ContainsKey("room4Income"))
            {
                item.addRoomIncome(3, Convert.ToInt64(dataList[i]["room4Income"]));
            }

            if (dataList[i].ContainsKey("room1Outlay"))
            {
                item.addRoomOutlay(0, Convert.ToInt64(dataList[i]["room1Outlay"]));
            }
            if (dataList[i].ContainsKey("room2Outlay"))
            {
                item.addRoomOutlay(1, Convert.ToInt64(dataList[i]["room2Outlay"]));
            }
            if (dataList[i].ContainsKey("room3Outlay"))
            {
                item.addRoomOutlay(2, Convert.ToInt64(dataList[i]["room3Outlay"]));
            }
            if (dataList[i].ContainsKey("room4Outlay"))
            {
                item.addRoomOutlay(3, Convert.ToInt64(dataList[i]["room4Outlay"]));
            }
        }

        return OpRes.opres_success;
    }

    private OpRes queryTotal(GMUser user, IMongoQuery imq, string tableName)
    {
        List<Dictionary<string, object>> dataList = DBMgr.getInstance().executeQuery(tableName, user.getDbServerID(), DbName.DB_PUMP, imq);
        if (dataList == null)
            return OpRes.op_res_failed;

        for (int i = 0; i < dataList.Count; i++)
        {
            EarningItem item = null;
            DateTime t = Convert.ToDateTime(dataList[i]["Date"]).ToLocalTime();
            if (m_total.ContainsKey(t))
            {
                item = m_total[t];
            }
            else
            {
                item = new EarningItem();
                m_total.Add(t, item);
            }

            item.m_time = t.ToShortDateString();
            if (dataList[i].ContainsKey("TodayOutlay"))
            {
                item.addRoomOutlay(4, Convert.ToInt64(dataList[i]["TodayOutlay"]));
            }
            if (dataList[i].ContainsKey("TodayIncome"))
            {
                item.addRoomIncome(4, Convert.ToInt64(dataList[i]["TodayIncome"]));
            }
        }

        return OpRes.opres_success;
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

    // 返回总盈利率   百家乐参数调整界面调用
    public string getTotalRate()
    {
        long income = getIncome() + m_robotIncome;
        long outcome = m_totalOutlay + m_robotOutlay;
        if (income == 0)
            return "0";

        double val = (double)(income - outcome) / income;
        return Math.Round(val, 3).ToString();
    }
}

// 捕鱼参数查询
public class QueryFishlordParam : QueryBase
{
    protected Dictionary<int, ResultFishlordExpRate> m_result = new Dictionary<int, ResultFishlordExpRate>();

    public override OpRes doQuery(object param, GMUser user)
    {
        m_result.Clear();
        return query(user, TableName.FISHLORD_ROOM);
    }

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
            m_result.Add(info.m_roomId, info);
        }

        return OpRes.opres_success;
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

    private OpRes query(GMUser user, IMongoQuery imq, ParamQueryGift param, string tableName)
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

//////////////////////////////////////////////////////////////////////////
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
        if (p1.m_roomId == p2.m_roomId)
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
}

// 金币最多
public class QueryMoneyAtMost : QueryBase
{
    private List<ResultMoneyMost> m_result = new List<ResultMoneyMost>();
    static string[] s_fieldGold = new string[] { "player_id", "gold" };
    static string[] s_fieldTicket = new string[] { "player_id", "ticket" };
    static string[] s_field = new string[] { "nickname" };

    public override OpRes doQuery(object param, GMUser user)
    {
        m_result.Clear();
        ParamQuery p = (ParamQuery)param;
        switch (p.m_way)
        {
            case QueryWay.by_way0:
                {
                    return queryGold(user, p.m_countEachPage);
                }
                break;
            case QueryWay.by_way1:
                {
                    return queryTicket(user, p.m_countEachPage);
                }
                break;
        }
        return OpRes.op_res_failed;
    }

    public override object getQueryResult()
    {
        return m_result;
    }

    private OpRes queryGold(GMUser user, int maxCount)
    {
        List<Dictionary<string, object>> dataList = DBMgr.getInstance().executeQuery(TableName.PLAYER_INFO,
            user.getDbServerID(), DbName.DB_PLAYER, null, 0, maxCount, s_fieldGold, "gold", false);

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
            m_result.Add(info);
        }

        return OpRes.opres_success;
    }

    private OpRes queryTicket(GMUser user, int maxCount)
    {
        List<Dictionary<string, object>> dataList = DBMgr.getInstance().executeQuery(TableName.PLAYER_INFO,
            user.getDbServerID(), DbName.DB_PLAYER, null, 0, maxCount, s_fieldTicket, "ticket", false);

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
        queryList.Add(Query.EQ("gameId", Convert.ToInt32(p.m_boxId)));
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

//////////////////////////////////////////////////////////////////////////
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
    private DateTime m_lastTime = DateTime.MinValue;

    public override OpRes doQuery(object param, GMUser user)
    {
        if (DateTime.Now < m_lastTime)
            return OpRes.opres_success;

        m_lastTime.AddMinutes(5);

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

public class TotalConsumeItem
{
    // 统计时间
    public DateTime m_time;

    // 原因-->消耗总量
    public Dictionary<int, long> m_result = new Dictionary<int, long>();

    public void add(int reason, long value)
    {
        m_result.Add(reason, value);
    }

    public long getValue(int reason)
    {
        if (m_result.ContainsKey(reason))
            return m_result[reason];

        return 0;
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
        if (r >= (int)FishLordExpend.fish_unlock_level_start) 
        {
            return "炮台升级-" + (r - (int)FishLordExpend.fish_unlock_level_start);
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
             DbName.DB_PUMP, imq);

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
            item.add(reason, val);

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
                if (game == "date")
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

// 金币增长历史排行
public class QueryCoinGrowthRank : QueryBase
{
    private List<ResultCoinGrowthRank> m_result = new List<ResultCoinGrowthRank>();

    public override OpRes doQuery(object param, GMUser user)
    {
        string time = (string)param;
        if (time == "")
            return OpRes.op_res_param_not_valid;

        DateTime mint = DateTime.Now, maxt = DateTime.Now;
        bool res = Tool.splitTimeStr(time, ref mint, ref maxt);
        if (!res)
            return OpRes.op_res_time_format_error;

        List<IMongoQuery> queryList = new List<IMongoQuery>();

        IMongoQuery imq1 = Query.LT("genTime", BsonValue.Create(maxt));
        IMongoQuery imq2 = Query.GTE("genTime", BsonValue.Create(mint));
        queryList.Add(Query.And(imq1, imq2));

        IMongoQuery imq = Query.And(queryList);
        m_result.Clear();
        return query(user, imq);
    }

    public override object getQueryResult()
    {
        return m_result;
    }

    private OpRes query(GMUser user, IMongoQuery imq)
    {
        List<Dictionary<string, object>> dataList = DBMgr.getInstance().executeQuery(TableName.PUMP_COIN_GROWTH_HISTORY,
            user.getDbServerID(), DbName.DB_PUMP, imq, 0, 20, null, "gold", false);
        if (dataList == null)
            return OpRes.opres_success;

        for (int i = 0; i < dataList.Count; i++)
        {
            ResultCoinGrowthRank info = new ResultCoinGrowthRank();
            m_result.Add(info);

            info.m_playerId = Convert.ToInt32(dataList[i]["playerId"]);
            info.m_nickName = Convert.ToString(dataList[i]["nickName"]);
            info.m_vipLevel = Convert.ToInt32(dataList[i]["vipLevel"]);
            info.m_gold = Convert.ToInt64(dataList[i]["gold"]);
            info.m_time = Convert.ToDateTime(dataList[i]["genTime"]).ToLocalTime().ToShortDateString();

            Dictionary<string, object> ret =
                QueryBase.getPlayerProperty(info.m_playerId, user, new string[] { "account" });
            if (ret != null)
            {
                if (ret.ContainsKey("account"))
                {
                    info.m_acc = Convert.ToString(ret["account"]);
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

// 经典捕鱼的消耗查询，只查消耗
public class QueryFishConsume : QueryBase
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
        if (p.m_currencyType != 0) // 0查全部的货币消耗
        {
            queryList.Add(Query.EQ("moneyType", BsonValue.Create(p.m_currencyType)));
        }
        
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
public class ParamGameHistory : ParamQueryBase
{
    public int m_gameId;
    public string m_time;
    public string m_bound;  // 靴-局数
}

public class ResultGameHistoryItem
{
    public string m_time;   // 开牌时间
    public int m_gameId;
    public long m_totalBound; // 一直递增的局数
}

// 百家乐
public class ResultGameHistoryBaccaratIem : ResultGameHistoryItem
{
    public long m_boundToday;  // 当天的局数
    public List<int> m_resultList = new List<int>();
    public List<int> m_cardListZhuang = new List<int>();
    public List<int> m_cardListXian = new List<int>();

    // 靴-局数
    public string getBootBound()
    {
        long x = (m_boundToday - 1) / 66 + 1;
        long b = (m_boundToday - 1) % 66 + 1;
        return x + "-" + b;
    }

    public string getResult() // 返回结果
    {
        int count = m_resultList.Count;
        if (count == 0)
            return "";

        string res = DefCC.s_baccaratResult[m_resultList[0]];
        for (int i = 1; i < count; i++)
        {
            res = res + "-" + DefCC.s_baccaratResult[m_resultList[i]];
        }
        return res;
    }

    public string getZhuangJiaCard()
    {
        return getCard(m_cardListZhuang);
    }

    public string getXianJiaCard()
    {
        return getCard(m_cardListXian);
    }

    private string getCard(List<int> cardList)
    {
        int count = cardList.Count;
        if (count == 0)
            return "";

        string res = DefCC.s_pokerNum[cardList[0]];
        for (int i = 1; i < count; i++)
        {
            res = res + "-" + DefCC.s_pokerNum[cardList[i]];
        }
        return res;
    }
}

// 骰宝
public class ResultGameHistoryDicetIem : ResultGameHistoryItem
{
    public int m_dice1;
    public int m_dice2;
    public int m_dice3;

    public string getResult()
    {
        return m_dice1 + "," + m_dice2 + "," + m_dice3 +
            HttpUtility.HtmlDecode("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;") +
            DefCC.s_diceStr[InfoDice.getResult(m_dice1, m_dice2, m_dice3)];
    }
}

// 牛牛
public class ResultGameHistoryCowstIem : ResultGameHistoryItem
{
    public InfoCows m_info = new InfoCows();

    public string getBankerCard()
    {
        return getCard(m_info.m_bankerCard);
    }

    public string getEastCard()
    {
        return getCard(m_info.m_eastCard);
    }

    public string getWestCard()
    {
        return getCard(m_info.m_westCard);
    }

    public string getSouthCard()
    {
        return getCard(m_info.m_southCard);
    }

    public string getNorthCard()
    {
        return getCard(m_info.m_northCard);
    }

    private string getCard(CowsCard card)
    {
        string res = "";
        foreach (CardInfo info in card.m_cards)
        {
            res = res + DefCC.s_pokerColorCows[info.flower] + info.point + "<br/>";
        }
        Cows_CardsCFGData d = Cows_CardsCFG.getInstance().getValue(card.m_cardType);
        if (d != null)
        {
            res = res + d.m_cardName;
        }
        return res;
    }
}

// 鳄鱼大亨
public class ResultGameHistoryCrocodiletIem : ResultGameHistoryItem
{
    public List<int> m_result = new List<int>();
    public int m_awardType;
    public int m_handSel;   // 彩金

    // 开奖结果
    public string getResult()
    {
        if (m_awardType == (int)e_award_type_def.e_type_normal)
        {
            return getResultStr();
        }
        return "";
    }

    // 射灯
    public string getSpotLight()
    {
        if (m_awardType == (int)e_award_type_def.e_type_spot_light)
        {
            return getResultStr();
        }
        return "";
    }

    // 人人有奖
    public string getAllPrizes()
    {
        if (m_awardType == (int)e_award_type_def.e_type_all_prizes)
        {
            if (m_result.Count > 0)
            {
                return m_result[0].ToString() + "倍";
           }
        }
        return "";
    }

    // 彩金
    public string getHandSel()
    {
        if (m_awardType == (int)e_award_type_def.e_type_handsel)
        {
            return getResultStr() + "<br/>" + m_handSel.ToString() + "倍彩金";
        }
        return "";
    }

    private string getResultStr()
    {
        string res = "";
        foreach (var id in m_result)
        {
            Crocodile_RateCFGData data = Crocodile_RateCFG.getInstance().getValue(id + 1);
            if (data != null)
            {
                res = res + data.m_name + "-";
            }
        }
        if (res != "")
        {
            res = res.Remove(res.Length - 1);
        }
        return res;
    }
}

// 黑红梅方
public class ResultGameHistoryShcdItem : ResultGameHistoryItem
{
    public int m_cardType;
    public int m_cardValue;

    public string getResult()
    {
        if (m_cardType == 4)
            return StrName.s_shcdArea[m_cardType];

        return StrName.s_shcdArea[m_cardType] + DefCC.s_pokerNum[m_cardValue];
    }
}

// 游戏历史记录查询
public class QueryGameHistory : QueryBase
{
    List<ResultGameHistoryItem> m_result = new List<ResultGameHistoryItem>();
    private QueryCondition m_cond = new QueryCondition();
    private delegate void fillData(ParamGameHistory param, GMUser user, List<Dictionary<string, object>> dataList);

    public QueryGameHistory()
    {
    }

    public override OpRes doQuery(object param, GMUser user)
    {
        m_result.Clear();
        m_cond.startQuery();
        OpRes res = makeQuery(param, user, m_cond);
        if (res != OpRes.opres_success)
            return res;

        IMongoQuery imq = m_cond.getImq();
        ParamGameHistory p = (ParamGameHistory)param;
        return query(p, imq, user);
    }

    // 返回查询结果
    public override object getQueryResult()
    {
        return m_result;
    }

    public override OpRes makeQuery(object param, GMUser user, QueryCondition queryCond)
    {
        ParamGameHistory p = (ParamGameHistory)param;
        bool res = false;

        DateTime mint = DateTime.Now, maxt = DateTime.Now;
        res = Tool.splitTimeStr(p.m_time, ref mint, ref maxt);
        if (!res)
            return OpRes.op_res_time_format_error;

        IMongoQuery imq1 = Query.LT("genTime", BsonValue.Create(maxt));
        IMongoQuery imq2 = Query.GTE("genTime", BsonValue.Create(mint));
        queryCond.addImq(Query.And(imq1, imq2));

        switch (p.m_gameId)
        {
            case (int)GameId.baccarat: // 百家乐
                {
                    if (!string.IsNullOrEmpty(p.m_bound))
                    {
                        int x = 0;
                        if (!int.TryParse(p.m_bound, out x))
                            return OpRes.op_res_pwd_not_valid;

                        int minV = (x - 1) * 66;
                        int maxV = x * 66 + 1;
                        IMongoQuery imqx1 = Query.LT("dayIndex", BsonValue.Create(maxV));
                        IMongoQuery imqx2 = Query.GT("dayIndex", BsonValue.Create(minV));
                        queryCond.addImq(Query.And(imqx1, imqx2));
                    }
                }
                break;
        }
        return OpRes.opres_success;
    }

    private OpRes query(ParamGameHistory param, IMongoQuery imq, GMUser user)
    {
        fillData fillFun = null;
        string tableName = getTableName(param, ref fillFun);
        if (tableName == "")
            return OpRes.op_res_not_found_data;

        user.totalRecord = DBMgr.getInstance().getRecordCount(tableName, imq,
            user.getDbServerID(), DbName.DB_PUMP);

        List<Dictionary<string, object>> dataList = DBMgr.getInstance().executeQuery(tableName,
                                                     user.getDbServerID(),
                                                     DbName.DB_PUMP,
                                                     imq,
                                                     (param.m_curPage - 1) * param.m_countEachPage,
                                                     param.m_countEachPage);

        if (dataList == null || dataList.Count == 0)
            return OpRes.op_res_not_found_data;

        fillFun(param, user, dataList);

        return OpRes.opres_success;
    }

    string getTableName(ParamGameHistory param, ref fillData fillFun)
    {
        string tableName = "";
        switch (param.m_gameId)
        {
            case (int)GameId.crocodile:
                {
                    fillFun = fillCrocodile;
                    tableName = TableName.HISTORY_CROCODILE;
                }
                break;
            case (int)GameId.dice:
                {
                    fillFun = fillDice;
                    tableName = TableName.HISTORY_DICE;
                }
                break;
            case (int)GameId.cows:
                {
                    fillFun = fillCows;
                    tableName = TableName.HISTORY_COWS;
                }
                break;
            case (int)GameId.baccarat:
                {
                    fillFun = fillBaccarat;
                    tableName = TableName.HISTORY_BACCARAT;
                }
                break;
            case (int)GameId.shcd:
                {
                    fillFun = fillShcd;
                    tableName = TableName.HISTORY_SHCD;
                }
                break;
        }
        return tableName;
    }

    // 填充百家乐数据
    void fillBaccarat(ParamGameHistory param, GMUser user, List<Dictionary<string, object>> dataList)
    {
        for (int i = 0; i < dataList.Count; i++)
        {
            Dictionary<string, object> data = dataList[i];
            ResultGameHistoryBaccaratIem tmp = new ResultGameHistoryBaccaratIem();
            m_result.Add(tmp);

            tmp.m_time = Convert.ToDateTime(data["genTime"]).ToLocalTime().ToString(ConstDef.DATE_TIME24);
            tmp.m_gameId = (int)GameId.baccarat;
            tmp.m_totalBound = Convert.ToInt64(data["gameIndex"]);
            tmp.m_boundToday = Convert.ToInt64(data["dayIndex"]);
            string[] arr = Tool.split(Convert.ToString(data["resultStr"]), ',', StringSplitOptions.RemoveEmptyEntries);
            for (int k = 0; k < arr.Length; k++)
            {
                tmp.m_resultList.Add(Convert.ToInt32(arr[k]));
            }
            arr = Tool.split(Convert.ToString(data["playerCardStr"]), ',', StringSplitOptions.RemoveEmptyEntries);
            for (int k = 0; k < arr.Length; k++)
            {
                tmp.m_cardListXian.Add(Convert.ToInt32(arr[k]));
            }
            arr = Tool.split(Convert.ToString(data["bankerCardStr"]), ',', StringSplitOptions.RemoveEmptyEntries);
            for (int k = 0; k < arr.Length; k++)
            {
                tmp.m_cardListZhuang.Add(Convert.ToInt32(arr[k]));
            }
        }
    }

    // 填充骰宝数据
    void fillDice(ParamGameHistory param, GMUser user, List<Dictionary<string, object>> dataList)
    {
        for (int i = 0; i < dataList.Count; i++)
        {
            Dictionary<string, object> data = dataList[i];
            ResultGameHistoryDicetIem tmp = new ResultGameHistoryDicetIem();
            m_result.Add(tmp);

            tmp.m_time = Convert.ToDateTime(data["genTime"]).ToLocalTime().ToString(ConstDef.DATE_TIME24);
            tmp.m_gameId = (int)GameId.dice;
            tmp.m_totalBound = Convert.ToInt64(data["gameIndex"]);
            tmp.m_dice1 = Convert.ToInt32(data["dice1"]);
            tmp.m_dice2 = Convert.ToInt32(data["dice2"]);
            tmp.m_dice3 = Convert.ToInt32(data["dice3"]);
        }
    }

    // 填充牛牛数据
    void fillCows(ParamGameHistory param, GMUser user, List<Dictionary<string, object>> dataList)
    {
        for (int i = 0; i < dataList.Count; i++)
        {
            Dictionary<string, object> data = dataList[i];
            ResultGameHistoryCowstIem tmp = new ResultGameHistoryCowstIem();
            m_result.Add(tmp);

            tmp.m_time = Convert.ToDateTime(data["genTime"]).ToLocalTime().ToString(ConstDef.DATE_TIME24);
            tmp.m_gameId = (int)GameId.cows;
            if (data.ContainsKey("gameIndex"))
            {
                tmp.m_totalBound = Convert.ToInt64(data["gameIndex"]);
            }
            
            GameDetail.detailInfoForCows(tmp.m_info, data);
        }
    }

    // 填充鳄鱼大亨数据
    void fillCrocodile(ParamGameHistory param, GMUser user, List<Dictionary<string, object>> dataList)
    {
        for (int i = 0; i < dataList.Count; i++)
        {
            Dictionary<string, object> data = dataList[i];
            ResultGameHistoryCrocodiletIem tmp = new ResultGameHistoryCrocodiletIem();
            m_result.Add(tmp);

            tmp.m_time = Convert.ToDateTime(data["genTime"]).ToLocalTime().ToString(ConstDef.DATE_TIME24);
            tmp.m_gameId = (int)GameId.crocodile;
            tmp.m_totalBound = Convert.ToInt64(data["gameIndex"]);
            tmp.m_awardType = Convert.ToInt32(data["awardType"]);
            tmp.m_handSel = Convert.ToInt32(data["handselProb"]);
            string[] arr = Tool.split(Convert.ToString(data["resultStr"]), ',', StringSplitOptions.RemoveEmptyEntries);
            for (int k = 0; k < arr.Length; k++)
            {
                tmp.m_result.Add(Convert.ToInt32(arr[k]));
            }   
        }
    }

    // 填充黑红梅方数据
    void fillShcd(ParamGameHistory param, GMUser user, List<Dictionary<string, object>> dataList)
    {
        for (int i = 0; i < dataList.Count; i++)
        {
            Dictionary<string, object> data = dataList[i];
            ResultGameHistoryShcdItem tmp = new ResultGameHistoryShcdItem();
            m_result.Add(tmp);

            tmp.m_time = Convert.ToDateTime(data["genTime"]).ToLocalTime().ToString(ConstDef.DATE_TIME24);
            tmp.m_gameId = (int)GameId.shcd;
            tmp.m_totalBound = Convert.ToInt64(data["gameIdx"]);
            tmp.m_cardType = Convert.ToInt32(data["cardType"]);
            tmp.m_cardValue = Convert.ToInt32(data["cardValue"]);
        }
    }
}

