using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;

// xml配置文件的读取
public class XmlConfigMaker
{
    // 从串中读取配置
    public XmlConfig loadFromString(string xml)
    {
        try
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            return load(doc, xml);
        }
        catch (System.Exception ex)
        {
            LOG.Error("读取xml[{0}]时发生异常[{1}]", xml, ex.ToString());
        }
        return null;
    }

    // 从xml文件中读取配置
    public XmlConfig loadFromFile(string xmlfile)
    {
        if (!File.Exists(xmlfile))
        {
            LOG.Error("找不到文件:" + xmlfile);
            return null;
        }
        try
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlfile);
            return load(doc, xmlfile);
        }
        catch (System.Exception ex)
        {
            LOG.Error("读取[{0}]时发生异常[{1}]", xmlfile, ex.ToString());
        }
        return null;
    }

    private XmlConfig load(XmlDocument doc, string xmlfile)
    {
        XmlConfig config = new XmlConfig(xmlfile);
        XmlNode node = doc.SelectSingleNode("/configuration");
        for (node = node.FirstChild; node != null; node = node.NextSibling)
        {
            if(node is XmlComment)  // 忽略注释
                continue;

            string key = node.Attributes["key"].Value;
            string type = node.Attributes["type"].Value;

            // 数组内元素仅支持基本数据类型
            if (type == "array") // <elem>value1</elem>  <elem>value2</elem>， 返回的是一个List
            {
                string elem_type = node.Attributes["elemtype"].Value;
                object arr = parseArray(elem_type, node.FirstChild);
                if (arr != null)
                {
                    config.add(key, arr);
                }
            }
            else if (type == "table") // 表格字段形式，返回的是一个 List<Dictionary<string, object>>
            {
                object v = parseTable(node.FirstChild);
                config.add(key, v);
            }
            else
            {
                string value = node.Attributes["value"].Value;
                object v = parse(key, value, type);
                config.add(key, v);
            }
        }
        return config;
    }

    private object parse(string key, string value, string type)
    {
        switch (type)
        {
            case "byte":
                {
                    byte v = Convert.ToByte(value);
                    return v;
                }
            case "char":
                {
                    char v = Convert.ToChar(value);
                    return v;
                }
            case "int":
                {
                    int v = Convert.ToInt32(value);
                    return v;
                }
            case "unit":
                {
                    uint v = Convert.ToUInt32(value);
                    return v;
                }
            case "float":
                {
                    float v = (float)Convert.ToDouble(value);
                    return v;
                }
            case "double":
                {
                    double v = Convert.ToDouble(value);
                    return v;
                }
            case "string":
                {
                    return value;
                }
            case "bool":
                {
                    return Convert.ToBoolean(value);
                }
            default:
                {
                    LOG.Error("XmlConfigMaker, 无法识别类型:" + type);
                }
                break;
        }
        return null;
    }

    // 解析数组
    private object parseArray(string elem_type, XmlNode first)
    {
        switch (elem_type)
        {
            case "byte":
                {
                    List<byte> arr = new List<byte>();
                    for (XmlNode tmp = first; tmp != null; tmp = tmp.NextSibling)
                    {
                        if (tmp is XmlComment)  // 忽略注释
                            continue;
                        arr.Add(Convert.ToByte(tmp.FirstChild.Value));
                    }
                    return arr;
                }
            case "char":
                {
                    List<char> arr = new List<char>();
                    for (XmlNode tmp = first; tmp != null; tmp = tmp.NextSibling)
                    {
                        if (tmp is XmlComment)  // 忽略注释
                            continue;
                        arr.Add(Convert.ToChar(tmp.FirstChild.Value));
                    }
                    return arr;
                }
            case "int":
                {
                    List<int> arr = new List<int>();
                    for (XmlNode tmp = first; tmp != null; tmp = tmp.NextSibling)
                    {
                        if (tmp is XmlComment)  // 忽略注释
                            continue;
                        arr.Add(Convert.ToInt32(tmp.FirstChild.Value));
                    }
                    return arr;
                }
            case "uint":
                {
                    List<uint> arr = new List<uint>();
                    for (XmlNode tmp = first; tmp != null; tmp = tmp.NextSibling)
                    {
                        if (tmp is XmlComment)  // 忽略注释
                            continue;
                        arr.Add(Convert.ToUInt32(tmp.FirstChild.Value));
                    }
                    return arr;
                }
            case "float":
                {
                    List<float> arr = new List<float>();
                    for (XmlNode tmp = first; tmp != null; tmp = tmp.NextSibling)
                    {
                        if (tmp is XmlComment)  // 忽略注释
                            continue;
                        arr.Add((float)Convert.ToDouble(tmp.FirstChild.Value));
                    }
                    return arr;
                }
            case "double":
                {
                    List<double> arr = new List<double>();
                    for (XmlNode tmp = first; tmp != null; tmp = tmp.NextSibling)
                    {
                        if (tmp is XmlComment)  // 忽略注释
                            continue;
                        arr.Add(Convert.ToDouble(tmp.FirstChild.Value));
                    }
                    return arr;
                }
            case "string":
                {
                    List<string> arr = new List<string>();
                    for (XmlNode tmp = first; tmp != null; tmp = tmp.NextSibling)
                    {
                        if (tmp is XmlComment)  // 忽略注释
                            continue;
                        arr.Add(tmp.FirstChild.Value);
                    }
                    return arr;
                }
            default:
                {
                    LOG.Error("parseArray, 无法识别类型:" + elem_type);
                }
                break;
        }
        return null;
    }
    
    // 解析表格, 表格内， 仅能是基本类型
    private object parseTable(XmlNode first)
    {
        List<Dictionary<string, object>> t = new List<Dictionary<string, object>>();

        for (XmlNode node = first; node != null; node = node.NextSibling)
        {
            if (node is XmlComment)  // 忽略注释
                continue;

            Dictionary<string, object> d = new Dictionary<string, object>();
            for (XmlNode tmp = node.FirstChild; tmp != null; tmp = tmp.NextSibling)
            {
                if (tmp is XmlComment)  // 忽略注释
                    continue;

                string key = tmp.Attributes["key"].Value;
                string value = tmp.Attributes["value"].Value;
                string type = tmp.Attributes["type"].Value;

                if (!d.ContainsKey(key))
                {
                    object v = parse(key, value, type);
                    d.Add(key, v);
                }
                else
                {
                    LOG.Error("parseTable中key[{0}]重复", key);
                }
            }
            t.Add(d);
        }
        return t;
    }
}

