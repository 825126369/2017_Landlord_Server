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

public class ServiceSendMail : ServiceBase
{
    private ThreadState m_curState;
    private bool m_run = true;
    private string[] m_playerFields = { "player_id" };
    private object m_lockObj = new object();
    
    // 要执行的任务
    private Dictionary<string, bool> m_taskFlag = new Dictionary<string, bool>();
    private Queue<ParamSendMailFullSvr> m_taskQue = new Queue<ParamSendMailFullSvr>();

    public ServiceSendMail()
    {
        m_sysType = ServiceType.serviceTypeSendMail;
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

        if (m_taskQue.Count > 0)
            return true;

        return false;
    }

    public override int doService(ServiceParam param)
    {
        if (param == null)
            return (int)RetCode.ret_error;

        ParamSendMailFullSvr p = (ParamSendMailFullSvr)param;
        lock (m_lockObj)
        {
            if (m_taskFlag.ContainsKey(p.m_dbServerIP))
            {
                return (int)RetCode.ret_busy;
            }
            m_taskFlag.Add(p.m_dbServerIP, true);
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
                        ParamSendMailFullSvr data = null;
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
                                sendMail(data);
                                lock (m_lockObj)
                                {
                                    try
                                    {
                                        m_taskFlag.Remove(data.m_dbServerIP);
                                    }
                                    catch (System.Exception ex)
                                    {
                                    }
                                }
                            }
                        }
                        m_curState = ThreadState.state_idle;
                    }
                    break;
            }
        }
    }

    private void sendMail(ParamSendMailFullSvr sendParam)
    {
        BsonDocument mailItem = null;

        if (sendParam.m_itemList != "")
        {
            List<ParamItem> tmpItem = new List<ParamItem>();
            Tool.parseItemList(sendParam.m_itemList, tmpItem);
           
            Dictionary<string, object> dd = new Dictionary<string, object>();
            for (int i = 0; i < tmpItem.Count; i++)
            {
                Dictionary<string, object> tmpd = new Dictionary<string, object>();
                tmpd.Add("giftId", tmpItem[i].m_itemId);
                tmpd.Add("count", tmpItem[i].m_itemCount);
                tmpd.Add("receive", false);
                dd.Add(i.ToString(), tmpd.ToBsonDocument());
            }
            mailItem = dd.ToBsonDocument();
        }

        DateTime now = DateTime.Now;
        DateTime nt = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);

        int skip = 0;
        List<Dictionary<string, object>> data = null;
        List<Dictionary<string, object>> docList = new List<Dictionary<string, object>>();

        int serverId = DBMgr.getInstance().getDbId(sendParam.m_dbServerIP);
        if (serverId == -1)
        {
            LogMgr.error("sendMail，找不到 dbIP={0} 相关信息", sendParam.m_dbServerIP);
            return;
        }

        IMongoQuery imq = createImq(sendParam);

        while (true)
        {
            data = ExportExcelBase.nextData(ref skip, 1000, imq, TableName.PLAYER_INFO, serverId, DbName.DB_PLAYER, m_playerFields);
            if (data == null)
                break;

            docList.Clear();
            for (int i = 0; i < data.Count; i++)
            {
                if (!data[i].ContainsKey("player_id"))
                    continue;

                Dictionary<string, object> tmp = new Dictionary<string, object>();
                tmp.Add("title", sendParam.m_title);
                tmp.Add("sender", sendParam.m_sender);
                tmp.Add("content", sendParam.m_content);

                tmp.Add("time", nt);
                tmp.Add("deadTime", nt.AddDays(sendParam.m_validDay));
                tmp.Add("isReceive", false);
                tmp.Add("playerId", Convert.ToInt32(data[i]["player_id"]));

                if (mailItem != null)
                {
                    tmp.Add("gifts", mailItem);
                }
                docList.Add(tmp);
            }

            DBMgr.getInstance().insertData(TableName.PLAYER_MAIL, docList, serverId, DbName.DB_PLAYER);
        }
    }

    // 构建查询条件
    private IMongoQuery createImq(ParamSendMailFullSvr sendParam)
    {
        IMongoQuery imq = null;
        if (sendParam.m_condition != null)
        {
            List<IMongoQuery> queryList = new List<IMongoQuery>();

            if (sendParam.m_condition.ContainsKey("logOutTime"))
            {
                string time = Convert.ToString(sendParam.m_condition["logOutTime"]);
                DateTime mint = DateTime.Now, maxt = DateTime.Now;
                Tool.splitTimeStr(time, ref mint, ref maxt);
                IMongoQuery imq1 = Query.LT("logout_time", BsonValue.Create(maxt));
                IMongoQuery imq2 = Query.GTE("logout_time", BsonValue.Create(mint));
                queryList.Add(Query.And(imq1, imq2));
            }

            if (sendParam.m_condition.ContainsKey("vipLevel"))
            {
                List<int> range = new List<int>();
                string rangeStr = Convert.ToString(sendParam.m_condition["vipLevel"]);
                Tool.parseNumList(rangeStr, range);

                int leftVal = Math.Min(range[0], range[1]);
                int rightVal = Math.Max(range[0], range[1]);
                IMongoQuery imq1 = Query.LTE("VipLevel", rightVal);
                IMongoQuery imq2 = Query.GTE("VipLevel", leftVal);
                queryList.Add(Query.And(imq1, imq2));
            }

            if (queryList.Count > 0)
            {
                imq = Query.And(queryList);
            }
        }

        return imq;
    }
}



