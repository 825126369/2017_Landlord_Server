using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System.Text;

// API回调服务
public class ApiCallBackService : ServiceBase
{
    // 回调URL参数
    const string CALL_URL_PARAM = "orderId={0}&userOrderId={1}&playerAcc={2}&money={3}&orderType={4}&result={5}&sign={6}";

    // 最大任务数量
    private const int MAX_TASK = 1000;

    // 最多回调次数
    private const int TRY_MAX_COUNT = 10;

    // 回调时间间隔 秒
    private const int CALL_INTERVAL = 15;

    private Thread m_threadWork = null;
    
    private bool m_run = true;

    // 是否重读db
    private bool m_isReadDb = true;

    // 同步
    private object m_lockObj = new object();

    // 回调队列
    private Queue<ApiCallOrderInfo> m_taskQue = new Queue<ApiCallOrderInfo>();

    public ApiCallBackService()
    {
        m_sysType = ServiceType.serviceTypeApiCallBack;
    }

    public override void initService()
    {
        readInfo();

        m_threadWork = new Thread(new ThreadStart(run));
        m_threadWork.Start();
    }

    public override void exitService()
    {
        m_run = false;
        if (m_threadWork != null)
        {
            m_threadWork.Join();
        }

        // 保存还没有回调的订单
        foreach (var info in m_taskQue)
        {
            saveInfo(info);
        }
        m_taskQue.Clear();
    }

    // 增加一个完成的api订单
    public void addApiOrder(OrderInfo info, string devKey)
    {
        if (info == null)
            return;

        if (string.IsNullOrEmpty(info.m_apiCallback)) // 没有回调地址
            return;

        ApiCallOrderInfo copyInfo = copyApiOrder(info);
        copyInfo.m_apiDevKey = devKey;

        if (m_taskQue.Count > MAX_TASK) 
        {
            // 达到了最大值，此时将信息写入db，供空闲时处理
            saveInfo(copyInfo);
            m_isReadDb = true;
        }
        else
        {
            lock (m_lockObj)
            {
                m_taskQue.Enqueue(copyInfo);
            }
        }
    }

    private void run()
    {
        ApiCallOrderInfo data = null;
        while (m_run)
        {
            while (m_taskQue.Count > 0)
            {
                if (!m_run)
                {
                    break;
                }

                lock (m_lockObj)
                {
                    data = m_taskQue.Dequeue();
                }

                if (data != null)
                {
                    bool res = call(data);
                    if (res) // 完成
                    {
                        MongodbPlayer.Instance.ExecuteRemoveBykey(TableName.API_ORDER_CALL, "key", data.m_key);
                    }
                    else 
                    {
                        lock (m_lockObj)
                        {
                            // 没有处理成功，重新加入队列
                            m_taskQue.Enqueue(data);
                        }
                    }
                }
            }

            if (m_run)
            {
                readInfo();
            }

            if (m_taskQue.Count == 0 && m_run)
            {
                Thread.Sleep(5000);
            }
        }
    }

    // 拷贝api调用所需信息
    private ApiCallOrderInfo copyApiOrder(OrderInfo info)
    {
        ApiCallOrderInfo copyInfo = new ApiCallOrderInfo();
        copyInfo.m_key = info.m_key;
        copyInfo.m_apiCallback = info.m_apiCallback;
        copyInfo.m_orderId = info.m_orderId;
        copyInfo.m_apiOrderId = info.m_apiOrderId;
        copyInfo.m_money = info.m_money;
        copyInfo.m_playerAcc = info.m_playerAcc;
        copyInfo.m_orderType = info.m_orderType;
        copyInfo.m_orderState = info.m_orderState;
        copyInfo.m_failReason = info.m_failReason;
        copyInfo.m_tryCount = 0;
        copyInfo.m_lastProcessTime = DateTime.MinValue;
        return copyInfo;
    }

    private void saveInfo(ApiCallOrderInfo info)
    {
        bool exist = MongodbPlayer.Instance.KeyExistsBykey(TableName.API_ORDER_CALL, "key", info.m_key);
        if (exist) // 这条记录已存在了
            return;

        Dictionary<string, object> data = new Dictionary<string, object>();
        data.Add("key", info.m_key);
        data.Add("orderId", info.m_orderId);

        data.Add("playerAcc", info.m_playerAcc);

        data.Add("money", info.m_money);
        data.Add("orderType", info.m_orderType);
        data.Add("orderState", info.m_orderState);
        data.Add("failReason", info.m_failReason);

        data.Add("apiOrderId", info.m_apiOrderId);
        data.Add("apiCallback", info.m_apiCallback);

        data.Add("tryCount", info.m_tryCount);
        data.Add("lastProcessTime", info.m_lastProcessTime);
        data.Add("devKey", info.m_apiDevKey);

        MongodbPlayer.Instance.ExecuteInsert(TableName.API_ORDER_CALL, data);
    }

    // 从数据库读取数据
    private void readInfo()
    {
        if (m_taskQue.Count > 0)
            return;

        if (!m_isReadDb)
            return;

        try
        {

            List<Dictionary<string, object>> dataList =
                MongodbPlayer.Instance.ExecuteGetListByQuery(TableName.API_ORDER_CALL,
                                                            null,
                                                            null,
                                                            "",
                                                            true,
                                                            0,
                                                            MAX_TASK);
            if (dataList.Count == 0)
            {
                m_isReadDb = false;
            }

            for (int i = 0; i < dataList.Count && m_run; i++)
            {
                Dictionary<string, object> data = dataList[i];
                ApiCallOrderInfo copyInfo = new ApiCallOrderInfo();
                copyInfo.m_key = Convert.ToString(data["key"]);
                copyInfo.m_apiCallback = Convert.ToString(data["apiCallback"]);
                copyInfo.m_orderId = Convert.ToString(data["orderId"]);
                copyInfo.m_apiOrderId = Convert.ToString(data["apiOrderId"]);
                copyInfo.m_money = Convert.ToInt64(data["money"]);
                copyInfo.m_playerAcc = Convert.ToString(data["playerAcc"]);
                copyInfo.m_orderType = Convert.ToInt32(data["orderType"]);
                copyInfo.m_orderState = Convert.ToInt32(data["orderState"]);
                copyInfo.m_failReason = Convert.ToInt32(data["failReason"]);

                if (data.ContainsKey("devKey"))
                {
                    copyInfo.m_apiDevKey = Convert.ToString(data["devKey"]);
                }

                lock (m_lockObj)
                {
                    m_taskQue.Enqueue(copyInfo);
                }
            }
        }
        catch (System.Exception ex)
        {
            LogMgr.log.Error(ex.ToString());
        }
    }

    // 回调api页面。返回true调用成功
    private bool call(ApiCallOrderInfo info)
    {
        if (info.m_tryCount >= TRY_MAX_COUNT) // 已重试过指定次数，都没有成功，不再调用
        {
            return true;
        }

        TimeSpan span = DateTime.Now - info.m_lastProcessTime;

        if (span.TotalSeconds > CALL_INTERVAL)
        {
            string url = constructURL(info);
            var ret = HttpPost.Get(new Uri(url));
            if (ret != null)
            {
                string retStr = Encoding.UTF8.GetString(ret);
                if (retStr == "ok")
                {
                    return true;
                }
            }

            info.m_tryCount++;
            info.m_lastProcessTime = DateTime.Now;
        }

        return false;
    }

    string constructURL(ApiCallOrderInfo info)
    {
        int result = 0;
        if (info.m_orderState != PlayerReqOrderState.STATE_FINISH)
        {
            result = info.m_failReason;
        }

        string sign = Tool.getMD5Hash(info.m_orderId + info.m_apiOrderId +
            info.m_playerAcc + info.m_money + info.m_orderType + result + info.m_apiDevKey);

        string str = string.Format(CALL_URL_PARAM,
            info.m_orderId,
            info.m_apiOrderId,
            info.m_playerAcc,
            info.m_money,
            info.m_orderType,
            result,
            sign);

        int index = info.m_apiCallback.IndexOf('?');
        if (index >= 0) // 回调本身携带了参数
        {
            return info.m_apiCallback + "&" + str;
        }

        return info.m_apiCallback + "?" + str;
    }
}


















