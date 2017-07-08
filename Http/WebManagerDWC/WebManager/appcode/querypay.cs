using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System.Text.RegularExpressions;

// 充值记录
public class RechargeItem
{
    // 充值时间
    public string m_time = "";
    // 玩家ID
    public int m_playerId;

    public DbServerInfo m_serverInfo = null;

    // 账号
    public string m_account = "";
    // 客户端类型
    public string m_clientType = "";
    // 订单ID
    public string m_orderId = "";
    public string m_payCode = "";
    // 充值金额
    public int m_totalPrice = 0;
    // 是否作了处理
    public bool m_process = false;
    public string m_processTime = "";
}

public class ParamQueryRecharge : ParamQuery
{
    // 平台索引
    public int m_platIndex = 0;

    public int m_result;

    public string m_range = "";

    // 游戏服务器索引
    public int m_gameServerIndex;

    // 游戏服务器ID
    public int m_serverId;
}

// 平台特有信息
public class PlatInfo
{
    // 平台名称
    public string m_platName = "";

    // 充值数据所在表名
    public string m_tableName;
}

public class QueryRecharge : QueryBase
{
    static string[] s_field = { "platform" };
    protected List<RechargeItem> m_result = new List<RechargeItem>();
    // 充值表
    private Dictionary<string, RechargeBase> m_items = new Dictionary<string, RechargeBase>();
    private PlatInfo m_platInfo = new PlatInfo();
    private RechargeBase m_rbase = null;
    private QueryCondition m_cond = new QueryCondition();

    public QueryRecharge()
    {
        m_items.Add("default", new RechargeDefault());
        m_items.Add("anysdk", new RechargeAnySdk());
        m_items.Add("qbao", new RechargeQbao());
        m_items.Add("baidu", new RechargeBaidu());
        m_items.Add("shuanglong", new RechargeShuangLog());
    }

    public PlatInfo getPlatInfo() { return m_platInfo; }

    public override OpRes doQuery(object param, GMUser user)
    {
        m_result.Clear();
        m_cond.startQuery();
        OpRes res = makeQuery(param, user, m_cond);
        if (res != OpRes.opres_success)
            return res;

        IMongoQuery imq = m_cond.getImq();
        ParamQueryRecharge p = (ParamQueryRecharge)param;
        return query(p, imq, m_rbase, user);
    }

    // 返回查询结果
    public override object getQueryResult()
    {
        return m_result;
    }

    public override OpRes makeQuery(object param, GMUser user, QueryCondition queryCond) 
    {
        ParamQueryRecharge p = (ParamQueryRecharge)param;

        int condCount = 0;
        PlatformInfo pinfo = null;

        if (!string.IsNullOrEmpty(p.m_param))
        {
            switch (p.m_way)
            {
                case QueryWay.by_way0: //  通过玩家id查询
                    {
                        int val = -1;
                        if (!int.TryParse(p.m_param, out val))
                        {
                            return OpRes.op_res_param_not_valid;
                        }
                        Dictionary<string, object> ret = QueryBase.getPlayerProperty(val, user, s_field);
                        if (ret == null)
                        {
                            return OpRes.op_res_not_found_data;
                        }
                        if (!ret.ContainsKey("platform"))
                        {
                            return OpRes.op_res_failed;
                        }

                        // 取玩家ID所在平台
                        string platName = Convert.ToString(ret["platform"]);
                        queryCond.addQueryCond("PlayerId", val);

                        pinfo = ResMgr.getInstance().getPlatformInfoByName(platName);

                        // 获取服务器ID
                        /*DbServerInfo dbInfo = ResMgr.getInstance().getDbInfo(user.m_dbIP);
                        if (dbInfo != null)
                        {
                            queryCond.addQueryCond("ServerId", dbInfo.m_serverId);
                        }*/
                    }
                    break;
                case QueryWay.by_way1: //  通过账号查询
                    {
                        Dictionary<string, object> ret = QueryBase.getPlayerPropertyByAcc(p.m_param, user, s_field);
                        if (ret == null)
                        {
                            return OpRes.op_res_not_found_data;
                        }
                        if (!ret.ContainsKey("platform"))
                        {
                            return OpRes.op_res_failed;
                        }
                       
                        // 取玩家账号所在平台，然后从相应的平台去查
                        string platName = Convert.ToString(ret["platform"]);
                        queryCond.addQueryCond("Account", p.m_param);

                        pinfo = ResMgr.getInstance().getPlatformInfoByName(platName);

                        // 获取服务器ID
                       /* DbServerInfo dbInfo = ResMgr.getInstance().getDbInfo(user.m_dbIP);
                        if (dbInfo != null)
                        {
                            queryCond.addQueryCond("ServerId", dbInfo.m_serverId);
                        }*/
                    }
                    break;
                case QueryWay.by_way2: //  通过订单号查询
                    {
                        pinfo = ResMgr.getInstance().getPlatformInfo(p.m_platIndex);
                        queryCond.addQueryCond("OrderID", p.m_param);
                    }
                    break;
                default:
                    {
                        return OpRes.op_res_failed;
                    }
            }
            condCount++;
        }
        else
        {
            pinfo = ResMgr.getInstance().getPlatformInfo(p.m_platIndex);

            // 获取服务器ID
            /*DbServerInfo dbInfo = ResMgr.getInstance().getDbInfo(user.m_dbIP);
            if (dbInfo != null)
            {
                queryCond.addQueryCond("ServerId", dbInfo.m_serverId);
            }*/
        }

        if (pinfo == null)
            return OpRes.op_res_need_sel_platform;

        if (!m_items.ContainsKey(pinfo.m_engName))
            return OpRes.op_res_not_found_data;

        m_rbase = m_items[pinfo.m_engName];
        
        m_platInfo.m_tableName = pinfo.m_tableName;
        m_platInfo.m_platName = pinfo.m_engName;

        if (queryCond.isExport())
        {
            queryCond.addCond("table", m_platInfo.m_tableName);
            queryCond.addCond("plat", m_platInfo.m_platName);
        }

        if (p.m_time != "")
        {
            DateTime mint = DateTime.Now, maxt = DateTime.Now;
            bool res = Tool.splitTimeStr(p.m_time, ref mint, ref maxt);
            if (!res)
                return OpRes.op_res_time_format_error;

            condCount++;
            if (queryCond.isExport())
            {
                queryCond.addCond("time", p.m_time);
            }
            else
            {
                IMongoQuery imq1 = Query.LT("PayTime", BsonValue.Create(maxt));
                IMongoQuery imq2 = Query.GTE("PayTime", BsonValue.Create(mint));
                queryCond.addImq(Query.And(imq1, imq2));
            }
        }

        if (p.m_result > 0)
        {
            queryCond.addQueryCond("Process", p.m_result == 1 ? true : false);
        }
        if (!string.IsNullOrEmpty(p.m_range))
        {
            if (!Tool.isTwoNumValid(p.m_range))
                return OpRes.op_res_param_not_valid;

            if (queryCond.isExport())
            {
                queryCond.addCond("range", p.m_range);
            }
            else
            {
                List<int> range = new List<int>();
                Tool.parseNumList(p.m_range, range);
                IMongoQuery timq1 = Query.LTE("RMB", BsonValue.Create(range[1]));
                IMongoQuery timq2 = Query.GTE("RMB", BsonValue.Create(range[0]));
                IMongoQuery tmpImq = Query.And(timq1, timq2);
                queryCond.addImq(tmpImq);
            }
        }
        
        if (condCount == 0)
            return OpRes.op_res_need_at_least_one_cond;

        return OpRes.opres_success; 
    }

    private OpRes query(ParamQueryRecharge param, IMongoQuery imq, RechargeBase r, GMUser user)
    {
        int serverId = DBMgr.getInstance().getSpecialServerId(DbName.DB_PAYMENT);
        if (serverId == -1)
            return OpRes.op_res_failed;

        user.totalRecord = DBMgr.getInstance().getRecordCount(m_platInfo.m_tableName, imq, serverId, DbName.DB_PAYMENT);

        List<Dictionary<string, object>> data =
             DBMgr.getInstance().executeQuery(m_platInfo.m_tableName, serverId, DbName.DB_PAYMENT, imq,
                                              (param.m_curPage - 1) * param.m_countEachPage, param.m_countEachPage);

        if (data == null || data.Count == 0)
        {
            return OpRes.op_res_not_found_data;
        }

        r.fillResultList(m_result, data);
        return OpRes.opres_success;
    }
}

//////////////////////////////////////////////////////////////////////////

public class RechargeBase
{
    public void fillResultList(List<RechargeItem> result, List<Dictionary<string, object>> data)
    {
        for (int i = 0; i < data.Count; i++)
        {
            RechargeItem tmp = new RechargeItem();
            result.Add(tmp);
            tmp.m_time = Convert.ToDateTime(data[i]["PayTime"]).ToLocalTime().ToString();
            tmp.m_account = Convert.ToString(data[i]["Account"]);
            tmp.m_orderId = Convert.ToString(data[i]["OrderID"]);
            tmp.m_payCode = Convert.ToString(data[i]["PayCode"]);
            tmp.m_process = Convert.ToBoolean(data[i]["Process"]);
            if (data[i].ContainsKey("RMB"))
            {
                tmp.m_totalPrice = Convert.ToInt32(data[i]["RMB"]);
            }
            if (data[i].ContainsKey("PlayerId"))
            {
                tmp.m_playerId = Convert.ToInt32(data[i]["PlayerId"]);
            }
           /* if (data[i].ContainsKey("ServerId"))
            {
                int id = Convert.ToInt32(data[i]["ServerId"]);
                tmp.m_serverInfo = ResMgr.getInstance().getDbInfoById(id);
            }*/
            fillResult(tmp, data[i]);
        }
    }

    public virtual void fillResult(RechargeItem tmp, Dictionary<string, object> data) { }
}

//////////////////////////////////////////////////////////////////////////

public class RechargeDefault : RechargeBase
{
    public override void fillResult(RechargeItem tmp, Dictionary<string, object> data)
    {
        tmp.m_clientType = "default";
    }
}

//////////////////////////////////////////////////////////////////////////

public class RechargeAnySdk : RechargeBase
{
    public override void fillResult(RechargeItem tmp, Dictionary<string, object> data)
    {
        tmp.m_clientType = "anysdk";
    }
}

//////////////////////////////////////////////////////////////////////////

public class RechargeQbao : RechargeBase
{
    public override void fillResult(RechargeItem tmp, Dictionary<string, object> data)
    {
        tmp.m_clientType = "qbao";
    }
}

//////////////////////////////////////////////////////////////////////////

public class RechargeBaidu : RechargeBase
{
    public override void fillResult(RechargeItem tmp, Dictionary<string, object> data)
    {
        tmp.m_clientType = "baidu";
    }
}

public class RechargeShuangLog : RechargeBase
{
    public override void fillResult(RechargeItem tmp, Dictionary<string, object> data)
    {
        tmp.m_clientType = "shuanglong";
    }
}








