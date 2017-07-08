using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Security.Cryptography;
using System.Net;
using System.Collections.Specialized;
using MongoDB.Bson;
using MongoDB.Driver;

// 分页浏览的生成器
public class PageBrowseGenerator
{
    private StringBuilder m_textBuilder = new StringBuilder();
    // 同时显示的最多页链接个数
    private int m_maxShowPage = 10;

    public PageBrowseGenerator()
    {
    }

    public void setTotalMaxPage(int p)
    {
        m_maxShowPage = p;
    }

    // 构造页链接
    // curpage:当前是第几页 
    // row_each_page:每页多少条记录
    // href:链接的是哪个页面
    // total_page:返回的是总页数
    // user具体user，返回页链接时，需要针对某个用户
    // url_param额外url参数，这里面不能含page关键字
    public string genPageFoot(long curpage,
                                int row_each_page,
                                string href,
                                ref long total_page,
                                GMUser user,
                                Dictionary<string, object> url_param = null)
    {
        if (url_param != null && url_param.ContainsKey("page"))
        {
            LOGW.Info("PageBrowseGenerator.genPageFoot含page关键字");
            return "";
        }
        long page_count = getPageCount(row_each_page, user);
        total_page = page_count;
        if (page_count <= 1)
            return "";

        long count = page_count >= m_maxShowPage ? m_maxShowPage : page_count;
        m_textBuilder.Remove(0, m_textBuilder.Length);
        m_textBuilder.Append(genPage(curpage, page_count, true, 1, href, "首页", url_param));
        m_textBuilder.Append(genPage(curpage, page_count, true, curpage - 1, href, "上一页", url_param));
        m_textBuilder.Append("  ");
        m_textBuilder.Append(genBody(curpage, href, count, page_count, url_param));
        m_textBuilder.Append(genPage(curpage, page_count, false, curpage + 1, href, "下一页", url_param));
        m_textBuilder.Append(genPage(curpage, page_count, false, page_count, href, "尾页", url_param));
        return m_textBuilder.ToString();
    }

    // 取得共多少页
    private long getPageCount(int row_each_page, GMUser user)
    {
        return (long)Math.Ceiling((double)user.totalRecord / row_each_page);
    }

    // page_count总页数, count有多少链接可以点选
    private string genBody(long curpage, string href, long count, long page_count, Dictionary<string, object> url_param)
    {
        // 向前
        long pre = count >> 1;
        // 向后
        long next = count - pre - 1;

        long start = curpage - pre >= 1 ? curpage - pre : 1;
        long end = start + count;

        if (end > page_count)
        {
            end = page_count + 1;
            start = end - count;
        }
        URLParam[] plist = new URLParam[count];

        long j = 0;
        for (long i = start; i < end; i++, j++)
        {
            plist[j] = new URLParam();
            if (i == curpage)
            {
                plist[j].m_url = "";
                plist[j].m_text = i.ToString();
            }
            else
            {
                plist[j].m_url = href;
                plist[j].m_text = i.ToString();
                plist[j].m_key = "page";
                plist[j].m_value = i.ToString();
                plist[j].m_exUrlParam = url_param;
            }
        }
        string str = Tool.genHyperlink(plist);
        return str;
    }

    // 产生第指定文本的页
    private string genPage(long curpage, 
                           long page_count, 
                           bool left,
                           long special_page,
                           string href,
                           string text,
                           Dictionary<string, object> url_param)
    {
        if (left)
        {
            if (curpage <= 1)
                return "";
        }
        else
        {
            if (curpage >= page_count)
                return "";
        }
        URLParam[] p = new URLParam[1];
        p[0] = new URLParam();
        p[0].m_url = href;
        p[0].m_text = text;
        p[0].m_key = "page";
        p[0].m_value = special_page.ToString();
        p[0].m_exUrlParam = url_param;
        return Tool.genHyperlink(p);
    }
}

public class ItemHelp
{
    public static string getRewardList(List<ParamItem> rewardList)
    {
        string result = "";
        string name = "";

        for (int i = 0; i < rewardList.Count; i++)
        {
            ItemCFGData data = ItemCFG.getInstance().getValue(rewardList[i].m_itemId);
            if (data != null)
            {
                name = data.m_itemName;
            }
            else
            {
                name = "";
            }
            result += string.Format("id : {0}, name:{1}, count : {2}", rewardList[i].m_itemId, name, rewardList[i].m_itemCount);
            result += "<br />";
        }

        return result;
    }

    // Dictionary 道具，数量
    public static string getRewardList(Dictionary<int, int> rewardList)
    {
        string result = "";
        string name = "";

        foreach(var item in rewardList)
        {
            ItemCFGData data = ItemCFG.getInstance().getValue(item.Key);
            if (data != null)
            {
                name = data.m_itemName;
            }
            else
            {
                name = "";
            }
            result += string.Format("id : {0}, name:{1}, count : {2}", item.Key, name, item.Value);
            result += "<br />";
        }

        return result;
    }

    // 生成道具列表的数组
    public static BsonDocument genItemBsonArray(List<ParamItem> itemList)
    {
        if (itemList == null)
            return null;

        Dictionary<string, object> dd = new Dictionary<string, object>();
        for (int i = 0; i < itemList.Count; i++)
        {
            Dictionary<string, object> tmpd = new Dictionary<string, object>();
            tmpd.Add("itemId", itemList[i].m_itemId);
            tmpd.Add("itemCount", itemList[i].m_itemCount);
            dd.Add(i.ToString(), tmpd.ToBsonDocument());
        }
        return dd.ToBsonDocument();
    }

    // 返回盈利率
    public static string getRate(long income, long outlay)
    {
        if (outlay == 0)
            return "1";

        double factGain = (double)income / outlay;
        return Math.Round(factGain, 3).ToString();
    }

    // 返回实际盈利率
    public static string getFactExpRate(long income, long outlay)
    {
        if (income == 0 && outlay == 0)
            return "0";
        if (income == 0)
            return "-∞";

        double factGain = (double)(income - outlay) / income;
        return Math.Round(factGain, 3).ToString();
    }

    public static string getCowsCardTypeName(int cardType)
    {
        XmlConfig cfg = ResMgr.getInstance().getRes("cows_card.xml");
        return cfg.getString(cardType.ToString(), "未知");
    }

    public static string getRate(long up, long down, int decimalNum)
    {
        if (down == 0)
            return "0";

        double r = (double)(up) / down;
        return Math.Round(r, decimalNum).ToString();
    }

    public static string getRate(double up, double down, int decimalNum)
    {
        if (down == 0)
            return "0";

        double r = (up) / down;
        return Math.Round(r, decimalNum).ToString();
    }

    // 生成一个json串
    public static string genJsonStr(Dictionary<string, object> data)
    {
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

    public static string genHTML(System.Web.UI.Control c)
    {
        StringWriter sw = new StringWriter();
        HtmlTextWriter w = new HtmlTextWriter(sw);
        c.RenderControl(w);
        return sw.GetStringBuilder().ToString();
    }
}