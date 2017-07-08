using System;
using System.Collections.Generic;
using System.Web;
using System.IO;
using System.Xml;

public class DbServerInfo
{
    // 主数据库IP，PlayerDB，它也作为关键字
    public string m_serverIp = "";
    public int m_serverId;
    public string m_serverName = "";

    // 日志数据库所在IP
    public string m_logDbIp = "";
}

public class PlatformInfo
{
    public string m_engName = "";
    public string m_chaName = "";
    public string m_tableName = "";
}

public class OpRightInfo
{
    // 职员类型
    public string m_staffType = "";
    // 发放奖励限制价值
    public int m_sendRewardLimit;
}

public class ResMgr
{
    private static ResMgr s_obj = null;
    // 表格所在路径
    private string m_path;
    // 存储表格容器
    private Dictionary<string, XmlConfig> m_allRes = new Dictionary<string, XmlConfig>();
    // 存储表格容器
    private Dictionary<string, IUserTabe> m_allTable = new Dictionary<string, IUserTabe>();

    private Dictionary<string, DbServerInfo> m_dbServer = new Dictionary<string, DbServerInfo>();
    private Dictionary<int, DbServerInfo> m_dbServerById = new Dictionary<int, DbServerInfo>();

    // 平台相关信息
    private Dictionary<string, PlatformInfo> m_plat = new Dictionary<string, PlatformInfo>();
    private Dictionary<int, PlatformInfo> m_platId = new Dictionary<int, PlatformInfo>();

    // 各类人员发放奖励时的限制
    private Dictionary<string, OpRightInfo> m_opRight = new Dictionary<string, OpRightInfo>();

    public static ResMgr getInstance()
    {
        if (s_obj == null)
        {
            s_obj = new ResMgr();
            s_obj.init();
        }
        return s_obj;
    }

    public ResMgr()
    {
        m_path = HttpRuntime.BinDirectory + "..\\" + "data";
    }

    // 设置表格所在路径
    public void setPath(string path)
    {
        m_path = path;
    }

    private void init()
    {
        XmlConfigMaker c = new XmlConfigMaker();
        loadXmlConfig("money_reason.xml", c);
        loadXmlConfig("dbserver.xml", c);
        loadTable("map_reduce.csv", new MapReduceTable(), '$');

        setUpDbServerInfo();
        setUpPlatformInfo(c);
        setUpOpRight(c);
    }

    // 取得某个表格
    public XmlConfig getRes(string name)
    {
        if (m_allRes.ContainsKey(name))
        {
            return m_allRes[name];
        }
        return null;
    }

    // 取得某个表格
    public T getTable<T>(string name) where T : IUserTabe
    {
        if (m_allTable.ContainsKey(name))
        {
            return (T)m_allTable[name];
        }
        return default(T);
    }

    // 返回平台名称
    public PlatformInfo getPlatformInfo(int index)
    {
        if (m_platId.ContainsKey(index))
        {
            return m_platId[index];
        }
        return null;
    }

    // 根据英文名，得到中文平台名
    public PlatformInfo getPlatformInfoByName(string name)
    {
        if (m_plat.ContainsKey(name))
        {
            return m_plat[name];
        }
        return null;
    }

    // 返回平台名称
    public string getPlatformName(int index, bool eng = true)
    {
        PlatformInfo data = getPlatformInfo(index);
        if (data == null)
        {
            return "none";
        }
        if (eng)
            return data.m_engName;
        
        return data.m_chaName;
    }

    public Dictionary<int, PlatformInfo> getAllPlatId()
    {
        return m_platId;
    }

    public Dictionary<string, DbServerInfo> getAllDb()
    {
        return m_dbServer;
    }

    public DbServerInfo getDbInfo(string ip)
    {
        if (m_dbServer.ContainsKey(ip))
            return m_dbServer[ip];
        
        return null;
    }

    public DbServerInfo getDbInfoById(int serverId)
    {
        if (m_dbServerById.ContainsKey(serverId))
            return m_dbServerById[serverId];

        return null;
    }

    public OpRightInfo getOpRightInfo(string staffType)
    {
        if (m_opRight.ContainsKey(staffType))
            return m_opRight[staffType];

        return null;
    }

    private XmlConfig loadXmlConfig(string file, XmlConfigMaker c, bool add = true)
    {
        string fullfile = Path.Combine(m_path, file);
        XmlConfig xml = c.loadFromFile(fullfile);
        if (xml != null && add)
        {
            m_allRes.Add(file, xml);
        }
        return xml;
    }

    private void loadTable(string file, IUserTabe table, char end_flag = ' ')
    {
        string fullfile = Path.Combine(m_path, file);
        if (!Csv.load(fullfile, table, end_flag))
        {
            LOGW.Info("读取文件[{0}]失败!", file);
        }
        else
        {
            if (!m_allTable.ContainsKey(file))
            {
                m_allTable.Add(file, table);
            }
        }
    }

    private void setUpDbServerInfo()
    {
        XmlConfig cfg = getRes("dbserver.xml");
        List<Dictionary<string, object>> t = cfg.getTable("server");
        for (int i = 0; i < t.Count; i++)
        {
            DbServerInfo info = new DbServerInfo();
            info.m_serverIp = Convert.ToString(t[i]["serverIp"]);
            info.m_serverId = Convert.ToInt32(t[i]["serverId"]);
            info.m_serverName = Convert.ToString(t[i]["serverName"]);
            info.m_logDbIp = Convert.ToString(t[i]["logDbIp"]);
            m_dbServer.Add(info.m_serverIp, info);
            m_dbServerById.Add(info.m_serverId, info);
        }
    }

    private void setUpPlatformInfo(XmlConfigMaker c)
    {
        XmlConfig cfg = loadXmlConfig("platform.xml", c, false);
        int count = cfg.getCount();
        for (int i = 0; i < count; i++)
        {
            List<Dictionary<string, object>> data = cfg.getTable(i.ToString());

            PlatformInfo info = new PlatformInfo();
            info.m_engName = Convert.ToString(data[0]["eng"]);
            info.m_chaName = Convert.ToString(data[0]["cha"]);
            info.m_tableName = Convert.ToString(data[0]["table"]);
            m_plat.Add(info.m_engName, info);
            m_platId.Add(i, info);
        }
    }

    private void setUpOpRight(XmlConfigMaker c)
    {
        string[] arr = new string[] { "service", "operation", "opDirector", "ceo", "admin" };
        XmlConfig cfg = loadXmlConfig("OpRight.xml", c, false);
        int count = cfg.getCount();
        for (int i = 0; i < arr.Length; i++)
        {
            List<Dictionary<string, object>> data = cfg.getTable(arr[i]);
            if (data != null)
            {
                OpRightInfo info = new OpRightInfo();
                info.m_staffType = arr[i];
                info.m_sendRewardLimit = Convert.ToInt32(data[0]["sendRewardLimit"]);
                m_opRight.Add(arr[i], info);
            }
            else
            {
                LOGW.Info("OpRight.xml找不到关键字[{0}]}", arr[i]);
            }
        }
    }
}

//////////////////////////////////////////////////////////////////////////

public class MapReduceItem
{
    public string m_map = "";
    public string m_reduce = "";
}

public class MapReduceTable : IUserTabe
{
    private Dictionary<string, MapReduceItem> m_items = new Dictionary<string, MapReduceItem>();
   
    public void beginRead()
    {
        m_items.Clear();
    }

    public void readLine(ITable table)
    {
        MapReduceItem item = new MapReduceItem();
        string key = table.fetch("fun").toStr();
        item.m_map = table.fetch("map").toStr();
        item.m_reduce = table.fetch("reduce").toStr();
        m_items.Add(key, item);
    }

    public void endRead()
    {
    }

    public MapReduceItem getItem(string key)
    {
        if(m_items.ContainsKey(key))
        {
            return m_items[key];
        }
        return null;
    }

    public static string getMap(string key)
    {
        MapReduceTable t = ResMgr.getInstance().getTable<MapReduceTable>("map_reduce.csv");
        if (t != null)
        {
            MapReduceItem item = t.getItem(key);
            if (item != null)
            {
                return item.m_map;
            }
        }
        return "";
    }

    public static string getReduce(string key)
    {
        MapReduceTable t = ResMgr.getInstance().getTable<MapReduceTable>("map_reduce.csv");
        if (t != null)
        {
            MapReduceItem item = t.getItem(key);
            if (item != null)
            {
                return item.m_reduce;
            }
        }
        return "";
    }
}

//////////////////////////////////////////////////////////////////////////

public class ItemCFG : XmlDataTable<ItemCFG, int, ItemCFGData>, IXmlData
{
    public void init()
    {
        string path = HttpRuntime.BinDirectory + "..\\" + "data";
        string file = Path.Combine(path, "M_ItemCFG.xml");
        XmlDocument doc = new XmlDocument();
        doc.Load(file);

        XmlNode node = doc.SelectSingleNode("/Root");

        for (node = node.FirstChild; node != null; node = node.NextSibling)
        {
            ItemCFGData tmp = new ItemCFGData();
            string sid = node.Attributes["ItemId"].Value;
            tmp.m_itemId = Convert.ToInt32(sid);
            tmp.m_itemName = node.Attributes["ItemName"].Value;
            m_data.Add(tmp.m_itemId, tmp);
        }
    }
}

//////////////////////////////////////////////////////////////////////////

public class QuestCFG : XmlDataTable<QuestCFG, int, QusetCFGData>, IXmlData
{
    // 每日任务
    private List<QusetCFGData> m_dailyTask = new List<QusetCFGData>();
    // 成就
    private List<QusetCFGData> m_achiveTask = new List<QusetCFGData>();

    public void init()
    {
        string path = HttpRuntime.BinDirectory + "..\\" + "data";
        string file = Path.Combine(path, "QuestCFG.xml");
        XmlDocument doc = new XmlDocument();
        doc.Load(file);

        XmlNode node = doc.SelectSingleNode("/Root");

        for (node = node.FirstChild; node != null; node = node.NextSibling)
        {
            QusetCFGData tmp = new QusetCFGData();
            string sid = node.Attributes["ID"].Value;
            tmp.m_questId = Convert.ToInt32(sid);
            tmp.m_questType = Convert.ToInt32(node.Attributes["Type"].Value);
            tmp.m_questName = node.Attributes["Name"].Value;
            m_data.Add(tmp.m_questId, tmp);

            if (tmp.m_questType == 1) // 每日
            {
                m_dailyTask.Add(tmp);
            }
            else
            {
                m_achiveTask.Add(tmp);
            }
        }
    }

    // 任务列表
    public List<QusetCFGData> getTaskList(TaskType taskType) 
    { 
        if(taskType == TaskType.taskTypeDaily)
            return m_dailyTask;
        return m_achiveTask;
    }
}

//////////////////////////////////////////////////////////////////////////

public class FishCFG : XmlDataTable<FishCFG, int, FishCFGData>, IXmlData
{
    public void init()
    {
        string path = HttpRuntime.BinDirectory + "..\\" + "data";
        string file = Path.Combine(path, "Fish_FishCFG.xml");
        XmlDocument doc = new XmlDocument();
        doc.Load(file);

        XmlNode node = doc.SelectSingleNode("/Root");

        for (node = node.FirstChild; node != null; node = node.NextSibling)
        {
            FishCFGData tmp = new FishCFGData();
            string sid = node.Attributes["ID"].Value;
            tmp.m_fishId = Convert.ToInt32(sid);
            tmp.m_fishName = node.Attributes["FishName"].Value;
            m_data.Add(tmp.m_fishId, tmp);
        }
    }
}

//////////////////////////////////////////////////////////////////////////

public class Crocodile_RateCFG : XmlDataTable<Crocodile_RateCFG, int, Crocodile_RateCFGData>, IXmlData
{
    public void init()
    {
        string path = HttpRuntime.BinDirectory + "..\\" + "data";
        string file = Path.Combine(path, "Crocodile_RateCFG.xml");
        XmlDocument doc = new XmlDocument();
        doc.Load(file);

        XmlNode node = doc.SelectSingleNode("/Root");

        for (node = node.FirstChild; node != null; node = node.NextSibling)
        {
            Crocodile_RateCFGData tmp = new Crocodile_RateCFGData();
            string sid = node.Attributes["Key"].Value;
            tmp.m_areaId = Convert.ToInt32(sid);
            tmp.m_name = node.Attributes["Name"].Value;
            m_data.Add(tmp.m_areaId, tmp);
        }
    }
}

//////////////////////////////////////////////////////////////////////////

public class Dice_BetAreaCFG : XmlDataTable<Dice_BetAreaCFG, int, Crocodile_RateCFGData>, IXmlData
{
    public void init()
    {
        string path = HttpRuntime.BinDirectory + "..\\" + "data";
        string file = Path.Combine(path, "Dice_BetAreaCFG.xml");
        XmlDocument doc = new XmlDocument();
        doc.Load(file);

        XmlNode node = doc.SelectSingleNode("/Root");

        for (node = node.FirstChild; node != null; node = node.NextSibling)
        {
            Crocodile_RateCFGData tmp = new Crocodile_RateCFGData();
            string sid = node.Attributes["Key"].Value;
            tmp.m_areaId = Convert.ToInt32(sid);
            tmp.m_name = node.Attributes["AreaName"].Value;
            m_data.Add(tmp.m_areaId, tmp);
        }
    }
}


//////////////////////////////////////////////////////////////////////////

public class RechargeCFGData
{
    public int m_payId;
    public int m_price;
}

public class RechargeCFG : XmlDataTable<RechargeCFG, int, RechargeCFGData>, IXmlData
{
    private List<RechargeCFGData> m_rechargeList = new List<RechargeCFGData>();

    public void init()
    {
        string path = HttpRuntime.BinDirectory + "..\\" + "data";
        string file = Path.Combine(path, "M_RechangeCFG.xml");
        XmlDocument doc = new XmlDocument();
        doc.Load(file);

        XmlNode node = doc.SelectSingleNode("/Root");

        for (node = node.FirstChild; node != null; node = node.NextSibling)
        {
            RechargeCFGData tmp = new RechargeCFGData();
            string sid = node.Attributes["ID"].Value;
            tmp.m_payId = Convert.ToInt32(sid);
            tmp.m_price = Convert.ToInt32(node.Attributes["Price"].Value);
            m_rechargeList.Add(tmp);
            m_data.Add(tmp.m_payId, tmp);
        }
    }

    public List<RechargeCFGData> getRechargeList()
    {
        return m_rechargeList;
    }
}
