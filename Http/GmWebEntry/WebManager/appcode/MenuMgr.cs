using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

public class MenuMgr
{
    private static MenuMgr s_obj = null;

    private List<MainMenu> m_menuList = new List<MainMenu>();

    public static MenuMgr getInstance()
    {
        if (s_obj == null)
        {
            s_obj = new MenuMgr();
            s_obj.init();
        }

        return s_obj;
    }

    public List<MainMenu> menuList
    {
        get { return m_menuList; }
    }

    private void init()
    {
        XmlDocument doc = new XmlDocument();
        doc.Load(HttpRuntime.BinDirectory + "..\\" + "data\\Navigation.xml");

        XmlNode node = doc.SelectSingleNode("Root/mainMenu");
        if (node != null)
        {
            for (; node != null; node = node.NextSibling)
            {
                if (node is XmlComment)
                    continue;

                MainMenu top = new MainMenu();
                top.m_name = node.Attributes["gameName"].Value;
                top.m_icon = node.Attributes["icon"].Value;

                foreach (XmlNode c in node.ChildNodes)
                {
                    SubMenu s = new SubMenu();
                    s.m_url = c.Attributes["url"].Value;
                    s.m_text = c.Attributes["text"].Value;
                    top.m_subList.Add(s);
                }

                m_menuList.Add(top);
            }
        }
    }
}

public class SubMenu
{
    public string m_url = "";
    public string m_text = "";
}

public class MainMenu
{
    public string m_name = "";
    public string m_icon = "";

    public List<SubMenu> m_subList = new List<SubMenu>();
}



