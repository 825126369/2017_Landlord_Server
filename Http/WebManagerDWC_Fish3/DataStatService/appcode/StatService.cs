using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System.Windows.Forms;
using System.Diagnostics;

public class StatService : ServiceBase
{
    private Thread m_threadWork = null;
    
    private bool m_run = true;

    private DataStatService.Form1 m_form;

    // 统计模块
    private Dictionary<int, StatBase> m_statModule = new Dictionary<int, StatBase>();

    SysMgr m_sysMgr = new SysMgr();

    public StatService()
    {
        m_sysType = ServiceType.serviceTypeStat;
    }

    public override void initService()
    {
        //initStatModule();
        //initChannel();

        m_sysMgr.addSys(new SysStatPlayerTotalMoney(), SysType.systype_stat_player_total_money);
        m_sysMgr.addSys(new SysStatRemain(), SysType.systype_remain_stat);
        m_sysMgr.addSys(new SysStatLose(), SysType.systype_lose_stat);
        //m_sysMgr.addSys(new StatPlayerDragonBall(), SysType.systype_PlayerDragonBall);
        m_sysMgr.addSys(new StatDragonBallTotal(), SysType.systype_DragonBallTotal);
        m_sysMgr.addSys(new StatOnlineGameTime(), SysType.systype_OnlineGameTime);
        //m_sysMgr.addSys(new StatPlayerTotalIncomeExpenses(), SysType.systype_PlayerTotalIncomeExpenses);

        m_sysMgr.addSys(new StatRechargePerHour(), SysType.systype_RechargeHour);
        m_sysMgr.addSys(new StatOnlinePlayerNumPerHour(), SysType.systype_OnlinePlayerNumHour);
        m_sysMgr.addSys(new StatGameTimeForPlayerActive(), SysType.systype_GameTimeForPlayerActive);
        m_sysMgr.addSys(new StatFirstRecharge(), SysType.systype_FirstRecharge);
        m_sysMgr.addSys(new StatPlayerGameBet(), SysType.systype_PlayerGameBet);

        m_sysMgr.init();

        m_threadWork = new Thread(new ThreadStart(run));
        m_threadWork.Start();
    }

    public override void exitService()
    {
        m_run = false;
        if (m_threadWork != null)
        {
            try
            {
                m_threadWork.Interrupt();
            }
            catch (System.Exception ex)
            {
            }
        }
        m_threadWork.Join();
        m_form = null;
    }

    public void setForm(DataStatService.Form1 f)
    {
        m_form = f;
        if (m_form != null)
        {
            XmlConfig xml = ResMgr.getInstance().getRes("dbserver.xml");
            string dbURL = xml.getString("mongodbAccount", "");
            m_form.setDbIP(dbURL);
        }
    }

    private void run()
    {
        while (m_run)
        {
            try
            {
                if (m_form != null)
                {
                    m_form.setState(0);
                }

//                 List<ChannelInfo> channelList = ResMgr.getInstance().getChannelList();
//                 foreach (var info in channelList)
//                 {
//                     bool res = statChannel(info);
//                     if (res)
//                     {
//                         resetChannelStatDay(info);
//                     }
//                 }
                m_sysMgr.update(0);

                if (m_form != null)
                {
                    m_form.setState(1);
                }
            }
            catch (System.Exception ex)
            {
                LogMgr.log.Error(ex.ToString());
            }

            if (m_run)
            {
                try
                {
                    XmlConfig xml = ResMgr.getInstance().getRes("dbserver.xml");
                    int sec = xml.getInt("statInterval", 60);
                    Thread.Sleep(sec);
                }
                catch (System.Exception ex)
                {
                }
            }
        }
    }

    private void initStatModule()
    {
        m_statModule.Add(StatFlag.STAT_FLAG_ACTIVE, new StatUnitActive());
        m_statModule.Add(StatFlag.STAT_FLAG_RECHARGE, new StatUnitRecharge());
        m_statModule.Add(StatFlag.STAT_FLAG_REMAIN, new StatUnitRemain());
        m_statModule.Add(StatFlag.STAT_FLAG_COUNT, new StatUnitCount());
    }

    private void initChannel()
    {
        List<ChannelInfo> channelList = ResMgr.getInstance().getChannelList();
        foreach (var info in channelList)
        {
            Dictionary<string, object> data =
                MongodbAccount.Instance.ExecuteGetOneBykey(TableName.CHANNEL_STAT_DAY, "channel", info.m_channelNum);
            if (data != null)
            {
                info.m_statDay = Convert.ToDateTime(data["statDay"]).ToLocalTime();
            }
            else
            {
                DateTime now = DateTime.Now.Date;
                info.m_statDay = now.AddDays(1);

                Dictionary<string, object> newData = new Dictionary<string, object>();
                newData.Add("statDay", info.m_statDay);
                newData.Add("channel", info.m_channelNum);
                MongodbAccount.Instance.ExecuteStoreBykey(TableName.CHANNEL_STAT_DAY, "channel", info.m_channelNum, newData);
            }
        }
    }

    private bool statChannel(ChannelInfo info)
    {
        if (!StatBase.canStat(info))
            return false;

        ParamStat param = new ParamStat();
        param.m_channel = info;

        StatResult result = new StatResult();

        foreach (var stat in m_statModule.Values)
        {
            stat.doStat(param, result);
        }

        Dictionary<string, object> newData = new Dictionary<string, object>();
        newData.Add("genTime", info.m_statDay.Date.AddDays(-1));
        newData.Add("channel", info.m_channelNum);

        newData.Add("regeditCount", result.m_regeditCount);
        newData.Add("deviceActivationCount", result.m_deviceActivationCount);
        newData.Add("activeCount", result.m_activeCount);

        newData.Add("totalIncome", result.m_totalIncome);
        newData.Add("rechargePersonNum", result.m_rechargePersonNum);
        newData.Add("rechargeCount", result.m_rechargeCount);

       // newData.Add("2DayRegeditCount", result.m_2DayRegeditCount);
        newData.Add("2DayRemainCount", result.m_2DayRemainCount);

       // newData.Add("3DayRegeditCount", result.m_3DayRegeditCount);
        newData.Add("3DayRemainCount", result.m_3DayRemainCount);

       // newData.Add("7DayRegeditCount", result.m_7DayRegeditCount);
        newData.Add("7DayRemainCount", result.m_7DayRemainCount);

        //newData.Add("30DayRegeditCount", result.m_30DayRegeditCount);
        newData.Add("30DayRemainCount", result.m_30DayRemainCount);

        IMongoQuery imq1 = Query.EQ("genTime", BsonValue.Create(info.m_statDay.Date.AddDays(-1)));
        IMongoQuery imq2 = Query.EQ("channel", BsonValue.Create(info.m_channelNum));
        IMongoQuery imq = Query.And(imq1, imq2);

        updateRemain(info, 1, result);
        updateRemain(info, 3, result);
        updateRemain(info, 7, result);
        updateRemain(info, 30, result);

        string str = MongodbAccount.Instance.ExecuteStoreByQuery(TableName.CHANNEL_TD, imq, newData);
        return str == string.Empty;
    }

    private void resetChannelStatDay(ChannelInfo info)
    {
        Dictionary<string, object> upData = new Dictionary<string, object>();
        upData.Add("statDay", info.m_statDay.Date.AddDays(1));
        MongodbAccount.Instance.ExecuteStoreBykey(TableName.CHANNEL_STAT_DAY, "channel", info.m_channelNum, upData);
        info.m_statDay = info.m_statDay.Date.AddDays(1);
    }

    // 更新往日留存
    private void updateRemain(ChannelInfo info, int days, StatResult result)
    {
        IMongoQuery imq1 = Query.EQ("genTime", StatBase.getRemainRegTime(info, days));
        IMongoQuery imq2 = Query.EQ("channel", BsonValue.Create(info.m_channelNum));
        IMongoQuery imq = Query.And(imq1, imq2);

        bool res = MongodbAccount.Instance.KeyExistsByQuery(TableName.CHANNEL_TD, imq);
        if (res)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            switch (days)
            {
                case 1:
                    {
                        data.Add("2DayRemainCount", result.m_2DayRemainCountTmp);
                    }
                    break;
                case 3:
                    {
                        data.Add("3DayRemainCount", result.m_3DayRemainCountTmp);
                    }
                    break;
                case 7:
                    {
                        data.Add("7DayRemainCount", result.m_7DayRemainCountTmp);
                    }
                    break;
                case 30:
                    {
                        data.Add("30DayRemainCount", result.m_30DayRemainCountTmp);
                    }
                    break;
            }

            MongodbAccount.Instance.ExecuteUpdateByQuery(TableName.CHANNEL_TD, imq, data);
        }
    }
}

//////////////////////////////////////////////////////////////////////////
public class StatBase
{
    /*
     *      统计
     *      param   输入参数
     *      result  返回统计结果
     */
    public virtual void doStat(object param, StatResult result) { }

    // 当前可否统计 返回true可以作统计了
    public static bool canStat(ChannelInfo param) 
    {
        DateTime now = DateTime.Now.Date;
        if (now >= param.m_statDay.Date)
            return true;

        return false;
    }

    /*
     *          返回留存计算时的注册日
     *          days 1为次日  3为3日.....
     */
    public static DateTime getRemainRegTime(ChannelInfo param, int days)
    {
        return param.m_statDay.AddDays(-(days + 1));
    }

    // 返回充值统计条件
    public static IMongoQuery getRechargeCond(ParamStat param, int days)
    {
        ChannelInfo cinfo = param.m_channel;

        DateTime mint = cinfo.m_statDay.Date.AddDays(-days), maxt = cinfo.m_statDay.Date;
        IMongoQuery imq1 = Query.LT("time", BsonValue.Create(maxt));
        IMongoQuery imq2 = Query.GTE("time", BsonValue.Create(mint));
        IMongoQuery imq3 = Query.EQ("channel", BsonValue.Create(cinfo.m_channelNum));

        IMongoQuery imq = Query.And(imq1, imq2, imq3);
        return imq;
    }
}

















