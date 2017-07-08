using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

public interface ITable
{
    FieldCsv fetch(int col);
    FieldCsv fetch(string col_name);
}

public interface IUserTabe
{
    // 开始读取
    void beginRead();
    // 读取其中的一行
    void readLine(ITable table);
    // 结束读取
    void endRead();
}

public class FieldCsv
{
    // 格子内容
    private string m_content;

    public FieldCsv(string c)
    {
        m_content = c;
    }

    public int toInt()
    {
        // 字符串为空，返回默认0
        if (m_content == string.Empty)
            return 0;

        int t = 0;
        try
        {
            t = Convert.ToInt32(m_content);
        }
        catch (Exception)
        {
            LOG.Info("读表格整数值时失败");
        }
        return t;
    }

    public float toFloat()
    {
        // 字符串为空，返回默认0
        if (m_content == string.Empty)
            return 0.0f;

        float t = 0.0f;
        try
        {
            t = (float)Convert.ToDouble(m_content);
        }
        catch (Exception)
        {
            LOG.Info("读表格浮点值时失败");
        }
        return t;
    }

    public byte toByte()
    {
        if (m_content == string.Empty)
            return 0;

        byte t = 0;
        try
        {
            t = Convert.ToByte(m_content);
        }
        catch (Exception)
        {
            LOG.Info("读表格byte值时失败!");
        }
        return t;
    }

    public string toStr()
    {
        return m_content;
    }
}

public class Csv
{
    public static bool load<T>(string file, T obj, char end_flag = ' ') where T : IUserTabe
    {
        if (obj == null)
        {
            LOG.Info("传入了空的引用");
            return false;
        }

        CsvReader cr = new CsvReader();
        if (!cr.load(file, end_flag))
            return false;

        int row = 0;
        cr.gotoLine(row);
        obj.beginRead();
        while (!cr.isFinish())
        {
            obj.readLine(cr);
            row++;
            cr.gotoLine(row);
        }
        obj.endRead();
        return true;
    }

    public static bool loadXml<T>(string file, T obj) where T : IUserTabe
    {
        if (obj == null)
        {
            LOG.Info("loadXml 传入了空的引用");
            return false;
        }

        XmlReader cr = new XmlReader();
        if (!cr.load(file))
            return false;

        int row = 0;
        cr.gotoLine(row);
        obj.beginRead();
        while (!cr.isFinish())
        {
            obj.readLine(cr);
            row++;
            cr.gotoLine(row);
        }
        obj.endRead();
        return true;
    }
}

public class CsvReader : ITable
{
    // 存储表格中的各行数据
    private Dictionary<int, List<FieldCsv>> m_contents = new Dictionary<int, List<FieldCsv>>();
    // 存储列名到列索引的对应关系
    private Dictionary<string, int> m_nameToCol = new Dictionary<string, int>();
    // 总行数
    private int m_totalRows;
    // 当前行
    private int m_curRow;

    public bool load(string file, char end_flag)
    {
        if (!File.Exists(file))
        {
            LOG.Info("文件[{0}]不存在!", file);
            return false;
        }

        StreamReader sr = new StreamReader(file, Encoding.Default);
        string line = sr.ReadLine();
        if (line != null)
        {
            if (!init(line))
                return false;
        }

        int row = 0;
        line = readLine(sr, end_flag);//sr.ReadLine();
        while (line != null)
        {
            if (!isEmptyLine(line))
            {
                addLine(line, row);
                row++;
            }
            line = readLine(sr, end_flag); //sr.ReadLine();
        }
        m_totalRows = row;
        sr.Close();
        return true;
    }

    public FieldCsv fetch(int col)
    {
        return get(m_curRow, col);
    }

    public FieldCsv fetch(string col_name)
    {
        if(!m_nameToCol.ContainsKey(col_name))
        {
            LOG.Info("表格中列名[{0}]不存在!", col_name);
            return null;
        }
        int col = m_nameToCol[col_name];
        return get(m_curRow, col);
    }

    // 移到某行
    public void gotoLine(int row)
    {
        m_curRow = row;
    }

    // 是否完成了
    public bool isFinish()
    {
        return m_curRow >= m_totalRows;
    }

    private FieldCsv get(int row, int col)
    {
        if (m_contents.ContainsKey(row))
        {
            List<FieldCsv> list = m_contents[row];
            return list[col];
        }
        return null;
    }

    // 初始化表头
    private bool init(string line)
    {
        m_nameToCol.Clear();
        string[] strs = parseLine(line);
        for (int i = 0; i < strs.Length; i++)
        {
            if (m_nameToCol.ContainsKey(strs[i]))
            {
                LOG.Info("读取表格错误，包含了相同的列名!");
                return false;
            }
            m_nameToCol[strs[i]] = i;
        }
        return true;
    }

    // 增加一行数据
    private void addLine(string line, int line_num)
    {
        // line为空白时，忽略本行
        if (line == "")
            return;
        string[] strs = parseLine(line);
        List<FieldCsv> arr = null;
        if (m_contents.ContainsKey(line_num))
        {
            arr = m_contents[line_num];
        }
        else
        {
            arr = new List<FieldCsv>();
            m_contents[line_num] = arr;
        }
        for (int i = 0; i < strs.Length; i++)
        {
            arr.Add(new FieldCsv(strs[i]));
        }

        int count = m_nameToCol.Count;
        count -= strs.Length;
        // 由于wps导出的csv与excel导出的有点不一样
        // 当从右边某个格子，开始不填写任何内容时，写入的逗号数与标题列的个数不符，
        // 这里补充下，补充成空串
        for (int i = 0; i < count; i++)
        {
            arr.Add(new FieldCsv(""));
        }
    }

    // 解析其中的一行数据
    private string[] parseLine(string line)
    {
        int pos = line.IndexOf('"');
        if (pos == -1)  // 里面不含双引号，是正常情况
        {
            char[] sp = { ',' };
            string[] strs = line.Split(sp);
            return strs;
        }
            
        StringBuilder sb = new StringBuilder(64);
        // 存储解析出的各个串
        List<string> res = new List<string>();
        int state = 0;
        int i = 0;
        while (i < line.Length)
        {
            switch (state)
            {
            case 0:
                {
                    if (line[i] == '"') // 遇到了双引号转状态
                    {
                        state = 1;
                    }
                    else if (line[i] == ',') // 遇到逗号，说明一个单词识别出来了
                    {
                        res.Add(sb.ToString());
                        sb.Remove(0, sb.Length);
                    }
                    else // 其他情况
                    {
                        sb.Append(line[i]);
                    }
                    i++;
                }
                break;
            case 1:
                {
                    // 取一个双引号
                    if (i + 1 < line.Length && line[i] == '"' && line[i + 1] == '"')
                    {
                        sb.Append('"');
                        i += 2;
                    }
                    // 一个单词结束了
                    else if (line[i] == '"' && (i + 1 >= line.Length || line[i + 1] == ','))
                    {
                        res.Add(sb.ToString());
                        sb.Remove(0, sb.Length);
                        i += 2;
                        state = 0;
                    }
                    else
                    {
                        sb.Append(line[i]);
                        i++;
                    }
                }
                break;
            }
        }
        if (sb.Length != 0)
        {
            res.Add(sb.ToString());
        }
        return res.ToArray();
    }

    // 是否空行，只含空格，或只含,
    private bool isEmptyLine(string line)
    {
        for (int i = 0; i < line.Length; i++)
        {
            if (line[i] != ' ' && line[i] != ',')
                return false;
        }
        return true;
    }

    // 读取一行数据，csv的一行数据，以,结束
    // end_flag是结束标记
    private string readLine(StreamReader sr, char end_flag = '$' )
    {
        string res = sr.ReadLine();
        // 不是空格时，表示使用结束标记
        if (end_flag != ' ' && res != null)
        {
            while (res[res.Length - 1] != end_flag)
            {
                res += sr.ReadLine();
            }
        }

        return res;
    }
}

public class XmlReader : ITable
{
    private List<List<FieldCsv>> m_content = new List<List<FieldCsv>>();
    private string m_fileName = "";
    // 存储列名到列索引的对应关系
    private Dictionary<string, int> m_nameToCol = new Dictionary<string, int>();
    // 总行数
    private int m_totalRows;
    // 当前行
    private int m_curRow;

    public bool load(string file)
    {
        if (!File.Exists(file))
        {
            LOG.Info("文件[{0}]不存在!", file);
            return false;
        }
        m_fileName = file;
        XmlDocument doc = new XmlDocument();
        doc.Load(file);
        m_content.Clear();
        m_nameToCol.Clear();
        m_totalRows = 0;
        parse(doc);
        return true;
    }

    public FieldCsv fetch(int col)
    {
        return get(m_curRow, col);
    }

    public FieldCsv fetch(string col_name)
    {
        if (!m_nameToCol.ContainsKey(col_name))
        {
            LOG.Info("表格中列名[{0}]不存在!", col_name);
            return null;
        }
        int col = m_nameToCol[col_name];
        return get(m_curRow, col);
    }

    // 移到某行
    public void gotoLine(int row)
    {
        m_curRow = row;
    }

    // 是否完成了
    public bool isFinish()
    {
        return m_curRow >= m_totalRows;
    }

    private FieldCsv get(int row, int col)
    {
        if (row < 0 || row >= m_content.Count)
        {
            LOG.Info("行数超出范围[{0}]!", row);
            return null;
        }
        List<FieldCsv> list = m_content[row];
        if (col < 0 || col >= list.Count)
        {
            LOG.Info("列数超出范围[{0}]!", col);
            return null;
        }
        return list[col];
    }

    private void parse(XmlDocument doc)
    {
        XmlNode node = doc.SelectSingleNode("/configuration");
        for (node = node.FirstChild; node != null; node = node.NextSibling)
        {
            if (node is XmlComment)  // 忽略注释
                continue;

            List<FieldCsv> res = parseOneItem(node);
            if (res.Count > 0)
            {
                m_content.Add(res);
                m_totalRows++;
            }
        }
    }

    private List<FieldCsv> parseOneItem(XmlNode node)
    {
        List<FieldCsv> result = new List<FieldCsv>();

        int i = 0;
        for (XmlNode tmp = node.FirstChild; tmp != null; tmp = tmp.NextSibling)
        {
            if (tmp is XmlComment)  // 忽略注释
                continue;

            if (tmp.FirstChild != null)
            {
                result.Add(new FieldCsv(tmp.FirstChild.Value));
            }
            else
            {
                result.Add(new FieldCsv(""));
            }

            if (!m_nameToCol.ContainsKey(tmp.Name))
            {
                m_nameToCol[tmp.Name] = i;
                i++;
            }
        }
        return result;
    }
}
