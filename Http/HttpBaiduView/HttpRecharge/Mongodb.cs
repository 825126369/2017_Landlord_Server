using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

using MongoDB.Driver;

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
        var tmp = mMongodbClient.GetCollection("anysdk_pay");
        if (!tmp.IndexExists("OrderID"))
            tmp.CreateIndex("OrderID");

        string[] indexes = new string[] { "Account", "OrderID", "Process" };
        if (!tmp.IndexExists(indexes))
            tmp.CreateIndex(indexes);

        indexes = new string[] { "Account", "Process" };
        if (!tmp.IndexExists(indexes))
            tmp.CreateIndex(indexes);

        tmp = mMongodbClient.GetCollection("pay_infos");
        if (!tmp.IndexExists("date"))
            tmp.CreateIndex("date");

        tmp = mMongodbClient.GetCollection("anysdk_pay");
        if (!tmp.IndexExists("OrderID"))
            tmp.CreateIndex("OrderID");
    }
}
