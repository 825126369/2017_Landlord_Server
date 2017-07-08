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
    public const string DATE_TIME1 = @"^\s*(\d{4})/(\d{1,2})/(\d{1,2})\s+(\d{1,2})$";
    public const string DATE_TIME2 = @"^\s*(\d{4})/(\d{1,2})/(\d{1,2})\s+(\d{1,2}):(\d{1,2})$";
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
    public const string NUM_LIST_BY_BLANK = @"\s*(-{0,1}\d+)(\s+-{0,1}\d+)+\s*$";

    public const string SINGLE_NUM = @"\s*\d+\s*$";
}

public class URLParam
{
    public string m_url = "";
    public string m_text = "";
    public string m_key = "";
    public string m_value = "";
    // 额外的url参数
    public Dictionary<string, object> m_exUrlParam = null;
}

public class Tool
{
    public static StringBuilder m_textBuilder = new StringBuilder();
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
        clearTextBuilder();

        if (param == null)
            return "";

        foreach (URLParam p in param)
        {
            if (p.m_url != "")
            {
                m_textBuilder.Append("<a ");

                m_textBuilder.Append(" style=");
                m_textBuilder.Append("\"");
                m_textBuilder.Append("border:1px solid black;");
                m_textBuilder.Append("padding : 5px 5px 5px 5px;");
                m_textBuilder.Append("text-decoration : none;");
                m_textBuilder.Append("font-size:medium;");
                m_textBuilder.Append("\"");

                m_textBuilder.Append(" href=");
                m_textBuilder.Append("\"");
                m_textBuilder.Append(p.m_url);

                m_textBuilder.Append("?");
                m_textBuilder.Append(p.m_key);
                m_textBuilder.Append("=");
                m_textBuilder.Append(p.m_value);

                // 添加额外的url参数
                if (p.m_exUrlParam != null)
                {
                    foreach (var urlp in p.m_exUrlParam)
                    {
                        m_textBuilder.Append('&');
                        m_textBuilder.Append(urlp.Key);
                        m_textBuilder.Append("=");
                        m_textBuilder.Append(Convert.ToString(urlp.Value));
                    }
                }
                m_textBuilder.Append("\"");
                
                m_textBuilder.Append(">");
                m_textBuilder.Append(p.m_text);
                m_textBuilder.Append("</a>");
            }
            else
            {
                m_textBuilder.Append(p.m_text);
            }
            
            m_textBuilder.Append("  ");
        }
        return m_textBuilder.ToString();
    }

    public static string genHyperlink(string url, URLParam param, string text)
    {
        clearTextBuilder();

        m_textBuilder.Append("<a ");
        m_textBuilder.Append("href=");
        m_textBuilder.Append(url);
        if (param != null)
        {
            m_textBuilder.Append("?");
            m_textBuilder.Append(param.m_key);
            m_textBuilder.Append("=");
            m_textBuilder.Append(param.m_value);
        }

        m_textBuilder.Append(" style=");
        m_textBuilder.Append("\"");
        m_textBuilder.Append("border:1px solid black;");
        m_textBuilder.Append("padding : 5px 5px 5px 5px;");
        m_textBuilder.Append("text-decoration:none");
        m_textBuilder.Append("\"");

        m_textBuilder.Append(">");
        m_textBuilder.Append(text);
        m_textBuilder.Append("</a>");
        return m_textBuilder.ToString();
    }

    public static void clearTextBuilder()
    {
        m_textBuilder.Remove(0, m_textBuilder.Length);
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
}
