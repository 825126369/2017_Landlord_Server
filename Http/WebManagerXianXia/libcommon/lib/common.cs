using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System.Collections.Generic;
using System.Web;
using System.Security.Cryptography;
using System.Net;
using System.Collections.Specialized;

public struct Exp
{
    public const string DATE_TIME = @"^\s*(\d{4})/(\d{1,2})/(\d{1,2})\s*$";
    public const string DATE_TIME1 = @"^\s*(\d{4})/(\d{1,2})/(\d{1,2})\s+(\d{1,2})\s*$";
    public const string DATE_TIME2 = @"^\s*(\d{4})/(\d{1,2})/(\d{1,2})\s+(\d{1,2}):(\d{1,2})\s*$";
    // 以空格相隔的两个数字
    public const string TWO_NUM_BY_SPACE = @"^\s*(-?\d+)\s+(-?\d+)\s*$";
    // 以空格相隔的两个数字
    public const string TWO_NUM_BY_SPACE_SEQ = @"^(\s*(\d+)\s+(\d+)\s*)(;\s*(\d+)\s+(\d+)\s*)+$";
    // 匹配IP地址
    public const string IP_ADDRESS = @"\s*(\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3})\s*$";
    public const string IP_ADDRESS1 = @"\s*(\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}:\d{1,5})\s*$";
    public const string PWD_RULE = @"^[a-zA-Z0-9]{6,15}$";
    // 匹配多个客服段
    public const string SERVICE_HELP_M = @"^([1-4],[^#,]+,[^#,]+)(#[1-4],[^#,]+,[^#,]+)+$";
    // 匹配一个客服段
    public const string SERVICE_HELP1 = @"^([1-4],[^#,]+,[^#,]+)$";
    public const string NUM_LIST_BY_BLANK = @"^\s*(-{0,1}\d+)(\s+-{0,1}\d+)+\s*$";

    public const string SINGLE_NUM = @"^\s*\d+\s*$";

    public const string NUM_LIST_BY_COMMA = @"^\s*(-{0,1}\d+)(\s*,\s*-{0,1}\d+)+\s*$";

    // 玩家账号匹配
    public const string ACCOUNT_PLAYER = @"^[0-9a-zA-Z]{6,30}$";

    // 玩家的密码
    public const string ACCOUNT_PLAYER_PWD = @"^[\S]{6,20}$";

    // 经销商的匹配
    public const string ACCOUNT_DEALER = @"^[a-zA-Z]{4,8}$";

    // 4位固定验证码
    public const string VER_CODE_FOUR = @"^[0-9]{4,4}$";

    // GM账号密码匹配
    public const string GM_ACCOUNT_PWD = @"^.{6,16}$";

    // 下级代理，3～10个英数
    public const string ACCOUNT_AGENCY = @"^[a-zA-Z0-9]{3,10}$";

    // API账号
    public const string ACCOUNT_API = @"^[a-zA-Z0-9]{3,10}$";

    // API前缀
    public const string ACCOUNT_API_PREFIX = @"^[a-zA-Z0-9]{2,10}$";

    // 用户自定义订单ID
    public const string USER_ORDER_ID = @"^[0-9a-zA-Z]{0,200}$";

    // 检测登录用
    public const string CHECK_LOGIN = @"^[0-9a-zA-Z#]{0,50}$";
}

public class URLParam
{
    public string m_url = "";
    public string m_text = "";
    public string m_key = "";
    public string m_value = "";
    public string m_className = "";

    public string m_target;

    // 额外的url参数
    public Dictionary<string, object> m_exUrlParam = null;

    public void clearExParam()
    {
        if (m_exUrlParam != null)
        {
            m_exUrlParam.Clear();
        }
    }

    public void addExParam(string key, object v)
    {
        if (m_exUrlParam == null)
        {
            m_exUrlParam = new Dictionary<string, object>();
        }
        m_exUrlParam.Add(key, v);
    }
}

public class Tool
{
    // 每天多少秒
    public const long SECONDS_EACH_DAY = 24 * 3600;
    // 每天包含的百纳秒数
    public const long TICKS_EACH_DAY = SECONDS_EACH_DAY * 10000000;

    // 生成时间
    public static long genTime(Match min, bool isaddoneday)
    {
        GroupCollection c = min.Groups;
        int y = Convert.ToInt32(c[1].Value);
        int m = Convert.ToInt32(c[2].Value);
        int d = Convert.ToInt32(c[3].Value);
        DateTime t = new DateTime(y, m, d);
        if (isaddoneday)
        {
            t = t.AddDays(1);
        }
        return t.Ticks;
    }

    private static bool genTime(Match match, bool isadd, int date_type, ref DateTime resTime)
    {
        GroupCollection c = match.Groups;
        int y = Convert.ToInt32(c[1].Value);
        if (y > 9999)
            return false;

        int m = Convert.ToInt32(c[2].Value);
        if (m < 1 || m > 12)
            return false;

        int d = Convert.ToInt32(c[3].Value);
        switch (date_type)
        {
            case 1:  // 日期
                {
                    DateTime t = new DateTime(y, m, d);
                    if (isadd)
                    {
                        t = t.AddDays(1);
                    }
                    resTime = t;
                    return true;
                }
            case 2: // 日期 时
                {
                    int hh = Convert.ToInt32(c[4].Value);
                    if (hh >= 0 && hh <= 23)
                    {
                        DateTime t = new DateTime(y, m, d, hh, 0, 0);
                        if (isadd)
                        {
                            t = t.AddHours(1);
                        }
                        resTime = t;
                        return true;
                    }
                }
                break;
            case 3: // 日期 时 分
                {
                    int hh = Convert.ToInt32(c[4].Value);
                    int mm = Convert.ToInt32(c[5].Value);
                    if (hh >= 0 && hh <= 23 && mm >= 0 && mm <= 59)
                    {
                        DateTime t = new DateTime(y, m, d, hh, mm, 0);
                        if (isadd)
                        {
                            t = t.AddMinutes(1);
                        }
                        resTime = t;
                        return true;
                    }
                }
                break;
        }

        return false;
    }

    // 构造一个超链接
    public static string genHyperlink(URLParam[] param)
    {
        StringBuilder textBuilder = new StringBuilder();

        if (param == null)
            return "";

        foreach (URLParam p in param)
        {
            if (p.m_url != "")
            {
                textBuilder.Append("<a ");

                textBuilder.Append(" style=");
                textBuilder.Append("\"");
                textBuilder.Append("border:1px solid black;");
                textBuilder.Append("padding : 5px 5px 5px 5px;");
                textBuilder.Append("text-decoration : none;");
                textBuilder.Append("font-size:medium;");
                textBuilder.Append("\"");

                textBuilder.Append(" href=");
                textBuilder.Append("\"");
                textBuilder.Append(p.m_url);

                textBuilder.Append("?");
                textBuilder.Append(p.m_key);
                textBuilder.Append("=");
                textBuilder.Append(HttpUtility.UrlEncode(p.m_value));

                // 添加额外的url参数
                if (p.m_exUrlParam != null)
                {
                    foreach (var urlp in p.m_exUrlParam)
                    {
                        textBuilder.Append('&');
                        textBuilder.Append(urlp.Key);
                        textBuilder.Append("=");
                        textBuilder.Append(HttpUtility.UrlEncode(Convert.ToString(urlp.Value)));
                    }
                }
                textBuilder.Append("\"");

                textBuilder.Append(">");
                textBuilder.Append(p.m_text);
                textBuilder.Append("</a>");
            }
            else
            {
                textBuilder.Append(p.m_text);
            }

            textBuilder.Append("  ");
        }
        return textBuilder.ToString();
    }

    public static string genHyperlink(URLParam param)
    {
        if (param == null)
            return "";

        StringBuilder textBuilder = new StringBuilder();

        textBuilder.Append("<a ");
        textBuilder.Append("href=");
        textBuilder.Append(param.m_url);

        if (!string.IsNullOrEmpty(param.m_key))
        {
            textBuilder.Append("?");
            textBuilder.Append(param.m_key);
            textBuilder.Append("=");
            textBuilder.Append(HttpUtility.UrlEncode(param.m_value));

            if (param.m_exUrlParam != null)
            {
                foreach (var urlp in param.m_exUrlParam)
                {
                    textBuilder.Append('&');
                    textBuilder.Append(urlp.Key);
                    textBuilder.Append("=");
                    textBuilder.Append(HttpUtility.UrlEncode(Convert.ToString(urlp.Value)));
                }
            }
        }

        textBuilder.Append(" class=");
        textBuilder.Append("\"" + param.m_className + "\"");
        if (!string.IsNullOrEmpty(param.m_target))
        {
            textBuilder.Append(" target=");
            textBuilder.Append("\"" + param.m_target + "\"");
        }
        textBuilder.Append(">");
        textBuilder.Append(param.m_text);
        textBuilder.Append("</a>");
        return textBuilder.ToString();
    }

    // 将时间串cur_time分隔，分隔成两个整数时间值，返回true表示成功
    public static bool splitTimeStr(string cur_time, ref DateTime minTime, ref DateTime maxTime)
    {
        // 首先分隔时间串
        string[] str = Tool.split(cur_time, '-');
        if (str.Length == 2)
        {
            Match m1 = null, m2 = null;
            int type1 = parseTimeStr(str[0], ref m1);
            if (type1 == 0)
                return false;
            int type2 = parseTimeStr(str[1], ref m2);
            if (type2 == 0)
                return false;

            bool res = Tool.genTime(m1, false, type1, ref minTime);
            if (!res)
                return false;

            res = Tool.genTime(m2, true, type2, ref maxTime);
            if (!res)
                return false;

            if (minTime >= maxTime)
            {
                return false;
            }
        }
        else if (str.Length == 1)
        {
            Match m1 = null;
            int type1 = parseTimeStr(str[0], ref m1);
            if (type1 == 0)
                return false;

            bool res = Tool.genTime(m1, false, type1, ref minTime);
            if (!res)
                return false;

            res = Tool.genTime(m1, true, type1, ref maxTime);
            if (!res)
                return false;
        }
        else
        {
            return false;
        }
        return true;
    }

    // 年月日
    public const int TIME_FORMAT_YMD = 1;
    // 年月日 时
    public const int TIME_FORMAT_YMD_H = 2;
    // 年月日 时:分
    public const int TIME_FORMAT_YMD_HM = 3;

    // 解析一个时间点，返回true成功
    // format为0不限格式，
    public static bool splitTimeStr(string curTime, ref DateTime resultTime, int format = 0)
    {
        Match m1 = null;
        int type1 = parseTimeStr(curTime, ref m1);
        if (type1 == 0)
            return false;

        if (format > 0)
        {
            if (type1 != format)
                return false;
        }

        bool res = genTime(m1, false, type1, ref resultTime);
        if (!res)
            return false;

        return true;
    }

    // 返回复选框
    public static string getChecked(bool issel)
    {
        return issel ? "checked=\"true\"" : "";
    }

    // 返回一个checkbox html串，issel表示是否选中
    public static string getCheckBoxHtml(string name, string value, bool issel, string text = "")
    {
        string str = "<input type= \"checkbox\" name=" + "\"" + name + "\"" + getChecked(issel) + " value= " + "\"" + value + "\"" + " runat=\"server\" />" + text;
        return str;
    }

    // 返回一个checkbox html串，issel表示是否选中
    public static string getRadioHtml(string name, string value, bool issel, string text = "")
    {
        string str = "<input type= \"radio\" name=" + "\"" + name + "\"" + getChecked(issel) + " value= " + "\"" + value + "\"" + " runat=\"server\" />" + text;
        return str;
    }

    public static string getTextBoxHtml(string id, string value)
    {
        string str = "<input type= \"text\" id=" + "\"" + id + "\"" + " value=" + "\"" + value + "\"" + " />";
        return str;
    }

    // 根据符号ch对串str进行拆分
    public static string[] split(string str, char ch, StringSplitOptions op = StringSplitOptions.None)
    {
        char[] sp = { ch };
        string[] arr = str.Split(sp, op);
        return arr;
    }

    private static int parseTimeStr(string time_str, ref Match m)
    {
        m = Regex.Match(time_str, Exp.DATE_TIME);
        if (!m.Success)
        {
            m = Regex.Match(time_str, Exp.DATE_TIME1);
            if (!m.Success)
            {
                m = Regex.Match(time_str, Exp.DATE_TIME2);
                if (m.Success)
                {
                    return 3;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return 2;
            }
        }
        return 1;
    }

    /*
     *  解析Item串列表，返回true成功
     *  item_str 可以为空，此时返回true，但out_items保持不变
     */
    public static bool parseItemList(string itemStr, List<ParamItem> out_items, bool isEmptyStrValid = true)
    {
        if (!isItemListValid(itemStr, isEmptyStrValid))
            return false;

        parseItem(itemStr, out_items);
        return true;
    }

    /**
     *  解析由空格相隔的数字串，存于结果outList中， 成功返回true
     */
    public static bool parseNumList(string numStr, List<int> outList = null)
    {
        Match m = Regex.Match(numStr, Exp.NUM_LIST_BY_BLANK);
        if (!m.Success)
        {
            m = Regex.Match(numStr, Exp.SINGLE_NUM);
            if (!m.Success)
                return false;

            if (outList != null)
            {
                outList.Add(Convert.ToInt32(numStr));
            }
            return true;
        }

        if (outList != null)
        {
            string[] arr = Tool.split(numStr, ' ', StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < arr.Length; i++)
            {
                int id = Convert.ToInt32(arr[i]);
                outList.Add(id);
            }
        }
        
        return true;
    }

    /**
     *  解析由逗号相隔的数字串，存于结果outList中， 成功返回true
     */
    public static bool parseNumListByComma(string numStr, List<int> outList = null)
    {
        Match m = Regex.Match(numStr, Exp.NUM_LIST_BY_COMMA);
        if (!m.Success)
        {
            m = Regex.Match(numStr, Exp.SINGLE_NUM);
            if (!m.Success)
                return false;

            if (outList != null)
            {
                outList.Add(Convert.ToInt32(numStr));
            }
            return true;
        }

        if (outList != null)
        {
            string[] arr = Tool.split(numStr, ',', StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < arr.Length; i++)
            {
                int id = Convert.ToInt32(arr[i]);
                outList.Add(id);
            }
        }

        return true;
    }

    /* 
     * 道具串列表是否合法
     * isEmptyStrValid 空串是否合法
     */
    public static bool isItemListValid(string itemStr, bool isEmptyStrValid = true)
    {
        if (itemStr == "" && isEmptyStrValid)
            return true;

        Match match = Regex.Match(itemStr, Exp.TWO_NUM_BY_SPACE);
        if (!match.Success)
        {
            match = Regex.Match(itemStr, Exp.TWO_NUM_BY_SPACE_SEQ);
            if (!match.Success)
            {
                return false;
            }
        }
        return true;
    }

    // 判定两个数字是否合法，以空格相隔
    public static bool isTwoNumValid(string itemStr, bool isEmptyStrValid = false)
    {
        if (itemStr == "" && isEmptyStrValid)
            return true;

        Match match = Regex.Match(itemStr, Exp.TWO_NUM_BY_SPACE);
        if (!match.Success)
        {
            return false;
        }
        return true;
    }

    // 分隔串，没考虑合法性
    public static void parseItem(string str, List<ParamItem> result)
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

    // 从字典内解析道具列表
    public static bool parseItemFromDic(Dictionary<string, object> dic, List<ParamItem> result)
    {
        if (result == null || dic == null) 
            return false;

        int i = 0;
        for (; i < dic.Count; i++)
        {
            Dictionary<string, object> tmp = dic[i.ToString()] as Dictionary<string, object>;
            ParamItem item = new ParamItem();
            item.m_itemId = Convert.ToInt32(tmp["itemId"]);
            item.m_itemCount = Convert.ToInt32(tmp["itemCount"]);
            result.Add(item);
        }
        return true;
    }

    // 返回时间串
    public static string getTimeStr(int seconds)
    {
        TimeSpan span = new TimeSpan(0, 0, seconds);
        if (span.Days > 0)
        {
            return string.Format("[{0}]天[{1}]小时[{2}]分[{3}]秒", span.Days, span.Hours, span.Minutes, span.Seconds);
        }
        if (span.Hours > 0)
        {
            return string.Format("[{0}]小时[{1}]分[{2}]秒", span.Hours, span.Minutes, span.Seconds);
        }
        return string.Format("[{0}]分[{1}]秒", span.Minutes, span.Seconds);
    }

    // MD5加密
    public static string getMD5Hash(String input)
    {
        MD5 md5 = new MD5CryptoServiceProvider();
        byte[] res = md5.ComputeHash(Encoding.Default.GetBytes(input), 0, input.Length);
        return BitConverter.ToString(res).Replace("-", "");
    }

    // 从字符串中取出售货亭编号
    public static string getSellerNum(string acc)
    {
        StringBuilder b = new StringBuilder();
        for (int i = acc.Length - 1; i >= 0; i--)
        {
            if (char.IsDigit(acc[i]))
            {
                b.Insert(0, acc[i]);
            }
            else
            {
                break;
            }
        }
        return b.ToString();
    }

    // 显示货币的转换
    public static double showMoneyValue(double curVal, int moneyBase)
    {
        return curVal / moneyBase;
    }
}

public class ParamItem
{
    public int m_itemId;
    public int m_itemCount;
}

///////////////////////////////////////////////////////////////////////////////

public static class BaseJsonSerializer
{
    public static string serialize(object base_object)
    {
        JsonSerializer serializer = new JsonSerializer();
        StringWriter sw = new StringWriter();
        serializer.Serialize(new JsonTextWriter(sw), base_object);
        return sw.ToString();
    }

    public static T deserialize<T>(string json_string)
    {
        JsonReader reader = new JsonTextReader(new StringReader(json_string));
        JsonSerializer serializer = new JsonSerializer();
        return serializer.Deserialize<T>(reader);
    }

    // 生成一个json串
    public static string genJsonStr(Dictionary<string, object> data)
    {
        if (data == null)
            return "{}";

        StringWriter sw = new StringWriter();
        JsonWriter writer = new JsonTextWriter(sw);
        genJsonStr(data, sw, writer);
        writer.Flush();
        return sw.GetStringBuilder().ToString();
    }

    private static void genJsonStr(Dictionary<string, object> data, StringWriter sw, JsonWriter writer)
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
                    genJsonStr(dataList[i], sw, writer);
                }
                writer.WriteEndArray();
            }
            else if (item.Value is Dictionary<string, object>)
            {
                genJsonStr((Dictionary<string, object>)item.Value, sw, writer);
            }
            else
            {
                writer.WriteValue(item.Value);
            }
        }
        writer.WriteEndObject();
    }
}

//////////////////////////////////////////////////////////////////////////

public enum FieldType { TypeNumber, TypeString };

public class DbField
{
    public string m_key;
    public FieldType m_fieldType;  // 0数值型 1字符串型

    public DbField(string key, FieldType fieldType)
    {
        m_key = key;
        m_fieldType = fieldType;
    }
}

public class DbFieldSet
{
    private Dictionary<string, DbField> m_items = new Dictionary<string, DbField>();
    private static DbFieldSet s_obj = null;

    public static DbFieldSet getInstance()
    {
        if (s_obj == null)
        {
            s_obj = new DbFieldSet();
            s_obj.init();
        }
        return s_obj;
    }

    public bool hasField(string key)
    {
        return m_items.ContainsKey(key);
    }

    public DbField getDbField(string key)
    {
        return m_items[key];
    }

    // 生成更新语句
    public string genUpdateSql(string key, object val)
    {
        if (!m_items.ContainsKey(key))
            return "";

        DbField f = m_items[key];
        switch (f.m_fieldType)
        {
            case FieldType.TypeNumber:
                {
                    return string.Format(" {0}={1} ", key, val);
                }
                break;
            case FieldType.TypeString:
                {
                    return string.Format(" {0}='{1}' ", key, val);
                }
                break;
        }

        return "";
    }

    private void init()
    {
        DbField field = new DbField("name", FieldType.TypeString);
        m_items.Add(field.m_key, field);

        field = new DbField("sex", FieldType.TypeNumber);
        m_items.Add(field.m_key, field);

        field = new DbField("city", FieldType.TypeString);
        m_items.Add(field.m_key, field);

        field = new DbField("mobilePhone", FieldType.TypeString);
        m_items.Add(field.m_key, field);

        field = new DbField("selfComment", FieldType.TypeString);
        m_items.Add(field.m_key, field);

        field = new DbField("birthDay", FieldType.TypeString);
        m_items.Add(field.m_key, field);
    }
}

///////////////////////////////////////////////////////////////////////////////
public struct RIGHT
{
    // 创建代理
    public const string CREATE_AGENCY = "createAgency";

    // 创建API
    public const string CREATE_API = "createAPI";

    // 审批API
    public const string APPROVE_API = "0";

    // 数据统计
    public const string DATA_STAT = "stat";

    // 删除账号
    public const string DEL_ACCOUNT = "1";

    // 上分下分
    public const string SCORE = "2";

    // 创建子账号
    public const string CREATE_SUB_ACC = "3";

    // 分级查看代理
    public const string VIEW_AGENCY = "4";

    public const string OPERATION = "operation";
}

public class RightItem
{
    public string m_rightName;

    public RightItem(string rightName)
    {
        m_rightName = rightName;
    }
}

// 可分配的权限
public class DispatchRight
{
    private Dictionary<string, RightItem> m_all = new Dictionary<string, RightItem>();

    public void addRight(string rightKey, RightItem item)
    {
        m_all.Add(rightKey, item);
    }

    public Dictionary<string, RightItem> allRight { get { return m_all; } }
}

public class RightMap
{
    public static string[] s_right = { "createAgency", "createAPI" };

    // 存储某个类型的gm的固有权限
    private static Dictionary<int, string> m_inherentRight = new Dictionary<int, string>();

    private static Dictionary<string, RightItem> m_all = new Dictionary<string, RightItem>();

    // 不同类型的账号的可分配权限 accType->权限
    private static Dictionary<int, DispatchRight> m_rightForAccType = new Dictionary<int, DispatchRight>();

    static RightMap()
    {
        initInherentRight();
        //////////////////////////////////////////////////////////////////////////
        addAllRight(RIGHT.CREATE_AGENCY, "创建代理");
        addAllRight(RIGHT.CREATE_API, "创建API");
        addAllRight(RIGHT.APPROVE_API, "审批API");
        addAllRight(RIGHT.DATA_STAT, "游戏参数统计");
        addAllRight(RIGHT.DEL_ACCOUNT, "删除账号");
        addAllRight(RIGHT.VIEW_AGENCY, "查看代理及会员");
        addAllRight(RIGHT.SCORE, "上分/下分");

        initAdminSubDispatchRight();
        initGeneralAgencyDispatchRight();
        initAgencyDispatchRight();
    }

    public static string getRightName(string r)
    {
        return r; // s_right[r];
    }

    public static bool hasRight(string rtype, string rightStr)
    {
        int index = rightStr.IndexOf(rtype);
        return index >= 0;
    }

    public static bool hasRight(int accType, string rtype, string rightStr)
    {
        if (hasInherentRight(accType, rtype))
            return true;

        int index = rightStr.IndexOf(rtype);
        return index >= 0;
    }

    // 账号类型accType是否拥有固有权限right
    public static bool hasInherentRight(int accType, string right)
    {
        if (accType == AccType.ACC_SUPER_ADMIN)
            return true;

        if (!m_inherentRight.ContainsKey(accType))
            return false;

        string r = m_inherentRight[accType];
        return r.IndexOf(right) >= 0;
    }

    public static Dictionary<string, RightItem> allRight { get { return m_all; } }

    public static Dictionary<string, RightItem> getDispatchRight(int accType)
    {
        if (!m_rightForAccType.ContainsKey(accType))
            return null;

        return m_rightForAccType[accType].allRight;
    }

    private static void initAdminSubDispatchRight()
    {
        DispatchRight dr = new DispatchRight();
        m_rightForAccType.Add(AccType.ACC_SUPER_ADMIN_SUB, dr);
        dr.addRight(RIGHT.APPROVE_API, m_all[RIGHT.APPROVE_API]);
        dr.addRight(RIGHT.DATA_STAT, m_all[RIGHT.DATA_STAT]);
        dr.addRight(RIGHT.DEL_ACCOUNT, m_all[RIGHT.DEL_ACCOUNT]);
        dr.addRight(RIGHT.VIEW_AGENCY, m_all[RIGHT.VIEW_AGENCY]);
        dr.addRight(RIGHT.SCORE, m_all[RIGHT.SCORE]);
    }
    
    // 总代理的可分配权限
    private static void initGeneralAgencyDispatchRight()
    {
        DispatchRight dr = new DispatchRight();
        m_rightForAccType.Add(AccType.ACC_GENERAL_AGENCY, dr);
        dr.addRight(RIGHT.VIEW_AGENCY, m_all[RIGHT.VIEW_AGENCY]);
        dr.addRight(RIGHT.SCORE, m_all[RIGHT.SCORE]);
    }

    // 代理的可分配权限
    private static void initAgencyDispatchRight()
    {
        DispatchRight dr = new DispatchRight();
        m_rightForAccType.Add(AccType.ACC_AGENCY, dr);
        dr.addRight(RIGHT.VIEW_AGENCY, m_all[RIGHT.VIEW_AGENCY]);
        dr.addRight(RIGHT.SCORE, m_all[RIGHT.SCORE]);
        dr.addRight(RIGHT.CREATE_AGENCY, m_all[RIGHT.CREATE_AGENCY]);
        dr.addRight(RIGHT.CREATE_API, m_all[RIGHT.CREATE_API]);
    }

    private static void addAllRight(string keyRight, string rightName)
    {
        RightItem item = new RightItem(rightName);
        m_all.Add(keyRight, item);
    }

    // 初始化固有权限
    private static void initInherentRight()
    {
        m_inherentRight.Add(AccType.ACC_GENERAL_AGENCY, RIGHT.CREATE_AGENCY + "," +
                                                        RIGHT.CREATE_API + "," +
                                                         "modify" + "," +
                                                         RIGHT.APPROVE_API);

        m_inherentRight.Add(AccType.ACC_AGENCY, "modify");

        m_inherentRight.Add(AccType.ACC_API, RIGHT.SCORE + "," + RIGHT.VIEW_AGENCY);

        m_inherentRight.Add(AccType.ACC_AGENCY_SUB, RIGHT.VIEW_AGENCY);

        m_inherentRight.Add(AccType.ACC_API_ADMIN, RIGHT.VIEW_AGENCY);

        m_inherentRight.Add(AccType.ACC_SUPER_ADMIN_SUB, RIGHT.OPERATION);
    }
}


