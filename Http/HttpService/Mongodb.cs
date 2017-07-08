using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Bson;

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
        var tmp = mMongodbClient.GetCollection("AccountTable");

        string[] indexes = new string[] { "acc", "platform" };
        if (!tmp.IndexExists(indexes))
            tmp.CreateIndex(indexes);

        indexes = new string[] { "acc_real" };
        if (!tmp.IndexExists(indexes))
            tmp.CreateIndex(indexes);

		indexes = new string[] { "acc_dev" };
        if (!tmp.IndexExists(indexes))
            tmp.CreateIndex(indexes);

        indexes = new string[] { "acc" };
        if (!tmp.IndexExists(indexes))
            tmp.CreateIndex(indexes);

        //////////////////////////////////////////////////////////
        //anysdk
        tmp = mMongodbClient.GetCollection("anysdk_login");

        indexes = new string[] { "uid" };
        if (!tmp.IndexExists(indexes))
            tmp.CreateIndex(indexes);
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
        var tmp = mMongodbClient.GetCollection("anysdk_pay");
        if (!tmp.IndexExists("Account"))
            tmp.CreateIndex("Account");

        if (!tmp.IndexExists("OrderID"))
            tmp.CreateIndex("OrderID");

        string[] indexes = new string[] { "Account", "OrderID", "Process" };
        if (!tmp.IndexExists(indexes))
            tmp.CreateIndex(indexes);

        indexes = new string[] { "Account", "Process" };
        if (!tmp.IndexExists(indexes))
            tmp.CreateIndex(indexes);

    }
}

public class ExceptionCheckInfo
{
    public static void doSaveCheckInfo(HttpRequest Request, string type)
    {
        string strId = Request.Params["serverId"];
        if (!string.IsNullOrEmpty(strId))
        {
            int serverId = Convert.ToInt32(strId);
            Dictionary<string, object> check = new Dictionary<string, object>();
            check.Add("serverId", serverId);
            check.Add("type", type);
            check.Add("time", DateTime.Now);
            IMongoQuery imq1 = Query.EQ("serverId", BsonValue.Create(serverId));
            IMongoQuery imq2 = Query.EQ("type", BsonValue.Create(type));
            IMongoQuery imq = Query.And(imq1, imq2);
            MongodbAccount.Instance.ExecuteStoreByQuery("svrExceptionCheck", imq, check);
        }
    }
}
