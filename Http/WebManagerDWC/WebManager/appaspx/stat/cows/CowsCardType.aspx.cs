using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.stat.cows
{
    public partial class CowsCardType : RefreshPageBase
    {
        private static string[] s_head = new string[] { "增加时间", "庄家牌型", 
            "闲家牌型-东", "闲家牌型-南", "闲家牌型-西", "闲家牌型-北", "选择" };

        private string[] m_content = new string[s_head.Length];
        private string m_sel = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck("stat", Session, Response);

            if (!IsPostBack)
            {
                XmlConfig cfg = ResMgr.getInstance().getRes("cows_card.xml");
                int count = cfg.getCount() - 1;
                string txt = "";
                string val="";
                for (int i = -1; i < count; i++)
                {
                    txt = cfg.getString(i.ToString(), "");
                    val = i.ToString();
                    m_banker.Items.Add(new ListItem(txt,val));

                    m_other1.Items.Add(new ListItem(txt, val));

                    m_other2.Items.Add(new ListItem(txt, val));

                    m_other3.Items.Add(new ListItem(txt, val));

                    m_other4.Items.Add(new ListItem(txt, val));
                }

                GMUser user = (GMUser)Session["user"];
                genTable(m_allCards, user);
            }
            else
            {
                m_sel = Request["sel"];

                if (IsRefreshed)
                {
                    GMUser user = (GMUser)Session["user"];
                    genTable(m_allCards, user);
                }
            }
        }

        protected void onAddCardType(object sender, EventArgs e)
        {
            if (IsRefreshed)
                return;

            GMUser user = (GMUser)Session["user"];

            ParamCowsCard param = new ParamCowsCard();
            ParamAddCowsCard p = new ParamAddCowsCard();
            p.m_bankerType = Convert.ToInt32(m_banker.SelectedValue);
            p.m_other1Type = Convert.ToInt32(m_other1.SelectedValue);
            p.m_other2Type = Convert.ToInt32(m_other2.SelectedValue);
            p.m_other3Type = Convert.ToInt32(m_other3.SelectedValue);
            p.m_other4Type = Convert.ToInt32(m_other4.SelectedValue);
            param.m_data = p;

            DyOpMgr mgr = user.getSys<DyOpMgr>(SysType.sysTypeDyOp);
            OpRes res = mgr.doDyop(param, DyOpType.opTypeSetCowsCard, user);
            m_res.InnerHtml = OpResMgr.getInstance().getResultString(res);

            genTable(m_allCards, user);
        }

        protected void onDelCard(object sender, EventArgs e)
        {
            if (IsRefreshed)
                return;

            GMUser user = (GMUser)Session["user"];

            if (string.IsNullOrEmpty(m_sel))
            {
                m_res.InnerHtml = OpResMgr.getInstance().getResultString(OpRes.op_res_failed);
                return;
            }

            DyOpMgr mgr = user.getSys<DyOpMgr>(SysType.sysTypeDyOp);

            ParamCowsCard param = new ParamCowsCard();
            param.m_op = 1;

            string []arr = Tool.split(m_sel, ',', StringSplitOptions.RemoveEmptyEntries);
            for(int i = 0; i < arr.Length; i++)
            {
                param.m_data = arr[i];
                mgr.doDyop(param, DyOpType.opTypeSetCowsCard, user);
            }

            genTable(m_allCards, user);
        }

        protected void genTable(Table table, GMUser user)
        {
            TableRow tr = new TableRow();
            table.Rows.Add(tr);

            int i = 0;
            for (; i < s_head.Length; i++)
            {
                TableCell td = new TableCell();
                tr.Cells.Add(td);
                td.Text = s_head[i];
            }

            OpRes res = user.doQuery(null, QueryType.queryTypeCowsCardsType);
            List<ResultCowsCard> qresult
                = (List<ResultCowsCard>)user.getQueryResult(QueryType.queryTypeCowsCardsType);

            for (i = 0; i < qresult.Count; i++)
            {
                ResultCowsCard r = qresult[i];
                m_content[0] = r.m_time;
                m_content[1] = ItemHelp.getCowsCardTypeName(r.m_bankerType);
                m_content[2] = ItemHelp.getCowsCardTypeName(r.m_other1Type);
                m_content[3] = ItemHelp.getCowsCardTypeName(r.m_other2Type);
                m_content[4] = ItemHelp.getCowsCardTypeName(r.m_other3Type);
                m_content[5] = ItemHelp.getCowsCardTypeName(r.m_other4Type);
                m_content[6] = Tool.getCheckBoxHtml("sel", r.m_key, false);

                tr = new TableRow();
                table.Rows.Add(tr);
                for (int j = 0; j < s_head.Length; j++)
                {
                    TableCell td = new TableCell();
                    tr.Cells.Add(td);
                    td.Text = m_content[j];
                }
            }
        }
    }
}