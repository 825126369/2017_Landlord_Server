using System;
using System.Collections.Generic;
using System.Text;

[Serializable]
public class CSystemAnnounce
{
    public string mId;                  // 公告Id
    public string mTitle;            // 标题
    public string mContent;          // 内容
    public string mGenTime;          // 生成时间
    public int mDayAmount;           // 显示天数
    public bool mIsVisible;          // 是否显示
}

// 公告管理
class NoticeMgr
{
    private static NoticeMgr s_mgr = null;

    public static NoticeMgr getInstance()
    {
        if (s_mgr == null)
        {
            s_mgr = new NoticeMgr();
        }
        return s_mgr;
    }

    public bool addNotice(string title, string content, int dayamount, GMUser user)
    {
        if (user == null)
            return false;

        string[] sp = { "\r\n" };
        string[] tmp = content.Split(sp, StringSplitOptions.None);
        int i = 0;
        content = "";
        for (; i < tmp.Length - 1; i++)
        {
            content += tmp[i];
            content += "&";
        }
        content += tmp[i];
        bool res = _addNotice(title, content, dayamount, user.getDbServerID());
        if (res)
        {
          //  OpLogMgr.getInstance().addLog(LogType.PUBLIC_NOTICE, new ParamAddNotice(title, content, dayamount), user);
        }
        user.setOpResult(res ? OpRes.opres_success : OpRes.op_res_failed);
        return res;
    }

    public bool activateNotice(int index, GMUser account)
    {
        if (account == null)
            return false;

        CSystemAnnounce pa = null;
        bool res = _activateNotice(index, ref pa, account.getDbServerID());
        if (res)
        {
            if (pa != null)
            {
               // OpLogMgr.getInstance().addLog(LogType.ACTIVATE_NOTICE, new ParamActivateNotice(pa.mTitle, pa.mContent, pa.mDayAmount), account);
            }
        }
        return res;
    }

    public bool deleteNotice(int[] arr_index, GMUser account)
    {
        if (account == null)
            return false;

        List<CSystemAnnounce> out_list = null;
        bool res = _deleteNotice(arr_index, ref out_list, account.getDbServerID());
        if (res)
        {
            if (out_list != null)
            {
              //  OpLogMgr.getInstance().addLog(LogType.DELETE_NOTICE, new ParamDeleteNotice(out_list), account);
            }
        }
        return res;
    }

    // 增加一条公告
    // 标题，内容，显示天数
    private bool _addNotice(string title, string content, int dayamount, int serverid)
    {
        Dictionary<string, object> data = null; // DBMgr.getInstance().getTableData("Config", "_key", "SystemAnnounceList", serverid, DbName.DB_FISH_LORD);
        if (data == null)
        {
            LOGW.Info("不存在Config, dbkey: SystemAnnounceList");
            return false;
        }

        List<CSystemAnnounce> tlist = getList(data);

        CSystemAnnounce a = new CSystemAnnounce();
        a.mTitle = title;
        a.mContent = content;
        a.mGenTime = DateTime.Now.ToString();
        a.mDayAmount = dayamount;
        a.mIsVisible = true;
        a.mId = Guid.NewGuid().ToString();

        // 先前没有公告
        if (tlist == null || tlist.Count == 0)
        {
            tlist = new List<CSystemAnnounce>();
        }
        else // 已存在公告
        {
            // 新增一条公告时，其他的公告设置为不显示
            foreach (CSystemAnnounce sa in tlist)
            {
                sa.mIsVisible = false;
            }
        }
        tlist.Add(a);
        //data["_value"] = DBMgr.getInstance().getUserDefValueAsString(tlist);
        return true; // DBMgr.getInstance().save("Config", data, "_key", "SystemAnnounceList", serverid, DbName.DB_FISH_LORD);
    }

    // 取得所有公告，存于列表result内
    public List<CSystemAnnounce> getAllNotice(GMUser user)
    {
        if (user == null)
            return null;
        Dictionary<string, object> data = null; // DBMgr.getInstance().getTableData("Config", "_key", "SystemAnnounceList", user.getDbServerID(), DbName.DB_FISH_LORD);
        return getList(data);
    }

    // 隐藏所有公告
    public void hideAllNotice(GMUser user)
    {
        if (user == null)
            return;
        Dictionary<string, object> data = null; // DBMgr.getInstance().getTableData("Config", "_key", "SystemAnnounceList", user.getDbServerID(), DbName.DB_FISH_LORD);
        List<CSystemAnnounce> tlist = getList(data);
        if (tlist == null)
            return;

        foreach (CSystemAnnounce t in tlist)
        {
            t.mIsVisible = false;
        }
       // data["_value"] = DBMgr.getInstance().getUserDefValueAsString(tlist);
       // DBMgr.getInstance().save("Config", data, "_key", "SystemAnnounceList", user.getDbServerID(), DbName.DB_FISH_LORD);
    }

    // 激活第index个公告
    private bool _activateNotice(int index, ref CSystemAnnounce out_an, int serverid)
    {
        Dictionary<string, object> data = null; // DBMgr.getInstance().getTableData("Config", "_key", "SystemAnnounceList", serverid, DbName.DB_FISH_LORD);
        if (data == null)
        {
            LOGW.Info("不存在dbkey: SystemAnnounceList");
            return false;
        }
        List<CSystemAnnounce> tlist = getList(data);
        if (tlist == null || tlist.Count == 0)
            return false;

        if (index < 0 || index >= tlist.Count)
            return false;

        for (int i = 0; i < tlist.Count; i++)
        {
            if (i == index)
            {
                tlist[i].mIsVisible = true;
                out_an = tlist[i];
            }
            else
            {
                tlist[i].mIsVisible = false;
            }
        }
        
        // 保存内容
       // data["_value"] = DBMgr.getInstance().getUserDefValueAsString(tlist);
        return true; // DBMgr.getInstance().save("Config", data, "_key", "SystemAnnounceList", serverid, DbName.DB_FISH_LORD);
    }

    // 删除索引列表中的公告
    private bool _deleteNotice(int[] arr_index, ref List<CSystemAnnounce> out_list, int serverid)
    {
        if (arr_index == null)
            return false;
        Dictionary<string, object> data = null; // DBMgr.getInstance().getTableData("Config", "_key", "SystemAnnounceList", serverid, DbName.DB_FISH_LORD);
        if (data == null)
        {
            LOGW.Info("不存在dbkey: SystemAnnounceList");
            return false;
        }
        List<CSystemAnnounce> tlist = getList(data);
        // 已经为空了，不用再删了
        if (tlist == null || tlist.Count == 0)
            return false;

        out_list = new List<CSystemAnnounce>();

        // 优先删除大索引
        for (int i = arr_index.Length - 1; i >= 0; i--)
        {
            if (arr_index[i] >= 0 && arr_index[i] < tlist.Count)
            {
                out_list.Add(tlist[ arr_index[i] ]);
                tlist.RemoveAt(arr_index[i]);
            }
        }

        // 保存内容
       // data["_value"] = DBMgr.getInstance().getUserDefValueAsString(tlist);
        return true; // DBMgr.getInstance().save("Config", data, "_key", "SystemAnnounceList", serverid, DbName.DB_FISH_LORD);
    }

    private List<CSystemAnnounce> getList(Dictionary<string, object> data)
    {
        if (data == null)
            return null;

        List<CSystemAnnounce> tlist = null;
        if (data.ContainsKey("_value"))
        {
            string val = Convert.ToString(data["_value"]);
            //tlist = DBMgr.getInstance().setUserDefValueAsString<List<CSystemAnnounce>>(val);
        }
        return tlist;
    }
}

