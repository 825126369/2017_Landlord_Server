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

public class StatService2 : ServiceBase
{
    private Thread m_threadWork = null;
    
    private bool m_run = true;

    private DataStatService.Form1 m_form;

    // 统计模块
    private Dictionary<int, StatBase> m_statModule = new Dictionary<int, StatBase>();

    SysMgr m_sysMgr = new SysMgr();

    public StatService2()
    {
        m_sysType = ServiceType.serviceTypeStat2;
    }

    public override void initService()
    {
        m_sysMgr.addSys(new StatPlayerTotalIncomeExpensesNew(), SysType.systype_PlayerTotalIncomeExpensesNew);

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

    private void run()
    {
        while (m_run)
        {
            try
            {
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















