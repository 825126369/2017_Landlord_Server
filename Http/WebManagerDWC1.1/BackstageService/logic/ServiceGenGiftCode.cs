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

// 礼包码生成服务
public class ServiceGenGiftCode : ServiceBase
{
    private ThreadState m_curState;
    private bool m_run = true;
    private object m_lockObj = new object();
    
    // 待生成队列
    private Queue<ParamGenGiftCode> m_taskQue = new Queue<ParamGenGiftCode>();
    private Thread m_thread = null;

    public ServiceGenGiftCode()
    {
        m_sysType = ServiceType.serviceTypeGiftCode;
    }

    public override void initService()
    {
        m_curState = ThreadState.state_idle;
        m_thread = new Thread(new ThreadStart(run));
        m_thread.Start();
    }

    public override void exitService()
    {
        m_run = false;
        m_thread = null;
    }

    public override bool isBusy()
    {
        if (m_curState == ThreadState.state_busy)
            return true;

        if (m_taskQue.Count > 0)
            return true;

        return false;
    }

    public override int doService(ServiceParam param)
    {
        if (param == null || m_thread == null)
            return (int)RetCode.ret_error;

        ParamGenGiftCode p = (ParamGenGiftCode)param;
        lock (m_lockObj)
        {
            m_taskQue.Enqueue(p);
            m_curState = ThreadState.state_busy;
        }
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
                        ParamGenGiftCode data = null;
                        DateTime now = DateTime.Now;
                        while (m_taskQue.Count > 0)
                        {
                            lock (m_lockObj)
                            {
                                data = m_taskQue.Dequeue();
                            }

                            if (!m_run)
                            {
                                break;
                            }

                            if (data != null)
                            {
                                genGiftCode(data, now);
                            }
                        }
                        m_curState = ThreadState.state_idle;
                    }
                    break;
            }
        }
    }

    private void genGiftCode(ParamGenGiftCode codeParam, DateTime now)
    {
        int i = 0, j = 0;
        List<Dictionary<string, object>> docList = new List<Dictionary<string, object>>();

        int serverId = DBMgr.getInstance().getDbId(codeParam.m_dbServerIP);
        if (serverId == -1)
            return;

        for (i = 0; i < codeParam.m_codeList.Count; i++)
        {
            GiftCodeInfo info = codeParam.m_codeList[i];

            for (j = 0; j < info.m_count; j++)
            {
                Dictionary<string, object> ret = createOneCode(info.m_giftId, info.m_plat, now);
                docList.Add(ret);

                if (docList.Count > 5000)
                {
                    DBMgr.getInstance().insertData(TableName.GIFT_CODE, docList, serverId, DbName.DB_ACCOUNT);
                    docList.Clear();
                }
            }
        }

        if (docList.Count > 0)
        {
            DBMgr.getInstance().insertData(TableName.GIFT_CODE, docList, serverId, DbName.DB_ACCOUNT);
        }
    }

    private Dictionary<string, object> createOneCode(long giftId, string plat, DateTime now)
    {
        Dictionary<string, object> tmp = new Dictionary<string, object>();
        tmp.Add("genTime", now);
        // 礼包码
        tmp.Add("giftCode", generateStringID());
        // 对应的礼包ID
        tmp.Add("giftId", giftId);
        // 为哪个平台生成的
        tmp.Add("plat", plat); 

        // 玩家使用礼包码时所在服务器id
        tmp.Add("playerServerId", -1);
        // 玩家平台
        tmp.Add("playerPlat", "");
        tmp.Add("playerId", -1);
        tmp.Add("playerAcc", "");
        // 使用时间
        tmp.Add("useTime", DateTime.MinValue);
        return tmp;
    }

    private string generateStringID()
    {
        long i = 1;
        byte[] arr = Guid.NewGuid().ToByteArray();
        foreach (byte b in arr)
        {
            i *= ((int)b + 1);
        }
        return string.Format("{0:x}", i - DateTime.Now.Ticks);
    }
}



