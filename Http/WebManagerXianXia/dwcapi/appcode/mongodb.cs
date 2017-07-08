using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Bson;
using System.Web.Configuration;

class MongodbLog : MongodbHelper<MongodbLog>
{
    protected override string get_dbname()
    {
        return "LogDB_DWC";
    }

    protected override string get_url()
    {
        return WebConfigurationManager.AppSettings["mongodbLog"];
    }

    protected override void init_table()
    {   
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
        return WebConfigurationManager.AppSettings["mongodbPlayer"];
    }

    protected override void init_table()
    {
    }
}

class MongodbAcc : MongodbHelper<MongodbAcc>
{
    protected override string get_dbname()
    {
        return "AccountDB";
    }

    protected override string get_url()
    {
        return WebConfigurationManager.AppSettings["mongodbAcc"];
    }

    protected override void init_table()
    {
    }
}

