using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

class StatByDayBase : SysBase
{
    protected DateTime m_statDay;

    public override void init()
    {
        Dictionary<string, object> data =
                 DBMgr.getInstance().getTableData(TableName.TOTAL_MONEY_STAT_DAY, "key", 
                getStatKey(), 0, DbName.DB_PLAYER);
        if (data != null)
        {
            m_statDay = Convert.ToDateTime(data["statDay"]).ToLocalTime();
        }
        else
        {
            DateTime now = DateTime.Now.Date;
            m_statDay = now.AddDays(1);
            resetStatDay(m_statDay);
        }
    }

    protected void resetStatDay(DateTime statDay)
    {
        Dictionary<string, object> upData = new Dictionary<string, object>();
        upData.Add("statDay", statDay);
        DBMgr.getInstance().update(TableName.TOTAL_MONEY_STAT_DAY, upData,
            "key", getStatKey(), 0, DbName.DB_PLAYER, true);
    }

    public virtual string getStatKey()
    {
        throw new Exception();
    }
}

//////////////////////////////////////////////////////////////////////////

class SysStatWinLose: StatByDayBase
{
    StatWinLoseService m_svr = null;
    static int STAT_DELTA = 1;

    public SysStatWinLose(StatWinLoseService svr)
    {
        m_svr = svr;
    }

    public override void init()
    {
        base.init();

        XmlConfig xml = ResMgr.getInstance().getRes("dbserver.xml");
        STAT_DELTA = xml.getInt("statDelta", 1);
    }

    public override string getStatKey()
    {
        return "winLose";
    }

    public override void update(double delta)
    {
        if (DateTime.Now < m_statDay)
            return;

        DateTime startTime = m_statDay.AddMinutes(-STAT_DELTA);
        DateTime endTime = m_statDay;
        List<int> playerList = getPlayerList();
        string[] fields = { "account" };

        foreach (var pid in playerList)
        {
            int res = m_svr.stat(pid, startTime, endTime);
           // LogMgr.log.InfoFormat("开始统计...{0}", pid);
            if (res == 0)
            {
                Dictionary<string, object> data =
                    DBMgr.getInstance().getTableData(TableName.PLAYER_INFO, "player_id", pid, fields, 0, DbName.DB_PLAYER);

                if (data != null)
                {
                    if (data.ContainsKey("account"))
                    {
                        m_svr.insertData(Convert.ToString(data["account"]), startTime);
                    }
                }
            }
        }

        m_statDay = m_statDay.AddMinutes(STAT_DELTA);
        resetStatDay(m_statDay);
    }

    List<int> getPlayerList()
    {
        List<int> playerList = new List<int>();

        IMongoQuery imq1 = Query.LT("genTime", BsonValue.Create(m_statDay));
        IMongoQuery imq2 = Query.GTE("genTime", BsonValue.Create(m_statDay.AddMinutes(-STAT_DELTA)));
        IMongoQuery imq = Query.And(imq1, imq2);

        MapReduceResult mapResult = DBMgr.getInstance().executeMapReduce(TableName.LOG_PLAYER_INFO,
                                                                    0,
                                                                    DbName.DB_PUMP,
                                                                    imq,
                                                                    MapReduceTable.getMap("winLosePlayer"),
                                                                    MapReduceTable.getReduce("winLosePlayer"));

        if (mapResult != null)
        {
            IEnumerable<BsonDocument> bson = mapResult.GetResults();
            foreach (BsonDocument d in bson)
            {
                try
                {
                    int playerId = Convert.ToInt32(d["_id"]);
                    playerList.Add(playerId);
                }
                catch (System.Exception ex)
                {
                }
            }
        }
        return playerList;
    }
}


