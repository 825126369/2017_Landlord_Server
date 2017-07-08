using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System.Text.RegularExpressions;
using System.Web.Configuration;
using System.Linq;

public class ParamMemberInfo : ParamQueryBase
{
    public string m_time;

    // 查某具体的账号
    public string m_acc;

    // 创建者，查看的是这个创建者的下级
    public string m_creator;
    // 创建者所在层次
    public int m_creatorDepth;

    // 搜索深度 0搜索全部的层次  1只从本账号向下搜索1层，目前只用到0，1两个值
    public int m_searchDepth;

    // 查询结果是否包括子账号， 0全部结果，不管有没有子账号 1 不包括子账号 2 只搜索子账号
    public int m_subAcc;

    // 搜索标志，不同的搜索可能含义不同。通过扩展方法区别
    public int m_flag;

    public ParamMemberInfo()
    {
        m_subAcc = 1;
    }

    public bool isSearchAll()
    {
        return m_searchDepth == 0;
    }

    public string getRootUser(GMUser user)
    {
        if (!string.IsNullOrEmpty(m_acc))
            return m_acc;

        if (string.IsNullOrEmpty(m_creator))
        {
            return user.m_user;
        }

        return m_creator;
    }

    public string m_resultCond;
}

public class MemberInfo
{
    // 创建时间
    public string m_createTime;
    // 账号
    public string m_acc;
    // 创建者
    public string m_owner;
    // 所属总代理
    public string m_generalAgency;
    public string m_gmRight;
    // 开发密钥
    public string m_devKey;

    public long m_money;
    public int m_moneyType;
    public int m_accType;
    // 所处状态
    public int m_state;

    // 别名
    public string m_aliasName = "";

    public string m_lastLoginDate = "";

    // 最后登录IP
    public string m_lastLoginIP = "";

    public double m_washRatio;         // 洗码比
    public double m_agentRatio;        // 代理占成

    public int m_depth;

    // 是否影响盈利率
    public bool m_isAffectRate;

    public string getRightName()
    {
        if (m_accType == AccType.ACC_AGENCY)
        {
            string str1 = "", str2 = "";
            if (RightMap.hasRight(RIGHT.CREATE_AGENCY, m_gmRight))
            {
                str1 = "yes";
            }
            else
            {
                str1 = "no";
            }
            if (RightMap.hasRight(RIGHT.CREATE_API, m_gmRight))
            {
                str2 = "yes";
            }
            else
            {
                str2 = "no";
            }
            return string.Format(StrName.s_rightDesc, str1, str2);
        }
        return "";
    }
}

// 查询玩家会员
public class QueryPlayerMember : QueryBase
{
    private List<MemberInfo> m_result = new List<MemberInfo>();
    private CommonSearchCmdBase m_generator = new CommonSearchCmdBase(CommonSearchCmdBase.SEARCH_TYPE_PLAYER);
    static string[] s_fields = { "NotSaveRate" };

    public override OpRes doQuery(object param, GMUser user)
    {
        m_result.Clear();
        ParamMemberInfo p = (ParamMemberInfo)param;
        return query(user, p);
    }

    public override object getQueryResult()
    {
        return m_result;
    }

    private OpRes query(GMUser user, ParamMemberInfo p)
    {
        string cmd = "";
        OpRes res = m_generator.genSearchSql(p, user, ref cmd);
        if (res != OpRes.opres_success)
            return res;

        if (p.m_countEachPage > 0 && p.m_curPage > 0)
        {
            cmd += string.Format(" LIMIT {0}, {1}", (p.m_curPage - 1) * p.m_countEachPage, p.m_countEachPage);
            user.totalRecord = user.sqlDb.getRecordCount(TableName.PLAYER_ACCOUNT_XIANXIA,
                p.m_resultCond, user.getMySqlServerID(), MySqlDbName.DB_XIANXIA);
        }

        List<Dictionary<string, object>> dataList =
            user.sqlDb.queryList(cmd, user.getMySqlServerID(), MySqlDbName.DB_XIANXIA);
        if (dataList == null)
            return OpRes.op_res_failed;

        // 账号所在数据库ID
        int accServerId = -1;
        if (user.isAdmin() || user.isGeneralAgency())
        {
            // 账号所在数据库ID
            accServerId = DBMgr.getInstance().getSpecialServerId(DbName.DB_ACCOUNT);
        }

        for (int i = 0; i < dataList.Count; i++)
        {
            MemberInfo info = new MemberInfo();
            m_result.Add(info);
            Dictionary<string, object> data = dataList[i];
            info.m_createTime = Convert.ToString(data["createTime"]);
            info.m_acc = Convert.ToString(data["acc"]);
            info.m_accType = AccType.ACC_PLAYER;
            info.m_owner = Convert.ToString(data["creator"]);
            info.m_moneyType = Convert.ToInt32(data["moneyType"]);
            info.m_state = Convert.ToInt32(data["state"]);
            if (info.m_state == PlayerState.STATE_IDLE) // 离线
            {
                info.m_money = Convert.ToInt64(data["money"]);
            }
            else // 在线
            {
                if (!(data["moneyOnline"] is DBNull))
                {
                    info.m_money = Convert.ToInt64(data["moneyOnline"]);
                }
            }

            if (!(data["aliasName"] is DBNull))
            {
                info.m_aliasName = Convert.ToString(data["aliasName"]);
            }

            if (!(data["lastLoginDate"] is DBNull))
            {
                info.m_lastLoginDate = Convert.ToDateTime(data["lastLoginDate"]).ToString();
            }

            if (!(data["enable"] is DBNull))
            {
                bool enable = Convert.ToBoolean(data["enable"]);
                if (!enable)
                {
                    info.m_state = PlayerState.STATE_BLOCK;
                }
            }

            if (accServerId >= 0)
            {
                // 查询最后登录IP
               /* Dictionary<string, object> retData = DBMgr.getInstance().getTableData(TableName.PLAYER_ACCOUNT,
                                                 "acc", info.m_acc, s_fields, accServerId, DbName.DB_ACCOUNT);
                if (retData != null)
                {
                    if (retData.ContainsKey("lastip"))
                    {
                        info.m_lastLoginIP = Convert.ToString(retData["lastip"]);
                    }
                }*/
            }

            if (!(data["playerWashRatio"] is DBNull))
            {
                info.m_washRatio = Convert.ToInt32(data["playerWashRatio"]);
            }

            Dictionary<string, object> retData = QueryBase.getPlayerPropertyByAcc(info.m_acc, user, s_fields);
            if (retData != null)
            {
                if (retData.ContainsKey("NotSaveRate"))
                {
                    info.m_isAffectRate = !Convert.ToBoolean(retData["NotSaveRate"]);
                }
                else
                {
                    info.m_isAffectRate = true;
                }
            }
            else
            {
                info.m_isAffectRate = true;
            }
        }
        
        return OpRes.opres_success;
    }
}

//////////////////////////////////////////////////////////////////////////

// 所创建的GM账号的查询
public class QueryGmAccount : QueryBase
{
    private List<MemberInfo> m_result = new List<MemberInfo>();
    private CommonSearchCmdBase m_generator = new CommonSearchCmdBase(CommonSearchCmdBase.SEARCH_TYPE_GM);

    public override OpRes doQuery(object param, GMUser user)
    {
        m_result.Clear();
        if (!RightMap.hasRight(user.m_accType, RIGHT.VIEW_AGENCY, user.m_right))
            return OpRes.op_res_no_right;

        ParamMemberInfo p = (ParamMemberInfo)param;

        string cmd = "";
        OpRes res = m_generator.genSearchSql(p, user, ref cmd);
        if (res != OpRes.opres_success)
            return res;

        List<Dictionary<string, object>> dataList = user.sqlDb.queryList(cmd,
            user.getMySqlServerID(), MySqlDbName.DB_XIANXIA);
        if (dataList == null)
            return OpRes.op_res_failed;

        for (int i = 0; i < dataList.Count; i++)
        {
            MemberInfo info = new MemberInfo();
            m_result.Add(info);

            Dictionary<string, object> data = dataList[i];
            info.m_createTime = Convert.ToString(data["createTime"]);
            info.m_acc = Convert.ToString(data["acc"]);
            info.m_money = Convert.ToInt64(data["money"]);
            info.m_moneyType = Convert.ToInt32(data["moneyType"]);
            info.m_accType = Convert.ToInt32(data["accType"]);
            info.m_owner = Convert.ToString(data["owner"]);
            info.m_generalAgency = Convert.ToString(data["generalAgency"]);
            info.m_gmRight = Convert.ToString(data["gmRight"]);
            info.m_state = Convert.ToInt32(data["state"]);

            if (!(data["aliasName"] is DBNull))
            {
                info.m_aliasName = Convert.ToString(data["aliasName"]);
            }

//             if (user.m_accType == AccType.ACC_API)
//             {
//                 info.m_devKey = Convert.ToString(data["devSecretKey"]);
//             }

            if (user.isAdmin() || user.isGeneralAgency())
            {
                /*if (data.ContainsKey("lastLoginIP"))
                {
                    if (!(data["lastLoginIP"] is DBNull))
                    {
                        info.m_lastLoginIP = Convert.ToString(data["lastLoginIP"]);
                    }
                }*/
            }

            if (!(data["agentRatio"] is DBNull))
            {
                info.m_agentRatio = Convert.ToDouble(data["agentRatio"]);
            }
            if (!(data["washRatio"] is DBNull))
            {
                info.m_washRatio = Convert.ToDouble(data["washRatio"]);
            }

            info.m_depth = Convert.ToInt32(data["depth"]);
        }

        return OpRes.opres_success;
    }

    public override object getQueryResult()
    {
        return m_result;
    }
}

//////////////////////////////////////////////////////////////////////////

public class MemberInfoDetail : MemberInfo
{
    public string m_name = "";
    public string m_birthDay = "";
    public int m_sex;
    public string m_home = "";
    public string m_gameClose = "";
    public string m_selfComment = "";

    // api后缀
    public string m_apiPostfix;
    public long m_gmId;

    public void reset()
    {
        m_home = m_gameClose = "";
    }
}

// 所创建的GM账号详细信息
public class QueryGmAccountDetail : QueryBase
{
    private const string GM_SEARCH_CMD = "select * from {0} where {1} ";

    MemberInfoDetail m_result = new MemberInfoDetail();

    public override OpRes doQuery(object param, GMUser user)
    {
        m_result.reset();

        ParamMemberInfo p = (ParamMemberInfo)param;
        string cmd = string.Format(GM_SEARCH_CMD, TableName.GM_ACCOUNT, string.Format(" acc='{0}' ", p.m_acc));

        List<Dictionary<string, object>> dataList = user.sqlDb.queryList(cmd,
            user.getMySqlServerID(), MySqlDbName.DB_XIANXIA);
        if (dataList == null)
            return OpRes.op_res_failed;

        for (int i = 0; i < dataList.Count; i++)
        {
            Dictionary<string, object> data = dataList[i];
            m_result.m_createTime = Convert.ToString(data["createTime"]);
            m_result.m_acc = Convert.ToString(data["acc"]);
            m_result.m_money = Convert.ToInt64(data["money"]);
            m_result.m_moneyType = Convert.ToInt32(data["moneyType"]);
            m_result.m_accType = Convert.ToInt32(data["accType"]);
            m_result.m_owner = Convert.ToString(data["owner"]);
            m_result.m_generalAgency = Convert.ToString(data["generalAgency"]);
            m_result.m_gmRight = Convert.ToString(data["gmRight"]);

            if (user.m_accType == AccType.ACC_API)
            {
                m_result.m_devKey = Convert.ToString(data["devSecretKey"]);
                m_result.m_apiPostfix = Convert.ToString(data["postfix"]);
            }

            if (data.ContainsKey("gmId"))
            {
                m_result.m_gmId = Convert.ToInt64(data["gmId"]) + ConstDef.GM_ID_OFFSET;
            }
            if (!(data["home"] is DBNull))
            {
                m_result.m_home = Convert.ToString(data["home"]);
            }
            if (!(data["aliasName"] is DBNull))
            {
                m_result.m_aliasName = Convert.ToString(data["aliasName"]);
            }
            if (!(dataList[i]["gameClose"] is DBNull))
            {
                m_result.m_gameClose = Convert.ToString(dataList[i]["gameClose"]);
            }
            if (!(data["agentRatio"] is DBNull))
            {
                m_result.m_agentRatio = Convert.ToDouble(data["agentRatio"]);
            }
            if (!(data["washRatio"] is DBNull))
            {
                m_result.m_washRatio = Convert.ToDouble(data["washRatio"]);
            }
            /*m_result.m_name = Convert.ToString(dataList[i]["name"]);
            if (!(dataList[i]["birthDay"] is DBNull))
            {
                m_result.m_birthDay = Convert.ToDateTime(dataList[i]["birthDay"]).ToShortDateString();
            }

            if (!(dataList[i]["sex"] is DBNull))
            {
                m_result.m_sex = Convert.ToInt32(dataList[i]["sex"]);
            }

            if (!(dataList[i]["city"] is DBNull))
            {
                m_result.m_city = Convert.ToString(dataList[i]["city"]);
            }

            if (!(dataList[i]["selfComment"] is DBNull))
            {
                m_result.m_selfComment = Convert.ToString(dataList[i]["selfComment"]);
            }*/
        }

        return OpRes.opres_success;
    }

    public override object getQueryResult()
    {
        return m_result;
    }
}

//////////////////////////////////////////////////////////////////////////

public class ParamGmAccountCascade
{
    // 查询的账号类型
    public int m_searchAccType;

    // 创建者，为空表示查所有
    public string m_owner = "";
}

// 级联
public class QueryGmAccountCascade : QueryBase
{
    private List<MemberInfo> m_result = new List<MemberInfo>();

    public override OpRes doQuery(object param, GMUser user)
    {
        m_result.Clear();
        ParamGmAccountCascade p = (ParamGmAccountCascade)param;
        string cmd = 
            string.Format("select acc from {0} where accType={1} and owner='{2}'",
            TableName.GM_ACCOUNT,
            p.m_searchAccType,
            p.m_owner);

        List<Dictionary<string, object>> dataList = user.sqlDb.queryList(cmd, user.getMySqlServerID(), MySqlDbName.DB_XIANXIA);
        if (dataList == null)
            return OpRes.op_res_failed;

        for (int i = 0; i < dataList.Count; i++)
        {
            MemberInfo info = new MemberInfo();
            m_result.Add(info);
            info.m_acc = Convert.ToString(dataList[i]["acc"]);
        }

        return OpRes.opres_success;
    }

    public override object getQueryResult()
    {
        return m_result;
    }
}

//////////////////////////////////////////////////////////////////////////
public class ResultScoreOpRecordItem
{
    public long m_id;
    public string m_opTime;
    public string m_opAcc;
    public string m_opAccAlias;
    public int m_opType;
    public long m_opScore;
    public string m_dstAcc;
    public int m_moneyType;
    // 目标账号余额
    public long m_dstRemainMoney;
    // 操作账号余额
    public long m_opRemainMoney = -1;

    public string m_orderId;
    public string m_apiOrder;
    public int m_logFrom;
    public string m_finishTime = "";
    public int m_opResult;
    public int m_failReason;
}

public class ParamScoreOpRecord : ParamQueryBase
{
    public string m_time;   // 时间段
    public string m_opAcc;  // 操作人
    public string m_dstAcc;  // 目标账号
    public int m_orderState; // 订单状态
}

// 查询上分下分操作记录
public class QueryScoreOpRecord : QueryBase
{
    private List<ResultScoreOpRecordItem> m_result = new List<ResultScoreOpRecordItem>();
  //  private CommonSearchCmdBase m_generator = new CommonSearchCmdBase(CommonSearchCmdBase.SEARCH_TYPE_GM);

    QueryCondition m_cond = new QueryCondition();

    public override OpRes doQuery(object param, GMUser user)
    {
        m_result.Clear();
        m_cond.startQuery();
        OpRes res = makeQuery(param, user, m_cond);
        if (res != OpRes.opres_success)
            return res;

        ParamScoreOpRecord p = (ParamScoreOpRecord)param;
        string cond = Convert.ToString(m_cond.getCond()["cond"]);

        string cmd = string.Format(SqlStrCMD.SQL_QUERY_SCORE_OP, 
            TableName.GM_SCORE,
            TableName.GM_ACCOUNT, 
            cond,
            (p.m_curPage - 1) * p.m_countEachPage,
            p.m_countEachPage);

        // 查看满足条件的记当个数
        user.totalRecord = user.sqlDb.getRecordCountNoWhere(TableName.GM_SCORE + "," + TableName.GM_ACCOUNT,
            cond, user.getMySqlServerID(), MySqlDbName.DB_XIANXIA);

        List<Dictionary<string, object>> dataList = user.sqlDb.queryList(cmd,
            user.getMySqlServerID(), MySqlDbName.DB_XIANXIA);
        if (dataList == null)
            return OpRes.op_res_failed;

        for (int i = 0; i < dataList.Count; i++)
        {
            ResultScoreOpRecordItem info = new ResultScoreOpRecordItem();
            m_result.Add(info);

            Dictionary<string, object> data = dataList[i];
            info.m_id = Convert.ToInt64(data["opId"]);
            info.m_opTime = Convert.ToDateTime(data["opTime"]).ToString();
            info.m_opAcc = Convert.ToString(data["opSrc"]);
            info.m_opAccAlias = "";
            info.m_opType = Convert.ToInt32(data["opType"]);
            info.m_opScore = Convert.ToInt64(data["opScore"]);
            info.m_dstAcc = Convert.ToString(data["opDst"]);
           // info.m_moneyType = Convert.ToInt32(data["moneyType"]);

            if (!(data["aliasName"] is DBNull))
            {
                info.m_opAccAlias = Convert.ToString(data["aliasName"]);
            }

            if (!(data["opDstRemainMoney"] is DBNull))
            {
                info.m_dstRemainMoney = Convert.ToInt64(data["opDstRemainMoney"]);
            }
            if (!(data["opRemainMoney"] is DBNull))
            {
                info.m_opRemainMoney = Convert.ToInt64(data["opRemainMoney"]);
            }

            //////////////////////////////////////////////////////////////////////////
            if (!(data["userOrderId"] is DBNull))
            {
                info.m_apiOrder = Convert.ToString(data["userOrderId"]);
            }

            if (!(data["opResult"] is DBNull))
            {
                info.m_opResult = Convert.ToInt32(data["opResult"]);
            }
            else
            {
                info.m_opResult = PlayerReqOrderState.STATE_FINISH;
            }

            if (!(data["failReason"] is DBNull))
            {
                info.m_failReason = Convert.ToInt32(data["failReason"]);
            }
            if (!(data["logFrom"] is DBNull))
            {
                info.m_logFrom = Convert.ToInt32(data["logFrom"]);
            }
            else
            {
                info.m_logFrom = OrderGenerator.ORDER_FROM_BG_OP;
            }
            if (!(data["finishTime"] is DBNull))
            {
                info.m_finishTime = Convert.ToDateTime(data["finishTime"]).ToString();
            }
            if (!(data["orderId"] is DBNull))
            {
                info.m_orderId = Convert.ToString(data["orderId"]);
            }
        }

        return OpRes.opres_success;
    }

    public override object getQueryResult()
    {
        return m_result;
    }

    public override OpRes makeQuery(object param, GMUser user, QueryCondition imq) 
    {
        ParamScoreOpRecord p = (ParamScoreOpRecord)param;

        string cond = "";
        OpRes res = getQueryCond(p, user, ref cond, imq);
        if (res == OpRes.opres_success)
        {
            imq.addCond("cond", cond);
            if (imq.isExport())
            {
                imq.addCond("moneyBase", DefCC.MONEY_BASE);
            }
        }

        return res;
    }

    // 构造查询条件
    OpRes getQueryCond(ParamScoreOpRecord p, GMUser user, ref string condStr, QueryCondition queryCond)
    {
        DateTime mint = DateTime.Now, maxt = mint;
        bool useTime = false;
        if (!string.IsNullOrEmpty(p.m_time))
        {
            useTime = Tool.splitTimeStr(p.m_time, ref mint, ref maxt);
            if (!useTime)
                return OpRes.op_res_time_format_error;
        }

        QueryCondGenerator gen = new QueryCondGenerator();

        gen.addCondition(string.Format(" opSrcCreateCode like '{0}%' ", ItemHelp.getCreateCodeSpecial(user)));

        if (!string.IsNullOrEmpty(p.m_opAcc))
        {
            gen.addCondition(string.Format("opSrc='{0}'", p.m_opAcc));

            if (queryCond.isExport())
            {
                queryCond.addCond("opSrc", p.m_opAcc);
            }
        }
        if (!string.IsNullOrEmpty(p.m_dstAcc))
        {
            gen.addCondition(string.Format("opDst='{0}'", p.m_dstAcc));

            if (queryCond.isExport())
            {
                queryCond.addCond("opDst", p.m_dstAcc);
            }
        }
        if (useTime)
        {
            gen.addCondition(string.Format(" opTime>='{0}' and opTime<'{1}' ", mint.ToString(ConstDef.DATE_TIME24),
                maxt.ToString(ConstDef.DATE_TIME24)));

            if(queryCond.isExport())
            {
                queryCond.addCond("time", p.m_time);
            }
        }

        if (p.m_orderState > -1)
        {
            gen.addCondition(string.Format("opResult={0}", p.m_orderState));

            if (queryCond.isExport())
            {
                queryCond.addCond("opResult", p.m_orderState);
            }
        }

        gen.addCondition(string.Format(" {0}.opSrc={1}.acc", TableName.GM_SCORE, TableName.GM_ACCOUNT));
        condStr = gen.and();

        return OpRes.opres_success;
    }
}

//////////////////////////////////////////////////////////////////////////
public class ParamQueryOpLog : ParamQuery
{
    public int m_logType;
}

public class ResultOpLogItem // 操作日志一项
{
    public long m_id;           // 操作ID
    public string m_opAcc;      // 操作账号
    public string m_opAccIP;    // 操作账号IP
    public string m_opName;     // 操作名称
    public string m_opDateTime; // 操作时间
    public string m_opDesc;     // 操作描述串
    public string m_comment;    // 注释
}

// 查询操作日志
public class QueryOpLog : QueryBase
{
    public const string SQL_QUERY_LOG = " SELECT {0}.* from {0},{1} {2}" +
                                        " order by opTime desc LIMIT {3}, {4} ";

    public const string SQL_COUNT = "SELECT COUNT(*) as cnt FROM {0},{1} {2}";

    List<ResultOpLogItem> m_result = new List<ResultOpLogItem>();

    public override OpRes doQuery(object param, GMUser user)
    {
        ParamQueryOpLog p = (ParamQueryOpLog)param;
        string cond = null;
        OpRes res = genQueryCond(p, user, ref cond);
        if (res != OpRes.opres_success)
            return res;

        m_result.Clear();

        string sqlCount = string.Format(SQL_COUNT,
                                    TableName.OPLOG,
                                    TableName.GM_ACCOUNT,
                                    cond);
        // 查看满足条件的记当个数
        user.totalRecord = user.sqlDb.getRecordCount(sqlCount,
                                                     user.getMySqlServerID(),
                                                     MySqlDbName.DB_XIANXIA);

        string cmd = string.Format(SQL_QUERY_LOG,
                                    TableName.OPLOG,
                                    TableName.GM_ACCOUNT,
                                    cond,
                                    (p.m_curPage - 1) * p.m_countEachPage,
                                    p.m_countEachPage);

        List<Dictionary<string, object>> dataList = user.sqlDb.queryList(cmd,
                                                                        user.getMySqlServerID(),
                                                                        MySqlDbName.DB_XIANXIA);

        if (dataList == null)
            return OpRes.op_res_failed;

        for (int i = 0; i < dataList.Count; i++)
        {
            ResultOpLogItem info = new ResultOpLogItem();
            m_result.Add(info);

            Dictionary<string, object> data = dataList[i];
            info.m_id = Convert.ToInt64(data["opId"]);
            info.m_opAcc = Convert.ToString(data["opAcc"]);
            info.m_opAccIP = Convert.ToString(data["opAccIP"]);
            info.m_opDateTime = Convert.ToDateTime(data["opTime"]).ToString();
            info.m_comment = Convert.ToString(data["opComment"]);

            OpInfo opInfo = OpLogMgr.getInstance().getOpInfo(Convert.ToInt32(data["opType"]));
            if (opInfo != null)
            {
                info.m_opName = opInfo.m_opName;
                info.m_opDesc = opInfo.m_param.getDescription(opInfo, Convert.ToString(data["opParam"]));
            }
            else
            {
                info.m_opName = info.m_opDesc = "";
            }
        }

        return OpRes.opres_success;
    }

    // 返回查询结果
    public override object getQueryResult()
    {
        return m_result;
    }

    private OpRes genQueryCond(ParamQueryOpLog p, GMUser user, ref string resultCond)
    {
        QueryCondGenerator gen = new QueryCondGenerator();
        if (p.m_logType != 0)
        {
            gen.addCondition(string.Format("opType={0}", p.m_logType));
        }
        if (!string.IsNullOrEmpty(p.m_time))
        {
            DateTime mint = DateTime.Now, maxt = DateTime.Now;
            bool res = Tool.splitTimeStr(p.m_time, ref mint, ref maxt);
            if (!res)
                return OpRes.op_res_time_format_error;

            string cond = string.Format("opTime>='{0}' and opTime<'{1}'", mint, maxt);
            gen.addCondition(cond);
        }

        gen.addCondition(string.Format("opAcc=acc and createCode like '{0}%' ", user.m_createCode));
        resultCond = gen.and();
        return OpRes.opres_success;
    }
}

//////////////////////////////////////////////////////////////////////////
public class ResultAPIItem 
{
    public string m_apiAcc;         
    public string m_apiPwd;         
    public string m_genTime;        
    public string m_apiCreator;     
    public string m_apiAliasName;   
    public double m_apiAgentRatio;
    public double m_apiWashRatio;
    public string m_apiPrefix;
}

// api审批查询
public class QueryApiApprove : QueryBase
{
    public const string SQL_QUERY_API = " SELECT * from {0} {1}" +
                                        " order by genTime desc  ";

    List<ResultAPIItem> m_result = new List<ResultAPIItem>();

    public override OpRes doQuery(object param, GMUser user)
    {
        OpRes code = canApprove(user);
        if (code != OpRes.opres_success)
            return code;

        m_result.Clear();
        string cond = genCond(param, user);
        string cmd = string.Format(SQL_QUERY_API,
                                    TableName.API_APPROVE,
                                    cond);

        List<Dictionary<string, object>> dataList = user.sqlDb.queryList(cmd,
                                                                        user.getMySqlServerID(),
                                                                        MySqlDbName.DB_XIANXIA);

        if (dataList == null || dataList.Count == 0)
            return OpRes.op_res_not_found_data;

        for (int i = 0; i < dataList.Count; i++)
        {
            ResultAPIItem info = new ResultAPIItem();
            m_result.Add(info);

            Dictionary<string, object> data = dataList[i];
            info.m_apiAcc = Convert.ToString(data["apiAcc"]);
            info.m_apiPwd = Convert.ToString(data["apiPwd"]);
            info.m_genTime = Convert.ToDateTime(data["genTime"]).ToString(ConstDef.DATE_TIME24);
            info.m_apiCreator = Convert.ToString(data["apiCreator"]);
            info.m_apiAliasName = Convert.ToString(data["apiAliasName"]);
            info.m_apiAgentRatio = Convert.ToDouble(data["apiAgentRatio"]);
            info.m_apiWashRatio = Convert.ToDouble(data["apiWashRatio"]);
            info.m_apiPrefix = Convert.ToString(data["apiPrefix"]);
        }

        return OpRes.opres_success;
    }

    // 返回查询结果
    public override object getQueryResult()
    {
        return m_result;
    }

    OpRes canApprove(GMUser user)
    {
        if (RightMap.hasRight(user.m_accType, RIGHT.APPROVE_API, user.m_right))
            return OpRes.opres_success;

        return OpRes.op_res_failed;
    }

    string genCond(object param, GMUser user)
    {
        QueryCondGenerator gen = new QueryCondGenerator();
        gen.addCondition(string.Format("apiCreatorCode like '{0}%' ",
                                    user.m_createCode));
        if (param != null)
        {
            string acc = (string)param;
            gen.addCondition(string.Format("apiAcc='{0}'", acc));
        }

        return gen.and();
    }
}

//////////////////////////////////////////////////////////////////////////
public class ParamPlayerOrderQuery
{
    // 操作 0 查询指定状态的订单  1查询某具体订单
    public int m_op;

    public int m_orderState;

    public string m_orderId;
    public string m_playerAcc;
}

// 查询玩家订单
public class QueryPlayerOrder : QueryBase
{
    public const string SQL_QUERY_ORDER = " SELECT * from {0} where {1}" +
                                        " order by orderTime desc  ";

    List<ResultPlayerOrderItem> m_result = new List<ResultPlayerOrderItem>();

    public override OpRes doQuery(object param, GMUser user)
    {
        ParamPlayerOrderQuery p = (ParamPlayerOrderQuery)param;
        m_result.Clear();
        string cmd = genOrderSqlCmd(p, user);

        List<Dictionary<string, object>> dataList = user.sqlDb.queryList(cmd,
                                                                        user.getMySqlServerID(),
                                                                        MySqlDbName.DB_XIANXIA);

        if (dataList == null)
            return OpRes.op_res_failed;

        for (int i = 0; i < dataList.Count; i++)
        {
            ResultPlayerOrderItem info = new ResultPlayerOrderItem();
            m_result.Add(info);

            Dictionary<string, object> data = dataList[i];
            info.m_orderId = Convert.ToString(data["orderId"]);
            info.m_orderTime = Convert.ToDateTime(data["orderTime"]).ToString(ConstDef.DATE_TIME24);
            info.m_playerAcc = Convert.ToString(data["playerAcc"]);
            info.m_playerOwner = Convert.ToString(data["playerOwner"]);
            info.m_playerOwnerCreator = Convert.ToString(data["playerOwnerCreator"]);
            info.m_orderState = Convert.ToInt32(data["orderState"]);
            info.m_orderMoney = Convert.ToInt32(data["orderMoney"]);
            info.m_orderType = Convert.ToInt32(data["orderType"]);
        }

        return OpRes.opres_success;
    }

    // 返回查询结果
    public override object getQueryResult()
    {
        return m_result;
    }

    // 返回查询语句
    string genOrderSqlCmd(ParamPlayerOrderQuery p, GMUser user)
    {
        string cmd = "";
        string tableName = "";
        switch (p.m_orderState)
        {
            case OrderState.STATE_WAIT:
                {
                    tableName = TableName.PLAYER_ORDER_WAIT;
                }
                break;
            default:
                {
                    tableName = TableName.PLAYER_ORDER_FINISH;
                }
                break;
        }

        switch (p.m_op)
        {
            case 0: // 查询指定状态订单
                {
                    cmd = string.Format(SQL_QUERY_ORDER,
                            tableName,
                            string.Format(" curOpAcc='{0}' and orderState='{1}' ",
                            user.m_user, p.m_orderState));

                }
                break;
            case 1:
                {
                    cmd = string.Format(SQL_QUERY_ORDER,
                           tableName,
                           string.Format(" orderId='{0}' and playerAcc='{1}' ",
                           p.m_orderId, p.m_playerAcc));
                }
                break;
        }
        return cmd;
    }
}

//////////////////////////////////////////////////////////////////////////
public class ParamRealTimeOrder : ParamScoreOpRecord
{

}

public class OrderInfoItem : OrderInfo
{
    public DateTime m_finishTime;
}

// 查询玩家在线时实时订单
public class QueryRealTimeOrder : QueryBase
{
    private List<OrderInfoItem> m_result = new List<OrderInfoItem>();
    public const string SQL_QUERY_REAL_TIME_ORDER = " SELECT * from {0} {1} " +
                                               " order by genTime desc LIMIT {2}, {3} ";

    public override OpRes doQuery(object param, GMUser user)
    {
        m_result.Clear();
        ParamRealTimeOrder p = (ParamRealTimeOrder)param;
        string cond = "";
        OpRes res = getQueryCond(p, user, ref cond);
        if (res != OpRes.opres_success)
            return res;

        string cmd = string.Format(SQL_QUERY_REAL_TIME_ORDER,
            TableName.PLAYER_ORDER_COMPLETE,
            cond,
            (p.m_curPage - 1) * p.m_countEachPage,
            p.m_countEachPage);

        // 查看满足条件的记当个数
        user.totalRecord = user.sqlDb.getRecordCountNoWhere(TableName.PLAYER_ORDER_COMPLETE,
                                                            cond, user.getMySqlServerID(), MySqlDbName.DB_XIANXIA);

        List<Dictionary<string, object>> dataList = user.sqlDb.queryList(cmd,
            user.getMySqlServerID(), MySqlDbName.DB_XIANXIA);
        if (dataList == null)
            return OpRes.op_res_failed;

        for (int i = 0; i < dataList.Count; i++)
        {
            OrderInfoItem info = new OrderInfoItem();
            m_result.Add(info);

            Dictionary<string, object> data = dataList[i];
            info.m_orderId = Convert.ToString(data["orderId"]);
            info.m_apiOrderId = Convert.ToString(data["apiOrderId"]);
            info.m_genTime = Convert.ToDateTime(data["genTime"]);
            info.m_finishTime = Convert.ToDateTime(data["finishTime"]);
            info.m_gmAcc = Convert.ToString(data["gmAcc"]);
            info.m_playerAcc = Convert.ToString(data["playerAcc"]);
            info.m_orderType = Convert.ToInt32(data["orderType"]);
            info.m_money = Convert.ToInt64(data["money"]);
            info.m_orderState = Convert.ToInt32(data["orderState"]);
            info.m_failReason = Convert.ToInt32(data["failReason"]);
        }

        return OpRes.opres_success;
    }

    public override object getQueryResult()
    {
        return m_result;
    }

    // 构造查询条件
    OpRes getQueryCond(ParamScoreOpRecord p, GMUser user, ref string condStr)
    {
        DateTime mint = DateTime.Now, maxt = mint;
        bool useTime = false;
        if (!string.IsNullOrEmpty(p.m_time))
        {
            useTime = Tool.splitTimeStr(p.m_time, ref mint, ref maxt);
            if (!useTime)
                return OpRes.op_res_time_format_error;
        }

        QueryCondGenerator gen = new QueryCondGenerator();

        gen.addCondition(string.Format(" gmCreateCode like '{0}%' ", ItemHelp.getCreateCodeSpecial(user)));

        if (!string.IsNullOrEmpty(p.m_opAcc))
        {
            gen.addCondition(string.Format("gmAcc='{0}'", p.m_opAcc));
        }
        if (!string.IsNullOrEmpty(p.m_dstAcc))
        {
            gen.addCondition(string.Format("playerAcc='{0}'", p.m_dstAcc));
        }
        if (useTime)
        {
            gen.addCondition(string.Format(" genTime>='{0}' and genTime<'{1}' ", mint.ToString(ConstDef.DATE_TIME24),
                maxt.ToString(ConstDef.DATE_TIME24)));
        }

        condStr = gen.and();

        return OpRes.opres_success;
    }
}
