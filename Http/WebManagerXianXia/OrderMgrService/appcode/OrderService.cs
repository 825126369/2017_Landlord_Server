using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System.Windows.Forms;

// 订单处理服务
public class OrderService : ServiceBase
{
    private Thread m_threadWork = null;
    
    private bool m_run = true;

    private string m_httpMonitor = "";

    // 订单管理
    private OrderMgr m_orderMgr;

    private DataStatService.Form1 m_form;

    private Dictionary<int, OrderStateBase> m_state = new Dictionary<int, OrderStateBase>();

    public OrderService()
    {
        m_sysType = ServiceType.serviceTypeOrder;
    }

    public override void initService()
    {
        XmlConfig xml = ResMgr.getInstance().getRes("dbserver.xml");
        m_httpMonitor = xml.getString("httpMonitor", "");

        initState();

        m_orderMgr = new OrderMgr();
        m_orderMgr.init();

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
        m_form = null;
    }

    public void setForm(DataStatService.Form1 f)
    {
        m_form = f;
        if (m_form != null)
        {
            XmlConfig xml = ResMgr.getInstance().getRes("dbserver.xml");
            string dbURL = xml.getString("mongodbPlayer", "");
            string mysql = xml.getString("mysql", "");
            m_form.setDbIP(dbURL, mysql);
        }
    }

    // 返回订单管理实例
    public OrderMgr getOrderMgr()
    {
        return m_orderMgr;
    }

    public string getHttpMonitor()
    {
        return m_httpMonitor;
    }

    private void run()
    {
        while (m_run)
        {
            try
            {
                List<Dictionary<string, object>> dataList =
                    MongodbPlayer.Instance.ExecuteGetListByQuery(TableName.PLAYER_ORDER_REQ,
                                                                null,
                                                                null,
                                                                "genTime",
                                                                true,
                                                                0,
                                                                1000);
                if (dataList.Count > 0 && m_form != null)
                {
                    m_form.setState(0);
                }

                for (int i = 0; i < dataList.Count && m_run; i++)
                {
                    OrderInfo info = OrderFactory.getInstance().getOrderInfo(dataList[i]);
                    if (info != null)
                    {
                        if (m_state.ContainsKey(info.m_orderState))
                        {
                            m_state[info.m_orderState].process(info, this);
                        }
                        else
                        {
                            LogMgr.log.ErrorFormat("订单的状态[{0}]未知，订单id[{1}]", info.m_orderState, info.m_orderId);
                        }

                        OrderFactory.getInstance().addIdleOrder(info);
                    }
                }

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
                Thread.Sleep(500);
            }
        }
    }

    private void initState()
    {
        m_state.Add(PlayerReqOrderState.STATE_FINISH, new OrderStateFinish());
        m_state.Add(PlayerReqOrderState.STATE_FAILED, new OrderStateFail());
        m_state.Add(PlayerReqOrderState.STATE_WAIT, new OrderStateWait());
        m_state.Add(PlayerReqOrderState.STATE_PROCESSING, new OrderStateProcessing());
    }
}


















