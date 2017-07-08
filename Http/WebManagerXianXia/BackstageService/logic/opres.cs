using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Web;

class OpResMgr
{
    // 取得结果串
    public static string getResultString(OpRes res)
    {
        XmlConfig xml = ResMgr.getInstance().getRes("opres.xml");
        return xml.getString(Convert.ToString((int)res), "");
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
    op_res_connect_failed,      // 连接db服务器失败
}

