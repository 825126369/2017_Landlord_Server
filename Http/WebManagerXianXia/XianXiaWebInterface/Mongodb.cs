using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Bson;
using System.Web.Configuration;

class MongodbPlayer : MongodbHelper<MongodbPlayer>
{
    protected override string get_dbname()
    {
        return "PlayerDB_DWC";
    }

    protected override string get_url()
    {
        return WebConfigurationManager.AppSettings["Mongodb"];
    }

    protected override void init_table()
    {
        var tmp = mMongodbClient.GetCollection("KickPlayer");
        
        if (!tmp.IndexExists("key"))
            tmp.CreateIndex("key");     
    }
}
