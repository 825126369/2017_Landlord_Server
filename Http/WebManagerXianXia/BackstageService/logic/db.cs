using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.IO;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

public class DBMgr
{
    private static DBMgr s_db = null;
    // ip与下标的对应
    private Dictionary<string, int> m_dbServer = new Dictionary<string, int>();
    private DBServer[] m_mongoServer = null;

    public static DBMgr getInstance()
    {
        if (s_db == null)
        {
            s_db = new DBMgr();
            s_db.open();
        }
        return s_db;
    }

    // 初始化
    private void open()
    {
        Dictionary<string, DbServerInfo> allDb = ResMgr.getInstance().getAllDb();
        int count = allDb.Count;
        m_mongoServer = new DBServer[count + 2];

        int i = 0;
        foreach (var info in allDb.Values)
        {
            if (!m_dbServer.ContainsKey(info.m_serverIp))
            {
                m_dbServer.Add(info.m_serverIp, i);
                m_mongoServer[i] = null;
                i++;
            }
        }

        XmlConfig xml = ResMgr.getInstance().getRes("dbserver.xml");
        string acc = xml.getString("account", "");
        string pay = xml.getString("payment", "");
        if (!m_dbServer.ContainsKey(acc))
        {
            m_dbServer.Add(acc, i);
            m_mongoServer[i] = null;
            i++;
        }
        if (!m_dbServer.ContainsKey(pay))
        {
            m_dbServer.Add(pay, i);
            m_mongoServer[i] = null;
            i++;
        }
    }

    private DBServer create(string url, DbServerInfo dbInfo, bool special = false)
    {
        try
        {
            DBServer tmp = new DBServer();
            bool res = tmp.init(url, dbInfo, special);
            if (!res)
                return null;
            return tmp;
        }
        catch (System.Exception ex)
        {
            LOG.Info("连接数据库 {0} 时发生异常!", url);
        }
        return null;
    }

    // 根据地址名称，取得下标
    public int getDbId(string pools, bool special = false)
    {
        if (m_dbServer.ContainsKey(pools))
        {
            DbServerInfo dbInfo = ResMgr.getInstance().getDbInfo(pools);

            int index = m_dbServer[pools];
            if (m_mongoServer[index] == null)
            {
                m_mongoServer[index] = create(pools, dbInfo, special);
                if (m_mongoServer[index] == null)
                {
                    LOG.Info("getDbId函数，无法联接数据库!");
                    return -1;
                }
            }
            return index;
        }
        LOG.Info("没有找到地址:{0}，将采用原地址", pools);
        return -1;
    }

    // 表是否存在
    public bool existTable(string tablename, int serverid, int dbid)
    {
        return m_mongoServer[serverid].getDB(dbid).TableExists(tablename);
    }

    // 清空表中的所有数据
    public bool clearTable(string tablename, int serverid, int dbid)
    {
        if (!existTable(tablename, serverid, dbid))
            return true;

        return m_mongoServer[serverid].getDB(dbid).ExecuteRemoveAll(tablename);
    }

    // 根据查询条件，在table内查询结果
    public List<Dictionary<string, object>> executeQuery(string tablename,                 // 表名
                                                         int serverid, 
                                                         int dbname,
                                                         IMongoQuery query = null,         // 查询条件，外部要自拼接
                                                         int skip = 0,
                                                         int limt = 0,
                                                         string[] fields = null,
                                                         string sort = "",
                                                         bool asc = true
                                                         )
    {
        return m_mongoServer[serverid].getDB(dbname).ExecuteQuery(tablename, query, fields, sort, asc, skip, limt);
    }

    // 从表tablename中获取一个数据列表
    public List<Dictionary<string, object>> getDataListFromTable(string tablename,
                                                                 int serverid,
                                                                 int dbname,
                                                                 string key,
                                                                 object val,
                                                                 string[] fields = null,
                                                                 string sort = "",
                                                                 bool asc = true,
                                                                 int skip = 0,
                                                                 int limt = 0)
    {
        return m_mongoServer[serverid].getDB(dbname).ExecuteGetListBykey(tablename, key, val, fields, sort, asc, skip, limt);
    }

    // 取得表tablename中， 字段名fieldname 为value行
    public Dictionary<string, object> getTableData(string tablename, string fieldname, object val, int serverid, int dbid)
    {
        try
        {
            return m_mongoServer[serverid].getDB(dbid).ExecuteGetOneBykey(tablename, fieldname, val);
        }
        catch (System.Exception ex)
        {
            LOG.Info(ex.Message);
            LOG.Info(ex.StackTrace);
        }
        return null;
    }

    // 取得表tablename中， 字段名fieldname 为value行
    public Dictionary<string, object> getTableData(string tablename, string fieldname, object val, string[] fields, int serverid, int dbid)
    {
        try
        {
            return m_mongoServer[serverid].getDB(dbid).ExecuteGetOneBykey(tablename, fieldname, val, fields);
        }
        catch (System.Exception ex)
        {
            LOG.Info(ex.Message);
            LOG.Info(ex.StackTrace);
        }
        return null;
    }

    // 取得表tablename中， 字段名fieldname 为value行
    public Dictionary<string, object> getTableData(string tablename, int serverid, int dbid, IMongoQuery query, string[] fields = null)
    {
        try
        {
            return m_mongoServer[serverid].getDB(dbid).ExecuteGetByQuery(tablename, query, fields);
        }
        catch (System.Exception ex)
        {
            LOG.Info(ex.Message);
            LOG.Info(ex.StackTrace);
        }
        return null;
    }

    // 返回表tablename中满足条件的记录个数
    public long getRecordCount(string tablename, IMongoQuery query, int serverid, int dbid)
    {
        try
        {
            return m_mongoServer[serverid].getDB(dbid).ExecuteGetCount(tablename, query);
        }
        catch (System.Exception ex)
        {
            LOG.Info(ex.ToString());
        }
        return 0;
    }

    // 返回表tablename中满足条件的记录个数
    public long getRecordCount(string tablename, string fieldname, object val, int serverid, int dbid)
    {
        IMongoQuery imq = Query.EQ(fieldname, BsonValue.Create(val));
        return getRecordCount(tablename, imq, serverid, dbid);
    }

    // 将数据保存到tablename内
    public bool save(string tablename, Dictionary<string, object> data, string filedname, object val, int serverid, int dbid)
    {
        if (data == null)
            return false;
        try
        {
            Dictionary<string, object> tmp = m_mongoServer[serverid].getDB(dbid).ExecuteGetBykey(tablename, filedname, val);
            // 没有数据，新插入一行
            if (tmp == null)
            {
                return m_mongoServer[serverid].getDB(dbid).ExecuteInsert(tablename, data);
            }
            else // 有，则替换
            {
                return m_mongoServer[serverid].getDB(dbid).ExecuteStoreBykey(tablename, filedname, val, data);
            }
        }
        catch (System.Exception ex)
        {
            LOG.Info(ex.Message);
            LOG.Info(ex.StackTrace);
        }
        return false;
    }

    // 将数据保存到tablename内
    public bool save(string tablename, Dictionary<string, object> data, IMongoQuery query, int serverid, int dbid)
    {
        if (data == null)
            return false;
        try
        {
            Dictionary<string, object> tmp = m_mongoServer[serverid].getDB(dbid).ExecuteGetByQuery(tablename, query);
            // 没有数据，新插入一行
            if (tmp == null)
            {
                return m_mongoServer[serverid].getDB(dbid).ExecuteInsert(tablename, data);
            }
            else // 有，则替换
            {
                return m_mongoServer[serverid].getDB(dbid).ExecuteStoreByQuery(tablename, query, data);
            }
        }
        catch (System.Exception ex)
        {
            LOG.Info(ex.ToString());
        }
        return false;
    }

    // 更新数据表中某些字段值
    public bool update(string tablename, Dictionary<string, object> data, IMongoQuery query, int serverid, int dbid)
    {
        if (data == null)
            return false;
        try
        {
            return m_mongoServer[serverid].getDB(dbid).ExecuteUpdateByQuery(tablename, query, data, UpdateFlags.None);
        }
        catch (System.Exception ex)
        {
            LOG.Info(ex.ToString());
        }
        return false;
    }

    public bool updateAll(string tablename, Dictionary<string, object> data, IMongoQuery query, int serverid, int dbid)
    {
        if (data == null)
            return false;
        try
        {
            return m_mongoServer[serverid].getDB(dbid).ExecuteUpdateByQuery(tablename, query, data, UpdateFlags.Multi);
        }
        catch (System.Exception ex)
        {
            LOG.Info(ex.ToString());
        }
        return false;
    }

    // 更新数据表中某些字段值
    public bool update(string tablename, Dictionary<string, object> data, string filedname, object val, int serverid, int dbid)
    {
        if (data == null)
            return false;
        try
        {
            return m_mongoServer[serverid].getDB(dbid).ExecuteUpdate(tablename, filedname, val, data, UpdateFlags.None);
        }
        catch (System.Exception ex)
        {
            LOG.Info(ex.ToString());
        }
        return false;
    }

    // 向表tablename中新增数据，若指定关键字的数据已存在，则不添加
    public bool addTableData(string tablename, Dictionary<string, object> data, string filedname, object val, int serverid, int dbid)
    {
        if (data == null)
            return false;
        try
        {
            Dictionary<string, object> tmp = m_mongoServer[serverid].getDB(dbid).ExecuteGetBykey(tablename, filedname, val);
            // 没有数据，新插入一行
            if (tmp == null)
            {
                return m_mongoServer[serverid].getDB(dbid).ExecuteInsert(tablename, data);
            }
        }
        catch (System.Exception ex)
        {
            LOG.Info(ex.Message);
            LOG.Info(ex.StackTrace);
        }
        return false;
    }

    // 从数据库移除记录
    public bool remove(string tablename, string filedname, object val, int serverid, int dbid)
    {
        try
        {
            return m_mongoServer[serverid].getDB(dbid).ExecuteRemoveBykey(tablename, filedname, val);
        }
        catch (System.Exception ex)
        {
            LOG.Info(ex.ToString());
        }
        return false;
    }

    public bool insertData(string table, List<Dictionary<string, object>> data, int serverid, int dbid, List<int> failIndex = null)
    {
        try
        {
            List<BsonDocument> docList = new List<BsonDocument>();
            for (int i = 0; i < data.Count; i++)
            {
                docList.Add(new BsonDocument(data[i]));
            }
            return m_mongoServer[serverid].getDB(dbid).ExecuteInsterList(table, docList, failIndex);
        }
        catch (System.Exception ex)
        {
            LOG.Info(ex.ToString());
        }
        return false;
    }

    // 执行map-reduce
    public MapReduceResult executeMapReduce(string tablename, int serverid, int dbname, IMongoQuery query,
                                                             string map_js, string reduce_js)
    {
        return m_mongoServer[serverid].getDB(dbname).executeMapReduce(tablename, query, map_js, reduce_js);
    }
}

public class MongodbHelper
{
    private MongoDatabase mMongodbClient = null;

    public MongodbHelper()
    {
    }

    public MongoDatabase MongoDb
    {
        get
        {
            return mMongodbClient;
        }
        set
        {
            mMongodbClient = value;
        }
    }

    MongoCollection<BsonDocument> check_table(string tablename, string indexname = "")
    {
        MongoCollection<BsonDocument> tmp = null;

        try
        {
            if (!mMongodbClient.CollectionExists(tablename))
            {
                CommandResult cr = mMongodbClient.CreateCollection(tablename);
                tmp = mMongodbClient.GetCollection(tablename);
            }
            else
            {
                tmp = mMongodbClient.GetCollection(tablename);
            }

            if (indexname != "" && !tmp.IndexExists(indexname))
                tmp.CreateIndex(indexname);

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        return tmp;
    }

    // 检测表tablename是否存在
    public bool TableExists(string tablename)
    {
        return mMongodbClient.CollectionExists(tablename);
    }

    public bool KeyExists(string table, object val)
    {
        return KeyExistsBykey(table, "_key", val);
    }

    public bool KeyExistsBykey(string table, string key, object val)
    {
        try
        {
            var cb = check_table(table, key);

            long count = cb.Count(Query.EQ(key, BsonValue.Create(val)));
            if (count > 0)
                return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        return false;
    }

    public bool ExecuteStore(string table, object val, Dictionary<string, object> data)
    {
        return ExecuteStoreBykey(table, "_key", val, data);
    }

    public bool ExecuteStoreBykey(string table, string key, object val, Dictionary<string, object> data)
    {
        try
        {
            if (!data.ContainsKey(key))
                data.Add(key, val);

            var cb = check_table(table, key);
            var retu = cb.Update(Query.EQ(key, BsonValue.Create(val)), new UpdateDocument(data), UpdateFlags.Upsert);
            return retu.Ok;
            //return retu.LastErrorMessage;
        }
        catch (Exception ex)
        {
            LOG.Info(ex.Message);
        }
        return true;
    }

    public bool ExecuteStoreByQuery(string table, IMongoQuery queries, Dictionary<string, object> data)
    {
        try
        {
            var cb = check_table(table);
            var retu = cb.Update(queries, new UpdateDocument(data), UpdateFlags.Upsert);

            return retu.Ok;
        }
        catch (Exception ex)
        {
            LOG.Info(ex.ToString());
        }
        return false;
    }

    public long ExecuteInc(string table, object val, long def = 1, long iv = 1)
    {
        return ExecuteIncBykey(table, "_key", val, "Count", def, iv);
    }

    public long ExecuteIncBykey(string table, string key, object val, string name = "Count", long def = 1, long iv = 1)
    {
        long nv = 0;
        try
        {
            var cb = check_table(table, key);
            var retf = cb.Find(Query.EQ(key, BsonValue.Create(val)));
            retf = retf.SetFields(new string[] { name });
            retf = retf.SetLimit(1);
            var it = retf.GetEnumerator();
            if (it.MoveNext())
            {
                nv = it.Current[name].AsInt64;
            }

            if (nv < def)
            {
                iv = def;
                nv += iv;
            }

            var retu = cb.Update(Query.EQ(key, BsonValue.Create(val)), Update.Inc(name, iv), UpdateFlags.Upsert);

            if (!retu.Ok)
                return nv;


            return nv;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        return nv;
    }

    public void ExecuteRemove(string table, object val)
    {
        ExecuteRemoveBykey(table, "_key", val);
    }

    // 从表table删除某条数据，条件是：字段名 key = val
    public bool ExecuteRemoveBykey(string table, string key, object val)
    {
        try
        {
            var cb = check_table(table, key);
            var ret = cb.Remove(Query.EQ(key, BsonValue.Create(val)));
            return ret.Ok;
        }
        catch (Exception ex)
        {
            LOG.Info(ex.ToString());
        }
        return false;
    }

    public Dictionary<string, object> ExecuteGet(string table, object val)
    {
        return ExecuteGetBykey(table, "_key", val);
    }

    public Dictionary<string, object> ExecuteGetBykey(string table, string key, object val)
    {
        Dictionary<string, object> retval = null;
        try
        {
            var cb = check_table(table, key);
            var retf = cb.FindOne(Query.EQ(key, BsonValue.Create(val)));

            if (retf != null)
            {
                if (retf.Contains("_id"))
                    retf.Remove("_id");
                retval = retf.ToDictionary();
            }
            return retval;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        return retval;
    }

    // 向表table中增加一行数据
    public bool ExecuteInsert(string table, Dictionary<string, object> data)
    {
        try
        {
            var cb = check_table(table);
            var retf = cb.Insert(new BsonDocument(data));
            return retf.Ok;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        return false;
    }

    public bool ExecuteInsterList(string table, List<BsonDocument> blist, List<int> resIndex = null)
    {
        if (blist == null || blist.Count == 0)
            return false;
        try
        {

            var cb = mMongodbClient.GetCollection(table);
            var ret = cb.InsertBatch(blist);

            if (resIndex != null)
            {
                int i = 0;
                var it = ret.GetEnumerator();
                while (it.MoveNext())
                {
                    if (!it.Current.Ok)
                    {
                        resIndex.Add(i);
                    }
                    i++;
                }
            }
        }
        catch (Exception ex)
        {
            return false;
        }

        return true;
    }

    public bool ExecuteUpdate(string table, string fieldname, object val, Dictionary<string, object> data, UpdateFlags flag)
    {
        return ExecuteUpdateByQuery(table, Query.EQ(fieldname, BsonValue.Create(val)), data, flag);
    }

    public bool ExecuteUpdateByQuery(string table, IMongoQuery queries, Dictionary<string, object> data, UpdateFlags flag)
    {
        try
        {
            var cb = check_table(table);

            UpdateBuilder ub = new UpdateBuilder();
            foreach (var item in data)
            {
                ub = ub.Set(item.Key, BsonValue.Create(item.Value));
            }

            var retu = cb.Update(queries, ub, flag);
            return retu.Ok;
        }
        catch (Exception ex)
        {
        }
        return false;
    }

    public List<Dictionary<string, object>> ExecuteGetList(string table, object val, string[] fields = null,
        string sort = "", bool asc = true, int skip = 0, int limt = 0)
    {
        return ExecuteGetListBykey(table, "_key", val, fields, sort, asc, skip, limt);
    }

    public List<Dictionary<string, object>> ExecuteGetListBykey(string table, string key, object val, string[] fields = null,
        string sort = "", bool asc = true, int skip = 0, int limt = 0)
    {
        List<Dictionary<string, object>> retlist = new List<Dictionary<string, object>>();
        try
        {
            var cb = check_table(table, key);
            //IMongoQuery imq = Query.And(new IMongoQuery[]{Query.EQ(key, BsonValue.Create(val)),Query.NotExists(""),})

            var ret = cb.Find(Query.EQ(key, BsonValue.Create(val)));

            if (fields != null)
                ret = ret.SetFields(fields);

            if (sort != string.Empty)
            {
                if (asc)
                    ret = ret.SetSortOrder(SortBy.Ascending(sort));
                else
                    ret = ret.SetSortOrder(SortBy.Descending(sort));
            }

            if (skip > 0)
                ret = ret.SetSkip(skip);
            if (limt > 0)
                ret = ret.SetLimit(limt);

            var it = ret.GetEnumerator();

            while (it.MoveNext())
            {
                if (it.Current.Contains("_id"))
                    it.Current.Remove("_id");
                retlist.Add(it.Current.ToDictionary());
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            retlist.Clear();
        }
        return retlist;
    }

    // 根据查询条件，在table内查询结果
    public List<Dictionary<string, object>> ExecuteQuery(string table,                 // 表名
                                                         IMongoQuery query,            // 查询条件，外部要自拼接
                                                         string[] fields = null,
                                                         string sort = "",
                                                         bool asc = true,
                                                         int skip = 0,
                                                         int limt = 0)
    {
        // 没有设置条件时，返回全部数据
        if (query == null)
            return ExecuteGetAll(table, fields, sort, asc, skip, limt);

        // 表不存在，直接返回
        if (!TableExists(table))
            return null;

        List<Dictionary<string, object>> retlist = new List<Dictionary<string, object>>();
        try
        {
            var cb = check_table(table);
            //IMongoQuery imq = Query.And(new IMongoQuery[]{Query.EQ(key, BsonValue.Create(val)),Query.NotExists(""),})

            var ret = cb.Find(query);

            if (fields != null)
                ret = ret.SetFields(fields);

            if (sort != string.Empty)
            {
                if (asc)
                    ret = ret.SetSortOrder(SortBy.Ascending(sort));
                else
                    ret = ret.SetSortOrder(SortBy.Descending(sort));
            }

            if (skip > 0)
                ret = ret.SetSkip(skip);
            if (limt > 0)
                ret = ret.SetLimit(limt);

            var it = ret.GetEnumerator();

            while (it.MoveNext())
            {
                if (it.Current.Contains("_id"))
                    it.Current.Remove("_id");
                retlist.Add(it.Current.ToDictionary());
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            retlist.Clear();
        }
        return retlist;
    }

    public Dictionary<string, object> ExecuteGetByQuery(string table, IMongoQuery queries, string[] fields = null)
    {
        Dictionary<string, object> retval = null;
        try
        {
            var cb = check_table(table);
            var retf = cb.Find(queries);

            if (fields != null)
                retf = retf.SetFields(fields);

            var it = retf.GetEnumerator();

            if (it.MoveNext())
            {
                if (it.Current.Contains("_id"))
                    it.Current.Remove("_id");
                retval = it.Current.ToDictionary();
            }
            return retval;
        }
        catch (Exception ex)
        {
            LOG.Info(ex.ToString());
        }
        return retval;
    }

    public List<Dictionary<string, object>> ExecuteGetAll(string table, string[] fields = null, string sort = "", bool asc = true, int skip = 0, int limt = 0)
    {
        List<Dictionary<string, object>> retlist = new List<Dictionary<string, object>>();
        try
        {
            var cb = check_table(table);
            var ret = cb.FindAll();

            if (fields != null)
                ret = ret.SetFields(fields);

            if (sort != string.Empty)
            {
                if (asc)
                    ret = ret.SetSortOrder(SortBy.Ascending(sort));
                else
                    ret = ret.SetSortOrder(SortBy.Descending(sort));
            }

            if (skip > 0)
                ret = ret.SetSkip(skip);
            if (limt > 0)
                ret = ret.SetLimit(limt);

            var it = ret.GetEnumerator();

            while (it.MoveNext())
            {
                if (it.Current.Contains("_id"))
                    it.Current.Remove("_id");
                retlist.Add(it.Current.ToDictionary());
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        return retlist;
    }

    // 清空表内的所有数据
    public bool ExecuteRemoveAll(string table)
    {
        try
        {
            var cb = check_table(table);
            var retf = cb.RemoveAll();
            return retf.Ok;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        return false;
    }

    public Dictionary<string, object> ExecuteGetOne(string table, object val, string[] fields = null)
    {
        return ExecuteGetOneBykey(table, "_key", val, fields);
    }

    public Dictionary<string, object> ExecuteGetOneBykey(string table, string key, object val, string[] fields = null)
    {
        Dictionary<string, object> retlist = null;
        try
        {
            var cb = check_table(table, key);
            var ret = cb.Find(Query.EQ(key, BsonValue.Create(val)));

            if (fields != null)
                ret = ret.SetFields(fields);

            ret = ret.SetLimit(1);

            var it = ret.GetEnumerator();

            while (it.MoveNext())
            {
                if (it.Current.Contains("_id"))
                    it.Current.Remove("_id");
                retlist = it.Current.ToDictionary();
                break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        return retlist;
    }

    public string ExecuteSaveList(string table, string key, List<Dictionary<string, object>> datalist)
    {
        if (datalist == null)
            return "ExecuteSaveList error: datalist is null";
        try
        {
            var cb = check_table(table, key);

            foreach (var it in datalist)
            {
                var ret = cb.Update(Query.EQ(key, BsonValue.Create(it[key])), new UpdateDocument(it), UpdateFlags.Upsert);
                if (!ret.Ok)
                    return ret.LastErrorMessage;
            }

            return string.Empty;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        return string.Empty;
    }

    // 返回表table中，满足查询条件的记录个数
    public long ExecuteGetCount(string table, IMongoQuery queries)
    {
        try
        {
            var cb = check_table(table);
            if (queries == null)
            {
                return cb.Count();
            }
            return cb.Count(queries);
        }
        catch (Exception ex)
        {
            LOG.Info(ex.ToString());
        }
        return 0;
    }

    public MapReduceResult executeMapReduce(string table, IMongoQuery query, string map_js, string reduce_js)
    {
        try
        {
            var cb = check_table(table);
            BsonJavaScript map = BsonJavaScript.Create(map_js);
            BsonJavaScript reduce = BsonJavaScript.Create(reduce_js);
            var ret = cb.MapReduce(query, map, reduce);
            if(ret.Ok)
                return ret;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        return null;
    }
}

// 一个server上的db列表
public class DbName
{
    // 玩家信息db
    public const int DB_PLAYER = 0;

    // 统计相关的数据库
    public const int DB_PUMP = 1;

    // 玩家账号库
    public const int DB_ACCOUNT = 2;

    // 充值记录数据库
    public const int DB_PAYMENT = 3;

    public const int DB_NAME_MAX = 4;
}

// 一个db服务器
public class DBServer
{
    private MongodbHelper[] m_client = null;
    public static string[] m_dbName = new string[] { "PlayerDB_DWC", "LogDB_DWC", "AccountDB", "PaymentDB" };

    // 传入ip地址 192.169.1.12
    public bool init(string url, DbServerInfo dbInfo, bool special = false)
    {
        m_client = new MongodbHelper[DbName.DB_NAME_MAX];
        XmlConfig xml = ResMgr.getInstance().getRes("dbserver.xml");
        string acc = xml.getString("account", "");
        string pay = xml.getString("payment", "");
        if (url == acc)
        {
            // 是账号数据库，这里只创建一个，即可
            m_client[DbName.DB_ACCOUNT] = new MongodbHelper();
            m_client[DbName.DB_ACCOUNT].MongoDb = create(m_dbName[DbName.DB_ACCOUNT], url);

            if (url == pay)
            {
                // 充值数据库只连接这个
                m_client[DbName.DB_PAYMENT] = new MongodbHelper();
                m_client[DbName.DB_PAYMENT].MongoDb = create(m_dbName[DbName.DB_PAYMENT], url);
            }
        }
        else if (url == pay)
        {
            // 充值数据库只连接这个
            m_client[DbName.DB_PAYMENT] = new MongodbHelper();
            m_client[DbName.DB_PAYMENT].MongoDb = create(m_dbName[DbName.DB_PAYMENT], url);
        }
        else // 其他游戏相关数据库
        {
            m_client[DbName.DB_PLAYER] = new MongodbHelper();
            m_client[DbName.DB_PLAYER].MongoDb = create(m_dbName[DbName.DB_PLAYER], url);

            m_client[DbName.DB_PUMP] = new MongodbHelper();
            m_client[DbName.DB_PUMP].MongoDb = create(m_dbName[DbName.DB_PUMP], dbInfo.m_logDbIp);
        }
        /*for (int i = 0; i < DbName.DB_NAME_MAX; i++)
        {            
            m_client[i] = new MongodbHelper();
            m_client[i].MongoDb = create(m_dbName[i], url);
        }*/
        return testDBConnect();
    }

    public MongodbHelper getDB(int index)
    {
        if (index < 0 || index >= DbName.DB_NAME_MAX)
        {
            LOG.Info("获取某数据库服务器上某个db时出错，索引超出范围, index = {0}", index);
            return null;
        }
        return m_client[index];
    }

    // pools是一个db服务器地址
    private MongoDatabase create(string dbname, string pools)
    {
        try
        {
            var connectionString = "mongodb://" + pools;
            var client = new MongoClient(connectionString);
            var server = client.GetServer();
            return server.GetDatabase(dbname);
        }
        catch (System.Exception ex)
        {
            LOG.Info("连接数据库 {0} 时发生异常, {1}", pools, ex.Message);
        }
        return null;
    }

    // 测试是否可以连接数据库
    private bool testDBConnect()
    {
        bool res = true;
        try
        {
            m_client[0].MongoDb.CollectionExists("account");
        }
        catch (System.Exception ex)
        {
            // 出现这个异常说明没有连上
            if (ex.Message.IndexOf("Unable to connect to server") >= 0)
            {
                res = false;
            }
        }
        return res;
    }
}






































