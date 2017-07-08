using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Configuration;


using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

public class MongodbHelper<T> where T : new ()
{   

    //数据库操作缓存队列
    public Dictionary<string, List<BsonDocument>> mMapNoSql = new Dictionary<string,List<BsonDocument>>();

    public void OnTimer_Insert2DB()
    {
              
        foreach (var item in mMapNoSql)
        {
            ExecuteInsterList(item.Key, item.Value);
        }

        //CLEAR
        mMapNoSql.Clear();
        
    }

    public MongodbHelper()
    {

    }
    
    bool is_init = false;
    public bool IsInit { get { return is_init; } }
    public void init()
    {
        if (is_init) return;

        var connectionString = "mongodb://" + get_url();
        var client = new MongoClient(connectionString);
        var server = client.GetServer();
        mMongodbClient = server.GetDatabase(get_dbname());
        init_table();

        is_init = true;
    }

    protected MongoDatabase mMongodbClient = null;

    public MongoDatabase GetDB
    {
        get
        {
            return mMongodbClient;
        }
    }

    static T mongodb = default(T);
    public static T Instance
    {
        get
        {   
            if(mongodb == null)
            {
                mongodb = new T();
                MongodbHelper<T> mh = mongodb as MongodbHelper<T>;
                if (!mh.IsInit)
                {
                    mh.init();
                }
            }
            
            return mongodb;
        }
    }

  
    public bool KeyExists(string table, object val)
    {
        return KeyExistsBykey(table, "_key", val);
    }

    public bool KeyExistsBykey(string table, string key, object val)
    {
        try
        {
            var cb = mMongodbClient.GetCollection(table);
            
            long count = cb.Count(Query.EQ(key, BsonValue.Create(val)));
            if (count > 0)
                return true;
        }
        catch (Exception)
        {
           
        }

        return false;
    }

    public bool KeyExistsByQuery(string table, IMongoQuery queries)
    {
        try
        {
            var cb = mMongodbClient.GetCollection(table);

            long count = cb.Count(queries);
            if (count > 0)
                return true;
        }
        catch (Exception)
        {

        }

        return false;
    }

    public string ExecuteStore(string table, object val, Dictionary<string, object> data)
    {
        return ExecuteStoreBykey(table, "_key", val, data);
    }

    public string ExecuteStoreBykey(string table, string key, object val, Dictionary<string, object> data)
    {
        try
        {
            if (!data.ContainsKey(key))
                data.Add(key, val);

            var cb = mMongodbClient.GetCollection(table);
            var retu = cb.Update(Query.EQ(key, BsonValue.Create(val)), new UpdateDocument(data), UpdateFlags.Upsert);

            if (!retu.Ok)
                return retu.LastErrorMessage;
        }
        catch (Exception ex)
        {            
            return ex.ToString();
        }
        return string.Empty;
    }

    public string ExecuteStoreByQuery(string table, IMongoQuery queries, Dictionary<string, object> data)
    {
        try
        {
            var cb = mMongodbClient.GetCollection(table);
            var retu = cb.Update(queries, new UpdateDocument(data), UpdateFlags.Upsert);

            if (!retu.Ok)
                return retu.LastErrorMessage;
        }
        catch (Exception ex)
        {        
            return ex.ToString();
        }
        return string.Empty;
    }

    public string ExecuteUpdate(string table, string val, Dictionary<string, object> data, UpdateFlags flags = UpdateFlags.Upsert)
    {
        return ExecuteUpdateByQuery(table, Query.EQ("_key", val), data, flags);
    }

    public string ExecuteUpdate(string table, string fieldname, object val, Dictionary<string, object> data, UpdateFlags flags = UpdateFlags.Upsert)
    {
        return ExecuteUpdateByQuery(table, Query.EQ(fieldname, BsonValue.Create(val)), data, flags);
    }

    public string ExecuteUpdateByQuery(string table, IMongoQuery queries, Dictionary<string, object> data, UpdateFlags flags = UpdateFlags.Upsert)
    {
        try
        {
            var cb = mMongodbClient.GetCollection(table);
            
            UpdateBuilder ub = new UpdateBuilder();
            foreach (var item in data)
            {
                ub = ub.Set(item.Key, BsonValue.Create(item.Value));                
            }

            var retu = cb.Update(queries, ub, flags);

            if (!retu.Ok)
                return retu.LastErrorMessage;
        }
        catch (Exception ex)
        {
            return ex.ToString();
        }
        return string.Empty;
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
            var cb = mMongodbClient.GetCollection(table);
            var retf = cb.Find(Query.EQ(key, BsonValue.Create(val)));
            retf = retf.SetFields(new string[] { name });
            retf = retf.SetLimit(1);
            var it = retf.GetEnumerator();
            if (it.MoveNext())
            {
                nv = it.Current[name].AsInt64;
            }

            long upval = iv;
            if (nv < def)
            {
                nv = def;
                upval = def+iv;
            }

            var retu = cb.Update(Query.EQ(key, BsonValue.Create(val)), Update.Inc(name, upval), UpdateFlags.Upsert);

            if (retu.Ok)
            {                
                nv += iv;
                return nv;
            }

            
            return nv;
        }
        catch (Exception)
        {
            
        }
        return nv;
    }

    public string ExecuteIncByQuery(string table, IMongoQuery queries, Dictionary<string, object> data)
    {
        try
        {
            var cb = mMongodbClient.GetCollection(table);

            UpdateBuilder ub = new UpdateBuilder();
            foreach (var item in data)
            {
                ub = ub.Inc(item.Key, Convert.ToInt64(item.Value));
            }

            var retu = cb.Update(queries, ub, UpdateFlags.Upsert);

            if (!retu.Ok)
                return retu.LastErrorMessage;
        }
        catch (Exception ex)
        {
            return ex.ToString();
        }
        return string.Empty;

    }

    public void ExecuteRemove(string table, object val)
    {
        ExecuteRemoveBykey(table, "_key", val);
    }

    // 从表table删除某条数据，条件是：字段名 key = val
    public string ExecuteRemoveBykey(string table, string key, object val)
    {
        try
        {
            var cb = mMongodbClient.GetCollection(table);
            var ret = cb.Remove(Query.EQ(key, BsonValue.Create(val)));
            if (!ret.Ok)
                return ret.LastErrorMessage;
        }
        catch (Exception ex)
        {            
            return ex.ToString();
        }
        return string.Empty;
    }

    public string ExecuteRemoveByQuery(string table, IMongoQuery query)
    {
        try
        {
            var cb = mMongodbClient.GetCollection(table);
            var ret = cb.Remove(query);
            if (!ret.Ok)
                return ret.LastErrorMessage;
        }
        catch (Exception ex)
        {
            return ex.ToString();
        }
        return string.Empty;
    }

    public Dictionary<string, object> ExecuteGet(string table, object val, string[] fields = null)
    {
        return ExecuteGetBykey(table, "_key", val, fields);
    }

    public Dictionary<string, object> ExecuteGetBykey(string table, string key, object val, string[] fields = null)
    {
        Dictionary<string, object> retval = null;
        try
        {            
            var cb = mMongodbClient.GetCollection(table);
            var retf = cb.Find(Query.EQ(key, BsonValue.Create(val)));            

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
        catch (Exception)
        {
            
        }
        return retval;
    }

    public Dictionary<string, object> ExecuteGetByQuery(string table, IMongoQuery queries, string[] fields = null)
    {
        Dictionary<string, object> retval = null;
        try
        {
            var cb = mMongodbClient.GetCollection(table);
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
        catch (Exception)
        {
            retval = null;  
        }
        return retval;
    }

    // 向表table中增加一行数据
    // 默认直接插入数据库，对于实时性要求不高的插入操作，建议设置bDealy = true
    public bool ExecuteInsert(string table, Dictionary<string, object> data, bool bDelay = false)
    {
        if (!bDelay)  //立即入库
        {
            try
            {
                var cb = mMongodbClient.GetCollection(table);
                var retf = cb.Insert(new BsonDocument(data));
                return retf.Ok;
            }
            catch (Exception ex)
            {
               
            }
        }
        else //加入队列
        {
            List<BsonDocument> listDocument = null;

            if(mMapNoSql.ContainsKey(table))
            {
                listDocument = mMapNoSql[table];
            }
            else
            {
                listDocument = new List<BsonDocument>();
            }

            //BsonDocument doc = new BsonDocument();
            //doc.AddRange(data);
            listDocument.Add(data.ToBsonDocument());
            mMapNoSql[table] = listDocument;
        }

        return false;
    }

    public List<Dictionary<string, object>> ExecuteGetList(string table, object val, string[] fields = null,
        string sort = "", bool asc = true, int skip = 0,int limt = 0)
    {
        return ExecuteGetListBykey(table, "_key", val, fields, sort, asc, skip, limt);
    }

    public List<Dictionary<string, object>> ExecuteGetListBykey(string table, string key, object val, string[] fields = null,
        string sort = "", bool asc = true, int skip = 0,int limt = 0)
    {
        List<Dictionary<string, object>> retlist = new List<Dictionary<string, object>>();
        try
        {
            var cb = mMongodbClient.GetCollection(table);
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
        catch (Exception)
        {
            
            retlist.Clear();
        }
        return retlist;
    }

    public List<Dictionary<string, object>> ExecuteGetListByQuery(string table, IMongoQuery queries, string[] fields = null,
        string sort = "", bool asc = true, int skip = 0, int limt = 0)
    {
        List<Dictionary<string, object>> retlist = new List<Dictionary<string, object>>();
        try
        {
            var cb = mMongodbClient.GetCollection(table);

            var ret = cb.Find(queries);

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
                //if (it.Current.Contains("_id"))
                //    it.Current.Remove("_id");
                retlist.Add(it.Current.ToDictionary());
            }
        }
        catch (Exception)
        {
           
            retlist.Clear();
        }
        return retlist;
    }

    public List<Dictionary<string, object>> ExecuteGetAll(string table, string[] fields = null, string sort = "", bool asc = true, int skip = 0, int limt = 0)
    {
        List<Dictionary<string, object>> retlist = new List<Dictionary<string, object>>();
        try
        {
            var cb = mMongodbClient.GetCollection(table);
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
        catch (Exception)
        {
            
        }
        return retlist;
    }

    public bool ExecuteRemoveAll(string table)
    {        
        try
        {
            var cb = mMongodbClient.GetCollection(table);
            var retf = cb.RemoveAll();
            return retf.Ok;
        }
        catch (Exception)
        {
            
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
            var cb = mMongodbClient.GetCollection(table);
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
        catch (Exception)
        {
                    
        }
        return retlist;
    }

    public string ExecuteSaveListByQuery(string table, string key, List<IMongoQuery> queries, List<Dictionary<string, object>> datalist)
    {
        if (datalist == null)
            return "ExecuteSaveList error: datalist is null";
        try
        {
            var cb = mMongodbClient.GetCollection(table);

            foreach (var it in datalist)
            {
                queries.Add(Query.EQ(key, BsonValue.Create(it[key])));
                var ret = cb.Update(Query.And(queries), new UpdateDocument(it), UpdateFlags.Upsert);
                if (!ret.Ok)
                    return ret.LastErrorMessage;
            }

            return string.Empty;
        }
        catch (Exception)
        {
         
        }
        return string.Empty;
    }

    //插入列表
    public bool ExecuteInsterList(string table, List<BsonDocument> blist)
    {
        if (blist == null || blist.Count == 0)
            return false;
        try
        {
            
            var cb = mMongodbClient.GetCollection(table);
            var ret = cb.InsertBatch(blist);

            var it = ret.GetEnumerator();
        }
        catch (Exception)
        {            
            return false;
        }

        return true;
    }

    // 返回表table中，满足查询条件的记录个数
    public long ExecuteGetCount(string table, IMongoQuery queries)
    {
        try
        {
            var cb = mMongodbClient.GetCollection(table);
            if (queries == null)
            {
                return cb.Count();
            }
            return cb.Count(queries);
        }
        catch (Exception)
        {
            
        }
        return 0;
    }

    public MapReduceResult executeMapReduce(string table, IMongoQuery query, string map_js, string reduce_js)
    {
        try
        {
            var cb = mMongodbClient.GetCollection(table);
            MapReduceArgs args = new MapReduceArgs();
            args.MapFunction = new BsonJavaScript(map_js);
            args.ReduceFunction = new BsonJavaScript(reduce_js);
            args.Query = query;
            
            var ret = cb.MapReduce(args);
            if (ret.Ok)
                return ret;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        return null;
    }

    protected virtual string get_dbname()
    {
        return string.Empty;
    }

    protected virtual string get_url()
    {
        return string.Empty;
    }

    //初始化表 增加索引
    protected virtual void init_table()
    {

    }
}
