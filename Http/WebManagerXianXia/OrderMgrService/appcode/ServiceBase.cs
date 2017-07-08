using System.Collections.Generic;

public enum ServiceType
{
    serviceTypeOrder,

    serviceTypeApiCallBack,
}

public class ServiceBase
{
    protected ServiceType m_sysType;

    public virtual void initService() { }

    public virtual void exitService() { }

    public ServiceType getSysType() { return m_sysType; }
}

public class ServiceMgr
{
    static ServiceMgr s_obj = null;

    protected Dictionary<ServiceType, ServiceBase> m_sys = new Dictionary<ServiceType, ServiceBase>();

    public static ServiceMgr getInstance()
    {
        if (s_obj == null)
        {
            s_obj = new ServiceMgr();
        }
        return s_obj;
    }

    public void init()
    {
        addSys(new OrderService());
        addSys(new ApiCallBackService());
        initService();
    }

    public void addSys(ServiceBase sys)
    {
        if (sys == null)
            return;

        m_sys.Add(sys.getSysType(), sys);
    }

    public T getSys<T>(ServiceType sysType) where T : ServiceBase 
    {
        if (m_sys.ContainsKey(sysType))
        {
            return (T)m_sys[sysType];
        }
        return default(T);
    }

    public void initService()
    {
        foreach (var p in m_sys.Values)
        {
            p.initService();
        }
    }

    public void exitService()
    {
        foreach (var p in m_sys.Values)
        {
            p.exitService();
        }
    }
}





