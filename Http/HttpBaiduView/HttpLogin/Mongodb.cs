using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

using MongoDB.Driver;

class MongodbAccount : MongodbHelper<MongodbAccount>
{
    protected override string get_dbname()
    {
        return "AccountDB";
    }

    protected override string get_url()
    {
        return ConfigurationManager.ConnectionStrings["Mongodb"].ConnectionString;
    }

    protected override void init_table()
    {
        //default
        var tmp = mMongodbClient.GetCollection("AccountTable");

        if (!tmp.IndexExists("acc"))
            tmp.CreateIndex("acc");

        if (!tmp.IndexExists("acc_dev"))
            tmp.CreateIndex("acc_dev");

        if (!tmp.IndexExists("acc_real"))
            tmp.CreateIndex("acc_real");

        if (!tmp.IndexExists("bindPhone"))
            tmp.CreateIndex("bindPhone");

        //////////////////////////////////////////////////////////
        //anysdk
        tmp = mMongodbClient.GetCollection("anysdk_login");

        if (!tmp.IndexExists("acc"))
            tmp.CreateIndex("acc");

        //////////////////////////////////////////////////////////
        //day info
        tmp = mMongodbClient.GetCollection("link_phone");
        if (!tmp.IndexExists("phone"))
            tmp.CreateIndex("phone");

        tmp = mMongodbClient.GetCollection("day_activation");
        if (!tmp.IndexExists("date"))
            tmp.CreateIndex("date");

        tmp = mMongodbClient.GetCollection("day_regedit");
        if (!tmp.IndexExists("date"))
            tmp.CreateIndex("date");
    }
}



class MongodbConfig : MongodbHelper<MongodbConfig>
{
    protected override string get_dbname()
    {
        return "ConfigDB";
    }

    protected override string get_url()
    {
        return ConfigurationManager.ConnectionStrings["Mongodb"].ConnectionString;
    }

    protected override void init_table()
    {
        var tmp = mMongodbClient.GetCollection("Versions");
        tmp.CreateIndex("type");

        tmp = mMongodbClient.GetCollection("Errors");
        tmp.CreateIndex("ver");
        tmp.CreateIndex("game");
        tmp.CreateIndex("time");

        tmp = mMongodbClient.GetCollection("TestServers");
        tmp.CreateIndex("channel");
    }
}



class MongodbPayment : MongodbHelper<MongodbPayment>
{
    protected override string get_dbname()
    {
        return "PaymentDB";
    }

    protected override string get_url()
    {
        return ConfigurationManager.ConnectionStrings["Mongodb"].ConnectionString;
    }

    protected override void init_table()
    {
        //anysdk
        var tmp = mMongodbClient.GetCollection("ex_orderinfo");
        if (!tmp.IndexExists("OrderID"))
            tmp.CreateIndex("OrderID");
    }
}
