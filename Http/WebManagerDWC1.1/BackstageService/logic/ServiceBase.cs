using System.Collections.Generic;

abstract public class ServiceBase
{
    protected ServiceType m_sysType;

    public virtual void initService() { }

    public virtual void exitService() { }

    public virtual int doService(ServiceParam param) { return (int)RetCode.ret_error; }

    public ServiceType getSysType() { return m_sysType; }

    // ÊÇ·ñÃ¦Âµ
    public abstract bool isBusy();
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
        addSys(new WorkThreadExport());
        addSys(new ServiceSendMail());
        addSys(new ServiceGenGiftCode());
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

    public int doService(ServiceParam param, ServiceType st)
    {
        if (!m_sys.ContainsKey(st))
            return (int)RetCode.ret_error;

        return m_sys[st].doService(param);
    }

    public bool isBusy()
    {
        foreach (var sys in m_sys)
        {
            if (sys.Value.isBusy())
                return true;
        }

        return false;
    }
}





