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

    SysMgr m_sysMgr = new SysMgr();

    public StatService()
    {
        m_sysType = ServiceType.serviceTypeStat;
    }

    public override void initService()
    {
        m_sysMgr.addSys(new SysStatPlayerTotalMoney(), SysType.systype_stat_player_total_money);
        m_sysMgr.addSys(new SysStatRemain(), SysType.systype_remain_stat);

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

















