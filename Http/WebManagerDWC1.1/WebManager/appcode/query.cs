using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System.Text.RegularExpressions;
using System.Web.Configuration;

public enum QueryType
{
    // GM账号
    queryTypeGmAccount,

    // 金币，钻石变化
    queryTypeMoney,

    // 抽奖
    queryTypeLottery,

    // 客服信息查询
    queryTypeServiceInfo,

    // 签到
    queryTypeSignIn,

    // 商店购买
    queryTypeShopBuy,

    // 邮件
    queryTypeMail,

    // 充值记录
    queryTypeRecharge,

    // 账号查询
    queryTypeAccount,

    // 登陆历史
    queryTypeLoginHistory,

    // 礼包查询
    queryTypeGift,

    // 礼包码查询
    queryTypeGiftCode,

    // 兑换
    queryTypeExchange,

    // 大厅通用数据
    queryTypeLobby,

    // 服务器收益
    queryTypeServerEarnings,

    // 捕鱼独立数据
    queryTypeIndependentFishlord,

    // 鳄鱼独立数据
    queryTypeIndependentCrocodile,

    // 骰宝独立数据
    queryTypeIndependentDice,

    // 骰宝盈利率
    queryTypeDiceEarnings,

    // 当前公告
    queryTypeCurNotice,

    // 捕鱼参数查询
    queryTypeFishlordParam,

    // 鳄鱼大亨参数查询
    queryTypeCrocodileParam,

    // 鱼的情况统计
    queryTypeFishStat,

    // 货币最多的玩家
    queryTypeMoneyAtMost,

    // 旧的盈利率
    queryTypeOldEaringsRate,

    // 经典捕鱼阶段分析
    queryTypeFishlordStage,

    // 当前在线
    queryTypeOnlinePlayerCount,

    // 操作日志
    queryTypeOpLog,

    // 查询玩家头像
    queryTypePlayerHead,

    // 消耗总计
    queryTypeTotalConsume,

    // 各游戏收入
    queryTypeGameRecharge
}

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

        m_items.Add(QueryType.queryTypeLottery, new QueryLottery());
        m_items.Add(QueryType.queryTypeSignIn, new QuerySignIn());

        m_items.Add(QueryType.queryTypeShopBuy, new QueryShopBuy());

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
        m_items.Add(QueryType.queryTypeCrocodileParam, new QueryCrocodileParam());
        m_items.Add(QueryType.queryTypeFishStat, new QueryFish());
        m_items.Add(QueryType.queryTypeMoneyAtMost, new QueryMoneyAtMost());

        m_items.Add(QueryType.queryTypeOldEaringsRate, new QueryOldEarningRate());
        m_items.Add(QueryType.queryTypeFishlordStage, new QueryFishlordStage());
        m_items.Add(QueryType.queryTypeDiceEarnings, new QueryDiceEarningsParam());
        m_items.Add(QueryType.queryTypeOnlinePlayerCount, new QueryOnlinePlayerCount());
        m_items.Add(QueryType.queryTypeOpLog, new QueryOpLog());

        m_items.Add(QueryType.queryTypePlayerHead, new QueryPlayerHead());
        m_items.Add(QueryType.queryTypeTotalConsume, new QueryTotalConsume());
        m_items.Add(QueryType.queryTypeGameRecharge, new QueryGameRechargeByDay());
    }
}

///////////////////////////////////////////////////////////////////////////////

public class QueryBase
{
    // 作查询
    public virtual OpRes doQuery(object param, GMUser user) { return OpRes.op_res_failed; }

    // 返回查询结果
    public virtual object getQueryResult() { return null; }

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
            return "礼券";

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
    static string[] m_field = { "nickname" };
    static string[] m_field1 = { "player_id" };
    private QueryCondition m_cond = new QueryCondition();

    // 作查询
    public override OpRes doQuery(object param, GMUser user)
    {
        m_result.Clear();
        m_cond.startQuery();
        OpRes res = makeQuery(param, user, m_cond);
        if (res != OpRes.opres_success)
            return res;

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

// 抽奖
public class QueryLottery : QueryBase
{
    private List<LotteryItem> m_result = new List<LotteryItem>();

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
        if (!string.IsNullOrEmpty(p.m_boxId))
        {
            try
            {
                int id = Convert.ToInt32(p.m_boxId);
                queryList.Add(Query.EQ("boxId", BsonValue.Create(id)));
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
        user.totalRecord = DBMgr.getInstance().getRecordCount(TableName.PUMP_LOTTERY, imq, user.getDbServerID(), DbName.DB_PUMP);

        List<Dictionary<string, object>> data =
             DBMgr.getInstance().executeQuery(TableName.PUMP_LOTTERY, user.getDbServerID(), DbName.DB_PUMP, imq,
                                              (param.m_curPage - 1) * param.m_countEachPage, param.m_countEachPage);

        if (data == null || data.Count == 0)
        {
            return OpRes.op_res_not_found_data;
        }

        int i = 0;
        for (i = 0; i < data.Count; i++)
        {
            LotteryItem tmp = new LotteryItem();
            tmp.m_genTime = Convert.ToDateTime(data[i]["genTime"]).ToLocalTime().ToString();
            tmp.m_playerId = Convert.ToInt32(data[i]["playerId"]);
            tmp.m_boxId = Convert.ToInt32(data[i]["boxId"]);
            tmp.m_cost = Convert.ToInt32(data[i]["cost"]);
            Tool.parseItemFromDic(data[i]["items"] as Dictionary<string, object>, tmp.m_rewardList);
            m_result.Add(tmp);
        }
        return OpRes.opres_success;
    }
}

//////////////////////////////////////////////////////////////////////////

public class SignInItem
{
    // 生成时间
    public string m_genTime = "";
    public int m_playerId;
    public int m_signCount;
    public int m_itemId;
    public int m_itemCount;
    public int m_signVipLevel;

    public string getItemName()
    {
        ItemCFGData data = ItemCFG.getInstance().getValue(m_itemId);
        if (data != null)
        {
            return data.m_itemName;
        }
        return "";
    }
}

// 签到
public class QuerySignIn : QueryBase
{
    private List<SignInItem> m_result = new List<SignInItem>();

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
        user.totalRecord = DBMgr.getInstance().getRecordCount(TableName.PUMP_PLAYER_SIGN, imq, user.getDbServerID(), DbName.DB_PUMP);

        List<Dictionary<string, object>> data =
             DBMgr.getInstance().executeQuery(TableName.PUMP_PLAYER_SIGN, user.getDbServerID(), DbName.DB_PUMP, imq,
                                              (param.m_curPage - 1) * param.m_countEachPage, param.m_countEachPage);

        if (data == null || data.Count == 0)
        {
            return OpRes.op_res_not_found_data;
        }

        int i = 0;
        for (i = 0; i < data.Count; i++)
        {
            SignInItem tmp = new SignInItem();
            tmp.m_genTime = Convert.ToDateTime(data[i]["genTime"]).ToLocalTime().ToString();
            tmp.m_playerId = Convert.ToInt32(data[i]["playerId"]);
            tmp.m_signCount = Convert.ToInt32(data[i]["signCount"]);
            tmp.m_itemId = Convert.ToInt32(data[i]["itemId"]);
            tmp.m_itemCount = Convert.ToInt32(data[i]["itemCount"]);
            tmp.m_signVipLevel = Convert.ToInt32(data[i]["vipLevel"]);
            m_result.Add(tmp);
        }
        return OpRes.opres_success;
    }
}

//////////////////////////////////////////////////////////////////////////

public class ShopBuyItem
{
    public string m_genTime = "";
    public int m_playerId;
    public int m_shopId;
    public int m_currencyType;
    public int m_cost;
    public int m_itemId;
    public int m_itemCount;

    public string getCost()
    {
        if (m_currencyType == (int)ItemType.e_itd_gold)
            return m_cost.ToString() + " 金币";

        if (m_currencyType == (int)ItemType.e_itd_gem)
            return m_cost.ToString() + " 钻石";

        return m_cost.ToString();
    }

    public string getItemName()
    {
        ItemCFGData data = ItemCFG.getInstance().getValue(m_itemId);
        if (data != null)
        {
            return data.m_itemName;
        }
        return "";
    }
}

// 商店购买
public class QueryShopBuy : QueryBase
{
    private List<ShopBuyItem> m_result = new List<ShopBuyItem>();

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
        if (!string.IsNullOrEmpty(p.m_boxId))
        {
            try
            {
                int id = Convert.ToInt32(p.m_boxId);
                queryList.Add(Query.EQ("shopId", BsonValue.Create(id)));
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
        user.totalRecord = DBMgr.getInstance().getRecordCount(TableName.PUMP_SHOP_BUY, imq, user.getDbServerID(), DbName.DB_PUMP);

        List<Dictionary<string, object>> data =
             DBMgr.getInstance().executeQuery(TableName.PUMP_SHOP_BUY, user.getDbServerID(), DbName.DB_PUMP, imq,
                                              (param.m_curPage - 1) * param.m_countEachPage, param.m_countEachPage);

        if (data == null || data.Count == 0)
        {
            return OpRes.op_res_not_found_data;
        }

        int i = 0;
        for (i = 0; i < data.Count; i++)
        {
            ShopBuyItem tmp = new ShopBuyItem();
            tmp.m_genTime = Convert.ToDateTime(data[i]["genTime"]).ToLocalTime().ToString();
            tmp.m_playerId = Convert.ToInt32(data[i]["playerId"]);
            tmp.m_shopId = Convert.ToInt32(data[i]["shopId"]);
            tmp.m_currencyType = Convert.ToInt32(data[i]["currencyType"]);
            tmp.m_cost = Convert.ToInt32(data[i]["cost"]);
            tmp.m_itemId = Convert.ToInt32(data[i]["itemId"]);
            tmp.m_itemCount = Convert.ToInt32(data[i]["itemCount"]);
            m_result.Add(tmp);
        }
        return OpRes.opres_success;
    }
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
    public long m_totalOutlay;

    // 系统总收入
    public long m_totalIncome;

    // 返回实际盈利率
    public string getFactExpRate()
    {
        if (m_totalIncome == 0)
            return "0";

        double factGain = (double)(m_totalIncome - m_totalOutlay) / m_totalIncome;
        return Math.Round(factGain, 3).ToString();
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
            return queryFishlord(user, imq);
        }
        else if (gameId == (int)GameId.crocodile)
        {
            return queryCrocodile(user, imq);
        }
        else if (gameId == (int)GameId.dice)
        {
            return queryDice(user, imq);
        }
        return OpRes.op_res_failed;
    }

    // 返回查询结果
    public override object getQueryResult()
    {
        return m_result;
    }

    // 经典捕鱼
    private OpRes queryFishlord(GMUser user, IMongoQuery imq)
    {
        List<Dictionary<string, object>> dataList = DBMgr.getInstance().executeQuery(TableName.PUMP_FISHLORD_EVERY_DAY, user.getDbServerID(), DbName.DB_PUMP, imq);
        for (int i = 0; i < dataList.Count; i++)
        {
            EarningItem item = new EarningItem();
            m_result.Add(item);

            item.m_time = Convert.ToDateTime(dataList[i]["Date"]).ToLocalTime().ToString();
            if (dataList[i].ContainsKey("TodayOutlay"))
            {
                item.m_totalOutlay = Convert.ToInt64(dataList[i]["TodayOutlay"]);
            }
            if (dataList[i].ContainsKey("TodayIncome"))
            {
                item.m_totalIncome = Convert.ToInt64(dataList[i]["TodayIncome"]);
            }
        }

        return OpRes.opres_success;
    }

    // 鳄鱼
    private OpRes queryCrocodile(GMUser user, IMongoQuery imq)
    {
        List<Dictionary<string, object>> dataList = DBMgr.getInstance().executeQuery(TableName.PUMP_CROCODILE_EVERY_DAY, user.getDbServerID(), DbName.DB_PUMP, imq);
        for (int i = 0; i < dataList.Count; i++)
        {
            EarningItem item = new EarningItem();
            m_result.Add(item);

            item.m_time = Convert.ToDateTime(dataList[i]["Date"]).ToLocalTime().ToString();
            if (dataList[i].ContainsKey("TodayOutlay"))
            {
                item.m_totalOutlay = Convert.ToInt64(dataList[i]["TodayOutlay"]);
            }
            if (dataList[i].ContainsKey("TodayIncome"))
            {
                item.m_totalIncome = Convert.ToInt64(dataList[i]["TodayIncome"]);
            }
        }

        return OpRes.opres_success;
    }

    // 骰宝
    private OpRes queryDice(GMUser user, IMongoQuery imq)
    {
        List<Dictionary<string, object>> dataList = DBMgr.getInstance().executeQuery(TableName.PUMP_DICE_EVERY_DAY, user.getDbServerID(), DbName.DB_PUMP, imq);
        for (int i = 0; i < dataList.Count; i++)
        {
            EarningItem item = new EarningItem();
            m_result.Add(item);

            item.m_time = Convert.ToDateTime(dataList[i]["Date"]).ToLocalTime().ToString();
            if (dataList[i].ContainsKey("TodayOutlay"))
            {
                item.m_totalOutlay = Convert.ToInt64(dataList[i]["TodayOutlay"]);
            }
            if (dataList[i].ContainsKey("TodayIncome"))
            {
                item.m_totalIncome = Convert.ToInt64(dataList[i]["TodayIncome"]);
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
        Crocodile_RateCFGData data = Dice_BetAreaCFG.getInstance().getValue(areaId);
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

// 骰宝盈利参数查询
public class QueryDiceEarningsParam : QueryBase
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
        List<Dictionary<string, object>> dataList = DBMgr.getInstance().executeQuery(TableName.DICE_ROOM,
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

            m_result.Add(info.m_roomId, info);
        }

        return OpRes.opres_success;
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

    // 返回实际盈利率
    public string getFactExpRate()
    {
        if (m_totalIncome == 0 && m_totalOutlay == 0)
            return "0";
        if (m_totalIncome == 0)
            return "-∞";

        double factGain = (double)(m_totalIncome - m_totalOutlay) / m_totalIncome;
        return Math.Round(factGain, 3).ToString();
    }
}

// 捕鱼参数查询
public class QueryFishlordParam : QueryBase
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
        List<Dictionary<string, object>> dataList = DBMgr.getInstance().executeQuery(TableName.FISHLORD_ROOM,
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
            
            m_result.Add(info.m_roomId, info);
        }

        return OpRes.opres_success;
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
        m_result.Clear();
        OpRes res = query(user);
        m_result.Sort(sortFish);
        return res;
    }

    public override object getQueryResult()
    {
        return m_result;
    }

    private OpRes query(GMUser user)
    {
        List<Dictionary<string, object>> dataList = DBMgr.getInstance().executeQuery(TableName.PUMP_ALL_FISH,
            user.getDbServerID(), DbName.DB_PUMP);

        for (int i = 0; i < dataList.Count; i++)
        {
            ResultFish info = new ResultFish();
            info.m_fishId = Convert.ToInt32(dataList[i]["fishid"]);
            info.m_hitCount = Convert.ToInt64(dataList[i]["hitcount"]);
            info.m_dieCount = Convert.ToInt64(dataList[i]["deadcount"]);
            info.m_outlay = Convert.ToInt64(dataList[i]["totaloutlay"]);
            info.m_income = Convert.ToInt64(dataList[i]["totalincome"]);
            m_result.Add(info);
        }

        return OpRes.opres_success;
    }

    private int sortFish(ResultFish p1, ResultFish p2)
    {
        return p1.m_fishId - p2.m_fishId;
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

        return query(p, imq, user);
    }

    // 返回查询结果
    public override object getQueryResult()
    {
        return m_result;
    }

    private OpRes query(ParamQueryGift param, IMongoQuery imq, GMUser user)
    {
        user.totalRecord = DBMgr.getInstance().getRecordCount(TableName.PUMP_FISH_TABLE_LOG, imq, user.getDbServerID(), DbName.DB_PUMP);

        List<Dictionary<string, object>> data =
             DBMgr.getInstance().executeQuery(TableName.PUMP_FISH_TABLE_LOG, user.getDbServerID(), DbName.DB_PUMP, imq,
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
                                                     param.m_countEachPage);
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
        return xml.getString(r.ToString(), "");
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
