using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public class RefreshPageBase : System.Web.UI.Page
{
    private string _strSessionKey;
    private static string _hiddenfieldName="__LastVIEWSTATE_SessionKey";
    private string _strLastViewstate;

    public RefreshPageBase()
    {
        _strSessionKey = System.Guid.NewGuid().ToString();
        _strLastViewstate = string.Empty;
    }

    public bool IsRefreshed
    {
        get
        {
            string str1 = GetSessinContent();
            _strLastViewstate = str1;
            string str2 = this.Session[GetSessinKey()] as string;
            bool flag1 = (str1 != null) && (str2 != null) && (str1 == str2);
            return flag1;
        }
    }

    protected override void Render(System.Web.UI.HtmlTextWriter writer)
    {
        string str = GetSessinKey();
        this.Session[str] = _strLastViewstate;
        ClientScript.RegisterHiddenField(_hiddenfieldName, _strSessionKey);
        base.Render(writer);
    }

    private string GetSessinKey()
    {
        string str = this.Request.Form[_hiddenfieldName];
        return (str == null) ? _strSessionKey : str;
    }

    private string GetSessinContent()
    {
        string str = this.Request.Form["__VIEWSTATE"];
        if (str == null)
        {
            return null;
        }
        return Tool.getMD5Hash(str);
    }
} 
 


