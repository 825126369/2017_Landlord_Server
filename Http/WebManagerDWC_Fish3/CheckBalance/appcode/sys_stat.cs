using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

//////////////////////////////////////////////////////////////////////////
// 总收支表
class StatPlayerTotalIncomeExpenses 
{
    string[] FIELDS = { "oldValue", "newValue", "genTime", "reason" };
    int INIT_VALUE = -99999;

    int m_startValue;
    int m_endValue;
    DateTime m_errorTime;

    Dictionary<int, string> m_items = new Dictionary<int, string>();
    public StatPlayerTotalIncomeExpenses()
    {
        m_items.Add(1, "金币");
        m_items.Add(2, "钻石");
        m_items.Add(11, "话费碎片");
        m_items.Add(14, "龙珠");
    }

    public void check(ParamCheck param)
    {
        m_startValue = m_endValue = INIT_VALUE;

        IMongoQuery imq1 = Query.LT("genTime", BsonValue.Create(param.m_endTime));
        IMongoQuery imq2 = Query.GTE("genTime", BsonValue.Create(param.m_startTime));
        IMongoQuery imq3 = Query.EQ("playerId", param.m_playerId);
        IMongoQuery imq4 = Query.EQ("itemId", param.m_itemId);
        IMongoQuery imq5 = Query.EQ("gameId", param.m_gameId);
        IMongoQuery imq = Query.And(imq1, imq2, imq3, imq4, imq5);

        int count = (int)MongodbLog.Instance.ExecuteGetCount(TableName.PUMP_PLAYER_MONEY, imq);
        param.m_from.begin(count);

        int skip = 0;
        List<Dictionary<string, object>> dataList = null;
        bool run = true;
        bool error = false;

        while (run)
        {
            dataList = QueryTool.nextData(MongodbLog.Instance,
                                      TableName.PUMP_PLAYER_MONEY,
                                      imq,
                                      ref skip,
                                      1000,
                                      FIELDS, "genTime");
            if (dataList == null)
                break;

            for (int i = 0; i < dataList.Count; i++)
            {
                Dictionary<string, object> data = dataList[i];
                if (param.m_gameId == 1 && Convert.ToInt32(data["reason"]) != 11)
                    continue;

                if (m_startValue == INIT_VALUE && m_endValue == INIT_VALUE)
                {
                    m_startValue = Convert.ToInt32(data["oldValue"]);
                    m_endValue = Convert.ToInt32(data["newValue"]);
                    m_errorTime = Convert.ToDateTime(data["genTime"]).ToLocalTime();
                }
                else
                {
                    int start = Convert.ToInt32(data["oldValue"]);
                    int end = Convert.ToInt32(data["newValue"]);

                    if (m_endValue != start)
                    {
                        error = true;
                        run = false;
                        m_errorTime = Convert.ToDateTime(data["genTime"]).ToLocalTime();
                        break;
                    }
                    else
                    {
                        m_startValue = start;
                        m_endValue = end;
                    }
                }

                param.m_from.finishOne();
            }
        }

        output(error, param);

        param.m_from.done();
    }

    void output(bool error, ParamCheck param)
    {
        string str = "";
        if (error)
        {
            str = "检测 不 通过";

            LogMgr.log.InfoFormat("检测完毕:游戏时间[{0}], 玩家[{1}]在游戏[{2}],道具[{3}]，结果[{4}], 不通过时间点[{5}]",
                            param.m_startTime.ToString(), 
                            param.m_playerId, 
                            StrName.s_gameName[param.m_gameId],
                            m_items[param.m_itemId],
                            str,
                            m_errorTime.ToString());
        }
        else
        {
            str = "检测通过";

            LogMgr.log.InfoFormat("检测完毕:游戏时间[{0}],玩家[{1}]在游戏[{2}]，道具[{3}]，结果[{4}]",
                            param.m_startTime.ToString(),
                            param.m_playerId,
                            StrName.s_gameName[param.m_gameId],
                            m_items[param.m_itemId],
                            str);
        }
    }
}
