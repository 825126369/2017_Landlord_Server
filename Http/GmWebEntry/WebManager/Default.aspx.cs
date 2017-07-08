using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            List<MainMenu> mlist = MenuMgr.getInstance().menuList;

            foreach(var item in mlist)
            {
                genMenu(item);
            }
        }

        protected void genMenu(MainMenu item)
        {
            Panel game = new Panel();
            game.CssClass = "game";
            divMain.Controls.Add(game);

            Panel tp = new Panel();
            tp.CssClass = "gameTitle";
            Image img = new Image();
            img.Width = 72;
            img.Height = 72;
            img.ImageUrl = string.Format("/img/{0}", item.m_icon);
            tp.Controls.Add(img);

            Label title = new Label();
            title.Text = item.m_name;
            tp.Controls.Add(title);
            game.Controls.Add(tp);

            foreach (var m in item.m_subList)
            {
               // Panel sub = new Panel();
               // sub.CssClass = "subMenu";
                HyperLink lnk = new HyperLink();
                lnk.Text = m.m_text;
                lnk.NavigateUrl = m.m_url;
                lnk.CssClass = "subMenu";
                lnk.Target = "_blank";
               // sub.Controls.Add(lnk);
                game.Controls.Add(lnk);
            }

            Panel clr = new Panel();
            clr.Style.Add("clear", "both");
            game.Controls.Add(clr);
        }
    }
}
