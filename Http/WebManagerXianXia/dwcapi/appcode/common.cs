using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using System.IO;

public class Helper
{
    // 上分，下分
    public static void doScore(HttpRequest Request, HttpResponse Response, int op) 
    {
        ParamScore param = new ParamScore();
        param.m_gmAccount = Request.QueryString["gmAcc"];
        param.m_gmPwd = Request.QueryString["gmPwd"];
        param.m_playerAcc = Request.QueryString["playerAcc"];
        param.m_score = Request.QueryString["score"];
        param.m_op = op;
        param.m_userOrderId = Request.QueryString["userOrderId"];
        param.m_apiCallBack = Request.QueryString["apiCallBack"];
        param.m_sign = Request.QueryString["sign"];

        if (!param.isParamValid())
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add("result", RetCode.RET_PARAM_NOT_VALID);
            Response.Write(Helper.genJsonStr(data));
            return;
        }

        DyOpScore dy = new DyOpScore();
        string retStr = dy.doDyop(param);

        Response.Write(retStr);
    }

    // 生成一个json串
    public static string genJsonStr(Dictionary<string, object> data, bool changeTimeToLocal = false)
    {
        StringWriter sw = new StringWriter();
        JsonWriter writer = new JsonTextWriter(sw);
        genJsonStr(data, sw, writer, changeTimeToLocal);
        writer.Flush();
        return sw.GetStringBuilder().ToString();
    }

    private static void genJsonStr(Dictionary<string, object> data, StringWriter sw, JsonWriter writer, bool changeTimeToLocal)
    {
        writer.WriteStartObject();
        foreach (var item in data)
        {
            writer.WritePropertyName(item.Key);

            if (item.Value is List<Dictionary<string, object>>)
            {
                writer.WriteStartArray();
                List<Dictionary<string, object>> dataList = (List<Dictionary<string, object>>)item.Value;
                for (int i = 0; i < dataList.Count; i++)
                {
                    genJsonStr(dataList[i], sw, writer, changeTimeToLocal);
                }
                writer.WriteEndArray();
            }
            else if (item.Value is Dictionary<string, object>)
            {
                genJsonStr((Dictionary<string, object>)item.Value, sw, writer, changeTimeToLocal);
            }
            else if (item.Value is DateTime)
            {
                var v = (DateTime)item.Value;
                if (changeTimeToLocal)
                {
                    v = v.ToLocalTime();
                }
                writer.WriteValue(v.ToString(ConstDef.DATE_TIME24));
            }
            else
            {
                writer.WriteValue(item.Value);
            }
        }
        writer.WriteEndObject();        
    }

    // 返回账号acc的余额
    public static long getRemainMoney(string acc, GMUser user)
    {
        string sql = "";
        SqlSelectGenerator gen = new SqlSelectGenerator();
        gen.addField("money");
        sql = gen.getResultSql(TableName.PLAYER_ACCOUNT_XIANXIA,
                                        string.Format("acc='{0}'", acc));

        Dictionary<string, object> data = user.sqlDb.queryOne(sql, MySqlDbName.DB_XIANXIA);
        if (data == null)
            return 0;

        return Convert.ToInt64(data["money"]);
    }

    // 通过账号返回玩家属性
    public static Dictionary<string, object> getPlayerPropertyByAcc(string acc, string[] fields)
    {
        Dictionary<string, object> ret =
                        MongodbPlayer.Instance.ExecuteGetOneBykey(TableName.PLAYER_INFO, "account", acc, fields);
        return ret;
    }

    // 存入货币到数据库的转换
    public static long saveMoneyValue(long curVal)
    {
        return curVal * CONST.MONEY_BASE;
    }

    // 显示货币的转换
    public static double showMoneyValue(double curVal)
    {
        return curVal / CONST.MONEY_BASE;
    }
}
