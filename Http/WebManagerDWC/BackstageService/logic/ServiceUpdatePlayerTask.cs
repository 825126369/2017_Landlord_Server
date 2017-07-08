using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using System.IO;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

public class ServiceUpdatePlayerTask : ServiceBase
{
    private ThreadState m_curState;
    private bool m_run = true;
    private string[] m_playerFields = { "player_id" };
    private object m_lockObj = new object();
    ServiceParam m_param;

    public ServiceUpdatePlayerTask()
    {
        m_sysType = ServiceType.serviceTypeUpdatePlayerTask;
    }

    public override void initService()
    {
        m_curState = ThreadState.state_idle;
        Thread t = new Thread(new ThreadStart(run));
        t.Start();
    }

    public override void exitService()
    {
        m_run = false;
    }

    public override bool isBusy()
    {
        if (m_curState == ThreadState.state_busy)
            return true;

        return false;
    }

    public override int doService(ServiceParam param)
    {
        if (param == null)
        {
            return (int)RetCode.ret_error;
        }
        if (m_curState == ThreadState.state_busy)
            return (int)RetCode.ret_busy;

        m_param = param;
        m_curState = ThreadState.state_busy;
        return (int)RetCode.ret_success;
    }

    private void run()
    {
        while (m_run)
        {
            switch (m_curState)
            {
                case ThreadState.state_idle:
                    {
                        Thread.Sleep(5000);
                    }
                    break;
                case ThreadState.state_busy:
                    {
                        updateTask(m_param);
                        m_curState = ThreadState.state_idle;
                    }
                    break;
            }
        }
    }

    private void updateTask(ServiceParam param)
    {
        int serverId = DBMgr.getInstance().getDbId(param.m_toServerIP);
        if (serverId == -1)
        {
            LogMgr.error("ServiceUpdatePlayerTask.updateTask, 找不到dbip:{0}", param.m_toServerIP);
            return;
        }

        IMongoQuery imq = Query.NE("player_id", 0);
        Dictionary<string, object> tmp = new Dictionary<string, object>();
        tmp.Add("NeedReflush", true);
        DBMgr.getInstance().updateAll(TableName.PLAYER_QUEST, tmp, imq, serverId, DbName.DB_PLAYER);
    }
}



