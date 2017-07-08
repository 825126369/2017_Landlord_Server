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
using CheckBalance;

enum State
{
    idle,
    busy,
}

public class ParamCheck
{
    public int m_playerId;
    public DateTime m_startTime;
    public DateTime m_endTime;
    public int m_gameId;
    public int m_itemId;

    public Form1 m_from;
}

public class StatService : ServiceBase
{
    private Thread m_threadWork = null;
    
    private bool m_run = true;

    State m_state = State.idle;

    StatPlayerTotalIncomeExpenses m_check = new StatPlayerTotalIncomeExpenses();

    ParamCheck m_param;

    public StatService()
    {
        m_sysType = ServiceType.serviceTypeStat;
    }

    public override void initService()
    {
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
    }

    public bool isBusy()
    {
        return m_state == State.busy;
    }

    public void startCheck(ParamCheck param)
    {
        m_param = param;
        m_state = State.busy;
    }

    private void run()
    {
        while (m_run)
        {
            try
            {
                switch (m_state)
                {
                    case State.busy:
                        {
                            m_check.check(m_param);
                            m_state = State.idle;
                        }
                        break;
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










