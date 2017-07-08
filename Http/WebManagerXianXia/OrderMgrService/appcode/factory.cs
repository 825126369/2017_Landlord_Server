using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// 订单管理工厂
public class OrderFactory
{
    static OrderFactory s_obj = null;

    private Queue<OrderInfo> m_orderList = new Queue<OrderInfo>();

    public static OrderFactory getInstance()
    {
        if (s_obj == null)
        {
            s_obj = new OrderFactory();
        }
        return s_obj;
    }

    public OrderInfo getOrderInfo(Dictionary<string, object> data)
    {
        try
        {
            OrderInfo info = create();
            info.m_key = Convert.ToString(data["key"]);
            info.m_orderId = Convert.ToString(data["orderId"]);
            info.m_apiOrderId = Convert.ToString(data["apiOrderId"]);
            info.m_genTime = Convert.ToDateTime(data["genTime"]).ToLocalTime();
            info.m_gmAcc = Convert.ToString(data["gmAcc"]);
            info.m_playerAcc = Convert.ToString(data["playerAcc"]);
            info.m_isApi = Convert.ToBoolean(data["isApi"]);
            info.m_apiCallback = Convert.ToString(data["apiCallback"]);

            info.m_money = Convert.ToInt64(data["money"]);
            if (data.ContainsKey("playerRemainMoney"))
            {
                info.m_playerRemainMoney = Convert.ToInt64(data["playerRemainMoney"]);
            }

            info.m_orderType = Convert.ToInt32(data["orderType"]);
            info.m_orderState = Convert.ToInt32(data["orderState"]);

            info.m_lastProcessTime = Convert.ToDateTime(data["lastProcessTime"]).ToLocalTime();
            info.m_tryCount = Convert.ToInt32(data["tryCount"]);
            info.m_failReason = Convert.ToInt32(data["failReason"]);

            if (data.ContainsKey("dstType"))
            {
                info.m_dstType = Convert.ToInt32(data["dstType"]);
            }
            if (data.ContainsKey("orderFrom"))
            {
                info.m_orderFrom = Convert.ToInt32(data["orderFrom"]);
            }
            
            return info;
        }
        catch (System.Exception ex)
        {        	
        }
        return null;
    }

    public void addIdleOrder(OrderInfo info)
    {
        if (info == null)
            return;

        reset(info);
        m_orderList.Enqueue(info);
    }

    private OrderInfo create()
    {
        if (m_orderList.Count > 0)
        {
            return m_orderList.Dequeue();
        }

        OrderInfo info = new OrderInfo();
        return info;
    }

    private void reset(OrderInfo info)
    {
        info.m_key = "";
        info.m_orderId = "";
        info.m_apiOrderId = "";
        info.m_gmAcc = "";
        info.m_playerAcc = "";
        info.m_apiCallback = "";
        info.m_money = 0;
        info.m_orderType = -1;
        info.m_tryCount = 0;
    }
}

