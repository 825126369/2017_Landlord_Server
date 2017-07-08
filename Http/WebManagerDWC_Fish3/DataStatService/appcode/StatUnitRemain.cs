using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System.Windows.Forms;

// 统计次日，3日留存率，7日留存，30日留存
public class StatUnitRemain : StatBase
{
    public const string ACC_KEY = "acc_real";

    // 注册账号列表
    protected List<string> m_regeditAccList = new List<string>();

    protected List<string> m_regeditDevList = new List<string>();

    public override void doStat(object param, StatResult result)
    {
        ParamStat p = (ParamStat)param;
        stat2DayRemain(p, result);
        stat3DayRemain(p, result);
        stat7DayRemain(p, result);
        stat30DayRemain(p, result);
    }

    // 统计次日留存
    private void stat2DayRemain(ParamStat param, StatResult result)
    {
        statXDayRemain(param, 1, ref result.m_2DayRemainCountTmp);
        statXDayDevRemain(param, 1, ref result.m_2DayDevRemainCountTmp);
    }

    // 统计3日留存
    private void stat3DayRemain(ParamStat param, StatResult result)
    {
        statXDayRemain(param, 3, ref result.m_3DayRemainCountTmp);
        statXDayDevRemain(param, 3, ref result.m_3DayDevRemainCountTmp);
    }

    // 统计7日留存
    private void stat7DayRemain(ParamStat param, StatResult result)
    {
        statXDayRemain(param, 7, ref result.m_7DayRemainCountTmp);
        statXDayDevRemain(param, 7, ref result.m_7DayDevRemainCountTmp);
    }

    // 统计30日留存
    private void stat30DayRemain(ParamStat param, StatResult result)
    {
        statXDayRemain(param, 30, ref result.m_30DayRemainCountTmp);
        statXDayDevRemain(param, 30, ref result.m_30DayDevRemainCountTmp);
    }

    // 统计留存
    private void statXDayRemain(ParamStat param, int days, ref int remainCount)
    {
        m_regeditAccList.Clear();

        ChannelInfo statDay = (ChannelInfo)param.m_channel;
        queryRegeditAcc(param, StatBase.getRemainRegTime(statDay, days));

        int regeditCount = m_regeditAccList.Count;
        if (regeditCount == 0) // 没有注册,此时今天的留存是0
            return;

        DateTime mint = statDay.m_statDay.Date.AddDays(-1);
        DateTime maxt = statDay.m_statDay.Date;
        int count = 0;
        foreach (var acc in m_regeditAccList)
        {
            if (isLogin(param, ACC_KEY, acc, mint, maxt))
            {
                count++;
            }
        }

        remainCount = count;
    }

    /*      
     *          获取某渠道在指定日的注册列表
     *          dt  指定日期
     */
    protected void queryRegeditAcc(ParamStat param, DateTime dt)
    {
        ChannelInfo cinfo = param.m_channel;

        DateTime mint = dt.Date, maxt = dt.Date.AddDays(1);

        IMongoQuery imq1 = Query.LT("time", BsonValue.Create(maxt));
        IMongoQuery imq2 = Query.GTE("time", BsonValue.Create(mint));
        IMongoQuery imq3 = Query.EQ("channel", BsonValue.Create(cinfo.m_channelNum));

        IMongoQuery imq = Query.And(imq1, imq2, imq3);

        List<Dictionary<string, object>> dataList =
            MongodbAccount.Instance.ExecuteGetListByQuery(cinfo.m_regeditTable, imq, new string[] { ACC_KEY });
        for (int i = 0; i < dataList.Count; i++)
        {
            if (dataList[i].ContainsKey(ACC_KEY))
            {
                string acc = Convert.ToString(dataList[i][ACC_KEY]);
                m_regeditAccList.Add(acc);
            }
        }
    }

    // 指定渠道的账号是否在一段时间内登录过
    private bool isLogin(ParamStat param, string key, string acc, DateTime mint, DateTime maxt)
    {
        IMongoQuery imq1 = Query.LT("time", BsonValue.Create(maxt));
        IMongoQuery imq2 = Query.GTE("time", BsonValue.Create(mint));
        IMongoQuery imq3 = Query.EQ(key, BsonValue.Create(acc));
        IMongoQuery imq4 = Query.EQ("channel", BsonValue.Create(param.m_channel.m_channelNum));
        IMongoQuery imq = Query.And(imq1, imq2, imq3, imq4);

        bool res = MongodbAccount.Instance.KeyExistsByQuery(param.m_channel.m_loginTable, imq);
        return res;
    }

    //////////////////////////////////////////////////////////////////////////
    // 统计设备留存
    private void statXDayDevRemain(ParamStat param, int days, ref int remainCount)
    {
        m_regeditDevList.Clear();

        ChannelInfo statDay = (ChannelInfo)param.m_channel;
        queryRegeditDev(param, StatBase.getRemainRegTime(statDay, days));

        int regeditCount = m_regeditDevList.Count;
        if (regeditCount == 0) // 没有注册,此时今天的留存是0
            return;

        DateTime mint = statDay.m_statDay.Date.AddDays(-1);
        DateTime maxt = statDay.m_statDay.Date;
        int count = 0;
        foreach (var acc in m_regeditDevList)
        {
            if (isLogin(param, "acc_dev", acc, mint, maxt))
            {
                count++;
            }
        }

        remainCount = count;
    }

    protected void queryRegeditDev(ParamStat param, DateTime dt)
    {
        ChannelInfo cinfo = param.m_channel;

        DateTime mint = dt.Date, maxt = dt.Date.AddDays(1);

        IMongoQuery imq1 = Query.LT("active_time", BsonValue.Create(maxt));
        IMongoQuery imq2 = Query.GTE("active_time", BsonValue.Create(mint));
        IMongoQuery imq3 = Query.EQ("channel", BsonValue.Create(cinfo.m_channelNum));

        IMongoQuery imq = Query.And(imq1, imq2, imq3);

        List<Dictionary<string, object>> dataList =
            MongodbAccount.Instance.ExecuteGetListByQuery(cinfo.m_deviceActivationTable, imq, new string[] { "phone" });
        for (int i = 0; i < dataList.Count; i++)
        {
            if (dataList[i].ContainsKey("phone"))
            {
                string acc = Convert.ToString(dataList[i]["phone"]);
                m_regeditDevList.Add(acc);
            }
        }
    }
}















