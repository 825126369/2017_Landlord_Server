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
}
