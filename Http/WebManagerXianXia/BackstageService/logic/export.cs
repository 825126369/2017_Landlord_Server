using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System.Text.RegularExpressions;
using System.IO;

// 导出Excel
public abstract class ExportExcelBase
{
    // 将数据导出到文件filename内
    public OpRes exportExcel(ExportParam param, string exportDir)
    {
        int id = DBMgr.getInstance().getDbId(param.m_dbServerIP);
        if (id == -1)
        {
            LogMgr.error("ExportExcelBase.exportExcel，找不到服务器:{0}", param.m_dbServerIP);
            return OpRes.op_res_failed;
        }

        string dir = Path.Combine(exportDir, param.m_account);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        string tmpDir = Path.Combine(dir, "tmp");
        if (!Directory.Exists(tmpDir))
        {
            Directory.CreateDirectory(tmpDir);
        }

        OpRes res = OpRes.opres_success;
        StreamWriter sw = null;
        string srcFileName = "";
        string dstFileName = "";
        try
        {
            string file = initExport(param, id);
            dstFileName = Path.Combine(dir, file);
            srcFileName = Path.Combine(tmpDir, file);
            sw = new StreamWriter(srcFileName, false, Encoding.Default);
            exportData(sw, param, id);
        }
        catch (System.Exception ex)
        {
            if (sw != null)
            {
                sw.Close();
                res = OpRes.op_res_failed;
            }
            LogMgr.error(ex.ToString());
        }
        finally
        {
            if (sw != null)
            {
                sw.Close();

                if (srcFileName != "" && dstFileName != "")
                {
                    try
                    {
                        File.Copy(srcFileName, dstFileName, true);
                        File.Delete(srcFileName);
                    }
                    catch (System.Exception ex)
                    {
                    }
                }
            }
        }
        return res;
    }

    // 返回下一条数据
    public static List<Dictionary<string, object>> nextData(ref int skip, int count, IMongoQuery imq, string table, 
        int serverid, int dbid, string[]fields = null, string sort = "", bool asc = true)
    {
        List<Dictionary<string, object>> data =
            DBMgr.getInstance().executeQuery(table, serverid, dbid, imq, skip, count, fields, sort, asc);
        if (data == null || data.Count == 0)
            return null;
        skip += count;
        return data;
    }

    // 通过guid查询EtPlayer表中指定的属性
    public static Dictionary<string, object> getPlayerProperty(int playerId, int serverId, string[] fields)
    {
        Dictionary<string, object> ret =
             DBMgr.getInstance().getTableData(TableName.PLAYER_INFO, "player_id", playerId, fields, serverId, DbName.DB_PLAYER);
        return ret;
    }

    // 通过账号返回玩家属性
    public static Dictionary<string, object> getPlayerPropertyByAcc(string acc, int serverId, string[] fields)
    {
        Dictionary<string, object> ret =
                        DBMgr.getInstance().getTableData(TableName.PLAYER_INFO, "account", acc, fields, serverId, DbName.DB_PLAYER);
        return ret;
    }

    public abstract OpRes exportData(StreamWriter sheet, ExportParam param, int dbServerId);

    // 初始化导出，返回存储的文件名
    public abstract string initExport(ExportParam param, int dbServerId);

    public static string convertToCsvStr(string str)
    {
        if (str.IndexOf(',') > 0)
            return "\"" + str + "\"";
        return str;
    }

    public static string genTimeName(string time)
    {
        DateTime mint = DateTime.Now, maxt = DateTime.Now;

        return mint.ToString("yyyy年MM月dd日 HH时mm分ss秒") +
            "~" + maxt.ToString("yyyy年MM月dd日 HH时mm分ss秒");
    }
}

//////////////////////////////////////////////////////////////////////////

// 充值记录导出
public class ExportRecharge : ExportExcelBase
{
    public const string TABLE_NAME = "recharge";
    private string[] m_playerFields = { "account", "player_id" };
    private IMongoQuery m_imq = null;
    // 在哪个表中查询
    private string m_searchTable = "";
    private string m_plat = "";

    public override string initExport(ExportParam param, int dbServerId)
    {
        string account = "";
        int playerId = -1;
        string dbName = "";
        int userOpDbId = -1;
        string time = "";

        List<IMongoQuery> queryList = new List<IMongoQuery>();
        Dictionary<string, object> cond = param.m_condition;
        if (cond != null)
        {
            m_searchTable = Convert.ToString(cond["table"]);
            m_plat = Convert.ToString(cond["plat"]);
            DbServerInfo info = ResMgr.getInstance().getDbInfo(Convert.ToString(cond["userOpDbIp"]));
            if (info != null)
            {
                dbName = info.m_serverName;
                userOpDbId = DBMgr.getInstance().getDbId(info.m_serverIp);
            }

            if (cond.ContainsKey("PlayerId"))
            {
                playerId = Convert.ToInt32(cond["PlayerId"]);
                queryList.Add(Query.EQ("PlayerId", BsonValue.Create(playerId)));
                
                if (userOpDbId >= 0)
                {
                    Dictionary<string, object> ret = getPlayerProperty(playerId, userOpDbId, m_playerFields);
                    if (ret != null)
                    {
                        account = Convert.ToString(ret["account"]);
                    }
                }
            }
            /*if (cond.ContainsKey("ServerId"))
            {
                int serverId = Convert.ToInt32(cond["ServerId"]);
                queryList.Add(Query.EQ("ServerId", BsonValue.Create(serverId)));
            }*/
            if (cond.ContainsKey("Account"))
            {
                account = Convert.ToString(cond["Account"]);
                queryList.Add(Query.EQ("Account", BsonValue.Create(account)));

                if (userOpDbId >= 0)
                {
                    Dictionary<string, object> ret = getPlayerPropertyByAcc(account, userOpDbId, m_playerFields);
                    if (ret != null)
                    {
                        playerId = Convert.ToInt32(ret["player_id"]);
                    }
                }
            }
            if (cond.ContainsKey("time")) // 时间
            {
                time = Convert.ToString(cond["time"]);
                DateTime mint = DateTime.Now, maxt = DateTime.Now;
                Tool.splitTimeStr(time, ref mint, ref maxt);
                IMongoQuery imq1 = Query.LT("PayTime", BsonValue.Create(maxt));
                IMongoQuery imq2 = Query.GTE("PayTime", BsonValue.Create(mint));
                queryList.Add(Query.And(imq1, imq2));
                
                time = mint.ToString("yyyy年MM月dd日 HH时mm分ss秒") + "~" + maxt.ToString("yyyy年MM月dd日 HH时mm分ss秒");
            }
            if (cond.ContainsKey("Process"))
            {
                bool result = Convert.ToBoolean(cond["Process"]);
                queryList.Add(Query.EQ("Process", BsonValue.Create(result)));
            }
            if (cond.ContainsKey("range")) // 范围
            {
                string rangeStr = Convert.ToString(cond["range"]);

                List<int> range = new List<int>();
                Tool.parseNumList(rangeStr, range);
                IMongoQuery timq1 = Query.LTE("RMB", BsonValue.Create(range[1]));
                IMongoQuery timq2 = Query.GTE("RMB", BsonValue.Create(range[0]));
                IMongoQuery tmpImq = Query.And(timq1, timq2);
                queryList.Add(tmpImq);
            }
        }

        m_imq = queryList.Count > 0 ? Query.And(queryList) : null;

        if (playerId == -1)
        {
            return string.Format("充值记录-{0}-{1}-{2}.csv", account, dbName, time);
        }
        return string.Format("充值记录-{0}-{1}-{2}-{3}.csv", account, playerId, dbName, time);
    }

    public override OpRes exportData(StreamWriter sheet, ExportParam param, int dbServerId)
    {
        int skip = 0;
        List<Dictionary<string, object>> data = null;

        sheet.WriteLine("充值时间,玩家ID,玩家账号,客户端类型,订单ID,PayCode,充值金额(元),process");

        int t = 0;

        while (true)
        {
            data = nextData(ref skip, 1000, m_imq, m_searchTable, dbServerId, DbName.DB_PAYMENT);
            if (data == null)
                break;

            for (int i = 0; i < data.Count; i++)
            {
                try
                {
                    sheet.Write(Convert.ToDateTime(data[i]["PayTime"]).ToLocalTime().ToString());
                    sheet.Write(",");

                    if (data[i].ContainsKey("PlayerId"))
                    {
                        t = Convert.ToInt32(data[i]["PlayerId"]);
                        sheet.Write(t);
                    }
                    sheet.Write(",");

                    sheet.Write(Convert.ToString(data[i]["Account"]));
                    sheet.Write(",");

                    /* if (data[i].ContainsKey("ServerId"))
                     {
                         t = Convert.ToInt32(data[i]["ServerId"]);
                         DbServerInfo info = ResMgr.getInstance().getDbInfoById(t);
                         if (info != null)
                         {
                             sheet.Write(info.m_serverName);
                         }
                     }
                     sheet.Write(",");*/

                    sheet.Write(m_plat);
                    sheet.Write(",");

                    sheet.Write(" " + Convert.ToString(data[i]["OrderID"] + " "));
                    sheet.Write(",");

                    sheet.Write(" " + Convert.ToString(data[i]["PayCode"]) + " ");
                    sheet.Write(",");

                    if (data[i].ContainsKey("RMB"))
                    {
                        t = Convert.ToInt32(data[i]["RMB"]);
                        sheet.Write(t);
                    }
                    sheet.Write(",");

                    sheet.Write(Convert.ToBoolean(data[i]["Process"]));
                    sheet.WriteLine();
                }
                catch (System.Exception ex)
                {
                    LogMgr.error("导出充值记录出现异常:{0}", ex.ToString());
                }
            }
        }

        return OpRes.opres_success;
    }
}

//////////////////////////////////////////////////////////////////////////
// 金币，礼券变化导出
public class ExportMoney : ExportExcelBase
{
    private string[] m_playerFields = { "account", "player_id" };
    private IMongoQuery m_imq = null;
    private string[] m_playerFields1 = { "nickname" };
    private int m_moneyBase = 1;

    private static string[] s_head = new string[] { "时间", "玩家ID", "玩家账号", "玩家昵称", "初始值", "结束值", "差值(结束值-初始值)", "投注", "返还", "盈利", "获胜盈利", "所在游戏", "_id", "原因" };

    public override string initExport(ExportParam param, int dbServerId)
    {
        string fileName = "";

        List<IMongoQuery> queryList = new List<IMongoQuery>();
        Dictionary<string, object> cond = param.m_condition;
        if (cond != null)
        {
            if (cond.ContainsKey("playerId"))
            {
                int playerId = Convert.ToInt32(cond["playerId"]);
                queryList.Add(Query.EQ("playerId", BsonValue.Create(playerId)));
                fileName += "-" + playerId.ToString();
            }
            if (cond.ContainsKey("reason"))
            {
                int reason = Convert.ToInt32(cond["reason"]);
                queryList.Add(Query.EQ("reason", BsonValue.Create(reason)));

                XmlConfig xml = ResMgr.getInstance().getRes("money_reason.xml");
                fileName += "-" + xml.getString(reason.ToString(), "");
            }
            if (cond.ContainsKey("time"))
            {
                string time = Convert.ToString(cond["time"]);
                DateTime mint = DateTime.Now, maxt = DateTime.Now;
                Tool.splitTimeStr(time, ref mint, ref maxt);
                IMongoQuery imq1 = Query.LT("genTime", BsonValue.Create(maxt));
                IMongoQuery imq2 = Query.GTE("genTime", BsonValue.Create(mint));
                queryList.Add(Query.And(imq1, imq2));

                fileName += "-" + mint.ToString("yyyy年MM月dd日 HH时mm分ss秒") + "~" + maxt.ToString("yyyy年MM月dd日 HH时mm分ss秒");
            }
            if (cond.ContainsKey("range"))
            {
                List<int> range = new List<int>();
                string rangeStr = Convert.ToString(cond["range"]);
                Tool.parseNumList(rangeStr, range);
                IMongoQuery imq1 = Query.LTE("addValue", BsonValue.Create(range[1]));
                IMongoQuery imq2 = Query.GTE("addValue", BsonValue.Create(range[0]));
                queryList.Add(Query.And(imq1, imq2));
            }
            if (cond.ContainsKey("gameId"))
            {
                int id = Convert.ToInt32(cond["gameId"]);
                queryList.Add(Query.EQ("gameId", id));
                fileName += "-" + StrName.s_gameName[id];
            }
        }

        m_imq = queryList.Count > 0 ? Query.And(queryList) : null;

        Regex reg = new Regex("^" + Convert.ToString(cond["transCode"]) + ".*", RegexOptions.IgnoreCase);
        BsonRegularExpression regexp = new BsonRegularExpression(reg);
        IMongoQuery imqTmp = Query.Matches("creator", regexp);
        m_imq = Query.And(m_imq, imqTmp);

        m_moneyBase = Convert.ToInt32(cond["moneyBase"]);

        return string.Format("金币变化{0}.csv", fileName);
    }

    public override OpRes exportData(StreamWriter sheet, ExportParam param, int dbServerId)
    {
        int skip = 0;
        List<Dictionary<string, object>> dataList = null;

        string head = string.Join(",", s_head);
        sheet.WriteLine(head);

        int t = 0;
        XmlConfig xml = ResMgr.getInstance().getRes("money_reason.xml");

        while (true)
        {
            dataList = nextData(ref skip, 1000, m_imq, TableName.PUMP_PLAYER_MONEY, dbServerId, DbName.DB_PUMP,
                null, "genTime", false);
            if (dataList == null)
                break;

            try
            {
                for (int i = 0; i < dataList.Count; i++)
                {
                    Dictionary<string,object>data = dataList[i];

                    sheet.Write(Convert.ToDateTime(data["genTime"]).ToLocalTime().ToString());
                    sheet.Write(",");

                    t = Convert.ToInt32(data["playerId"]);
                    sheet.Write(t);
                    sheet.Write(",");

                    if (data.ContainsKey("playerAcc"))
                    {
                        sheet.Write(Convert.ToString(data["playerAcc"]));
                    }
                    sheet.Write(",");

                    Dictionary<string, object> ret = getPlayerProperty(t, dbServerId, m_playerFields1);
                    if (ret != null && ret.ContainsKey("nickname"))
                    {
                        sheet.Write(Convert.ToString(ret["nickname"]));
                    }
                    sheet.Write(",");

                    t = Convert.ToInt32(data["oldValue"]);
                    sheet.Write(Tool.showMoneyValue(t, m_moneyBase));
                    sheet.Write(",");

                    int t1 = Convert.ToInt32(data["newValue"]);
                    sheet.Write(Tool.showMoneyValue(t1, m_moneyBase));
                    sheet.Write(",");

                    t = t1 - t;
                    sheet.Write(Tool.showMoneyValue(t, m_moneyBase));
                    sheet.Write(",");

                    t1 = 0;
                    if (data.ContainsKey("playerOutlay"))
                    {
                        t1 = Convert.ToInt32(data["playerOutlay"]);
                        sheet.Write(Tool.showMoneyValue(t1, m_moneyBase));
                    }
                    sheet.Write(",");

                    if (data.ContainsKey("playerIncome"))
                    {
                        t = Convert.ToInt32(data["playerIncome"]);
                        sheet.Write(Tool.showMoneyValue(t, m_moneyBase));

                        t = t - t1;
                    }
                    sheet.Write(",");

                    sheet.Write(Tool.showMoneyValue(t, m_moneyBase));
                    sheet.Write(",");

                    if (data.ContainsKey("playerWinBet"))
                    {
                        t = Convert.ToInt32(data["playerWinBet"]);
                        sheet.Write(Tool.showMoneyValue(t, m_moneyBase));
                    }
                    sheet.Write(",");

                    t = Convert.ToInt32(data["gameId"]);
                    sheet.Write(StrName.s_gameName[t]);
                    sheet.Write(",");

                    if (data.ContainsKey("_id"))
                    {
                        sheet.Write(Convert.ToString(data["_id"]));
                    }

                    sheet.WriteLine();
                }
            }
            catch (System.Exception ex)
            {
                LogMgr.error(ex.ToString());
            }
        }

        return OpRes.opres_success;
    }
}

//////////////////////////////////////////////////////////////////////////

// 任务导出基类
public class ExportTaskBase : ExportExcelBase
{
    private string[] m_playerFields = { "account", "player_id" };
    private IMongoQuery m_imq = null;
    private string[] m_playerFields1 = { "nickname" };
    protected string m_tableName = "";

    public override string initExport(ExportParam param, int dbServerId)
    {
        string account = "";
        int playerId = -1;
        string dbName = "";
        string time = "";

        List<IMongoQuery> queryList = new List<IMongoQuery>();
        Dictionary<string, object> cond = param.m_condition;
        if (cond != null)
        {
            DbServerInfo info = ResMgr.getInstance().getDbInfo(param.m_dbServerIP);
            if (info != null)
            {
                dbName = info.m_serverName;
            }

            if (cond.ContainsKey("playerId"))
            {
                playerId = Convert.ToInt32(cond["playerId"]);
                queryList.Add(Query.EQ("playerId", BsonValue.Create(playerId)));

                Dictionary<string, object> ret = getPlayerProperty(playerId, dbServerId, m_playerFields);
                if (ret != null)
                {
                    account = Convert.ToString(ret["account"]);
                }
            }
            if (cond.ContainsKey("taskId"))
            {
                int taskId = Convert.ToInt32(cond["taskId"]);
                queryList.Add(Query.EQ("taskId", BsonValue.Create(taskId)));
            }
            if (cond.ContainsKey("time"))
            {
                time = Convert.ToString(cond["time"]);
                DateTime mint = DateTime.Now, maxt = DateTime.Now;
                Tool.splitTimeStr(time, ref mint, ref maxt);
                IMongoQuery imq1 = Query.LT("genTime", BsonValue.Create(maxt));
                IMongoQuery imq2 = Query.GTE("genTime", BsonValue.Create(mint));
                queryList.Add(Query.And(imq1, imq2));

                time = mint.ToString("yyyy年MM月dd日 HH时mm分ss秒") + "~" + maxt.ToString("yyyy年MM月dd日 HH时mm分ss秒");
            }
        }

        m_imq = queryList.Count > 0 ? Query.And(queryList) : null;

        return getExportFileName(playerId, account, dbName, time);
    }

    protected virtual string getExportFileName(int playerId, string account, string dbName, string time) { return ""; }

    public override OpRes exportData(StreamWriter sheet, ExportParam param, int dbServerId)
    {
        int skip = 0;
        List<Dictionary<string, object>> data = null;

        sheet.WriteLine("时间,玩家ID,任务ID,任务名称,道具id,道具名称,数量,道具id,道具名称,数量,道具id,道具名称,数量,道具id,道具名称,数量,道具id,道具名称,数量");

        int t = 0;
        XmlConfig xml = ResMgr.getInstance().getRes("money_reason.xml");

        List<ParamItem> rewardList = new List<ParamItem>();

        while (true)
        {
            data = nextData(ref skip, 1000, m_imq, m_tableName, dbServerId, DbName.DB_PUMP);
            if (data == null)
                break;

            for (int i = 0; i < data.Count; i++)
            {
                sheet.Write(Convert.ToDateTime(data[i]["genTime"]).ToLocalTime().ToString());
                sheet.Write(",");

                t = Convert.ToInt32(data[i]["playerId"]);
                sheet.Write(t);
                sheet.Write(",");

                t = Convert.ToInt32(data[i]["taskId"]);
                sheet.Write(t);
                sheet.Write(",");

                QusetCFGData qd = QuestCFG.getInstance().getValue(t);
                if (qd != null)
                {
                    sheet.Write(qd.m_questName);
                }
                sheet.Write(",");

                rewardList.Clear();
                Tool.parseItemFromDic(data[i]["items"] as Dictionary<string, object>, rewardList);

                for (int j = 0; j < rewardList.Count; j++)
                {
                    sheet.Write(rewardList[j].m_itemId);
                    sheet.Write(",");
                    
                    ItemCFGData pItem = ItemCFG.getInstance().getValue(rewardList[j].m_itemId);
                    if (pItem != null)
                    {
                        sheet.Write(pItem.m_itemName);
                    }
                    sheet.Write(",");

                    sheet.Write(rewardList[j].m_itemCount);

                    if (j != rewardList.Count - 1)
                    {
                        sheet.Write(",");
                    }
                }
                sheet.WriteLine();
            }
        }

        return OpRes.opres_success;
    }
}

// 每日任务导出
public class ExportDailyTask : ExportTaskBase
{
    public ExportDailyTask()
    {
        m_tableName = TableName.PUMP_DAILY_TASK;
    }

    protected override string getExportFileName(int playerId, string account, string dbName, string time)
    {
        if (playerId == -1)
        {
            return string.Format("每日任务-{0}-{1}-{2}.csv", account, dbName, time);
        }
        return string.Format("每日任务-{0}-{1}-{2}-{3}.csv", account, playerId, dbName, time);
    }
}

// 成就导出
public class ExportAchievement : ExportTaskBase
{
    public ExportAchievement()
    {
        m_tableName = TableName.PUMP_TASK;
    }

    protected override string getExportFileName(int playerId, string account, string dbName, string time)
    {
        if (playerId == -1)
        {
            return string.Format("成就-{0}-{1}-{2}.csv", account, dbName, time);
        }
        return string.Format("成就-{0}-{1}-{2}-{3}.csv", account, playerId, dbName, time);
    }
}

//////////////////////////////////////////////////////////////////////////

// 金币预警导出
public class ExportMoneyWarn : ExportExcelBase
{
    static string[] s_fieldGold = new string[] { "player_id", "gold", "safeBoxGold" };
    static string[] s_fieldTicket = new string[] { "player_id", "ticket" };
    static string[] s_field = new string[] { "nickname" };
    private int m_sel;
    private int m_count;

    public override string initExport(ExportParam param, int dbServerId)
    {
        string dbName = "";

        List<IMongoQuery> queryList = new List<IMongoQuery>();
        Dictionary<string, object> cond = param.m_condition;
        if (cond != null)
        {
            DbServerInfo info = ResMgr.getInstance().getDbInfo(param.m_dbServerIP);
            if (info != null)
            {
                dbName = info.m_serverName;
            }

            if (cond.ContainsKey("sel"))
            {
                m_sel = Convert.ToInt32(cond["sel"]);
            }
            if (cond.ContainsKey("count"))
            {
                m_count = Convert.ToInt32(cond["count"]);
            }
        }

        string time = DateTime.Now.ToString("yyyy年MM月dd日");
        return string.Format("{0}预警-{1}-{2}.csv", m_sel == 0 ? "金币" : "钻石", time, dbName);
    }

    public override OpRes exportData(StreamWriter sheet, ExportParam param, int dbServerId)
    {
        int skip = 0;
        List<Dictionary<string, object>> data = null;

        if (m_sel == 0)
        {
            sheet.WriteLine("玩家ID,玩家昵称,金币,保险箱");
        }
        else
        {
            sheet.WriteLine("玩家ID,玩家昵称,钻石");
        }

        //while (true)
        {
            string[] retField = null;
            if(m_sel == 0)
            {
                retField = s_fieldGold;
            }
            else
            {
                retField = s_fieldTicket;
            }
            IMongoQuery imq = Query.EQ("is_robot", false);
            data = nextData(ref skip, m_count, imq, TableName.PLAYER_INFO, dbServerId, DbName.DB_PLAYER, retField, retField[1], false);
            int t = 0;
            for (int i = 0; i < data.Count; i++)
            {
                t = Convert.ToInt32(data[i]["player_id"]);
                sheet.Write(t);
                sheet.Write(",");

                Dictionary<string, object> ret = getPlayerProperty(t, dbServerId, s_field);
                if (ret != null && ret.ContainsKey("nickname"))
                {
                    sheet.Write(Convert.ToString(ret["nickname"]));
                }
                sheet.Write(",");

                if (m_sel == 0)
                {
                    t = Convert.ToInt32(data[i]["gold"]);
                    sheet.Write(t);
                    if (data[i].ContainsKey("safeBoxGold"))
                    {
                        sheet.Write(",");
                        t = Convert.ToInt32(data[i]["safeBoxGold"]);
                        sheet.Write(t);
                    }
                }
                else
                {
                    t = Convert.ToInt32(data[i]["ticket"]);
                    sheet.Write(t);
                }
                sheet.WriteLine();
            }
        }

        return OpRes.opres_success;
    }
}

/////////////////////////////////////////////////////////////////////////
// 导出上下分记录
public class ExportScoreOpLog : ExportExcelBase
{
    private static string[] s_head = new string[] { "编号", "日期", "操作账号","余额","别名","类型", 
        "操作金额", "操作目标账号","目标账号余额",  "订单ID", "API订单", "来源", "处理时间", "处理结果", "备注"};

    private int m_moneyBase = 1;
    private string m_sqlCond = "";
    CMySqlDbServer m_sqlDb;

    public override string initExport(ExportParam param, int dbServerId)
    {
        string fileName = "";

        Dictionary<string, object> cond = param.m_condition;
        if (cond != null)
        {
            m_sqlCond = Convert.ToString(cond["cond"]);

            if (cond.ContainsKey("opSrc"))
            {
                fileName += "-" + Convert.ToString(cond["opSrc"]);
            }
            if (cond.ContainsKey("opDst"))
            {
                fileName += "-" + Convert.ToString(cond["opDst"]);
            }
            if (cond.ContainsKey("time"))
            {
                fileName += "-" + genTimeName(Convert.ToString(cond["time"]));
            }
            if (cond.ContainsKey("opResult"))
            {
                fileName += "-" + StrName.s_realTimeOrderState[Convert.ToInt32(cond["opResult"])];
            }
        }

        m_moneyBase = Convert.ToInt32(cond["moneyBase"]);

        XmlConfig xml = ResMgr.getInstance().getRes("dbserver.xml");
        string mysql = xml.getString("mysql", "");
        string connectStr = xml.getString("connectStr", "");
        m_sqlDb = new CMySqlDbServer(mysql, connectStr);

        return string.Format("上分下分记录{0}.csv", fileName);
    }

    public override OpRes exportData(StreamWriter sheet, ExportParam param, int dbServerId)
    {
        int skip = 0;
        List<Dictionary<string, object>> dataList = null;

        string head = string.Join(",", s_head);
        sheet.WriteLine(head);

        int t = 0;
        long nlong = 0;
        string str = "";

        while (true)
        {
            string sql = cmdStr(skip, 1000);
            dataList = m_sqlDb.queryList(sql, CMySqlDbName.DB_XIANXIA);
            if (dataList == null || dataList.Count == 0)
                break;

            skip += dataList.Count;
            try
            {
                for (int i = 0; i < dataList.Count; i++)
                {
                    Dictionary<string, object> data = dataList[i];
                    nlong = Convert.ToInt64(data["opId"]);
                    sheet.Write(nlong);
                    sheet.Write(",");

                    str = Convert.ToDateTime(data["opTime"]).ToString();
                    sheet.Write(str);
                    sheet.Write(",");

                    str = Convert.ToString(data["opSrc"]);
                    sheet.Write(str);
                    sheet.Write(",");

                    if (!(data["opRemainMoney"] is DBNull))
                    {
                        nlong = Convert.ToInt64(data["opRemainMoney"]);
                        sheet.Write(Tool.showMoneyValue(nlong, m_moneyBase));
                    }
                    sheet.Write(",");

                    if (!(data["aliasName"] is DBNull))
                    {
                        str = Convert.ToString(data["aliasName"]);
                        sheet.Write(str);
                    }
                    sheet.Write(",");

                    t = Convert.ToInt32(data["opType"]);
                    sheet.Write(StrName.s_scoreOpName[t]);
                    sheet.Write(",");

                    nlong = Convert.ToInt64(data["opScore"]);
                    sheet.Write(Tool.showMoneyValue(nlong, m_moneyBase));
                    sheet.Write(",");

                    str = Convert.ToString(data["opDst"]);
                    sheet.Write(str);
                    sheet.Write(",");

                    if (!(data["opDstRemainMoney"] is DBNull))
                    {
                        nlong = Convert.ToInt64(data["opDstRemainMoney"]);
                        sheet.Write(Tool.showMoneyValue(nlong, m_moneyBase));
                    }
                    sheet.Write(",");

                    if (!(data["orderId"] is DBNull))
                    {
                        str = Convert.ToString(data["orderId"]);
                        sheet.Write(str);
                    }
                    sheet.Write(",");

                    if (!(data["userOrderId"] is DBNull))
                    {
                        str = Convert.ToString(data["userOrderId"]);
                        sheet.Write(str);
                    }
                    sheet.Write(",");

                    if (!(data["logFrom"] is DBNull))
                    {
                        t = Convert.ToInt32(data["logFrom"]);
                    }
                    else
                    {
                        t = OrderGenerator.ORDER_FROM_BG_OP;
                    }
                    sheet.Write(StrName.s_logFrom[t]);
                    sheet.Write(",");

                    if (!(data["finishTime"] is DBNull))
                    {
                        str = Convert.ToDateTime(data["finishTime"]).ToString();
                        sheet.Write(str);
                    }
                    sheet.Write(",");

                    if (!(data["opResult"] is DBNull))
                    {
                        t = Convert.ToInt32(data["opResult"]);
                    }
                    else
                    {
                        t = PlayerReqOrderState.STATE_FINISH;
                    }
                    sheet.Write(StrName.s_realTimeOrderState[t]);
                    sheet.Write(",");

                    if (t == PlayerReqOrderState.STATE_FAILED)
                    {
                        if (!(data["failReason"] is DBNull))
                        {
                            t = Convert.ToInt32(data["failReason"]);
                            sheet.Write(StrName.getRealTimeOrderFailReason(t));
                        }
                    }

                    sheet.Write(",");
                    sheet.WriteLine();
                }
            }
            catch (System.Exception ex)
            {
                LogMgr.error(ex.ToString());
            }
        }

        return OpRes.opres_success;
    }

    string cmdStr(int skip, int count)
    {
        string cmd = string.Format(SqlStrCMD.SQL_QUERY_SCORE_OP,
                                    TableName.GM_SCORE,
                                    TableName.GM_ACCOUNT,
                                    m_sqlCond,
                                    skip,
                                    count);
        return cmd;
    }
}

