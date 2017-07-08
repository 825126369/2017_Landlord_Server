using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class OrderStateBase
{
    // 处理订单
    public virtual void process(OrderInfo info, OrderService service)
    {

    }

    public string getOrderParam(OrderInfo info, int subCmd)
    {
        return string.Format("cmd={0}&key={1}&playerAcc={2}&money={3}&orderType={4}&subCmd={5}",
           CMD.CMD_ORDER, info.m_key, info.m_playerAcc, info.m_money, info.m_orderType,
           subCmd);
    }

    // 向游戏服务器提交订单
    public string postOrderToGameServer(OrderInfo info, int subCmd, OrderService service)
    {
        string url = service.getHttpMonitor();
        string fmt = string.Format(url, getOrderParam(info, subCmd));
        var ret = HttpPost.Get(new Uri(fmt));
        if (ret != null)
        {
            string retStr = Encoding.UTF8.GetString(ret);
            return retStr;
        }
        return "";
    }
}

//////////////////////////////////////////////////////////////////////////
// 订单完成
public class OrderStateFinish : OrderStateBase
{
    public override void process(OrderInfo info, OrderService service)
    {
        service.getOrderMgr().completeOrder(info);
    }
}

//////////////////////////////////////////////////////////////////////////
// 订单等待处理
public class OrderStateWait : OrderStateBase
{
    public override void process(OrderInfo info, OrderService service)
    {
        Dictionary<string, object> data = new Dictionary<string, object>();
        data.Add("orderState", PlayerReqOrderState.STATE_PROCESSING);
        MongodbPlayer.Instance.ExecuteUpdate(TableName.PLAYER_ORDER_REQ, "key", info.m_key, data);
        string res = postOrderToGameServer(info, SubCmd.SUB_CMD_NEW_ORDER, service);
        if (res == "") // 没有提交成功，下轮处理
        {
            data["orderState"] = PlayerReqOrderState.STATE_WAIT;
            MongodbPlayer.Instance.ExecuteUpdate(TableName.PLAYER_ORDER_REQ, "key", info.m_key, data);
        }
    }
}

//////////////////////////////////////////////////////////////////////////
// 订单处理中
public class OrderStateProcessing : OrderStateBase
{
    public override void process(OrderInfo info, OrderService service)
    {
        DateTime now = DateTime.Now;
        TimeSpan span = now - info.m_lastProcessTime;

        // 30秒未处理，重发订单
        if (span.TotalSeconds > 40)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add("lastProcessTime", now);
            MongodbPlayer.Instance.ExecuteUpdate(TableName.PLAYER_ORDER_REQ, "key", info.m_key, data);

            postOrderToGameServer(info, SubCmd.SUB_CMD_RECHECK_PROCESSING, service);
        }
    }
}

//////////////////////////////////////////////////////////////////////////
// 订单处理失败
public class OrderStateFail : OrderStateBase
{
    public override void process(OrderInfo info, OrderService service)
    {
        switch (info.m_failReason)
        {
            case RetCode.RET_MONEY_NOT_ENOUGH:  // 余额不足
                {
                    service.getOrderMgr().completeOrder(info);
                }
                break;
            case RetCode.RET_PLAYER_NOT_IN_LOBBY: // 不在大厅
                {
                    /*if (info.m_tryCount < 5 && !info.m_isApi)
                    {
                        Dictionary<string, object> data = new Dictionary<string, object>();
                        data.Add("tryCount", info.m_tryCount + 1);
                        data.Add("orderState", PlayerReqOrderState.STATE_PROCESSING);

                        MongodbPlayer.Instance.ExecuteUpdate(TableName.PLAYER_ORDER_REQ, "key",
                            info.m_key, data);

                        // 不在大厅，重试5次
                        postOrderToGameServer(info, SubCmd.SUB_CMD_REPROCESS_FAILED_ORDER, service);
                    }
                    else*/ // 当作失败订单，需要返还gm余额
                    {
                        service.getOrderMgr().completeOrder(info);
                    }
                }
                break;
            case RetCode.RET_PLAYER_OFFLINE: // 玩家不在线
                {
                    service.getOrderMgr().processOfflineOrder(info);
                }
                break;
        }
    }
}



