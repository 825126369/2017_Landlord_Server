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

public class ResMgr
{
    private static ResMgr s_obj = null;
    // 表格所在路径
    private string m_path;
    // 存储表格容器
    private Dictionary<string, XmlConfig> m_allRes = new Dictionary<string, XmlConfig>();
    private Dictionary<string, DbServerInfo> m_dbServer = new Dictionary<string, DbServerInfo>();
    private Dictionary<int, DbServerInfo> m_dbServerById = new Dictionary<int, DbServerInfo>();

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
        m_path = @"..\data\";
    }

    // 设置表格所在路径
    public void setPath(string path)
    {
        m_path = path;
    }

    private void init()
    {
        loadXmlConfig("dbserver.xml");
        loadXmlConfig("opres.xml");
        loadXmlConfig("money_reason.xml");

        setUpDbServerInfo();
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

    public string getChannel()
    {
        XmlConfig xml = ResMgr.getInstance().getRes("dbserver.xml");
        string channel = xml.getString("channel", "");
        return channel;
    }

    private void loadXmlConfig(string file)
    {
        XmlConfigMaker c = new XmlConfigMaker();
        string fullfile = Path.Combine(m_path, file);
        XmlConfig xml = c.loadFromFile(fullfile);
        if (xml != null)
        {
            m_allRes.Add(file, xml);
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
}

//////////////////////////////////////////////////////////////////////////

public class ItemCFG : XmlDataTable<ItemCFG, int, ItemCFGData>, IXmlData
{
    public void init()
    {
        string path = @"..\data\";
        string file = Path.Combine(path, "ItemCFG.xml");
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
        string path = @"..\data\";
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
        if (taskType == TaskType.taskTypeDaily)
            return m_dailyTask;
        return m_achiveTask;
    }
}
