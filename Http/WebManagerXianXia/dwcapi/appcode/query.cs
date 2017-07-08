using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web.Configuration;
using System.Linq;
using System.Globalization;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

// 查询信息
public class ParamQuery : ParamBase
{
    // 当前查询第几页，以1开始计数
    public int m_curPage;

    // 每页多少条记录
    public int m_countEachPage;

    public string m_curPageStr = "";
    public string m_countEachPageStr = "";

    public override bool isParamValid()
    {
        if (!base.isParamValid())
            return false;

        if (string.IsNullOrEmpty(m_countEachPageStr))
            return false;

        if (!int.TryParse(m_countEachPageStr, out m_countEachPage))
            return false;

        if (m_countEachPage < 0)
            return false;

        if (m_countEachPage > 0)
        {
            if (string.IsNullOrEmpty(m_curPageStr))
                return false;

            if (!int.TryParse(m_curPageStr, out m_curPage))
                return false;

            if (m_curPage <= 0)
                return false;
        }
                
        if (m_countEachPage > 1000)
        {
            m_countEachPage = 1000;
        }
        return true;
    }
}

public class ParamQueryPlayer : ParamBase
{
    public ParamQueryPlayer()
    {
        m_fieldIndex = 2;
    }
}

// 查询玩家详细信息
public class QueryPlayerDetailInfo : DyOpBase
{
    public override string doQuery(object param)
    {
        ParamQueryPlayer p = (ParamQueryPlayer)param;
        bool cres = createGMUser(p);
        if (!cres)
        {
            return Helper.genJsonStr(m_retData);
        }
        if (!p.checkSign(m_gmUser))
        {
            m_retData.Add("result", RetCode.RET_SIGN_ERROR);
            return Helper.genJsonStr(m_retData);
        }

        PlayerDetail player = new PlayerDetail(p.m_playerAcc, m_gmUser);
        if (!player.m_isExists)
        {
            m_retData.Add("result", RetCode.RET_NO_PLAYER);
            return Helper.genJsonStr(m_retData);
        }

        if (!player.isOwner(m_gmUser.m_acc))
        {
            m_retData.Add("result", RetCode.RET_NO_RIGHT);
            return Helper.genJsonStr(m_retData);
        }

        m_retData.Add("result", RetCode.RET_SUCCESS);
        m_retData.Add("playerAcc", p.m_playerAcc);

        double remainMoney = Helper.showMoneyValue(player.isInGame() ? player.m_moneyOnline : player.m_money);

        m_retData.Add("money", remainMoney);
        //m_retData.Add("moneyType", player.m_moneyType);
        m_retData.Add("state", player.m_state);
        m_retData.Add("createTime", player.m_createTime);

        return Helper.genJsonStr(m_retData);
    }
}

//////////////////////////////////////////////////////////////////////////

public class ParamQueryPlayerTrade : ParamQuery
{
    public string m_startTime;
    public string m_endTime;

    public string m_opTypeStr;
    public int m_opType = 2;

    public ParamQueryPlayerTrade()
    {
        m_fieldIndex = 2;
    }

    public override bool isParamValid()
    {
        if (!base.isParamValid())
            return false;

        if (string.IsNullOrEmpty(m_startTime))
            return false;

        if (string.IsNullOrEmpty(m_endTime))
            return false;

        if (!string.IsNullOrEmpty(m_opTypeStr))
        {
            if (!int.TryParse(m_opTypeStr, out m_opType))
            {
                return false;
            }
        }
        return true;
    }
}

// 查询玩家的上分下分情况
public class QueryPlayerTradeInfo : DyOpBase
{
    public const string SQL_COUNT_COND = " opDst='{0}' and opTime >='{1}' and opTime <= '{2}' {3} ";

    public const string SQL_RECORD = "select opTime,opType,opScore,moneyType from {0} " + 
        " where opDst='{1}' and opTime >='{2}' and opTime <= '{3}' {4} LIMIT {5}, {6} ";

    public const string SQL_OP_COND = " and opType={0} ";

    public override string doQuery(object param)
    {
        ParamQueryPlayerTrade p = (ParamQueryPlayerTrade)param;
        DateTime startT, endT;
        if (!DateTime.TryParseExact(p.m_startTime, CONST.DATE_TIME_FORMAT, CONST.DATE_PROVIDER, DateTimeStyles.None, out startT))
        {
            m_retData.Add("result", RetCode.RET_PARAM_NOT_VALID);
            return Helper.genJsonStr(m_retData);
        }
        if (!DateTime.TryParseExact(p.m_endTime, CONST.DATE_TIME_FORMAT, CONST.DATE_PROVIDER, DateTimeStyles.None, out endT))
        {
            m_retData.Add("result", RetCode.RET_PARAM_NOT_VALID);
            return Helper.genJsonStr(m_retData);
        }

        bool cres = createGMUser(p);
        if (!cres)
        {
            return Helper.genJsonStr(m_retData);
        }
        if (!p.checkSign(m_gmUser))
        {
            m_retData.Add("result", RetCode.RET_SIGN_ERROR);
            return Helper.genJsonStr(m_retData);
        }

        Player player = new Player(p.m_playerAcc, m_gmUser);
        if (!player.m_isExists)
        {
            m_retData.Add("result", RetCode.RET_NO_PLAYER);
            return Helper.genJsonStr(m_retData);
        }

        if (!player.isOwner(m_gmUser.m_acc))
        {
            m_retData.Add("result", RetCode.RET_NO_RIGHT);
            return Helper.genJsonStr(m_retData);
        }

        m_retData.Add("result", RetCode.RET_SUCCESS);
        m_retData.Add("playerAcc", p.m_playerAcc);

        if (p.m_countEachPage == 0) // 返回条数
        {
            string cond = string.Format(SQL_COUNT_COND, p.m_playerAcc, startT.ToString(ConstDef.DATE_TIME24),
                endT.ToString(ConstDef.DATE_TIME24), getOpCond(p));
            long count = m_gmUser.sqlDb.getRecordCount(TableName.GM_SCORE, cond, MySqlDbName.DB_XIANXIA);
            m_retData.Add("recordCount", count);
        }
        else
        {
            string cmd = string.Format(SQL_RECORD,
                                       TableName.GM_SCORE,
                                       p.m_playerAcc,
                                       startT.ToString(ConstDef.DATE_TIME24),
                                       endT.ToString(ConstDef.DATE_TIME24),
                                       getOpCond(p),
                                       (p.m_curPage - 1) * p.m_countEachPage,
                                       p.m_countEachPage);

            List<Dictionary<string, object>> dataList = m_gmUser.sqlDb.queryList(cmd, MySqlDbName.DB_XIANXIA);
            m_retData.Add("record", dataList);
        }
       
        return Helper.genJsonStr(m_retData);
    }

    private string getOpCond(ParamQueryPlayerTrade p)
    {
        if (p.m_opType == 0 || p.m_opType == 1)
            return string.Format(SQL_OP_COND, p.m_opType);

        return "";
    }
}

//////////////////////////////////////////////////////////////////////////
// 玩家的金币变化结构
public class ParamQueryPlayerMoneyChange : ParamQuery
{
    public string m_startTime;
    public string m_endTime;

    public ParamQueryPlayerMoneyChange()
    {
        m_fieldIndex = 3;
    }

    public override bool isParamValid()
    {
        if (!base.isParamValid())
            return false;

        if (string.IsNullOrEmpty(m_startTime))
            return false;

        if (string.IsNullOrEmpty(m_endTime))
            return false;

        return true;
    }

    protected override bool playerAccIsValid()
    {
        return true;
    }
}

// 查询玩家的玩家的金币变化，每局的押注情况。
public class QueryPlayerMoneyChangeInGame : DyOpBase
{
    public static string[] s_fields = { "genTime", "playerAcc", "gameId", "playerOutlay", "playerIncome", "oldValue", "newValue", "exInfo" };

    public override string doQuery(object param)
    {
        ParamQueryPlayerMoneyChange p = (ParamQueryPlayerMoneyChange)param;

        bool cres = createGMUser(p);
        if (!cres)
        {
            return Helper.genJsonStr(m_retData);
        }
        if (!p.checkSign(m_gmUser))
        {
            m_retData.Add("result", RetCode.RET_SIGN_ERROR);
            return Helper.genJsonStr(m_retData);
        }

        if (!string.IsNullOrEmpty(p.m_playerAcc))
        {
            Player player = new Player(p.m_playerAcc, m_gmUser);
            if (!player.m_isExists)
            {
                m_retData.Add("result", RetCode.RET_NO_PLAYER);
                return Helper.genJsonStr(m_retData);
            }

            if (!player.isOwner(m_gmUser.m_acc))
            {
                m_retData.Add("result", RetCode.RET_NO_RIGHT);
                return Helper.genJsonStr(m_retData);
            }
        }
       
        IMongoQuery imq = null;
        bool code = createCond(p, ref imq);
        if (!code)
        {
            return Helper.genJsonStr(m_retData);
        }

        m_retData.Add("result", RetCode.RET_SUCCESS);

        if (!string.IsNullOrEmpty(p.m_playerAcc))
        {
            m_retData.Add("playerAcc", p.m_playerAcc);
        }
        
        if (p.m_countEachPage == 0) // 返回条数
        {
            long count = MongodbLog.Instance.ExecuteGetCount(TableName.LOG_PLAYER_INFO, imq);
            m_retData.Add("totalCount", count);  // 记录的总条数
        }
        else
        {
            long count = MongodbLog.Instance.ExecuteGetCount(TableName.LOG_PLAYER_INFO, imq);
           // m_retData.Add("totalCount", count);  // 记录的总条数
           // m_retData.Add("curPage", p.m_curPage);
            // 单位
            m_retData.Add("moneyBase", CONST.MONEY_BASE);

            List<Dictionary<string, object>> dataList =
                MongodbLog.Instance.ExecuteGetListByQuery(TableName.LOG_PLAYER_INFO, imq, s_fields, "genTime", false,
                (p.m_curPage - 1) * p.m_countEachPage, p.m_countEachPage);

            m_retData.Add("record", dataList);

            Dictionary<string, object> pageInfo = new Dictionary<string, object>();
            m_retData.Add("pagination", pageInfo);
            pageInfo.Add("currentPage", p.m_curPage);
            pageInfo.Add("totalPages", (int)Math.Ceiling((double)count / p.m_countEachPage));
            pageInfo.Add("itemsPerPage", p.m_countEachPage);
            pageInfo.Add("totalCount", count);
        }

        return Helper.genJsonStr(m_retData, true);
    }

    // 构造查询条件
    bool createCond(ParamQueryPlayerMoneyChange p, ref IMongoQuery imq)
    {
        DateTime startT, endT;
        if (!DateTime.TryParseExact(p.m_startTime, CONST.DATE_TIME_FORMAT, CONST.DATE_PROVIDER, DateTimeStyles.None, out startT))
        {
            m_retData.Add("result", RetCode.RET_PARAM_NOT_VALID);
            return false;
        }
        if (!DateTime.TryParseExact(p.m_endTime, CONST.DATE_TIME_FORMAT, CONST.DATE_PROVIDER, DateTimeStyles.None, out endT))
        {
            m_retData.Add("result", RetCode.RET_PARAM_NOT_VALID);
            return false;
        }

        List<IMongoQuery> queryList = new List<IMongoQuery>();

        if (!string.IsNullOrEmpty(p.m_playerAcc))
        {
            Dictionary<string, object> data = Helper.getPlayerPropertyByAcc(p.m_playerAcc, new string[] { "player_id" });
            if (data == null)
            {
                m_retData.Add("result", RetCode.RET_NO_PLAYER);
                return false;
            }
            if (!data.ContainsKey("player_id"))
            {
                m_retData.Add("result", RetCode.RET_NO_PLAYER);
                return false;
            }

            queryList.Add(Query.EQ("playerId", BsonValue.Create(data["player_id"])));
        }
        else
        {
            Regex reg = new Regex("^" + transCode(m_gmUser.m_createCode) + ".*", RegexOptions.IgnoreCase);
            BsonRegularExpression regexp = new BsonRegularExpression(reg);
            IMongoQuery tmp = Query.Matches("creator", regexp);
            queryList.Add(tmp);
        }
        IMongoQuery imq1 = Query.LT("genTime", BsonValue.Create(endT));
        IMongoQuery imq2 = Query.GTE("genTime", BsonValue.Create(startT));
        queryList.Add(Query.And(imq1, imq2));
        imq = Query.And(queryList);

        return true;
    }

    string transCode(string code)
    {
        code = code.Replace("(", "\\(");
        code = code.Replace(")", "\\)");
        return code;
    }
}

//////////////////////////////////////////////////////////////////////////
// 输赢报表
public class ParamQueryPlayerWinLose : ParamQuery
{
    public string m_startTime;
    public string m_endTime;

    public ParamQueryPlayerWinLose()
    {
        m_fieldIndex = 2;
    }

    public override bool isParamValid()
    {
        if (!base.isParamValid())
            return false;

        if (string.IsNullOrEmpty(m_startTime))
            return false;

        if (string.IsNullOrEmpty(m_endTime))
            return false;

        return true;
    }
}

// 玩家的输赢报表
public class QueryPlayerWinLoseReport : DyOpBase
{
    public const string SQL_COUNT_COND = " playerAcc='{0}' and date >='{1}' and date <= '{2}'  ";

    public const string SQL_RECORD = "select date,playerOutlay,playerIncome,washCount from {0} " +
        " where date >='{1}' and date <= '{2}' and playerAcc='{3}' LIMIT {4}, {5} ";

    public override string doQuery(object param)
    {
        ParamQueryPlayerWinLose p = (ParamQueryPlayerWinLose)param;
        bool cres = createGMUser(p);
        if (!cres)
        {
            return Helper.genJsonStr(m_retData);
        }
        if (!p.checkSign(m_gmUser))
        {
            m_retData.Add("result", RetCode.RET_SIGN_ERROR);
            return Helper.genJsonStr(m_retData);
        }

        Player player = new Player(p.m_playerAcc, m_gmUser);
        if (!player.m_isExists)
        {
            m_retData.Add("result", RetCode.RET_NO_PLAYER);
            return Helper.genJsonStr(m_retData);
        }

        if (!player.isOwner(m_gmUser.m_acc))
        {
            m_retData.Add("result", RetCode.RET_NO_RIGHT);
            return Helper.genJsonStr(m_retData);
        }

        string sqlCmd = "", cond = "";
        bool code = createQueryCond(p, ref sqlCmd, ref cond);
        if (code)
        {
            m_retData.Add("result", RetCode.RET_SUCCESS);
            m_retData.Add("playerAcc", p.m_playerAcc);

            if (p.m_countEachPage == 0) // 返回条数
            {
                long count = m_gmUser.sqlDb.getRecordCount(TableName.PLAYER_WIN_LOSE, cond, MySqlDbName.DB_XIANXIA);
                m_retData.Add("totalCount", count);
            }
            else
            {
                long count = m_gmUser.sqlDb.getRecordCount(TableName.PLAYER_WIN_LOSE, cond, MySqlDbName.DB_XIANXIA);
                m_retData.Add("totalCount", count);
                m_retData.Add("curPage", p.m_curPage);
                // 单位
                m_retData.Add("moneyBase", CONST.MONEY_BASE);

                List<Dictionary<string, object>> dataList = m_gmUser.sqlDb.queryList(sqlCmd, MySqlDbName.DB_XIANXIA);
                m_retData.Add("record", dataList);
            }
        }

        return Helper.genJsonStr(m_retData);
    }

    bool createQueryCond(ParamQueryPlayerWinLose p, ref string sqlCmd, ref string condCmd)
    {
        DateTime startT, endT;
        if (!DateTime.TryParseExact(p.m_startTime, CONST.DATE_TIME_FORMAT_PART_DAY, CONST.DATE_PROVIDER, DateTimeStyles.None, out startT))
        {
            m_retData.Add("result", RetCode.RET_PARAM_NOT_VALID);
            return false;
        }
        if (!DateTime.TryParseExact(p.m_endTime, CONST.DATE_TIME_FORMAT_PART_DAY, CONST.DATE_PROVIDER, DateTimeStyles.None, out endT))
        {
            m_retData.Add("result", RetCode.RET_PARAM_NOT_VALID);
            return false;
        }

        condCmd = string.Format(SQL_COUNT_COND, p.m_playerAcc,
            startT.ToString(ConstDef.DATE_TIME24), endT.ToString(ConstDef.DATE_TIME24));

        if (p.m_countEachPage > 0)
        {
            sqlCmd = string.Format(SQL_RECORD,
                               TableName.PLAYER_WIN_LOSE,
                               startT.ToString(ConstDef.DATE_TIME24),
                               endT.ToString(ConstDef.DATE_TIME24),
                               p.m_playerAcc,
                               (p.m_curPage - 1) * p.m_countEachPage,
                               p.m_countEachPage);
        }
       
        return true;
    }
}

//////////////////////////////////////////////////////////////////////////
public class ParamQueryOrderResult : ParamBase
{
    public string m_orderId;

    public ParamQueryOrderResult()
    {
        m_fieldIndex = 2;
    }

    public override bool isParamValid()
    {
        m_playerAcc = "11";
        if (!base.isParamValid())
            return false;

        if (string.IsNullOrEmpty(m_orderId))
            return false;

        return true;
    }

    // 检测签名
    public override bool checkSign(GMUser user)
    {
        string sign = Tool.getMD5Hash(m_gmAccount + m_gmPwd + m_orderId + user.m_devSecretKey);
        return m_sign == sign;
    }
}

// 查询订单执行结果
public class QueryOrderResult : DyOpBase
{
    public const string SQL_RECORD = "select opResult,failReason from {0} " +
        " where orderId='{1}' ";

    public override string doQuery(object param)
    {
        ParamQueryOrderResult p = (ParamQueryOrderResult)param;
        bool cres = createGMUser(p);
        if (!cres)
        {
            return Helper.genJsonStr(m_retData);
        }
        if (!p.checkSign(m_gmUser))
        {
            m_retData.Add("result", RetCode.RET_SIGN_ERROR);
            return Helper.genJsonStr(m_retData);
        }

        m_retData.Add("result", RetCode.RET_SUCCESS);

        string sqlCmd = string.Format(SQL_RECORD, TableName.GM_SCORE, p.m_orderId);
        Dictionary<string, object> data = m_gmUser.sqlDb.queryOne(sqlCmd, MySqlDbName.DB_XIANXIA);
        if (data == null)
        {
            m_retData.Add("orderState", PlayerReqOrderState.STATE_PROCESSING);
        }
        else
        {
            m_retData.Add("orderState", Convert.ToInt32(data["opResult"]));
        }

        return Helper.genJsonStr(m_retData);
    }
}

//////////////////////////////////////////////////////////////////////////
// 某个API下所有玩家的输赢总和
public class ParamQueryWinLoseSum : ParamBase
{
    public string m_startTime;
    public string m_endTime;

    public ParamQueryWinLoseSum()
    {
        m_fieldIndex = 3;
    }

    public override bool isParamValid()
    {
        m_playerAcc = " ";
        if (!base.isParamValid())
            return false;

        if (string.IsNullOrEmpty(m_startTime))
            return false;

        if (string.IsNullOrEmpty(m_endTime))
            return false;

        return true;
    }

    // 检测签名
    public override bool checkSign(GMUser user)
    {
        string sign = Tool.getMD5Hash(m_gmAccount + m_gmPwd + user.m_devSecretKey);
        return m_sign == sign;
    }
}

// 某个API下所有玩家的输赢总和
public class QueryWinLoseSumAPI : DyOpBase
{
    private const string WIN_LOSE_SEARCH_CMD = "select sum(playerOutlay) as playerOutlaySum, " +
                                               "sum(playerIncome) as playerIncomeSum, sum(washCount) as washCountSum " +
                                               "from {0} where date >='{1}' and date <= '{2}' and playerCreateCode like '{3}%'  ";

    public override string doQuery(object param)
    {
        ParamQueryWinLoseSum p = (ParamQueryWinLoseSum)param;
        bool cres = createGMUser(p);
        if (!cres)
        {
            return Helper.genJsonStr(m_retData);
        }
        if (!p.checkSign(m_gmUser))
        {
            m_retData.Add("result", RetCode.RET_SIGN_ERROR);
            return Helper.genJsonStr(m_retData);
        }

        string sqlCmd = "";
        bool code = createQueryCmd(p, ref sqlCmd);
        if (code)
        {
            Dictionary<string, object> data = m_gmUser.sqlDb.queryOne(sqlCmd, MySqlDbName.DB_XIANXIA);
            if (data != null &&
                data.ContainsKey("playerOutlaySum") &&
                data.ContainsKey("playerIncomeSum") &&
                data.ContainsKey("washCountSum"))
            {
                m_retData.Add("result", RetCode.RET_SUCCESS);
                if (data["playerIncomeSum"] is DBNull)
                {
                    m_retData.Add("totalIncome", 0);
                }
                else
                {
                    m_retData.Add("totalIncome", Convert.ToInt64(data["playerIncomeSum"]));
                }

                if (data["playerOutlaySum"] is DBNull)
                {
                    m_retData.Add("totalOutlay", 0);
                }
                else
                {
                    m_retData.Add("totalOutlay", Convert.ToInt64(data["playerOutlaySum"]));
                }

                if (data["washCountSum"] is DBNull)
                {
                    m_retData.Add("washCountSum", 0);
                }
                else
                {
                    m_retData.Add("washCountSum", Convert.ToInt64(data["washCountSum"]));
                }

                // 货币单位
                m_retData.Add("moneyBase", CONST.MONEY_BASE);
            }
            else
            {
                m_retData.Add("result", RetCode.RET_DB_ERROR);
            }
        }

        return Helper.genJsonStr(m_retData);
    }

    bool createQueryCmd(ParamQueryWinLoseSum p, ref string sqlCmd)
    {
        DateTime startT, endT;
        if (!DateTime.TryParseExact(p.m_startTime, CONST.DATE_TIME_FORMAT_PART_DAY, CONST.DATE_PROVIDER, DateTimeStyles.None, out startT))
        {
            m_retData.Add("result", RetCode.RET_PARAM_NOT_VALID);
            return false;
        }
        if (!DateTime.TryParseExact(p.m_endTime, CONST.DATE_TIME_FORMAT_PART_DAY, CONST.DATE_PROVIDER, DateTimeStyles.None, out endT))
        {
            m_retData.Add("result", RetCode.RET_PARAM_NOT_VALID);
            return false;
        }

        sqlCmd = string.Format(WIN_LOSE_SEARCH_CMD,
                                TableName.PLAYER_WIN_LOSE,
                                startT.ToString(ConstDef.DATE_TIME24),
                                endT.ToString(ConstDef.DATE_TIME24),
                                m_gmUser.m_createCode);

        return true;
    }
}

//////////////////////////////////////////////////////////////////////////
public class ParamQueryWinLosePlayerList : ParamBase
{
    public string m_startTime;
    public string m_endTime;

    public ParamQueryWinLosePlayerList()
    {
        m_fieldIndex = 3;
    }

    public override bool isParamValid()
    {
        if (!base.isParamValid())
            return false;

        if (string.IsNullOrEmpty(m_startTime))
            return false;

        if (string.IsNullOrEmpty(m_endTime))
            return false;

        return true;
    }

    // 检测签名
    public override bool checkSign(GMUser user)
    {
        string sign = Tool.getMD5Hash(m_gmAccount + m_gmPwd + user.m_devSecretKey);
        return m_sign == sign;
    }

    protected override bool playerAccIsValid()
    {
        return true;
    }
}

// 某个API下每个玩家的输赢情况，输出输赢情况的列表
public class QueryWinLosePlayerListAPI : DyOpBase
{
    private const string WIN_LOSE_SEARCH_CMD = "select sum(playerOutlay) as playerOutlaySum, " +
                                               "sum(playerIncome) as playerIncomeSum, sum(washCount) as washCountSum " +
                                               "from {0} where date >='{1}' and date <= '{2}' ";

    const string SEL_PLAYER = "select acc from {0} where {1} ";
    
    public override string doQuery(object param)
    {
        ParamQueryWinLosePlayerList p = (ParamQueryWinLosePlayerList)param;
        bool cres = createGMUser(p);
        if (!cres)
        {
            return Helper.genJsonStr(m_retData);
        }
        if (!p.checkSign(m_gmUser))
        {
            m_retData.Add("result", RetCode.RET_SIGN_ERROR);
            return Helper.genJsonStr(m_retData);
        }

        string oriCmd = "";
        bool code = createQueryCmd(p, ref oriCmd);
        if (code)
        {
            List<Dictionary<string, object>> winLoseList = new List<Dictionary<string, object>>();
            calWinLose(oriCmd, winLoseList);

            m_retData.Add("result", RetCode.RET_SUCCESS);
            m_retData.Add("winLoseList", winLoseList);
            // 货币单位
            m_retData.Add("moneyBase", CONST.MONEY_BASE);
        } 
        
        return Helper.genJsonStr(m_retData);
    }

    bool createQueryCmd(ParamQueryWinLosePlayerList p, ref string sqlCmd)
    {
        DateTime startT, endT;
        if (!DateTime.TryParseExact(p.m_startTime, CONST.DATE_TIME_FORMAT_PART_DAY, CONST.DATE_PROVIDER, DateTimeStyles.None, out startT))
        {
            m_retData.Add("result", RetCode.RET_PARAM_NOT_VALID);
            return false;
        }
        if (!DateTime.TryParseExact(p.m_endTime, CONST.DATE_TIME_FORMAT_PART_DAY, CONST.DATE_PROVIDER, DateTimeStyles.None, out endT))
        {
            m_retData.Add("result", RetCode.RET_PARAM_NOT_VALID);
            return false;
        }

        sqlCmd = string.Format(WIN_LOSE_SEARCH_CMD,
                                TableName.PLAYER_WIN_LOSE,
                                startT.ToString(ConstDef.DATE_TIME24),
                                endT.ToString(ConstDef.DATE_TIME24));

        return true;
    }

    List<Dictionary<string, object>> getPlayerList()
    {
        string cmd = string.Format(SEL_PLAYER,
                                TableName.PLAYER_ACCOUNT_XIANXIA,
                                string.Format("creator='{0}' ", m_gmUser.m_acc));

        return m_gmUser.sqlDb.queryList(cmd, MySqlDbName.DB_XIANXIA);
    }

    void calWinLose(string oriCmd, List<Dictionary<string, object>> winLoseList)
    {
        string sqlCmd = "";
        List<Dictionary<string, object>> playerList = getPlayerList();
        foreach (var player in playerList)
        {
            sqlCmd = oriCmd + string.Format("  and playerAcc='{0}' ", Convert.ToString(player["acc"]));

            Dictionary<string, object> data = m_gmUser.sqlDb.queryOne(sqlCmd, MySqlDbName.DB_XIANXIA);
            if (data != null &&
                data.ContainsKey("playerOutlaySum") &&
                data.ContainsKey("playerIncomeSum") &&
                data.ContainsKey("washCountSum"))
            {
                Dictionary<string, object> tmp = new Dictionary<string, object>();
                winLoseList.Add(tmp);
                tmp.Add("playerAcc", Convert.ToString(player["acc"]));

                if (data["playerIncomeSum"] is DBNull)
                {
                    tmp.Add("totalIncome", 0);
                }
                else
                {
                    tmp.Add("totalIncome", Convert.ToInt64(data["playerIncomeSum"]));
                }

                if (data["playerOutlaySum"] is DBNull)
                {
                    tmp.Add("totalOutlay", 0);
                }
                else
                {
                    tmp.Add("totalOutlay", Convert.ToInt64(data["playerOutlaySum"]));
                }

                if (data["washCountSum"] is DBNull)
                {
                    tmp.Add("washCountSum", 0);
                }
                else
                {
                    tmp.Add("washCountSum", Convert.ToInt64(data["washCountSum"]));
                }
            }
        }
    }
}

//////////////////////////////////////////////////////////////////////////
public class ParamQueryLoginLog : ParamBase
{
    public ParamQueryLoginLog()
    {
        m_fieldIndex = 2;
    }

    public override bool isParamValid()
    {
        if (!base.isParamValid())
            return false;

        return true;
    }

    // 检测签名
    public override bool checkSign(GMUser user)
    {
        string sign = Tool.getMD5Hash(m_gmAccount + m_gmPwd + m_playerAcc + user.m_devSecretKey);
        return m_sign == sign;
    }

    protected override bool playerAccIsValid()
    {
        return true;
    }
}

// 登录日志
public class QueryLoginLog : DyOpBase
{
    static string[] FIELDS = { "acc", "ip", "time" };

    public override string doQuery(object param)
    {
        ParamQueryLoginLog p = (ParamQueryLoginLog)param;
        bool cres = createGMUser(p);
        if (!cres)
        {
            return Helper.genJsonStr(m_retData);
        }
        if (!p.checkSign(m_gmUser))
        {
            m_retData.Add("result", RetCode.RET_SIGN_ERROR);
            return Helper.genJsonStr(m_retData);
        }

        IMongoQuery imq = Query.LT("time", DateTime.Now);
        if (!string.IsNullOrEmpty(p.m_playerAcc))
        {
            IMongoQuery imq2 = Query.EQ("acc", p.m_playerAcc);
            imq = Query.And(imq, imq2);
        }

        List<Dictionary<string, object>> dataList = MongodbAcc.Instance.ExecuteGetListByQuery(TableName.PLAYER_LOGIN, imq, FIELDS, "time", false, 0, 20);
        for (int i = 0; i < dataList.Count; i++)
        {
            if (dataList[i].ContainsKey("_id"))
            {
                dataList[i].Remove("_id");
            }

            if (!string.IsNullOrEmpty(p.m_playerAcc))
            {
                dataList[i].Remove("acc");
            }
        }

        if (!string.IsNullOrEmpty(p.m_playerAcc))
        {
            m_retData.Add("acc", p.m_playerAcc);
        }
        
        m_retData.Add("loginList", dataList);
        return Helper.genJsonStr(m_retData, true);
    }
}
