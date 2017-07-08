//#define _OLD_RIGHT_
using System;
using System.Web.SessionState;
using System.Web;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Linq;

/*
    all:拥有所有权限
*/

// 权限的检测结果
enum RightResCode
{
    right_success,              // 成功
    right_not_login,            // 未登陆
    right_no_right,             // 没有权限操作
    right_need_switch,          // 需要切换服务器
}

// 权限管理
class RightMgr
{
    private static RightMgr s_mgr = null;
    // 权限串
    Dictionary<string, string> m_rightMap = new Dictionary<string, string>();

    // 从数据库中读出的权限串，人员类型->权限串
    Dictionary<string, string> m_rightCheck = new Dictionary<string, string>();

    // GM人员类型-->权限集合
    Dictionary<string, RightSet> m_rs = new Dictionary<string, RightSet>();

    public static RightMgr getInstance()
    {
        if (s_mgr == null)
        {
            s_mgr = new RightMgr();
            s_mgr.init();
        }
        return s_mgr;
    }
#if _OLD_RIGHT_
    // 判断account是否具有权限right
    public RightResCode hasRight(string right, HttpSessionState session, string className = "")
    {
        if (session["user"] == null)
        {
            return RightResCode.right_not_login;
        }

        if (right == "")
            return RightResCode.right_success;

        GMUser account = (GMUser)session["user"];
        if(!m_rightCheck.ContainsKey(account.m_type))
            return RightResCode.right_no_right;

        if (!account.isSwitchDbServer)
            return RightResCode.right_need_switch;

        string r = m_rightCheck[account.m_type];

        // 具有所有权限，直接返回true
        if (r.IndexOf("all") >= 0)
            return RightResCode.right_success;

        if (r.IndexOf(right) >= 0)
            return RightResCode.right_success;

        return RightResCode.right_no_right;
    }

    public bool canEdit(string right, GMUser user)
    {
        return true;
    }
#else
    public RightResCode hasRight(string right, HttpSessionState session, string className = "")
    {
        if (session["user"] == null)
        {
            return RightResCode.right_not_login;
        }

        if(right == "")
            return RightResCode.right_success;

        GMUser user = (GMUser)session["user"];

        if (!user.isSwitchDbServer)
            return RightResCode.right_need_switch;

        if (user.m_type == "admin")
            return RightResCode.right_success;

        if (!m_rs.ContainsKey(user.m_type))
            return RightResCode.right_no_right;

        RightSet rs = m_rs[user.m_type];
        if (!rs.canView(right))
            return RightResCode.right_no_right;

        return RightResCode.right_success;
    }  
    
    public bool canEdit(string right, GMUser user)
    {
        if(user.m_type == "admin")
            return true;

        if (!m_rs.ContainsKey(user.m_type))
            return false;

        RightSet rs = m_rs[user.m_type];
        return rs.canEdit(right);
    }
 
#endif

    // 对当前要进行的操作进行检验
    public bool opCheck(string right, HttpSessionState session, HttpResponse response, string className = "")
    {
        RightResCode code = hasRight(right, session);
        if (code == RightResCode.right_success)
            return true;
        if (code == RightResCode.right_not_login)
        {
            response.Write("<script>location.href='/Account/Login.aspx';</script>");
            response.End();
            //response.Redirect("~/Account/Login.aspx");
        }
        if (code == RightResCode.right_no_right)
        {
            response.Redirect("~/appaspx/Error.aspx?right=" + right);
        }
        if (code == RightResCode.right_need_switch)
        {
            response.Redirect("~/appaspx/Error.aspx?right=" + "@@");
        }
        return false;
    }


    // 返回权限名称
    public string getRrightName(string right)
    {
        if (m_rightMap.ContainsKey(right))
            return m_rightMap[right];
        return "";
    }

    // 获取权限列表
    public Dictionary<string, string> getRightMap()
    {
        return m_rightMap;
    }

    // 从权限表获取所有权限
    public List<Dictionary<string, object>> getAllRight()
    {
        List<Dictionary<string, object>> data = DBMgr.getInstance().executeQuery(TableName.RIGHT, 0, DbName.DB_ACCOUNT, null);
        return data;
    }

    // 从权限表获取所有权限
    public string getRight(string type)
    {
        List<Dictionary<string, object>> data = DBMgr.getInstance().executeQuery(TableName.RIGHT, 0, DbName.DB_ACCOUNT, null);
        foreach (Dictionary<string, object> dt in data)
        {
            if (Convert.ToString(dt["type"]) == type)
            {
                string str = Convert.ToString(dt["right"]);
                return str;
            }
        }
        return "";
    }

    // 修改人员权限
    public bool modifyRight(string type, string right)
    {
        Dictionary<string, object> data = new Dictionary<string, object>();
        data["type"] = type;
        data["right"] = right;
        m_rightCheck[type] = right;
        return DBMgr.getInstance().save(TableName.RIGHT, data, "type", type, 0, DbName.DB_ACCOUNT);
    }

    public string getRightJson()
    {
        Dictionary<string, object> ret = new Dictionary<string, object>();

        // 人员类型列表
        List<Dictionary<string, object>> gmTypeList = new List<Dictionary<string, object>>();
        var drs = from d in m_rs
                       orderby d.Value.m_genTime
                       select d;
        foreach (var d in drs)
        {
            Dictionary<string, object> dt = new Dictionary<string, object>();
            dt.Add("gmTypeId", d.Key);
            dt.Add("gmTypeName", d.Value.m_gmTypeName);
            gmTypeList.Add(dt);
        }
        ret.Add("gmTypeList", gmTypeList);

        // 每个GM类型的权限
        foreach (var d in m_rs)
        {
            Dictionary<string, object> dt1 = new Dictionary<string, object>();
            ret.Add(d.Key, dt1);

            foreach (var sd in d.Value.getRightSet())
            {
                Dictionary<string, object> dt2 = new Dictionary<string, object>();
                dt1.Add(sd.Key, dt2);

                dt2.Add("canEdit", sd.Value.m_canEdit);
                dt2.Add("canView", sd.Value.m_canView);
            }
        }

        List<Dictionary<string, object>> rightList = new List<Dictionary<string, object>>();
        // 所有权限
        var allRight = ResMgr.getInstance().getBaseRight();

        foreach (var d in allRight)
        {
            Dictionary<string, object> dt = new Dictionary<string, object>();
            dt.Add("rightId", d.Key);
            dt.Add("rightName", d.Value.m_rightName);
            dt.Add("category", d.Value.m_category);
            rightList.Add(dt);
        }
        ret.Add("rightList", rightList);

        return ItemHelp.genJsonStr(ret);
    }

    public OpRes modifyGmTypeRight(string gmType, string fun, string right)
    {
        Dictionary<string, object> data = new Dictionary<string, object>();
        data.Add(fun, right);
        bool res = DBMgr.getInstance().update(TableName.GM_TYPE, data, "id", gmType, 0, DbName.DB_ACCOUNT);
        if (res)
        {
            RightSet rst = null;
            if (m_rs.ContainsKey(gmType))
            {
                rst = m_rs[gmType];
            }
            else
            {
                rst = new RightSet();
                m_rs.Add(gmType, rst);
            }
            rst.addRightByStr(fun, right);
        }
        return res ? OpRes.opres_success : OpRes.op_res_failed;
    }

    // 返回所有的GM类型
    public List<AccountType> getAllGmType()
    {
        List<AccountType> result = new List<AccountType>();

        var drs = from d in m_rs
                  orderby d.Value.m_genTime
                  select d;
        foreach (var d in drs)
        {
            AccountType at = new AccountType(d.Key, d.Value.m_gmTypeName);
            result.Add(at);

        }
        return result;
    }

    public string getGmTypeName(string gmType)
    {
        if (m_rs.ContainsKey(gmType))
        {
            return m_rs[gmType].m_gmTypeName;
        }
        return "";
    }

    private void init()
    {
        try
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(HttpRuntime.BinDirectory + "..\\" + "data\\right.xml");

            XmlNode node = doc.SelectSingleNode("/configuration");

            for (node = node.FirstChild; node != null; node = node.NextSibling)
            {
                string right = node.Attributes["right"].Value;
                string fmt = node.Attributes["fmt"].Value;
                if (m_rightMap.ContainsKey(right))
                {
                    LOGW.Info("读right.xml时，发生了错误，出现了重复的权限 {0}", right);
                }
                else
                {
                    m_rightMap.Add(right, fmt);
                }
            }
        }
        catch (System.Exception ex)
        {
            LOGW.Info(ex.Message);
            LOGW.Info(ex.StackTrace);
        }
        finally
        {
            modifyRight("admin", "all");  // 管理员类型
        }

        List<Dictionary<string, object>> allR = getAllRight();
        foreach (var d in allR)
        {
            string st = Convert.ToString(d["type"]);
            string rt = Convert.ToString(d["right"]);
            m_rightCheck[st] = rt;
        }

        readRightSet();
    }

    // 读取权限集
    public void readRightSet()
    {
        m_rs.Clear();
        List<Dictionary<string, object>> dataList =
            DBMgr.getInstance().executeQuery(TableName.GM_TYPE, 0, DbName.DB_ACCOUNT, null, 0, 0, null, "genTime");
        if (dataList == null || dataList.Count == 0)
            return;

        for (int i = 0; i < dataList.Count; i++)
        {
            Dictionary<string, object> data = dataList[i];
            RightSet rs = new RightSet();
            string typeId = "";
            DateTime time = DateTime.Now;

            foreach (var d in data)
            {
                if (d.Key == "_id")
                {
                    continue;
                }
                else if (d.Key == "id")
                {
                    typeId = Convert.ToString(d.Value);
                }
                else if (d.Key == "typeName")
                {
                    rs.m_gmTypeName = Convert.ToString(d.Value);
                }
                else if (d.Key == "genTime")
                {
                    time = Convert.ToDateTime(d.Value).ToLocalTime();
                }
                else
                {
                    rs.addRightByStr(d.Key, Convert.ToString(d.Value));
                }
            }
            rs.setTime(time);
            m_rs.Add(typeId, rs);
        }
    }
}

// 权限基本信息
public class RightInfo
{
    public string m_rightName; // 权限名称，由RightDef定义
    public bool m_canEdit;     // 可否编辑
    public bool m_canView;     // 可否查看
}

// 权限集合
public class RightSet
{
    // 权限名称-->权限信息
    Dictionary<string, RightInfo> m_rs = new Dictionary<string, RightInfo>();

    public string m_gmTypeName;
    public DateTime m_genTime;

    public RightInfo getRightInfo(string rightName)
    {
        if (m_rs.ContainsKey(rightName))
            return m_rs[rightName];

        return null;
    }

    public bool canEdit(string rightName)
    {
        RightInfo info = getRightInfo(rightName);
        return info == null ? false : info.m_canEdit;
    }

    public bool canView(string rightName)
    {
        RightInfo info = getRightInfo(rightName);
        return info == null ? false : info.m_canView;
    }

    public void reset()
    {
        m_rs.Clear();
    }

    public Dictionary<string, RightInfo> getRightSet()
    {
        return m_rs;
    }

    public bool addRight(string rightName, bool canView, bool canEdit)
    {
        RightInfo info = null;
        if (m_rs.ContainsKey(rightName))
        {
            info = m_rs[rightName];
        }
        else
        {
            info = new RightInfo();
            info.m_rightName = rightName;
            m_rs.Add(rightName, info);
        }

        info.m_canView = canView;
        info.m_canEdit = canEdit;

        return true;
    }

    // rs以,相隔 表示 可查看 可编辑
    public bool addRightByStr(string rightName, string rs)
    {
        if (string.IsNullOrEmpty(rs))
            return true;

        string[] arr = Tool.split(rs, ',', StringSplitOptions.RemoveEmptyEntries);
        return addRight(rightName, arr[0] == "1", arr[1] == "1");
    }

    public void setTime(DateTime t)
    {
        m_genTime = t;
    }
}

//////////////////////////////////////////////////////////////////////////
#if _OLD_RIGHT_
public struct RightDef
{
    // gm账号类型的编辑，添加修改
    public const string GM_TYPE_EDIT = "-1";

    public const string OP_PLAYER_MONEY_CHANGE = "operation";    // 玩家金币变化详细
    public const string OP_BG_RECHARGE = "operation";            // 后台充值
    public const string OP_OPERATION_NOTICE = "operation";       // 运营公告
    public const string OP_NOTIFY_MSG = "operation";             // 通告消息
    public const string OP_MAINTENANCE_NOTICE = "operation";     // 维护公告
    public const string OP_MONEY_WARN = "operation";             // 金币预警
    public const string OP_CUR_ONLINE_NUM = "operation";         // 当前在线人数
    public const string OP_FREEZE_HEAD = "operation";            // 冻结头像
    public const string OP_INFORM_HEAD = "operation";            // 举报头像
    public const string OP_CHANNEL_EDIT = "operation";            // 渠道测试编辑
    public const string OP_MONEY_RANK = "operation";             // 金币增长排行
    public const string OP_WISH_CURSE = "operation";             // 祝福诅咒
    public const string OP_MAX_ONLINE = "operation";             // 最高在线
    public const string OP_PLAYER_TOTAL_MONEY = "operation";     // 玩家金币总和
    public const string OP_PLAYER_OP = "operation";              // 玩家相关操作
    public const string OP_RECHARGE_POINT = "operation";         // 付费点统计
    public const string OP_WEEK_CHAMPION_SETTING = "operation";         // 大奖赛冠军设置

    public const string SVR_ACCOUNT_QUERY = "service";         // 账号查询
    public const string SVR_RECHARGE_QUERY = "service";         // 充值记录查询
    public const string SVR_SEND_MAIL = "service";             // 发邮件
    public const string SVR_MAIL_CHECK = "service";             // 邮件检查
    public const string SVR_RESET_PLAYER_PWD = "service";        // 重置密码
    public const string SVR_BLOCK_PLAYER_ID = "service";        // 停封玩家ID
    public const string SVR_EXCHANGE_MGR = "service";        // 兑换管理
    public const string SVR_ADD_SERVICE_INFO = "service";        // 增加客服信息

    public const string TD_ACTIVE_DATA = "operation";        // 活跃数据
    public const string TD_LTV = "operation";                // LTV统计
    public const string TD_R_LOSE = "operation";                // 大R流失
    public const string TD_RECHARGE_PLAYER_STAT = "operation";                // 充值用户统计
    public const string TD_PLAYER_DB_MONITOR = "operation";                // 玩家龙珠监控
    public const string TD_DAILY_DB = "operation";                // 每日龙珠
    public const string TD_RECHARGE_PLAYER_MONITOR = "operation";                // 充值玩家监控
    public const string TD_INCOME_EXPENSES = "operation";                // 活跃玩家收支统计
    public const string TD_RECHARGE_PER_HOUR = "operation";                // 实时付费
    public const string TD_ONLINE_PER_HOUR = "operation";                // 实时在线

    public const string DATA_OLD_EARINGS_RATE = "stat";                // 盈利率重置查询
    public const string DATA_PLAYER_FAVOR = "stat";                // 玩家喜好
    public const string DATA_INDEPEND_LOBBY = "stat";                // 独立数据大厅
    public const string DATA_MONEY_FLOW = "stat";                // 游戏金币流动统计
    public const string DATA_TOTAL_CONSUME = "stat";                // 消耗收入总计
    public const string DATA_GAME_INCOME_EACH_DAY = "stat";                // 游戏每天收入统计
    public const string DATA_RELOAD_TABLE = "stat";                // 重新加载表格
    public const string DATA_PLAYER_LOSE = "stat";                // 流失查询
    public const string DATA_SHOP_EXCHANGE = "stat";                // 商城兑换
    public const string DATA_STAR_LOTTERY = "stat";                // 星星抽奖

    public const string FISH_PARAM_CONTROL = "stat";                // 经典捕鱼参数调整
    public const string FISH_DESK_EARNING_QUERY = "stat";                // 经典捕鱼桌子盈利率
    public const string FISH_ALG_ANALYZE = "stat";                // 经典捕鱼算法分析
    public const string FISH_STAT = "stat";                // 鱼的统计
    public const string FISH_INDEPEND_DATA = "stat";                // 独立数据-捕鱼
    public const string FISH_CONSUME = "stat";                // 捕鱼消耗
    public const string FISH_WEEK_CHAMPION = "stat";                // 大奖赛周冠军
    public const string FISH_CHAMPION_RANK = "stat";                // 大奖赛名次
    public const string FISH_BOSS_CONSUME = "stat";                // BOSS消耗
    public const string FISH_BOSS_CONTROL = "stat";                // 经典捕鱼BOSS控制

    public const string CROD_PARAM_CONTROL = "stat";                // 鳄鱼大亨参数调整
    public const string CROD_INDEPEND_DATA = "stat";                // 独立数据-鳄鱼
    public const string CROD_RESULT_CONTROL = "stat";                // 鳄鱼大亨结果控制

    public const string DICE_EARINGS = "stat";                // 骰宝盈利率
    public const string DICE_INDEPEND_DATA = "stat";                // 独立数据-骰宝
    public const string DICE_RESULT_CONTROL = "stat";                // 骰宝结果控制

    public const string BACC_PARAM_CONTROL = "stat";                // 百家乐参数调整
    public const string BACC_PLAYER_BANKER = "stat";                // 上庄情况
    public const string BACC_RESULT_CONTROL = "stat";                // 百家乐结果控制

    public const string COW_PARAM_CONTROL = "stat";                // 牛牛参数调整
    public const string COW_PLAYER_BANKER = "stat";                // 牛牛上庄查询
    public const string COW_INDEPEND_DATA = "stat";                // 牛牛独立数据
    public const string COW_CARD_TYPE = "stat";                    // 牛牛牌型设置

    public const string D5_PARAM_CONTROL = "stat";                    // 五龙参数调整
    public const string D5_EARNINGS = "stat";                         // 五龙具体盈利情况

    public const string SHCD_RESULT_CONTROL = "stat";                // 黑红梅方结果控制
    public const string SHCD_PARAM_CONTROL = "stat";                 // 黑红梅方参数调整
    public const string SHCD_INDEPEND_DATA = "stat";                 // 独立数据-黑红梅方

    public const string CALF_PARAM_CONTROL = "stat";                 // 套牛参数调整
    public const string CALF_INDEPEND_DATA = "stat";                 // 套牛独立数据
    public const string CALF_LEVEL_DATA = "stat";                    // 套牛关卡数据
    public const string CALF_CLASS_STAT = "stat";                    // 套牛分类统计

    public const string OTHER_VIEW_LOG = "viewlog";                    // 查看日志
}

#else

// 权限类型的定义
public struct RightDef
{
    // gm账号类型的编辑，添加修改
    public const string GM_TYPE_EDIT = "-1";

    public const string OP_PLAYER_MONEY_CHANGE = "1";    // 玩家金币变化详细
    public const string OP_BG_RECHARGE = "2";            // 后台充值
    public const string OP_OPERATION_NOTICE = "3";       // 运营公告
    public const string OP_NOTIFY_MSG = "4";             // 通告消息
    public const string OP_MAINTENANCE_NOTICE = "5";     // 维护公告
    public const string OP_MONEY_WARN = "6";             // 金币预警
    public const string OP_CUR_ONLINE_NUM = "7";         // 当前在线人数
    public const string OP_FREEZE_HEAD = "8";            // 冻结头像
    public const string OP_INFORM_HEAD = "9";            // 举报头像
    public const string OP_CHANNEL_EDIT = "10";            // 渠道测试编辑
    public const string OP_MONEY_RANK = "11";             // 金币增长排行
    public const string OP_WISH_CURSE = "12";             // 祝福诅咒
    public const string OP_MAX_ONLINE = "13";             // 最高在线
    public const string OP_PLAYER_TOTAL_MONEY = "14";     // 玩家金币总和
    public const string OP_PLAYER_OP = "15";              // 玩家相关操作
    public const string OP_RECHARGE_POINT = "16";         // 付费点统计
    public const string OP_WEEK_CHAMPION_SETTING = "17";         // 大奖赛冠军设置

    public const string SVR_ACCOUNT_QUERY = "1001";         // 账号查询
    public const string SVR_RECHARGE_QUERY = "1002";         // 充值记录查询
    public const string SVR_SEND_MAIL = "1003";             // 发邮件
    public const string SVR_MAIL_CHECK = "1004";             // 邮件检查
    public const string SVR_RESET_PLAYER_PWD = "1005";        // 重置密码
    public const string SVR_BLOCK_PLAYER_ID = "1006";        // 停封玩家ID
    public const string SVR_EXCHANGE_MGR = "1007";        // 兑换管理
    public const string SVR_ADD_SERVICE_INFO = "1008";        // 增加客服信息

    public const string TD_ACTIVE_DATA = "2001";        // 活跃数据
    public const string TD_LTV = "2002";                // LTV统计
    public const string TD_R_LOSE = "2003";                // 大R流失
    public const string TD_RECHARGE_PLAYER_STAT = "2004";                // 充值用户统计
    public const string TD_PLAYER_DB_MONITOR = "2005";                // 玩家龙珠监控
    public const string TD_DAILY_DB = "2006";                // 每日龙珠
    public const string TD_RECHARGE_PLAYER_MONITOR = "2007";                // 充值玩家监控
    public const string TD_INCOME_EXPENSES = "2008";                // 活跃玩家收支统计
    public const string TD_RECHARGE_PER_HOUR = "2009";                // 实时付费
    public const string TD_ONLINE_PER_HOUR = "2010";                // 实时在线

    public const string DATA_OLD_EARINGS_RATE = "3001";                // 盈利率重置查询
    public const string DATA_PLAYER_FAVOR = "3002";                // 玩家喜好
    public const string DATA_INDEPEND_LOBBY = "3003";                // 独立数据大厅
    public const string DATA_MONEY_FLOW = "3004";                // 游戏金币流动统计
    public const string DATA_TOTAL_CONSUME = "3005";                // 消耗收入总计
    public const string DATA_GAME_INCOME_EACH_DAY = "3006";                // 游戏每天收入统计
    public const string DATA_RELOAD_TABLE = "3007";                // 重新加载表格
    public const string DATA_PLAYER_LOSE = "3008";                // 流失查询
    public const string DATA_SHOP_EXCHANGE = "3009";                // 商城兑换
    public const string DATA_STAR_LOTTERY = "3010";                // 星星抽奖

    public const string FISH_PARAM_CONTROL = "4001";                // 经典捕鱼参数调整
    public const string FISH_DESK_EARNING_QUERY = "4002";                // 经典捕鱼桌子盈利率
    public const string FISH_ALG_ANALYZE = "4003";                // 经典捕鱼算法分析
    public const string FISH_STAT = "4004";                // 鱼的统计
    public const string FISH_INDEPEND_DATA = "4005";                // 独立数据-捕鱼
    public const string FISH_CONSUME = "4006";                // 捕鱼消耗
    public const string FISH_WEEK_CHAMPION = "4007";                // 大奖赛周冠军
    public const string FISH_CHAMPION_RANK = "4008";                // 大奖赛名次
    public const string FISH_BOSS_CONSUME = "4009";                // BOSS消耗
    public const string FISH_BOSS_CONTROL = "4010";                // 经典捕鱼BOSS控制

    public const string CROD_PARAM_CONTROL = "5001";                // 鳄鱼大亨参数调整
    public const string CROD_INDEPEND_DATA = "5002";                // 独立数据-鳄鱼
    public const string CROD_RESULT_CONTROL = "5003";                // 鳄鱼大亨结果控制

    public const string DICE_EARINGS = "6001";                // 骰宝盈利率
    public const string DICE_INDEPEND_DATA = "6002";                // 独立数据-骰宝
    public const string DICE_RESULT_CONTROL = "6003";                // 骰宝结果控制

    public const string BACC_PARAM_CONTROL = "7001";                // 百家乐参数调整
    public const string BACC_PLAYER_BANKER = "7002";                // 上庄情况
    public const string BACC_RESULT_CONTROL = "7003";                // 百家乐结果控制

    public const string COW_PARAM_CONTROL = "8001";                // 牛牛参数调整
    public const string COW_PLAYER_BANKER = "8002";                // 牛牛上庄查询
    public const string COW_INDEPEND_DATA = "8003";                // 牛牛独立数据
    public const string COW_CARD_TYPE = "8004";                    // 牛牛牌型设置

    public const string D5_PARAM_CONTROL = "9001";                    // 五龙参数调整
    public const string D5_EARNINGS = "9002";                         // 五龙具体盈利情况

    public const string SHCD_RESULT_CONTROL = "10001";                // 黑红梅方结果控制
    public const string SHCD_PARAM_CONTROL = "10002";                 // 黑红梅方参数调整
    public const string SHCD_INDEPEND_DATA = "10003";                 // 独立数据-黑红梅方

    public const string CALF_PARAM_CONTROL = "11001";                 // 套牛参数调整
    public const string CALF_INDEPEND_DATA = "11002";                 // 套牛独立数据
    public const string CALF_LEVEL_DATA = "11003";                    // 套牛关卡数据
    public const string CALF_CLASS_STAT = "11004";                    // 套牛分类统计

    public const string OTHER_VIEW_LOG = "10000001";                    // 查看日志
}

#endif