using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System.Windows.Forms;

// 统计付费
public class StatUnitRecharge : StatBase
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

        MapReduceResult mapResult = MongodbPayment.Instance.executeMapReduce(cinfo.m_paymentTable,
                                                                             imq,
                                                                             MapReduceTable.getMap("recharge"),
                                                                             MapReduceTable.getReduce("recharge"));
        if (mapResult != null)
        {
            IEnumerable<BsonDocument> bson = mapResult.GetResults();
            foreach (BsonDocument d in bson)
            {
                BsonValue resValue = d["value"];
                result.m_totalIncome += resValue["total"].ToInt32();
                result.m_rechargePersonNum++;
                result.m_rechargeCount += resValue["rechargeCount"].ToInt32();
            }
        }

        statPaytype(imq, cinfo);
    }

    void statPaytype(IMongoQuery imq, ChannelInfo cinfo)
    {
        MapReduceResult mapResult = MongodbPayment.Instance.executeMapReduce(cinfo.m_paymentTable,
                                                                             imq,
                                                                             MapReduceTable.getMap("rechargePayType"),
                                                                             MapReduceTable.getReduce("rechargePayType"));
        if (mapResult != null)
        {
            IEnumerable<BsonDocument> bson = mapResult.GetResults();
            foreach (BsonDocument d in bson)
            {
                int payType = Convert.ToInt32(d["_id"]);
                BsonValue resValue = d["value"];

                int total = resValue["total"].ToInt32();
                int rechargePerson = resValue["rechargePerson"].ToInt32();
                int rechargeCount = resValue["rechargeCount"].ToInt32();

                Dictionary<string, object> upData = new Dictionary<string, object>();
                upData.Add("genTime", cinfo.m_statDay.Date.AddDays(-1));
                upData.Add("channel", cinfo.m_channelNum);
                upData.Add(payType.ToString() + "_rmb", total);
                upData.Add(payType.ToString() + "_rechargePerson", rechargePerson);
                upData.Add(payType.ToString() + "_rechargeCount", rechargeCount);

                IMongoQuery upCond = Query.And(Query.EQ("genTime", cinfo.m_statDay.Date.AddDays(-1)),
                    Query.EQ("channel", cinfo.m_channelNum));
                string str = MongodbAccount.Instance.ExecuteStoreByQuery(TableName.CHANNEL_TD_PAY, upCond, upData);
            }
        }
    }
}

//////////////////////////////////////////////////////////////////////////
// 玩家价值
public class StatLTV : StatUnitRemain
{
    public const string ACC_KEY_1 = "real_acc";

    public override void doStat(object param, StatResult result)
    {
        ParamStat p = (ParamStat)param;
        statXDayTotalRecharge(p, 0, ref result.m_newAccIncome, result);
        statXDayTotalRecharge(p, 1, ref result.m_1DayTotalRechargeTmp);
        statXDayTotalRecharge(p, 3, ref result.m_3DayTotalRechargeTmp);
        statXDayTotalRecharge(p, 7, ref result.m_7DayTotalRechargeTmp);
        statXDayTotalRecharge(p, 14, ref result.m_14DayTotalRechargeTmp);
        statXDayTotalRecharge(p, 30, ref result.m_30DayTotalRechargeTmp);
        statXDayTotalRecharge(p, 60, ref result.m_60DayTotalRechargeTmp);
        statXDayTotalRecharge(p, 90, ref result.m_90DayTotalRechargeTmp);
    }

    // 统计几天前，注册玩家充值总额
    private void statXDayTotalRecharge(ParamStat param, int days, ref int totalRecharge, StatResult result = null)
    {
        totalRecharge = 0;
        m_regeditAccList.Clear();

        ChannelInfo statDay = (ChannelInfo)param.m_channel;
        queryRegeditAcc(param, StatBase.getRemainRegTime(statDay, days));
        if (m_regeditAccList.Count == 0) // 没有注册
            return;

        int playerCount = 0;
        IMongoQuery imq = StatBase.getRechargeCond(param, days + 1);
        foreach (var acc in m_regeditAccList)
        {
            int tmp = statPlayerRecharge(statDay, imq, acc);
            totalRecharge += tmp;

            if (tmp > 0)
            {
                playerCount++;
            }
        }

        if (days == 0)
        {
            result.m_newAccRechargePersonNum = playerCount;
        }
    }

    int statPlayerRecharge(ChannelInfo cinfo, IMongoQuery cond, string acc)
    {
        IMongoQuery imq = Query.And(cond, Query.EQ(StatLTV.ACC_KEY_1, acc));
        MapReduceResult mapResult = MongodbPayment.Instance.executeMapReduce(cinfo.m_paymentTable,
                                                                             imq,
                                                                             MapReduceTable.getMap("recharge"),
                                                                             MapReduceTable.getReduce("recharge"));
        int total = 0;
        if (mapResult != null)
        {
            IEnumerable<BsonDocument> bson = mapResult.GetResults();
            foreach (BsonDocument d in bson)
            {
                BsonValue resValue = d["value"];
                total += resValue["total"].ToInt32();
            }
        }
        return total;
    }
}













