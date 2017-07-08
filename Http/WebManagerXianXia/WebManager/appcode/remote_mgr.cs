using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.IO;
using System.Web.Configuration;

// 远程服务对象的管理
public class RemoteMgr
{
    private static RemoteMgr s_obj = null;
    // 所有服务端结点
    private Dictionary<int, ServersEngine> m_serversEngines = new Dictionary<int, ServersEngine>();
    
    public static RemoteMgr getInstance()
    {
        if (s_obj == null)
        {
            s_obj = new RemoteMgr();
            s_obj.init();
        }
        return s_obj;
    }

    // 初始化
    public void init()
    {
        string exportIP = WebConfigurationManager.AppSettings["exportIP"];
        int exportPort = Convert.ToInt32(WebConfigurationManager.AppSettings["exportPort"]);

        TcpChannel channel = new TcpChannel();
        ChannelServices.RegisterChannel(channel, false);

        string str = string.Format("tcp://{0}:{1}/ServersEngine", exportIP, exportPort);
        ServersEngine obj = (ServersEngine)Activator.GetObject(typeof(ServersEngine), str);

        m_serversEngines.Add(0, obj);
    }

    // 关闭
    public void shutDown()
    {
    }

    // 返回远程服务状态
    public int getRemoteServerState(int remoteId)
    {
        if (!m_serversEngines.ContainsKey(remoteId))
        {
            return StateMgr.state_unknown;
        }

        try
        {
            ServersEngine e = m_serversEngines[remoteId];
            e.testRemoteServer();
            return StateMgr.state_running;
        }
        catch (System.Exception ex)
        {
        }
        return StateMgr.state_closed;
    }

    public OpRes reqExportExcel(ExportParam param)
    {
        int state = getRemoteServerState(0);
        if (state == StateMgr.state_closed)
        {
            return OpRes.op_res_export_excel_not_open;
        }

        ServersEngine e = m_serversEngines[0];
        int retCode = e.reqService(param, ServiceType.serviceTypeExportExcel);

        if (retCode == (int)RetCodeRemote.ret_error)
            return OpRes.op_res_failed;

        if (retCode == (int)RetCodeRemote.ret_busy)
            return OpRes.op_res_export_service_busy;

        return OpRes.op_res_has_commit_export;
    }

    public OpRes reqSendMail(ParamSendMailFullSvr param)
    {
        int state = getRemoteServerState(0);
        if (state == StateMgr.state_closed)
        {
            return OpRes.op_res_export_excel_not_open;
        }

        try
        {
            ServersEngine e = m_serversEngines[0];
            int retCode = e.reqService(param, ServiceType.serviceTypeSendMail);

            if (retCode == (int)RetCodeRemote.ret_error)
                return OpRes.op_res_failed;

            if (retCode == (int)RetCodeRemote.ret_busy)
                return OpRes.op_res_export_service_busy;

            return OpRes.opres_success;
        }
        catch (System.Exception ex)
        {
        }
        return OpRes.op_res_failed;
    }

    public OpRes reqGenGiftCode(ParamGenGiftCode param)
    {
        int state = getRemoteServerState(0);
        if (state == StateMgr.state_closed)
        {
            return OpRes.op_res_export_excel_not_open;
        }

        try
        {
            ServersEngine e = m_serversEngines[0];
            int retCode = e.reqService(param, ServiceType.serviceTypeGiftCode);

            if (retCode == (int)RetCodeRemote.ret_error)
                return OpRes.op_res_failed;

            if (retCode == (int)RetCodeRemote.ret_busy)
                return OpRes.op_res_export_service_busy;

            return OpRes.opres_success;
        }
        catch (System.Exception ex)
        {
        }
        return OpRes.op_res_failed;
    }
}

//////////////////////////////////////////////////////////////////////////

public class StateMgr
{
    static string[] s_name = { "正在运行", "已关闭", "未知" };
    static string[] s_opName = { "操作成功", "操作失败", "远程监控未开启" };

    public const int state_running = 0;  // 正在运行
    public const int state_closed = 1;   // 已关闭
    public const int state_unknown = 2;  // 未知

    public const int op_success = 0;  // 操作成功
    public const int op_failed = 1;   // 操作失败
    public const int op_remote_not_start = 2;  // 远程监控未开启

    public static string getStateName(int state)
    {
        if (state < 0 || state >= s_name.Length)
            return "";

        return s_name[state];
    }

    public static string getOpName(int op)
    {
        if (op < 0 || op >= s_opName.Length)
            return "";

        return s_opName[op];
    }
}

