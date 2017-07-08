using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.appaspx.operation
{
    public partial class OperationJPushApp : System.Web.UI.Page
    {
        private static string[] s_head = new string[] { "平台", "应用名称", "AppKey", "APISecret", "选择" };
        private string[] m_content = new string[s_head.Length];
        private string m_platList = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck("operation", Session, Response);
            if (IsPostBack)
            {
                m_platList = Request["sel"];
                if (m_platList == null)
                {
                    m_platList = "";
                }
            }
            else
            {
                m_platform.Items.Add("default");
                for (int i = (int)PaymentType.e_pt_none + 1; i < (int)PaymentType.e_pt_max; i++)
                {
                    PlatformInfo data = ResMgr.getInstance().getPlatformInfo(i);
                    if (data == null)
                    {
                        m_platform.Items.Add("###");
                    }
                    else
                    {
                        m_platform.Items.Add(data.m_chaName);
                    }
                }

                GMUser user = (GMUser)Session["user"];
                DyOpMgr mgr = user.getSys<DyOpMgr>(SysType.sysTypeDyOp);
                genTable(m_result, user, mgr);
            }
        }

        protected void onAddApp(object sender, EventArgs e)
        {
            ParamJPushAddApp p = new ParamJPushAddApp();
            p.m_isAdd = true;
            PlatformInfo data = ResMgr.getInstance().getPlatformInfo(m_platform.SelectedIndex);
            if (data != null)
            {
                p.m_platName = data.m_engName;
            }
            else
            {
                p.m_platName = "default";
            }
            p.m_appKey = m_appKey.Text;
            p.m_apiSecret = m_apiSecret.Text;
            p.m_appName = m_appName.Text;
            GMUser user = (GMUser)Session["user"];
            DyOpMgr mgr = user.getSys<DyOpMgr>(SysType.sysTypeDyOp);
            OpRes res = mgr.doDyop(p, DyOpType.opTypePushApp, user);
            m_res.InnerHtml = OpResMgr.getInstance().getResultString(res);
            genTable(m_result, user, mgr);
        }

        protected void onDelApp(object sender, EventArgs e)
        {
            GMUser user = (GMUser)Session["user"];
            ParamJPushAddApp p = new ParamJPushAddApp();
            p.m_isAdd = false;
            p.m_platName = m_platList;
            DyOpMgr mgr = user.getSys<DyOpMgr>(SysType.sysTypeDyOp);
            mgr.doDyop(p, DyOpType.opTypePushApp, user);
            genTable(m_result, user, mgr);
        }

        private void genTable(Table table, GMUser user, DyOpMgr mgr)
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

            List<ParamJPushAddApp> resultList = new List<ParamJPushAddApp>();
            DyOpJPushAddApp dyip = (DyOpJPushAddApp)mgr.getDyOp(DyOpType.opTypePushApp);
            dyip.getAppList(user, resultList);
            for (i = 0; i < resultList.Count; i++)
            {
                tr = new TableRow();
                table.Rows.Add(tr);

                m_content[0] = resultList[i].m_platName;
                m_content[1] = resultList[i].m_appName;
                m_content[2] = resultList[i].m_appKey;
                m_content[3] = resultList[i].m_apiSecret;
                m_content[4] = Tool.getCheckBoxHtml("sel", m_content[0], false);

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