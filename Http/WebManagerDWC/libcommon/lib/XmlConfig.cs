using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// XML的配置
public class XmlConfig
{
    private Dictionary<string, object> m_config = new Dictionary<string, object>();
    private string m_file;

    public XmlConfig(string file)
    {
        m_file = file;
    }
    
    // 返回个数
    public int getCount() { return m_config.Count; }

    // 存在关键字为key的值，则返回，否则返回默认值defvalue
    public byte getByte(string key, byte defvalue)
    {
        if (m_config.ContainsKey(key))
        {
            return (byte)m_config[key];
        }
        LOG.Warn("getByte, 找不到关键字[{0}], 将使用默认值[{1}]", key, defvalue);
        return defvalue;
    }

    public char getChar(string key, char defvalue)
    {
        if (m_config.ContainsKey(key))
        {
            return (char)m_config[key];
        }
        LOG.Warn("getChar, 找不到关键字[{0}], 将使用默认值[{1}]", key, defvalue);
        return defvalue;
    }

    public int getInt(string key, int defvalue)
    {
        if (m_config.ContainsKey(key))
        {
            return (int)m_config[key];
        }
        LOG.Warn("getInt, 找不到关键字[{0}], 将使用默认值[{1}]", key, defvalue);
        return defvalue;
    }

    public uint getUInt(string key, uint defvalue)
    {
        if (m_config.ContainsKey(key))
        {
            return (uint)m_config[key];
        }
        LOG.Warn("getUInt, 找不到关键字[{0}], 将使用默认值[{1}]", key, defvalue);
        return defvalue;
    }

    public float getFloat(string key, float defvalue)
    {
        if (m_config.ContainsKey(key))
        {
            return (float)m_config[key];
        }
        LOG.Warn("getFloat, 找不到关键字[{0}], 将使用默认值[{1}]", key, defvalue);
        return defvalue;
    }

    public double getDouble(string key, double defvalue)
    {
        if (m_config.ContainsKey(key))
        {
            return (double)m_config[key];
        }
        LOG.Warn("getDouble, 找不到关键字[{0}], 将使用默认值[{1}]", key, defvalue);
        return defvalue;
    }

    public string getString(string key, string defvalue)
    {
        if (m_config.ContainsKey(key))
        {
            return (string)m_config[key];
        }
        LOG.Warn("getString, 找不到关键字[{0}], 将使用默认值[{1}]", key, defvalue);
        return defvalue;
    }

    public bool getBool(string key, bool defvalue)
    {
        if (m_config.ContainsKey(key))
        {
            return (bool)m_config[key];
        }
        LOG.Warn("getBool, 找不到关键字[{0}], 将使用默认值[{1}]", key, defvalue);
        return defvalue;
    }

    // 返回数组
    public List<T> getArray<T>(string key)
    {
        if (m_config.ContainsKey(key))
        {
            return (List<T>)m_config[key];
        }
        return null;
    }

    // 返回表格
    public List<Dictionary<string, object>> getTable(string key)
    {
        if (m_config.ContainsKey(key))
        {
            return (List<Dictionary<string, object>>)m_config[key];
        }
        return null;
    }

    // 设置key的值为val
    public bool setValue(string key, object val)
    {
        if (m_config.ContainsKey(key))
        {
            m_config[key] = val;
            return true;
        }
        LOG.Warn("setValue, 找不到关键字[{0}]", key);
        return false;
    }

    public bool add(string key, byte value)
    {
        if (valid(key))
        {
            m_config.Add(key, value);
            return true;
        }
        return false;
    }

    public bool add(string key, char value)
    {
        if (valid(key))
        {
            m_config.Add(key, value);
            return true;
        }
        return false;
    }

    // 添加整数值
    public bool add(string key, int value)
    {
        if (valid(key))
        {
            m_config.Add(key, value);
            return true;
        }
        return false;
    }

    public bool add(string key, uint value)
    {
        if (valid(key))
        {
            m_config.Add(key, value);
            return true;
        }
        return false;
    }

    public bool add(string key, float value)
    {
        if (valid(key))
        {
            m_config.Add(key, value);
            return true;
        }
        return false;
    }

    public bool add(string key, double value)
    {
        if (valid(key))
        {
            m_config.Add(key, value);
            return true;
        }
        return false;
    }

    public bool add(string key, string value)
    {
        if (valid(key))
        {
            m_config.Add(key, value);
            return true;
        }
        return false;
    }

    // 增加自定义类型
    public bool add(string key, object value)
    {
        if (valid(key))
        {
            m_config.Add(key, value);
            return true;
        }
        return false;
    }

    private bool valid(string key)
    {
        if (m_config.ContainsKey(key))
        {
            LOG.Error("配置文件[{0}], 关键字[{1}]重复", m_file, key);
            return false;
        }
        return true;
    }
}

