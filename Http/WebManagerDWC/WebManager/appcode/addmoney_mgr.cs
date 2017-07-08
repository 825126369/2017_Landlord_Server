using System;
using System.Collections;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System.Text.RegularExpressions;

// 增加钱币的参数
class AddMoneyParam
{
    public int m_gold;  // 金币
    public int m_gem;   // 钻石
    public int m_dbId;
}

// 向DB存储数据的格式
[Serializable]
public class DBAddMoney
{
    // 玩家的GUID
    public string m_playerGUID;
    // 增加的金币数
    public int m_gold;
    // 增加的钻石数
    public int m_gem;
    public bool m_isBot;
}

// 加钱管理
class AddMoneyMgr
{
    private static AddMoneyMgr s_mgr = null;
    // 动态加钱
    private const string DB_KEY_ADD_MONEY = "GM_ADD_MONEY";
    private Dictionary<GrantType, GrantBenefitsBase> m_items = new Dictionary<GrantType, GrantBenefitsBase>();

    public static AddMoneyMgr getInstance()
    {
        if (s_mgr == null)
        {
            s_mgr = new AddMoneyMgr();
            s_mgr.init();
        }
        return s_mgr;
    }

    // 给指定玩家加钱，gold金币, gem钻石
    public bool addPlayerMoney(int playerid, int gold, int gem, GMUser account)
    {
        if (gold < 0 || gem < 0 || account == null)
        {
            LOG.Info("给玩家id [{0}]增加金币时出错，传入的值为负!", playerid);
            return false;
        }

        bool res = _addPlayerMoney(playerid, gold, gem, account.getDbServerID());
        if (res)
        {
            Dictionary<string, object> data =
               null; // DBMgr.getInstance().getTableData("EtPlayer", "Id", playerid, account.getDbServerID(), DbName.DB_FISH_LORD);
            if (data != null)
            {
                if (data.ContainsKey("IsBot"))
                {
                     bool isbot = Convert.ToBoolean(data["IsBot"]);
                     if (!isbot)
                     {
                         OpLogMgr.getInstance().addLog(LogType.ADD_MONEY, new ParamAddMoney(playerid, gold, gem), account);
                     }
                }
            }
        }
        account.setOpResult(res ? OpRes.opres_success : OpRes.op_res_failed);
        return res;
    }

    // 向机器人批量加钱
    public bool addMoneyToBot(int gold, int gem, GMUser account)
    {
        if (gold < 0 || gem < 0 || account == null)
        {
            LOG.Info("给机器人加钱时出错，传入的值为负!");
            return false;
        }

        int serverid = account.getDbServerID();
        List<Dictionary<string, object>> datalist =
            null; // DBMgr.getInstance().getDataListFromTable("EtPlayer", serverid, DbName.DB_FISH_LORD, "IsBot", true);

        if (datalist != null)
        {
            foreach (Dictionary<string, object> data in datalist)
            {
                int playerid = Convert.ToInt32(data["Id"]);
                _addPlayerMoney(playerid, gold, gem, serverid);
            }
        }
        account.setOpResult(OpRes.opres_success);
        OpLogMgr.getInstance().addLog(LogType.BATCH_ADD_MONEY, new ParamAddMoney(gold, gem), account);
        return true;
    }

    // 发放福利的接口
    public OpRes doGrantBenefit(object param, GrantType type, GMUser user)
    {
        if (!m_items.ContainsKey(type))
        {
            LOG.Info("不存在名称为[{0}]的发放福利功能", type);
            return OpRes.op_res_failed;
        }
        return m_items[type].doGrantBenefit(param, user);
    }

    // 查到结果
    private void onFindPlayer(Dictionary<string, object> data, string guid, object param)
    {
        if (data == null || param == null)
        {
            return;
        }

        try
        {
            if (data.ContainsKey("IsBot"))
            {
                bool isbot = Convert.ToBoolean(data["IsBot"]);
                // 是机器人就加
                if (isbot)
                {
                    AddMoneyParam p = (AddMoneyParam)param;
                    int playerid = Convert.ToInt32(data["Id"]);
                    _addPlayerMoney(playerid, p.m_gold, p.m_gem, p.m_dbId);
                }
            }
            else
            {
                LOG.Info("批量查询玩家数据时，有错，玩家guid [{0}]", guid);
            }
        }
        catch (System.Exception ex)
        {
            LOG.Info(ex.Message);
            LOG.Info(ex.StackTrace);
        }
    }

    // 给指定玩家加钱，gold金币, gem钻石
    private bool _addPlayerMoney(int playerid, int gold, int gem, int serverid)
    {
        if (gold < 0 || gem < 0)
        {
            LOG.Info("给玩家id [{0}]增加金币时出错，传入的值为负!", playerid);
            return false;
        }

        Dictionary<string, object> data = null; // DBMgr.getInstance().getTableData("EtPlayer", "Id", playerid, serverid, DbName.DB_FISH_LORD);
        // 没有找到该玩家
        if (data == null)
            return false;

        string guid = Convert.ToString(data["_key"]);
        data = null; // DBMgr.getInstance().getTableData(DB_KEY_ADD_MONEY, "player_guid", guid, serverid, DbName.DB_GM_BACK);
        if (data == null)
        {
            data = new Dictionary<string, object>();
            data["player_guid"] = guid;
            data["gold"] = gold;
            data["gem"] = gem;
        }
        else
        {
            int d1 = Convert.ToInt32(data["gold"]);
            int d2 = Convert.ToInt32(data["gem"]);
            d1 += gold;
            d2 += gem;
            data["gold"] = d1;
            data["gem"] = d2;
        }
        return true; // DBMgr.getInstance().save(DB_KEY_ADD_MONEY, data, "player_guid", guid, serverid, DbName.DB_GM_BACK);
    }

    private void init()
    {
        // 增加金币
        m_items.Add(GrantType.grant_type_gold, new GrantGold());
        m_items.Add(GrantType.grant_type_gem, new GrantGem());
        m_items.Add(GrantType.gran_type_item, new GrantItem());
        m_items.Add(GrantType.gran_type_ticket, new GrantTicket());
    }
}

//////////////////////////////////////////////////////////////////////////
// 发放的福利类型
public enum GrantType
{
    grant_type_gold,    // 发放金币
    grant_type_gem,     // 发放钻石
    gran_type_item,     // 发放道具
    gran_type_ticket,   // 发放礼券
}

public enum GrantTarget
{
    grant_target_someone, // 给某个人
    grant_target_all,     // 给所有用户发
    grant_target_vip,     // 向所有VIP发
}

public enum PaymentType
{
    e_pt_none = 0,
    e_pt_mm,            //MM支付
    e_pt_weipai,        //微派
    e_pt_oppo,          //oppo 可可
    e_pt_lv_an,         //绿岸
    e_pt_yidongjidi,    //移动基地
    e_pt_igame,         //电信爱游戏
    e_pt_unipay,         //联通沃商店
    e_pt_alipay,        //支付宝

    e_pt_mm1,            //MM支付
    e_pt_qcpay,         //千尺
    e_pt_zhy,           //智慧游
    e_pt_oppay,         //冒泡
    e_pt_yywan,         //yywan

    e_pt_anysdk,        //anysdk综合
    e_pt_xiaomi,        //小米

    e_pt_jodo,          //卓动淘游戏
    e_pt_nduo,          //N多，这是平台名，不是很多的意思
    e_pt_uucun,         //悠悠村

    e_pt_pingan,        //平安游戏

    e_pt_yidong2,       //移动 渠道版
    //蓝色2.0
    e_pt_mm3,           //移动MM
    e_pt_igame3,        //爱游戏
    e_pt_unipay3,       //沃商店

    e_pt_igame4,        //爱游戏

    e_pt_max,
}

public class ParamGrant
{
    public GrantTarget m_target;
    // 发放参数
    public string m_grantParam;
    public string m_playerId = "";
    // 指定的等级
    public string m_level = "";
    // 平台索引
    public int m_platIndex = 0;

    // 给指定玩家列表发放福利时，这里会返回发放失败的玩家，
    // 列表返回空，则表示成功。
    public string m_failedPlayer = "";
    // 发放成功的玩家列表
    public string m_successPlayer = "";
    public string m_failedItem = "";
}

public class DBAddBenefit
{
    // 玩家的GUID
    public string m_playerGUID = "";
    // 发放的福利类型
    public int m_grantType;
    // 发放数量
    public int m_count;
    // 额外参数1
    public int m_exParam1;
    public string m_exParam2 = "";
}

//////////////////////////////////////////////////////////////////////////

// 发放福利基类
public class GrantBenefitsBase
{
    // 发放福利的表格
    public const string GM_BENEFITS = "GM_GRANT_BENEFITS";
    private IMongoQuery m_imq = null;
    private int m_curIndex;
    private string[] m_fields = null;
    private string[] m_fields2 = null;
    // 平台相关
    private IMongoQuery m_imqPlatform = null;
    // 账号域
    private string[] m_fieldAccount = new string[] { "Account" };

    public virtual OpRes doGrantBenefit(object param, GMUser user)
    {
        return OpRes.op_res_failed;
    }

    // imq是指定的外部条件，若为空，则内部使用默认条件，否则使用imq
    public string firstPlayerGUID(bool isvip, GMUser user, IMongoQuery imq = null, string plat = "none")
    {
        if (isvip)
        {
            m_fields = new string[2] { "_key", "VipMgrEntityId" };
            m_fields2 = new string[2] { "_key", "VipCardLevel" };
        }
        else
        {
            m_fields = new string[1] { "_key" };
        }
        m_curIndex = 1;
        if (imq == null)
        {
            m_imq = Query.NE("_key", "");
        }
        else
        {
            m_imq = imq;
        }
        if (plat != "none")
        {
            m_imqPlatform = Query.EQ("ClientType", BsonValue.Create(plat));
        }
        else
        {
            m_imqPlatform = Query.NE("ClientType", BsonValue.Create(""));
        }

        if(isvip)
            return nextGUIDVIP(user, m_curIndex - 1);
        return nextGUID(user, m_curIndex - 1);
    }

    // 初始化发放福利
    public void initBenefit(bool isvip, GMUser user, IMongoQuery imq = null, string plat = "none")
    {
        if (isvip)
        {
            m_fields = new string[2] { "_key", "VipMgrEntityId" };
            m_fields2 = new string[2] { "_key", "VipCardLevel" };
        }
        else
        {
            m_fields = new string[1] { "_key" };
        }
        m_curIndex = 0;
        if (imq == null)
        {
            m_imq = Query.NE("_key", "");
        }
        else
        {
            m_imq = imq;
        }
        if (plat != "none")
        {
            m_imqPlatform = Query.EQ("ClientType", BsonValue.Create(plat));
        }
        else
        {
            m_imqPlatform = Query.NE("ClientType", BsonValue.Create(""));
        }
    }

    public string nextPlayerGUID(GMUser user)
    {
        m_curIndex++;
        if (m_fields.Length == 2)
            return nextGUIDVIP(user, m_curIndex - 1);
        return nextGUID(user, m_curIndex - 1);
    }

    public bool nextPlayerGUIDList(GMUser user, int count, List<string> res)
    {
        if (m_fields.Length == 2)
            return nextGUIDVIPList(user, ref m_curIndex, count, res);
        return nextGUIDList(user, ref m_curIndex, count, res);
    }

    public bool addBenefitToDB(DBAddBenefit param, GMUser user)
    {
        IMongoQuery imq = getBenefitsQuery(param);

        Dictionary<string, object> data = null; // DBMgr.getInstance().getTableData(GM_BENEFITS, user.getDbServerID(), DbName.DB_GM_BACK, imq);
        if (data == null)
        {
            data = new Dictionary<string, object>();
            data["player_guid"] = param.m_playerGUID;
            data["type"] = param.m_grantType;
            data["count"] = param.m_count;
            data["param1"] = param.m_exParam1; // 对于道具表示道具的ID
        }
        else
        {
            int d = Convert.ToInt32(data["count"]);
            d += param.m_count;
            data["count"] = d;
        }
        return true; // DBMgr.getInstance().insertData(GM_BENEFITS, data, user.getDbServerID(), DbName.DB_GM_BACK);
    }

    public bool addFullBenefitToDB(DBAddBenefit param, GMUser user, int plat)
    {
        Dictionary<string, object> data =
            null; // DBMgr.getInstance().getTableData("GM_FULL_BENEFITS", "type", param.m_grantType, user.getDbServerID(), DbName.DB_GM_BACK); //new Dictionary<string, object>();
        if (data == null)
        {
            data = new Dictionary<string, object>();
            data["type"] = param.m_grantType;
            data["count"] = param.m_count;
            data["param2"] = param.m_exParam2; // 对于道具表示道具的ID
            data["plat"] = plat;
            data["Gentime"] = DateTime.Now.Ticks;
        }
        else
        {
            int d = Convert.ToInt32(data["count"]);
            if (param.m_count < 0)
            {
                d += param.m_count;
                data["count"] = d;
            }
            else
            {
                data["count"] = param.m_count;
            }
            data["plat"] = plat;
            data["Gentime"] = DateTime.Now.Ticks;
        }

        return true; // DBMgr.getInstance().save("GM_FULL_BENEFITS", data, "type", param.m_grantType, user.getDbServerID(), DbName.DB_GM_BACK);
    }

    private string nextGUID(GMUser user, int skip)
    {
        bool res = true;
        while (res)
        {
            List<Dictionary<string, object>> data =
               null; // DBMgr.getInstance().executeQuery("Account", user.getDbServerID(), DbName.DB_FISH_LORD, m_imqPlatform, skip, 1, m_fieldAccount);
            if (data == null || data.Count == 0)
                return "";

            string acc = Convert.ToString(data[0]["Account"]);

            IMongoQuery tmp_imq = Query.And(m_imq, Query.EQ("AccoutName", BsonValue.Create(acc)));

            data = null; // DBMgr.getInstance().executeQuery("EtPlayer", user.getDbServerID(), DbName.DB_FISH_LORD, tmp_imq, 0, 1, m_fields);
            if (data == null || data.Count == 0)
            {
                skip = m_curIndex;
                m_curIndex++;
            }
            else
            {
                return Convert.ToString(data[0]["_key"]); 
            }
        }

        return "";

     /*   List<Dictionary<string, object>> data =
            DBMgr.getInstance().executeQuery("EtPlayer", user.getDbServerID(), DbName.DB_FISH_LORD, m_imq, skip, 1, m_fields);
        if (data == null || data.Count == 0)
            return "";
        return Convert.ToString(data[0]["_key"]);*/
    }

    // 返回true表示成功，false失败。
    // res_list返回串列表
    private bool nextGUIDList(GMUser user, ref int skip, int count, List<string> res_list)
    {
        res_list.Clear();
        
        List<Dictionary<string, object>> data =
            null; // DBMgr.getInstance().executeQuery("Account", user.getDbServerID(), DbName.DB_FISH_LORD, m_imqPlatform, skip, count, m_fieldAccount);
        if (data == null || data.Count == 0)
            return false;
        
        skip += count;

        for (int i = 0; i < data.Count; i++)
        {
            if (data[i].ContainsKey("Account"))
            {
                string acc = Convert.ToString(data[i]["Account"]);
                IMongoQuery tmp_imq = Query.And(m_imq, Query.EQ("AccoutName", BsonValue.Create(acc)));
                List<Dictionary<string, object>> ret_data = null; // DBMgr.getInstance().executeQuery("EtPlayer", user.getDbServerID(), DbName.DB_FISH_LORD, tmp_imq, 0, 1, m_fields);
                if (ret_data != null && ret_data.Count > 0)
                {
                    res_list.Add(Convert.ToString(ret_data[0]["_key"]));
                }
            }
        }
        return true;
    }

    // 需要多边一个条件，是vip
    private string nextGUIDVIP(GMUser user, int skip)
    {
        bool res = true;
        while (res)
        {
            List<Dictionary<string, object>> data =
               null; // DBMgr.getInstance().executeQuery("Account", user.getDbServerID(), DbName.DB_FISH_LORD, m_imqPlatform, skip, 1, m_fieldAccount);
            if (data == null || data.Count == 0)
                return "";

            string acc = Convert.ToString(data[0]["Account"]);

            IMongoQuery tmp_imq = Query.EQ("AccoutName", BsonValue.Create(acc));

            data = null; // DBMgr.getInstance().executeQuery("EtPlayer", user.getDbServerID(), DbName.DB_FISH_LORD, tmp_imq, 0, 1, m_fields);
            if (data == null || data.Count == 0)
            {
                skip = m_curIndex;
                m_curIndex++;
            }
            else
            {
              //  m_fields = new string[2] { "_key", "VipMgrEntityId" };
                string vip_ip = Convert.ToString(data[0]["VipMgrEntityId"]);
                string guid = Convert.ToString(data[0]["_key"]);
                IMongoQuery imq_vip = Query.And(m_imq, Query.EQ("_key", BsonValue.Create(vip_ip)));

                data = null; // DBMgr.getInstance().executeQuery("EtVipMgr", user.getDbServerID(), DbName.DB_FISH_LORD, imq_vip, 0, 1, m_fields2);
                if (data == null || data.Count == 0)
                {
                    skip = m_curIndex;
                    m_curIndex++;
                }
                else
                {
                    return guid;
                }
            }
        }

        return "";

       /* bool res = true;
        while (res)
        {
            List<Dictionary<string, object>> data =
                DBMgr.getInstance().executeQuery("EtVipMgr", user.getDbServerID(), DbName.DB_FISH_LORD, m_imq, skip, 1, m_fields2);
            if (data == null || data.Count == 0)
                return "";

            string vipkey = Convert.ToString(data[0]["_key"]);

            Dictionary<string, object> ret = DBMgr.getInstance().getTableData("EtPlayer", "VipMgrEntityId", vipkey, m_fields, user.getDbServerID(), DbName.DB_FISH_LORD);
            if (ret != null)
            {
                return Convert.ToString(ret["_key"]);
            }
            skip = m_curIndex;
            m_curIndex++;
        }
        return "";*/
    }

    // 需要多边一个条件，是vip
    private bool nextGUIDVIPList(GMUser user, ref int skip, int count, List<string> res_list)
    {
        res_list.Clear();
        List<Dictionary<string, object>> data =
            null; // DBMgr.getInstance().executeQuery("Account", user.getDbServerID(), DbName.DB_FISH_LORD, m_imqPlatform, skip, count, m_fieldAccount);
        if (data == null || data.Count == 0)
            return false;

        skip += count;

        for (int i = 0; i < data.Count; i++)
        {
            string acc = Convert.ToString(data[i]["Account"]);

            Dictionary<string, object> ret_data = null; // DBMgr.getInstance().getTableData("EtPlayer", "AccoutName", acc, m_fields, user.getDbServerID(), DbName.DB_FISH_LORD);
            if (ret_data != null)
            {
                string vip_ip = Convert.ToString(ret_data["VipMgrEntityId"]);
                IMongoQuery imq_vip = Query.And(m_imq, Query.EQ("_key", BsonValue.Create(vip_ip)));
                List<Dictionary<string, object>> vip_data = null; // DBMgr.getInstance().executeQuery("EtVipMgr", user.getDbServerID(), DbName.DB_FISH_LORD, imq_vip, 0, 1, m_fields2);
                if (vip_data != null && vip_data.Count > 0)
                {
                    res_list.Add(Convert.ToString(ret_data["_key"]));
                }
            }
        }
        return true;
    }

    protected virtual IMongoQuery getBenefitsQuery(DBAddBenefit param)
    {
        return Query.And(new IMongoQuery[] { Query.EQ("player_guid", BsonValue.Create(param.m_playerGUID)), Query.EQ("type", BsonValue.Create(param.m_grantType)) });
    }

    // 根据用户ID返回用户的GUID
    protected string getGUIDById(int id, GMUser user)
    {
        m_fields = new string[1] { "_key" };
        Dictionary<string, object> data = null; // DBMgr.getInstance().getTableData("EtPlayer", "Id", id, m_fields, user.getDbServerID(), DbName.DB_FISH_LORD);
        if (data == null)
            return "";
        return Convert.ToString(data["_key"]);
    }

    // 取得等级条件，若是VIP，则返回VIP等级条件；不是则返回玩家等级条件
    protected virtual IMongoQuery getLevelImq(bool isvip, int level)
    {
        if (isvip)
        {
            return Query.GTE("VipCardLevel", BsonValue.Create(level));
        }
        return Query.GTE("PlayerLevel", BsonValue.Create(level));
    }

    // 返回平台名称
    protected string getPlatformName(int index, bool eng = true)
    {
        XmlConfig xml = ResMgr.getInstance().getRes("platform.xml");
        List<Dictionary<string, object>> ldata = xml.getTable(index.ToString());
        if (ldata == null)
        {
            return "none";
        }
        if(eng)
            return Convert.ToString(ldata[0]["eng"]);
        return Convert.ToString(ldata[0]["cha"]);
    }

    // 给某个玩家列表发放福利
    protected OpRes addReardToPlayerList(ParamGrant param, int count, int m_grantType, GMUser user)
    {
        List<int> playerList = new List<int>();
        bool res = Tool.parseNumList(param.m_playerId, playerList);
        if (!res)
            return OpRes.op_res_param_not_valid;

        try
        {
            for (int i = 0; i < playerList.Count; i++)
            {
                int playerid = playerList[i];
                string guid = getGUIDById(playerid, user);
                if (guid != "")
                {
                    DBAddBenefit tmp = new DBAddBenefit();
                    tmp.m_playerGUID = guid;
                    tmp.m_grantType = m_grantType;
                    tmp.m_count = count;
                    if (!addBenefitToDB(tmp, user))
                    {
                        param.m_failedPlayer += playerid + " ";
                    }
                    else
                    {
                        param.m_successPlayer += playerid + " ";
                    }
                }
                else
                {
                    param.m_failedPlayer += playerid + " ";
                }
            }
        }
        catch (System.Exception ex)
        {
        }
        return param.m_successPlayer == "" ? OpRes.op_res_failed : OpRes.opres_success;
    }
}

//////////////////////////////////////////////////////////////////////////
// 增加金币
public class GrantGold : GrantBenefitsBase
{
    public override OpRes doGrantBenefit(object param, GMUser user)
    {
        ParamGrant p = (ParamGrant)param;
        int count = 0;
        try
        {
            count = Convert.ToInt32(p.m_grantParam);
        }
        catch (System.Exception ex)
        {
            return OpRes.op_res_param_not_valid;
        }

        // 发放金币时，可以是负，这样可以减少玩家金币
       // if(count <= 0)
          //  return OpRes.op_res_failed;

        if (p.m_target == GrantTarget.grant_target_someone)
        {
            OpRes res = addReardToPlayerList(p, count, (int)GrantType.grant_type_gold, user);
            if (res == OpRes.opres_success)
            {
                OpLogMgr.getInstance().addLog(LogType.LOG_TYPE_ADD_BENEFIT_GOLD, new ParamAddBenefitGold(p.m_successPlayer, count, (int)GrantTarget.grant_target_someone, 0), user);
            }
            return res;
        }

        int level = 1;
        // 查询条件
        if (p.m_level != "")
        {
            try
            {
                // 转至level
                level = Convert.ToInt32(p.m_level);
                if (level <= 0)
                {
                    level = 1;
                }
            }
            catch (System.Exception ex)
            {
                return OpRes.op_res_failed;
            }
        }
        IMongoQuery imq = getLevelImq(p.m_target == GrantTarget.grant_target_vip, level);

       /* string guid = firstPlayerGUID(p.m_target == GrantTarget.grant_target_vip, user, imq, getPlatformName(p.m_platIndex));
        while (guid != "")
        {
            DBAddBenefit tmp = new DBAddBenefit();
            tmp.m_playerGUID = guid;
            tmp.m_grantType = (int)GrantType.grant_type_gold;
            tmp.m_count = count;
            addBenefitToDB(tmp, user);
            guid = nextPlayerGUID(user);
        }*/
#if _OLD_BENEFIT_

        List<string> guid_list = new List<string>();
        initBenefit(p.m_target == GrantTarget.grant_target_vip, user, imq, getPlatformName(p.m_platIndex));
        bool run = nextPlayerGUIDList(user, 1000, guid_list);
        while (run)
        {
            for (int i = 0; i < guid_list.Count; i++)
            {
                DBAddBenefit tmp = new DBAddBenefit();
                tmp.m_playerGUID = guid_list[i];
                tmp.m_grantType = (int)GrantType.grant_type_gold;
                tmp.m_count = count;
                addBenefitToDB(tmp, user);
            }
            guid_list.Clear();
            run = nextPlayerGUIDList(user, 1000, guid_list);
        }
#else
        DBAddBenefit tmp = new DBAddBenefit();
        tmp.m_grantType = (int)GrantType.grant_type_gold;
        tmp.m_count = count;
        addFullBenefitToDB(tmp, user, p.m_platIndex);
#endif
        OpLogMgr.getInstance().addLog(LogType.LOG_TYPE_ADD_BENEFIT_GOLD,
            new ParamAddBenefitGold("", count, (int)p.m_target, level, getPlatformName(p.m_platIndex, false)), user);
        return OpRes.opres_success;
    }
}

//////////////////////////////////////////////////////////////////////////
// 发放钻石
public class GrantGem : GrantBenefitsBase
{
    public override OpRes doGrantBenefit(object param, GMUser user)
    {
        ParamGrant p = (ParamGrant)param;
        int count = 0;
        try
        {
            count = Convert.ToInt32(p.m_grantParam);
        }
        catch (System.Exception ex)
        {
            return OpRes.op_res_param_not_valid;
        }

       // if (count <= 0)
        //    return OpRes.op_res_failed;

        if (p.m_target == GrantTarget.grant_target_someone)
        {
            OpRes res = addReardToPlayerList(p, count, (int)GrantType.grant_type_gem, user);
            if (res == OpRes.opres_success)
            {
                OpLogMgr.getInstance().addLog(LogType.LOG_TYPE_ADD_BENEFIT_GEM, new ParamAddBenefitGem(p.m_successPlayer, count, (int)GrantTarget.grant_target_someone, 0), user);
            }
            return res;
        }

        int level = 1;
        if (p.m_level != "")
        {
            try
            {
                // 转至level
                level = Convert.ToInt32(p.m_level);
                if (level <= 0)
                {
                    level = 1;
                }
            }
            catch (System.Exception ex)
            {
                return OpRes.op_res_failed;
            }
        }
        IMongoQuery imq = getLevelImq(p.m_target == GrantTarget.grant_target_vip, level);
        /*string guid = firstPlayerGUID(p.m_target == GrantTarget.grant_target_vip, user, imq, getPlatformName(p.m_platIndex));
        while (guid != "")
        {
            DBAddBenefit tmp = new DBAddBenefit();
            tmp.m_playerGUID = guid;
            tmp.m_grantType = (int)GrantType.grant_type_gem;
            tmp.m_count = count;
            addBenefitToDB(tmp, user);
            guid = nextPlayerGUID(user);
        }*/
#if _OLD_BENEFIT_
        List<string> guid_list = new List<string>();
        initBenefit(p.m_target == GrantTarget.grant_target_vip, user, imq, getPlatformName(p.m_platIndex));
        bool run = nextPlayerGUIDList(user, 1000, guid_list);
        while (run)
        {
            for (int i = 0; i < guid_list.Count; i++)
            {
                DBAddBenefit tmp = new DBAddBenefit();
                tmp.m_playerGUID = guid_list[i];
                tmp.m_grantType = (int)GrantType.grant_type_gem;
                tmp.m_count = count;
                addBenefitToDB(tmp, user);
            }
            guid_list.Clear();
            run = nextPlayerGUIDList(user, 1000, guid_list);
        }
#else
        DBAddBenefit tmp = new DBAddBenefit();
        tmp.m_grantType = (int)GrantType.grant_type_gem;
        tmp.m_count = count;
        addFullBenefitToDB(tmp, user, p.m_platIndex);
#endif
        OpLogMgr.getInstance().addLog(LogType.LOG_TYPE_ADD_BENEFIT_GEM, 
            new ParamAddBenefitGem("", count, (int)p.m_target, level, getPlatformName(p.m_platIndex, false)), user);
        return OpRes.opres_success;
    }
}

//////////////////////////////////////////////////////////////////////////

// 发放礼券
public class GrantTicket : GrantBenefitsBase
{
    public override OpRes doGrantBenefit(object param, GMUser user)
    {
        ParamGrant p = (ParamGrant)param;
        int count = 0;
        try
        {
            count = Convert.ToInt32(p.m_grantParam);
        }
        catch (System.Exception ex)
        {
            return OpRes.op_res_param_not_valid;
        }

        //if (count <= 0)
          //  return OpRes.op_res_failed;

        if (p.m_target == GrantTarget.grant_target_someone)
        {
            OpRes res = addReardToPlayerList(p, count, (int)GrantType.gran_type_ticket, user);
            if (res == OpRes.opres_success)
            {
                OpLogMgr.getInstance().addLog(LogType.LOG_TYPE_ADD_BENEFIT_GIFT, new ParamAddBenefitGift(p.m_successPlayer, count, (int)GrantTarget.grant_target_someone, 0), user);
            }
            return res;
        }

        int level = 1;
        if (p.m_level != "")
        {
            try
            {
                // 转至level
                level = Convert.ToInt32(p.m_level);
                if (level <= 0)
                {
                    level = 1;
                }
            }
            catch (System.Exception ex)
            {
                return OpRes.op_res_failed;
            }
        }
#if _OLD_BENEFIT_
        IMongoQuery imq = getLevelImq(p.m_target == GrantTarget.grant_target_vip, level);
        List<string> guid_list = new List<string>();
        initBenefit(p.m_target == GrantTarget.grant_target_vip, user, imq, getPlatformName(p.m_platIndex));
        bool run = nextPlayerGUIDList(user, 1000, guid_list);
        while (run)
        {
            for (int i = 0; i < guid_list.Count; i++)
            {
                DBAddBenefit tmp = new DBAddBenefit();
                tmp.m_playerGUID = guid_list[i];
                tmp.m_grantType = (int)GrantType.gran_type_ticket;
                tmp.m_count = count;
                addBenefitToDB(tmp, user);
            }
            guid_list.Clear();
            run = nextPlayerGUIDList(user, 1000, guid_list);
        }
#else
        DBAddBenefit tmp = new DBAddBenefit();
        tmp.m_grantType = (int)GrantType.gran_type_ticket;
        tmp.m_count = count;
        addFullBenefitToDB(tmp, user, p.m_platIndex);
#endif

        OpLogMgr.getInstance().addLog(LogType.LOG_TYPE_ADD_BENEFIT_GIFT,
            new ParamAddBenefitGift("", count, (int)p.m_target, level, getPlatformName(p.m_platIndex, false)), user);
        return OpRes.opres_success;
    }
}

//////////////////////////////////////////////////////////////////////////

public class ParamItem
{
    public int m_itemId;
    public int m_itemCount;
}

// 发放道具
public class GrantItem : GrantBenefitsBase
{
    public override OpRes doGrantBenefit(object param, GMUser user)
    {
        ParamGrant p = (ParamGrant)param;
        List<ParamItem> item_list = new List<ParamItem>();
        try
        {
            Match match = Regex.Match(p.m_grantParam, Exp.TWO_NUM_BY_SPACE);
            if (!match.Success)
            {
                match = Regex.Match(p.m_grantParam, Exp.TWO_NUM_BY_SPACE_SEQ);
                if (!match.Success)
                {
                    return OpRes.op_res_param_not_valid;
                }
            }
            parseItem(p.m_grantParam, item_list);
        }
        catch (System.Exception ex)
        {
            return OpRes.op_res_failed;
        }

        if (item_list.Count == 0)
            return OpRes.op_res_param_not_valid;

        for (int i = 0; i < item_list.Count; i++)
        {
            
        }

        if (p.m_target == GrantTarget.grant_target_someone)
        {
            OpRes res = addItemToSomeOne(p, item_list, user);
            if (res == OpRes.opres_success)
            {
            }
            return res;
        }

        int level = 1;
        if (p.m_level != "")
        {
            try
            {
                // 转至level
                level = Convert.ToInt32(p.m_level);
                if (level <= 0)
                {
                    level = 1;
                }
            }
            catch (System.Exception ex)
            {
                return OpRes.op_res_failed;
            }
        }
        IMongoQuery imq = getLevelImq(p.m_target == GrantTarget.grant_target_vip, level);
        /*string guid = firstPlayerGUID(p.m_target == GrantTarget.grant_target_vip, user, imq, getPlatformName(p.m_platIndex));
        while (guid != "")
        {
            for (int i = 0; i < item_list.Count; i++)
            {
                DBAddBenefit tmp = new DBAddBenefit();
                tmp.m_playerGUID = guid;
                tmp.m_grantType = (int)GrantType.gran_type_item;
                tmp.m_count = item_list[i].m_itemCount;
                tmp.m_exParam1 = item_list[i].m_itemId;       // 这个参数存储道具ID
                addBenefitToDB(tmp, user);
            }
            guid = nextPlayerGUID(user);
        }*/
#if _OLD_BENEFIT_
        List<string> guid_list = new List<string>();
        initBenefit(p.m_target == GrantTarget.grant_target_vip, user, imq, getPlatformName(p.m_platIndex));
        bool run = nextPlayerGUIDList(user, 1000, guid_list);
        while (run)
        {
            for (int j = 0; j < guid_list.Count; j++)
            {
                for (int i = 0; i < item_list.Count; i++)
                {
                    DBAddBenefit tmp = new DBAddBenefit();
                    tmp.m_playerGUID = guid_list[j];
                    tmp.m_grantType = (int)GrantType.gran_type_item;
                    tmp.m_count = item_list[i].m_itemCount;
                    tmp.m_exParam1 = item_list[i].m_itemId;       // 这个参数存储道具ID
                    addBenefitToDB(tmp, user);
                }
            }
            run = nextPlayerGUIDList(user, 1000, guid_list);
        }
#else
        DBAddBenefit tmp = new DBAddBenefit();
        tmp.m_grantType = (int)GrantType.gran_type_item;
        tmp.m_exParam2 = BaseJsonSerializer.serialize(item_list);
        addFullBenefitToDB(tmp, user, p.m_platIndex);
#endif
//         OpLogMgr.getInstance().addLog(LogType.LOG_TYPE_ADD_BENEFIT_ITEM,
//             new ParamAddBenefitItem(p.m_playerId, item_list, (int)p.m_target, level, getPlatformName(p.m_platIndex, false)), user);
        return OpRes.opres_success;
    }

    protected override IMongoQuery getBenefitsQuery(DBAddBenefit param)
    {
        return Query.And(new IMongoQuery[] { Query.EQ("player_guid", BsonValue.Create(param.m_playerGUID)),
            Query.EQ("type", BsonValue.Create(param.m_grantType)),
            Query.EQ("param1", BsonValue.Create(param.m_exParam1))});
    }

    private OpRes addItemToSomeOne(ParamGrant param, List<ParamItem> item_list, GMUser user)
    {
        List<int> playerList = new List<int>();
        bool res = Tool.parseNumList(param.m_playerId, playerList);
        if (!res)
            return OpRes.op_res_param_not_valid;

        try
        {
            bool addSucc = false;

            for (int j = 0; j < playerList.Count; j++)
            {
                int playerid = playerList[j];
                string guid = getGUIDById(playerid, user);
                if (guid != "")
                {
                    addSucc = false;

                    for (int i = 0; i < item_list.Count; i++)
                    {
                        DBAddBenefit tmp = new DBAddBenefit();
                        tmp.m_playerGUID = guid;
                        tmp.m_grantType = (int)GrantType.gran_type_item;
                        tmp.m_count = item_list[i].m_itemCount;
                        tmp.m_exParam1 = item_list[i].m_itemId;
                        if (!addBenefitToDB(tmp, user))
                        {
                            param.m_failedPlayer += playerid + " ";
                            param.m_failedItem += tmp.m_exParam1 + " ";
                        }
                        else
                        {
                            if (!addSucc)
                            {
                                param.m_successPlayer += playerid + " ";
                                addSucc = true;
                            }
                        }
                    }
                }
                else
                {
                    param.m_failedPlayer += playerid + " ";
                    param.m_failedItem += "* ";
                }
            }
        }
        catch (System.Exception ex)
        {
        }
        return param.m_successPlayer == "" ? OpRes.op_res_failed : OpRes.opres_success;
    }

    private void parseItem(string str, List<ParamItem> result)
    {
        string[] arr = str.Split(';');
        int i = 0;
        for (; i < arr.Length; i++)
        {
            string[] tmp = Tool.split(arr[i], ' ', StringSplitOptions.RemoveEmptyEntries);
            ParamItem item = new ParamItem();
            item.m_itemId = Convert.ToInt32(tmp[0]);
            item.m_itemCount = Convert.ToInt32(tmp[1]);
            result.Add(item);
        }
    }
}




