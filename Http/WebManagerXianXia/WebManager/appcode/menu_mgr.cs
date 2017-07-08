using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Web.UI;
using System.Web.UI.WebControls;

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
        doc.Load(HttpContext.Current.Server.MapPath("\\data\\Navigation.xml"));
        XmlNode node = doc.SelectSingleNode("Root/mainMenu");
        if (node != null)
        {
            for (; node != null; node = node.NextSibling)
            {
                MainMenu top = new MainMenu();
                top.m_name = node.Attributes["gameName"].Value;
                foreach (XmlNode c in node.ChildNodes)
                {
                    top.add(c.Attributes["url"].Value, c.Attributes["text"].Value);
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

    public List<SubMenu> m_subList = new List<SubMenu>();

    public BulletedList m_dst = new BulletedList();

    public MainMenu()
    {
        m_dst.DisplayMode = BulletedListDisplayMode.HyperLink;
    }

    public void add(string url, string text)
    {
        SubMenu s = new SubMenu();
        s.m_url = url;
        s.m_text = text;
        m_subList.Add(s);

        ListItem item = new ListItem();
        item.Text = text;
        item.Value = url;
        m_dst.Items.Add(item);
    }
}



