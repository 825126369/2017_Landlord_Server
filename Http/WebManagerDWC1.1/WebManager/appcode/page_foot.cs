using System;
using System.Collections.Generic;
using System.Web;
using System.IO;

public class PageGen
{
    // 每页多少条记录
    protected int m_rowEachPage = 50;

    // 当前页
    protected int m_curPage = 1;

    public PageGen()
    {
    }

    public PageGen(int row_each_page)
    {
        m_rowEachPage = row_each_page;
    }

    public int rowEachPage
    {
        get { return m_rowEachPage; }
        set { m_rowEachPage = value; }
    }

    public int curPage
    {
        get { return m_curPage; }
        set { m_curPage = value; }
    }

    public virtual bool parse(HttpRequest Request)
    {
        bool res = false;
        string page = Request.QueryString["page"];
        if (page != null)
        {
            m_curPage = Convert.ToInt32(page);
            res = true;
        }
        return res;
    }

    // 生成分页
    public virtual void genPage(ParamQueryBase query_param, string url, ref string page_link, ref string foot, GMUser user)
    {
    }
}

//////////////////////////////////////////////////////////////////////////

public class PageGenMoney : PageGen
{
    public string m_time = "";
    public string m_param = "";
    public int m_way;
    public int m_filter;
    public int m_property;
    public string m_range = "";
    public int m_gameId;

    public PageGenMoney(int row_each_page)
        : base(row_each_page)
    {
    }

    public override bool parse(HttpRequest Request)
    {
        bool res = base.parse(Request);

        string str = Request.QueryString["time"];
        if (str != null)
        {
            m_time = str;
            res = true;
        }
        str = Request.QueryString["param"];
        if (str != null)
        {
            m_param = str;
            res = true;
        }
        str = Request.QueryString["way"];
        if (str != null)
        {
            m_way = Convert.ToInt32(str);
            res = true;
        }
        str = Request.QueryString["filter"];
        if (str != null)
        {
            m_filter = Convert.ToInt32(str);
            res = true;
        }
        str = Request.QueryString["property"];
        if (str != null)
        {
            m_property = Convert.ToInt32(str);
            res = true;
        }
        str = Request.QueryString["range"];
        if (str != null)
        {
            m_range = str;
            res = true;
        }
        str = Request.QueryString["gameId"];
        if (str != null)
        {
            m_gameId = Convert.ToInt32(str);
            res = true;
        }
        return res;
    }

    public override void genPage(ParamQueryBase queryParam, string url, ref string pageLink, ref string foot, GMUser user)
    {
        ParamMoneyQuery dparam = (ParamMoneyQuery)queryParam;
        PageBrowseGenerator p = new PageBrowseGenerator();
        long total_page = 0;
        Dictionary<string, object> urlParam = new Dictionary<string, object>();
        urlParam["param"] = dparam.m_param;
        urlParam["time"] = dparam.m_time;
        urlParam["way"] = (int)dparam.m_way;
        urlParam["filter"] = dparam.m_filter;
        urlParam["property"] = dparam.m_property;
        urlParam["range"] = dparam.m_range;
        urlParam["gameId"] = dparam.m_gameId;

        pageLink = p.genPageFoot(queryParam.m_curPage, m_rowEachPage, url, ref total_page, user, urlParam);
        if (total_page != 0)
        {
            foot = queryParam.m_curPage + "/" + total_page;
        }
    }
}

//////////////////////////////////////////////////////////////////////////

public class PageGenDailyTask : PageGen
{
    public string m_time = "";
    public string m_param = "";
    public int m_way;

    public PageGenDailyTask(int rowEachPage)
        : base(rowEachPage)
    {
    }

    public override bool parse(HttpRequest Request)
    {
        bool res = base.parse(Request);

        string str = Request.QueryString["time"];
        if (str != null)
        {
            m_time = str;
            res = true;
        }
        str = Request.QueryString["param"];
        if (str != null)
        {
            m_param = str;
            res = true;
        }
        str = Request.QueryString["way"];
        if (str != null)
        {
            m_way = Convert.ToInt32(str);
            res = true;
        }
        return res;
    }

    public override void genPage(ParamQueryBase queryParam, string url, ref string pageLink, ref string foot, GMUser user)
    {
        ParamQuery dparam = (ParamQuery)queryParam;
        PageBrowseGenerator p = new PageBrowseGenerator();
        long total_page = 0;
        Dictionary<string, object> url_param = new Dictionary<string, object>();
        url_param["param"] = dparam.m_param;
        url_param["time"] = dparam.m_time;
        url_param["way"] = (int)dparam.m_way;
        pageLink = p.genPageFoot(queryParam.m_curPage, m_rowEachPage, url, ref total_page, user, url_param);
        if (total_page != 0)
        {
            foot = queryParam.m_curPage + "/" + total_page;
        }
    }
}

//////////////////////////////////////////////////////////////////////////

// 抽奖
public class PageGenLottery : PageGen
{
    public string m_time = "";
    public string m_playerId = "";
    public string m_boxId = "";
    public string m_param = "";

    public PageGenLottery(int rowEachPage)
        : base(rowEachPage)
    {
    }

    public override bool parse(HttpRequest Request)
    {
        bool res = base.parse(Request);

        string str = Request.QueryString["time"];
        if (str != null)
        {
            m_time = str;
            res = true;
        }
        str = Request.QueryString["playerId"];
        if (str != null)
        {
            m_playerId = str;
            res = true;
        }
        str = Request.QueryString["boxId"];
        if (str != null)
        {
            m_boxId = str;
            res = true;
        }
        str = Request.QueryString["param"];
        if (str != null)
        {
            m_param = str;
            res = true;
        }
        return res;
    }

    public override void genPage(ParamQueryBase queryParam, string url, ref string pageLink, ref string foot, GMUser user)
    {
        ParamLottery dparam = (ParamLottery)queryParam;
        PageBrowseGenerator p = new PageBrowseGenerator();
        long total_page = 0;
        Dictionary<string, object> urlParam = new Dictionary<string, object>();
        urlParam["time"] = dparam.m_time;
        urlParam["playerId"] = dparam.m_playerId;
        urlParam["boxId"] = dparam.m_boxId;
        urlParam["param"] = dparam.m_param;
        pageLink = p.genPageFoot(queryParam.m_curPage, m_rowEachPage, url, ref total_page, user, urlParam);
        if (total_page != 0)
        {
            foot = queryParam.m_curPage + "/" + total_page;
        }
    }
}

//////////////////////////////////////////////////////////////////////////

// 充值记录的分页查询
public class PageGenRecharge : PageGen
{
    public int m_platIndex = 0;
    public int m_result;
    public string m_range = "";
    public int m_way;
    public string m_param = "";
    public string m_time = "";
    public int m_serverIndex = 0;

    public PageGenRecharge(int row_each_page)
        : base(row_each_page)
    {
    }

    public override bool parse(HttpRequest Request)
    {
        bool res = base.parse(Request);

        string str = Request.QueryString["plat"];
        if (str != null)
        {
            m_platIndex = Convert.ToInt32(str);
            res = true;
        }
        str = Request.QueryString["result"];
        if (str != null)
        {
            m_result = Convert.ToInt32(str);
            res = true;
        }
        str = Request.QueryString["range"];
        if (str != null)
        {
            m_range = str;
            res = true;
        }
        str = Request.QueryString["way"];
        if (str != null)
        {
            m_way = Convert.ToInt32(str);
            res = true;
        }
        str = Request.QueryString["param"];
        if (str != null)
        {
            m_param = str;
            res = true;
        }
        str = Request.QueryString["time"];
        if (str != null)
        {
            m_time = str;
            res = true;
        }
        str = Request.QueryString["server"];
        if (str != null)
        {
            m_serverIndex = Convert.ToInt32(str);
            res = true;
        }
        return res;
    }

    public override void genPage(ParamQueryBase query_param, string url, ref string page_link, ref string foot, GMUser user)
    {
        ParamQueryRecharge dparam = (ParamQueryRecharge)query_param;
        PageBrowseGenerator p = new PageBrowseGenerator();
        long total_page = 0;
        Dictionary<string, object> url_param = new Dictionary<string, object>();
        url_param["param"] = dparam.m_param;
        url_param["way"] = (int)dparam.m_way;
        url_param["time"] = dparam.m_time;
        url_param["plat"] = dparam.m_platIndex;
        url_param["result"] = dparam.m_result;
        url_param["range"] = dparam.m_range;
        url_param["server"] = dparam.m_gameServerIndex;
        page_link = p.genPageFoot(query_param.m_curPage, m_rowEachPage, url, ref total_page, user, url_param);
        if (total_page != 0)
        {
            foot = query_param.m_curPage + "/" + total_page;
        }
    }
}


//////////////////////////////////////////////////////////////////////////

// 礼券分页
public class PageGift : PageGen
{
    // 状态
    public int m_state = 0;
    public string m_playerId = "";

    public PageGift(int rowEachPage)
        : base(rowEachPage)
    {
    }

    public override bool parse(HttpRequest Request)
    {
        bool res = base.parse(Request);

        string str = Request.QueryString["state"];
        if (str != null)
        {
            m_state = Convert.ToInt32(str);
            res = true;
        }
        str = Request.QueryString["playerId"];
        if (str != null)
        {
            m_playerId = str;
            res = true;
        }
        return res;
    }

    public override void genPage(ParamQueryBase query_param, string url, ref string page_link, ref string foot, GMUser user)
    {
        ParamQueryGift dparam = (ParamQueryGift)query_param;
        PageBrowseGenerator p = new PageBrowseGenerator();
        long total_page = 0;
        Dictionary<string, object> url_param = new Dictionary<string, object>();
        url_param["state"] = dparam.m_state;
        url_param["playerId"] = dparam.m_param;
        page_link = p.genPageFoot(query_param.m_curPage, m_rowEachPage, url, ref total_page, user, url_param);
        if (total_page != 0)
        {
            foot = query_param.m_curPage + "/" + total_page;
        }
    }
}






