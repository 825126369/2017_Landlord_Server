using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Web;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System.Reflection;

// 操作结构
[Serializable]
class LogMsg
{
    // log ID号
    public long m_id = 0; 
    // 操作的DB服务器IP
    public string m_opDbIP = "";   
    // 操作账号
    public string m_account = "";   
    // 账号IP
    public string m_accountIP = ""; 
    // 操作类型
    public int m_opType = 0;
    // 操作时间
    public long m_opTime = 0;
    // 操作参数
    public string m_opParam = "";
}

class OpInfo
{
    // 操作类型
    public int m_opType;
    // 操作名称
    public string m_opName;
    // 格式串
    public string m_fmt = "";
    public OpParam m_param = null;

    public OpInfo(int type, string name, string fmt, string class_name)
    {
        m_opType = type;
        m_opName = name;
        m_fmt = fmt;
        if (class_name != "")
        {
            m_param = createOpParam(class_name);
        }
    }

    private OpParam createOpParam(string class_name)
    {
        Assembly t = Assembly.Load("WebManager");
        OpParam obj = (OpParam)t.CreateInstance(class_name);
        return obj;
    }
}

// 操作日志的管理
class OpLogMgr
{
    private static OpLogMgr s_mgr = null;
    private long m_id = 0;
    private Dictionary<int, OpInfo> m_opFmt = new Dictionary<int, OpInfo>();
    public StringBuilder m_textBuilder = new StringBuilder();

    public static OpLogMgr getInstance()
    {
        if (s_mgr == null)
        {
            s_mgr = new OpLogMgr();
            s_mgr.init();
        }
        return s_mgr;
    }

    // 返回总的记录条数
    public long totalRecord { get { return m_id; } }

    // 函数自拼接所需要的参数
    public void addLog(int op_type, OpParam op_param, GMUser user)
    {
        m_id = CountMgr.getInstance().getCurId(CountMgr.OP_LOG_COUNT_KEY);
        Dictionary<string, object> data = new Dictionary<string, object>();
        data["id"] = m_id;
        data["OpDbIP"] = user.m_dbIP;
        data["account"] = user.m_user;
        data["accountIP"] = user.m_ip;
        data["OpType"] = op_type;
        data["OpTime"] = DateTime.Now;
        data["OpParam"] = op_param.getString();

        // 操作日志存于0号服务器
        bool res = DBMgr.getInstance().save(TableName.OPLOG, data, "id", m_id, 0, DbName.DB_ACCOUNT);
    }

    public OpInfo getOpInfo(int opid)
    {
        if (m_opFmt.ContainsKey(opid))
        {
            return m_opFmt[opid];
        }
        return null;
    }

    public List<Dictionary<string, object>> getAllOpLog(int start, int count)
    {
        IMongoQuery imq = Query.And(new IMongoQuery[] { Query.GTE("id", BsonValue.Create(start)), Query.LT("id", BsonValue.Create(start + count)) });
        return DBMgr.getInstance().executeQuery(TableName.OPLOG, 0, DbName.DB_ACCOUNT, imq);
    }

    private void init()
    {
        // 加载格式XML ------------------------------------------------
        try
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(HttpRuntime.BinDirectory + "..\\" + "data\\format.xml");

            XmlNode node = doc.SelectSingleNode("/configuration");

            for (node = node.FirstChild; node != null; node = node.NextSibling)
            {
                string sid = node.Attributes["opid"].Value;
                int id = Convert.ToInt32(sid);
                string name = node.Attributes["opname"].Value;
                string fmt = node.Attributes["fmt"].Value;
                string classname = "";
                if (node.Attributes["class"] != null)
                {
                    classname = node.Attributes["class"].Value;
                }
                if (m_opFmt.ContainsKey(id))
                {
                    LOGW.Info("读format.xml时，发生了错误，出现了重复的ID {0}", id);
                }
                else
                {
                    m_opFmt.Add(id, new OpInfo(id, name, fmt, classname));
                }
            }
        }
        catch (System.Exception ex)
        {
            LOGW.Info(ex.Message);
            LOGW.Info(ex.StackTrace);
        }
    }
}



