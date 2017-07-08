using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class StatDragonItem
{
    public int m_playerId;       // 玩家ID

    public long m_dbgain;        // 打到龙珠
    public long m_dbsend;        // 送出龙珠 
    public long m_dbaccept;      // 收取龙珠
    public long m_dbexchange;    // 龙珠兑换
    public long m_dbRemain;      // 剩余龙珠
    public long m_dbStart;       //初始龙珠

    public long m_goldByRecharge; // 充值获得的金币
    public long m_goldByOther;    // 其他途径获得的金币
    public long m_goldConsume;       // 金币花费累计
    public long m_goldRemain;     // 金币结余
    public long m_goldStart;      // 初始金币

    public long m_gemByRecharge;  // 钻石充值获得
    public long m_gemByOther;     // 钻石其余途径获得
    public long m_gemConsume;     // 钻石消耗
    public long m_gemRemain;      // 钻石剩余
    public long m_gemStart;       // 初始钻石

    public int m_todayRecharge;   // 今日累计充值
}

public class StatDragonDailyItem
{
    public DateTime m_time;          // 日期
    public long m_todayRecharge;     // 每日充值
    public long m_dragonBallGen;     // 龙珠产出
    public long m_dragonBallConsume; // 龙珠消耗，兑换
    public long m_dragonBallRemain;  // 每日龙珠结余
}

public class RechargePlayerMonitorItemBase
{
    public int m_playerId;
    public int m_curFishLevel;           // 当前捕鱼等级
    public int m_totalRecharge;          // 累计付费
    public int m_totalGameTime;          // 累计游戏时间

    public DateTime m_firstRechargeTime;  // 初次付费日期
    public int m_firstRechargeGameTime;   // 初次付费游戏时间
    public int m_firstRechargePoint;      // 初次付费点
    public int m_firstRechargeGold;       // 初次付费时拥有金币
    public int m_firstRechargeFishLevel;  // 初次付费捕鱼等级

    public DateTime m_secondRechargeTime;  // 再次付费日期
    public int m_secondRechargeGameTime;   // 再次付费游戏时间
    public int m_secondRechargePoint;      // 再次付费点
    public int m_secondRechargeGold;       // 再次付费时拥有金币
    public int m_secondRechargeFishLevel;  // 再次付费捕鱼等级

    public DateTime m_regTime;             // 注册日期
    public int m_remainDragon;             // 龙珠结余
    public long m_gainDragon;              // 累计获得龙珠
    public long m_sendDragon;              // 累计转出龙珠

    public string getGameTime(int gameTime)
    {
        return Tool.getTimeStr(gameTime);
    }
}

// 玩家总收支表
public class StatIncomeExpensesItemBase
{
    public int m_playerId;          // 玩家ID

    public long m_goldFreeGain;     // 金币免费获得
    public long m_goldRechargeGain; // 金币充值获得 
    public long m_goldConsume;      // 金币消耗
    public long m_goldRemain;       // 剩余金币
    public long m_goldStart;        // 初始金币

    public long m_gemFreeGain;      // 钻石免费获得
    public long m_gemRechargeGain;  // 钻石充值获得
    public long m_gemConsume;       // 钻石消耗
    public long m_gemRemain;        // 钻石剩余
    public long m_gemStart;         // 初始钻石

    public long m_dbFreeGain;       // 龙珠获得
    public long m_dbConsume;        // 龙珠消耗
    public long m_dbRemain;         // 剩余龙珠
    public long m_dbStart;          // 初始龙珠

    public long m_chipFreeGain;     // 话费碎片获得
    public long m_chipConsume;      // 话费碎片消耗
    public long m_chipRemain;       // 剩余话费碎片
    public long m_chipStart;        // 初始话费碎片
}

// 活跃行为--用户喜好
public class GameTimeForPlayerFavorBase
{
    public DateTime m_time;

    public int m_playerCount;   // 统计的玩家数量

    // 活跃用户在各游戏内的游戏总时间
    public Dictionary<int, long> m_activePlayer = new Dictionary<int, long>();

    // 付费用户在各游戏内的游戏总时间
    public Dictionary<int, long> m_rechargePlayer = new Dictionary<int, long>();

    public void reset()
    {
        m_activePlayer.Clear();
        m_rechargePlayer.Clear();
    }

    public void addGameTime(int playerType, int gameId, long time)
    {
        switch (playerType)
        {
            case 1: // 活跃用户
                {
                    m_activePlayer[gameId] = time;
                }
                break;
            case 2: // 付费用户
                {
                    m_rechargePlayer[gameId] = time;
                }
                break;
        }
    }

    public Dictionary<int, long> getGameTime(int playerType)
    {
        Dictionary<int, long> ret = null;
        switch (playerType)
        {
            case 1: // 活跃用户
                {
                    ret = m_activePlayer;
                }
                break;
            case 2: // 付费用户
                {
                    ret = m_rechargePlayer;
                }
                break;
        }
        return ret;
    }
}

// 活跃行为--平均游戏时长分布
public class GameTimeForDistributionBase
{
    public DateTime m_time;
    public int m_gameId;
    public int m_Less10s;
    public int m_Less30s;
    public int m_Less60s;
    public int m_Less5min;
    public int m_Less10min;
    public int m_Less30min;
    public int m_Less60min;
    public int m_GT60min;
}

// 首付游戏时长分布
public class FirstRechargeGameTimeDistributionBase
{
    public int m_Less1min;
    public int m_Less10min;
    public int m_Less30min;
    public int m_Less60min;
    public int m_Less3h;
    public int m_Less5h;
    public int m_Less12h;
    public int m_Less24h;
    public int m_GT24h;
}

// 玩家首付计费点分布
public class FirstRechargePointDistribution
{
    // 计费点-->玩家数
    public Dictionary<int, int> m_point = new Dictionary<int, int>();

    public void add(int payPoint, int count)
    {
        if (m_point.ContainsKey(payPoint))
        {
            m_point[payPoint] += count;
        }
        else
        {
            m_point[payPoint] = count;
        }
    }

    public void reset()
    {
        m_point.Clear();
    }
}

//////////////////////////////////////////////////////////////////////////
public class ItemBasePlayerGameBet
{
    public double m_sum;
    public long m_max;
    public long m_min;
}

public class TInfo<T1, T2>
{
    public T1 m_first;
    public T2 m_second;
}



