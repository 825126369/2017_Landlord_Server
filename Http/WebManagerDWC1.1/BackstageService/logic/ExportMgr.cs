using System;
using System.Collections.Generic;
using System.Web;
using System.IO;

// 导出管理
public class ExportMgr
{
    private Dictionary<string, ExportExcelBase> m_tables = new Dictionary<string, ExportExcelBase>();

    public string m_exportDir = "";

    public void init()
    {
        m_tables.Add(ExportRecharge.TABLE_NAME, new ExportRecharge());
        m_tables.Add(TableName.PUMP_PLAYER_MONEY, new ExportMoney());

        // 每日任务
        m_tables.Add(TableName.PUMP_DAILY_TASK, new ExportDailyTask());
        // 成就
        m_tables.Add(TableName.PUMP_TASK, new ExportAchievement());
        // 金币预警导出
        m_tables.Add(TableName.PLAYER_INFO, new ExportMoneyWarn());

        XmlConfig xml = ResMgr.getInstance().getRes("dbserver.xml");
        m_exportDir = xml.getString("exportDir", "");
    }

    public void export(ExportParam param)
    {
        if (!m_tables.ContainsKey(param.m_tableName))
            return;

        m_tables[param.m_tableName].exportExcel(param, m_exportDir);
    }
}



