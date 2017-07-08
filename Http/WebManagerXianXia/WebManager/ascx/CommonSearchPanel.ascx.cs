using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.ascx
{
    public partial class CommonSearchPanel : System.Web.UI.UserControl
    {
        protected static string[] s_player = { "所有玩家", "直属玩家" };
        protected static string[] s_other = { "所有下级", "直属下级" };

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        public void fillCondtion(ParamMemberInfo param, GMUser user)
        {
            param.m_acc = m_acc.Text;
            param.m_time = __gmAccountCascadeStaticTime.Text;
            param.m_searchDepth = Convert.ToInt32(m_way.SelectedValue);
            param.m_creator = m_creator.Text;
        }

        public void setLevelName(bool player)
        {
            string[] arr;
            if (player)
            {
                arr = s_player;
            }
            else
            {
                arr = s_other;
            }
            for (int i = 0; i < arr.Length; i++)
            {
                m_way.Items.Add(new ListItem(arr[i], i.ToString()));
            }
            m_way.SelectedIndex = 0;
        }

        public RadioButtonList getWay()
        {
            return m_way;
        }

        public global::System.Web.UI.HtmlControls.HtmlTableRow getViewLevel()
        {
            return tdViewLevel;
        }
    }
}