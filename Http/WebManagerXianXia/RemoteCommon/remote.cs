using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Security;
using System.Threading;

public enum RetCodeRemote
{
    ret_success,
    ret_busy,
    ret_error,
}

public enum ServiceType
{
    serviceTypeExportExcel,
    serviceTypeSendMail,
    serviceTypeGiftCode,
}

[Serializable]
public class ServiceParam
{
}

[Serializable]
public class ExportParam : ServiceParam
{
    // 账号
    public string m_account = "";

    // 服务器IP
    public string m_dbServerIP;

    // 表名
    public string m_tableName = "";

    // 导出条件
    public Dictionary<string, object> m_condition;
}

// 全局福利
[Serializable]
public class ParamSendMailFullSvr : ServiceParam
{
    // 往哪个服务器发
    public string m_dbServerIP = "";
    public string m_title = "";
    public string m_sender = "";
    public string m_content = "";
    public string m_itemList = "";
    public int m_validDay;

    // 发放条件
    public Dictionary<string, object> m_condition;
}

[Serializable]
public class GiftCodeInfo
{
    // 礼包ID
    public long m_giftId;
    // 平台
    public string m_plat = "";
    // 生成个数
    public int m_count;
}

// 生成礼包码
[Serializable]
public class ParamGenGiftCode : ServiceParam
{
    // 写到哪个db服务器
    public string m_dbServerIP = "";
    public List<GiftCodeInfo> m_codeList = new List<GiftCodeInfo>();
}

public delegate int CallBackService(ServiceParam param, ServiceType st);

[Serializable]
public class ServersEngine : System.MarshalByRefObject
{
    public static CallBackService s_callService = null;

    // 测试远程服务的运行状态
    public int testRemoteServer()
    {
        return 1;
    }

    public int reqService(ServiceParam param, ServiceType st)
    {
        if (s_callService == null)
            return (int)RetCodeRemote.ret_error;

        return s_callService(param, st);
    }
}

