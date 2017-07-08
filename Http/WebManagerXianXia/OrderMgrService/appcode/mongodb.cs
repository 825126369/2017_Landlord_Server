using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Bson;

class MongodbPlayer : MongodbHelper<MongodbPlayer>
{
    protected override string get_dbname()
    {
        return "PlayerDB_DWC";
    }

    protected override string get_url()
    {
        XmlConfig xml = ResMgr.getInstance().getRes("dbserver.xml");
        string dbURL = xml.getString("mongodbPlayer", "");
        return dbURL;
    }

    protected override void init_table()
    {
        var tmp = mMongodbClient.GetCollection(TableName.PLAYER_ORDER_REQ);
        if (!tmp.IndexExists("key"))
            tmp.CreateIndex("key");
    }
}

