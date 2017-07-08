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
}

class MongodbPlayer : MongodbHelper<MongodbPlayer>
{
    protected override string get_dbname()
    {
        return "PlayerDB_DWC";
    }

    protected override string get_url()
    {
        return ConfigurationManager.ConnectionStrings["Mongodb"].ConnectionString;
    }
}