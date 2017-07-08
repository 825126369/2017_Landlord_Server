using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Web;

// 各种操作信息
class OpResInfo
{
    // 操作类型
    public int m_opType;
    // 格式串
    public string m_fmt = "";

    public OpResInfo(int type, string fmt)
    {
        m_opType = type;
        m_fmt = fmt;
    }
}

class OpResMgr
{
    private static OpResMgr s_mgr = null;
    private Dictionary<int, OpResInfo> m_ops = new Dictionary<int, OpResInfo>();

    public static OpResMgr getInstance()
    {
        if (s_mgr == null)
        {
            s_mgr = new OpResMgr();
            s_mgr.init();
        }
        return s_mgr;
    }

    // 取得结果串
    public string getResultString(OpRes res)
    {
        int id = (int)res;
        if (m_ops.ContainsKey(id))
        {
            OpResInfo info = m_ops[id];
            return info.m_fmt;
        }
        return "";
    }

    private void init()
    {
        try
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(HttpRuntime.BinDirectory + "..\\" + "data\\opres.xml");

            XmlNode node = doc.SelectSingleNode("/configuration");

            for (node = node.FirstChild; node != null; node = node.NextSibling)
            {
                string sid = node.Attributes["opres"].Value;
                int id = Convert.ToInt32(sid);
                string fmt = node.Attributes["fmt"].Value;
                if (m_ops.ContainsKey(id))
                {
                   // LOGW.Info("读opres.xml时，发生了错误，出现了重复的ID {0}", id);
                }
                else
                {
                    m_ops.Add(id, new OpResInfo(id, fmt));
                }
            }
        }
        catch (System.Exception ex)
        {
        }
    }
}

public enum OpRes
{
    opres_success,              // 成功
    op_res_failed,              // 失败
    op_res_time_format_error,   // 时间格式错
    op_res_not_found_data,      // 没有找到相关数据
    op_res_not_select_any_item, // 没有选择任何项目
    op_res_param_not_valid,     // 参数非法
    op_res_item_not_exist,      // 不存在该道具
    op_res_pwd_not_valid,       // 密码格式不正确
    op_res_export_excel_not_open,  // 导出Excel服务未开启
    op_res_has_commit_export,      // 已提交导出，稍候到 Excel下载  页面获取
    op_res_export_service_busy,    // 导出Excel服务繁忙，请稍候重试
    op_res_need_at_least_one_cond,  // 至少需要输入一个条件
    op_res_has_bind_mobile_phone,   // 已绑定手机，需自行找回
    op_res_need_sel_platform,       // 需要选择平台
    op_res_not_bind_phone,          // 没有绑定手机
    op_res_player_not_exist,        // 玩家不存在
    op_res_data_duplicate,          // 数据重复
    op_res_reward_beyond_limit,     // 奖励超出限额
}

