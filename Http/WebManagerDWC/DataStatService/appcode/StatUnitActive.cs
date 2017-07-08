using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System.Windows.Forms;

// 统计活跃
public class StatUnitActive : StatBase
{
    public override void doStat(object param, StatResult result)
    {
        ParamStat p = (ParamStat)param;

        ChannelInfo cinfo = p.m_channel;

        DateTime mint = cinfo.m_statDay.Date.AddDays(-1), maxt = cinfo.m_statDay.Date;
        IMongoQuery imq1 = Query.LT("time", BsonValue.Create(maxt));
        IMongoQuery imq2 = Query.GTE("time", BsonValue.Create(mint));
        IMongoQuery imq3 = Query.EQ("channel", BsonValue.Create(cinfo.m_channelNum));

        IMongoQuery imq = Query.And(imq1, imq2, imq3);

        MapReduceResult mapResult = MongodbAccount.Instance.executeMapReduce(cinfo.m_loginTable,
                                                                             imq,
                                                                             MapReduceTable.getMap("active"),
                                                                             MapReduceTable.getReduce("active"));
        if (mapResult != null)
        {
            result.m_activeCount = (int)mapResult.OutputCount;
        }
    }
}















