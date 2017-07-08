using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.service
{
    public partial class ServiceInfo : System.Web.UI.Page
    {
        private static string[] s_head = new string[] { "平台(cha)", "平台", "客服信息", "删除标志" };
        private string[] m_content = new string[s_head.Length];
        private string m_delList = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck(RightDef.SVR_ADD_SERVICE_INFO, Session, Response);
            if (!IsPostBack)
            {
                genTable(m_curHelp, (GMUser)Session["user"]);

                Dictionary<int, PlatformInfo> data = ResMgr.getInstance().getAllPlatId();
                var platList = from d in data 
                               orderby d.Key
                               select d;
                foreach(var plat in platList)
                {
                    m_platform.Items.Add(plat.Value.m_chaName);
                }
            }
            else
            {
                m_delList = Request["sel"];
                if (m_delList == null)
                {
                    m_delList = "";
                }
            }
        }

        protected void onCommit(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];
            ParamServiceInfo param = new ParamServiceInfo();
            param.m_key = ResMgr.getInstance().getPlatformName(m_platform.SelectedIndex, true);
            param.m_desc = m_desc.Text;

            DyOpMgr mgr = user.getSys<DyOpMgr>(SysType.sysTypeDyOp);
            OpRes res = mgr.doDyop(param, DyOpType.opTypeServiceInfo, user);
            m_res.InnerHtml = OpResMgr.getInstance().getResultString(res);
            if (res == OpRes.opres_success)
            {
                genTable(m_curHelp, user);
            }
        }

        protected void onDelInfo(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];
            if (m_delList != "")
            {
                ParamServiceInfo param = new ParamServiceInfo();
                param.m_key = m_delList;
                param.m_isAdd = false;

                DyOpMgr mgr = user.getSys<DyOpMgr>(SysType.sysTypeDyOp);
                OpRes res = mgr.doDyop(param, DyOpType.opTypeServiceInfo, user);
                genTable(m_curHelp, user);
            }
        }

        private void genTable(Table table, GMUser user)
        {
            table.GridLines = GridLines.Both;
            TableRow tr = new TableRow();
            table.Rows.Add(tr);
            TableCell td = null;

            int i = 0, j = 0;
            // 表头
            for (i = 0; i < s_head.Length; i++)
            {
                td = new TableCell();
                tr.Cells.Add(td);
                td.Text = s_head[i];
            }

            QueryMgr qmgr = user.getSys<QueryMgr>(SysType.sysTypeQuery);
            qmgr.doQuery(null, QueryType.queryTypeServiceInfo, user);
            List<ServiceInfoItem> sInfo = (List<ServiceInfoItem>)qmgr.getQueryResult(QueryType.queryTypeServiceInfo);
            for (i = 0; i < sInfo.Count; i++)
            {
                tr = new TableRow();
                table.Rows.Add(tr);

                m_content[0] = sInfo[i].m_platChaName;
                m_content[1] = sInfo[i].m_platEngName;
                m_content[2] = sInfo[i].m_info;
                m_content[3] = Tool.getCheckBoxHtml("sel", m_content[1], false);

                for (j = 0; j < s_head.Length; j++)
                {
                    td = new TableCell();
                    tr.Cells.Add(td);
                    td.Text = m_content[j];
                }
            }
        }
    }
}