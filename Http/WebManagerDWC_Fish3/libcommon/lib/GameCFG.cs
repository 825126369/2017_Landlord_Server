using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

public interface IXmlData
{
    void init();
}

public class XmlDataTable<T, KEY, VALUE> where T : IXmlData, new()
{
    public static T s_obj = default(T);

    protected Dictionary<KEY, VALUE> m_data = new Dictionary<KEY, VALUE>();

    public static T getInstance()
    {
        if (s_obj == null)
        {
            s_obj = new T();
            s_obj.init();
        }
        return s_obj;
    }

    public VALUE getValue(KEY k)
    {
        if (m_data.ContainsKey(k))
            return m_data[k];

        return default(VALUE);
    }

    public Dictionary<KEY, VALUE> getAllData()
    {
        return m_data;
    }
}

public class ItemCFGData
{
    public int m_itemId;
    public string m_itemName = "";
}

// 日常或成就相关
public class QusetCFGData
{
    // 任务ID
    public int m_questId;
    // 任务类型
    public int m_questType;
    // 任务名称
    public string m_questName = "";
}

public enum TaskType
{
    taskTypeDaily = 1,
    taskTypeAchieve = 2,
}

// 鱼表
public class FishCFGData
{
    // 鱼ID
    public int m_fishId;

    // 鱼名称
    public string m_fishName = "";
}

// 鳄鱼数据
public class Crocodile_RateCFGData
{
    // 区域ID
    public int m_areaId;

    // 名称
    public string m_name = "";

    public string m_icon;
    public string m_color;
}


public class Dice_BetAreaCFGData
{
    // 区域ID
    public int m_areaId;

    // 名称
    public string m_name;

    public int m_span = 0;

    public string m_desc;
}
