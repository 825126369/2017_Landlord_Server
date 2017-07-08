using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Configuration;

namespace WebManager
{
    public partial class SiteMaster : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
           GMUser user = (GMUser)Session["user"];
           if (user != null)
           {
               tdLoginAcc.InnerText = user.m_user;
               tdAccLevel.InnerText = StrName.s_accountType[user.m_accType];
               tdRemainMoney.InnerText = (user.m_accType == AccType.ACC_SUPER_ADMIN ||
                   user.m_accType == AccType.ACC_SUPER_ADMIN_SUB) ? "0" : ItemHelp.toStrByComma(ItemHelp.showMoneyValue(user.m_money));
               tdAgentRatio.InnerText = (user.m_agentRatio * 100).ToString() + "%";
               tdWashRatio.InnerText = (user.m_washRatio * 100).ToString() + "%";

               accType.Value = user.m_accType.ToString();
               if (!RightMap.hasRight(user.m_accType, RIGHT.DATA_STAT, user.m_right))
               {
                   statParam.Visible = false;
               }
               if (!user.isAdmin())
               {
                   if (!RightMap.hasRight(RIGHT.APPROVE_API, user.m_right))
                   {
                      // m_liApiApprove.Visible = false;
                   }
               }
               if (!user.isAPIAcc())
               {
                   m_liApiLogViewer.Visible = false;
                   m_liApiScore.Visible = false;
                   m_liApiLimit.Visible = false;
               }

               setOnlineNum(user);
           }
        }
 
        // 动态生成菜单
        private void genMenu()
        {
            Dictionary<string, DbServerInfo> db = ResMgr.getInstance().getAllDb();
            foreach(DbServerInfo info in db.Values)
            {
                MenuItem item = new MenuItem();
                item.Text = info.m_serverName;
                item.NavigateUrl = "/appaspx/SelectMachine.aspx?db=" + info.m_serverIp;
               // NavigationMenu.Items.Add(item);
            }
        }

        private void genShortCutMenu()
        {
            /*try
            {
                // 生成快捷菜单
                List<MainMenu> menuList = MenuMgr.getInstance().menuList;
                foreach (var m in menuList)
                {
                    TableCell td = new TableCell();
                    Label L = new Label();
                    L.Text = m.m_name;
                    L.CssClass = "cShortCutTitle";
                    td.Controls.Add(L);

                    td.Controls.Add(m.m_dst);
                   // shortCutMenu.Cells.Add(td);
                }
            }
            catch (System.Exception ex)
            {
                LOGW.Info(ex.ToString());
            }*/
        }

        void setOnlineNum(GMUser user)
        {
            OpRes res = user.doQuery(null, QueryType.queryTypeOnlinePlayerCount);
            int c = (int)user.getQueryResult(QueryType.queryTypeOnlinePlayerCount);
            curOnline.InnerText = c.ToString();
        }
    }
}
