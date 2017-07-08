using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System.Windows.Forms;

// 玩家的输赢统计，累计一天的变化值，然后向mysql插入一条记录
public class StatWinLoseService : ServiceBase
{
    private Thread m_threadWork = null;
    
    private bool m_run = true;

    private ParamStatWinLose m_param = new ParamStatWinLose();

    private PumpBase m_stat;

    private MySqlDbServer m_sqlDb;

    private int m_serverId = 0; // 查询的db服务器id

    private DataStatService.Form1 m_form;

    SysMgr m_sysMgr = new SysMgr();

    public StatWinLoseService()
    {
        m_sysType = ServiceType.serviceTypeWinLose;
    }

    public override void initService()
    {
        XmlConfig xml = ResMgr.getInstance().getRes("dbserver.xml");
        string mysql = xml.getString("mysql", "");
        string connectStr = xml.getString("connectStr", "");
        m_sqlDb = new MySqlDbServer(mysql, connectStr);

        string alg = xml.getString("statAlg", "A");
        m_stat = createStatUnit(alg, m_serverId);

        m_sysMgr.addSys(new SysStatWinLose(this), SysType.sysTypeWinLoseStat);
        m_sysMgr.init();

        m_threadWork = new Thread(new ThreadStart(run));
        m_threadWork.Start();
    }

    public override void exitService()
    {
        m_run = false;
        m_threadWork.Join();
        m_sqlDb = null;
        m_form = null;
    }

    public void setForm(DataStatService.Form1 f)
    {
        m_form = f;
        if (m_form != null)
        {
            XmlConfig xml = ResMgr.getInstance().getRes("dbserver.xml");
            string mysql = xml.getString("mysql", "");
            m_form.setDbIP(DBMgr.getInstance().getIP(m_serverId), mysql);
        }
    }

    private void run()
    {
//         Dictionary<string, object> data = new Dictionary<string, object>();
//         data.Add("playerAcc", "123test00");
//         data.Add("playerId", "0");
//         data.Add("startTime", DateTime.Now);
//         data.Add("endTime", DateTime.Now);

        // 只找完整退出的记录
      //  IMongoQuery imq = Query.EQ("full", BsonValue.Create(true));

        while (m_run)
        {
            try
            {
                /*List<Dictionary<string, object>> dataList =
                    DBMgr.getInstance().executeQuery(TableName.PLAYER_FOR_WIN_LOSE,
                                                     m_serverId,
                                                     DbName.DB_PLAYER,
                                                     imq,
                                                     0,
                                                     1000);
                if (dataList.Count > 0 && m_form != null)
                {
                    m_form.setState(0);
                }
                
                for (int i = 0; i < dataList.Count && m_run; i++)
                {
                    Dictionary<string, object> data = dataList[i];
                    int retCode = stat(data);
                    if (retCode == 0)
                    {
                        bool res = insertData(data);
                        if (res)
                        {
                            DBMgr.getInstance().remove(TableName.PLAYER_FOR_WIN_LOSE, "key", data["key"], m_serverId, DbName.DB_PLAYER);
                        }
                    }
                    else if (retCode == FunRet.RET_LACK_PARAM)
                    {
                        if (data.ContainsKey("key"))
                        {
                            DBMgr.getInstance().remove(TableName.PLAYER_FOR_WIN_LOSE, "key", data["key"], m_serverId, DbName.DB_PLAYER);
                        }
                    }
                }*/

                if (m_form != null)
                {
                    m_form.setState(0);
                }
                
                m_sysMgr.update(0);

                if (m_form != null)
                {
                    m_form.setState(1);
                }
            }
            catch (System.Exception ex)
            {
                LogMgr.log.Error(ex.ToString());
            }

            if (m_run)
            {
                Thread.Sleep(1000);
            }
        }
    }

    // 统计exitGamePlayer退出游戏的玩家数据
    public int stat(int playerId, DateTime startT, DateTime endT)
    {
        m_param.m_playerId = playerId;
        m_param.m_start = startT;
        m_param.m_end = endT;

        return m_stat.doStat(m_param);
    }

    public bool insertData(string playerAcc, DateTime now)
    {
        ResultStatWinLose result = (ResultStatWinLose)m_stat.getStatResult();
        if (result.empty())
        {
            //LogMgr.log.InfoFormat("{0}, {1}, 数据空", now, playerAcc);
            return true;
        }

        int count = 0;
       // string playerAcc = Convert.ToString(exitGamePlayer["playerAcc"]);
     
        //DateTime now = DateTime.Now.Date;
        string cond = string.Format(" date='{0}' and playerAcc='{1}' ", now.Date.ToString(ConstDef.DATE_TIME24),
            playerAcc);
        // 判定当天的记录是否存在
        bool exists = m_sqlDb.keyExists(TableName.PLAYER_WIN_LOSE, cond, MySqlDbName.DB_XIANXIA);
        if (exists) // 该玩家当天记录存在，更新
        {
            SqlUpdateGenerator gen = new SqlUpdateGenerator();
            gen.addField("playerOutlay", string.Format("playerOutlay+{0}", result.m_playerOutlay), FieldType.TypeNumber);
            gen.addField("playerIncome", string.Format("playerIncome+{0}", result.m_playerIncome), FieldType.TypeNumber);
            gen.addField("washCount", string.Format("washCount+{0}", result.m_washCount), FieldType.TypeNumber);
            string sql = gen.getResultSql(TableName.PLAYER_WIN_LOSE, cond);
            count = m_sqlDb.executeOp(sql, MySqlDbName.DB_XIANXIA);
        }
        else // 插入一条新的记录
        {
            string createCode = null;
            string creator = null;
            string sql = string.Format("select createCode, creator from {0} where acc='{1}' ",
                TableName.PLAYER_ACCOUNT_XIANXIA, playerAcc);

            Dictionary<string, object> data = m_sqlDb.queryOne(sql, MySqlDbName.DB_XIANXIA);
            if (data != null)
            {
                creator = Convert.ToString(data["creator"]);
                createCode = Convert.ToString(data["createCode"]);
            }
            else
            {
                createCode = "noPlayer";
                creator = "noPlayer";
            }

            SqlInsertGenerator gen = new SqlInsertGenerator();
            gen.addField("date", now.Date.ToString(ConstDef.DATE_TIME24), FieldType.TypeString);
            gen.addField("playerAcc", playerAcc, FieldType.TypeString);
            gen.addField("playerCreator", creator, FieldType.TypeString);
            gen.addField("playerCreateCode", createCode, FieldType.TypeString);
            gen.addField("playerOutlay", result.m_playerOutlay, FieldType.TypeNumber);
            gen.addField("playerIncome", result.m_playerIncome, FieldType.TypeNumber);
            gen.addField("washCount", result.m_washCount, FieldType.TypeNumber);
            sql = gen.getResultSql(TableName.PLAYER_WIN_LOSE);
            count = m_sqlDb.executeOp(sql, MySqlDbName.DB_XIANXIA);
        }

        return count > 0;
    }

    PumpBase createStatUnit(string alg, int serverId)
    {
        if (alg == "A")
        {
            return new StatWinLoseA(serverId);
        }

        return new StatWinLoseB(serverId);
    }
}

//////////////////////////////////////////////////////////////////////////

public class PumpBase
{
    // 开始统计
    public virtual int doStat(object param) { return 0; }

    // 返回统计结果
    public virtual object getStatResult() { return null; }
}

//////////////////////////////////////////////////////////////////////////

class ResultStatWinLose
{
    public int m_playerId;
    public long m_playerOutlay;
    public long m_playerIncome;
    public long m_washCount;

    public void reset()
    {
        m_playerId = 0;
        m_playerOutlay = 0;
        m_playerIncome = 0;
        m_washCount = 0;
    }

    public bool empty()
    {
        return m_playerOutlay == 0 && m_playerIncome == 0 && m_washCount == 0;
    }
}

class ParamStatWinLose
{
    public int m_playerId;
    public DateTime m_start;
    public DateTime m_end;
}

// 统计A
public class StatWinLoseA : PumpBase
{
    ResultStatWinLose m_result = new ResultStatWinLose();
    int m_serverId = 0; // 查询的db服务器id

    protected string m_algKey;

    public StatWinLoseA(int serverId)
    {
        m_serverId = serverId;
        m_algKey = "winLoseStatA";
    }

    // 开始统计，返回0成功
    public override int doStat(object param)
    {
        m_result.reset();
        int retCode = 0;
        DBMgr.getInstance().checkDb(m_serverId);

        ParamStatWinLose p = (ParamStatWinLose)param;
        List<IMongoQuery> queryList = new List<IMongoQuery>();

        IMongoQuery imq1 = Query.LT("genTime", BsonValue.Create(p.m_end));
        IMongoQuery imq2 = Query.GTE("genTime", BsonValue.Create(p.m_start));
        queryList.Add(Query.And(imq1, imq2));
        queryList.Add(Query.EQ("playerId", BsonValue.Create(p.m_playerId)));
        IMongoQuery imq = Query.And(queryList);

        MapReduceResult map_result = DBMgr.getInstance().executeMapReduce(TableName.LOG_PLAYER_INFO,
                                                                            m_serverId,
                                                                            DbName.DB_PUMP,
                                                                            imq,
                                                                            MapReduceTable.getMap(m_algKey),
                                                                            MapReduceTable.getReduce(m_algKey));
        
        if (map_result != null)
        {
            IEnumerable<BsonDocument> bson = map_result.GetResults();
            foreach (BsonDocument d in bson)
            {
                try
                {
                    m_result.m_playerId = Convert.ToInt32(d["_id"]);

                    BsonValue resValue = d["value"];
                    m_result.m_playerOutlay = Convert.ToInt64(resValue["playerOutlay"]);
                    m_result.m_playerIncome = Convert.ToInt64(resValue["playerIncome"]);
                    m_result.m_washCount = Convert.ToInt64(resValue["washCount"]);
                }
                catch (System.Exception ex)
                {
                    retCode = FunRet.RET_HAS_EXCEPTION;
                    LogMgr.log.Error("StatWinLoseA:" + ex.ToString());
                }
            }
        }
        else
        {
            retCode = FunRet.RET_HAS_EXCEPTION;
        }

        return retCode;
    }

    public override object getStatResult() { return m_result; }
}

//////////////////////////////////////////////////////////////////////////
// 统计B
public class StatWinLoseB : StatWinLoseA
{
    public StatWinLoseB(int serverId)
        : base(serverId)
    {
        m_algKey = "winLoseStatB";
    }
}






















