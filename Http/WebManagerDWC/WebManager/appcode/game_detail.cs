using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

// 游戏详细信息
public class GameDetail
{
    public static GameDetailInfo parseGameInfo(GameId gameId, int index, GMUser user)
    {
        MoneyItemDetail item = getMoneyItem(index, user);
        if (item == null)
            return null;

        object info = null;
        switch (gameId)
        {
            case GameId.baccarat:
                {
                    info = parseInfoBaccarat(item.m_exInfo);
                }
                break;
            case GameId.cows:
                {
                    info = parseInfoCows(item.m_exInfo, user);
                }
                break;
            case GameId.crocodile:
                {
                    info = parseInfoCrocodile(item.m_exInfo);
                }
                break;
            case GameId.dice:
                {
                    info = parseInfoDice(item.m_exInfo);
                }
                break;
            case GameId.fishpark:
                {
                    info = parseInfoFishPark(item.m_exInfo);
                }
                break;
            case GameId.shcd:
                {
                    info = parseInfoShcd(item.m_exInfo);
                }
                break;
        }
        if (info == null)
            return null;

        GameDetailInfo resInfo = new GameDetailInfo();
        resInfo.m_item = item;
        resInfo.m_detailInfo = info;
        return resInfo;
    }

    // 解析百家乐json串信息
    public static InfoBaccarat parseInfoBaccarat(string exInfo)
    {
        InfoBaccarat info = null;
        try
        {
            Dictionary<string, object> dic = BaseJsonSerializer.deserialize<Dictionary<string, object>>(exInfo);
            info = new InfoBaccarat();
            if (dic.ContainsKey("betinfo"))
            {
                info.m_betInfo = BaseJsonSerializer.deserialize<List<BetInfo>>(dic["betinfo"].ToString());
            }
            if (dic.ContainsKey("bankercard"))
            {
                info.m_bankerCard = BaseJsonSerializer.deserialize<List<CardInfo>>(dic["bankercard"].ToString());
            }
            if (dic.ContainsKey("playercard"))
            {
                info.m_playerCard = BaseJsonSerializer.deserialize<List<CardInfo>>(dic["playercard"].ToString());
            }
            if (dic.ContainsKey("isbanker"))
            {
                int t = Convert.ToInt32(dic["isbanker"]);
                info.setIsBanker(t);
            }
            if (dic.ContainsKey("chargerate"))
            {
                info.m_serviceChargeRatio = Convert.ToInt32(dic["chargerate"]);
            }
            if (dic.ContainsKey("charge"))
            {
                info.m_serviceCharge = Convert.ToInt32(dic["charge"]);
            }
        }
        catch (System.Exception ex)
        {
        	
        }
        return info;
    }

    // 解析牛牛json串信息
    public static InfoCows parseInfoCows(string exInfo, GMUser user)
    {
        InfoCows info = null;
        try
        {
            DbCowsBet bet = BaseJsonSerializer.deserialize<DbCowsBet>(exInfo);
            if (bet == null)
                return null;

            info = new InfoCows();
            info.m_betInfo = bet;

            ObjectId oid = ObjectId.Parse(bet.key);
            Dictionary<string, object> data = DBMgr.getInstance().getTableData(TableName.PUMP_COWS_CARD,
                  "_id",
                  oid,
                  user.getDbServerID(),
                  DbName.DB_PUMP);

            if (data == null)
                return null;

            detailInfoForCows(info, data);
        }
        catch (System.Exception ex)
        {

        }
        return info;
    }

    public static void detailInfoForCows(InfoCows info, Dictionary<string, object> data)
    {
        if (info == null || data == null)
            return;

        if (data.ContainsKey("bankerCards")) // 庄家牌型
        {
            DbCowsCard c = BaseJsonSerializer.deserialize<DbCowsCard>(data["bankerCards"].ToString());
            info.createBankerCard(c);
        }
        if (data.ContainsKey("otherCards1")) // 东牌型
        {
            DbCowsCard c = BaseJsonSerializer.deserialize<DbCowsCard>(data["otherCards1"].ToString());
            info.createEastCard(c);
        }
        if (data.ContainsKey("otherCards2")) // 南牌型
        {
            DbCowsCard c = BaseJsonSerializer.deserialize<DbCowsCard>(data["otherCards2"].ToString());
            info.createSouthCard(c);
        }
        if (data.ContainsKey("otherCards3")) // 西牌型
        {
            DbCowsCard c = BaseJsonSerializer.deserialize<DbCowsCard>(data["otherCards3"].ToString());
            info.createWestCard(c);
        }
        if (data.ContainsKey("otherCards4")) // 北牌型
        {
            DbCowsCard c = BaseJsonSerializer.deserialize<DbCowsCard>(data["otherCards4"].ToString());
            info.createNorthCard(c);
        }
    }

    // 解析鳄鱼大亨json串信息
    public static InfoCrocodile parseInfoCrocodile(string exInfo)
    {
        InfoCrocodile info = null;
        try
        {
            Dictionary<string, object> dic = BaseJsonSerializer.deserialize<Dictionary<string, object>>(exInfo);
            info = new InfoCrocodile();
            if (dic.ContainsKey("betinfo"))
            {
                info.m_betInfo = BaseJsonSerializer.deserialize<List<BetInfoCrocodile>>(dic["betinfo"].ToString());
            }
            if (dic.ContainsKey("resulttype"))
            {
                info.m_resultType =
                    BaseJsonSerializer.deserialize<List<DbCrocodileResultType>>(dic["resulttype"].ToString());
            }
            if (dic.ContainsKey("resultList"))
            {
                info.m_resultList = BaseJsonSerializer.deserialize<List<DbCrocodileResultList>>(dic["resultList"].ToString());
            }
        }
        catch (System.Exception ex)
        {

        }
        return info;
    }

    // 解析骰宝json串信息
    public static InfoDice parseInfoDice(string exInfo)
    {
        InfoDice info = null;
        try
        {
            Dictionary<string, object> dic = BaseJsonSerializer.deserialize<Dictionary<string, object>>(exInfo);
            info = new InfoDice();
            if (dic.ContainsKey("dics_info"))
            {
                int num = Convert.ToInt32(dic["dics_info"]);
                info.setDiceNum(num);
            }
            if (dic.ContainsKey("bet_info"))
            {
                info.m_betInfo = BaseJsonSerializer.deserialize<List<DbDiceBet>>(dic["bet_info"].ToString());
            }
        }
        catch (System.Exception ex)
        {
        }
        return info;
    }

    // 解析鳄鱼公园json串信息
    public static InfoFishPark parseInfoFishPark(string exInfo)
    {
        InfoFishPark info = null;
        try
        {
            Dictionary<string, object> dic = BaseJsonSerializer.deserialize<Dictionary<string, object>>(exInfo);
            info = new InfoFishPark();
            if (dic.ContainsKey("Abandonedbullets"))
            {
                info.m_abandonedbullets = Convert.ToInt32(dic["Abandonedbullets"]);
            }
            if (dic.ContainsKey("fishinfos"))
            {
                info.m_fish = BaseJsonSerializer.deserialize<List<DbFish>>(dic["fishinfos"].ToString());
            }
            info.sort();
        }
        catch (System.Exception ex)
        {

        }
        return info;
    }

    // 解析黑红梅方json串信息
    public static InfoShcd parseInfoShcd(string exInfo)
    {
        InfoShcd info = null;
        try
        {
            info = BaseJsonSerializer.deserialize<InfoShcd>(exInfo);
        }
        catch (System.Exception ex)
        {

        }
        return info;
    }

    private static MoneyItemDetail getMoneyItem(int index, GMUser user)
    {
        List<MoneyItemDetail> result = (List<MoneyItemDetail>)user.getQueryResult(QueryType.queryTypeMoneyDetail);
        if (index < 0 || index >= result.Count)
            return null;

        return result[index];
    }

    public static CardInfo createCardInfo(int val)
    {
        CardInfo info = new CardInfo();
        info.flower = (val - 1) / 13;
        info.point = (val - 1) % 13 + 1;
        return info;
    }
}

public class GameDetailInfo
{
    public MoneyItem m_item;
    public object m_detailInfo;  // 详细信息
}

// 百家乐游戏信息
[Serializable]
public class InfoBaccarat
{
    // 0  1  2    3   4
    // 和 闲  闲对 庄对 庄
    public List<BetInfo> m_betInfo;
    // 庄家牌型
    public List<CardInfo> m_bankerCard;
    // 闲家牌型
    public List<CardInfo> m_playerCard;

    // 手续费比例
    public int m_serviceChargeRatio;
    // 具体手续费用
    public int m_serviceCharge;

    public bool m_isBanker = false;

    public bool isBanker() { return m_isBanker; }

    // 返回押注信息 betId押注id
    public BetInfo getBetInfo(int betId)
    {
        for (int i = 0; i < m_betInfo.Count; i++)
        {
            if (m_betInfo[i].bet_id == betId)
                return m_betInfo[i];
        }
        return new BetInfo(0, 0, 0);
    }

    public void setIsBanker(int isBanker)
    {
        if (isBanker == 1)
        {
            m_isBanker = true;
        }
    }

    public long sumBet()
    {
        int sum = 0;
        foreach (var item in m_betInfo)
        {
            sum += item.bet_count;
        }
        return sum;
    }

    public long sumAward()
    {
        int sum = 0;
        foreach (var item in m_betInfo)
        {
            sum += item.award_count;
        }
        return sum;
    }
}

// 下注信息
public class BetInfo
{
    public int bet_id;
    public int bet_count;
    public int award_count;

    public BetInfo() { }

    public BetInfo(int id, int c, int w)
    {
        bet_id = id;
        bet_count = c;
        award_count = w;
    }
}

// 牌
public class CardInfo
{
    public int flower;
    public int point;
}

//////////////////////////////////////////////////////////////////////////
public class DbCowsBet
{
    public string type;
    public string key;      // key可以找到具体牌型信息
    public long betgold0;   // 东 押注
    public long wingold0;   // 东 得奖

    public long betgold1;   // 南 押注
    public long wingold1;   // 南 得奖

    public long betgold2;    // 西 押注
    public long wingold2;    // 西 得奖

    public long betgold3;    // 北 押注
    public long wingold3;    // 北 得奖

    public double costrate;  // 上庄手续百分比
    public long costgold;    // 具体的手续费用

    public long sumBet()
    {
        return betgold0 + betgold1 + betgold2 + betgold3;
    }

    public long sumWin()
    {
        return wingold0 + wingold1 + wingold2 + wingold3;
    }
}

public class DbCowsCard
{
    public int cardsType;
    public int cardsValue;
    public int cards0;
    public int cards1;
    public int cards2;
    public int cards3;
    public int cards4;
}

public class CowsCard
{
    // 牌型  对应Cows_CardCFG.xls中的id
    public int m_cardType;

    public List<CardInfo> m_cards;

    public void addCard(DbCowsCard c)
    {
        if (c == null)
            return;

        m_cardType = c.cardsType;
        m_cards = new List<CardInfo>();

        CardInfo info = GameDetail.createCardInfo(c.cards0);
        m_cards.Add(info);

        info = GameDetail.createCardInfo(c.cards1);
        m_cards.Add(info);

        info = GameDetail.createCardInfo(c.cards2);
        m_cards.Add(info);

        info = GameDetail.createCardInfo(c.cards3);
        m_cards.Add(info);

        info = GameDetail.createCardInfo(c.cards4);
        m_cards.Add(info);
    }
}

// 牛牛信息
public class InfoCows
{
    // 押注信息
    public DbCowsBet m_betInfo;

    // 庄家牌型
    public CowsCard m_bankerCard;

    // 东牌型
    public CowsCard m_eastCard;
    // 南牌型
    public CowsCard m_southCard;
    // 西牌型
    public CowsCard m_westCard;
    // 北牌型
    public CowsCard m_northCard;

    public void createBankerCard(DbCowsCard c)
    {
        m_bankerCard = new CowsCard();
        m_bankerCard.addCard(c);
    }

    public void createEastCard(DbCowsCard c)
    {
        m_eastCard = new CowsCard();
        m_eastCard.addCard(c);
    }

    public void createSouthCard(DbCowsCard c)
    {
        m_southCard = new CowsCard();
        m_southCard.addCard(c);
    }

    public void createWestCard(DbCowsCard c)
    {
        m_westCard = new CowsCard();
        m_westCard.addCard(c);
    }

    public void createNorthCard(DbCowsCard c)
    {
        m_northCard = new CowsCard();
        m_northCard.addCard(c);
    }

    // 自己是否上庄
    public bool isBanker()
    {
        if (m_betInfo == null)
            return false;

        return m_betInfo.type == "1";
    }

    // 返回手续费百分比
    public double getServiceChargeRatio()
    {
        return m_betInfo.costrate * 100;
    }
}

//////////////////////////////////////////////////////////////////////////
public enum e_award_type_def
{
    e_type_normal = 0,        // 正常-转灯
    e_type_all_prizes = 1,    // 人人有奖
    e_type_spot_light = 2,    // 射灯
    e_type_handsel = 3,       // 彩金
    e_type_error = 4,
};

public class BetInfoCrocodile : BetInfo
{
    public int rate;
}

// 鳄鱼大亨信息
public class InfoCrocodile
{
    // 押注区域的相关信息, 其中的字段bet_id，从0开始，取表格数据时，需要加1
    public List<BetInfoCrocodile> m_betInfo;

    // 结果类型
    public List<DbCrocodileResultType> m_resultType;

    // 结果列表
    public List<DbCrocodileResultList> m_resultList;

    public e_award_type_def getResultType()
    {
        if (m_resultType == null ||
            m_resultType.Count == 0)
            return e_award_type_def.e_type_error;

        DbCrocodileResultType t = m_resultType[0];
        if (t.type == "all_prizes")
        {
            return e_award_type_def.e_type_all_prizes;
        }
        if (t.type == "handsel")
        {
            return e_award_type_def.e_type_handsel;
        }
        if (t.type == "spot_light")
        {
            return e_award_type_def.e_type_spot_light;
        }

        return e_award_type_def.e_type_normal;
    }

    public string getResultParam()
    {
        DbCrocodileResultType t = m_resultType[0];
        return t.param;
    }

    // 返回彩金时的结果ID
    public string getHandSelResultId()
    {
        if (m_resultList.Count == 0)
            return "";

        return m_resultList[0].result_id;
    }

    // 从1开始的betId
    public BetInfoCrocodile getBetInfo(int betId)
    {
        betId--;
        foreach (var item in m_betInfo)
        {
            if (item.bet_id == betId)
                return item;
        }

        return null;
    }
}

// 鳄鱼结果类型
public class DbCrocodileResultType
{
    public string type;
    public string param;
}

public class DbCrocodileResultList
{
    // 结果区域ID，从0开始计数，取值后，需要加1
    public string result_id;
}

//////////////////////////////////////////////////////////////////////////
// 骰宝下注
public class DbDiceBet
{
    public int type;
    public int bet_gold;
    public int win_gold;
}

// 骰宝信息
public class InfoDice
{
    // 3个骰子面值
    public int[] m_diceNum = new int[3];

    // 下注信息
    public List<DbDiceBet> m_betInfo;

    // 将num进行分解
    public void setDiceNum(int num)
    {
        string str = num.ToString();
        for (int i = 0; i < str.Length; i++)
        {
            m_diceNum[i] = Convert.ToInt32(str[i]) - 0x30;
        }
    }

    // 返回结果 0大 1小 2 豹子
    public int getResult()
    {
        return getResult(m_diceNum[0], m_diceNum[1], m_diceNum[2]);
    }

    public DbDiceBet getDiceBet(int type)
    {
        foreach (var d in m_betInfo)
        {
            if (d.type == type)
                return d;
        }
        return null;
    }

    // 返回结果 0大 1小 2 豹子
    public static int getResult(int dice0, int dice1, int dice2)
    {
        if (dice0 == dice1 &&
            dice1 == dice2)
            return 2;

        int sum = dice0 + dice1 + dice2;
        if (sum >= 4 && sum <= 10)
            return 1;

        return 0;
    }
}

//////////////////////////////////////////////////////////////////////////
public class DbFish
{
    public int fishid;
    public int hitcount;
    public int deadcount;
    public int totalincome;
    public int totaloutlay;
}

// 鳄鱼公园信息
public class InfoFishPark
{
    // 废弹数量
    public int m_abandonedbullets;
    public List<DbFish> m_fish;

    public void sort()
    {
        m_fish.Sort(sortFish);
    }

    private int sortFish(DbFish p1, DbFish p2)
    {
        return p1.fishid - p2.fishid;
    }
}

//////////////////////////////////////////////////////////////////////////
// 黑红梅方信息
public class InfoShcd
{
    public int card_type;
    public int card_value;
    public List<BetInfoCrocodile> betinfo;

    public BetInfoCrocodile getAreaBet(int id)
    {
        for (int i = 0; i < betinfo.Count; i++)
        {
            if (id == betinfo[i].bet_id)
                return betinfo[i];
        }
        return null;
    }
}














