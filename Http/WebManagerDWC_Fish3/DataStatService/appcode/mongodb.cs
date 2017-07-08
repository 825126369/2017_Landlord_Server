using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Bson;

// 账号数据库
class MongodbAccount : MongodbHelper<MongodbAccount>
{
    protected override string get_dbname()
    {
        return "AccountDB";
    }

    protected override string get_url()
    {
        XmlConfig xml = ResMgr.getInstance().getRes("dbserver.xml");
        string dbURL = xml.getString("mongodbAccount", "");
        return dbURL;
    }

    protected override void init_table()
    {
        try
        {
            var table1 = GetDB.GetCollection("RegisterLog");
            if (table1 != null)
            {
                table1.CreateIndex(IndexKeys.Ascending(StatUnitRemain.ACC_KEY), IndexOptions.SetBackground(true));
                table1.CreateIndex(IndexKeys.Ascending("channel"), IndexOptions.SetBackground(true));
                table1.CreateIndex(IndexKeys.Ascending("time"), IndexOptions.SetBackground(true));
            }

            var table2 = GetDB.GetCollection("link_phone");
            if (table2 != null)
            {
                table2.CreateIndex(IndexKeys.Ascending("channel"), IndexOptions.SetBackground(true));
                table2.CreateIndex(IndexKeys.Ascending("active_time"), IndexOptions.SetBackground(true));
            }

            var table3 = GetDB.GetCollection(TableName.PLAYER_LOGIN);
            if (table3 != null)
            {
                table3.CreateIndex(IndexKeys.Ascending("channel"), IndexOptions.SetBackground(true));
                table3.CreateIndex(IndexKeys.Ascending("time"), IndexOptions.SetBackground(true));
                table3.CreateIndex(IndexKeys.Ascending(StatUnitRemain.ACC_KEY), IndexOptions.SetBackground(true));
                table3.CreateIndex(IndexKeys.Ascending("acc_dev"), IndexOptions.SetBackground(true));
            }
        }
        catch (System.Exception ex)
        {
        }
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
        XmlConfig xml = ResMgr.getInstance().getRes("dbserver.xml");
        string dbURL = xml.getString("mongodbAccount", "");
        return dbURL;
    }

    protected override void init_table()
    {
        try
        {
            var table = GetDB.GetCollection("PayLog");
            table.CreateIndex(IndexKeys.Ascending(StatLTV.ACC_KEY_1), IndexOptions.SetBackground(true));
            table.CreateIndex(IndexKeys.Ascending("channel"), IndexOptions.SetBackground(true));
            table.CreateIndex(IndexKeys.Ascending("time"), IndexOptions.SetBackground(true));
        }
        catch (System.Exception ex)
        {
        }
    }
}

// 玩家数据库
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
    }
}

// 日志数据库
class MongodbLog : MongodbHelper<MongodbLog>
{
    protected override string get_dbname()
    {
        return "LogDB_DWC";
    }

    protected override string get_url()
    {
        XmlConfig xml = ResMgr.getInstance().getRes("dbserver.xml");
        string dbURL = xml.getString("mongodbLog", "");
        return dbURL;
    }

    protected override void init_table()
    {
        try
        {
            var table = GetDB.GetCollection(TableName.PUMP_PLAYER_MONEY);
            table.CreateIndex(IndexKeys.Ascending("genTime"), IndexOptions.SetBackground(true));

            var table2 = GetDB.GetCollection(TableName.STAT_INCOME_EXPENSES);
            table2.CreateIndex(IndexKeys.Ascending("genTime"), IndexOptions.SetBackground(true));
        }
        catch (System.Exception ex)
        {
        }
    }
}

// 游戏数据库
class MongodbGame : MongodbHelper<MongodbGame>
{
    protected override string get_dbname()
    {
        return "GameDB";
    }

    protected override string get_url()
    {
        XmlConfig xml = ResMgr.getInstance().getRes("dbserver.xml");
        string dbURL = xml.getString("mongodbGame", "");
        return dbURL;
    }

    protected override void init_table()
    {
    }
}
