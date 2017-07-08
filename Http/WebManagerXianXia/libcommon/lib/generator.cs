using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

public class DbFieldMap
{
    public string m_key;
    public FieldType m_fieldType;
    public object m_value;

    public DbFieldMap(string key, FieldType fieldType, object value)
    {
        m_key = key;
        m_fieldType = fieldType;
        m_value = value;
    }
}

// MySql更新语句生成
public class SqlUpdateGenerator
{
    public const string SQL_UPDATE = "UPDATE {0} set {1} where {2} ";

    private Dictionary<string, DbFieldMap> m_items = new Dictionary<string, DbFieldMap>();
    private StringBuilder m_textBuilder = new StringBuilder();

    public void addField(string fieldName, object value, FieldType ftype)
    {
        DbFieldMap item = new DbFieldMap(fieldName, ftype, value);
        m_items.Add(fieldName, item);
    }

    public void reset()
    {
        m_textBuilder.Remove(0, m_textBuilder.Length);
        m_items.Clear();
    }

    public int count() { return m_items.Count; }

    public string getResultSql(string tableName, string cond)
    {
        foreach (var f in m_items.Values)
        {
            string sql = genUpdateSql(f);
            m_textBuilder.Append(sql);
            m_textBuilder.Append(',');
        }
        m_textBuilder.Remove(m_textBuilder.Length - 1, 1);

        return string.Format(SQL_UPDATE, tableName, m_textBuilder.ToString(), cond);
    }

    // 生成更新语句
    private string genUpdateSql(DbFieldMap field)
    {
        switch (field.m_fieldType)
        {
            case FieldType.TypeNumber:
                {
                    return string.Format(" {0}={1} ", field.m_key, field.m_value);
                }
                break;
            case FieldType.TypeString:
                {
                    return string.Format(" {0}='{1}' ", field.m_key, field.m_value);
                }
                break;
        }

        return "";
    }
}

//////////////////////////////////////////////////////////////////////////
// MySql插入语句生成
public class SqlInsertGenerator
{
    public const string SQL_INSERT = "insert into {0} ({1}) values ({2}) ";

    private Dictionary<string, DbFieldMap> m_items = new Dictionary<string, DbFieldMap>();
    private StringBuilder m_keyBuilder = new StringBuilder();
    private StringBuilder m_valueBuilder = new StringBuilder();

    public void addField(string fieldName, object value, FieldType ftype)
    {
        DbFieldMap item = new DbFieldMap(fieldName, ftype, value);
        m_items.Add(fieldName, item);
    }

    public void reset()
    {
        m_keyBuilder.Remove(0, m_keyBuilder.Length);
        m_valueBuilder.Remove(0, m_valueBuilder.Length);
        m_items.Clear();
    }

    public int count() { return m_items.Count; }

    public string getResultSql(string tableName)
    {
        foreach (var f in m_items.Values)
        {
            m_keyBuilder.Append(f.m_key);
            m_keyBuilder.Append(',');

            string sql = genSql(f);
            m_valueBuilder.Append(sql);
            m_valueBuilder.Append(',');
        }
        m_keyBuilder.Remove(m_keyBuilder.Length - 1, 1);
        m_valueBuilder.Remove(m_valueBuilder.Length - 1, 1);

        return string.Format(SQL_INSERT, tableName, m_keyBuilder.ToString(), m_valueBuilder.ToString());
    }

    private string genSql(DbFieldMap field)
    {
        switch (field.m_fieldType)
        {
            case FieldType.TypeNumber:
                {
                    return string.Format("{0}", field.m_value);
                }
                break;
            case FieldType.TypeString:
                {
                    return string.Format("'{0}'", field.m_value);
                }
                break;
        }

        return "";
    }
}

//////////////////////////////////////////////////////////////////////////
// 查询条件构造
public class QueryCondGenerator
{
    private StringBuilder m_textBuilder = new StringBuilder();
    private List<string> m_condList = new List<string>();

    public void reset()
    {
        m_textBuilder.Remove(0, m_textBuilder.Length);
        m_condList.Clear();
    }

    // 增加一个条件。传入时，需要是一个完整的条件
    public void addCondition(string cond)
    {
        m_condList.Add(cond);
    }

    // 把所有条件用 与 连接起来
    // hasWhereKey 返回的条件中是否含有where关键字
    public string and(bool hasWhereKey = true)
    {
        if (m_condList.Count == 0)
            return "";

        m_textBuilder.Remove(0, m_textBuilder.Length);

        if (hasWhereKey)
        {
            m_textBuilder.Append(" where ");
        }
        
        m_textBuilder.Append(" ( ");
        m_textBuilder.Append(m_condList[0]);
        m_textBuilder.Append(" ) ");

        for (int i = 1; i < m_condList.Count; i++)
        {
            m_textBuilder.Append(" and ");
            m_textBuilder.Append(" ( ");
            m_textBuilder.Append(m_condList[i]);
            m_textBuilder.Append(" ) ");
        }

        return m_textBuilder.ToString();
    }
}

//////////////////////////////////////////////////////////////////////////
// MySql查询语句生成
public class SqlSelectGenerator
{
    public const string SQL_SELECT = "select {0} from {1} where {2} ";

    private List<string> m_items = new List<string>();
    private StringBuilder m_textBuilder = new StringBuilder();

    public void addField(string fieldName)
    {
        m_items.Add(fieldName);
    }

    public void reset()
    {
        m_textBuilder.Remove(0, m_textBuilder.Length);
        m_items.Clear();
    }

    public int count() { return m_items.Count; }

    public string getResultSql(string tableName, string cond)
    {
        if (m_items.Count == 0)
        {
            return string.Format(SQL_SELECT, "*", tableName, cond);
        }

        m_textBuilder.Append(m_items[0]);
        for (int i = 1; i < m_items.Count;i++ )
        {
            m_textBuilder.Append(',');
            m_textBuilder.Append(m_items[i]);
        }

        return string.Format(SQL_SELECT, m_textBuilder.ToString(), tableName, cond);
    }
}

//////////////////////////////////////////////////////////////////////////
// MySql删除语句生成
public class SqlDeleteGenerator
{
    public const string SQL_DEL = "delete from {0} where {1} ";

    public string getResultSql(string tableName, string cond)
    {
        return string.Format(SQL_DEL, tableName, cond);
    }
}

//////////////////////////////////////////////////////////////////////////
// 上下分订单生成器
public class OrderGenerator
{
    // 来自于后台操作
    public const int ORDER_FROM_BG_OP = 1;
    // 来自于API上下分订单
    public const int ORDER_FROM_API = 2;
    // 来自于玩家提交的订单
    public const int ORDER_FROM_PLAYER_ORDER = 3;

    /* 
     *          生成一个订单
     *          gmAcc           上下分的gm账号
     *          playerAcc       上下分的目标玩家账号
     *          money           上下分金额
     *          orderType       订单类型  0上分  1下分
     *          dstType         目标账号类型
     *          orderFrom       订单来源
     *          orderId         订单ID，为空时，内部生成订单id，否则直接用这个
     *          apiOrderId      api自定义的订单ID，便于对照
     *          apiCallback     上下分完成后的回调页面
     */
    public Dictionary<string, object> genOrder(string gmAcc, string playerAcc,
                                               long money, int orderType,
                                               int dstType,
                                               int orderFrom,
                                               string orderId = "",
                                               string apiOrderId = "", 
                                               string apiCallback = "")
    {
        Dictionary<string, object> data = new Dictionary<string, object>();
        data.Add("key", Guid.NewGuid().ToString());
        if (string.IsNullOrEmpty(orderId))
        {
            data.Add("orderId", genOrderId(gmAcc, playerAcc));
        }
        else
        {
            data.Add("orderId", orderId);
        }
        
        DateTime now = DateTime.Now;
        data.Add("genTime", now);
        data.Add("lastProcessTime", now);

        data.Add("gmAcc", gmAcc);
        data.Add("playerAcc", playerAcc);
        data.Add("dstType", dstType);

        data.Add("orderFrom", orderFrom);

        data.Add("money", money);
        data.Add("orderType", orderType);
        data.Add("orderState", PlayerReqOrderState.STATE_WAIT);
        data.Add("tryCount", 0);
        data.Add("failReason", 0);

        data.Add("isApi", orderFrom == ORDER_FROM_API);
        data.Add("apiOrderId", apiOrderId);
        data.Add("apiCallback", apiCallback);

        return data;
    }

    public static string genOrderId(string gmAcc, string playerAcc)
    {
        DateTime now = DateTime.Now;
        int year = now.Year;
        int month = now.Month;
        int day = now.Day;
        int hh = now.Hour;
        int minute = now.Minute;
        int second = now.Second;

        Random r = new Random();
        string str = "";
        for (int i = 0; i < 4; i++)
        {
            str += r.Next(10);
        }

        return string.Format("{0}{1:D2}{2:D2}{3:D2}{4:D2}{5:D2}{6}_{7}_{8}",
            year, month, day, hh, minute, second, str, gmAcc, playerAcc);
    }

    // 生成写入mysql完成订单的sql语句
    public static string genCmdOrderToMySql(OrderInfo info, string createCode)
    {
        SqlInsertGenerator gen = new SqlInsertGenerator();
        gen.addField("keyGUID", info.m_key, FieldType.TypeString);
        gen.addField("orderId", info.m_orderId, FieldType.TypeString);
        gen.addField("apiOrderId", info.m_apiOrderId, FieldType.TypeString);

        gen.addField("genTime", info.m_genTime.ToString(ConstDef.DATE_TIME24), FieldType.TypeString);
        // 处理时间
        gen.addField("finishTime", DateTime.Now.ToString(ConstDef.DATE_TIME24), FieldType.TypeString);

        gen.addField("gmAcc", info.m_gmAcc, FieldType.TypeString);
        gen.addField("gmCreateCode", createCode, FieldType.TypeString);

        gen.addField("playerAcc", info.m_playerAcc, FieldType.TypeString);
        gen.addField("isApi", info.m_isApi, FieldType.TypeNumber);
        gen.addField("money", info.m_money, FieldType.TypeNumber);
        gen.addField("orderType", info.m_orderType, FieldType.TypeNumber);
        gen.addField("orderState", info.m_orderState, FieldType.TypeNumber);
        gen.addField("tryCount", info.m_tryCount, FieldType.TypeNumber);
        gen.addField("failReason", info.m_failReason, FieldType.TypeNumber);

        string sqlCmd = gen.getResultSql(TableName.PLAYER_ORDER_COMPLETE);
        return sqlCmd;
    }

    // 生成写入 上下分记录的 sql语句  opSrcRemainMoney目标账号余额
    public static string genSqlForLogScore(OrderInfo info, string createCode, long opSrcRemainMoney)
    {
        SqlInsertGenerator gen = new SqlInsertGenerator();
        gen.addField("keyGUID", info.m_key, FieldType.TypeString);
        gen.addField("orderId", info.m_orderId, FieldType.TypeString);
        gen.addField("userOrderId", info.m_apiOrderId, FieldType.TypeString);

        gen.addField("opTime", info.m_genTime.ToString(ConstDef.DATE_TIME24), FieldType.TypeString);
        // 处理时间
        gen.addField("finishTime", DateTime.Now.ToString(ConstDef.DATE_TIME24), FieldType.TypeString);

        gen.addField("opSrc", info.m_gmAcc, FieldType.TypeString);
        gen.addField("opSrcCreateCode", createCode, FieldType.TypeString);
        gen.addField("opDst", info.m_playerAcc, FieldType.TypeString);
        gen.addField("opDstType", info.m_dstType, FieldType.TypeNumber);

        gen.addField("logFrom", info.m_orderFrom, FieldType.TypeNumber);
        gen.addField("opScore", info.m_money, FieldType.TypeNumber);
        gen.addField("opType", info.m_orderType, FieldType.TypeNumber);

        gen.addField("opResult", info.m_orderState, FieldType.TypeNumber);
        gen.addField("tryCount", info.m_tryCount, FieldType.TypeNumber);
        gen.addField("failReason", info.m_failReason, FieldType.TypeNumber);

        // 余额
        gen.addField("opDstRemainMoney", info.m_playerRemainMoney, FieldType.TypeNumber);

        // 操作账号余额
        gen.addField("opRemainMoney", opSrcRemainMoney, FieldType.TypeNumber);

        string sqlCmd = gen.getResultSql(TableName.GM_SCORE);
        return sqlCmd;
    }

    /* 
     *      生成玩家离线订单，订单可直接写入mysql。
     *      对于api，若玩家离线，只有成功，才生成离线订单，便于对照
     */
    public static OrderInfo genOfflineSuccessOrder(string gmAcc, 
                                                   string playerAcc,
                                                   long money,
                                                   int orderType,
                                                   int dstType,
                                                   long dstRemainMoney,
                                                   int orderFrom,
                                                   string apiOrderId = "")
    {
        OrderInfo info = new OrderInfo();
        info.m_key = Guid.NewGuid().ToString();
        info.m_orderId = genOrderId(gmAcc, playerAcc);
        info.m_apiOrderId = apiOrderId;
        info.m_genTime = DateTime.Now;
        info.m_gmAcc = gmAcc;
        info.m_playerAcc = playerAcc;
        info.m_dstType = dstType;
        info.m_isApi = (orderFrom == ORDER_FROM_API);
        info.m_money = money;
        info.m_playerRemainMoney = dstRemainMoney;
        info.m_orderType = orderType;
        info.m_orderState = PlayerReqOrderState.STATE_FINISH;
        info.m_tryCount = 0;
        info.m_orderFrom = orderFrom;
        return info;
    }
}

//////////////////////////////////////////////////////////////////////////
// 订单信息
public class OrderInfo
{
    // guid
    public string m_key;

    // 订单ID
    public string m_orderId;

    // api号可能传入一个自定义订单
    public string m_apiOrderId;

    // 订单的生成时间
    public DateTime m_genTime;

    // 订单的执行者
    public string m_gmAcc;

    // 玩家账号acc，订单执行目标。对于后台操作，并不一定是玩家，也可能是GM账号
    public string m_playerAcc;

    // 目标账号类型，是玩家还是gm账号
    public int m_dstType;

    // 是否由API号发出的
    public bool m_isApi;

    // api回调页面
    public string m_apiCallback;

    // 加的钱
    public long m_money;

    // 上下分成功后，玩家余额
    public long m_playerRemainMoney;

    // 订单类型  ScropOpType 0上分 1下分
    public int m_orderType;

    // 订单的当前状态  PlayerReqOrderState
    public int m_orderState;

    // 上次处理时间，对于失败的订单，每隔一定时间重试一次，直到被处理
    public DateTime m_lastProcessTime;

    // 订单处理失败时重新尝试处理次数
    public int m_tryCount;

    // 订单处理失败的原因   PlayerReqOrderFailReason
    public int m_failReason;

    // 订单来源
    public int m_orderFrom;
}

//////////////////////////////////////////////////////////////////////////
// 一个玩家
public class CCPlayer
{
    public static string SQL_QUERY = "SELECT acc,creator,money,state,enable,createCode,moneyOnline FROM {0} where acc='{1}' ";

    public long m_money;

    public long m_moneyOnline; // 玩家在线时金币

    public int m_state;

    public string m_owner;

    public string m_createCode;

    public bool m_enable;

    public bool m_isExists = false;

    public CCPlayer()
    {
    }

    public void init(string acc, Dictionary<string, object> r)
    {
        if (r != null)
        {
            do
            {
                string dbAcc = Convert.ToString(r["acc"]);
                if (dbAcc != acc)
                {
                    m_isExists = false;
                    break;
                }

                m_money = Convert.ToInt64(r["money"]);
                if (!(r["moneyOnline"] is DBNull))
                {
                    m_moneyOnline = Convert.ToInt64(r["moneyOnline"]);
                }

               // m_moneyOnline = Convert.ToInt64(r["moneyOnline"]);
                m_state = Convert.ToInt32(r["state"]);
                m_createCode = Convert.ToString(r["createCode"]);
                m_owner = Convert.ToString(r["creator"]);

                if (!(r["enable"] is DBNull))
                {
                    m_enable = Convert.ToBoolean(r["enable"]);
                }
                else
                {
                    m_enable = true;
                }

                m_isExists = true;
            } while (false);
        }
    }

    public bool isInGame()
    {
        return m_state == PlayerState.STATE_GAME;
    }

    public bool isOwner(string owner)
    {
        return m_owner == owner;
    }

    // 账号是否停封
    public bool isAccStop()
    {
        return !m_enable;
    }

    // 余额是否充足
    public bool isMoneyEnough(long score)
    {
        long pm = 0;
        if (isInGame())
        {
            pm = m_moneyOnline;
        }
        else
        {
            pm = m_money;
        }

        if (pm < score)
            return false;

        return true;
    }
}

//////////////////////////////////////////////////////////////////////////
// 循环查询订单状态，共查10次
public class QueryOrderState
{
    public const string SQL_RECORD = "select opResult,failReason from {0} " +
        " where orderId='{1}' ";

    public delegate Dictionary<string, object> SearchFun(string sql, int dbName);

    // delta 毫秒
    public int queryOrderState(string orderId, SearchFun f, int totalCount, double delta)
    {
        string sqlCmd = string.Format(SQL_RECORD, TableName.GM_SCORE, orderId);
        Stopwatch watch = new Stopwatch();
        watch.Start();
        int curCount = 0;
        while (curCount < totalCount)
        {
            if (watch.ElapsedMilliseconds > delta)
            {
                Dictionary<string, object> data = f(sqlCmd, CMySqlDbName.DB_XIANXIA);

                if (data != null)
                {
                    return Convert.ToInt32(data["opResult"]);
                }
                curCount++;
                watch.Restart();
            }
        }
        return PlayerReqOrderState.STATE_PROCESSING;
    }
}

//////////////////////////////////////////////////////////////////////////
// 玩家订单项
public class ResultPlayerOrderItem
{
    public string m_orderId;
    public string m_orderTime;
    public string m_playerAcc;
    public string m_playerOwner;
    public string m_playerOwnerCreator;

    public int m_orderState;
    public int m_orderMoney;
    public int m_orderType;
}

// 玩家订单的处理
public class CPlayerOrder
{
    // orderState OrderState类型
    public static string genInsertSql(ResultPlayerOrderItem item, string opAcc, int orderState)
    {
        SqlInsertGenerator gen = new SqlInsertGenerator();
        gen.addField("orderId", item.m_orderId, FieldType.TypeString);
        gen.addField("orderTime", item.m_orderTime, FieldType.TypeString);
        gen.addField("playerAcc", item.m_playerAcc, FieldType.TypeString);
        gen.addField("playerOwner", item.m_playerOwner, FieldType.TypeString);
        gen.addField("curOpAcc", opAcc, FieldType.TypeString);
        gen.addField("orderState", orderState, FieldType.TypeNumber);
        gen.addField("playerOwnerCreator", item.m_playerOwnerCreator, FieldType.TypeString);
        gen.addField("orderMoney", item.m_orderMoney, FieldType.TypeNumber);
        gen.addField("orderType", item.m_orderType, FieldType.TypeNumber);

        string cmd = gen.getResultSql(TableName.PLAYER_ORDER_FINISH);
        return cmd;
    }

    // orderState OrderState类型
    public static string genUpdateSql(string orderId, int orderState)
    {
        SqlUpdateGenerator up = new SqlUpdateGenerator();
        up.addField("orderState", orderState, FieldType.TypeNumber);

        string cmd = up.getResultSql(TableName.PLAYER_ORDER_WAIT, string.Format("orderId='{0}' ", orderId));
        return cmd;
    }

    public static string genRemoveSql(string orderId)
    {
        SqlDeleteGenerator sql = new SqlDeleteGenerator();
        string cmd = sql.getResultSql(TableName.PLAYER_ORDER_WAIT, string.Format("orderId='{0}' ", orderId));
        return cmd;
    }

    public static ResultPlayerOrderItem toOrder(Dictionary<string, object> data)
    {
        if (data == null)
            return null;

        ResultPlayerOrderItem info = new ResultPlayerOrderItem();

        info.m_orderId = Convert.ToString(data["orderId"]);
        info.m_orderTime = Convert.ToDateTime(data["orderTime"]).ToString(ConstDef.DATE_TIME24);
        info.m_playerAcc = Convert.ToString(data["playerAcc"]);
        info.m_playerOwner = Convert.ToString(data["playerOwner"]);
        info.m_playerOwnerCreator = Convert.ToString(data["playerOwnerCreator"]);
        info.m_orderState = Convert.ToInt32(data["orderState"]);
        info.m_orderMoney = Convert.ToInt32(data["orderMoney"]);
        info.m_orderType = Convert.ToInt32(data["orderType"]);

        return info;
    }
}


