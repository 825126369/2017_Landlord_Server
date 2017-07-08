using System;
using System.Web.SessionState;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Xml;
using System.IO;

public class ChannelSys : SysBase
{
    public ChannelSys()
    {
        m_sysType = SysType.sysTypeChannel;
    }

    public static ChannelSys getInstance()
    {
        return SysMgr.getGlobalSys<ChannelSys>(SysType.sysTypeChannel);
    }

    public override void initSys() 
    {
       
    }

    public bool addChannel(string channelNo, string channelName)
    {
        if (channelNo == "" || channelName == "")
            return false;

        ChannelInfo info = Channel.getInstance().getChannel(channelNo);
        if (info != null)
            return false;

        bool res = true;
        HttpContext.Current.Application.Lock();
        try
        {
            appendChannelToXml(channelNo, channelName);
        }
        catch (System.Exception ex)
        {
            res = false;
        }
        finally
        {
            HttpContext.Current.Application.UnLock();
        }

        return res;
    }

    public bool delChannel(string channelNo)
    {
        ChannelInfo info = Channel.getInstance().getChannel(channelNo);
        if (info == null)
            return false;

        bool res = true;
        HttpContext.Current.Application.Lock();
        try
        {
            delChannelToXml(channelNo);
        }
        catch (System.Exception ex)
        {
            res = false;
        }
        finally
        {
            HttpContext.Current.Application.UnLock();
        }

        return res;
    }

    private bool appendChannelToXml(string channelNo, string channelName)
    {
        XmlDocument doc = new XmlDocument();
        string fileName = HttpContext.Current.Server.MapPath("~/data/channel.xml");
        doc.Load(fileName);
        XmlNode root = doc.SelectSingleNode("/Root");
        XmlElement newElem = (XmlElement)doc.CreateElement("add");
        root.AppendChild(newElem);

        XmlAttribute attr = doc.CreateAttribute("channelNo");
        attr.Value = channelNo;
        newElem.Attributes.Append(attr);

        attr = doc.CreateAttribute("channelName");
        attr.Value = channelName;
        newElem.Attributes.Append(attr);

        doc.Save(fileName);

        Channel.getInstance().addChannel(channelNo, channelName);
        return true;
    }

    private bool delChannelToXml(string channelNo)
    {
        bool res = false;

        XmlDocument doc = new XmlDocument();
        string fileName = HttpContext.Current.Server.MapPath("~/data/channel.xml");
        doc.Load(fileName);
        XmlNode root = doc.SelectSingleNode("/Root");
        XmlNode node = root.FirstChild;
        for (; node != null; node = node.NextSibling)
        {
            if (node.Attributes["channelNo"].Value == channelNo)
            {
                res = true;
                root.RemoveChild(node);
                break;
            }
        }

        if (res)
        {
            Channel.getInstance().delChannel(channelNo);
            doc.Save(fileName);
        }
        return true;
    }
}



















